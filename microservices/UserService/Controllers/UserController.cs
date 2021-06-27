using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspCoreTools;
using AspCoreTools.Email;
using Microsoft.AspNetCore.Mvc;
using DatabaseLayer;
using Skills;
using UserService.Contracts;
using AspCoreTools.Jwt;
using MongoDB.Bson;
using Crypto;
using MongoDB.Driver;

namespace UserService.Controllers
{
    using DatabaseLayer.Models;
    using User = DatabaseLayer.Models.User;


    [Route("api/User")]
    [MultiArgJsonBody]
    public class UserController : Controller, IUserService
    {
        // TODO: Revise our security standards
        private bool CheckPassword(String password)
        {
            return String.IsNullOrEmpty(password) || password.Length > 4;
        }

        private async Task AddGroup(User user, Group group)
        {
            user.AddGroup(group);
            group.AddMember(user);

            await user.Save();
            await group.Save();
        }

        [HttpGet("{targetUser}")]
        public async Task<User> GetUser([UidFromToken] String userId, [FromRoute] String targetUser)
        {
            // For now, allow any user to view any other user info
            if (String.IsNullOrEmpty(userId))
                return HttpContext.SendForbidden<User>("Permission denied, user must be logged in");

            var user = await DatabaseLayer.Models.User.Load(targetUser);
            if (user is null)
                return HttpContext.SendBadRequest<User>("User not found");
            user.Password = null;
            
            return user;
        }

        [HttpPost("CreateUser")]
        public async Task<User> CreateUser(String firstName, String lastName, String email, String password)
        {
            if (String.IsNullOrEmpty(firstName) || String.IsNullOrEmpty(lastName))
                return HttpContext.SendBadRequest<User>("Empty first or last name");

            if (String.IsNullOrEmpty(email) || !email.Contains("@"))
                return HttpContext.SendBadRequest<User>("Invalid email");

            if (!CheckPassword(password))
                return HttpContext.SendBadRequest<User>("Password is way below our standards");

            if (MongoDBLayer.GetCollectionForModel<User>().AsQueryable().Any(usr => usr.Email == email))
                return HttpContext.SendBadRequest<User>("User with this email already exists");

            var user = new User(firstName, lastName, email, PasswordUtils.HashPassword(password));
            await user.Save();
            user.Password = null;
            return user;
        }

        [HttpDelete("{userId}")]
        public async Task<bool> DeleteUser([FromRoute] String userId, String targetUser)
        {
            if (!HttpContext.User.IsInRole(GlobalRole.Admin.ToString()))
                return HttpContext.SendForbidden("Permission denied. Only admins can delete users");

            await MongoDBLayer.DeleteOne<User>(targetUser);
            return HttpContext.SendOk();
        }

        [HttpPost("ChangePersonalInfo")]
        public async Task<bool> ChangePersonalInfo([UidFromToken] String userId, String firstName, String lastName)
        {
            if (String.IsNullOrEmpty(userId))
                return HttpContext.SendForbidden("Permission denied. User must be logged in");

            //This can be performed in single query, but this way is safer
            var user = await DatabaseLayer.Models.User.Load(userId);

            //TODO: Add appropriate exception type
            if (user is null)
                return HttpContext.SendBadRequest("User not found");


            if (!String.IsNullOrEmpty(firstName))
                user.FirstName = firstName.Trim();
            if (!String.IsNullOrEmpty(lastName))
                user.LastName = lastName.Trim();

            await user.Save();
            return HttpContext.SendOk();
        }

        [HttpPost("ChangeEmail")]
        public async Task<bool> ChangeEmail([UidFromToken] string userId, String email)
        {
            if (String.IsNullOrEmpty(userId))
                return HttpContext.SendForbidden("Permission denied. User must be logged in");

            var user = await DatabaseLayer.Models.User.Load(userId);

            //TODO: Add appropriate exception type
            if (user is null)
                return HttpContext.SendBadRequest("User not found");

            // TODO: check if it's actually a valid email
            if (String.IsNullOrEmpty(email) || !email.Contains("@"))
                return HttpContext.SendBadRequest("Invalid email");

            user.Email = email;
            await user.Save();
            return HttpContext.SendOk();
        }

        [HttpPost("ChangePassword")]
        public async Task<bool> ChangePassword([UidFromToken] string userId, String oldPassword, String newPassword)
        {
            if (String.IsNullOrEmpty(userId))
                return HttpContext.SendForbidden("Permission denied. User must be logged in");

            // TODO: Check whether new password is valid first since it's cheap
            var user = await DatabaseLayer.Models.User.Load(userId);

            //TODO: Add appropriate exception type
            if (user is null)
                return HttpContext.SendBadRequest("User not found");

            if (!PasswordUtils.ComparePassword(oldPassword, user.Password))
                return HttpContext.SendBadRequest("Old password is incorrect");


            if (!CheckPassword(newPassword))
                return HttpContext.SendBadRequest("New password does not match security criteria");


            user.Password = PasswordUtils.HashPassword(newPassword);
            await user.Save();

            return HttpContext.SendOk();
        }

        [HttpPost("AddGlobalRole")]
        public async Task<bool> AddGlobalRole([UidFromToken] String userId, String targetUser, GlobalRole role)
        {
            if (String.IsNullOrEmpty(userId))
                return HttpContext.SendForbidden("Permission denied. User must be logged in");

            if (!HttpContext.User.IsInRole(GlobalRole.Admin.ToString()))
                return HttpContext.SendForbidden("Permission denied. Only Admins can modify global roles");

            var user = await DatabaseLayer.Models.User.Load(targetUser);

            if (user is null)
                return HttpContext.SendBadRequest("User not found");


            user.GlobalRoles.Add(role);
            await user.Save();

            return HttpContext.SendOk();
        }

        [HttpPost("RemoveGlobalRole")]
        public async Task<bool> RemoveGlobalRole([UidFromToken] String userId, String targetUserId, GlobalRole role)
        {
            if (String.IsNullOrEmpty(userId))
                return HttpContext.SendForbidden("Permission denied. User must be logged in");

            if (!HttpContext.User.IsInRole(GlobalRole.Admin.ToString()))
                return HttpContext.SendForbidden("Permission denied. Only Admins can modify global roles");


            var user = await DatabaseLayer.Models.User.Load(targetUserId);

            if (user is null)
                return HttpContext.SendBadRequest("User not found");


            user.GlobalRoles.Remove(role);
            await user.Save();

            return HttpContext.SendOk();
        }

        [HttpGet("{userId}/GetGroups")]
        public async Task<List<Group>> GetGroups([UidFromToken] String userId)
        {
            if (String.IsNullOrEmpty(userId))
                return HttpContext.SendForbidden<List<Group>>("Permission denied. User must be logged in");

            var user = await DatabaseLayer.Models.User.Load(userId);

            if (user is null)
                return HttpContext.SendBadRequest<List<Group>>("User not found");

            // TODO: Expose API for getting actual IDs
            var result = await MongoDBLayer.FindById<Group>(user.GroupIds);
            return result.ToList();
        }

        [HttpPost("AddCourse")]
        public async Task<bool> JoinCourse([UidFromToken] String userId, String courseId)
        {
            if (String.IsNullOrEmpty(userId))
                return HttpContext.SendForbidden("Permission denied. User must be logged in");

            var user = await DatabaseLayer.Models.User.Load(userId);

            if (user is null)
                return HttpContext.SendBadRequest("User not found");


            var course = await Course.Load(courseId);

            if (course is null)
                return HttpContext.SendBadRequest("Course not found");


            if (course.EnrollmentRecords.ContainsKey(user))
                return HttpContext.SendBadRequest("User is already enrolled in this course");

            user.AddCourse(course);
            course.EnrollmentRecords[user] = new List<NumRangeSkill>();

            // Off we go
            await user.Save();
            await course.Save();

            return HttpContext.SendOk();
        }

        [HttpPost("RemoveCourse")]
        public async Task<bool> LeaveCourse([UidFromToken] String userId, String courseId)
        {
            if (String.IsNullOrEmpty(userId))
                return HttpContext.SendForbidden("Permission denied. User must be logged in");

            var user = await DatabaseLayer.Models.User.Load(userId);

            if (user is null)
                return HttpContext.SendBadRequest("User not found");

            ObjectId.TryParse(courseId, out var courseObjectId);
            if (!user.CourseIds.Contains(courseObjectId))
                return HttpContext.SendBadRequest("User is not enrolled in the specified course");

            var course = await Course.Load(courseId);

            if (course is null)
                return HttpContext.SendBadRequest("Course not found");


            user.RemoveCourse(course);
            course.EnrollmentRecords.Remove(user);

            await user.Save();
            await course.Save();

            return HttpContext.SendOk();
        }

        [HttpGet("GetCourses")]
        public async Task<List<Course>> GetCourses([UidFromToken] String userId)
        {
            if (String.IsNullOrEmpty(userId))
                return HttpContext.SendForbidden<List<Course>>("Permission denied. User must be logged in");

            var user = await DatabaseLayer.Models.User.Load(userId);

            if (user is null)
                return HttpContext.SendBadRequest<List<Course>>("User not found");

            var courses = (await MongoDBLayer.FindById<Course>(user.CourseIds)).ToList();
            
            return courses;
        }

        [HttpPost("AddGroupInvitation/{userId}/{groupId}")]
        public async Task<bool> AddGroupInvitation([FromRoute] String userId,[FromRoute] String groupId)
        {
            if (String.IsNullOrEmpty(userId))
                return HttpContext.SendForbidden("Permission denied. User must be logged in");

            var user = await DatabaseLayer.Models.User.Load(userId);

            if (user is null)
                return HttpContext.SendBadRequest("User not found");


            var group = await Group.Load(groupId);

            if (group is null)
                return HttpContext.SendBadRequest("Group not found");

            if (!user.CourseIds.Contains(group.CourseId))
                return HttpContext.SendBadRequest("User is not enrolled in the same course as the group");

            var course = await MongoDBLayer.FindOneById<Course>(group.CourseId);

            if (user.GroupIds.Any(id => course.GroupIds.Contains(id)))
                return HttpContext.SendBadRequest("User is already in a group within the course");


            if (user.Invitations.TryGetValue(group, out InvitationStatus status) && status == InvitationStatus.Invited)
                return HttpContext.SendBadRequest("Cannot add an invitation since one is already in progress");


            user.Invitations[group] = InvitationStatus.Invited;
            group.Invitations[user] = InvitationStatus.Invited;

            // Yolo
            await user.Save();
            await group.Save();

            await EmailClient.SendEmail(
                EmailTemplates.GroupInviteSubject(group.Name),
                EmailTemplates.GroupInviteBody(group.Name, user.FirstName, group.IdStr),
                user.Email
            );

            return HttpContext.SendOk();
        }

        [HttpPost("AcceptGroupInvitation/{groupId}")]
        public async Task<bool> AcceptGroupInvitation([UidFromToken] String userId, String groupId)
        {
            if (String.IsNullOrEmpty(userId))
                return HttpContext.SendForbidden("Permission denied. User must be logged in");

            var user = await DatabaseLayer.Models.User.Load(userId);

            if (user is null)
                return HttpContext.SendBadRequest("User not found");


            var group = await Group.Load(groupId);

            if (group is null)
                return HttpContext.SendBadRequest("Group not found");


            if (!user.Invitations.TryGetValue(group, out InvitationStatus status) && status != InvitationStatus.Invited)
                return HttpContext.SendBadRequest(
                    "Unable to accept invitation since it does not exist or it is not in 'Invited' state");


            user.Invitations[group] = InvitationStatus.Accepted;
            group.Invitations[user] = InvitationStatus.Accepted;

            await AddGroup(user, group);

            return HttpContext.SendOk();
        }

        [HttpPost("RejectGroupInvitation/{groupId}")]
        public async Task<bool> RejectGroupInvitation([UidFromToken] String userId, String groupId)
        {
            if (String.IsNullOrEmpty(userId))
                return HttpContext.SendForbidden("Permission denied. User must be logged in");

            var user = await DatabaseLayer.Models.User.Load(userId);

            if (user is null)
                return HttpContext.SendBadRequest("User not found");


            var group = await Group.Load(groupId);

            if (group is null)
                return HttpContext.SendBadRequest("Group not found");


            if (!user.Invitations.TryGetValue(group, out InvitationStatus status) && status != InvitationStatus.Invited)
                return HttpContext.SendBadRequest(
                    "Unable to accept invitation since it does not exist or it is not in 'Invited' state");


            user.Invitations[group] = InvitationStatus.Rejected;
            group.Invitations[user] = InvitationStatus.Rejected;
            await user.Save();
            await group.Save();

            return HttpContext.SendOk();
        }

        public async Task<bool> LeaveGroup([UidFromToken] String userId, String groupId)
        {
            if (String.IsNullOrEmpty(userId))
                return HttpContext.SendForbidden("Permission denied. User must be logged in");

            var user = await DatabaseLayer.Models.User.Load(userId);

            if (user is null)
                return HttpContext.SendBadRequest("User not found");


            var group = await Group.Load(groupId);

            if (group is null)
                return HttpContext.SendBadRequest("Group not found");

            if (!group.MemberIds.Contains(user.Id))
                return HttpContext.SendBadRequest("User is not present in the group");

            group.RemoveMember(user);
            user.RemoveGroup(group);

            await group.Save();
            await user.Save();

            return HttpContext.SendOk();
        }

        public IEnumerable<User> GetAll()
        {
            return new List<User>();
        }
    }
}
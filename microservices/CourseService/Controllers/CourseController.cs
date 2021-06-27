using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseService.Contracts;
using Microsoft.AspNetCore.Mvc;
using DatabaseLayer.Models;
using AspCoreTools;
using AspCoreTools.Email;
using AspCoreTools.Jwt;
using DatabaseLayer;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using Skills;

namespace CourseService.Controllers
{
    [Route("api/Course")]
    [MultiArgJsonBody]
    public class CourseController : Controller, ICourseService
    {
        private String GetAuthenticatedUserId()
        {
            return HttpContext.User.GetId();
        }

        private bool IsUserOwner(Course course, String userId = null)
        {
            userId = userId ?? GetAuthenticatedUserId();
            return !String.IsNullOrEmpty(userId) && course.OwnerId.Equals(userId);
        }

        private bool IsUserMember(Course course, String userId = null)
        {
            userId = userId ?? GetAuthenticatedUserId();
            return course.EnrollmentRecords.ContainsId(userId);
        }

        private bool IsUserInstructor(Course course, String userId = null)
        {
            userId = userId ?? GetAuthenticatedUserId();
            return ObjectId.TryParse(userId, out var authenticatedUserId) && course.InstructorIds
                       .Contains(authenticatedUserId);
        }

        private bool IsUserLoggedOn()
        {
            return !(GetAuthenticatedUserId() is null);
        }

        private bool IsUserOwnerOrInstructor(Course course, String userId = null)
        {
            return IsUserOwner(course, userId) || IsUserInstructor(course, userId);
        }

        [HttpGet("{courseId}/GetCourse")]
        public async Task<Course> GetCourse([FromRoute] string courseId)
        {
            if (!IsUserLoggedOn())
                return HttpContext.SendForbidden<Course>("Access denied. User must be logged on");

            var course = await Course.Load(courseId);

            if (course is null)
                return HttpContext.SendBadRequest<Course>("Course not found");

            return course;
        }


        [HttpGet("{courseId}/GetCourseGroups")]
        public async Task<List<Group>> GetCourseGroups([FromRoute] string courseId)
        {
            if (!IsUserLoggedOn())
                return HttpContext.SendForbidden<List<Group>>("Access denied. User must be logged on");

            var course = await Course.Load(courseId);

            if (course is null)
                return HttpContext.SendBadRequest<List<Group>>("Course not found");

            var groups = await course.Groups.Value;

            if (groups == null)
                return HttpContext.SendBadRequest<List<Group>>("Course doesnt have groups");

            return groups.ToList();
        }

        [HttpGet("{courseId}/GetUsersGroup/{userId}")]
        public async Task<Group> GetUsersGroup([FromRoute] string courseId, [FromRoute] string userId)
        {
            if (!IsUserLoggedOn())
                return HttpContext.SendForbidden<Group>("Access denied. User must be logged on");

            var course = await Course.Load(courseId);

            if (course is null)
                return HttpContext.SendBadRequest<Group>("Course not found");

            var groups = course.Groups.Value.Result.ToList();
            var group = groups
                .FirstOrDefault(g => g.Members.Value.Result
                    .Any(m => m.Id == ObjectId.Parse(userId)));

            if (group is null)
                return HttpContext.SendBadRequest<Group>("User does not belong to any group");

            return group;
        }

        [HttpGet("{courseId}/GetUserSkills/{userId}")]
        public async Task<List<NumRangeSkill>> GetUserSkills([FromRoute] string courseId, [FromRoute] string userId)
        {
            if (!IsUserLoggedOn())
                return HttpContext.SendForbidden<List<NumRangeSkill>>("Access denied. User must be logged on");

            var course = await Course.Load(courseId);

            if (course is null)
                return HttpContext.SendBadRequest<List<NumRangeSkill>>("Course not found");

            var user = await DatabaseLayer.Models.User.Load(ObjectId.Parse(userId));
            if (user is null)
                return HttpContext.SendBadRequest<List<NumRangeSkill>>("User not found");

            var skills = course.EnrollmentRecords[user];
            return skills;
        }

        [HttpPost("{courseId}/UpdateUserSkills/{userId}")]
        public async Task<bool> UpdateUserSkills([FromRoute] string courseId, [FromRoute] string userId,
            JToken[] updatedSkills)
        {
            if (updatedSkills == null)
                return HttpContext.SendBadRequest<bool>("updatedSkills passed as null");

            if (!IsUserLoggedOn())
                return HttpContext.SendForbidden<bool>("Access denied. User must be logged on");

            var course = await Course.Load(courseId);

            if (course is null)
                return HttpContext.SendBadRequest<bool>("Course not found");

            var user = await DatabaseLayer.Models.User.Load(ObjectId.Parse(userId));
            if (user is null)
                return HttpContext.SendBadRequest<bool>("User not found");

            List<Skill> skillsDeserialized;
            try
            {
                skillsDeserialized = updatedSkills.Select(SkillFactory.GetSkill).ToList();
            }
            catch (ArgumentException)
            {
                return HttpContext.SendBadRequest<bool>("Unable to parse skills");
            }

            course.EnrollmentRecords[user] = skillsDeserialized.Cast<NumRangeSkill>().ToList();

            await course.Save();
            return HttpContext.SendOk();
        }

        [HttpGet("{courseId}/GetSkillDomain")]
        public async Task<List<String>> GetSkillDomain([FromRoute] string courseId)
        {
            if (!IsUserLoggedOn())
                return HttpContext.SendForbidden<List<String>>("Access denied. User must be logged on");

            var course = await Course.Load(courseId);

            if (course is null)
                return HttpContext.SendBadRequest<List<String>>("Course not found");

            var skills = course.PermittedSkills.Select(skill => skill.Name).ToList();
            return skills;
        }

        [HttpPost("CreateCourse")]
        public async Task<Course> CreateCourse(string[] emails, string name, string description, string startDate,
            string endDate, int? minGroupSize, int? maxGroupSize, JToken[] skills)
        {
            if (!IsUserLoggedOn())
                return HttpContext.SendForbidden<Course>("Access denied. User must be logged on");
            if (!User.IsInRole(GlobalRole.CourseCreator.ToString()))
                return HttpContext.SendUnauthorized<Course>("User not authorized to create courses.");

            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(description) || String.IsNullOrEmpty(startDate)
                || String.IsNullOrEmpty(endDate) || minGroupSize is null || maxGroupSize is null || skills is null)
                return HttpContext.SendBadRequest<Course>("All API parameters are mandatory, but some are invalid");

            if (minGroupSize < 1 || minGroupSize > maxGroupSize)
                return HttpContext.SendBadRequest<Course>("Min group size must be positive and <= max group size");

            var user = await DatabaseLayer.Models.User.Load(GetAuthenticatedUserId());

            if (user is null)
                return HttpContext.SendBadRequest<Course>("User not found");

            List<NumRangeSkill> skillsDeserialized;
            try
            {
                skillsDeserialized = skills.Select(SkillFactory.GetSkill).Cast<NumRangeSkill>().ToList();
            }
            catch (ArgumentException)
            {
                return HttpContext.SendBadRequest<Course>("Unable to parse skills");
            }

            var course = new Course(user, name, description,
                new GroupPreferences((int) minGroupSize, (int) minGroupSize, startDate, endDate), skillsDeserialized);
            user.AddCourse(course);

            // As always, let's hope for the best
            await course.Save();
            await user.Save();

            var invitedUsers = emails
                .Select(email =>
                    MongoDBLayer.GetCollectionForModel<User>().AsQueryable().FirstOrDefault(usr => usr.Email == email))
                .Where(usr => usr != null)
                .ToArray();

            course.EnrollmentRecords[user] = new List<NumRangeSkill>();
            
            foreach (var invitedUser in invitedUsers)
            {
                course.EnrollmentRecords[invitedUser] = new List<NumRangeSkill>();
                invitedUser.AddCourse(course);
                
                //TODO: Move this save task out and await all later. Not sure if mongo is thread safe enough for it though
                await invitedUser.Save();
            }

            await course.Save();
            
            var tasks = invitedUsers.Select(usr =>
                EmailClient.SendEmail(
                    EmailTemplates.CourseCreateSubject(course.Name),
                    EmailTemplates.CourseCreateBody(
                        course.Name, 
                        course.IdStr),
                    usr.Email)
            );

            //TODO: Run this in BG somewhere
            await Task.WhenAll(tasks);

            return course;
        }

        [HttpPost("ModifyGroupSize")]
        public async Task<bool> ModifyGroupSize(string courseId, int? minSize, int? maxSize)
        {
            if (!IsUserLoggedOn())
                return HttpContext.SendForbidden("Access denied. User must be logged on");

            if (minSize is null && maxSize is null)
                return HttpContext.SendBadRequest("min and max size cannot both be null");
            if (minSize != null && minSize < 0)
                return HttpContext.SendBadRequest("Min size cannot be negative");
            if (minSize != null && maxSize != null && maxSize < minSize)
                return HttpContext.SendBadRequest(
                    "Max size cannot be less than min size. It just doesn't make any sense.");

            var course = await Course.Load(courseId);

            if (course is null)
                return HttpContext.SendBadRequest("Course not found");

            if (!IsUserOwnerOrInstructor(course))
                return HttpContext.SendForbidden(
                    "Addess denied. Only owners and instructors can modify course preferences");

            course.Preferences.MinGroupSize = minSize ?? course.Preferences.MinGroupSize;

            if (maxSize != null && maxSize < course.Preferences.MinGroupSize)
                return HttpContext.SendBadRequest(
                    "Max size cannot be less than min size. It just doesn't make any sense.");

            course.Preferences.MaxGroupSize = maxSize ?? course.Preferences.MaxGroupSize;

            await course.Save();

            return HttpContext.SendOk();
        }

        [HttpPost("ModifyStartEndDates")]
        public async Task<bool> ModifyStartEndDates(string courseId, string startDate, string endDate)
        {
            if (!IsUserLoggedOn())
                return HttpContext.SendForbidden("Access denied. User must be logged on");

            if (String.IsNullOrEmpty(startDate) && String.IsNullOrEmpty(endDate))
                return HttpContext.SendBadRequest("Start and end dates cannot both be null or empty");

            //TODO: do some actual date verification

            var course = await Course.Load(courseId);

            if (course is null)
                return HttpContext.SendBadRequest("Course not found");

            if (!IsUserOwnerOrInstructor(course))
                return HttpContext.SendForbidden(
                    "Access denied. Only owners and instructors can modify course preferences");

            course.Preferences.StartDate = startDate ?? course.Preferences.StartDate;
            course.Preferences.EndDate = endDate ?? course.Preferences.EndDate;

            await course.Save();

            return HttpContext.SendOk();
        }

        [HttpPost("ModifyDescription")]
        public async Task<bool> ModifyDescription(string courseId, string description)
        {
            if (!IsUserLoggedOn())
                return HttpContext.SendForbidden("Access denied. User must be logged on");

            if (String.IsNullOrEmpty(description))
                return HttpContext.SendBadRequest("Description cannot be null or empty");

            var course = await Course.Load(courseId);

            if (course is null)
                return HttpContext.SendBadRequest("Course not found");

            if (!IsUserOwnerOrInstructor(course))
                return HttpContext.SendForbidden(
                    "Access denied. Only owners and instructors can modify course preferences");

            course.Description = description;

            await course.Save();

            return HttpContext.SendOk();
        }

        [HttpPost("ChangeOwner")]
        public async Task<bool> ChangeOwner(string courseId, string newOwnerId)
        {
            if (!IsUserLoggedOn())
                return HttpContext.SendForbidden("Access denied. User must be logged on");

            if (String.IsNullOrEmpty(courseId) || String.IsNullOrEmpty(newOwnerId))
                return HttpContext.SendBadRequest("Course id or new owner id cannot be null");

            if (!ObjectId.TryParse(newOwnerId, out var newOwnerObjId) ||
                !MongoDBLayer.GetCollectionForModel<User>().AsQueryable().Any(usr => usr.Id == newOwnerObjId))
                return HttpContext.SendBadRequest("New owner is a fake");

            var course = await Course.Load(courseId);
            if (course is null)
                return HttpContext.SendBadRequest("Course not found");

            if (!IsUserOwner(course))
                return HttpContext.SendForbidden("Access denied. Only owners can transfer ownership");

            course.OwnerId = newOwnerId;
            course.InstructorIds.Remove(newOwnerObjId);

            await course.Save();
            return HttpContext.SendOk();
        }

        [HttpPost("AddInstructor")]
        public async Task<bool> AddInstructor(string courseId, string instructorId)
        {
            if (!IsUserLoggedOn())
                return HttpContext.SendForbidden("Access denied. User must be logged on");

            var course = await Course.Load(courseId);

            if (course is null)
                return HttpContext.SendBadRequest("Course not found");

            if (!IsUserOwner(course))
                return HttpContext.SendForbidden("Access denied. Only owners can add instructors");

            var instructor = await DatabaseLayer.Models.User.Load(instructorId);

            if (instructor is null)
                return HttpContext.SendBadRequest("Instructor not found");


            course.InstructorIds.Add(instructor.Id);
            instructor.AddCourse(course);

            //YOLO
            await course.Save();
            await instructor.Save();

            return HttpContext.SendOk();
        }

        [HttpPost("RemoveInstructor")]
        public async Task<bool> RemoveInstructor(string courseId, string instructorId)
        {
            if (!IsUserLoggedOn())
                return HttpContext.SendForbidden("Access denied. User must be logged on");

            var course = await Course.Load(courseId);

            if (course is null)
                return HttpContext.SendBadRequest("Course not found");

            if (!IsUserOwner(course))
                return HttpContext.SendForbidden("Access denied. Only owners can remove instructors");

            var instructor = await DatabaseLayer.Models.User.Load(instructorId);

            if (instructor is null)
                return HttpContext.SendBadRequest("Instructor not found");

            course.InstructorIds.Remove(instructor.Id);
            instructor.RemoveCourse(course);

            await course.Save();
            await instructor.Save();

            return HttpContext.SendOk();
        }

        [HttpPost("AddGroup")]
        public async Task<Group> AddGroup(string courseId, string name, string bio)
        {
            if (!IsUserLoggedOn())
                return HttpContext.SendForbidden<Group>("Access denied. User must be logged on");

            var course = await Course.Load(courseId);

            if (course is null)
                return HttpContext.SendBadRequest<Group>("Course not found");

            if (!IsUserMember(course))
                return HttpContext.SendForbidden<Group>("Access denied. Only members can create groups within course");

            if (!ObjectId.TryParse(GetAuthenticatedUserId(), out var userObjId))
                return HttpContext.SendBadRequest<Group>("Failed to parse logged on user Id");

            var group = new Group(name, bio, course.Id);
            group.MemberIds.Add(userObjId);

            return group;
        }
    }
}
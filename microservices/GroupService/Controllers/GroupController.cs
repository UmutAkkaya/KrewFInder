using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspCoreTools;
using AspCoreTools.Jwt;
using DatabaseLayer;
using DatabaseLayer.Models;
using FluentValidation;
using FluentValidation.Results;
using GroupService.Contracts;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using Skills;

namespace GroupService.Controllers
{
    public class GroupValidator : AbstractValidator<Group>
    {
        private int MIN_GROUP_NAME_LEN = 1;
        private int MAX_GROUP_NAME_LEN = 255;

        public GroupValidator()
        {
            RuleFor(group => group.Name).NotEmpty().Length(MIN_GROUP_NAME_LEN, MAX_GROUP_NAME_LEN);
            RuleFor(group => group.Bio).NotEmpty();
            RuleFor(group => group.Course).NotNull();
        }
    }

    [Route("api/Group")]
    public class GroupController : Controller, IGroupInterface
    {
        private readonly GroupValidator _groupValidator = new GroupValidator();

        public ValidationResult Validate(Group group)
        {
            return _groupValidator.Validate(group);
        }


        // Post api/Group
        [HttpPost("")]
        [MultiArgJsonBody]
        public async Task<Group> UpdateGroup([UidFromToken] string userId, string id, string name, string bio,
            JToken[] skills)
        {
            var user = await DatabaseLayer.Models.User.Load(ObjectId.Parse(userId));
            if (user is null)
                return HttpContext.SendBadRequest<Group>("User not found");

            Group group = await Group.Load(ObjectId.Parse(id));
            if (group is null)
            {
                return HttpContext.SendBadRequest<Group>(string.Format("Group with id {0} was not found", id));
            }

            if (!group.MemberIds.Contains(user.Id))
            {
                return HttpContext.SendBadRequest<Group>("User is not a member of this group!");
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(bio))
            {
                return HttpContext.SendBadRequest<Group>("One of the required fields is empty");
            }

            group.Name = name.Trim();
            group.Bio = bio.Trim();
            List<NumRangeSkill> skillsDeserialized;
            try
            {
                skillsDeserialized = skills.Select(SkillFactory.GetSkill).Cast<NumRangeSkill>().ToList();
            }
            catch (ArgumentException)
            {
                return HttpContext.SendBadRequest<Group>("Unable to parse skills");
            }

            // NOTE: HACK TO FIX DESERIALIZATION
            group.DesiredSkills = skillsDeserialized;
            await group.Save();
            return group;
        }

        // PUT api/group/:groupId/info/update
        [HttpPut("{groupId}/info/update")]
        [MultiArgJsonBody]
        public async Task<Group> UpdateInfo([FromRoute] string groupId, string name, string bio)
        {
            var group = await Group.Load(groupId);
            if (group is null)
            {
                return HttpContext.SendBadRequest<Group>(string.Format("Group with id {0} was not found", groupId));
            }

            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(bio))
            {
                return HttpContext.SendBadRequest<Group>(string.Format("Empty name and bio for group id {0}", groupId));
            }

            if (!string.IsNullOrEmpty(name))
            {
                group.Name = name.Trim();
            }

            if (!string.IsNullOrEmpty(bio))
            {
                group.Bio = bio.Trim();
            }

            await group.Save();
            return group;
        }

        // PUT api/group/members/remove
        [HttpPut("{groupId}/members/remove")]
        [MultiArgJsonBody]
        public async Task<Group> RemoveMembers([FromRoute] string groupId, List<string> memberIds)
        {
            var group = await Group.Load(groupId);
            if (group is null)
            {
                return HttpContext.SendBadRequest<Group>(string.Format("Group with id {0} was not found", groupId));
            }

            if (memberIds is null || memberIds.Count == 0)
            {
                return HttpContext.SendBadRequest<Group>(
                    string.Format("Empty list of members for removal from group id {0}", groupId));
            }

            var memberObjectIds = memberIds.Select(idStr => ObjectId.Parse(idStr));
            group.RemoveMembers(memberObjectIds);
            await group.Save();
            return group;
        }


        // GET api/group/:groupId/members
        [HttpGet("{groupId}/members")]
        public async Task<IEnumerable<User>> GetMembers(string groupId)
        {
            var group = await Group.Load(groupId);
            if (group is null)
            {
                return HttpContext.SendBadRequest<IEnumerable<User>>("Bad group id");
            }

            return await group.Members.Value;
        }
        //TODO: Add Desired skills API

        // DELETE api/group/:groupId
        [HttpDelete("{groupId}")]
        public async Task<bool> DeleteGroup([FromBody] string groupId)
        {
            await MongoDBLayer.DeleteOne<Group>(groupId);
            return HttpContext.SendOk();
        }

        // GET api/group/:id
        [HttpGet("{id}")]
        public async Task<Group> GetGroup([FromRoute] string id)
        {
            Group group = await Group.Load(id);
            if (group is null)
            {
                return HttpContext.SendBadRequest<Group>("Group not found");
            }

            return group;
        }

        [HttpPost("CreateGroup")]
        [MultiArgJsonBody]
        public async Task<Group> CreateGroup([UidFromToken] string userId, string name, string bio, string courseId,
            JToken[] skills)
        {
            Course course = await Course.Load(courseId);
            if (course is null)
            {
                return HttpContext.SendBadRequest<Group>("Bad course id");
            }

            if (skills == null)
                return HttpContext.SendBadRequest<Group>("Skills passed as null");

            Group group = new Group(name, bio, course);

            List<NumRangeSkill> skillsDeserialized;
            try
            {
                skillsDeserialized = skills.Select(SkillFactory.GetSkill).Cast<NumRangeSkill>().ToList();
            }
            catch (ArgumentException)
            {
                return HttpContext.SendBadRequest<Group>("Unable to parse skills");
            }

            group.DesiredSkills = skillsDeserialized;
            ValidationResult validationResult = Validate(group);
            if (!validationResult.IsValid)
            {
                return HttpContext.SendBadRequest<Group>(validationResult.Errors[0].ErrorMessage);
            }

            group.AddMember(ObjectId.Parse(userId));
            await group.Save();
            User usr = await DatabaseLayer.Models.User.Load(userId);
            usr.AddGroup(group);
            await usr.Save();

            course.AddGroup(group);
            await course.Save();
            
            return group;
        }

        // DELETE api/group/:groupId/members
        [HttpDelete("{groupId}/members")]
        public async Task<Group> RemoveMember([FromRoute] string groupId, [FromBody] string userId)
        {
            var lst = new List<string> {userId};
            return await RemoveMembers(groupId, lst);
        }

        [HttpPost("{groupId}/suggest")]
        [MultiArgJsonBody]
        public async Task<Group> Suggest([FromRoute] string groupId)
        {
            var group = await Group.Load(groupId);
            if (group is null)
            {
                return HttpContext.SendBadRequest<Group>(string.Format("Group with id {0} was not found", groupId));
            }

            var org = await MongoDBLayer.FindOneById<Course>(group.CourseId);

            //11/10 suggestions engine
            var userSuggestions = org.EnrollmentRecords.Keys.ToArray();
            foreach (var user in userSuggestions)
            {
                if (!group.Invitations.ContainsKey(user))
                {
                    group.Invitations.Add(user, InvitationStatus.Suggested);
                    user.Invitations.Add(group, InvitationStatus.Suggested);
                    //TODO: Move this save task out and await all later. Not sure if mongo is thread safe enough for it though
                    await user.Save();
                }
            }

            var groupSuggestions = await org.Groups.Value;
            foreach (var suggestedGroup in groupSuggestions)
            {
                if (!group.MergerInvitations.ContainsKey(suggestedGroup) &&
                    group.MemberIds.Count + suggestedGroup.MemberIds.Count <= org.Preferences.MaxGroupSize)
                {
                    group.MergerInvitations.Add(suggestedGroup, MergerInvitationStatus.Suggested);
                    suggestedGroup.MergerInvitations.Add(group, MergerInvitationStatus.Suggested);
                    //TODO: Move this save task out and await all later. Not sure if mongo is thread safe enough for it though
                    await suggestedGroup.Save();
                }
            }

            await group.Save();

            return group;
        }
    }
}
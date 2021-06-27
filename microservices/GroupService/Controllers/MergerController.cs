using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspCoreTools;
using AspCoreTools.Email;
using DatabaseLayer;
using DatabaseLayer.Models;
using GroupService.Contracts;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace GroupService.Controllers
{
    [Route("api/Merge/")]
    public class MergerController : Controller, IMergerInterface
    {
        private GroupValidator _groupValidator;

        private async Task validateCoursePreferences(Group inviterGroup, Group inviteeGroup)
        {
            if (inviterGroup.CourseId != inviteeGroup.CourseId)
            {
                throw new InvalidOperationException(string.Format(
                    "Group {0} is in a different Course than Group {1}", inviterGroup.IdStr,
                    inviteeGroup.IdStr));
            }

            var course = await Course.Load(inviterGroup.CourseId);
            var inviterCount = inviterGroup.MemberIds.Count;
            var inviteeCount = inviteeGroup.MemberIds.Count;
            var maxGroupSize = course.Preferences.MaxGroupSize;

            // Check if by merging groups we will exceed allowed max group size
            if ((inviterCount + inviteeCount) > maxGroupSize)
            {
                throw new InvalidOperationException(string.Format(
                    "Course {0} allows only {1} members in the group. Merging groups would make it {2} members in total.",
                    course.IdStr,
                    course.Preferences.MaxGroupSize, inviterCount + inviteeCount));
            }
        }

        private async Task validateInvitationStatus(Group inviterGroup, Group inviteeGroup)
        {
            await validateCoursePreferences(inviterGroup, inviteeGroup);

            /* Check both of groups have appropriate merger invites*/
            if (!((inviterGroup.MergerInvitations.ContainsKey(inviteeGroup)) &&
                  (inviteeGroup.MergerInvitations.ContainsKey(inviterGroup))))
            {
                throw new InvalidOperationException(string.Format(
                    "There is no merging invite to merge group {0} with group {1}", inviterGroup.IdStr,
                    inviteeGroup.IdStr));
            }

            if (inviteeGroup.MergerInvitations[inviterGroup] != MergerInvitationStatus.Invited ||
                inviterGroup.MergerInvitations[inviteeGroup] != MergerInvitationStatus.Sent
            )
            {
                throw new InvalidOperationException(string.Format(
                    "Group {0} has already made decision on merging with group {1}", inviterGroup.IdStr,
                    inviteeGroup.IdStr));
            }
        }

        // POST api/merge/:inviterGroupId/:inviteeGroupId/reject
        [HttpPost("{inviterGroupId}/{inviteeGroupId}/reject")]
        public async Task<bool> RejectMergerInvitation([FromRoute] string inviterGroupId,
            [FromRoute] string inviteeGroupId)
        {
            var inviterGroup = await Group.Load(new ObjectId(inviterGroupId));
            var inviteeGroup = await Group.Load(new ObjectId(inviteeGroupId));

            if (inviterGroup is null)
                return HttpContext.SendBadRequest(string.Format("Group with id '{0}' was not found!", inviterGroupId));

            if (inviteeGroup is null)
                return HttpContext.SendBadRequest(string.Format("Group with id '{0}' was not found!", inviteeGroupId));
            try
            {
                validateInvitationStatus(inviterGroup, inviteeGroup);
            }
            catch (Exception e)
            {
                return HttpContext.SendBadRequest(e.Message);
            }

            inviterGroup.MergerInvitations[inviteeGroup] = MergerInvitationStatus.Rejected;
            inviteeGroup.MergerInvitations[inviterGroup] = MergerInvitationStatus.Rejected;
            await inviteeGroup.Save();
            await inviterGroup.Save();
            return true;
        }

        // POST api/merge/:inviterGroupId/:inviteeGroupId/revoke
        [HttpPost("{inviterGroupId}/{inviteeGroupId}/revoke")]
        public async Task<bool> RevokeMergerInvitation([FromRoute] string inviterGroupId,
            [FromRoute] string inviteeGroupId)
        {
            var inviterGroup = await Group.Load(new ObjectId(inviterGroupId));
            if (inviterGroup is null)
                return HttpContext.SendBadRequest(string.Format("Group with id '{0}' was not found!", inviterGroupId));

            var inviteeGroup = await Group.Load(new ObjectId(inviteeGroupId));
            if (inviteeGroup is null)
                return HttpContext.SendBadRequest(string.Format("Group with id '{0}' was not found!", inviteeGroupId));

            try
            {
                validateInvitationStatus(inviterGroup, inviteeGroup);
            }
            catch (Exception e)
            {
                return HttpContext.SendBadRequest(e.Message);
            }


            inviterGroup.MergerInvitations[inviteeGroup] = MergerInvitationStatus.Revoked;
            inviteeGroup.MergerInvitations[inviterGroup] = MergerInvitationStatus.Revoked;
            await inviteeGroup.Save();
            await inviterGroup.Save();
            return true;
        }

        // POST api/merge/:inviterGroupId/:inviteeGroupId/accept
        [HttpPost("{inviterGroupId}/{inviteeGroupId}/accept")]
        public async Task<bool> AcceptMergerInvitation([FromRoute] string inviterGroupId,
            [FromRoute] string inviteeGroupId)
        {
            var inviterGroup = await Group.Load(new ObjectId(inviterGroupId));
            if (inviterGroup is null)
                return HttpContext.SendBadRequest(string.Format("Group with id '{0}' was not found!", inviterGroupId));

            var inviteeGroup = await Group.Load(new ObjectId(inviteeGroupId));
            if (inviteeGroup is null)
                return HttpContext.SendBadRequest(string.Format("Group with id '{0}' was not found!", inviteeGroupId));

            try
            {
                await validateInvitationStatus(inviterGroup, inviteeGroup);
            }
            catch (Exception e)
            {
                return HttpContext.SendBadRequest(e.Message);
            }

            inviterGroup.MergerInvitations[inviteeGroup] = MergerInvitationStatus.Accepted;
            inviterGroup.AddMembers(inviteeGroup.MemberIds);
            await inviterGroup.Save();

            inviteeGroup.MergerInvitations[inviterGroup] = MergerInvitationStatus.Accepted;
            inviteeGroup.isMerged = true;
            await inviteeGroup.Save();
            return true;
        }

        // POST api/merge/create/:inviterGroupId/:inviteeGroupId
        [HttpPost("create/{inviterGroupId}/{inviteeGroupId}")]
        public async Task<bool> CreateMergerInvitation([FromRoute] string inviterGroupId,
            [FromRoute] string inviteeGroupId)
        {
            var inviterGroup = await Group.Load(new ObjectId(inviterGroupId));
            if (inviterGroup is null)
                return HttpContext.SendBadRequest(string.Format("Group with id '{0}' was not found!", inviterGroupId));

            var inviteeGroup = await Group.Load(new ObjectId(inviteeGroupId));
            if (inviteeGroup is null)
                return HttpContext.SendBadRequest(string.Format("Group with id '{0}' was not found!", inviteeGroupId));

            try
            {
                await validateCoursePreferences(inviterGroup, inviteeGroup);
            }
            catch (Exception e)
            {
                return HttpContext.SendBadRequest(e.Message);
            }

            /*
             * When a merger invite created:
             * invitee sees 'invited' invite status from inviter
             * inviter sees 'sent' invite status for invitee
             * 
             */
            inviteeGroup.MergerInvitations[inviterGroup] = MergerInvitationStatus.Invited;
            inviterGroup.MergerInvitations[inviteeGroup] = MergerInvitationStatus.Sent;
            await inviterGroup.Save();

            await inviteeGroup.Save();


            var tasks = new List<Task>();
            foreach (var inviter in await inviterGroup.Members.Value)
            {
                tasks.Add(EmailClient.SendEmail(
                    EmailTemplates.GroupMergerSubject(inviterGroup.Name, inviteeGroup.Name),
                    EmailTemplates.GroupMergerBody(inviterGroup.Name, inviteeGroup.Name, inviterGroupId, inviteeGroupId,
                        inviter.FirstName),
                    inviter.Email
                ));
            }
            foreach (var invitee in await inviteeGroup.Members.Value)
            {
                tasks.Add(EmailClient.SendEmail(
                    EmailTemplates.GroupMergerSubject(inviteeGroup.Name, inviterGroup.Name),
                    EmailTemplates.GroupMergerBody(inviteeGroup.Name, inviterGroup.Name, inviteeGroupId, inviterGroupId,
                        invitee.FirstName),
                    invitee.Email
                ));
            }

            return true;
        }
    }
}
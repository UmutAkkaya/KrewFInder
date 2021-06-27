using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseLayer.Models;
using Newtonsoft.Json.Linq;
using Skills;

namespace GroupService.Contracts
{
    public interface IGroupInterface
    {
        Task<Group> GetGroup(string groupId);
        Task<Group> CreateGroup(string userId, string name, string bio, string courseId, JToken[] skills);
        Task<Group> UpdateInfo(string groupId, string newName, string newBio);

        /* Members */
        Task<Group> RemoveMember(string groupId, string userId);
        Task<Group> RemoveMembers(string groupId, List<string> userId);
        Task<IEnumerable<User>> GetMembers(string groupId);
        
        //TODO:
        
        /* Mergers */
//        Task<Dictionary<Group, MergerInvitationStatus>> GetMergerInvitations(string groupId);
//        /* Desired Skills */
//        Task<bool> AddDesiredSkill(string groupId, Skill skill);
//        Task<bool> RemoveDesiredSkill(string groupId, Skill skill);
//        Task<List<Skill>> GetDesiredSkills(string groupId);
    }
}
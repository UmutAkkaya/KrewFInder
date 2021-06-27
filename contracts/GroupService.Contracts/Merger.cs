using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseLayer.Models;
using Skills;

namespace GroupService.Contracts
{
    public interface IMergerInterface
    {
        /* Merger Invitations */
        Task<bool> CreateMergerInvitation(string inviterGroupId, string inviteeGroupId);
        Task<bool> AcceptMergerInvitation(string inviterGroupId, string inviteeGroupId);
        Task<bool> RejectMergerInvitation(string inviterGroupId, string inviteeGroupId);

        Task<bool> RevokeMergerInvitation(string inviterGroupId, string inviteeGroupId);
     
    }
}
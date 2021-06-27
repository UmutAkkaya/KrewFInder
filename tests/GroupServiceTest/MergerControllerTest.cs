using System;
using System.Linq;
using System.Threading.Tasks;
using AspCoreTools.Middleware.Exceptions;
using DatabaseLayer.Models;
using GroupService.Controllers;
using GroupServiceTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GroupServiceTest
{
    [TestClass]
    public class MergerControllerTest: AbstractControllerTest
    {
        private string _databaseNameCache;
        private MergerController _controller;
        const string _inviterName = "Inviter";
        const string _inviteeName = "Receiver";
        Group _inviter;
        Group _invitee; 

        private string _testingDatabase
        {
            get
            {
                return _databaseNameCache ?? (_databaseNameCache =
                           "test_" + (int) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
            }
        }

        public async Task InitInviterInviteeGroups()
        {
            await _tstCourse.Save();
            _inviter = getNewTestGroup(_inviterName);
            _invitee = getNewTestGroup(_inviteeName);
            _inviter.MergerInvitations[_invitee] = MergerInvitationStatus.Sent;
            _invitee.MergerInvitations[_inviter] = MergerInvitationStatus.Invited;
            _invitee.AddMember(getNewTestUser("inviteeMember1"));
            _invitee.AddMember(getNewTestUser("inviteeMember2"));
            _inviter.AddMember(getNewTestUser("inviterMember1"));
            _inviter.AddMember(getNewTestUser("inviterMember2"));
            await _inviter.Save();
            await _invitee.Save();
        }

        public async void CleanupInviterInviteeGroups()
        {
            await _inviter.Delete();
            await _invitee.Delete();
        }

        [TestInitialize]
        public void InitController()
        {
            _controller = new MergerController();
            InitTstModels(_controller);
            setController(_controller);
            
        }

        private async void CleanModels()
        {
            await _tstUser.Delete();
            await _tstCourse.Delete();
            await _tstGroup.Delete();
        }


        [TestMethod]
        public async Task TestCreateMergerInvite()
        {

            await InitInviterInviteeGroups();
            await _controller.CreateMergerInvitation(_inviter.IdStr, _invitee.IdStr);
            Group inviter = await Group.Load(_inviter.IdStr);
            Group invitee = await Group.Load(_invitee.IdStr);
            Assert.AreEqual(inviter.MergerInvitations[invitee], MergerInvitationStatus.Sent);
            Assert.AreEqual(invitee.MergerInvitations[inviter], MergerInvitationStatus.Invited);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ClientException))]
        public async Task TestCreateMergerInviteExceedMaxMembersLimit()
        {
            await InitInviterInviteeGroups();
            _tstCourse.Preferences.MaxGroupSize = 1;
            await _tstCourse.Save();
            await _controller.CreateMergerInvitation(_inviter.IdStr, _invitee.IdStr);
        }
        
        [TestMethod]
        public async Task AcceptMergerInviteTest()
        {
            await InitInviterInviteeGroups();
            int inviteeMemberCount = _invitee.MemberIds.Count; 
            int inviterMemberCount = _inviter.MemberIds.Count;
            int expectedMemberCountAfter = inviteeMemberCount + inviterMemberCount;
            bool result = await _controller.AcceptMergerInvitation(_inviter.IdStr, _invitee.IdStr);
            Group inviter = await Group.Load(_inviter.IdStr);
            Group invitee = await Group.Load(_invitee.IdStr);
            Assert.IsTrue(result);
            Assert.AreEqual(inviter.MergerInvitations[invitee], MergerInvitationStatus.Accepted);
            Assert.AreEqual(invitee.MergerInvitations[inviter], MergerInvitationStatus.Accepted);
            
            //all invitee member ids are in inviter
            Assert.IsTrue(invitee.MemberIds.All(inviter.MemberIds.Contains));
            Assert.IsTrue(inviter.MemberIds.Count == expectedMemberCountAfter);
            Assert.IsTrue(invitee.MemberIds.Count == inviteeMemberCount);
            Assert.IsTrue(invitee.isMerged);
            Assert.IsFalse(inviter.isMerged);


        }
        [TestMethod]
        [ExpectedException(typeof(ClientException))]
        public async Task AcceptMergerInviteExceedMaxMembersLimitTest()
        {
            _tstCourse.Preferences.MaxGroupSize = 1;
            await _tstCourse.Save();
            await InitInviterInviteeGroups();
            await _controller.AcceptMergerInvitation(_inviter.IdStr, _invitee.IdStr);
        }
 
        [TestMethod]
        public async Task RejectMergerInviteTest()
        {
            await InitInviterInviteeGroups();
            int inviteeMemberCount = _invitee.MemberIds.Count; 
            int inviterMemberCount = _inviter.MemberIds.Count;
            bool result = await _controller.RejectMergerInvitation(_inviter.IdStr, _invitee.IdStr);
            Group inviter = await Group.Load(_inviter.IdStr);
            Group invitee = await Group.Load(_invitee.IdStr);
            Assert.IsTrue(result);
            Assert.AreEqual(inviter.MergerInvitations[invitee], MergerInvitationStatus.Rejected);
            Assert.AreEqual(invitee.MergerInvitations[inviter], MergerInvitationStatus.Rejected);
            Assert.IsTrue(inviter.MemberIds.Count == inviterMemberCount);
            Assert.IsTrue(invitee.MemberIds.Count == inviteeMemberCount);
            Assert.IsFalse(invitee.isMerged);
            Assert.IsFalse(inviter.isMerged);


        }
        
        [TestMethod]
        public async Task RevokeMergerInviteTest()
        {
            await InitInviterInviteeGroups();
            int inviteeMemberCount = _invitee.MemberIds.Count; 
            int inviterMemberCount = _inviter.MemberIds.Count;
            bool result = await _controller.RevokeMergerInvitation(_inviter.IdStr, _invitee.IdStr);
            Group inviter = await Group.Load(_inviter.IdStr);
            Group invitee = await Group.Load(_invitee.IdStr);
            Assert.IsTrue(result);
            Assert.AreEqual(invitee.MergerInvitations[inviter], MergerInvitationStatus.Revoked);
            Assert.AreEqual(inviter.MergerInvitations[invitee], MergerInvitationStatus.Revoked);
            Assert.IsTrue(inviter.MemberIds.Count == inviterMemberCount);
            Assert.IsTrue(invitee.MemberIds.Count == inviteeMemberCount);
            Assert.IsFalse(invitee.isMerged);
            Assert.IsFalse(inviter.isMerged);

        }
        
        [TestMethod]
        [ExpectedException(typeof(ClientException))]
        public async Task AcceptNonExistentMergerInviteTest()
        {

            var inviter = getNewTestUser("Inviter");
            var invitee = getNewTestUser("Invitee");
            await inviter.Save();
            await invitee.Save();
            bool result = await _controller.AcceptMergerInvitation(inviter.IdStr, invitee.IdStr);

        }
        
    }
}
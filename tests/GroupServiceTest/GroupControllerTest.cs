using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspCoreTools.Middleware.Exceptions;
using DatabaseLayer.Models;
using GroupService.Controllers;
using GroupServiceTests;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
namespace GroupServiceTest
{
    [TestClass]
    public class GroupControllerTest : AbstractControllerTest
    {
        private string _databaseNameCache;
        private GroupController _controller;

        private string _testingDatabase
        {
            get
            {
                return _databaseNameCache ?? (_databaseNameCache =
                           "test_" + (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
            }
        }
        [TestInitialize]
        public void initController()
        {
            _controller = new GroupController();
            InitTstModels(_controller);
            setController(_controller);
        }

        private async void cleanModels()
        {
            await _tstUser.Delete();
            await _tstCourse.Delete();
            await _tstGroup.Delete();
        }

        [TestCleanup]
        public void cleanup()
        {
            cleanModels();

        }

        [TestMethod]
        public async Task CreateGroupTest()
        {
            await _tstCourse.Save();
            await _tstUser.Save();
            JToken[] tokens = { JToken.Parse("{\"name\": \"Punctuality, Responsibility, Hardworking\", \"minValue\": 0, \"maxValue\": 5, \"value\": 25, \"type\": \"numrangeskill\"}") };
            Group createdGroup = await _controller.CreateGroup(_tstUser.IdStr,_tstGroup.Name, _tstGroup.Bio, _tstCourse.IdStr, tokens);
            Assert.AreEqual(_tstGroup, createdGroup);
        }

        [TestMethod]
        public async Task GetGroupTest()
        {
            await _tstGroup.Save();
            Group group = await _controller.GetGroup(_tstGroup.IdStr);
            Assert.AreEqual(_tstGroup, group);
        }

        [TestMethod]
        [ExpectedException(typeof(ClientException))]
        public async Task GetGroupBadIdTest()
        {
            await _tstGroup.Save();
            Group group = await _controller.GetGroup(BAD_ID);
        }

        [TestMethod]
        public async Task UpdateNameOnlyTest()
        {
            await _tstGroup.Save();
            string newName = _tstGroup.Name + "111";
            Group updatedGroup = await _controller.UpdateInfo(_tstGroup.IdStr, newName, "");
            Assert.AreEqual(newName, updatedGroup.Name);

            //change back to check if other attributes remained the same
            updatedGroup.Name = _tstGroup.Name;
            Assert.AreEqual(updatedGroup, _tstGroup);
        }

        [TestMethod]
        public async Task UpdateBioOnlyTest()
        {
            await _tstGroup.Save();
            string newBio = _tstGroup.Bio + "111";
            Group updatedGroup = await _controller.UpdateInfo(_tstGroup.IdStr, "", newBio);
            Assert.AreEqual(newBio, updatedGroup.Bio);

            //change back to check if other attributes remained the same
            updatedGroup.Bio = _tstGroup.Bio;
            Assert.AreEqual(updatedGroup, _tstGroup);
        }

        [TestMethod]
        [ExpectedException(typeof(ClientException))]
        public async Task UpdateInfoBadIdTest()
        {
            await _tstGroup.Save();
            string newBio = _tstGroup.Bio + "111";
            string newName = _tstGroup.Name + "111";
            Group updatedGroup = await _controller.UpdateInfo(BAD_ID, newName, newBio);

        }

        [TestMethod]
        [ExpectedException(typeof(ClientException))]
        public async Task UpdateInfoBadBioTest()
        {
            await _tstGroup.Save();
            string newBio = "";
            string newName = "";
            Group updatedGroup = await _controller.UpdateInfo(_tstGroup.IdStr, newName, newBio);
            Assert.IsNull(updatedGroup);
        }

        [TestMethod]
        public async Task UpdateRemoveMemberTest()
        {
            await _tstGroup.Save();
            var memberIds = new List<string>();
            memberIds.Add(_tstUser.IdStr);

            Group updatedGroup = await _controller.RemoveMembers(_tstGroup.IdStr, memberIds);
            int membersCount = (await updatedGroup.Members.Value).Count();
            Assert.IsTrue(membersCount == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ClientException))]
        public async Task RemoveMemberBadIdTest()
        {
            await _tstGroup.Save();
            Group updatedGroup = await _controller.RemoveMembers(BAD_ID, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ClientException))]
        public async Task RemoveMemberBadListTest()
        {
            await _tstGroup.Save();
            Group updatedGroup = await _controller.RemoveMembers(_tstGroup.IdStr, null);
        }

        [TestMethod]
        public async Task DeleteGroupTest()
        {
            await _tstGroup.Save();
            bool result = await _controller.DeleteGroup(_tstGroup.IdStr);
            AssertOk();
        }

        [TestMethod]
        public async Task RemoveGroupMemberTest()
        {
            User userToDelete = getNewTestUser("DELETE", "THISMEMBER");
            _tstGroup.AddMember(userToDelete);
            await _tstGroup.Save();
            Group group = await _controller.RemoveMember(_tstGroup.IdStr, userToDelete.IdStr);
            _tstGroup.RemoveMember(userToDelete);
            Assert.AreEqual(group, _tstGroup);
            AssertOk();
        }

    }
}
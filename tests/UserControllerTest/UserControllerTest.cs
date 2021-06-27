using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserService.Controllers;
using UserService.Contracts;
using DatabaseLayer.Models;
using DatabaseLayer;
using DatabaseLayerTest;
using System.Threading.Tasks;
using Crypto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Skills;
using AspCoreTools.Middleware.Exceptions;

namespace UserControllerTest
{
    [TestClass]
    public class UserControllerTest : DatabaseTest
    {
        private UserController _controller;

        private User _tstUser;
        private String _firstName = "John";
        private String _lastName = "Smith";
        private String _email = "donotreply@reply.com";
        private String _password = "security101";

        private Group _tstGroup;
        private string _grpName = "DaKrew";
        private NumRangeSkill _desiredSkill = new NumRangeSkill("Punctuality, Responsibility, Hardworking", 25);

        private string _grpBio =
            "We are the Alpha and Omega. We are the creators. Thanks to us, you will no longer have to suffer" +
            " from being unlucky with your teammates ever again. And if you do - you can even leave a review";

        private Course _tstCourse;
        private String _courseName = "CSC302";
        private String _courseDescription = "Engineering Software";
        private GroupPreferences _grpPrefs = new GroupPreferences(0, 5, "Today", "Tomorrow");

        private void InitTstModels()
        {
            _tstUser = new User(_firstName, _lastName, _email, PasswordUtils.HashPassword(_password));
            _tstUser.GlobalRoles.Add(GlobalRole.Admin);
            _tstCourse = new Course(_tstUser, _courseName, _courseDescription, _grpPrefs);
            _tstGroup = new Group(_grpName, _grpBio, _tstCourse);
            _tstGroup.DesiredSkills.Add(_desiredSkill);
        }

        private void verifyUser(User user)
        {
            Assert.AreEqual(user.FirstName, _firstName);
            Assert.AreEqual(user.LastName, _lastName);
            Assert.AreEqual(user.Email, _email);
        }

        private string _databaseNameCache;
        private string _testingDatabase
        {
            get
            {
                return _databaseNameCache ?? (_databaseNameCache =
                           "test_" + (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
            }
        }

        private void AssertOk()
        {
            Assert.AreEqual(_controller.HttpContext.Response.StatusCode, 200);
        }

        private void AssertBadRequest()
        {
            Assert.AreEqual(_controller.HttpContext.Response.StatusCode, 400);
        }

        [TestInitialize()]
        public void InitController()
        {
            _controller = new UserController();
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        // TODO: Add authentication tests!
        [TestMethod]
        public async Task CreateUserTest()
        {
            var user = await _controller.CreateUser(_firstName, _lastName, _email, _password);
            Assert.IsNull(user.Password);
            verifyUser(user);
            AssertOk();
        }

        [TestMethod]
        public async Task CreateUserLoadTest()
        {
            var controllerUser = await _controller.CreateUser(_firstName, _lastName, _email, _password);
            var user = await User.Load(controllerUser.Id);
            verifyUser(user);

            AssertOk();
        }

        [TestMethod]
        [ExpectedException(typeof(ClientException))]
        public async Task CreateUserBadName()
        {
            await _controller.CreateUser("", _lastName, _email, _password);
        }

        [TestMethod]
        [ExpectedException(typeof(ClientException))]
        public async Task CreateUserBadEmail()
        {
            await _controller.CreateUser(_firstName, _lastName, "email", _password);
        }

        [TestMethod]
        [ExpectedException(typeof(ClientException))]
        public async Task CreateUserBadPassword()
        {
            await _controller.CreateUser(_firstName, _lastName, _email, "lol");
        }

        [TestMethod]
        public async Task GetUserTest()
        {
            InitTstModels();
            await _tstUser.Save();

            var user = await _controller.GetUser(_tstUser.IdStr, _tstUser.IdStr);
            Assert.IsNull(user.Password);
            verifyUser(user);

            AssertOk();
        }

        [TestMethod]
        [ExpectedException(typeof(ClientException))]
        public async Task GetNonExistentUser()
        {
            InitTstModels();
            await _controller.GetUser(_tstUser.IdStr, "This is clearly doesn't exist");
        }

        [TestMethod]
        public async Task UpdateUserName()
        {
            InitTstModels();
            await _tstUser.Save();

            String newFirstName = "New First Name";
            String newLastName = "Last";

            await _controller.ChangePersonalInfo(_tstUser.IdStr, newFirstName, newLastName);


            var user = await User.Load(_tstUser.IdStr);

            Assert.AreEqual(user.FirstName, newFirstName);
            Assert.AreEqual(user.LastName, newLastName);

            AssertOk();
        }

        [TestMethod]
        public async Task UpdateUserFirstName()
        {
            InitTstModels();
            await _tstUser.Save();

            String newFirstName = "New First Name";

            await _controller.ChangePersonalInfo(_tstUser.IdStr, newFirstName, null);


            var user = await User.Load(_tstUser.IdStr);

            Assert.AreEqual(user.FirstName, newFirstName);
            Assert.AreEqual(user.LastName, _lastName);

            AssertOk();
        }

        [TestMethod]
        public async Task UpdateUserLastName()
        {
            InitTstModels();
            await _tstUser.Save();

            String newLastName = "Last";

            await _controller.ChangePersonalInfo(_tstUser.IdStr, null, newLastName);


            var user = await User.Load(_tstUser.IdStr);

            Assert.AreEqual(user.FirstName, _firstName);
            Assert.AreEqual(user.LastName, newLastName);

            AssertOk();
        }

        [TestMethod]
        public async Task UpdateUserNameNullTest()
        {
            InitTstModels();
            await _tstUser.Save();

            await _controller.ChangePersonalInfo(_tstUser.IdStr, null, null);


            var user = await User.Load(_tstUser.IdStr);

            Assert.AreEqual(user.FirstName, _firstName);
            Assert.AreEqual(user.LastName, _lastName);

            AssertOk();
        }

        [TestMethod]
        public async Task ChangePasswordTest()
        {
            InitTstModels();
            await _tstUser.Save();

            var newPassword = "New security101";
            await _controller.ChangePassword(_tstUser.IdStr, _password, newPassword);

            var user = await User.Load(_tstUser.IdStr);

            Assert.IsTrue(PasswordUtils.ComparePassword(newPassword, user.Password));

            AssertOk();
        }

        [TestMethod]
        [ExpectedException(typeof(ClientException))]
        public async Task ChangeBadPasswordTest()
        {
            InitTstModels();
            await _tstUser.Save();

            var newPassword = "lole";
            await _controller.ChangePassword(_tstUser.IdStr, _password, newPassword);

            await User.Load(_tstUser.IdStr);
        }


        [TestMethod]
        [ExpectedException(typeof(ClientException))]
        public async Task ChangeIncorrectPasswordTest()
        {
            InitTstModels();
            await _tstUser.Save();

            var newPassword = "valid password";
            await _controller.ChangePassword(_tstUser.IdStr, "incorrect", newPassword);

        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void AddGlobalRoleTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void RemoveGlobalRoleTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public async Task GetGroupsTest()
        {
            InitTstModels();
            _tstUser.AddGroup(_tstGroup);
            await _tstUser.Save();

            var groups = _controller.GetGroups(_tstUser.IdStr);

            var user = await User.Load(_tstUser.IdStr);
            Assert.IsTrue(user.GroupIds.Contains(_tstGroup.Id));
        }

        [TestMethod]
        public async Task GetGroupsEmptyTest()
        {
            InitTstModels();
            await _tstUser.Save();

            var user = await User.Load(_tstUser.IdStr);
            Assert.IsTrue(_tstUser.GroupIds.Count == 0);
        }

    }
}

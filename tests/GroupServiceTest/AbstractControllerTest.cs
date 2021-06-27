using System;
using System.Collections.Generic;
using System.Linq;
using Crypto;
using DatabaseLayer.Models;
using DatabaseLayerTest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skills;

namespace GroupServiceTests
{
    [TestClass]
    public abstract class AbstractControllerTest : DatabaseTest
    {
        protected User _tstUser;
        protected const string _firstName = "John";
        protected const string _lastName = "Smith";
        protected const string _email = "donotreply@reply.com";
        protected const string _password = "security101";

        protected Group _tstGroup;
        protected const string _grpName = "DaKrew";
        protected NumRangeSkill _desiredSkill = new NumRangeSkill("Punctuality, Responsibility, Hardworking", 25);

        protected const string _grpBio =
            "We are the Alpha and Omega. We are the creators. Thanks to us, you will no longer have to suffer" +
            " from being unlucky with your teammates ever again. And if you do - you can even leave a review";

        protected Course _tstCourse;
        protected const string _courseName = "CSC302";
        protected const string _courseDescription = "The only course you need to take.";
        protected GroupPreferences _grpPrefs = new GroupPreferences(0, 6, "Today", "Tomorrow");
        protected Controller _controller;

        protected const string BAD_ID = "VERY_VERY_BAD_ID";

        protected void InitTstModels(Controller controller)
        {
            _tstUser = getNewTestUser();
            _tstCourse = new Course(_tstUser, _courseName, _courseDescription, _grpPrefs);
            _tstGroup = getNewTestGroup();
            _tstGroup.AddMember(_tstUser);
        }

        protected void setController(Controller controller)
        {
            _controller = controller;
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }


        /* helper methods */
        private string _databaseNameCache;

        private string _testingDatabase
        {
            get
            {
                return _databaseNameCache ?? (_databaseNameCache =
                           "test_" + (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
            }
        }

        protected void AssertOk()
        {
            Assert.AreEqual(_controller.HttpContext.Response.StatusCode, 200);
        }

        protected void AssertBadRequest()
        {
            Assert.AreEqual(_controller.HttpContext.Response.StatusCode, 400);
        }

        protected Group getNewTestGroup(string name = _grpName)
        {
            Group group = new Group(_grpName, _grpBio, _tstCourse);
            group.DesiredSkills = new List<NumRangeSkill> { _desiredSkill };
            return group;
        }

        protected User getNewTestUser(string firstName = _firstName, string lastName = _lastName, string email = _email)
        {
            User user = new User(firstName, lastName, email, PasswordUtils.HashPassword(_password));
            return user;
        }
    }
}
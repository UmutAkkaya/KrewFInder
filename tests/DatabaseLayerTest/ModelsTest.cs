using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLayer;
using DatabaseLayer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Skills;
using Crypto;


namespace DatabaseLayerTest
{
    // This is a simple collection of tests to ensure each model saves and loads its fields correctly
    [TestClass]
    public class ModelsTest : DatabaseTest
    {
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
            Assert.IsTrue(PasswordUtils.ComparePassword(_password, user.Password));
            Assert.IsTrue(user.GlobalRoles.Contains(GlobalRole.Admin));
        }

        private void verifyGroup(Group group)
        {
            Assert.AreEqual(group.Name, _grpName);
            Assert.AreEqual(group.Bio, _grpBio);
            Assert.AreEqual(_tstGroup.DesiredSkills[0].Name, _desiredSkill.Name);
        }

        private void verifyCourse(Course course)
        {
            Assert.AreEqual(course.Name, _courseName);
            Assert.AreEqual(course.Preferences.StartDate, _grpPrefs.StartDate);
        }

        [TestMethod]
        public async Task UserSaveLoad()
        {
            InitTstModels();
            _tstUser.AddCourse(_tstCourse);
            _tstUser.AddGroup(_tstGroup);
            _tstUser.Invitations[_tstGroup] = InvitationStatus.Invited;
            await _tstGroup.Save();
            await _tstCourse.Save();
            await _tstUser.Save();

            var tstUserReloaded = await User.Load(_tstUser.Id);

            verifyUser(tstUserReloaded);
            Assert.AreEqual(tstUserReloaded.Invitations[_tstGroup], InvitationStatus.Invited);

            var courses = await tstUserReloaded.Courses.Value;
            var groups = await tstUserReloaded.Groups.Value;
            verifyCourse(courses.ToList()[0]);
            verifyGroup(groups.ToList()[0]);
        }

        [TestMethod]
        public async Task GroupSaveLoad()
        {
            InitTstModels();
            _tstGroup.AddMember(_tstUser);
            _tstGroup.Invitations[_tstUser] = InvitationStatus.Accepted;
            _tstGroup.MergerInvitations[_tstGroup] = MergerInvitationStatus.Invited;
            await _tstUser.Save();
            await _tstCourse.Save();
            await _tstGroup.Save();

            var tstGroupReloaded = await Group.Load(_tstGroup.Id);
            verifyGroup(tstGroupReloaded);
            Assert.AreEqual(tstGroupReloaded.Invitations[_tstUser], InvitationStatus.Accepted);
            Assert.AreEqual(tstGroupReloaded.MergerInvitations[_tstGroup], MergerInvitationStatus.Invited);

            var members = await tstGroupReloaded.Members.Value;
            verifyUser(members.ToList()[0]);
        }

        [TestMethod]
        public async Task CourseSaveLoad()
        {
            InitTstModels();
            _tstCourse.AddGroup(_tstGroup);
            var tstSkills = new List<NumRangeSkill>();
            var tstSkillName = "Aim, CS, Map awarness, Working Hard";
            tstSkills.Add(new NumRangeSkill(tstSkillName, 3));
            _tstCourse.EnrollmentRecords[_tstUser] = tstSkills;

            await _tstUser.Save();
            await _tstGroup.Save();
            await _tstCourse.Save();

            var tstCourseReloaded = await Course.Load(_tstCourse.Id);
            verifyCourse(tstCourseReloaded);

            var groups = await tstCourseReloaded.Groups.Value;
            verifyGroup(groups.ToList()[0]);

            Assert.AreEqual(tstCourseReloaded.EnrollmentRecords[_tstUser][0].Name, tstSkillName);
        }

        [TestMethod]
        public async Task UserQueryTest()
        {
            InitTstModels();
            await _tstUser.Save();

            var tstUserReloaded = MongoDBLayer.GetCollectionForModel<User>()
                .AsQueryable().FirstOrDefault(usr => usr.FirstName.Equals(_firstName));
            verifyUser(tstUserReloaded);
        }

        [TestMethod]
        public async Task NonExistentQueryTest()
        {
            InitTstModels();
            await _tstUser.Save();
            var user = await MongoDBLayer.FindOne<User>(usr => usr.FirstName.Equals("This first name clearly doesn't exist"));
            Assert.IsNull(user);
        }
    }
}

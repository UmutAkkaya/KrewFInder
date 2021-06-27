using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace DatabaseLayer.Models
{
    public enum InvitationStatus
    {
        Invited, Revoked, Rejected, Accepted, Suggested, Unknown
    }

    public enum GlobalRole
    {
        Admin,
        CourseCreator
    }

    [CollectionName("users")]
    public class User : DatabaseModel
    {

        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }

        // Password should be saved as BSON to the database, but not shown when deserialized to user
        [JsonIgnore]
        public byte[] Password { get; set; } //Plaintext only
        public ISet<GlobalRole> GlobalRoles { get; set; } = new HashSet<GlobalRole>();

        public LazyLoadDictionary<Group, InvitationStatus> Invitations { get; set; } = new LazyLoadDictionary<Group, InvitationStatus>();

        [BsonIgnore]
        [JsonIgnore]
        public Lazy<Task<IEnumerable<Course>>> Courses { get; private set; }
        [BsonElement("Courses")]
        public ISet<ObjectId> CourseIds { get; private set; } = new HashSet<ObjectId>();

        [BsonIgnore]
        [JsonIgnore]
        public Lazy<Task<IEnumerable<Group>>> Groups { get; private set; }
        [BsonElement("Groups")]
        public ISet<ObjectId> GroupIds { get; private set; } = new HashSet<ObjectId>();

        private User()
        {
            Courses = new Lazy<Task<IEnumerable<Course>>>(async () => await MongoDBLayer.FindById<Course>(CourseIds));
            Groups = new Lazy<Task<IEnumerable<Group>>>(async () => await MongoDBLayer.FindById<Group>(GroupIds));
        }

        public User(String firstName, String lastName, String email, byte[] password) : this()
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
        }

        public void AddCourse(Course course)
        {
            CourseIds.Add(course.Id);
        }

        public void RemoveCourse(Course course)
        {
            CourseIds.Remove(course.Id);
        }

        public void AddGroup(Group group)
        {
            GroupIds.Add(group.Id);
        }

        public void RemoveGroup(Group group)
        {
            GroupIds.Remove(group.Id);
        }

        public static async Task<User> Load(ObjectId id)
        {
            return await MongoDBLayer.FindOneById<User>(id);
        }

        public static async Task<User> Load(String id)
        {
            return await MongoDBLayer.FindOneById<User>(id);
        }

        public override async Task Save()
        {
            await MongoDBLayer.UpsertOne(this);
        }

        public override async Task Delete()
        {
            await MongoDBLayer.DeleteOne(this);
        }
    }
}

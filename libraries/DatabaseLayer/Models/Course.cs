using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Skills;
using Newtonsoft.Json;

namespace DatabaseLayer.Models
{
    public class GroupPreferences
    {
        public int MinGroupSize { get; set; }
        public int MaxGroupSize { get; set; }
        public String StartDate { get; set; }
        public String EndDate { get; set; }

        public GroupPreferences(int minGroupSize, int maxGroupSize, String startDate, String endDate)
        {
            MinGroupSize = minGroupSize;
            MaxGroupSize = maxGroupSize;
            StartDate = startDate;
            EndDate = endDate;
        }
    }

    [CollectionName("courses")]
    public class Course : DatabaseModel
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public GroupPreferences Preferences { get; set; }

        [BsonIgnore]
        [JsonIgnore]
        public Lazy<Task<User>> Owner { get; set; }
        [BsonElement("Owner")]
        public String OwnerId { get; set; }

        public LazyLoadDictionary<User, List<NumRangeSkill>> EnrollmentRecords { get; private set; } = new LazyLoadDictionary<User, List<NumRangeSkill>>();

        [BsonIgnore]
        [JsonIgnore]
        public Lazy<Task<IEnumerable<Group>>> Groups { get; private set; }
        [BsonElement("Groups")]
        public List<ObjectId> GroupIds { get; private set; } = new List<ObjectId>();

        public List<NumRangeSkill> PermittedSkills { get; private set; }

        [BsonIgnore]
        [JsonIgnore]
        public Lazy<Task<IEnumerable<User>>> Instructors { get; private set; }
        [BsonElement("Instructors")]
        public HashSet<ObjectId> InstructorIds { get; private set; } = new HashSet<ObjectId>();

        private Course()
        {
            Owner = new Lazy<Task<User>>(async () => await MongoDBLayer.FindOneById<User>(OwnerId));
            Groups = new Lazy<Task<IEnumerable<Group>>>(async () => await MongoDBLayer.FindById<Group>(GroupIds));
            Instructors = new Lazy<Task<IEnumerable<User>>>(async () => await MongoDBLayer.FindById<User>(InstructorIds));
        }


        public Course(User owner, String name, String description, GroupPreferences preferences, List<NumRangeSkill> skills = null) : this()
        {
            Name = name;
            Preferences = preferences;
            OwnerId = owner.IdStr;
            Description = description;
            PermittedSkills = skills ?? new List<NumRangeSkill>();
        }

        public void AddGroup(Group group)
        {
            GroupIds.Add(group.Id);
        }

        public void RemoveGroup(Group group)
        {
            GroupIds.Remove(group.Id);
        }

        // TODO: Add tests
        public void AddInstructor(User instructor)
        {
            InstructorIds.Add(instructor.Id);
        }

        public void RemoveInstructor(User instructor)
        {
            InstructorIds.Remove(instructor.Id);
        }

        public override async Task Save()
        {
            await MongoDBLayer.UpsertOne(this);
        }

        public static async Task<Course> Load(ObjectId id)
        {
            return await MongoDBLayer.FindOneById<Course>(id);
        }

        public static async Task<Course> Load(String id)
        {
            return await MongoDBLayer.FindOneById<Course>(id);
        }

        public override async Task Delete()
        {
            await MongoDBLayer.DeleteOne(this);
        }
    }
}

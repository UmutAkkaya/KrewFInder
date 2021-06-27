using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using Skills;

namespace DatabaseLayer.Models
{
    public enum MergerInvitationStatus
    {
        Invited, Sent, Revoked, Rejected, Accepted, Suggested, Unknown
    }

    [CollectionName("groups")]
    public class Group : DatabaseModel
    {
        public String Name { get; set; }
        public String Bio { get; set; }
        
        /* Flag is true if this group has been merged into another group for this course */
        public bool isMerged { get; set; }

        [BsonIgnore]
        [JsonIgnore]
        public Lazy<Task<Course>> Course { get; private set; }
        [BsonElement("Course")]
        public ObjectId CourseId { get; private set; }

        [BsonIgnore]
        [JsonIgnore]
        public Lazy<Task<IEnumerable<User>>> Members { get; private set; }
        [BsonElement("Members")]
        public ISet<ObjectId> MemberIds { get; private set; } = new HashSet<ObjectId>();

        public LazyLoadDictionary<User, InvitationStatus> Invitations { get; private set; } = new LazyLoadDictionary<User, InvitationStatus>();

        public LazyLoadDictionary<Group,MergerInvitationStatus> MergerInvitations { get; private set; } = new LazyLoadDictionary<Group, MergerInvitationStatus>();

        public List<NumRangeSkill> DesiredSkills { get; set; } = new List<NumRangeSkill>();

        private Group()
        {
            Course = CourseId.GetLazyLoader<Course>();
            Members = new Lazy<Task<IEnumerable<User>>>(async () => await MongoDBLayer.FindById<User>(MemberIds));
        }

        public Group(String name, String bio, ObjectId courseId) : this()
        {
            Name = name;
            Bio = bio;
            CourseId = courseId;
        }

        public Group(String name, String bio, Course course) : this(name, bio, course.Id)
        {

        }

        public void AddMembers(IEnumerable<ObjectId> members)
        {
            members.All(objId => MemberIds.Add(objId));
        }

        public void RemoveMembers(IEnumerable<ObjectId> members)
        {
            members.All(objId => MemberIds.Remove(objId));
        }

        public void AddMember(User user)
        {
            AddMember(user.Id);
        }

        public void AddMember(ObjectId userId)
        {
            MemberIds.Add(userId);
        }

        public void RemoveMember(User user)
        {
            RemoveMember(user.Id);
        }

        public void RemoveMember(ObjectId userId)
        {
            MemberIds.Remove(userId);
        }

        public static async Task<Group> Load(ObjectId id)
        {
            return await MongoDBLayer.FindOneById<Group>(id);
        }

        public static async Task<Group> Load(String id)
        {
            return await MongoDBLayer.FindOneById<Group>(id);
        }

        public override async Task Save()
        {
            await MongoDBLayer.UpsertOne(this);
        }

        public override async Task Delete()
        {
            await MongoDBLayer.DeleteOne(this);
        }

        public override Boolean Equals(object obj)
        {
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;

            Group group = (Group) obj;
            /* Todo: override equals/GetHashCode for all models to check list equalities */
            return
                (Name == group.Name) &&
                (Bio == group.Bio) &&
                (isMerged == group.isMerged) &&
                (MemberIds.Count == group.MemberIds.Count) &&
//                (MemberIds.All(group.MemberIds.Contains)) &&
                (DesiredSkills.Count == group.DesiredSkills.Count)
//                (DesiredSkills.All(group.DesiredSkills.Contains))
                ;
        }

        public override int GetHashCode()
        {
            //http://www.loganfranken.com/blog/692/overriding-equals-in-c-part-2/
            unchecked
            {
                // Choose large primes to avoid hashing collisions
                const int HashingBase = (int) 2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Name) ? Name.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Bio) ? Bio.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^
                       (!Object.ReferenceEquals(null, isMerged) ? isMerged.GetHashCode() : 0);
                return hash;
            }
        }
    }
}
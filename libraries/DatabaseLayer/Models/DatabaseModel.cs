using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using Newtonsoft.Json;

namespace DatabaseLayer.Models
{
    // Two models are compared and hashed by their ID.
    public abstract class DatabaseModel : IEqualityComparer<DatabaseModel>
    {
        [BsonId]
        public ObjectId Id { get; private set; }

        // ObjectId gets serialized in its component rather than whole string
        public String IdStr { get; private set; }

        public abstract Task Save();
        public abstract Task Delete();

        protected DatabaseModel()
        {
            Id = ObjectId.GenerateNewId();
            IdStr = Id.ToString();
        }

        public bool Equals(DatabaseModel x, DatabaseModel y)
        {
            if (x is null || y is null) return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(DatabaseModel obj)
        {
            return obj.Id.GetHashCode();
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            DatabaseModel otherModel = obj as DatabaseModel;
            return !ReferenceEquals(otherModel, null) && Id == otherModel.Id;
        }

    }

    // Class for storing pairs of Documents mapped to other items. I.e. User : InvitationStatus
    // This should always have Reference initialized, but not necessarily the Document
    public class ReferenceMapping<T1, T2> where T1 : DatabaseModel
    {
        public ObjectId Reference { get; set; }
        [BsonIgnore] private T1 _document;
        [BsonIgnore]
        public T1 Document
        {
            get { return _document ?? (_document = MongoDBLayer.FindOneById<T1>(Reference).Result); }
            set => _document = value;
        }
        public T2 Item { get; set; }

        public ReferenceMapping(T1 document, T2 item)
        {
            Reference = document.Id;
            Document = document;
            Item = item;
        }

        public ReferenceMapping(ReferenceMapping<T1, T2> uninitializedMapping, T1 document)
        {
            Reference = uninitializedMapping.Reference;
            Item = uninitializedMapping.Item;
            Document = document;
        }
    }

    public static class ObjectIdExtension
    {
        public static Lazy<Task<T>> GetLazyLoader<T>(this ObjectId id) where T : DatabaseModel
        {
            return new Lazy<Task<T>>(async () => await MongoDBLayer.FindOneById<T>(id));
        }

    }

    // Just like a normal dictionary, except that it loads documents from database when it's actually needed
    // The idea is that only Obj ID map is serialized, but not cache, which can be reinitialized with either individual items or wholly
    public class LazyLoadDictionary<T1, T2> where T1 : DatabaseModel
    {
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        private IDictionary<ObjectId, T2> _idDictionary = new Dictionary<ObjectId, T2>();

        [BsonIgnore]
        [JsonIgnore]
        private IDictionary<T1, T2> _cache = new Dictionary<T1, T2>();

        public LazyLoadDictionary()
        {
        }

        // Reload cache from database
        public void Initialize()
        {
            if (_cache.Count == _idDictionary.Count) return;

            foreach (var document in MongoDBLayer.FindById<T1>(_idDictionary.Keys).Result)
            {
                _cache[document] = _idDictionary[document.Id];
            }
        }

        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        {
            Initialize();
            return _cache.GetEnumerator();
        }

        public void Add(KeyValuePair<T1, T2> item)
        {
            _idDictionary.Add(new KeyValuePair<ObjectId, T2>(item.Key.Id, item.Value));
            _cache.Add(item);
        }

        public void Clear()
        {
            _idDictionary.Clear();
            _cache.Clear();
        }

        public bool Contains(KeyValuePair<T1, T2> item)
        {
            return _idDictionary.Contains(new KeyValuePair<ObjectId, T2>(item.Key.Id, item.Value));
        }

        public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex)
        {
            Initialize();
            _cache.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<T1, T2> item)
        {
            bool result = _idDictionary.Remove(new KeyValuePair<ObjectId, T2>(item.Key.Id, item.Value));
            _cache.Remove(item);
            return result;
        }

        public int Count => _idDictionary.Count();
        public bool IsReadOnly { get; }
        public void Add(T1 key, T2 value)
        {
            _idDictionary.Add(key.Id, value);
            _cache.Add(key, value);
        }

        public bool ContainsId(ObjectId id)
        {
            return _idDictionary.ContainsKey(id);
        }

        public bool ContainsId(String id)
        {
            return ObjectId.TryParse(id, out var objectId) && ContainsId(objectId);
        }

        public bool ContainsKey(T1 key)
        {
            return _idDictionary.ContainsKey(key.Id);
        }

        public bool Remove(T1 key)
        {
            bool success = _idDictionary.Remove(key.Id);
            _cache.Remove(key);
            return success;
        }

        public bool TryGetValue(T1 key, out T2 value)
        {
            return _idDictionary.TryGetValue(key.Id, out value);
        }

        public T2 this[T1 key]
        {
            get => _idDictionary[key.Id];
            set
            {
                _idDictionary[key.Id] = value;
                _cache[key] = value;
            }
        }

        public ICollection<T1> Keys
        {
            get
            {
                Initialize();
                return _cache.Keys;
            }
        }
        public ICollection<T2> Values
        {
            get
            {
                return _idDictionary.Values;
            }
        }
    }
}

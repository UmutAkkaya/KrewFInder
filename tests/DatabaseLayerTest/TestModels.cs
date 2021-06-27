using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using DatabaseLayer;
using DatabaseLayer.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace DatabaseLayerTest
{
    // Dummy models so that we don't depend on actual models
    [CollectionName("testmodel1")]
    class TestModel1 : DatabaseModel
    {
        public String Str1 { get; set; }
        public int Int1 { get; set; }

        [BsonElement("Model2Reference1")]
        private ObjectId _model2Reference1;
        [BsonIgnore]
        public Lazy<Task<TestModel2>> Model2Reference1 { get; private set; }

        public LazyLoadDictionary<TestModel2, int> ModelIntMap { get; private set; } = new LazyLoadDictionary<TestModel2, int>();

        private TestModel1()
        {
            Model2Reference1 = new Lazy<Task<TestModel2>>(async () => await MongoDBLayer.FindOneById<TestModel2>(_model2Reference1));
        }

        public TestModel1(String str1, int int1) : this()
        {
            Str1 = str1;
            Int1 = int1;
        }

        public void SetModel2Reference(TestModel2 document)
        {
            _model2Reference1 = document.Id;
        }

        public override async Task Save()
        {
            await MongoDBLayer.UpsertOne(this);
        }

        public static async Task<TestModel1> Load(ObjectId id)
        {
            return await MongoDBLayer.FindOneById<TestModel1>(id);
        }

        public override async Task Delete()
        {
            await MongoDBLayer.DeleteOne(this);
        }
    }

    [CollectionName("testmodel2")]
    class TestModel2 : DatabaseModel
    {
        public String Str1;
        public int Int1;

        [BsonElement("Model1Reference1")]
        private ObjectId _model1Reference1;
        [BsonIgnore]
        public Lazy<Task<TestModel1>> Model1Reference1 { get; private set; }

        public ISet<int> IntSet { get; set; } = new HashSet<int>();

        [BsonElement("Model1References")]
        private List<ObjectId> _model1References = new List<ObjectId>();
        [BsonIgnore]
        public Lazy<Task<IEnumerable<TestModel1>>> Model1References { get; private set; }

        private TestModel2()
        {
            Model1Reference1 = new Lazy<Task<TestModel1>>(async () => await MongoDBLayer.FindOneById<TestModel1>(_model1Reference1));
            Model1References = new Lazy<Task<IEnumerable<TestModel1>>>(async () => await MongoDBLayer.FindById<TestModel1>(_model1References));
        }

        public TestModel2(String str1, Int32 int1) : this()
        {
            Str1 = str1;
            Int1 = int1;
        }

        public void AddModel1Reference(TestModel1 document)
        {
            _model1References.Add(document.Id);
        }

        public void RemoveModel1Reference(TestModel1 document)
        {
            _model1References.Remove(document.Id);
        }

        public void SetModel1Reference(TestModel1 document)
        {
            _model1Reference1 = document.Id;
        }

        public override async Task Save()
        {
            await MongoDBLayer.UpsertOne(this);
        }

        public static async Task<TestModel2> Load(ObjectId id)
        {
            return await MongoDBLayer.FindOneById<TestModel2>(id);
        }

        public override async Task Delete()
        {
            await MongoDBLayer.DeleteOne(this);
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using MongoDB.Bson;

namespace DatabaseLayerTest
{
    [TestClass]
    public class SaveLoadTest : DatabaseTest
    {
        private const String _str1 = "You must construct additional test cases";
        private const int _int1 = 42;

        private TestModel1 _testModel1;

        private const String _str2 = "The unseen bug is the deadliest";
        private const int _int2 = -76;

        private TestModel2 _testModel2;

        private void InitTestModels()
        {
            _testModel1 = new TestModel1(_str1, _int1);
            _testModel2 = new TestModel2(_str2, _int2);
        }

        [TestMethod]
        public void CheckDefaultId()
        {
            InitTestModels();

            // Ensure new objects come out with non-default ID
            Assert.AreNotEqual(_testModel1.Id, new ObjectId());
        }

        [TestMethod]
        public async Task SaveLoadSingleModel()
        {
            InitTestModels();
            await _testModel1.Save();

            var testModel2 = await MongoDBLayer.FindOneById<TestModel1>(_testModel1.Id);
            Assert.AreEqual(testModel2.Str1, _testModel1.Str1);
            Assert.AreEqual(testModel2.Int1, _testModel1.Int1);

        }

        [TestMethod]
        public async Task CheckLazyLoaderInitialized()
        {
            InitTestModels();
            // Ensure lazy loaders are initialized upon object construction (even if not loaded from DB)
            Assert.IsNull(await _testModel1.Model2Reference1.Value);
        }

        [TestMethod]
        public async Task SaveLoadModifySingle()
        {
            InitTestModels();
            await _testModel1.Save();

            var testModel2 = await MongoDBLayer.FindOneById<TestModel1>(_testModel1.Id);
            testModel2.Str1 = _str2;
            await testModel2.Save();

            var testModel3 = await MongoDBLayer.FindOneById<TestModel1>(_testModel1.Id);
            Assert.AreEqual(testModel3.Str1, _str2);
            Assert.AreEqual(testModel3.Int1, _int1);

        }

        [TestMethod]
        public async Task SaveLoadSet()
        {
            int testNum = 3;
            InitTestModels();
            _testModel2.IntSet.Add(testNum);
            await _testModel2.Save();
            var testModel2Loaded = await TestModel2.Load(_testModel2.Id);

            Assert.IsTrue(testModel2Loaded.IntSet.Contains(testNum));
            Assert.IsFalse(testModel2Loaded.IntSet.Contains(666));
        }

        [TestMethod]
        public async Task LazyLoadSingleValue()
        {
            InitTestModels();

            _testModel1.SetModel2Reference(_testModel2);
            await _testModel1.Save();
            await _testModel2.Save();

            TestModel1 model1Reloaded = await TestModel1.Load(_testModel1.Id);
            TestModel2 model2Referenced = await model1Reloaded.Model2Reference1.Value;

            Assert.AreEqual(model2Referenced.Str1, _str2);
            Assert.AreEqual(model2Referenced.Int1, _int2);
        }

        [TestMethod]
        public async Task LazyLoadNullReference()
        {
            InitTestModels();
            await _testModel1.Save();

            var testModel1 = await TestModel1.Load(_testModel1.Id);
            Assert.IsNull(await testModel1.Model2Reference1.Value);
        }

        [TestMethod]
        public async Task LazyLoadCircularReference()
        {
            InitTestModels();

            _testModel1.SetModel2Reference(_testModel2);
            _testModel2.SetModel1Reference(_testModel1);
            await _testModel1.Save();
            await _testModel2.Save();

            var testModel1 = await TestModel1.Load(_testModel1.Id);
            var testModel2 = await testModel1.Model2Reference1.Value;
            var testModel3 = await testModel2.Model1Reference1.Value; // Back to square one

            Assert.AreEqual(testModel3.Str1, _str1);
            Assert.AreEqual(testModel3.Int1, _int1);
        }

        [TestMethod]
        public void LazyDictionaryNoSaveTest()
        {
            InitTestModels();
            int tstInt = 25;
            _testModel1.ModelIntMap[_testModel2] = tstInt;
            Assert.AreEqual(_testModel1.ModelIntMap[_testModel2], tstInt);
        }

        [TestMethod]
        public async Task LazyDictionarySaveLoadTest()
        {
            InitTestModels();
            int tstInt = 694;
            _testModel1.ModelIntMap[_testModel2] = tstInt;
            await _testModel1.Save();
            var testModel1Reloaded = await TestModel1.Load(_testModel1.Id);
            Assert.AreEqual(testModel1Reloaded.ModelIntMap[_testModel2], tstInt);
        }

        [TestMethod]
        public async Task LazyDictionaryInitializerTest()
        {
            InitTestModels();
            int tstInt = -2340;
            _testModel1.ModelIntMap[_testModel2] = tstInt;
            await _testModel1.Save();
            await _testModel2.Save();
            var testModel1Reloaded = await TestModel1.Load(_testModel1.Id);
            var testModel2Referenced = testModel1Reloaded.ModelIntMap.Keys.ToList()[0];
            Assert.AreEqual(testModel2Referenced, _testModel2);
            Assert.AreEqual(testModel2Referenced.Str1, _str2);
        }

        [TestMethod]
        public async Task LazyListSaveLoadEmpty()
        {
            InitTestModels();
            await _testModel2.Save();
            var testModel2Reloaded = await TestModel2.Load(_testModel2.Id);

            Assert.IsTrue(!(await testModel2Reloaded.Model1References.Value).Any());
        }

        [TestMethod]
        public async Task LazyListSaveLoadOne()
        {
            InitTestModels();
            await _testModel1.Save();
            _testModel2.AddModel1Reference(_testModel1);
            await _testModel2.Save();

            var testModel2Reloaded = await TestModel2.Load(_testModel2.Id);
            var model1ListReloaded = (await testModel2Reloaded.Model1References.Value).ToList();
            Assert.AreEqual(model1ListReloaded[0].Str1, _str1);
            Assert.AreEqual(model1ListReloaded[0].Int1, _int1);
        }


        [TestMethod]
        public async Task LazyListSaveLoadMany()
        {
            InitTestModels();
            var tstInt = 366;
            var tstStr = "Are we there yet?";

            var tstModel1 = new TestModel1(tstStr, tstInt);
            await _testModel1.Save();
            await tstModel1.Save();

            _testModel2.AddModel1Reference(_testModel1);
            _testModel2.AddModel1Reference(tstModel1);
            await _testModel2.Save();

            var testModel2Reloaded = await TestModel2.Load(_testModel2.Id);
            var model1References = (await testModel2Reloaded.Model1References.Value).ToList();
            Assert.AreEqual(model1References[0].Str1, _str1);
            Assert.AreEqual(model1References[1].Int1, tstInt);
        }

        [TestMethod]
        public async Task LazyListNoSaveTest()
        {
            InitTestModels();
            Assert.IsTrue((await _testModel2.Model1References.Value).Count() == 0);
        }
    }
}

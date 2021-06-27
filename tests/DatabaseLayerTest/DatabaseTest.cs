﻿using System;
using System.Collections.Generic;
using System.Text;
using DatabaseLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DatabaseLayerTest
{
    public abstract class DatabaseTest
    {

        private string _databaseNameCache;
        private string _testingDatabase
        {
            get
            {
                return _databaseNameCache ?? (_databaseNameCache =
                           "test_" + (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
            }
        }

        [TestInitialize()]
        public void Initialize()
        {

            if (Environment.GetEnvironmentVariable("DAKREW_USE_LOCALHOST") != null)
                MongoDBLayer.InitializeDatabase("mongodb://localhost/test", _testingDatabase);
            else
                MongoDBLayer.InitializeDatabase("ec2-18-233-156-157.compute-1.amazonaws.com", MongoDBLayer.DEFAULT_PORT, "admin", "h7WsssrE3sseqkHb4DssAGrv9CQ9c", "admin", _testingDatabase);

            MongoDBLayer.DropTestDatabase(_testingDatabase);
        }



        [TestCleanup()]
        public void Cleanup()
        {
            MongoDBLayer.DropTestDatabase(_testingDatabase);
        }
    }
}

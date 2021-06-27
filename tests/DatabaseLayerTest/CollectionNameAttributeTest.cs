using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseLayer;
using DatabaseLayer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace DatabaseLayerTest
{
    // This test ensures that all models have marked the desired collection name
    [TestClass]
    public class CollectionNameAttributeTest
    {
        [TestMethod]
        public void CheckAttributes()
        {
            var subclasses =
                from type in Assembly.GetAssembly(typeof(DatabaseModel)).GetTypes()
                where type.IsSubclassOf(typeof(DatabaseModel))
                select type;

            foreach (var type in subclasses)
            {
                if (!(type.GetCustomAttributes(typeof(CollectionName)).FirstOrDefault() is CollectionName collectionNameAttribute))
                {
                    throw new InvalidModelException(
                        String.Format(
                            "Type {0} does not appear to be a valid model. Models must specify CollectionName attribute", nameof(type))
                    );
                }
            }
        }
    }
}

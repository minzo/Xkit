using Corekit.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Corekit.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestInitialize]
        public void Initialize()
        {

        }

        [TestMethod]
        public void RevisionControlCollection()
        {
            var collection = new RevisionControlCollection<int>(4, 2);
            collection.Add(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10 }));

            collection.Add(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11 }));

            collection.Add(12);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11, 12 }));

            collection.Remove(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, default, 12 }));

            collection.Add(13);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 13, 12 }));
        }


        [TestMethod]
        public void DynamicProperty()
        {
            for (var i = 0; i < _count; i++)
            {
                new DynamicProperty<float>(_DynamicDefinition);
            }
        }

        private int _count = 10000000;
        private IDynamicPropertyDefinition _DynamicDefinition = new DynamicPropertyDefinition<float>();
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Tests
{
    [TestClass]
    public class XmlSerializerExtension
    {
        [TestInitialize]
        public void Initialize()
        {

        }

        [TestCleanup]
        public void Cleanup()
        {
            if (System.IO.File.Exists(XmlPath))
            {
                System.IO.File.Delete(XmlPath);
            }
        }

        public class Simple
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }


        [TestMethod]
        public void SerializeAndDeserialize()
        {
            var source = new Simple() { Name = "Test", Value = "Test" };

            Corekit.Extensions.XmlSerializerExtensions.SerializeXml(source, XmlPath);
            Assert.IsTrue(System.IO.File.Exists(XmlPath));

            var target = Corekit.Extensions.XmlSerializerExtensions.DeserializeXml<Simple>(XmlPath);
            Assert.AreEqual(source.Name, target.Name);
            Assert.AreEqual(source.Value, target.Value);
        }

        public readonly string XmlPath = "Test.xml";
    }
}

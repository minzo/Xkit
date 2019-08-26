using Microsoft.VisualStudio.TestTools.UnitTesting;
using Corekit.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Corekit.Extensions.Tests
{
    [TestClass]
    public class StringExtensions
    {
        private readonly Dictionary<string, string> Variables = new Dictionary<string, string>();

        [TestInitialize]
        public void Initialize()
        {
            Variables.Add("Material", "マテリアル");
            Variables.Add("SubMaterial", "サブマテリアル");
            Variables.Add("RootDir", @"C:\Windows");
        }

        [TestMethod]
        public void ExpandVariables()
        {
            var str0 = "Test_{Material}";
            var ret0 = "Test_マテリアル";
            Assert.IsTrue(ret0 == str0.ExpandVariables("{", "}", Variables));

            var str1 = "%RootDir%";
            var ret1 = @"C:\Windows";
            Assert.IsTrue(ret1 == str1.ExpandVariables("%", "%", Variables));

            var str2 = "Test_${Material}_${SubMaterial}";
            var ret2 = "Test_マテリアル_サブマテリアル";
            Assert.IsTrue(ret2 == str2.ExpandVariables("${", "}", Variables));
        }
    }
}

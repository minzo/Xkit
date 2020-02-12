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
            this.Variables.Clear();
            this.Variables.Add("Material", "マテリアル");
            this.Variables.Add("SubMaterial", "サブマテリアル");
            this.Variables.Add("RootDir", @"C:\Windows");
        }

        [TestMethod]
        public void ExpandVariables()
        {
            var str0 = "Test_{Material}";
            var ret0 = "Test_マテリアル";
            Assert.IsTrue(ret0 == str0.ExpandVariables("{", "}", this.Variables));

            var str1 = "%RootDir%";
            var ret1 = @"C:\Windows";
            Assert.IsTrue(ret1 == str1.ExpandVariables("%", "%", this.Variables));

            var str2 = "Test_${Material}_${SubMaterial}";
            var ret2 = "Test_マテリアル_サブマテリアル";
            Assert.IsTrue(ret2 == str2.ExpandVariables("${", "}", this.Variables));
        }

        [TestMethod]
        public void Substring()
        {
            Assert.IsTrue("Main" == "[Main]".Substring("[", "]"));

            Assert.IsTrue("Main"== "<Main>Main</Main>".Substring("<Main>", "</Main>"));

            Assert.IsTrue("Main" == "<<Main>>Main</Main>".Substring("<<Main>>", "</Main>"));
        }

        [TestMethod]
        public void SubtringWith()
        {
            Assert.IsTrue("[Main]" == "[Main]:CONTENT".SubstringWith("[", "]"));

            Assert.IsTrue("<Main>Main</Main>" == "<Main>Main</Main>".SubstringWith("<Main>", "</Main>"));

            Assert.IsTrue("<<Main>>Main</Main>" == "<<Main>>Main</Main>".SubstringWith("<<Main>>", "</Main>"));
        }
    }
}

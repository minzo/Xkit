using Corekit.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Tests
{
    [TestClass]
    public class InheritanceObject
    {
        public InheritanceObject()
        {
            this._Manager = new Models.InheritanceObjectManager();
        }

        [TestInitialize]
        public void Initialize()
        {
            var sub = this._Manager.CreateTypeInfo("Vector2f");
            sub.AddPrpoperty("X", this._Manager.GetTypeInfo(Corekit.Models.InheritanceObjectBuildInTypeName.F32));
            sub.AddPrpoperty("Y", this._Manager.GetTypeInfo(Corekit.Models.InheritanceObjectBuildInTypeName.F32));

            var info = this._Manager.CreateTypeInfo("Test");
            info.AddPrpoperty("String0", this._Manager.GetTypeInfo(Corekit.Models.InheritanceObjectBuildInTypeName.String));
            info.AddPrpoperty("String1", this._Manager.GetTypeInfo(Corekit.Models.InheritanceObjectBuildInTypeName.String));
            info.AddPrpoperty("String2", this._Manager.GetTypeInfo(Corekit.Models.InheritanceObjectBuildInTypeName.String));
            info.AddPrpoperty("String3", this._Manager.GetTypeInfo(Corekit.Models.InheritanceObjectBuildInTypeName.String));
            info.AddPrpoperty("Vector2", this._Manager.GetTypeInfo("Vector2f"));
        }

        [TestMethod]
        public void Trial()
        {
            // あくまでコレクションクラスであり配列ではない
            Assert.IsFalse(typeof(List<int>).IsArray);
            Assert.IsFalse(typeof(ObservableCollection<int>).IsArray);
            Assert.IsFalse(typeof(Dictionary<string, string>).IsArray);
            Assert.IsFalse(typeof(HashSet<string>).IsArray);
            Assert.IsTrue(typeof(int[]).IsArray);

            // ポインタ型
            Assert.IsTrue(typeof(int*).IsPointer);

            // ポインタ型ではない
            Assert.IsFalse(typeof(List<Corekit.Models.InheritanceObject>).IsPointer);

            // 参照型でもない
            Assert.IsFalse(typeof(List<int>).IsByRef);
            Assert.IsFalse(typeof(ObservableCollection<int>).IsByRef);
            Assert.IsFalse(typeof(Dictionary<string, string>).IsByRef);
            Assert.IsFalse(typeof(HashSet<string>).IsByRef);
            Assert.IsFalse(typeof(List<Corekit.Models.InheritanceObject>).IsByRef);
        }

        [TestMethod]
        public void InheritanceTest()
        {
            var info = this._Manager.CreateTypeInfo("Test");
            var io = new Corekit.Models.InheritanceObject(info);

            {
                var p = io.GetProperty("String0");
                p.Value = "Hoge0";
                Assert.AreEqual(p.Value, "Hoge0");
            }

            {
                var p = io.GetProperty("String1");
                p.Value = "Hoge1";
                Assert.AreEqual(p.Value, "Hoge1");
            }

            {
                var p0 = io.GetProperty("String0");
                var p1 = io.GetProperty("String1");
                Assert.AreNotEqual(p0.Value, p1.Value);
            }

            {


            }

        }

        [TestMethod]
        public void CreateInheritanceObject()
        {
            var info = new Corekit.Models.InheritanceObjectTypeInfo("Test");
            var io = new Corekit.Models.InheritanceObject(info);
        }

        private readonly Corekit.Models.InheritanceObjectManager _Manager;
    }
}

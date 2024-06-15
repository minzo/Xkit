using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Tests
{
    [TestClass]
    public class InheritanceObject
    {
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

            var array = new Corekit.Models.InheritanceObject[8];
            array[0].SetValue(10);
        }

        [TestMethod]
        public void NewInheritanceObject()
        {
            var info = new Corekit.Models.InheritanceObjectTypeInfo("Test");
            var io = new Corekit.Models.InheritanceObject(info);
        }
    }
}

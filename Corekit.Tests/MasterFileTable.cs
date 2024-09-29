using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Tests
{
    [TestClass]
    public class MasterFileTable
    {
        [TestMethod]
        public void EnumerateFiles()
        {
            var list = Corekit.IO.File.EnumerateFiles("D:");
            foreach (var entity in list)
            {
                Console.WriteLine(entity.Path);
            }
            new EnumerateVolume.PInvokeWin32() { Drive = "D:" }.EnumerateVolume(out var hoge, new[] { "*" });
        }
    }
}

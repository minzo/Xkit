using Microsoft.VisualStudio.TestTools.UnitTesting;
using Corekit.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Corekit.Extensions.Tests
{
    [TestClass]
    public class PathUtil
    {
        [TestMethod]
        public void GetFileNameWithoutAllExtensions()
        {
            var filePath0 = @"file.txt.zip";
            Assert.AreEqual(filePath0.GetFileNameWithoutAllExtensions(), "file");

            var filePath1 = @"file.txt.zip.bin";
            Assert.AreEqual(filePath1.GetFileNameWithoutAllExtensions(), "file");
        }
    }
}

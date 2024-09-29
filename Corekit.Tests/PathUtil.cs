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

        [TestMethod]
        public void GetRelativePath()
        {
            {
                var dirPath = @"C:\home\TestDir\";
                var filePath = @"C:\home\TestDir\TestData.bin";
                var relativePath = filePath.GetRelativePath(dirPath);
                Assert.AreEqual(relativePath, "TestData.bin");
            }

            {
                var dirPath = @"C:\テスト\";
                var filePath = @"C:\テスト\TestData.bin";
                var relativePath = filePath.GetRelativePath(dirPath);
                Assert.AreEqual(relativePath, "TestData.bin");
            }
        }

        [TestMethod]
        public void GetUnixPath()
        {
            var filePath = @"C:\home\TestDir\TestData.bin";
            var unixPath = @"C:/home/TestDir/TestData.bin";

            var path = filePath.GetUnixPath();
            Assert.AreEqual(path, unixPath);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corekit.Extensions.Tests
{
    [TestClass]
    public class PathUtil
    {
        [TestMethod]
        public void GetFileNameWithoutAllExtensions()
        {
            var filePath0 = @"file.txt.zip";
            Assert.AreEqual("file", filePath0.GetFileNameWithoutAllExtensions());

            var filePath1 = @"file.txt.zip.bin";
            Assert.AreEqual("file", filePath1.GetFileNameWithoutAllExtensions());
        }

        [TestMethod]
        public void GetRelativePath()
        {
            {
                var dirPath = @"C:\home\TestDir";
                var filePath = @"C:\home\TestDir\TestData.bin";
                var relativePath = filePath.GetRelativePath(dirPath);
                Assert.AreEqual("TestData.bin", relativePath);
            }

            {
                var dirPath = @"D:\home\TestDir\";
                var filePath = @"D:\home\TestDir\TestData.bin";
                var relativePath = filePath.GetRelativePath(dirPath);
                Assert.AreEqual("TestData.bin", relativePath);
            }

            {
                var dirPath = @"E:\テスト\";
                var filePath = @"E:\テスト\TestData.bin";
                var relativePath = filePath.GetRelativePath(dirPath);
                Assert.AreEqual("TestData.bin", relativePath);
            }
        }

        [TestMethod]
        public void GetUnixPath()
        {
            var filePath = @"C:\home\TestDir\TestData.bin";
            var unixPath = @"C:/home/TestDir/TestData.bin";

            var path = filePath.GetUnixPath();
            Assert.AreEqual(unixPath, path);
        }
    }
}

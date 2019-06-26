using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Corekit.Tests
{
    [TestClass]
    public class JobManagerTest
    {
        [TestMethod]
        public void JobRunningTest()
        {
            using (var manager = new JobManager())
            {
                for (int i = 0; i < 20; i++)
                {
                    manager.Request(Job);
                }

                do
                {
                    System.Threading.Thread.Sleep(millisec);
                }
                while (manager.IsRunning);
            }
        }

        [TestMethod]
        public void JobSingleRunningTest()
        {
            using (var manager = new JobManager(1))
            {
                for (int i = 0; i < 10; i++)
                {
                    manager.Request(Job);
                }

                do
                {
                    System.Threading.Thread.Sleep(millisec);
                }
                while (manager.IsRunning);
            }
        }

        [TestMethod]
        public void JobCancelTest()
        {
            var manager = new JobManager();

            for (int i = 0; i < 100; i++)
            {
                manager.Request(Job);
            }

            for (int i = 0; i < 5; i++)
            {
                System.Threading.Thread.Sleep(1000);
            }

            manager.Dispose();

            Assert.IsTrue(!manager.IsRunning);
        }

        private void Job()
        {
            System.Threading.Thread.Sleep(millisec * random.Next(10));
        }

        private readonly Random random = new Random(100);
        private readonly int millisec = 1000;
        private JobManager jobManager;
    }
}

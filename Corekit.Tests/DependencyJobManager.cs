using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Corekit.Tests
{
    [TestClass]
    public class DependencyJobManagerTest
    {
        [TestMethod]
        public void JobRunningTest()
        {
            using (var manager = new DependencyJobManager())
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

        private void Job()
        {
            System.Threading.Thread.Sleep(millisec * random.Next(10));
        }

        private readonly Random random = new Random(100);
        private readonly int millisec = 1000;
    }
}

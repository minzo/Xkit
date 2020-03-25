using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Tests
{
    [TestClass]
    public class Job
    {
        [TestMethod]
        public void JobRunningTest()
        {
            using(var manager = new Corekit.Worker.JobManager())
            {
                var list = new List<Corekit.Worker.Job>();
                var result = new List<int>();
                var rand = new Random(1);

                for (int i = 0; i < 1000; i++)
                {
                    var count = i;
                    list.Add(new Corekit.Worker.Job(() => {
                        result.Add(count);
                        System.Threading.Thread.Sleep(rand.Next(5, 200));
                    }, manager));
                }

                for (int i = 0; i < list.Count - 1; i++)
                {
                    list[i].DependsOn(list[i + 1]);
                }

                foreach (var job in list)
                {
                    job.RequestStart();
                    System.Threading.Thread.Sleep(1);
                }

                do
                {
                    System.Threading.Thread.Sleep(millisec);
                }
                while (manager.IsRunning);

                Assert.IsTrue(result.SequenceEqual(Enumerable.Range(0, 1000).Reverse().ToList()));
            }
        }

        [TestMethod]
        public void RequestStart()
        {
            using (var manager = new Corekit.Worker.JobManager())
            {
                var result = new List<int>();
                var job = new Corekit.Worker.Job(() => result.Add(0), manager);
                Parallel.For(0, 10, _ => job.RequestStart());
                do
                {
                    System.Threading.Thread.Sleep(millisec);
                }
                while (manager.IsRunning);
                Assert.IsTrue(result.Count == 1);
            }
        }

        private readonly int millisec = 1000;
    }
}

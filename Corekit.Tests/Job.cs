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

                for (int i = 0; i < 20; i++)
                {
                    var count = i;
                    list.Add(new Corekit.Worker.Job(() => {
                        result.Add(count);
                        System.Threading.Thread.Sleep(100);
                    }, manager));
                }

                for (int i = 0; i < list.Count - 1; i++)
                {
                    list[i].DependsOn(list[i + 1]);
                }

                foreach(var job in list)
                {
                    job.RequestStart();
                    System.Threading.Thread.Sleep(1);
                }

                do
                {
                    System.Threading.Thread.Sleep(millisec);
                }
                while (manager.IsRunning);

                Assert.IsTrue(result.SequenceEqual(Enumerable.Range(0, 20).Reverse().ToList()));
            }
        }

        public void RequestStartTest()
        {
            using (var manager = new Corekit.Worker.JobManager())
            {
                var job = new Corekit.Worker.Job(() => {
                }, manager);
                Parallel.For(0, 10, _ => job.RequestStart());
            }
        }

        private readonly int millisec = 1000;
    }
}

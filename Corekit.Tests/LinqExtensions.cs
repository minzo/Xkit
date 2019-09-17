using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corekit.Extensions.Tests
{
    [TestClass]
    public class LinqExtensions
    {
        [TestMethod]
        public void CrossJoin()
        {
            var numeric = new[] { 1, 2, 3 };
            var abcd    = new[] { "A", "B", "C", "D" };
            var xyz     = new[] { "X", "Y", "Z" };

            var result1 = new[] {
                "1_A", "1_B", "1_C", "1_D",
                "2_A", "2_B", "2_C", "2_D",
                "3_A", "3_B", "3_C", "3_D"
            };

            var isEqual1 = numeric
                .CrossJoin(abcd, (a, b) => $"{a}_{b}")
                .SequenceEqual(result1);

            Assert.IsTrue(isEqual1);

            var result2 = new[] {
                "1_A_X", "1_A_Y", "1_A_Z",
                "1_B_X", "1_B_Y", "1_B_Z",
                "1_C_X", "1_C_Y", "1_C_Z",
                "1_D_X", "1_D_Y", "1_D_Z",

                "2_A_X", "2_A_Y", "2_A_Z",
                "2_B_X", "2_B_Y", "2_B_Z",
                "2_C_X", "2_C_Y", "2_C_Z",
                "2_D_X", "2_D_Y", "2_D_Z",

                "3_A_X", "3_A_Y", "3_A_Z",
                "3_B_X", "3_B_Y", "3_B_Z",
                "3_C_X", "3_C_Y", "3_C_Z",
                "3_D_X", "3_D_Y", "3_D_Z",
            };

            var isEqual2 = numeric
                .CrossJoin(abcd, (a, b) => $"{a}_{b}")
                .CrossJoin(xyz, (a, b) => $"{a}_{b}")
                .SequenceEqual(result2);

            var sequence = numeric.Select(i => i.ToString().AsEnumerable())
                .CrossJoin(abcd, (a, b) => a.Concat(b.AsEnumerable()))
                .CrossJoin(xyz, (a, b) => a.Concat(b.AsEnumerable()));
             
            Assert.IsTrue(isEqual2);
        }
    }
}

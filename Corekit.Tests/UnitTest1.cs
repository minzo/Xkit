using Corekit.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Corekit.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestInitialize]
        public void Initialize()
        {

        }

        [TestMethod]
        public void DynamicProperty()
        {
            for (var i = 0; i < _count; i++)
            {
                new DynamicProperty<float>(_DynamicDefinition);
            }
        }

        private int _count = 10000000;
        private IDynamicPropertyDefinition _DynamicDefinition = new DynamicPropertyDefinition<float>();
    }
}

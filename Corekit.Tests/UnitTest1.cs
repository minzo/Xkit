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

        [TestMethod]
        public void InheritableProperty()
        {
            for (var i = 0; i < _count; i++)
            {
                new InheritableProperty<float>(_InheritableDefinition);
            }
        }

        public void InheritableItem()
        {
            for (var i = 0; i < _count; i++)
            {
                new InheritableItem();
            }
        }

        private int _count = 10000000;
        private IDynamicPropertyDefinition _DynamicDefinition = new DynamicPropertyDefinition<float>();
        private InheritablePropertyDefinition<float> _InheritableDefinition = new InheritablePropertyDefinition<float>();
    }
}

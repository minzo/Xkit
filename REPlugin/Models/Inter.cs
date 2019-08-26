using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPlugin.Models
{
    public class Inter
    {
        public Inter()
        {
            this._Dictionary = new Dictionary<string, object>();
        }

        public Inter GetProperty(string path)
        {
            return null;
        }

        public Inter TryGetProperty(string path)
        {
            return null;
        }

        public IEnumerable<Inter> GetValidValueAsList(string path = null)
        {
            return null;
        }

        public void SetValue<T>(T value)
        {
        }

        public T GetValue<T>() where T : class
        {
            return null;
        }

        public Inter AddArrayElement()
        {
            return null;
        }

        public Inter AddDicionaryElement(string key)
        {
            return null;
        }

        private Dictionary<string, object> _Dictionary;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPlugin.Models
{
    internal class Inter
    {
        public Inter GetProperty(string path)
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
    }
}

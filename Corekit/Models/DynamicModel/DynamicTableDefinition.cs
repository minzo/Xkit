using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Models
{
    /// <summary>
    /// DynamicTableの枠
    /// </summary>
    public interface IDynamicTableFrame : INotifyPropertyChanged
    {
        string Name { get; }
    }


    /// <summary>
    /// DynamicTableの定義
    /// </summary>
    public class DynamicTableDefinition
    {
        public IEnumerable<IDynamicTableFrame> Rows { get; set; }

        public IEnumerable<IDynamicTableFrame> Cols { get; set; }

        public Action<IDynamicTableFrame, IDynamicTableFrame, IDynamicProperty> CellCreated { get; set; }
    }
}

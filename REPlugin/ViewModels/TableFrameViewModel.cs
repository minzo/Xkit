using Corekit.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPlugin.ViewModels
{
    /// <summary>
    /// デザイナーに都合の良いほう?
    /// </summary>
    public class TableFrameViewModel : IDynamicTableFrame
    {
        /// <summary>
        /// プログラマーが見るほうの候補名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// プロパティ名
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// コンディション
        /// </summary>
        public object Condition { get; }

        public bool? IsReadOnly => false;

        public bool IsDeletable => true;

        public bool IsMovable => true;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }

    /// <summary>
    /// 
    /// </summary>
    public class TableFrame : IDynamicTableFrame
    {
        public string Name { get; set; }

        public bool? IsReadOnly => true;

        public bool IsDeletable => false;

        public bool IsMovable => false;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}

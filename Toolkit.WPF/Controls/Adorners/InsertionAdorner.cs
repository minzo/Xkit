using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace Toolkit.WPF.Controls.Adorners
{
    /// <summary>
    /// 挿入先を表示する Adorner
    /// </summary>
    internal class InsertionAdorner : Adorner, IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InsertionAdorner(UIElement adornedElement)
            : base(adornedElement)
        {

        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

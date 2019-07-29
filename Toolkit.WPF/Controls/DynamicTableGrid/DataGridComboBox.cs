using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Toolkit.WPF.Controls
{
    /// <summary>
    /// DataGrid用のComboBox
    /// </summary>
    internal class DataGridComboBox : ComboBox
    {
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }
    }
}
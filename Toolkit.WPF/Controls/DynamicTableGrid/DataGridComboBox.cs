using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Toolkit.WPF.Controls
{
    /// <summary>
    /// DataGrid用のComboBox
    /// </summary>
    public class DataGridComboBox : ComboBox
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataGridComboBox()
        {
            this.Loaded += this.OnLoaded;
            this.PreviewKeyDown += this.OnPreviewKeyDown;
            this.DropDownClosed += this.OnDropDownClosed;
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.IsDropDownOpen = true;
            this._OwnerDataGrid = EnumerateParent(this).OfType<DataGrid>().FirstOrDefault();
        }

        /// <summary>
        /// OnPreviewKeyDown
        /// </summary>
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.IsDropDownOpen = false;
            }
            else if (e.Key == Key.Enter)
            {
                var ev = new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, Key.Enter) { RoutedEvent = KeyDownEvent };
                InputManager.Current.ProcessInput(ev);
            }
        }

        /// <summary>
        /// OnDropDownClosed
        /// </summary>
        private void OnDropDownClosed(object sender, EventArgs e)
        {
            this._OwnerDataGrid?.CommitEdit(DataGridEditingUnit.Cell, true);
        }

        /// <summary>
        /// VisualParentを列挙する
        /// </summary>
        private static IEnumerable<DependencyObject> EnumerateParent(DependencyObject dp)
        {
            while ((dp = VisualTreeHelper.GetParent(dp)) != null)
            {
                yield return dp;
            }
        }

        private DataGrid _OwnerDataGrid;
    }
}
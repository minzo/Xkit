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
            this.Unloaded += this.OnUnloaded;
            this.PreviewKeyDown += this.OnPreviewKeyDown;
            this.DropDownClosed += this.OnDropDownClosed;
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            EnumerateChildren(this)
                .OfType<Border>()
                .Where(i => i.Name == "templateRoot")
                .FirstOrDefault()
                ?.SetCurrentValue(Border.BackgroundProperty, this.Background);

            this._DataGridColumnOwner = EnumerateParent(this).OfType<DataGridCell>().FirstOrDefault()?.Column;
            this._DataGridOwner = EnumerateParent(this).OfType<DataGrid>().FirstOrDefault();

            if (this._DataGridOwner?.IsReadOnly == true || this._DataGridColumnOwner?.IsReadOnly == true|| this.IsReadOnly)
            {
                return;
            }

            this.IsDropDownOpen = true;
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.IsDropDownOpen = false;
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
            this.GetBindingExpression(ComboBox.TextProperty)?.UpdateSource();
            this._DataGridOwner?.CommitEdit(DataGridEditingUnit.Cell, true);
            this._DataGridOwner?.CommitEdit(DataGridEditingUnit.Row, true);
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

        /// <summary>
        /// VisualChildrenを列挙する
        /// </summary>
        private static IEnumerable<DependencyObject> EnumerateChildren(DependencyObject dp)
        {
            var count = VisualTreeHelper.GetChildrenCount(dp);
            var children = Enumerable.Range(0, count).Select(i => VisualTreeHelper.GetChild(dp, i));
            return children.Concat(children.SelectMany(i => EnumerateChildren(i)));
        }

        private DataGrid _DataGridOwner;
        private DataGridColumn _DataGridColumnOwner;
    }
}
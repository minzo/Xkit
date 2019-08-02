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
    public class DataGridComboBox : ComboBox
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataGridComboBox()
        {
            this.Loaded += this.OnLoaded;
            this.PreviewKeyDown += this.OnPreviewKeyDown;
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.IsDropDownOpen = true;
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
                var ev = new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, Key.Enter);
                InputManager.Current.ProcessInput(ev);
            }
        }
    }
}
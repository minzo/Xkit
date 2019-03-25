using Corekit;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Toolkit.WPF.Controls
{
    /// <summary>
    /// LoggerConsolePanel.xaml の相互作用ロジック
    /// </summary>
    public partial class LoggerConsolePanel : UserControl
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoggerConsolePanel()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, (s, e) => FilterTextBox.Focus(), (s, e) => e.CanExecute = true));
        }
    }
}

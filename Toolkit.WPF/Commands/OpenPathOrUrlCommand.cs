using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;

namespace Toolkit.WPF.Commands
{
    /// <summary>
    /// PathやUrlを開くコマンド
    /// CommandParameter でわたってきたパスやURLを開きます
    /// </summary>
    public class OpenPathOrUrlCommand : MarkupExtension, ICommand
    {
        /// <summary>
        /// 開く
        /// </summary>
        public void Open(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

        /// <summary>
        /// 実行可能か
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// 実行
        /// </summary>
        public void Execute(object parameter)
        {
            var url = parameter?.ToString();
            if (!string.IsNullOrEmpty(url))
            {
                this.Open(url);
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Toolkit.WPF;
using Xkit.Plugins.Sample.Models;

namespace Xkit.Plugins.Sample.ViewModels
{
    /// <summary>
    /// FrameのVM
    /// </summary>
    internal class FrameViewModel
    {
        public IEnumerable<Frame> Frames { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FrameViewModel(IEnumerable<Frame> model)
        {
            this.Frames = model;

            this.AddDefinition = new DelegateCommand(_ => {});
            this.RemoveDefinition = new DelegateCommand(_ => {});
        }

        #region Command

        public ICommand AddDefinition { get; }

        public ICommand RemoveDefinition { get; }

        #endregion
    }
}

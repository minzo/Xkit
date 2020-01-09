using System;
using System.Collections.Generic;
using System.Text;
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
        }
    }
}

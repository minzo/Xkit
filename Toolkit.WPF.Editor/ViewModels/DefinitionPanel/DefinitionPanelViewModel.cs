using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tookit.WPF.Editor.Models;

namespace Tookit.WPF.Editor.ViewModels.DefinitionPanel
{
    internal class DefinitionPanelViewModel<TModel, TViewModel>
    {
        public IReadOnlyList<TViewModel> ViewModels { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DefinitionPanelViewModel(IEnumerable<TModel> models, Func<TModel, TViewModel> factory)
        {
            this.ViewModels = models.Select(i => factory(i)).ToList();
        }
    }
}

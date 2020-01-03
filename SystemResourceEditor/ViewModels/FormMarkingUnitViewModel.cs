using System;
using System.Collections.Generic;
using System.Text;
using System.Resource.Models.Data;
using System.Collections.ObjectModel;
using Corekit.Extensions;

namespace System.Resource.ViewModels
{
    /// <summary>
    /// FormMarkingUnitVM
    /// </summary>
    internal class FormMarkingUnitViewModel
    {
        public string Name { get; } = "FormMarking";

        public string Description { get; } = "";

        public FormMarkingSetDefinitionViewModel FormMarkingSetDefinitionVM { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormMarkingUnitViewModel(System.Resource.Models.Data.Resource resource)
        {
            this._Model = resource.FormMarkingUnit;
            this.FormMarkingSetDefinitionVM = new FormMarkingSetDefinitionViewModel(this._Model);
        }

        private FormMarkingUnit _Model;
    }
}

using Corekit.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace System.Resource.Models.Data
{
    /// <summary>
    /// FormMarkingSet
    /// </summary>
    public class FormMarkingSet : DynamicItem
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Comment { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormMarkingSet()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormMarkingSet(IDynamicItemDefinition definition)
            : base(definition)
        {
        }
    }
}

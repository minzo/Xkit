using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corekit.Models;

namespace Toolkit.WPF.Sample.Editor.ViewModels
{
    public class ExpressionViewModel<T> : TypedColletion<DynamicItem>, IDynamicTable<DynamicItem, T>
    {
        private object model;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ExpressionViewModel(object model)
        {
            this.model = model;
        }

        /// <summary>
        /// 値の取得
        /// </summary>
        public T GetPropertyValue(string rowName, string colName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetPropertyValue(string rowName, string colName, T value)
        {
            throw new NotImplementedException();
        }
    }
}

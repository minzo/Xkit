using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPlugin.Models
{
    /// <summary>
    /// プロパティ定義
    /// </summary>
    public class RuntimeProperty
    {
        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 要素
        /// </summary>
        public IEnumerable<string> Elements => this._Elements;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RuntimeProperty()
        {
            this._Elements = new ObservableCollection<string>();
        }

        /// <summary>
        /// 要素セット
        /// </summary>
        public void SetElements(string name, IEnumerable<string> elements)
        {
            this.Name = name;
            this._Elements.Clear();
            foreach (var element in elements)
            {
                this._Elements.Add(element);
            }
        }

        private readonly ObservableCollection<string> _Elements;
    }
}

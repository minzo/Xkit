using Corekit.Extensions;
using REPlugin.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPlugin.ViewModels
{
    /// <summary>
    /// プロパティ定義のVM
    /// </summary>
    internal class RuntimePropertyViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティ要素VM
        /// </summary>
        public class ElementViewModel : INotifyPropertyChanged
        {
            public string Elemenet { get; }

            public string Dest { get => this._Dest; set => this.SetProperty(ref this._Dest, value); }

            public ElementViewModel(string element)
            {
                this.Elemenet = element;
                this.Dest = element;
            }

            private string _Dest;

#pragma warning disable CS0067
            public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
        }

        /// <summary>
        /// 定義名
        /// </summary>  
        public string Name => this._Definition.Name;

        /// <summary>
        /// 要素名
        /// </summary>
        public ICollection<ElementViewModel> Elements => this._Elements;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RuntimePropertyViewModel(RuntimeProperty definition)
        {
            this._Definition = definition;

            this._Elements = definition.Elements
                .Select(i => new ElementViewModel(i))
                .ToObservableCollection();

            this._Elements
                .ForEach(i => i.PropertyChanged += this.InvokePropertyChanged);
        }

        /// <summary>
        /// プロパティ変更通知
        /// </summary>
        private void InvokePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        private readonly RuntimeProperty _Definition;
        private readonly ObservableCollection<ElementViewModel> _Elements;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}

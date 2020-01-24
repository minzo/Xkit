using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Corekit.Extensions;
using Corekit.Models;
using Toolkit.WPF.Commands;
using Xkit.Plugins.Sample.Models;

namespace Xkit.Plugins.Sample.ViewModels
{
    /// <summary>
    /// 一括編集のVM
    /// </summary>
    internal class BatchEditPanelViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 一括編集アイテム
        /// </summary>
        internal class BatchEditItem : INotifyPropertyChanged
        {
            public object Owner => this._Item.Owner;

            public object Value
            {
                get => this._Item.GetPropertyValue(this._PropertyName);
                set => this._Item.SetPropertyValue(this._PropertyName, value);
            }

            public object NewValue { 
                get => this._NewValue;
                set => this.SetProperty(ref this._NewValue, value);
            }

            public BatchEditItem(EventTrigger item, string propertyName)
            {
                this._PropertyName = propertyName;
                this._Item = item;
                this.NewValue = this.Value;
            }

            public void SetPropertyName(string propertyName)
            {
                this._PropertyName = propertyName;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }

            private EventTrigger _Item;
            private string _PropertyName;
            private object _NewValue;

#pragma warning disable CS0067
            public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
        }

        /// <summary>
        /// 対象とするプロパティ名
        /// </summary>
        public string PropertyName { 
            get => this._PropertyName;
            set
            {
                if (this.SetProperty(ref this._PropertyName, value))
                {
                    this.Items
                        .OfType<BatchEditItem>()
                        .ForEach(i => i.SetPropertyName(this._PropertyName));
                }
            }
        }

        /// <summary>
        /// 対象とするプロパティ名リスト
        /// </summary>
        public IEnumerable<string> PropertyNameList { get; }

        /// <summary>
        /// 編集アイテム
        /// </summary>
        public IEnumerable<object> Items { get; }

        /// <summary>
        /// 計算値
        /// </summary>
        public object Value { 
            get => this._Value;
            set
            {
                if (this.SetProperty(ref this._Value, value))
                {
                    this.DoCalc();
                }
            }
        }

        /// <summary>
        /// 操作
        /// </summary>
        public enum Operation
        {
            Addition,
            Subtraction,
            Multiplication
        }

        /// <summary>
        /// 操作
        /// </summary>
        public Operation Operator { 
            get => this._Operator; 
            set => this.SetProperty(ref this._Operator, value);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BatchEditPanelViewModel(IEnumerable<EventTrigger> triggers)
        {
            this.PropertyNameList = triggers.FirstOrDefault()?.Definition
                .Where(i => i.ValueType == typeof(float) || i.ValueType == typeof(int))
                .Select(i => i.Name);

            this._PropertyName = this.PropertyNameList.FirstOrDefault();

            this.Items = triggers
                .Select(i => new BatchEditItem(i, this._PropertyName))
                .ToList();

            this.ApplyCommand = new DelegateCommand(_ => this.DoCalc());
        }

        /// <summary>
        /// 計算実行
        /// </summary>
        private void DoCalc()
        {
            this.Items
                .OfType<BatchEditItem>()
                .ForEach(i => i.NewValue = this.Calc(this.Operator, i.Value, this.Value));
        }

        /// <summary>
        /// 計算処理
        /// </summary>
        private object Calc(Operation ope, object lha, object rha)
        {
            if (float.TryParse(lha?.ToString(), out float _lha) &&
                float.TryParse(rha?.ToString(), out float _rha))
            {
                switch (ope)
                {
                    case Operation.Addition:
                        return _lha + _rha;
                    case Operation.Subtraction:
                        return _lha - _rha;
                    case Operation.Multiplication:
                        return _lha * _rha;
                    default:
                        throw new Exception("定義されていな計算です");
                }
            }
            return 0;
        }

        #region コマンド

        public ICommand ApplyCommand { get; }

        #endregion

        private string _PropertyName;
        private object _Value;
        private Operation _Operator;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}

using System.ComponentModel;

namespace Corekit.Models
{
    /// <summary>
    /// 継承プロパティ
    /// </summary>
    public class InheritanceProperty<T> : IDynamicProperty<T>
    {
        /// <summary>
        /// 定義
        /// </summary>
        public IDynamicPropertyDefinition Definition { get; }

        /// <summary>
        /// Owner
        /// </summary>
        public IDynamicItem Owner { get; }

        /// <summary>
        /// 読み取り専用か (Ownerの状態も考慮して最終的な状態を返します)
        /// </summary>
        public bool IsReadOnly => this.Owner?.Definition?.IsReadOnly == true || this.Definition.IsReadOnly == true;

        /// <summary>
        /// 継承している状態か
        /// </summary>
        public bool IsInherited => !this._HasValue && (this._InheritanceSource != null || this.Owner is InheritanceItem);

        /// <summary>
        /// 値
        /// </summary>
        public T Value
        {
            get => this.GetValue_();
            set
            {
                // 値がセットされたときに有効な値を持っていなかったので今回HasValueはTrueとなる
                bool isHasValueChanged = !this._HasValue;
                this._HasValue = true;

                bool isValueChanged = !Equals(this._Value, value);
                if (isValueChanged)
                {
                    if (this._Value is INotifyPropertyChanged oldValue)
                    {
                        oldValue.PropertyChanged -= this.OnPropertyChanged;
                    }

                    if (value is INotifyPropertyChanged newValue)
                    {
                        newValue.PropertyChanged += this.OnPropertyChanged;
                    }

                    this._Value = value;
                }

                if (isValueChanged || isHasValueChanged)
                {
                    this.PropertyChanged?.Invoke(this, _ChangedEventArgs);
                }
            }
        }

        /// <summary>
        /// 値を取得する
        /// </summary>
        public object GetValue() => this.GetValue_();

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetValue(object value) => this.Value = (T)value;

        /// <summary>
        /// 継承状態にします
        /// </summary>
        public void EnableInheritance(IDynamicProperty<T> inheritanceSource)
        {
            this._InheritanceSource = inheritanceSource;
            this._HasValue = false;
        }

        /// <summary>
        /// 継承状態を解除します
        /// </summary>
        public void DisableInheritance()
        {
            this._InheritanceSource = null;
            this._HasValue = true;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InheritanceProperty(IDynamicPropertyDefinition definition, IDynamicItem owner)
        {
            this.Definition = definition;
            this.Owner = owner;
            this._Value = (T)this.Definition.GetDefaultValue();

            if (this._Value is INotifyPropertyChanged value)
            {
                value.PropertyChanged += this.OnPropertyChanged;
            }
        }

        /// <summary>
        /// 継承を考慮せずにこのインスタンスが保持している値を取得します
        /// </summary>
        protected T RawValue => this._Value;

        /// <summary>
        /// 値を取得する
        /// </summary>
        private T GetValue_()
        {
            if (this._HasValue)
            {
                return this._Value;
            }
            else if (this._InheritanceSource != null)
            {
                return this._InheritanceSource.Value;
            }
            else if (this.Owner is InheritanceItem inheritance && inheritance.IsInherited)
            {
                return inheritance.GetPropertyValue<T>(this.Definition.Name);
            }
            else
            {
                return (T)this.Definition.GetDefaultValue();
            }
        }

        /// <summary>
        /// プロパティ変更通知
        /// </summary>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, _ChangedEventArgs);
        }

        private IDynamicProperty<T> _InheritanceSource = null;
        private bool _HasValue = false;
        private T _Value;

        public event PropertyChangedEventHandler PropertyChanged;
        private static readonly PropertyChangedEventArgs _ChangedEventArgs = new PropertyChangedEventArgs(nameof(Value));
    }
}

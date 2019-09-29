using Corekit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Xkit.Plugins.Sample.Models
{
    /// <summary>
    /// イベントトリガー
    /// </summary>
    public class EventTrigger : DynamicItem
    {
        public new string Owner { get => this.GetPropertyValueImpl<string>(); set => this.SetPropertyValueImpl(value); }

        public string Key { get => this.GetPropertyValueImpl<string>(); set => this.SetPropertyValueImpl(value); }

        public int Variation { get => this.GetPropertyValueImpl<int>(); set => this.SetPropertyValueImpl(value); }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EventTrigger(Cell cell) 
            : base(new DynamicItemDefinition(_PropertyDefinitions) { Name = $"{cell.Sources.Name}_{cell.Targets.Name}" })
        {
            this.Owner = $"{cell.Sources.Name}_{cell.Targets.Name}";
            this.Key = this.Definition.Name;
        }

        /// <summary>
        /// プロパティ設定
        /// </summary>
        private void SetPropertyValueImpl<T>(T value, [CallerMemberName]string propertyName = null)
        {
            this.SetPropertyValue<T>(propertyName, value);
        }

        /// <summary>
        /// プロパティ取得
        /// </summary>
        private T GetPropertyValueImpl<T>([CallerMemberName]string propertyName = null)
        {
            return this.GetPropertyValue<T>(propertyName);
        }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static EventTrigger()
        {
            _PropertyDefinitions = new IDynamicPropertyDefinition[] {
                new DynamicPropertyDefinition<string>(){ Name = "Owner" },
                new DynamicPropertyDefinition<string>(){ Name = "Key" },
                new DynamicPropertyDefinition<float>(){ Name = "Volume" },
                new DynamicPropertyDefinition<float>(){ Name = "Pitch" },
                new DynamicPropertyDefinition<int>(){ Name = "Variation" },
            };
        }

        private static readonly IEnumerable<IDynamicPropertyDefinition> _PropertyDefinitions;
    }
}

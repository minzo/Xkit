using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corekit.Models
{
    /// <summary>
    /// 定義
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Name={Name} Type={ValueType}")]
    public class TypeDefinition
    {
        /// <summary>
        /// 定義名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 型
        /// </summary>
        public ValueType ValueType { get; }

        /// <summary>
        /// プリミティブか
        /// </summary>
        public bool IsPrimitive => this.ValueType <= ValueType.String;

        /// <summary>
        /// プロパティ定義
        /// </summary>
        public IEnumerable<PropertyDefinition> PropertyDefinitions { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TypeDefinition(string name, ValueType valueType)
        {
            this.Name = name;
            this.ValueType = valueType;
            this.PropertyDefinitions = Enumerable.Empty<PropertyDefinition>();
        }

        /// <summary>e
        /// コンストラクタ
        /// </summary>
        public TypeDefinition(string name, params PropertyDefinition[] propertyDefinitions)
            : this(name, ValueType.Class)
        {
            this.PropertyDefinitions = propertyDefinitions;
        }

        public static readonly TypeDefinition Bool   = new TypeDefinition(nameof(Bool),   ValueType.Bool);
        public static readonly TypeDefinition S32    = new TypeDefinition(nameof(S32),    ValueType.S32);
        public static readonly TypeDefinition F32    = new TypeDefinition(nameof(F32),    ValueType.F32);
        public static readonly TypeDefinition String = new TypeDefinition(nameof(String), ValueType.String);
        public static readonly TypeDefinition List   = new TypeDefinition(nameof(List),   ValueType.List);
        public static readonly TypeDefinition Vec3   = new TypeDefinition(nameof(Vec3),  new PropertyDefinition("X", F32), new PropertyDefinition("Y", F32), new PropertyDefinition("Z", F32));
        public static readonly TypeDefinition Color  = new TypeDefinition(nameof(Color), new PropertyDefinition("R", F32), new PropertyDefinition("G", F32), new PropertyDefinition("B", F32));
    }

    /// <summary>
    /// プロパティ定義
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Name={Name} Type={TypeDefinition}")]
    public class PropertyDefinition
    {
        /// <summary>
        /// メンバー名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 型定義
        /// </summary>
        public TypeDefinition TypeDefinition { get; }

        /// <summary>
        /// デフォルト値
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PropertyDefinition(string name, TypeDefinition typeDefinition, object defaultValue = null)
        {
            this.Name = name;
            this.TypeDefinition = typeDefinition;
            this.DefaultValue = defaultValue;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Corekit.Models;

namespace Tookit.WPF.Editor.Models
{
    /// <summary>
    /// マテリアル
    /// </summary>
    public class Material : IRuntimeEnumMember, IDisplayColor, INotifyPropertyChanged
    {
        /// <summary>
        /// 名前
        /// </summary>
        public string Name
        {
            get => this.GetPropertyValue<string>();
            set => this.SetPropertyValue(value);
        }

        /// <summary>
        /// 表示名
        /// </summary>
        public string DisplayName
        {
            get => this.GetPropertyValue<string>();
            set => this.SetPropertyValue(value);
        }

        /// <summary>
        /// 説明
        /// </summary>
        public string Description
        {
            get => this.GetPropertyValue<string>();
            set => this.SetPropertyValue(value);
        }

        /// <summary>
        /// 参照されているサブマテリアル
        /// </summary>
        public IReadOnlyList<Guid> ReferencedSubMaterials => this._ReferencedSubMaterials;

        /// <summary>
        /// 表示色
        /// </summary>
        public Color DisplayColor
        {
            get => this.GetPropertyValue();
            set => this.SetPropertyValue(value);
        }

        /// <summary>
        /// GUID
        /// </summary>
        public Guid Guid { get; } = new Guid();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Material()
        {
            this._Instance = new Property(_TypeDefinition);
            this._Instance.PropertyChanged += this.PropertyChanged;
        }

        public void AddReferencedSubMaterial()
        {
        }

        public void RemoveReferencedSubMaterial()
        {

        }

        private Color GetPropertyValue([CallerMemberName] string propertyPath = null)
        {
            var instance = this._Instance.GetProperty(propertyPath);
            var R = instance.GetProperty("R").GetValue<float>();
            var G = instance.GetProperty("G").GetValue<float>();
            var B = instance.GetProperty("B").GetValue<float>();
            return new Color() { A = 255, R = (byte)(255 * R), G = (byte)(255 * G), B = (byte)(255 * B) };
        }

        private void SetPropertyValue(Color value, [CallerMemberName] string propertyPath = null)
        {
            var instance = this._Instance.GetProperty(propertyPath);
            instance.GetProperty("R").SetValue(value.R / 255.0f);
            instance.GetProperty("G").SetValue(value.G / 255.0f);
            instance.GetProperty("B").SetValue(value.B / 255.0f);
        }

        private T GetPropertyValue<T>([CallerMemberName] string propertyPath = null)
        {
            return this._Instance.GetProperty(propertyPath).GetValue<T>();
        }

        private void SetPropertyValue<T>(T value, [CallerMemberName] string propertyPath = null)
        {
            this._Instance.GetProperty(propertyPath).SetValue(value);
        }

        private Property _Instance;

        private List<Guid> _ReferencedSubMaterials = new List<Guid>();

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static Material()
        {
            _TypeDefinition = new TypeDefinition(nameof(Material),
                new PropertyDefinition("Name", TypeDefinition.String),
                new PropertyDefinition("DisplayName", TypeDefinition.String),
                new PropertyDefinition("Description", TypeDefinition.String),
                new PropertyDefinition("DisplayColor", TypeDefinition.Color));
        }

        private static readonly TypeDefinition _TypeDefinition;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}

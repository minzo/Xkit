using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Corekit.Extensions;

namespace Tookit.WPF.Editor.Models
{
    /// <summary>
    /// マテリアル
    /// </summary>
    public class Material : IRuntimeEnumMember, IDisplayColor, INotifyPropertyChanged
    {
        public string Name
        {
            get => this._Name;
            set => this.SetProperty(ref this._Name, value);
        }

        public string DisplayName
        {
            get => this._DisplayName;
            set => this.SetProperty(ref this._DisplayName, value);
        }

        public string Description
        {
            get => this._Description;
            set => this.SetProperty(ref this._Description, value);
        }

        public IReadOnlyList<Guid> ReferencedSubMaterials => this._ReferencedSubMaterials;

        public Color DisplayColor
        {
            get => this._DisplayColor;
            set => this.SetProperty(ref this._DisplayColor, value);
        }

        public Guid Guid { get; } = new Guid();


        public void AddReferencedSubMaterial()
        {
        }

        public void RemoveReferencedSubMaterial()
        {

        }

        private string _Name;
        private string _DisplayName;
        private string _Description;
        private List<Guid> _ReferencedSubMaterials = new List<Guid>();
        private Color _DisplayColor = Colors.Black;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class SubMaterial : IRuntimeEnumMember
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public List<string> RefMaterial { get; } = new List<string>();

        public Guid Guid { get; } = new Guid();
    }
}

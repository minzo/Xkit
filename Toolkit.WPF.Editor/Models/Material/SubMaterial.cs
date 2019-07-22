using Corekit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tookit.WPF.Editor.Models
{
    /// <summary>
    /// サブマテリアル
    /// </summary>
    public class SubMaterial : IRuntimeEnumMember
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public List<string> RefMaterial { get; } = new List<string>();

        public Guid Guid { get; } = new Guid();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SubMaterial()
        {

        }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static SubMaterial()
        {
            _TypeDefinition = new TypeDefinition(nameof(SubMaterial),
                new PropertyDefinition("Name", TypeDefinition.String),
                new PropertyDefinition("DisplayName", TypeDefinition.String),
                new PropertyDefinition("Description", TypeDefinition.String),
                new PropertyDefinition("DisplayColor", TypeDefinition.Color));
        }

        private static readonly TypeDefinition _TypeDefinition;

    }
}

using Corekit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.WPF.Sample
{
    internal class InheritanceObjectViewModel
    {
        public List<InheritanceObject> Items { get; }

        public InheritanceObjectViewModel()
        {
            this._Manager = new InheritanceObjectManager();

            var info = this._Manager.CreateTypeInfo("Test");
            info.AddPrpoperty("String0", this._Manager.GetTypeInfo(Corekit.Models.InheritanceObjectBuildInTypeName.String));
            info.AddPrpoperty("String1", this._Manager.GetTypeInfo(Corekit.Models.InheritanceObjectBuildInTypeName.String));
            info.AddPrpoperty("String2", this._Manager.GetTypeInfo(Corekit.Models.InheritanceObjectBuildInTypeName.String));
            info.AddPrpoperty("String3", this._Manager.GetTypeInfo(Corekit.Models.InheritanceObjectBuildInTypeName.String));


            this.Items = new List<InheritanceObject>()
            {
                new InheritanceObject(info),
                new InheritanceObject(info),
                new InheritanceObject(info),
            };
        }

        private readonly InheritanceObjectManager _Manager;
    }
}

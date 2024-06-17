﻿using Corekit.Extensions;
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

            var parent = new InheritanceObject(info);

            var child0 = new InheritanceObject(info);
            child0.InheritanceSource = parent;

            var child1 = new InheritanceObject(info);
            child1.InheritanceSource = parent;

            parent.GetProperty("String1").SetValue("Test");

            this.Items = new List<InheritanceObject>()
            {
                parent,
                child0,
                child1
            };
        }

        private readonly InheritanceObjectManager _Manager;
    }
}

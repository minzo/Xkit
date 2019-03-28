﻿using Corekit.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.WPF.Sample.Editor.Models
{
    public class Config
    {
        public ObservableCollection<Module> A_Modules { get; } = new ObservableCollection<Module>(Enumerable.Range(0, 3).Select(i => new Module() { Name = "A_Module" + i }));

        public ObservableCollection<Module> B_Modules { get; } = new ObservableCollection<Module>(Enumerable.Range(0, 3).Select(i => new Module() { Name = "B_Module" + i }));

        public DynamicTable<bool> PauseTable { get; }

        public DynamicTable<double> ScaleTable { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Config()
        {
            PauseTable = new DynamicTable<bool>(A_Modules, new ReadOnlyObservableCollection<Module>(A_Modules));
            ScaleTable = new DynamicTable<double>(A_Modules, A_Modules);
        }
    }
}
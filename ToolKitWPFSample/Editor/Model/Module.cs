﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolKit.WPF.Models;

namespace ToolKit.WPF.Sample.Editor.Model
{
    public class Module : IDynamicTableFrame
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; set; }
    }
}

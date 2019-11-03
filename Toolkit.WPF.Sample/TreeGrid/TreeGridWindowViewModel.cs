﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corekit.Extensions;

namespace Toolkit.WPF.Sample
{
    public class TreeGridItem : INotifyPropertyChanged
    {
        public List<TreeGridItem> Children { get; set; } = new List<TreeGridItem>();

        public string PropertyPath { get; set; } = "Name";

        public string Name { get; set; }

        public bool IsExpanded {
            get => this._IsExpanded;
            set => this.SetProperty(ref this._IsExpanded, value);
        }

        private bool _IsExpanded = false;


#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }


    public class TreeGridWindowViewModel
    {
        static public List<TreeGridItem> items { get; set; } = new List<TreeGridItem>()
        {
            new TreeGridItem() { Name = "Parent0", Children = new List<TreeGridItem>() {
                new TreeGridItem() { Name = "Children00", Children = new List<TreeGridItem>() {
                    new TreeGridItem() { Name = "GroundChildren00" },
                    new TreeGridItem() { Name = "GroundChildren01" },
                }},
                new TreeGridItem() { Name = "Children01" }
            } },
            new TreeGridItem() { Name = "Parent1" },
            new TreeGridItem() { Name = "Parent2" },
            new TreeGridItem() { Name = "Parent2" },
            new TreeGridItem() { Name = "Parent2" },
            new TreeGridItem() { Name = "Parent2" },
            new TreeGridItem() { Name = "Parent2" },
            new TreeGridItem() { Name = "Parent2" },
            new TreeGridItem() { Name = "Parent2" },
            new TreeGridItem() { Name = "Parent2" },
            new TreeGridItem() { Name = "Parent2" },
            new TreeGridItem() { Name = "Parent2" },
            new TreeGridItem() { Name = "Parent2" },
            new TreeGridItem() { Name = "Parent2" },
            new TreeGridItem() { Name = "Parent2" },
            new TreeGridItem() { Name = "Parent2" },
            new TreeGridItem() { Name = "Parent0", Children = new List<TreeGridItem>() {
                new TreeGridItem() { Name = "Children00", Children = new List<TreeGridItem>() {
                    new TreeGridItem() { Name = "GroundChildren00" },
                    new TreeGridItem() { Name = "GroundChildren01" },
                }},
                new TreeGridItem() { Name = "Children01" }
            } },
            new TreeGridItem() { Name = "Parent0", Children = new List<TreeGridItem>() {
                new TreeGridItem() { Name = "Children00", Children = new List<TreeGridItem>() {
                    new TreeGridItem() { Name = "GroundChildren00" },
                    new TreeGridItem() { Name = "GroundChildren01" },
                }},
                new TreeGridItem() { Name = "Children01" }
            } },
            new TreeGridItem() { Name = "Parent0", Children = new List<TreeGridItem>() {
                new TreeGridItem() { Name = "Children00", Children = new List<TreeGridItem>() {
                    new TreeGridItem() { Name = "GroundChildren00" },
                    new TreeGridItem() { Name = "GroundChildren01" },
                }},
                new TreeGridItem() { Name = "Children01" }
            } },
            new TreeGridItem() { Name = "Parent0", Children = new List<TreeGridItem>() {
                new TreeGridItem() { Name = "Children00", Children = new List<TreeGridItem>() {
                    new TreeGridItem() { Name = "GroundChildren00" },
                    new TreeGridItem() { Name = "GroundChildren01" },
                }},
                new TreeGridItem() { Name = "Children01" }
            } },
        };

        public ObservableCollection<TreeGridItem> Items { get; } = new ObservableCollection<TreeGridItem>(items.EnumerateTree(i => i.Children));
    }
}

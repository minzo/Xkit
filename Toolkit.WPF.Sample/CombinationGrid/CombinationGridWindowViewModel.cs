﻿using Corekit.Extensions;
using Corekit.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Toolkit.WPF.Sample
{
    internal class CombinationGridWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// トリガー
        /// </summary>
        internal class TriggerItem
        {
            public string Key { get; } = "Key";

            public float Volume { get; }

            public float Pitch { get; }

            public float FadeIn { get; }
        }

        /// <summary>
        /// セル
        /// </summary>
        internal class Cell
        {
            public IReadOnlyCollection<TriggerItem> TriggerItems { get; }

            public Cell()
            {
                this.TriggerItems = new List<TriggerItem>()
                {
                    new TriggerItem()
                };
            }
        }

        /// <summary>
        /// Table
        /// </summary>
        public CombinationTable<Cell,string,string> Table { get; }

        /// <summary>
        /// Cell
        /// </summary>
        public IEnumerable<Cell> SelectedCells { get; set; }

        /// <summary>
        /// Trigger
        /// </summary>
        public IEnumerable<TriggerItem> SelectedTriggers { get; set; }

        /// <summary>
        /// 選択情報
        /// </summary>
        public IEnumerable<(object Item, string PropertyName)> SelectedInfos
        {
            set
            {
                var cells = value?
                    .Select(i => (i.Item as IDynamicItem).GetPropertyValue<Cell>(i.PropertyName))
                    .ToList();

                var triggers = cells?
                    .SelectMany(i => i.TriggerItems)
                    .ToList();

                this.SetProperty(cells, nameof(this.SelectedCells));
                this.SetProperty(triggers, nameof(this.SelectedTriggers));
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CombinationGridWindowViewModel()
        {
            var row = new Combination<string>();
            row.AddDefinition("Mat", new List<string>() { "Body", "Stone", "Wood", "Metal" });
            row.AddDefinition("Sub", new List<string>() { "Small", "Middle", "Big" });
            var col = new Combination<string>();
            col.AddDefinition("Obj", new List<string>() { "Small", "Middle" });
            col.AddDefinition("Mas", new List<string>() { "Light", "Normal", "Heavy" });
            col.AddDefinition("Col", new List<string>() { "White", "Gray", "Black" });
            this.Table = new CombinationTable<Cell,string,string>(row, col);

            this.Initialize();
        }

        private void Initialize()
        {
            foreach (var row in this.Table)
            {
                foreach (var col in row.Value)
                {
                    this.Table.SetPropertyValue(row.Definition.Name, col.Definition.Name, new Cell());
                }
            }
        }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}

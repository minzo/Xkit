using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Toolkit.WPF.Models;

namespace Toolkit.WPF.Sample
{
    class CIEViewModel
    {
        public ObservableCollection<object> Items { get; }

        public CIEViewModel()
        {
            var type = new DynamicType("test", new DynamicPropertyInfo[] {
                new DynamicPropertyInfo<string>("Name"),
                new DynamicPropertyInfo<string>("Path"),
                new DynamicPropertyInfo<bool>("ON"),
            });

            var items = new ObservableCollection<object>(new[] {
                new CommonIntermediateExpression(type),
                new CommonIntermediateExpression(type),
                new CommonIntermediateExpression(type),
            });

            this.Items = items;
        }
    }
}

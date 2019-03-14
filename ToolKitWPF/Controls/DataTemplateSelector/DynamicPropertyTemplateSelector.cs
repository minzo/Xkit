using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Corekit.Models;

namespace ToolKit.WPF.Controls
{
    [ContentProperty("Templates")]
    public class DynamicPropertyTemplateSelector : DataTemplateSelector
    {
        public List<DataTemplate> Templates { get; set; } = new List<DataTemplate>();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;
            var prop = item as IDynamicProperty;
            var type = prop.Definition.ValueType;
            var template = Templates.FirstOrDefault(i => type == i.DataType as Type);
            return template ?? base.SelectTemplate(item, container);
        }
    }
}

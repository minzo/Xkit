using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Corekit.Models;

namespace Toolkit.WPF.Controls
{
    [ContentProperty("Templates")]
    public class DynamicPropertyTemplateSelector : DataTemplateSelector
    {
        public List<DataTemplate> Templates { get; set; } = new List<DataTemplate>();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return Templates.LastOrDefault();
            }

            var type = (item as IDynamicProperty)?.Definition.ValueType ?? item.GetType();
            var template = Templates.FirstOrDefault(i => (i.DataType as Type).IsAssignableFrom(type));
            return template ?? base.SelectTemplate(item, container);
        }
    }
}

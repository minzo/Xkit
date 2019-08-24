using Corekit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Toolkit.WPF.Controls
{
    [ContentProperty("Templates")]
    public class DynamicPropertyTemplateSelector : DataTemplateSelector
    {
        public List<DataTemplate> Templates { get; set; } = new List<DataTemplate>();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var prop = item as IDynamicProperty;
            if (prop == null)
            {
                return this.Templates.LastOrDefault();
            }

            var type = prop.GetValue()?.GetType() ?? prop.Definition.ValueType;                
            var template = this.Templates.FirstOrDefault(i => (i.DataType as Type).IsAssignableFrom(type));
            return template ?? base.SelectTemplate(item, container);
        }
    }
}

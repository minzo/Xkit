using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Toolkit.WPF.Controls
{
    [ContentProperty("Templates")]
    public class DataTypeTemplateSelector : DataTemplateSelector
    {
        public List<DataTemplate> Templates { get; set; } = new List<DataTemplate>();

        public string TargetPropertyPath { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return this.Templates.LastOrDefault();
            }

            var type = item.GetType();
            if (!string.IsNullOrWhiteSpace(this.TargetPropertyPath))
            {
                type = type.GetProperty(this.TargetPropertyPath)?.PropertyType ?? type;
            }

            var template = this.Templates.FirstOrDefault(i => (i.DataType as Type).IsAssignableFrom(type));
            return template ?? base.SelectTemplate(item, container);
        }
    }
}

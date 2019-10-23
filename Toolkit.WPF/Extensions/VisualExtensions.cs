using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Toolkit.WPF.Extensions
{
    public static class VisualExtensions
    {
        /// <summary>
        /// VisualParentを列挙する
        /// </summary>
        public static IEnumerable<DependencyObject> EnumerateParent(this DependencyObject source)
        {
            while ((source = VisualTreeHelper.GetParent(source)) != null)
            {
                yield return source;
            }
        }

        /// <summary>
        /// VisualChildrenを列挙する
        /// </summary>
        public static IEnumerable<DependencyObject> EnumerateChildren(this DependencyObject source)
        {
            var count = VisualTreeHelper.GetChildrenCount(source);
            var children = Enumerable.Range(0, count).Select(i => VisualTreeHelper.GetChild(source, i));
            return children.Concat(children.SelectMany(i => i.EnumerateChildren()));
        }
    }
}

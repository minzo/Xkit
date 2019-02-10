using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace ToolKit.WPF.Controls
{
    public class DataGridTreeColumn : DataGridBoundColumn
    {
        /*
         * DataGridOwner は DataGrid の Columns が増減するときの ColumnsChanged の際に set される
         * 
         * DataGrid の Filtering を使って Children にあたる行の表示状態を制御したい
         * Rowに開閉状態を付加したい
         * DataGridCell の VisualChildren を辿ると DataGridRow ぐらいまではとれる
         * SmartTree の場合は SmartTree が開閉状態を管理しているので、理想は DataGrid が TreeGridColumn の ExpandedMemberPath をもとにフィルタリングしてくれると嬉しい...?
         * SmartTree の Filtering が どうやって実現しているかによるね
         * 
         * Sort は 深さ優先探索順 で並べた状態 Filtering はツリーの開閉で決めてほしい
         * 結局 DataGrid をカスタマイズするしかなさそう
         * 
         * LoadTemplateContent のタイミングで dataItem が Row の DataContext のはず
         * DataContext の Binding() { Path = ExpandedProperty } で、開閉状態の値をとりたい?
         * 
         */

        #region template

        public DataTemplate CellTemplate
        {
            get { return (DataTemplate)GetValue(CellTemplateProperty); }
            set { SetValue(CellTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellTemplateProperty =
            DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(DataGridTreeColumn), new PropertyMetadata(null));



        public DataTemplateSelector CellTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(CellTemplateSelectorProperty); }
            set { SetValue(CellTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellTemplateSelectorProperty =
            DependencyProperty.Register("CellTemplateSelector", typeof(DataTemplateSelector), typeof(DataGridTreeColumn), new PropertyMetadata(null));



        public DataTemplate CellEditingTemplate
        {
            get { return (DataTemplate)GetValue(CellEditingTemplateProperty); }
            set { SetValue(CellEditingTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellEditingTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellEditingTemplateProperty =
            DependencyProperty.Register("CellEditingTemplate", typeof(DataTemplate), typeof(DataGridTreeColumn), new PropertyMetadata(null));


        public DataTemplateSelector CellEditingTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(CellEditingTemplateSelectorProperty); }
            set { SetValue(CellEditingTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellEditingTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellEditingTemplateSelectorProperty =
            DependencyProperty.Register("CellEditingTemplateSelector", typeof(DataTemplateSelector), typeof(DataGridTreeColumn), new PropertyMetadata(null));

        #endregion

        #region ChildrenProperty

        public BindingBase ChildrenBinding
        {
            get { return (BindingBase)GetValue(ChildrenBindingProperty); }
            set { SetValue(ChildrenBindingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChildrenBinding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildrenBindingProperty =
            DependencyProperty.Register("ChildrenBinding", typeof(BindingBase), typeof(DataGridTreeColumn), new PropertyMetadata(null));


        public string ChildrenPropertyPath
        {
            get { return (string)GetValue(ChildrenPropertyPathProperty); }
            set { SetValue(ChildrenPropertyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChildrenPropertyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildrenPropertyPathProperty =
            DependencyProperty.Register("ChildrenPropertyPath", typeof(string), typeof(DataGridTreeColumn), new PropertyMetadata(null));

        #endregion

        #region IsExpandedProperty

        public BindingBase IsExpandedBinding
        {
            get { return (BindingBase)GetValue(IsExpandedBindingProperty); }
            set { SetValue(IsExpandedBindingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExpandedBinding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedBindingProperty =
            DependencyProperty.Register("IsExpandedBinding", typeof(BindingBase), typeof(DataGridTreeColumn), new PropertyMetadata(null));

        public string ExpandedPropertyPath
        {
            get { return (string)GetValue(ExpandedPropertyPathProperty); }
            set { SetValue(ExpandedPropertyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExpandedPropertyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpandedPropertyPathProperty =
            DependencyProperty.Register("ExpandedPropertyPath", typeof(string), typeof(DataGridTreeColumn), new PropertyMetadata(null));

        #endregion

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return LoadTemplateContent(cell, dataItem, true);
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return LoadTemplateContent(cell, dataItem, false);
        }

        private FrameworkElement LoadTemplateContent(DataGridCell cell, object dataItem, bool isEditing)
        {
            if(TryLoadTemplate(cell, dataItem, isEditing, out var element))
            {
                if( element.FindName("DockPanel") is DockPanel panel)
                {

                }

                if( element.FindName("ExpandedButton") is ToggleButton button)
                {
                }
            }

            return element;
        }

        private bool TryLoadTemplate(DataGridCell cell, object dataItem, bool isEditing, out FrameworkElement element)
        {
            DataTemplate template = null;

            if (isEditing)
            {
                template = CellEditingTemplate ?? CellEditingTemplateSelector?.SelectTemplate(dataItem, cell);
            }
            else
            {
                template = CellTemplate ?? CellTemplateSelector.SelectTemplate(dataItem, cell);
            }

            element = template?.LoadContent() as FrameworkElement;

            return element != null;
        }

        private void UpdateFilter(object item, bool isContracted)
        {
            if (contractedList.Contains(item) != isContracted)
            {
                MakeFilterFlag(item, isContracted);
                var collection = CollectionViewSource.GetDefaultView(DataGridOwner.ItemsSource);
                collection.Filter = i => !unvisibleList.Contains(i);
            }
        }

        private void MakeFilterFlag(object item, bool isContracted)
        {
            var children = childrenPropertyInfo.GetValue(item) as IEnumerable<object>;

            if (isContracted)
            {
                contractedList.Add(item);
                foreach (var child in children)
                {
                    MakeFilterFlag(child, isContracted);
                    unvisibleList.Add(child);
                }
            }
            else
            {
                contractedList.Remove(item);
                foreach (var child in children)
                {
                    MakeFilterFlag(child, isContracted);
                    unvisibleList.Remove(child);
                }
            }
        }

        private IEnumerable<object> GetChildren(object item)
        {
            return childrenPropertyInfo?.GetValue(item) as IEnumerable<object>;
        }

        private HashSet<object> contractedList = new HashSet<object>();
        private HashSet<object> unvisibleList = new HashSet<object>();
        private Dictionary<object, int> levelOfTreeDepth = new Dictionary<object, int>();
        private PropertyInfo childrenPropertyInfo;
    }
}

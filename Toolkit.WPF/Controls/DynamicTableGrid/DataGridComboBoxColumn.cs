using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Toolkit.WPF.Controls
{
    /// <summary>
    /// DataGridComboBoxColumn
    /// </summary>
    public class DataGridComboBoxColumn : DataGridColumn
    {
        /// <summary>
        /// ItemsSource
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DataGridComboBox.ItemsSourceProperty.AddOwner(typeof(DataGridComboBoxColumn), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// DisplayMemberPath
        /// </summary>
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayMemberPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DataGridComboBox.DisplayMemberPathProperty.AddOwner(typeof(DataGridComboBoxColumn), new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// SelectedValuePath
        /// </summary>
        public string SelectedValuePath
        {
            get { return (string)GetValue(SelectedValuePathProperty); }
            set { SetValue(SelectedValuePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedValuePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedValuePathProperty =
            DataGridComboBox.DisplayMemberPathProperty.AddOwner(typeof(DataGridComboBoxColumn), new FrameworkPropertyMetadata(string.Empty));


        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var textBlock = new TextBlock();
            BindingOperations.SetBinding(textBlock, DataGridComboBox.TextProperty, new Binding() { });
            return textBlock;
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var comboBox = new DataGridComboBox();
            BindingOperations.SetBinding(comboBox, DataGridComboBox.TextProperty, new Binding() { });
            return comboBox;
        }

        private static void SetBinding(BindingBase binding, DependencyObject target, DependencyProperty property)
        {
            if (binding != null)
            {
                BindingOperations.SetBinding(target, property, binding);
            }
            else
            {
                BindingOperations.ClearBinding(target, property);
            }
        }
    }
}

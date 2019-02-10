using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ToolKit.WPF.Controls
{
    /// <summary>
    /// TreeGrid.xaml の相互作用ロジック
    /// </summary>
    public partial class TreeGrid : DataGrid
    {
        /// <summary>
        /// フィルター用のテキスト
        /// </summary>
        public string FilterText
        {
            get { return (string)GetValue(FilterTextProperty); }
            set { SetValue(FilterTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilterText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterTextProperty =
            DependencyProperty.Register("FilterText", typeof(string), typeof(TreeGrid), new PropertyMetadata(null, (d, e) =>
            {
                
            }));


        /// <summary>
        /// 
        /// </summary>
        public string ChildrenPropertyPath
        {
            get { return (string)GetValue(ChildrenPropertyPathProperty); }
            set { SetValue(ChildrenPropertyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChilrenPropertyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildrenPropertyPathProperty =
            DependencyProperty.Register("ChildrenPropertyPath", typeof(string), typeof(TreeGrid), new PropertyMetadata(null));


        /// <summary>
        /// 
        /// </summary>
        public string IsExpandedPropertyPath
        {
            get { return (string)GetValue(IsExpandedPropertyPathProperty); }
            set { SetValue(IsExpandedPropertyPathProperty, value); }
        }

        //// Using a DependencyProperty as the backing store for IsExpandedPropertyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedPropertyPathProperty =
            DependencyProperty.Register("IsExpandedPropertyPath", typeof(string), typeof(TreeGrid), new PropertyMetadata(null));


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TreeGrid()
        {
            InitializeComponent();
            CanUserAddRows = false;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (oldValue != null)
            {
                CollectionViewSource.GetDefaultView(oldValue).CollectionChanged -= OnCollectionChanged;
                contractedList.Clear();
                unvisibleList.Clear();
                levelOfTreeDepth.Clear();
            }

            if (newValue != null)
            {
                CollectionViewSource.GetDefaultView(newValue).CollectionChanged += OnCollectionChanged;
                var collection = newValue as IEnumerable<object>;
                childrenPropertyInfo = collection
                    ?.FirstOrDefault()
                    ?.GetType()
                    ?.GetProperty(ChildrenPropertyPath);

                void calcLevelOfTreeDepth(IEnumerable<object> items, int level = 0)
                {
                    foreach(var item in items)
                    {
                        if(!levelOfTreeDepth.ContainsKey(item))
                        {
                            levelOfTreeDepth.Add(item, level);
                        }

                        if (childrenPropertyInfo.GetValue(item) is IEnumerable<object> children)
                        {
                            calcLevelOfTreeDepth(children, level + 1);
                        }
                    }
                }

                calcLevelOfTreeDepth(collection);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.OldItems != null)
            {
                foreach(var item in e.OldItems)
                {
                    childrenPropertyInfo.GetValue(item);
                }
            }

            if(e.NewItems != null)
            {
                foreach(var item in e.NewItems)
                {
                    childrenPropertyInfo.GetValue(item);
                }
            }
        }


        /// <summary>
        /// OnAutoGeneratingColumn    
        /// </summary>
        protected override void OnAutoGeneratingColumn(DataGridAutoGeneratingColumnEventArgs e)
        {
            base.OnAutoGeneratingColumn(e);

            if (Columns.Any(i => i.GetType() == typeof(TreeGridColumn)))
            {
                // TreeGridColumは１つまで?
            }
            else if (TryFindResource(nameof(TreeGridColumn)) is TreeGridColumn column)
            {
                column.Header = e.Column.Header;
                e.Column = column;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        internal bool HasChildren(object item)
        {
            var info = item.GetType().GetProperty(ChildrenPropertyPath);
            var children = info.GetValue(item) as IEnumerable<object>;
            return children.Any();
        }

        internal int  GetIndent(object item)
        {
            levelOfTreeDepth.TryGetValue(item, out int result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateFilter(object item, bool isContracted)
        {
            if( contractedList.Contains(item) != isContracted)
            {
                MakeFilterFlag(item, isContracted);
                var collection = CollectionViewSource.GetDefaultView(ItemsSource);
                collection.Filter = i => !unvisibleList.Contains(i);
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        private HashSet<object> contractedList = new HashSet<object>();
        private HashSet<object> unvisibleList = new HashSet<object>();
        private Dictionary<object, int> levelOfTreeDepth = new Dictionary<object, int>();

        private PropertyInfo childrenPropertyInfo;
    }

    /// <summary>
    /// TreeGridColumn
    /// </summary>
    public class TreeGridColumn : DataGridBoundColumn
    {
        #region Binding

        public BindingBase IsExpandedBinding
        {
            get { return (BindingBase)GetValue(IsExpandedBindingProperty); }
            set { SetValue(IsExpandedBindingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExpandedBinding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedBindingProperty =
            DependencyProperty.Register("IsExpandedBinding", typeof(BindingBase), typeof(TreeGridColumn), new PropertyMetadata(null));

        #endregion

        #region template

        public DataTemplate CellTemplate
        {
            get { return (DataTemplate)GetValue(CellTemplateProperty); }
            set { SetValue(CellTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellTemplateProperty =
            DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(TreeGridColumn), new PropertyMetadata(null));



        public DataTemplateSelector CellTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(CellTemplateSelectorProperty); }
            set { SetValue(CellTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellTemplateSelectorProperty =
            DependencyProperty.Register("CellTemplateSelector", typeof(DataTemplateSelector), typeof(TreeGridColumn), new PropertyMetadata(null));



        public DataTemplate CellEditingTemplate
        {
            get { return (DataTemplate)GetValue(CellEditingTemplateProperty); }
            set { SetValue(CellEditingTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellEditingTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellEditingTemplateProperty =
            DependencyProperty.Register("CellEditingTemplate", typeof(DataTemplate), typeof(TreeGridColumn), new PropertyMetadata(null));


        public DataTemplateSelector CellEditingTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(CellEditingTemplateSelectorProperty); }
            set { SetValue(CellEditingTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellEditingTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellEditingTemplateSelectorProperty =
            DependencyProperty.Register("CellEditingTemplateSelector", typeof(DataTemplateSelector), typeof(TreeGridColumn), new PropertyMetadata(null));

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
            ChooseCellTemplateAndSelector(isEditing, out var template, out var selector);

            if( template == null )
            {
                template = selector?.SelectTemplate(dataItem, cell);
            }

            var element = template?.LoadContent() as FrameworkElement;

            var treeGrid = DataGridOwner as TreeGrid;

            if( element?.FindName("DockPanel") is DockPanel dockPanel)
            {
                var indent = treeGrid?.GetIndent(dataItem) * 12.0 ?? 0.0;
                dockPanel.Margin = new Thickness(indent, 0.0, 0.0, 0.0);
            }

            if (element?.FindName("ExpandButton") is ToggleButton button)
            {
                SetBinding(button, ToggleButton.IsCheckedProperty, IsExpandedBinding, treeGrid?.IsExpandedPropertyPath);
                button.Checked += (s, e) => treeGrid?.UpdateFilter(dataItem, (bool)button.IsChecked);
                button.Unchecked += (s, e) => treeGrid?.UpdateFilter(dataItem, (bool)button.IsChecked);
                button.Visibility = treeGrid?.HasChildren(dataItem) == true ? Visibility.Visible : Visibility.Hidden;
            }

            if (element?.FindName("Text") is TextBlock textBlock)
            {
                SetBinding(textBlock, TextBlock.TextProperty, Binding, treeGrid?.DisplayMemberPath);
            }

            if (element?.FindName("Text") is TextBox textBox)
            {
                SetBinding(textBox, TextBox.TextProperty, Binding, treeGrid?.DisplayMemberPath);
            }

            cell.PreviewKeyDown -= OnPreviewKeyDown;
            cell.PreviewKeyDown += OnPreviewKeyDown;

            return element;
        }

        private void ChooseCellTemplateAndSelector(bool isEditing, out DataTemplate template, out DataTemplateSelector selector)
        {
            if (isEditing)
            {
                template = CellEditingTemplate;
                selector = CellEditingTemplateSelector;
            }
            else
            {
                template = CellTemplate;
                selector = CellTemplateSelector;
            }
        }

        private void SetBinding(DependencyObject target, DependencyProperty dp, BindingBase binding, string path)
        {
            if(binding != null)
            {
                BindingOperations.SetBinding(target, dp, binding);
            }
            else if(path !=null)
            {
                BindingOperations.SetBinding(target, dp, new Binding(path) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            }
            else
            {
                BindingOperations.ClearBinding(target, dp);
            }
        }


        private Dictionary<object, int> levelOfTreeDepth = new Dictionary<object, int>();

        private int CaldLevelOfTreeDepth(object dataItem)
        {
            if ( levelOfTreeDepth.TryGetValue(dataItem, out int level) )
            {
                return level;
            }

            var info = dataItem?.GetType()?.GetProperty("Children");
            var children = info.GetValue(dataItem) as IEnumerable<object>;

            foreach (var child in children)
            {

            }

            return level;
        }

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            void FindVisualChildren(DependencyObject dp)
            {
                var childrenCount = VisualTreeHelper.GetChildrenCount(dp);
                if (childrenCount > 0)
                {
                    switch (dp)
                    {
                        case TextBox v:
                            v.Focus();
                            v.SelectAll();
                            v.Height = editingElement.Height;
                            return;

                        case ComboBox v:
                            v.Focus();
                            v.IsDropDownOpen = true;
                            v.Height = editingElement.Height;
                            return;

                        case Control v:
                            v.Focus();
                            return;

                        default:
                            for(var i=0; i< childrenCount; i++)
                            {
                                FindVisualChildren(VisualTreeHelper.GetChild(dp, i));
                            }
                            return;
                    }
                }
            }

            FindVisualChildren(editingElement);

            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var cell = sender as DataGridCell;

            if (cell.IsEditing)
            {
                if (e.Key == Key.Escape)
                {
                    DataGridOwner?.CancelEdit();
                    e.Handled = true;
                }
            }
            else if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                if (IsBeginEditCharacter(e.Key) || IsBeginEditCharacter(e.ImeProcessedKey))
                {
                    DataGridOwner?.BeginEdit();

                    // ReferenceSource の DataGridTextBoxColumn を参考にした
                    //
                    // The TextEditor for the TextBox establishes contact with the IME
                    // engine lazily at background priority. However in this case we
                    // want to IME engine to know about the TextBox in earnest before
                    // PostProcessing this input event. Only then will the IME key be
                    // recorded in the TextBox. Hence the call to synchronously drain
                    // the Dispatcher queue.
                    //
                    Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Background);
                }
            }
        }

        private bool IsBeginEditCharacter(Key key)
        {
            var isCharacter = key >= Key.A && key <= Key.Z;
            var isNumber = (key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9);
            var isSpecial = key == Key.F2 || key == Key.Space || key == Key.Enter;
            return isCharacter || isNumber || isSpecial;
        }
    }
}

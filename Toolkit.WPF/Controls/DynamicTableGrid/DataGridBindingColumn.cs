using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Toolkit.WPF.Controls
{
    /// <summary>
    /// DataGridBindingColumn
    /// </summary>
    public class DataGridBindingColumn : DataGridBoundColumn
    {
        #region template

        public DataTemplate CellTemplate
        {
            get { return (DataTemplate)GetValue(CellTemplateProperty); }
            set { SetValue(CellTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellTemplateProperty =
            DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(DataGridBindingColumn), new PropertyMetadata(null));



        public DataTemplateSelector CellTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(CellTemplateSelectorProperty); }
            set { SetValue(CellTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellTemplateSelectorProperty =
            DependencyProperty.Register("CellTemplateSelector", typeof(DataTemplateSelector), typeof(DataGridBindingColumn), new PropertyMetadata(null));



        public DataTemplate CellEditingTemplate
        {
            get { return (DataTemplate)GetValue(CellEditingTemplateProperty); }
            set { SetValue(CellEditingTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellEditingTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellEditingTemplateProperty =
            DependencyProperty.Register("CellEditingTemplate", typeof(DataTemplate), typeof(DataGridBindingColumn), new PropertyMetadata(null));


        public DataTemplateSelector CellEditingTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(CellEditingTemplateSelectorProperty); }
            set { SetValue(CellEditingTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellEditingTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellEditingTemplateSelectorProperty =
            DependencyProperty.Register("CellEditingTemplateSelector", typeof(DataTemplateSelector), typeof(DataGridBindingColumn), new PropertyMetadata(null));

        #endregion

        #region Generate Element

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return this.LoadTemplateContent(cell, dataItem, true);
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return this.LoadTemplateContent(cell, dataItem, false);
        }

        #endregion

        /// <summary>
        /// LoadTempalteContent
        /// </summary>
        private FrameworkElement LoadTemplateContent(DataGridCell cell, object dataItem, bool isEditing)
        {
            DataTemplate template = null;
            DataTemplateSelector selector = null;

            this.ChooseCellTemplateAndSelector(isEditing, out template, out selector);

            var contentPresenter = new ContentPresenter()
            {
                ContentTemplate = template,
                ContentTemplateSelector = selector,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            cell.VerticalContentAlignment = VerticalAlignment.Stretch;
            cell.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            cell.VerticalAlignment = VerticalAlignment.Stretch;
            cell.HorizontalAlignment = HorizontalAlignment.Stretch;

            if (this.Binding != null)
            {
                BindingOperations.SetBinding(contentPresenter, ContentPresenter.ContentProperty, this.Binding);
                cell.PreviewKeyDown += this.OnPreviewKeyDown;
                cell.PreviewMouseLeftButtonDown += this.OnPrevMouseLeftButtonDown;
            }
            else
            {
                BindingOperations.ClearBinding(contentPresenter, ContentPresenter.ContentProperty);
                cell.PreviewKeyDown -= this.OnPreviewKeyDown;
                cell.PreviewMouseLeftButtonDown -= this.OnPrevMouseLeftButtonDown;
            }

            return contentPresenter;
        }

        /// <summary>
        /// CellTemplateセレクタ
        /// </summary>
        private void ChooseCellTemplateAndSelector(bool isEditing, out DataTemplate template, out DataTemplateSelector selector)
        {
            if(isEditing)
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

        /// <summary>
        /// セル編集開始前
        /// </summary>
        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            if ((editingElement as ContentPresenter)?.Content != null)
            {
                foreach (var child in EnumerateChildren(editingElement))
                {
                    switch (child)
                    {
                        case TextBox v:
                            v.Focus();
                            v.SelectAll();
                            v.Height = editingElement.Height;
                            break;

                        case ComboBox v:
                            v.Focus();
                            v.IsDropDownOpen = true;
                            v.Height = editingElement.Height;
                            break;

                        default:
                            break;
                    }
                }
            }

            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

        /// <summary>
        /// キー押下
        /// </summary>
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var cell = sender as DataGridCell;

            if (cell.IsEditing)
            {
                if (e.Key == Key.Escape)
                {
                    this.DataGridOwner?.CancelEdit();
                    e.Handled = true;
                }
            }
            else if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                // 読み取り専用のセルや列に対してBeginEditしないようにする
                if (cell.IsReadOnly || this.IsReadOnly)
                {
                    return;
                }

                if (this.IsBeginEditCharacter(e.Key) || this.IsBeginEditCharacter(e.ImeProcessedKey))
                {
                    this.DataGridOwner?.BeginEdit();

                    // ReferenceSource の DataGridTextBoxColumn を参考にした
                    //
                    // The TextEditor for the TextBox establishes contact with the IME
                    // engine lazily at background priority. However in this case we
                    // want to IME engine to know about the TextBox in earnest before
                    // PostProcessing this input event. Only then will the IME key be
                    // recorded in the TextBox. Hence the call to synchronously drain
                    // the Dispatcher queue.
                    //
                    this.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Background);
                }
            }

            if (e.Key == Key.Space)
            {
                e.Handled = this.CheckBoxEditAssist(cell);
            }
            else if (e.Key == Key.Delete)
            {
            }
        }

        /// <summary>
        /// マウスクリック
        /// </summary>
        private void OnPrevMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = this.CheckBoxEditAssist(sender as DataGridCell);
        }

        /// <summary>
        /// チェックボックス入力補助
        /// </summary>
        private bool CheckBoxEditAssist(DataGridCell cell)
        {
            if (cell == null || cell.IsReadOnly || !cell.IsSelected)
            {
                return false;
            }

            var checkBox = EnumerateChildren(cell).OfType<CheckBox>().FirstOrDefault();
            if (checkBox?.IsEnabled ?? false)
            {
                checkBox.IsChecked = !checkBox.IsChecked;
                return true;
            }

            return false;
        }

        /// <summary>
        /// セル編集開始キー判定
        /// </summary>
        private bool IsBeginEditCharacter(Key key)
        {
            var isCharacter = key >= Key.A && key <= Key.Z;
            var isNumber = (key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9);
            var isSpecial = key == Key.F2 || key == Key.Space;
            var isOem = (key >= Key.Oem1 && key <= Key.OemBackslash);
            var isBack = key == Key.Back;
            return isCharacter || isNumber || isSpecial || isBack || isOem;
        }

        /// <summary>
        /// VisualChildrenを列挙する
        /// </summary>
        private static IEnumerable<DependencyObject> EnumerateChildren(DependencyObject dp)
        {
            var count = VisualTreeHelper.GetChildrenCount(dp);
            var children = Enumerable.Range(0, count).Select(i => VisualTreeHelper.GetChild(dp, i));
            return children.Concat(children.SelectMany(i => EnumerateChildren(i)));
        }
    }
}

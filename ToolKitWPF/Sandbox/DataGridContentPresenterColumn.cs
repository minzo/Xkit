using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Toolkit.WPF.Sandbox
{
    public class DataGridContentPresenterColumn : DataGridBoundColumn
    {
        #region template

        public DataTemplate CellTemplate
        {
            get { return (DataTemplate)GetValue(CellTemplateProperty); }
            set { SetValue(CellTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellTemplateProperty =
            DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(DataGridContentPresenterColumn), new PropertyMetadata(null));



        public DataTemplateSelector CellTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(CellTemplateSelectorProperty); }
            set { SetValue(CellTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellTemplateSelectorProperty =
            DependencyProperty.Register("CellTemplateSelector", typeof(DataTemplateSelector), typeof(DataGridContentPresenterColumn), new PropertyMetadata(null));



        public DataTemplate CellEditingTemplate
        {
            get { return (DataTemplate)GetValue(CellEditingTemplateProperty); }
            set { SetValue(CellEditingTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellEditingTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellEditingTemplateProperty =
            DependencyProperty.Register("CellEditingTemplate", typeof(DataTemplate), typeof(DataGridContentPresenterColumn), new PropertyMetadata(null));


        public DataTemplateSelector CellEditingTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(CellEditingTemplateSelectorProperty); }
            set { SetValue(CellEditingTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellEditingTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellEditingTemplateSelectorProperty =
            DependencyProperty.Register("CellEditingTemplateSelector", typeof(DataTemplateSelector), typeof(DataGridContentPresenterColumn), new PropertyMetadata(null));

        #endregion

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return LoadTemplateContent(cell, dataItem, true);
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return LoadTemplateContent(cell, dataItem, false);
        }

        private class DataContextProxy {
            public object Proxy { get; }
            public DataContextProxy(object proxy) { Proxy = proxy; }
        }

        private FrameworkElement LoadTemplateContent(DataGridCell cell, object dataItem, bool isEditing)
        {
            DataTemplate template = null;
            DataTemplateSelector selector = null;

            ChooseCellTemplateAndSelector(isEditing, out template, out selector);

            var contentPresenter = new ContentPresenter()
            {
                Name = "CONTENT_PRESENTER",
                ContentTemplate = template,
                ContentTemplateSelector = selector,
                DataContext = new DataContextProxy(dataItem),
            };

            contentPresenter.Loaded += (s, e) => {
//                if (e.NewValue as DataContextProxy == null)
                {
                    (s as FrameworkElement).DataContext = new DataContextProxy(dataItem);
                    if (Binding != null)
                    {
                        BindingOperations.SetBinding(contentPresenter, ContentPresenter.ContentProperty, Binding);
                        cell.PreviewKeyDown += OnPreviewKeyDown;
                    }
                    else
                    {
                        BindingOperations.ClearBinding(contentPresenter, ContentPresenter.ContentProperty);
                        cell.PreviewKeyDown -= OnPreviewKeyDown;
                    }
                }
            };


            cell.VerticalContentAlignment = VerticalAlignment.Stretch;
            cell.HorizontalAlignment = HorizontalAlignment.Stretch;

            return contentPresenter;
        }

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


        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            void FindVisualChildren(DependencyObject dp)
            {
                if (VisualTreeHelper.GetChildrenCount(dp) > 0)
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
                            FindVisualChildren(VisualTreeHelper.GetChild(dp, 0));
                            return;
                    }
                }
            }


            var presenter = editingElement as ContentPresenter;

            if ( presenter?.Content != null)
            {
                FindVisualChildren(presenter);
            }

            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var cell = sender as DataGridCell;

            if( cell.IsEditing )
            {
                if(e.Key == Key.Escape)
                {
                    DataGridOwner?.CancelEdit();
                    e.Handled = true;
                }
            }
            else if(!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                if(IsBeginEditCharacter(e.Key) || IsBeginEditCharacter(e.ImeProcessedKey))
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

        public override object OnCopyingCellClipboardContent(object item)
        {
            return base.OnCopyingCellClipboardContent(item);
        }

        public override void OnPastingCellClipboardContent(object item, object cellContent)
        {
            base.OnPastingCellClipboardContent(item, cellContent);
        }
    }
}

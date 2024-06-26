﻿using System;
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
        /// <summary>
        /// トグルボタン系コントロール入力補助が有効か（セルクリック・スペースキーでの切り替え）
        /// </summary>
        public bool EnableToggleButtonAssist { get; set; } = true;

        #region Template

        public DataTemplate CellTemplate
        {
            get { return (DataTemplate)this.GetValue(CellTemplateProperty); }
            set { this.SetValue(CellTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellTemplateProperty =
            DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(DataGridBindingColumn), new PropertyMetadata(null));

        public DataTemplateSelector CellTemplateSelector
        {
            get { return (DataTemplateSelector)this.GetValue(CellTemplateSelectorProperty); }
            set { this.SetValue(CellTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellTemplateSelectorProperty =
            DependencyProperty.Register("CellTemplateSelector", typeof(DataTemplateSelector), typeof(DataGridBindingColumn), new PropertyMetadata(null));

        public DataTemplate CellEditingTemplate
        {
            get { return (DataTemplate)this.GetValue(CellEditingTemplateProperty); }
            set { this.SetValue(CellEditingTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellEditingTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellEditingTemplateProperty =
            DependencyProperty.Register("CellEditingTemplate", typeof(DataTemplate), typeof(DataGridBindingColumn), new PropertyMetadata(null));

        public DataTemplateSelector CellEditingTemplateSelector
        {
            get { return (DataTemplateSelector)this.GetValue(CellEditingTemplateSelectorProperty); }
            set { this.SetValue(CellEditingTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellEditingTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellEditingTemplateSelectorProperty =
            DependencyProperty.Register("CellEditingTemplateSelector", typeof(DataTemplateSelector), typeof(DataGridBindingColumn), new PropertyMetadata(null));

        #endregion

        #region Generate Element

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return this.OnGenerateElement(cell, dataItem);
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return this.OnGenerateElement(cell, dataItem);
        }

        private FrameworkElement OnGenerateElement(DataGridCell cell, object dataItem)
        {
            cell.VerticalContentAlignment = VerticalAlignment.Stretch;
            cell.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            cell.VerticalAlignment = VerticalAlignment.Stretch;
            cell.HorizontalAlignment = HorizontalAlignment.Stretch;

            cell.PreviewKeyDown -= this.OnPreviewKeyDown;
            cell.PreviewMouseLeftButtonDown -= this.OnPrevMouseLeftButtonDown;

            if (this.Binding != null)
            {
                cell.PreviewKeyDown += this.OnPreviewKeyDown;
                cell.PreviewMouseLeftButtonDown += this.OnPrevMouseLeftButtonDown;
            }

            this.ChooseCellTemplateAndSelector(cell.IsEditing, out DataTemplate template, out DataTemplateSelector selector);

            return this.LoadTemplateContent(cell, dataItem, template, selector);
        }

        #endregion

        /// <summary>
        /// LoadTempalteContent
        /// </summary>
        protected virtual FrameworkElement LoadTemplateContent(DataGridCell cell, object dataItem, DataTemplate template, DataTemplateSelector selector)
        {
            var contentPresenter = new ContentPresenter()
            {
                ContentTemplate = template,
                ContentTemplateSelector = selector,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            TrySetBinding(contentPresenter, ContentPresenter.ContentProperty, this.Binding);

            return contentPresenter;
        }

        /// <summary>
        /// CellTemplateセレクタ
        /// </summary>
        protected void ChooseCellTemplateAndSelector(bool isEditing, out DataTemplate template, out DataTemplateSelector selector)
        {
            if(isEditing)
            {
                template = this.CellEditingTemplate;
                selector = this.CellEditingTemplateSelector;
            }
            else
            {
                template = this.CellTemplate;
                selector = this.CellTemplateSelector;
            }
        }

        /// <summary>
        /// セル編集開始前
        /// </summary>
        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            bool TryPrepareControlForEdit(FrameworkElement frameworkElement, RoutedEventArgs routedEventArgs)
            {
                switch (frameworkElement)
                {
                    case TextBox v:
                        v.Focus();
                        v.SelectAll();

                        // マウスクリックだったらTextBoxのキャレットの位置をマウス位置にする
                        if (routedEventArgs is MouseButtonEventArgs mouseEventArgs)
                        {
                            var characterIndex = v.GetCharacterIndexFromPoint(Mouse.GetPosition(v), false);
                            if (characterIndex >= 0)
                            {
                                v.Select(characterIndex, 0);
                            }
                        }
                        return true;

                    case ComboBox v:
                        v.Focus();
                        v.IsDropDownOpen = true;
                        return true;

                    default:
                        return false;
                }
            }

            object GetEditTargetValue(DependencyObject dp)
            {
                switch (dp)
                {
                    case TextBox v:
                        return v.Text;

                    case ComboBox v:
                        if (BindingOperations.GetBinding(v, ComboBox.SelectedItemProperty) != null)
                        {
                            return v.SelectedItem;
                        }
                        else if (BindingOperations.GetBinding(v, ComboBox.SelectedValueProperty) != null)
                        {
                            return v.SelectedValue;
                        }
                        else
                        {
                            return v.Text;
                        }
                    case CheckBox v:
                        return v.IsChecked;
                    default:
                        return null;
                }
            }

            if (editingElement != null)
            {
                bool isPrepareSucceeded = false;

                // セルを編集開始したときにマウスのあった場所のコントロールを編集状態にする
                if (editingElement.InputHitTest(Mouse.GetPosition(editingElement)) is FrameworkElement element)
                {
                    while (element != null)
                    {
                        if (TryPrepareControlForEdit(element, editingEventArgs))
                        {
                            isPrepareSucceeded = true;
                            break;
                        }
                        
                        if (TryPrepareControlForEdit(element.TemplatedParent as FrameworkElement, editingEventArgs))
                        {
                            isPrepareSucceeded = true;
                            break;
                        }

                        element = element.Parent as FrameworkElement;
                    }
                }

                // 無ければVisualTreeで最初に見つかるコントロール
                if (!isPrepareSucceeded)
                {
                    foreach (var child in EnumerateChildren(editingElement).OfType<FrameworkElement>())
                    {
                        if (TryPrepareControlForEdit(child, editingEventArgs))
                        {
                            isPrepareSucceeded = true;
                            break;
                        }
                    }
                }

                // 編集前の値を収集する
                var uneditedValueDict = new Dictionary<DependencyObject, object>();
                foreach (var child in EnumerateChildren(editingElement))
                {
                    var value = GetEditTargetValue(child);
                    if (value != null)
                    {
                        uneditedValueDict.TryAdd(child, value);
                    }
                }

                return uneditedValueDict;
            }

            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

        /// <summary>
        /// セル編集破棄時処理
        /// </summary>
        protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
        {
            if ((editingElement as ContentPresenter)?.Content == null)
            {
                return;
            }

            if (uneditedValue is Dictionary<DependencyObject, object> uneditedValueDict)
            {
                foreach (var child in EnumerateChildren(editingElement))
                {
                    if (uneditedValueDict.TryGetValue(child, out var value))
                    {
                        switch (child)
                        {
                            case TextBox v:
                                v.SetCurrentValue(TextBox.TextProperty, value);
                                v.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                                break;

                            case ComboBox v:
                                var bindingExpression = v.GetBindingExpression(ComboBox.SelectedItemProperty);
                                if (bindingExpression != null)
                                {
                                    v.SetCurrentValue(ComboBox.SelectedItemProperty, value);
                                    bindingExpression.UpdateSource();
                                    break;
                                }

                                bindingExpression = v.GetBindingExpression(ComboBox.SelectedValueProperty);
                                if (bindingExpression != null)
                                {
                                    v.SetCurrentValue(ComboBox.SelectedValueProperty, value);
                                    bindingExpression.UpdateSource();
                                    break;
                                }

                                bindingExpression = v.GetBindingExpression(ComboBox.SelectedValueProperty);
                                if (bindingExpression != null)
                                {
                                    v.SetCurrentValue(ComboBox.TextProperty, value); ;
                                    bindingExpression?.UpdateSource();
                                    break;
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            base.CancelCellEdit(editingElement, uneditedValue);
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

                // スペースキーでトグルボタンの入力補助
                if (e.Key == Key.Space)
                {
                    e.Handled = this.ToggleButtonEditAssist(cell);
                    if(e.Handled)
                    {
                        return;
                    }
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
                    if (e.Key == Key.ImeProcessed)
                    {
                        this.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Background);
                    }
                }
            }
        }

        /// <summary>
        /// マウスクリック
        /// </summary>
        private void OnPrevMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = this.ToggleButtonEditAssist(sender as DataGridCell);
        }

        /// <summary>
        /// トグルボタン入力補助
        /// </summary>
        private bool ToggleButtonEditAssist(DataGridCell cell)
        {
            if (!this.EnableToggleButtonAssist)
            {
                return false;
            }

            if (cell == null || cell.IsReadOnly || !cell.IsSelected)
            {
                return false;
            }

            var control = EnumerateChildren(cell).OfType<Control>().FirstOrDefault();
            if(control is System.Windows.Controls.Primitives.ToggleButton toggleButton && toggleButton.IsEnabled)
            {
                toggleButton.IsChecked = !toggleButton.IsChecked;
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
        /// TrySetBinding
        /// </summary>
        protected static bool TrySetBinding(DependencyObject dependencyObject, DependencyProperty dependencyProperty, BindingBase binding)
        {
            if (binding != null)
            {
                BindingOperations.SetBinding(dependencyObject, dependencyProperty, binding);
                return true;
            }
            else
            {
                BindingOperations.ClearBinding(dependencyObject, dependencyProperty);
                return false;
            }
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

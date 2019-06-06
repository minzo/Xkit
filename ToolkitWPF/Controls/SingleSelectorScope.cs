using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Toolkit.WPF.Controls
{
    public class SingleSelectorScope
    {
        #region IsSingleSelectorScope

        private static bool GetIsSingleSelectorScope(DependencyObject obj)
        {           
            return (bool)obj.GetValue(IsSingleSelectorScopeProperty);
        }

        public static void SetIsSingleSelectorScope(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSingleSelectorScopeProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsSingleSelectorScope.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSingleSelectorScopeProperty =
            DependencyProperty.RegisterAttached("IsSingleSelectorScope", typeof(bool), typeof(SingleSelectorScope), new PropertyMetadata(false));

        #endregion

        #region IsSingleSelectorControl

        public static bool GetIsSingleSelectorControl(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSingleSelectorControlProperty);
        }

        public static void SetIsSingleSelectorControl(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSingleSelectorControlProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsSingleSelectorControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSingleSelectorControlProperty =
            DependencyProperty.RegisterAttached("IsSingleSelectorControl", typeof(bool), typeof(SingleSelectorScope), new PropertyMetadata(false, (d,e)=>
            {
                var root = EnumerateParent(d).FirstOrDefault(i => GetIsSingleSelectorScope(i));
                if(root == null)
                {
                    return;
                }

                if (e.NewValue.Equals(true))
                {
                    RegisterSingleSelectorControl(root, d);
                }
                else
                {
                    UnregisterSingleSelectorControl(root, d);
                }
            }));

        #endregion

        #region SingleSelectorControls

        private static List<DependencyObject> GetSingleSelectorControlList(DependencyObject obj)
        {
            var list = (List<DependencyObject>)obj.GetValue(SingleSelectorControlListProperty);
            if(list == null)
            {
                list = new List<DependencyObject>();
                obj.SetValue(SingleSelectorControlListProperty, list);
            }
            return list;
        }

        // Using a DependencyProperty as the backing store for SingleSelectorControls.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty SingleSelectorControlListProperty =
            DependencyProperty.RegisterAttached("SingleSelectorControlList", typeof(List<DependencyObject>), typeof(SingleSelectorScope), new PropertyMetadata(null));

        #endregion

        #region ScopeRoot

        private static DependencyObject GetScopeRoot(DependencyObject obj)
        {
            return (DependencyObject)obj.GetValue(ScopeRootProperty);
        }

        private static void SetScopeRoot(DependencyObject obj, DependencyObject value)
        {
            obj.SetValue(ScopeRootProperty, value);
        }

        // Using a DependencyProperty as the backing store for ScopeRootControl.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ScopeRootProperty =
            DependencyProperty.RegisterAttached("ScopeRoot", typeof(DependencyObject), typeof(SingleSelectorScope), new PropertyMetadata(null)); 

        #endregion


        /// <summary>
        /// ScopeRootに登録する
        /// </summary>
        private static void RegisterSingleSelectorControl(DependencyObject root, DependencyObject child)
        {
            GetSingleSelectorControlList(root).Add(child);
            SetScopeRoot(child, root);

            switch (child)
            {
                case MultiSelector v:
                    v.SelectionChanged += OnSelectionChanged;
                    break;
            }
        }

        /// <summary>
        /// ScopeRootから登録解除する
        /// </summary>
        private static void UnregisterSingleSelectorControl(DependencyObject root, DependencyObject child)
        {
            GetSingleSelectorControlList(root).Remove(child);
            SetScopeRoot(child, null);

            switch (child)
            {
                case MultiSelector v:
                    v.SelectionChanged -= OnSelectionChanged;
                    break;
            }
        }

        /// <summary>
        /// 選択状態の変更
        /// </summary>
        private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var root = GetScopeRoot(sender as DependencyObject);
            if (root != null)
            {
                var children = GetSingleSelectorControlList(root).Where(i => i != sender);

                foreach (var child in children.OfType<DataGrid>().Where(i => i.SelectionUnit != DataGridSelectionUnit.FullRow))
                {
                    child.SelectedCells.Clear();
                    child.UnselectAllCells();
                }

                foreach (var child in children.OfType<MultiSelector>())
                {
                    child.SelectedItems.Clear();
                    child.UnselectAll();
                }

                foreach (var child in children.OfType<ListBox>())
                {
                    child.SelectedItems.Clear();
                    child.UnselectAll();
                }
            }
        }

        /// <summary>
        /// VisualParentを列挙する
        /// </summary>
        private static IEnumerable<DependencyObject> EnumerateParent(DependencyObject dp)
        {
            while ((dp = VisualTreeHelper.GetParent(dp)) != null)
            {
                yield return dp;
            }
        }
    }
}

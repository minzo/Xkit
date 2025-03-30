using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.WPF.Sample
{
    public class Prop
    {
        public string Value1 { get; set; } = "Val1";

        public string Value2 { get; set; } = "Val2";
    }

    public struct Val
    {
        public string Value1 { get; set; } = "Val1";

        public string Value2 { get; set; } = "Val2";

        public Val() { }
    }

    public class Item
    {
        public string Name { get; set; }
        public string Value { get; set; }


        /// <summary>
        /// クラスのメンバーを直接 Binding すると
        /// メンバーが直接書き換わるため Prop というプロパティとしては何もセットされない
        /// </summary>
        public Prop Prop { get; set; } = new Prop();

        /// <summary>
        /// 構造体のメンバーを Binding しても
        /// 構造体のコピーによりGUI から編集した値を代入できない
        /// </summary>
        public Val Val { get; set; } = new Val();
    }

    internal class DataGridBindingTestWindowViewModel
    {
        public List<Item> Items { get; set; }

        public DataGridBindingTestWindowViewModel()
        {
            this.Items = new List<Item>();
            this.Items.Add(new Item() { Name = "0" });
            this.Items.Add(new Item() { Name = "1" });
            this.Items.Add(new Item() { Name = "2" });
        }
    }
}

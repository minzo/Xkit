using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Corekit.Extensions;
using System.Diagnostics;

namespace Corekit.Tests
{
    [TestClass]
    public class Sandbox
    {
        enum NodeType
        {
            Folder,
            Item,
        }

        [DebuggerDisplay("{NodeType} {Name}")]
        class Tree
        {
            public string Name { get; set; }

            public NodeType NodeType { get; set; }

            public ObservableCollection<Tree> Children { get; }

            public Tree(string name, NodeType type, IEnumerable<Tree> children = null)
            {
                this.Name = name;
                this.NodeType = type;
                if(children != null)
                {
                    this.Children = new ObservableCollection<Tree>(children);
                }
                else
                {
                    this.Children = new ObservableCollection<Tree>();
                }
            }

            public Tree()
            {
                Children = new ObservableCollection<Tree>();
            }
        }

        class TreeInfo
        {
            public int ChildStartIndex = -1;
            public int ChildEndIndex = -1;
            public int ParentIndex = -1;
        }

        Dictionary<Tree, TreeInfo> dict = new Dictionary<Tree, TreeInfo>();

        Tree Root;

        [TestMethod]
        public void Test()
        {
            var flat =  Root
                .EnumerateTreeBreadthFirst(i => i.Children);

            var flat2 = EnumerateInfo(Root);
        }

        private IEnumerable<Tree> EnumerateInfo(Tree node)
        {
            var index = -1;
            var list = new List<Tree>();
            var infos = new List<TreeInfo>();

            void EnqueueChildren(Tree tree, int parentIndex)
            {
                foreach (var child in tree.Children)
                {
                    if (child.NodeType == NodeType.Folder)
                    {
                        EnqueueChildren(child, parentIndex);
                    }
                    else
                    {
                        list.Add(child);
                        infos.Add(new TreeInfo() { ParentIndex = parentIndex });
                    }
                }
            }

            EnqueueChildren(node, -1);
            index = 0;

            while (list.Count - index > 0)
            {
                var item = list[index];
                var info = infos[index];

                info.ChildStartIndex = list.Count;
                EnqueueChildren(item, index);
                info.ChildEndIndex = list.Count - 1;
                index++;
            }
            
            return list;
        }

        [TestInitialize]
        public void Initialize()
        {
            Root = new Tree("Root", NodeType.Item, new[] { 
                new Tree("A1-1", NodeType.Item, new[]{
                    new Tree("A3", NodeType.Folder, new[]{
                        new Tree("A6-3", NodeType.Item),
                        new Tree("A7", NodeType.Folder, new[]{
                            new Tree("A13-4", NodeType.Item),
                            new Tree("A14-5", NodeType.Item),
                            new Tree("A15-6", NodeType.Item),
                            new Tree("A16-7", NodeType.Item),
                        }),
                        new Tree("A8-8", NodeType.Item),
                        new Tree("A9", NodeType.Folder, new[]{
                            new Tree("A17-9", NodeType.Item),
                            new Tree("A18-10", NodeType.Item),
                            new Tree("A19-11", NodeType.Item),
                            new Tree("A20-12", NodeType.Item),
                        }),
                    }),
                }),
                new Tree("B2-2", NodeType.Item, new[]{
                    new Tree("B4-13", NodeType.Item, new[]{
                        new Tree("B10-15", NodeType.Item),
                        new Tree("B11-16", NodeType.Item),
                        new Tree("B12-17", NodeType.Item),
                    }),
                    new Tree("B5-14", NodeType.Item),
                }),
            });
        }
    }
}

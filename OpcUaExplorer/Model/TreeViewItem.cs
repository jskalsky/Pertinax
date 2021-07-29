using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUaExplorer.Model
{
    public class TreeViewItem
    {
        public TreeViewItem(string name)
        {
            Name = name;
            Children = new ObservableCollection<TreeViewItem>();
            Tag = null;
        }
        public ObservableCollection<TreeViewItem> Children { get; }
        public string Name { get; }
        public BrowseItem Tag { get; set; }

        public void AddChild(string name, BrowseItem bi)
        {
            TreeViewItem child = new TreeViewItem(name);
            child.Tag = bi;
            Children.Add(child);
        }
    }
}

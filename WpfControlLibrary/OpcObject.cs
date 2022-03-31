using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary
{
    public class OpcObject : INotifyPropertyChanged
    {
        private const string DefaultName = "Pertinax";
        private const string DefaultItemName = "Var";
        private readonly ObservableCollection<OpcObjectItem> _items = new ObservableCollection<OpcObjectItem>();
        private string _name;
        private bool _publish;
        private bool _enablePublish;
        private bool _isImported;
        private OpcObjectItem _selectedItem;
        private static int _nextDefaultNameIndex = 1;
        private int _nextItemIndex = 1;
        public OpcObject()
        {
            Name = $"{DefaultName}{_nextDefaultNameIndex++}";
            Publish = false;
            IsImported = false;
        }
        public OpcObject(string name, bool publish, bool imported)
        {
            Name = name;
            int index = GetDefaultIndex(name, DefaultName);
            if(index > 0 && index >= _nextDefaultNameIndex)
            {
                _nextDefaultNameIndex = index + 1;
            }
            Publish = publish;
            IsImported = imported;
        }

        public OpcObject(bool publish, bool imported) : this()
        {
            Publish = publish;
            IsImported = imported;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        public bool Publish
        {
            get { return _publish; }
            set { _publish = value; OnPropertyChanged("Publish"); }
        }

        public bool EnablePublish
        {
            get { return _enablePublish; }
            set { _enablePublish = value; OnPropertyChanged("EnablePublish"); }
        }

        public bool IsImported
        {
            get { return _isImported; }
            set { _isImported = value; OnPropertyChanged("IsImported"); }
        }

        public OpcObjectItem SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged("SelectedItem"); }
        }
        public ObservableCollection<OpcObjectItem> Items => _items;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public OpcObjectItem AddItem()
        {
            return AddItem($"{DefaultItemName}{_nextItemIndex}");
        }
        public OpcObjectItem AddItem(string name)
        {
            int index = GetDefaultIndex(name,DefaultItemName);
            if (index > 0 && index >= _nextItemIndex)
            {
                _nextItemIndex = index + 1;
            }
            OpcObjectItem ooi = new OpcObjectItem(name, Publish);
            _items.Add(ooi);
            return ooi;
        }

        public void AddItem(OpcObjectItem ooi)
        {
            int index = GetDefaultIndex(ooi.Name, DefaultItemName);
            if (index > 0 && index >= _nextItemIndex)
            {
                _nextItemIndex = index + 1;
            }
            _items.Add(ooi);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public string GetDefaultName()
        {
            return $"{DefaultName}{_nextDefaultNameIndex++}";
        }

        public int GetDefaultIndex(string name, string defaultName)
        {
            LinkedList<char> ll = new LinkedList<char>();
            string text = string.Empty;
            for (int i = name.Length - 1; i >= 0; --i)
            {
                if (char.IsDigit(name[i]))
                {
                    ll.AddFirst(name[i]);
                }
                else
                {
                    text = name.Substring(0, i + 1);
                    break;
                }
            }
            if (ll.Count != 0)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return -1;
                }
                if (text == defaultName)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach(char ch in ll)
                    {
                        sb.Append(ch);
                    }
                    return int.Parse(sb.ToString());
                }
            }
            return -1;
        }
    }
}

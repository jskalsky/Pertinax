using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfControlLibrary.View
{
    /// <summary>
    /// Interaction logic for VariableProperties.xaml
    /// </summary>
    public partial class VariableProperties : UserControl, INotifyPropertyChanged
    {
        private readonly string[] _basicTypes = new string[] { "Boolean", "UInt8", "Int8", "UInt16", "Int16", "UInt32", "Int32", "Float", "Double" };
        private string _selectedBasicType;
        private readonly string[] _access = new string[] { "Read", "Write", "ReadWrite" };
        private string _selectedAccess;
        private readonly string[] _kind = new string[] { "Jednoduchá proměnná", "Pole", "Objekt" };
        private string _selectedKind;
        private string _varName;
        private string _varId;
        private int _varCount;
        private int _arrayLength;

        private bool _enableArrayLength;
        private bool _enableObjectName;
        private bool _enableVarName;
        private bool _enableVarId;
        private bool _enableBasicType;
        private bool _enableAccess;

        private Visibility _visSimple;
        private Visibility _visArray;
        private Visibility _visObject;
        private Visibility _visId;

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler KindChanged;

        public VariableProperties()
        {
            Debug.Print("100");
            InitializeComponent();
            Debug.Print("101");
            ComboKind.ItemsSource = _kind;
            ComboType.ItemsSource = _basicTypes;
            ComboAccess.ItemsSource = _access;
            Debug.Print("102");
        }

        public static readonly DependencyProperty VarKindProperty = DependencyProperty.Register("VarKind", typeof(string), typeof(VariableProperties),
            new PropertyMetadata(string.Empty, OnVarKindPropertyChanged));
        public string VarKind
        {
            get { Debug.Print($"Get Depend {(string)GetValue(VarKindProperty)}"); return (string)GetValue(VarKindProperty); }
            set { SetValue(VarKindProperty, value); Debug.Print($"Set Depend {value};"); }
        }
        private static void OnVarKindPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Debug.Print($"VarKindPropertyChanged {dependencyObject}, {e.NewValue}");
            if (dependencyObject is VariableProperties varp)
            {
                varp.ComboKind.SelectedItem = (string)e.NewValue;
                varp.ServiceVisible(varp.SelectedKind);
                varp.KindChanged?.Invoke(varp, new PropertyChangedEventArgs("KindProperty"));
            }
        }

        public static readonly DependencyProperty VarBasicTypeProperty = DependencyProperty.Register("VarBasicType", typeof(string), typeof(VariableProperties),
            new PropertyMetadata(string.Empty, OnVarBasicTypePropertyChanged));
        public string VarBasicType
        {
            get { Debug.Print($"Get Depend {(string)GetValue(VarBasicTypeProperty)}"); return (string)GetValue(VarBasicTypeProperty); }
            set { SetValue(VarBasicTypeProperty, value); Debug.Print($"Set Depend {value};"); }
        }
        private static void OnVarBasicTypePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Debug.Print($"VarBasicTypePropertyChanged {dependencyObject}, {e.NewValue}");
            if (dependencyObject is VariableProperties varp)
            {
                varp.ComboType.SelectedItem = (string)e.NewValue;
            }
        }

        public static readonly DependencyProperty VarAccessProperty = DependencyProperty.Register("VarAccess", typeof(string), typeof(VariableProperties),
            new PropertyMetadata(string.Empty, OnVarAccessPropertyChanged));
        public string VarAccess
        {
            get { Debug.Print($"Get Depend {(string)GetValue(VarAccessProperty)}"); return (string)GetValue(VarAccessProperty); }
            set { SetValue(VarAccessProperty, value); Debug.Print($"Set Depend {value};"); }
        }
        private static void OnVarAccessPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Debug.Print($"VarAccessPropertyChanged {dependencyObject}, {e.NewValue}");
            if (dependencyObject is VariableProperties varp)
            {
                varp.ComboAccess.SelectedItem = (string)e.NewValue;
            }
        }

        public static readonly DependencyProperty VarNameProperty = DependencyProperty.Register("VarName", typeof(string), typeof(VariableProperties),
            new PropertyMetadata(string.Empty, OnVarNamePropertyChanged));
        public string VarName
        {
            get { Debug.Print($"Get Depend {(string)GetValue(VarNameProperty)}"); return (string)GetValue(VarNameProperty); }
            set { SetValue(VarNameProperty, value); Debug.Print($"Set Depend {value};"); }
        }
        private static void OnVarNamePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Debug.Print($"VarNamePropertyChanged {dependencyObject}, {e.NewValue}");
            if (dependencyObject is VariableProperties varp)
            {
                varp.TextBoxName.Text = (string)e.NewValue;
            }
        }

        public static readonly DependencyProperty VarIdProperty = DependencyProperty.Register("VarId", typeof(string), typeof(VariableProperties),
            new PropertyMetadata(string.Empty, OnVarIdPropertyChanged));
        public string VarId
        {
            get { Debug.Print($"Get Depend {(string)GetValue(VarIdProperty)}"); return (string)GetValue(VarIdProperty); }
            set { SetValue(VarIdProperty, value); Debug.Print($"Set Depend {value};"); }
        }
        private static void OnVarIdPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Debug.Print($"VarIdPropertyChanged {dependencyObject}, {e.NewValue}");
            if (dependencyObject is VariableProperties varp)
            {
                varp.TextBoxId.Text = (string)e.NewValue;
            }
        }

        public static readonly DependencyProperty VisibleIdProperty = DependencyProperty.Register("VisibleId", typeof(Visibility), typeof(VariableProperties),
            new PropertyMetadata(Visibility.Visible, OnVisibleIdPropertyChanged));
        public Visibility VisibleId
        {
            get { return (Visibility)GetValue(VisibleIdProperty); }
            set { SetValue(VisibleIdProperty, value); }
        }
        private static void OnVisibleIdPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Debug.Print($"VisibleIdPropertyChanged {dependencyObject}, {e.NewValue}");
            if (dependencyObject is VariableProperties varp)
            {
                varp.LabelId.Visibility = (Visibility)e.NewValue;
                varp.TextBoxId.Visibility = (Visibility)e.NewValue;
                varp.LabelName.Visibility = (Visibility)e.NewValue;
                varp.TextBoxName.Visibility = (Visibility)e.NewValue;
            }
        }

        public string[] BasicTypes
        {
            get { return _basicTypes; }
        }
        public string SelectedBasicType
        {
            get { return _selectedBasicType; }
            set { _selectedBasicType = value; OnPropertyChanged("SelectedBasicType"); }
        }
        public string[] Access
        {
            get { return _access; }
        }
        public string SelectedAccess
        {
            get { return _selectedAccess; }
            set { _selectedAccess = value; }
        }
        public string[] Kind
        {
            get { return _kind; }
        }
        public string SelectedKind
        {
            get { return _selectedKind; }
            set { _selectedKind = value; OnPropertyChanged("SelectedKind"); }
        }


        public int VarCount
        {
            get { return _varCount; }
            set
            {
                _varCount = value;
            }
        }

        public int ArrayLength
        {
            get { return _arrayLength; }
            set
            {
                _arrayLength = value;
            }
        }

        public Visibility VisSimple
        {
            get { return _visSimple; }
            set { _visSimple = value; OnPropertyChanged("VisSimple"); }
        }

        public Visibility VisArray
        {
            get { return _visArray; }
            set { _visArray = value; OnPropertyChanged("VisArray"); }
        }
        public Visibility VisObject
        {
            get { return _visObject; }
            set { _visObject = value; OnPropertyChanged("VisObject"); }
        }
        public Visibility VisId
        {
            get { return _visId; }
            set { _visId = value; OnPropertyChanged("VisId"); }
        }

        private void ServiceVisible(string kind)
        {
            Debug.Print($"selectedKind= {kind}, {Kind[0]}, {Kind[1]}, {Kind[2]}");
            if(string.IsNullOrEmpty(kind))
            {
                return;
            }
            LabelAccess.Visibility = Visibility.Collapsed;
            LabelArrayLength.Visibility = Visibility.Collapsed;
            LabelObjectName.Visibility = Visibility.Collapsed;
            LabelId.Visibility = Visibility.Collapsed;
            LabelName.Visibility = Visibility.Collapsed;
            LabelType.Visibility = Visibility.Collapsed;

            ComboAccess.Visibility = Visibility.Collapsed;
            ComboObject.Visibility = Visibility.Collapsed;
            ComboType.Visibility = Visibility.Collapsed;
            TextBoxId.Visibility = Visibility.Collapsed;
            TextBoxName.Visibility = Visibility.Collapsed;
            NudArrayLength.Visibility = Visibility.Collapsed;
            Debug.Print("600");
            if (kind == Kind[0] || kind == Kind[1])
            {
                Debug.Print("601");
                LabelType.Visibility = Visibility.Visible;
                ComboType.Visibility = Visibility.Visible;
                LabelAccess.Visibility = Visibility.Visible;
                ComboAccess.Visibility = Visibility.Visible;
                LabelName.Visibility = Visibility.Visible;
                TextBoxName.Visibility = Visibility.Visible;
                LabelId.Visibility = Visibility.Visible;
                TextBoxId.Visibility=Visibility.Visible;    
                if (kind == Kind[1])
                {
                    Debug.Print("602");
                    LabelArrayLength.Visibility = Visibility.Visible;
                    NudArrayLength.Visibility = Visibility.Visible;
                }
                Debug.Print("603");
            }
            else
            {
                if (kind == Kind[2])
                {
                    Debug.Print("604");
                    LabelObjectName.Visibility= Visibility.Visible;
                    ComboObject.Visibility= Visibility.Visible;
                    LabelName.Visibility = Visibility.Visible;
                    TextBoxName.Visibility = Visibility.Visible;
                    LabelId.Visibility = Visibility.Visible;
                    TextBoxId.Visibility = Visibility.Visible;
                    Debug.Print("605");
                }
            }
        }
        private void Kind_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.Print($"Kind_SelectionChanged {sender}, {e.AddedItems.Count}");
            if (e.AddedItems.Count == 1)
            {
                if (e.AddedItems[0] is string selectedKind)
                {
                    ServiceVisible(selectedKind);
                }
            }
            e.Handled = true;
            Debug.Print("610");
        }
        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

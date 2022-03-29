using OpcUaPars;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConfigOpcUa
{
    public class ConfigOpcUa : ConfigPtx.CfgPtx
    {
        private const int InitialCapacity = 256;

        private readonly Dictionary<string, byte> _basicTypes = new Dictionary<string, byte>() { { "Boolean", 0 }, { "UInt8", 1 }, { "UInt16", 2 }, { "UInt32", 3 }, { "Int8", 4 }, { "Int16", 5 },
            {"Int32", 6 }, {"Float", 7 }, {"Double", 8 } };
        private readonly Dictionary<string, char> _ptxBasicTypes = new Dictionary<string, char>() { {"Boolean", 'B' }, { "UInt8", 'U' }, { "UInt16", 'W' }, { "UInt32", 'Q' }, { "Int8", 'C' },
            { "Int16", 'I' }, {"Int32", 'L' }, {"Float", 'R' }, {"Double", 'D' } };
        private readonly Dictionary<char, byte> _ptxTypeCode = new Dictionary<char, byte>() { {'B', 1 }, {'U', 3 }, { 'W', 5 }, {'Q', 7 }, { 'C', 2 },
            { 'I', 4 }, {'L', 6 }, {'R', 8 }, {'D', 11 } };
        private readonly Dictionary<string, byte> _access = new Dictionary<string, byte>() { { "Read", 0 }, { "Write", 1 }, { "ReadWrite", 2 } };

        private readonly List<WpfControlLibrary.OpcObject> _objects;
        private string _localIpAddress;
        private string _groupAddress;
        private int _publisherId;
        private readonly List<WpfControlLibrary.SubscriberItem> _subscriberItems;
        private readonly List<WpfControlLibrary.PublisherItem> _publisherItems;

        private readonly List<WpfControlLibrary.PortsNode> _ports;
        private readonly Dictionary<string, string> _allPorts;
        public ConfigOpcUa()
        {
            lName = "OpcUa";
            lDescription = "OpcUa";
            lVersion = "1.0.0.0";
            string appFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "Pertinax";
            if (!Directory.Exists(appFolder)) Directory.CreateDirectory(appFolder);
#if DEBUG
            FileStream myTraceLog = new FileStream(appFolder + System.IO.Path.DirectorySeparatorChar + "ConfigOpcUa.deb", FileMode.Create, FileAccess.Write, FileShare.Write);
            TextWriterTraceListener myListener = new TextWriterTraceListener(myTraceLog);
            Debug.Listeners.Add(myListener);
            Debug.AutoFlush = true;
            Debug.WriteLine("Start");
#endif
            _objects = new List<WpfControlLibrary.OpcObject>();
            _subscriberItems = new List<WpfControlLibrary.SubscriberItem>();
            _publisherItems = new List<WpfControlLibrary.PublisherItem>();
            _ports = new List<WpfControlLibrary.PortsNode>();
            _allPorts = new Dictionary<string, string>();
            _publisherId = 1;
        }

        public static void IntelMotorola(byte[] bytes, int start, int length)
        {
            for (int i = 0; i < length / 2; ++i)
            {
                byte b = bytes[start + i];
                bytes[start + i] = bytes[start + length - i - 1];
                bytes[start + length - i - 1] = b;
            }
        }

        public static int IntelMotorola(int val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            IntelMotorola(bytes, 0, 4);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static ushort IntelMotorola(ushort val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            IntelMotorola(bytes, 0, 2);
            return BitConverter.ToUInt16(bytes, 0);
        }

        public static uint IntelMotorola(uint val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            IntelMotorola(bytes, 0, 4);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static double IntelMotorola(double val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            IntelMotorola(bytes, 0, 8);
            return BitConverter.ToDouble(bytes, 0);
        }


        private string GetBasicType(byte b)
        {
            foreach (KeyValuePair<string, byte> pair in _basicTypes)
            {
                if (pair.Value == b)
                {
                    return pair.Key;
                }
            }
            return null;
        }
        private string GetAccess(byte b)
        {
            foreach (KeyValuePair<string, byte> pair in _access)
            {
                if (pair.Value == b)
                {
                    return pair.Key;
                }
            }
            return null;
        }

        public override void CheckProject(string pName, string pErrName)
        {
            Debug.Print($"CheckProject {pName}, {pErrName}");
        }

        public override byte[] CheCoLabel(int mod, string pLabel)
        {
            Debug.Print($"CheCoLabel {mod}, {pLabel}");
            if (!_allPorts.TryGetValue(pLabel, out string port))
            {
                throw new ApplicationException($"Unknown label {pLabel}");
            }
            string[] items = port.Split('.');
            for (int i = 0; i < items.Length; ++i)
            {
                Debug.Print($"item {i}, {items[i]}");
            }
            if (!_ptxTypeCode.TryGetValue(items[2][0], out byte tc))
            {
                throw new ApplicationException($"Bad type {pLabel}");
            }
            if (mod != 1)
            {
                return new byte[] { };
            }
            MemoryStream ms = new MemoryStream(InitialCapacity);
            BinaryWriter bw = new BinaryWriter(ms);

            ushort compiledType = 0;
            if (items[3] == "Pub")
            {
                compiledType = 2;
            }
            else
            {
                if (items[3] == "Sub")
                {
                    compiledType = 3;
                }
            }
            ushort compiledIndex = 0;
            if (items[4].Contains("Sub") || items[4].Contains("Pub"))
            {
                compiledIndex = ushort.Parse(items[4].Remove(0, 3));
                Debug.Print($"compiledIndex= {compiledIndex}");
            }
            bw.Write(tc);
            bw.Write(IntelMotorola(-1));
            bw.Write(IntelMotorola(compiledType));
            bw.Write(IntelMotorola(compiledIndex));
            bw.Write((uint)0);
            bw.Write((ushort)0);
            bw.Write(Encoding.ASCII.GetBytes(items[5]));
            bw.Close();
            return ms.ToArray();
        }

        public override string CreatePort(System.Windows.Forms.IWin32Window hWnd)
        {
            Debug.Print($"CreatePort");
            WpfControlLibrary.PortDialog pd = new WpfControlLibrary.PortDialog();
            if (pd.DataContext is WpfControlLibrary.ViewModelPorts vmp)
            {
                vmp.RootNodes.Clear();
                foreach (WpfControlLibrary.PortsNode pn in _ports)
                {
                    vmp.RootNodes.Add(pn);
                }
            }
            pd.ShowDialog();
            if (pd.SelectedPort != null)
            {
                Debug.Print($"pd.SelectedPort= {pd.SelectedPort}");
                return pd.SelectedPort;
            }
            return string.Empty;
        }

        private WpfControlLibrary.OpcObject FindOpcObject(string name)
        {
            foreach (WpfControlLibrary.OpcObject oo in _objects)
            {
                if (oo.Name == name)
                {
                    return oo;
                }
            }
            return null;
        }
        private void AddItemsToObject(ObjectTypeType ott, WpfControlLibrary.OpcObject oo)
        {
            foreach (VariablesType vt in ott.Variables)
            {
                WpfControlLibrary.OpcObjectItem ooi = new WpfControlLibrary.OpcObjectItem(vt.Name);
                ooi.SelectedBasicType = GetBasicType(vt.BasicType);
                ooi.SelectedAccess = GetAccess(vt.AccessType);
                ooi.ArraySizeValue = vt.ArraySize;
                ooi.SelectedRank = (vt.Type == 0) ? "SimpleVariable" : "Array";
                Debug.Print($"Item {vt.Name}");
                oo.AddItem(ooi);
            }
        }

        private void Import(string path, string objectName, WpfControlLibrary.MainViewModel mvm = null)
        {
            if (File.Exists(path))
            {
                XmlSerializer serializerOpc = new XmlSerializer(typeof(OPCUAParametersType));
                using (TextReader tr = new StreamReader(path))
                {
                    OPCUAParametersType parsOpc = (OPCUAParametersType)serializerOpc.Deserialize(tr);
                    foreach(SubscriberType subscriberType in parsOpc.Subscriber)
                    {
                        if(subscriberType.PublisherRootType == objectName)
                        {
                            foreach (ObjectTypeType ott in parsOpc.ObjectType)
                            {
                                if (ott.Name == objectName)
                                {
                                    string[] items = ott.Description.Split(';');
                                    bool publish = false;
                                    bool isImported = false;
                                    if (items.Length == 2)
                                    {
                                        bool.TryParse(items[0], out publish);
                                        bool.TryParse(items[1], out isImported);
                                    }
                                    WpfControlLibrary.OpcObject oo = new WpfControlLibrary.OpcObject(ott.Name, publish, true);
                                    if (oo != null)
                                    {
                                        AddItemsToObject(ott, oo);
                                        _objects.Add(oo);
                                        if(mvm != null)
                                        {
                                            mvm.Objects.Add(oo);
                                        }
                                    }
                                    string[] itemsSub = subscriberType.Description.Split(';');
                                    if(items.Length == 5)
                                    {
                                        int writerId = 0;
                                        int.TryParse(itemsSub[2], out writerId);
                                        int datasetId = 0;
                                        int.TryParse(itemsSub[3], out datasetId);
                                        WpfControlLibrary.SubscriberItem si = new WpfControlLibrary.SubscriberItem(path, objectName, parsOpc.PublisherId, writerId, datasetId);
                                        _subscriberItems.Add(si);
                                        if(mvm != null)
                                        {
                                            mvm.SubscriberObjects.Add(si);
                                        }
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public override void LoadConfig(string pName)
        {
            Debug.Print($"LoadConfig= {pName}");
            _objects.Clear();
            _localIpAddress = string.Empty;
            _groupAddress = string.Empty;
            _publisherId = 1;
            _subscriberItems.Clear();
            _publisherItems.Clear();
            _ports.Clear();
            _allPorts.Clear();
            if (File.Exists(pName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(OPCUAParametersType));
                using (TextReader tr = new StreamReader(pName))
                {
                    OPCUAParametersType pars = (OPCUAParametersType)serializer.Deserialize(tr);

                    foreach (ObjectTypeType ott in pars.ObjectType)
                    {
                        string[] objectItems = ott.Description.Split(';');
                        bool publish = false;
                        bool isImported = false;
                        if (objectItems.Length == 2)
                        {
                            bool.TryParse(objectItems[0], out publish);
                            bool.TryParse(objectItems[1], out isImported);
                        }
                        WpfControlLibrary.OpcObject oo = new WpfControlLibrary.OpcObject(ott.Name, publish, isImported);
                        Debug.Print($"object= {oo.Name}");
                        if (oo == null)
                        {
                            continue;
                        }
                        AddItemsToObject(ott, oo);
                        _objects.Add(oo);
                    }

                    if (pars.UsePublisher)
                    {
                        _publisherId = (int)pars.PublisherId;
                        Debug.Print($"_publisherId= {_publisherId}");
                        foreach (SubscriberType subscriberType in pars.Subscriber)
                        {
                            string[] items = subscriberType.Description.Split(';');
                            int writerId = 0;
                            int dataSetWriter = 0;
                            if (items.Length == 5)
                            {
                                if (string.IsNullOrEmpty(_localIpAddress))
                                {
                                    _localIpAddress = items[0];
                                }
                                if (string.IsNullOrEmpty(_groupAddress))
                                {
                                    _groupAddress = items[1];
                                }
                                int.TryParse(items[2], out writerId);
                                int.TryParse(items[3], out dataSetWriter);
                            }
                            WpfControlLibrary.OpcObject opcObject = FindOpcObject(items[4]);
                            if (opcObject != null)
                            {
                                WpfControlLibrary.PublisherItem pi = new WpfControlLibrary.PublisherItem(opcObject, _publisherId, writerId, dataSetWriter, subscriberType.SendPeriod);
                                _publisherItems.Add(pi);
                            }
                        }
                    }
                    if (pars.UseSubscriber)
                    {
                        Debug.Print($"Subscriber {_subscriberItems}");
                        foreach (PublisherType pt in pars.Publisher)
                        {
                            string[] items = pt.Description.Split(';');
                            if (items.Length == 5)
                            {
                                if (string.IsNullOrEmpty(_localIpAddress))
                                {
                                    _localIpAddress = items[0];
                                }
                                if (string.IsNullOrEmpty(_groupAddress))
                                {
                                    _groupAddress = items[1];
                                }
                                string path = items[2];
                                string objectName = items[3];
                                Import(path, objectName);
                            }
                        }
                    }
                    CreatePorts();
                }
            }
        }

        private void CreatePorts()
        {
            _ports.Clear();
            WpfControlLibrary.PortsNode outputs = new WpfControlLibrary.PortsNode("Outputs");
            WpfControlLibrary.PortsNode inputs = new WpfControlLibrary.PortsNode("Inputs");
            _ports.Add(outputs);
            _ports.Add(inputs);
            foreach (WpfControlLibrary.OpcObject oo in _objects)
            {
                if (oo.Publish)
                {
                    WpfControlLibrary.PortsNode obj = outputs.Add(oo.Name);
                    foreach (WpfControlLibrary.OpcObjectItem ooi in oo.Items)
                    {
                        if (_ptxBasicTypes.TryGetValue(ooi.SelectedBasicType, out char typeChar))
                        {
                            string portText = $"O.OPCUA.{typeChar}.Pub.Sub1.{ooi.Name}({oo.Name})";
                            Debug.Print($"porText= {portText}");
                            obj.Add(portText);
                            _allPorts[portText.ToUpperInvariant()] = portText;
                        }
                    }
                }
                else
                {
                    /*                    if(oo.Subscribe)
                                        {
                                            WpfControlLibrary.PortsNode obj = inputs.Add(oo.Name);
                                            foreach (WpfControlLibrary.OpcObjectItem ooi in oo.Items)
                                            {
                                                if (_ptxBasicTypes.TryGetValue(ooi.SelectedBasicType, out char typeChar))
                                                {
                                                    string portText = $"I.OPCUA.{typeChar}.Pub.Sub1.{ooi.Name}({oo.Name})";
                                                    Debug.Print($"porText= {portText}");
                                                    obj.Add(portText);
                                                    _allPorts[portText.ToUpperInvariant()] = portText;
                                                }
                                            }
                                        }*/
                }
            }
        }
        public override void MakeConfig(System.Windows.Forms.IWin32Window hWnd, string pName)
        {
            Debug.Print($"MakeConfig {pName}");
            LoadConfig(pName);
            WpfControlLibrary.MainWindow mainWindow = new WpfControlLibrary.MainWindow();
            if (mainWindow.DataContext is WpfControlLibrary.MainViewModel vm)
            {
                vm.PropertyChanged += Vm_PropertyChanged;
                if (!string.IsNullOrEmpty(_localIpAddress))
                {
                    vm.LocalIpAddressString = _localIpAddress;
                }
                if (!string.IsNullOrEmpty(_groupAddress))
                {
                    vm.GroupAddressString = _groupAddress;
                }
                vm.Objects.Clear();
                Debug.Print($"Objects= {_objects.Count}");
                foreach (WpfControlLibrary.OpcObject oo in _objects)
                {
                    vm.Objects.Add(oo);
                }
                if (vm.Objects.Count > 0)
                {
                    vm.SelectedOpcObject = vm.Objects[0];
                }
                vm.PublisherId = _publisherId;

                foreach (WpfControlLibrary.SubscriberItem si in _subscriberItems)
                {
                    vm.SubscriberObjects.Add(si);
                }
                if (vm.SubscriberObjects.Count != 0)
                {
                    vm.SelectedSubscriberItem = vm.SubscriberObjects[0];
                }
                foreach(WpfControlLibrary.PublisherItem pi in _publisherItems)
                {
                    vm.PublisherObjects.Add(pi);
                }
                vm.WindowTitle = $"Configurator OpcUa - {pName}";
            }

            if ((bool)mainWindow.ShowDialog())
            {
                if (mainWindow.DataContext is WpfControlLibrary.MainViewModel mvm)
                {
                    SaveConfiguration(pName, mvm);
                }
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug.Print($"Vm_PropertyChanged {e.PropertyName}, {sender}");
            if (e.PropertyName == "SubscriberPath")
            {
                if (sender is WpfControlLibrary.MainViewModel mvm)
                {
                    List<string> objectNames = new List<string>();
                    if (File.Exists(mvm.SubscriberPath))
                    {
                        XmlSerializer serializerOpc = new XmlSerializer(typeof(OPCUAParametersType));
                        using (TextReader tr = new StreamReader(mvm.SubscriberPath))
                        {
                            OPCUAParametersType parsOpc = (OPCUAParametersType)serializerOpc.Deserialize(tr);
                            foreach (ObjectTypeType ott in parsOpc.ObjectType)
                            {
                                objectNames.Add(ott.Name);
                            }
                        }
                    }
                    foreach(string objectName in objectNames)
                    {
                        Import(mvm.SubscriberPath, objectName, mvm);
                    }
                }
            }
        }

        public override void OS9Files(out string pFiles, string pName, int typ)
        {
            Debug.Print($"OS9Files {pName}, {typ}");
            pFiles = "drv_opcua\r\nopcua\r\ndriver\r\n";
        }

        public override void StartupLines(out string pLinesW, out string pLines, string pName, int typ)
        {
            Debug.Print($"Tady StartupLines {pName}, {typ}");
            pLines = string.Empty;
            pLinesW = string.Empty;
            pLinesW = "";
            pLines = "agent.load drv_opcua\r\nagent.runprg drv_opcua opcua " + Path.GetFileName(pName) +
                     "\r\ndriver.link opcua\r\n";
        }

        public override void UnLoadConfig()
        {
            Debug.Print($"UnLoadConfig");
        }

        /*        private void SaveConfiguration(string fileName, ViewModel vm)
                {
                    OpcConfiguration opc = new OpcConfiguration();
                    opc.GroupIpAddress = vm.GroupAddressString;
                    List<OpcConfigurationObject> objects = new List<OpcConfigurationObject>();
                    List<OpcConfigurationObjectItem> items = new List<OpcConfigurationObjectItem>();
                    foreach (OpcObject oo in vm.Objects)
                    {
                        OpcConfigurationObject ot = new OpcConfigurationObject();
                        ot.Name = oo.Name;
                        ot.PublishingInterval = (ushort)oo.PublishingInterval;
                        ot.PublishingIntervalSpecified = true;
                        ot.Pub = oo.Pub;
                        ot.PubSpecified = true;
                        ot.Sub = oo.Sub;
                        ot.SubSpecified = true;
                        items.Clear();
                        foreach (OpcObjectItem ooi in oo.Items)
                        {
                            OpcConfigurationObjectItem oit = new OpcConfigurationObjectItem();
                            oit.Name = ooi.Name;
                            oit.ArraySize = (ushort)ooi.ArraySizeValue;
                            oit.ArraySizeSpecified = true;
                            oit.Access = ooi.SelectedAccess;
                            oit.BasicType = ooi.SelectedBasicType;
                            oit.Rank = ooi.SelectedRank;
                            items.Add(oit);
                        }
                        ot.Items = items.ToArray();
                        objects.Add(ot);
                    }
                    opc.Objects = objects.ToArray();
                    XmlSerializer serializer = new XmlSerializer(typeof(OpcConfiguration));
                    using (TextWriter tw = new StreamWriter(fileName))
                    {
                        serializer.Serialize(tw, opc);
                    }
                }*/
        private ObjectTypeType CreateObjectTypeType(WpfControlLibrary.OpcObject opcObject, ushort id)
        {
            List<VariablesType> variables = new List<VariablesType>();
            ObjectTypeType ott = new ObjectTypeType();
            ott.Id = id;
            ott.Name = opcObject.Name;
            ott.Description = $"{opcObject.Publish};{opcObject.IsImported}";
            ott.VariablesCount = (ushort)opcObject.Items.Count;
            foreach (WpfControlLibrary.OpcObjectItem ooi in opcObject.Items)
            {
                if (_basicTypes.TryGetValue(ooi.SelectedBasicType, out byte typeCode))
                {
                    VariablesType vt = new VariablesType();
                    vt.Name = ooi.Name;
                    vt.BasicType = typeCode;
                    if (_access.TryGetValue(ooi.SelectedAccess, out byte accessCode))
                    {
                        vt.AccessType = accessCode;
                        vt.ArraySize = (ushort)ooi.ArraySizeValue;
                        vt.Type = (ooi.SelectedRank == "SimpleVariable") ? (byte)0 : (byte)1;
                        variables.Add(vt);
                    }
                }
            }
            ott.Variables = variables.ToArray();
            return ott;
        }

        private ObjectTypeType GetObjectFromPublisher(string path, string name, ushort id, out ushort publisherId)
        {
            publisherId = 0;
            if (File.Exists(path))
            {
                XmlSerializer serializerOpc = new XmlSerializer(typeof(OPCUAParametersType));
                using (TextReader tr = new StreamReader(path))
                {
                    OPCUAParametersType parsOpc = (OPCUAParametersType)serializerOpc.Deserialize(tr);
                    foreach (ObjectTypeType objectTypeType in parsOpc.ObjectType)
                    {
                        if (objectTypeType.Name == name)
                        {
                            bool subscribe = true;
                            string[] descrItems = objectTypeType.Description.Split(';');
                            objectTypeType.Description = $"{subscribe};{descrItems[1]};{descrItems[2]};{descrItems[3]}";
                            objectTypeType.Id = id;
                            publisherId = parsOpc.PublisherId;
                            return objectTypeType;
                        }
                    }
                }
            }
            return null;
        }
        private void SaveConfiguration(string fileName, WpfControlLibrary.MainViewModel mvm)
        {
            OPCUAParametersType pars = new OPCUAParametersType();
            pars.ObjectTypeCount = (ushort)mvm.Objects.Count;
            pars.UsePublisher = (mvm.PublisherObjects.Count != 0) ? true : false;
            pars.UseSubscriber = (mvm.SubscriberObjects.Count != 0) ? true : false;
            //            pars.UseServer = true;
            pars.PublisherId = (ushort)mvm.PublisherId;

            List<SubscriberType> sts = new List<SubscriberType>();
            foreach (WpfControlLibrary.PublisherItem pi in mvm.PublisherObjects)
            {
                SubscriberType st = new SubscriberType();
                st.PublisherRootType = pi.OpcObject.Name;
                st.SendPeriod = (ushort)pi.Interval;
                st.Description = $"{mvm.LocalIpAddressString};{mvm.GroupAddressString};{pi.WriterGroupId};{pi.DataSetWriterId};{pi.OpcObject.Name}";
                sts.Add(st);
                pars.Subscriber = sts.ToArray();
            }
            pars.SubscribersCount = (ushort)mvm.PublisherObjects.Count;

            List<ObjectTypeType> objects = new List<ObjectTypeType>();
            ushort id = 1;
            List<PublisherType> pts = new List<PublisherType>();
            foreach (WpfControlLibrary.SubscriberItem subscriberItem in mvm.SubscriberObjects)
            {
                PublisherType pt = new PublisherType();
                pt.Description = $"{mvm.LocalIpAddressString};{mvm.GroupAddressString};{subscriberItem.ConfigurationPath};{subscriberItem.ObjectName}";
                pts.Add(pt);
                pars.Publisher = pts.ToArray();
            }
            pars.PublishersCount = (ushort)mvm.SubscriberObjects.Count;

            foreach (WpfControlLibrary.OpcObject oo in mvm.Objects)
            {
                ObjectTypeType objectTypeType = CreateObjectTypeType(oo, id++);
                objects.Add(objectTypeType);
            }
            pars.ObjectType = objects.ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(OPCUAParametersType));
            using (TextWriter tw = new StreamWriter(fileName))
            {
                serializer.Serialize(tw, pars);
            }
        }
    }
}

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
using WpfControlLibrary;
using WpfControlLibrary.DataModel;

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
        private bool _serverEncryption;
        private readonly List<WpfControlLibrary.SubscriberItem> _subscriberItems;
        private readonly List<WpfControlLibrary.PublisherItem> _publisherItems;
        private readonly List<WpfControlLibrary.ClientItem> _clientItems;
        private readonly List<WpfControlLibrary.ServerItem> _serverItems;

        private readonly List<WpfControlLibrary.PortsNode> _ports;
        private readonly Dictionary<string, Flag> _allPorts;
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
            _clientItems = new List<WpfControlLibrary.ClientItem>();
            _serverItems = new List<WpfControlLibrary.ServerItem>();
            _ports = new List<WpfControlLibrary.PortsNode>();
            _allPorts = new Dictionary<string, Flag>();
            _publisherId = 1;
            _serverEncryption = false;
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
            if (!_allPorts.TryGetValue(pLabel, out Flag flag))
            {
                throw new ApplicationException($"Unknown label {pLabel}");
            }
            string[] items = flag.Text.Split('.');
            //            for (int i = 0; i < items.Length; ++i)
            //            {
            //                Debug.Print($"item {i}, {items[i]}");
            //            }
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

            int flagIndex = 5;
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
                else
                {
                    if (items[3] == "Server")
                    {
                        compiledType = 0;
                        flagIndex = 4;
                    }
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
            bw.Write(IntelMotorola(flag.ArrayIndex));
            bw.Write(Encoding.ASCII.GetBytes(items[flagIndex]));
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
        private void AddItemsToObject(ObjectTypeType ott, WpfControlLibrary.OpcObject oo, bool publish)
        {
            try
            {
                if (ott.Variables == null)
                {
                    return;
                }
                foreach (VariablesType vt in ott.Variables)
                {
                    string[] iitems = vt.Description.Split(';');
                    ushort ns = 0;
                    string id = string.Empty;
                    if (iitems.Length == 2)
                    {
                        ushort.TryParse(iitems[0], out ns);
                        id = iitems[1];
                    }
                    else
                    {
                        if (iitems.Length == 1)
                        {
                            id = iitems[0];
                        }
                    }
                    WpfControlLibrary.OpcObjectItem ooi = new WpfControlLibrary.OpcObjectItem(vt.Name, GetAccess(vt.AccessType), string.Empty,vt.ArraySize, GetBasicType(vt.BasicType),
                        false,$"{ns}:{id}", oo);
                    ooi.SelectedBasicType = GetBasicType(vt.BasicType);
                    ooi.SelectedAccess = GetAccess(vt.AccessType);
                    ooi.WriteOutside = (ooi.SelectedAccess != ooi.Access[0]);
                    ooi.ArraySizeValue = vt.ArraySize;
                    ooi.SelectedRank = (vt.Type == 0) ? ooi.Rank[0] : ooi.Rank[1];
                    oo.AddItem(ooi);
                }
            }
            catch (Exception e)
            {
                Debug.Print($"Exc= {e.Message}");
            }
            Debug.Print("AddItemsToObject End");
        }

        private WpfControlLibrary.OpcObject ImportObject(OPCUAParametersType pars, string name, string cfgName, bool publish)
        {
            /*            foreach (ObjectTypeType ott in pars.ObjectType)
                        {
                            Debug.Print($"ImportObject {ott.Name}");
                            string[] items = ott.Description.Split(';');
                            Debug.Print($"items= {items.Length}");
                            if (items.Length == 2)
                            {
                                bool pub = false;
                                bool.TryParse(items[0], out pub);
                                Debug.Print($"pub= {pub}, {items[0]}, {publish}");
                                if (ott.Name == name && pub == publish)
                                {
                                    Debug.Print("300");
                                    WpfControlLibrary.OpcObject oo = new WpfControlLibrary.OpcObject($"{cfgName}_{name}", false, true);
                                    Debug.Print("301");
                                    if (oo != null)
                                    {
                                        Debug.Print("302");
                                        AddItemsToObject(ott, oo, publish);
                                        Debug.Print("303");
                                        return oo;
                                    }
                                }
                            }
                        }*/
            return null;
        }

        private void Import(string path, string objectName, bool receive, bool monitoring, int period, bool encrypt, WpfControlLibrary.MainViewModel mvm = null)
        {
            Debug.Print("Import");
            if (File.Exists(path))
            {
                XmlSerializer serializerOpc = new XmlSerializer(typeof(OPCUAParametersType));
                using (TextReader tr = new StreamReader(path))
                {
                    OPCUAParametersType parsOpc = (OPCUAParametersType)serializerOpc.Deserialize(tr);
                    if (parsOpc.Subscriber != null)
                    {
                        foreach (SubscriberType subscriberType in parsOpc.Subscriber)
                        {
                            if (subscriberType.PublisherRootType == objectName)
                            {
                                WpfControlLibrary.OpcObject oo = ImportObject(parsOpc, objectName, Path.GetFileNameWithoutExtension(path), true);
                                if (oo != null)
                                {
                                    string[] itemsSub = subscriberType.Description.Split(';');
                                    if (itemsSub.Length == 3)
                                    {
                                        int writerId = 0;
                                        int.TryParse(itemsSub[0], out writerId);
                                        int datasetId = 0;
                                        int.TryParse(itemsSub[1], out datasetId);
                                        WpfControlLibrary.SubscriberItem si = new WpfControlLibrary.SubscriberItem(path, objectName, parsOpc.PublisherId, writerId, datasetId, oo, receive);
                                        _subscriberItems.Add(si);
                                        if (mvm != null)
                                        {
                                            mvm.SubscriberObjects.Add(si);
                                            mvm.SelectedSubscriberItem = si;
                                        }
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    string[] ips = parsOpc.ServerRootType.Split(';');
                    Debug.Print($"ips= {ips.Length}");
                    if (ips.Length == 2)
                    {
                        Debug.Print("200");
                        WpfControlLibrary.OpcObject oo = ImportObject(parsOpc, objectName, Path.GetFileNameWithoutExtension(path), false);
                        Debug.Print("201");
                        if (oo != null)
                        {
                            WpfControlLibrary.ClientItem ci = new WpfControlLibrary.ClientItem(path, objectName, ips[0], oo, receive, period, encrypt, monitoring);
                            _clientItems.Add(ci);
                            if (mvm != null)
                            {
                                mvm.ClientObjects.Add(ci);
                            }
                            return;
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
            _clientItems.Clear();
            _serverItems.Clear();
            _ports.Clear();
            _allPorts.Clear();
            NodeIdBase.Clear();

            if (File.Exists(pName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(OPCUAParametersType));
                using (TextReader tr = new StreamReader(pName))
                {
                    OPCUAParametersType pars = (OPCUAParametersType)serializer.Deserialize(tr);
                    string[] ips = pars.ServerRootType.Split(';');
                    if (ips.Length == 2)
                    {
                        _localIpAddress = ips[0];
                        _groupAddress = ips[1];
                    }
                    _serverEncryption = pars.ServerEncryption;
                    foreach (ObjectTypeType ott in pars.ObjectType)
                    {
                        string[] objectItems = ott.Description.Split(';');
                        bool serverObject = false;
                        bool clientObject = false;
                        bool publisherObject = false;
                        bool subscriberObject = false;
                        bool isImported = false;
                        if (objectItems.Length == 5)
                        {
                            bool.TryParse(objectItems[0], out serverObject);
                            bool.TryParse(objectItems[1], out clientObject);
                            bool.TryParse(objectItems[2], out publisherObject);
                            bool.TryParse(objectItems[3], out subscriberObject);
                            bool.TryParse(objectItems[4], out isImported);
                        }

                        string[] nodeIdItems = ott.BaseType.Split(';');
                        ushort namespaceIndex = 0;
                        string nodeId = string.Empty;
                        Debug.Print($"nodeIdItems= {nodeIdItems.Length}");
                        if (nodeIdItems.Length == 2)
                        {
                            if (ushort.TryParse(nodeIdItems[0], out ushort ns))
                            {
                                namespaceIndex = ns;
                            }

                            nodeId = $"{namespaceIndex}:{nodeIdItems[1]}";
                        }
                        else
                        {
                            nodeId = $"{namespaceIndex}:{nodeIdItems[0]}";
                        }
                        if (!isImported)
                        {
                            Debug.Print($"nodeId= {nodeId}");
                            WpfControlLibrary.OpcObject oo = new WpfControlLibrary.OpcObject(ott.Name, serverObject, clientObject, publisherObject, subscriberObject, isImported, nodeId);
                            AddItemsToObject(ott, oo, publisherObject);
                            _objects.Add(oo);
                            if (serverObject)
                            {
                                _serverItems.Add(new ServerItem(oo));
                            }
                        }
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
                            if (items.Length == 3)
                            {
                                int.TryParse(items[0], out writerId);
                                int.TryParse(items[1], out dataSetWriter);
                            }
                            WpfControlLibrary.OpcObject opcObject = FindOpcObject(items[2]);
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
                            bool receive = false;
                            if (items.Length >= 4)
                            {
                                string path = items[0];
                                string objectName = items[1];
                                bool.TryParse(items[2], out receive);
                                Import(path, objectName, receive, false, 0, false);
                            }
                        }
                    }
                    if (pars.UseClient)
                    {
                        foreach (ServerType st in pars.Server)
                        {
                            string[] items = st.Description.Split(';');
                            if (items.Length == 6)
                            {
                                bool receive = false;
                                bool monitoring = false;
                                string path = items[0];
                                string objectName = items[1];
                                bool.TryParse(items[3], out receive);
                                bool.TryParse(items[5], out monitoring);
                                if (string.IsNullOrEmpty(path))
                                {
                                    foreach (WpfControlLibrary.OpcObject oo in _objects)
                                    {
                                        if (oo.Name == items[4])
                                        {
                                            WpfControlLibrary.ClientItem ci = new ClientItem(path, objectName, items[2], oo, receive, st.QueryPeriod, monitoring, st.ClientEncryption);
                                            _clientItems.Add(ci);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    Import(path, objectName, receive, monitoring, st.QueryPeriod, st.ClientEncryption);
                                }
                            }
                        }
                    }
                    CreatePorts();
                }
            }
        }

        private string[] ItemToText(string objectName, WpfControlLibrary.OpcObjectItem ooi)
        {
            List<string> flags = new List<string>();
            string flag = $"{ooi.Name}({objectName})";
            Debug.Print($"ItemToText {objectName}, {ooi.Name}, {ooi.SelectedRank}");
            if (ooi.SelectedRank == ooi.Rank[0])
            {
                flags.Add(flag);
            }
            else
            {
                for (int i = 0; i < ooi.ArraySizeValue; ++i)
                {
                    flags.Add($"{flag}.{i}");
                }
            }
            return flags.ToArray();
        }

        private ushort GetArrayIndex(string text)
        {
            int i = text.LastIndexOf('.');
            if (i < 0)
            {
                return 0;
            }

            string index = text.Substring(i + 1);
            if (ushort.TryParse(index, out ushort result))
            {
                return result;
            }

            return 0;
        }
        private void CreatePorts()
        {
            _ports.Clear();
            WpfControlLibrary.PortsNode outputs = new WpfControlLibrary.PortsNode("Výstupy");
            WpfControlLibrary.PortsNode inputs = new WpfControlLibrary.PortsNode("Vstupy");
            _ports.Add(outputs);
            _ports.Add(inputs);

            StringBuilder sb = new StringBuilder();
            foreach (WpfControlLibrary.OpcObject oo in _objects)
            {
                WpfControlLibrary.PortsNode objectNodeIn = inputs.Add(oo.Name);
                WpfControlLibrary.PortsNode objectNodeOut = outputs.Add(oo.Name);
                if (!oo.PublisherObject && !oo.IsImported)
                {
                    bool founded = false;
                    foreach (WpfControlLibrary.ClientItem ci in _clientItems)
                    {
                        if (ci.OpcObject.Name == oo.Name)
                        {
                            founded = true;
                            break;
                        }
                    }

                    if (!founded)
                    {
                        foreach (WpfControlLibrary.OpcObjectItem ooi in oo.Items)
                        {
                            if (_ptxBasicTypes.TryGetValue(ooi.SelectedBasicType, out char typeChar))
                            {
                                string[] flags = ItemToText(oo.Name, ooi);
                                foreach (string flag in flags)
                                {
                                    sb.Clear();
                                    if (ooi.WriteOutside)
                                    {
                                        sb.Append($"I.OPCUA.{typeChar}.Server.{flag}");
                                        objectNodeIn.Add(sb.ToString());
                                    }
                                    else
                                    {
                                        sb.Append($"O.OPCUA.{typeChar}.Server.{flag}");
                                        objectNodeOut.Add(sb.ToString());
                                    }
                                    _allPorts[sb.ToString().ToUpperInvariant()] = new Flag(sb.ToString(), GetArrayIndex(flag));
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (oo.PublisherObject && !oo.IsImported)
                    {
                        foreach (WpfControlLibrary.OpcObjectItem ooi in oo.Items)
                        {
                            if (_ptxBasicTypes.TryGetValue(ooi.SelectedBasicType, out char typeChar))
                            {
                                string[] flags = ItemToText(oo.Name, ooi);
                                foreach (string flag in flags)
                                {
                                    sb.Clear();
                                    sb.Append($"O.OPCUA.{typeChar}.Pub.Sub1.{flag}");
                                    objectNodeOut.Add(sb.ToString());
                                    _allPorts[sb.ToString().ToUpperInvariant()] = new Flag(sb.ToString(), GetArrayIndex(flag));
                                }
                            }
                        }
                    }
                }
            }
            foreach (WpfControlLibrary.SubscriberItem si in _subscriberItems)
            {
                if (si.Receive)
                {
                    WpfControlLibrary.PortsNode obj = inputs.Add(si.OpcObject.Name);
                    foreach (WpfControlLibrary.OpcObjectItem ooi in si.OpcObject.Items)
                    {
                        if (_ptxBasicTypes.TryGetValue(ooi.SelectedBasicType, out char typeChar))
                        {
                            string[] flags = ItemToText(si.OpcObject.Name, ooi);
                            foreach (string flag in flags)
                            {
                                sb.Clear();
                                sb.Append($"I.OPCUA.{typeChar}.Sub.Pub1.{flag}");
                                obj.Add(sb.ToString());
                                _allPorts[sb.ToString().ToUpperInvariant()] = new Flag(sb.ToString(), GetArrayIndex(flag));
                            }
                        }
                    }
                }
            }
            int serverIndex = 1;
            foreach (WpfControlLibrary.ClientItem ci in _clientItems)
            {
                WpfControlLibrary.PortsNode objOuts = outputs.Add(ci.OpcObject.Name);
                WpfControlLibrary.PortsNode objIns = inputs.Add(ci.OpcObject.Name);
                foreach (WpfControlLibrary.OpcObjectItem ooi in ci.OpcObject.Items)
                {
                    if (_ptxBasicTypes.TryGetValue(ooi.SelectedBasicType, out char typeChar))
                    {
                        string[] flags = ItemToText(ci.OpcObject.Name, ooi);
                        foreach (string flag in flags)
                        {
                            sb.Clear();
                            if (ooi.WriteOutside)
                            {
                                sb.Append($"O.OPCUA.{typeChar}.Client.Server{serverIndex}.{flag}");
                                objOuts.Add(sb.ToString());
                            }
                            else
                            {
                                sb.Append($"I.OPCUA.{typeChar}.Client.Server{serverIndex}.{flag}");
                                objIns.Add(sb.ToString());
                            }
                            _allPorts[sb.ToString().ToUpperInvariant()] = new Flag(sb.ToString(), GetArrayIndex(flag));
                        }
                    }
                }
                ++serverIndex;
            }
        }
        public override void MakeConfig(System.Windows.Forms.IWin32Window hWnd, string pName)
        {
            Debug.Print($"MakeConfig {pName}");
            LoadConfig(pName);
            Debug.Print("10");
            WpfControlLibrary.MainWindow mainWindow = new WpfControlLibrary.MainWindow();
            Debug.Print($"11 {mainWindow.DataContext}");
            if (mainWindow.DataContext is WpfControlLibrary.MainViewModel vm)
            {
                Debug.Print("12");
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
                foreach (WpfControlLibrary.PublisherItem pi in _publisherItems)
                {
                    vm.PublisherObjects.Add(pi);
                }
                foreach (WpfControlLibrary.ClientItem ci in _clientItems)
                {
                    vm.ClientObjects.Add(ci);
                }

                foreach (ServerItem si in _serverItems)
                {
                    vm.ServerObjects.Add(si);
                }
                vm.WindowTitle = $"OpcUa - {pName}";
                vm.EncryptServer = _serverEncryption;
            }

            Debug.Print("1");
            if ((bool)mainWindow.ShowDialog())
            {
                Debug.Print("2");
                if (mainWindow.DataContext is WpfControlLibrary.MainViewModel mvm)
                {
                    Debug.Print("3");
                    SaveConfiguration(pName, mvm);
                    Debug.Print("4");
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
                    foreach (string objectName in objectNames)
                    {
                        Import(mvm.SubscriberPath, objectName, false, false, 100, false, mvm);
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
            ott.Description = $"{opcObject.ServerObject};{opcObject.ClientObject};{opcObject.PublisherObject};{opcObject.SubscriberObject};{opcObject.IsImported}";
            ott.VariablesCount = (ushort)opcObject.Items.Count;
            ott.BaseType = $"{opcObject.NodeId.NamespaceIndex};{opcObject.NodeId.GetIdentifier()}";
            foreach (WpfControlLibrary.OpcObjectItem ooi in opcObject.Items)
            {
                if (_basicTypes.TryGetValue(ooi.SelectedBasicType, out byte typeCode))
                {
                    VariablesType vt = new VariablesType
                    {
                        Name = ooi.Name,
                        BasicType = typeCode,
                        Description = $"{ooi.NodeId.NamespaceIndex};{ooi.NodeId.GetIdentifier()}"
                    };
                    ooi.SelectedAccess = ooi.WriteOutside ? ooi.Access[1] : ooi.Access[0];
                    if (_access.TryGetValue(ooi.SelectedAccess, out byte accessCode))
                    {
                        vt.AccessType = accessCode;
                        vt.ArraySize = (ushort)ooi.ArraySizeValue;
                        vt.Type = (ooi.SelectedRank == ooi.Rank[0]) ? (byte)0 : (byte)1;
                        Debug.Print($"Object= {opcObject.Name}, Item= {vt.Name}, Type={vt.Type}");
                        Debug.Flush();
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
            Debug.Print($"SaveConfiguration {fileName}");
            OPCUAParametersType pars = new OPCUAParametersType();
            pars.ObjectTypeCount = (ushort)mvm.Objects.Count;
            pars.UsePublisher = (mvm.PublisherObjects.Count != 0) ? true : false;
            pars.UseSubscriber = (mvm.SubscriberObjects.Count != 0) ? true : false;
            pars.UseClient = (mvm.ClientObjects.Count != 0) ? true : false;
            pars.UseServer = (mvm.ServerObjects.Count != 0) ? true : false;
            pars.PublisherId = (ushort)mvm.PublisherId;
            pars.ServerRootType = $"{mvm.LocalIpAddressString};{mvm.GroupAddressString}";

            pars.ServerEncryption = mvm.EncryptServer;

            List<ObjectTypeType> objects = new List<ObjectTypeType>();
            ushort id = 1;
            List<SubscriberType> sts = new List<SubscriberType>();
            foreach (WpfControlLibrary.PublisherItem pi in mvm.PublisherObjects)
            {
                SubscriberType st = new SubscriberType
                {
                    PublisherRootType = pi.OpcObject.Name,
                    SendPeriod = (ushort)pi.SendingPeriod,
                    Description = $"{pi.WriterGroupId};{pi.DataSetWriterId};{pi.OpcObject.Name}"
                };
                sts.Add(st);
            }
            pars.Subscriber = sts.ToArray();
            pars.SubscribersCount = (ushort)mvm.PublisherObjects.Count;

            List<PublisherType> pts = new List<PublisherType>();
            foreach (WpfControlLibrary.SubscriberItem subscriberItem in mvm.SubscriberObjects)
            {
                PublisherType pt = new PublisherType();
                pt.Description = $"{subscriberItem.ConfigurationPath};{subscriberItem.ObjectName};{subscriberItem.Receive};{subscriberItem.OpcObject.Name};{subscriberItem.PublisherId};{subscriberItem.WriterGroupId};{subscriberItem.DataSetWriterId}";
                pts.Add(pt);
                ObjectTypeType objectTypeType = CreateObjectTypeType(subscriberItem.OpcObject, id++);
                objects.Add(objectTypeType);
            }
            pars.Publisher = pts.ToArray();
            pars.PublishersCount = (ushort)mvm.SubscriberObjects.Count;

            List<ServerType> listSt = new List<ServerType>();
            foreach (WpfControlLibrary.ClientItem clientItem in mvm.ClientObjects)
            {
                ServerType st = new ServerType();
                st.Description = $"{clientItem.ConfigurationPath};{clientItem.ObjectName};{clientItem.IpAddress};{clientItem.Validity};{clientItem.OpcObject.Name};{clientItem.Monitoring}";
                Debug.Print($"Server {clientItem.RxTxPeriod}");
                st.QueryPeriod = (ushort)clientItem.RxTxPeriod;
                listSt.Add(st);
                if (clientItem.OpcObject.IsImported)
                {
                    ObjectTypeType objectTypeType = CreateObjectTypeType(clientItem.OpcObject, id++);
                    objects.Add(objectTypeType);
                }

                st.ClientEncryption = clientItem.EncryptClient;
            }
            pars.Server = listSt.ToArray();
            pars.ServersCount = (ushort)listSt.Count();

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
                Debug.Print("Po serialize");
            }

            string fileCsv = Path.ChangeExtension(fileName, ".csv");
            using (TextWriter tw = new StreamWriter(fileCsv))
            {
                foreach (WpfControlLibrary.OpcObject oo in mvm.Objects)
                {
                    if (!oo.IsImported && oo.ServerObject)
                    {
                        foreach (WpfControlLibrary.OpcObjectItem ooi in oo.Items)
                        {
                            tw.WriteLine($"{ooi.Name};{ooi.SelectedBasicType};{ooi.NodeId.NamespaceIndex};{ooi.NodeId.GetIdentifier()}");
                        }
                    }
                }
            }
        }
    }
}

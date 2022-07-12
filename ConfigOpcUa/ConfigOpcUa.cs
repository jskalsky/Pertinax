using OpcUaPars;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OpcUaCfg;
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

        private string _localIpAddress;
        private string _groupAddress;
        private int _publisherId;
        private bool _serverEncryption;
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


        private void LoadNode(DataModelNode parent, OpcUaCfg.node n, ObservableCollection<DataModelNode> dataModel)
        {
            DataModelNode dmn = null;
            if (n.Item is OpcUaCfg.nodeFolder folder)
            {
                dmn = new DataModelFolder(folder.name, NodeIdBase.GetNodeIdBase(folder.id), parent);
                DataModelNamespace ns = dmn.GetNamespace();
                IdFactory.AddName(ns.Namespace, IdFactory.NameFolder, folder.name);
                NodeIdBase.AddSystemNodeId(ns.Namespace, dmn.NodeId);
            }
            else
            {
                if (n.Item is OpcUaCfg.nodeNamespace nodeNamespace)
                {
                    dmn = new DataModelNamespace(nodeNamespace.index);
                }
                else
                {
                    if (n.Item is OpcUaCfg.nodeSimple_var simpleVar)
                    {
                        dmn = new DataModelSimpleVariable(simpleVar.name, NodeIdBase.GetNodeIdBase(simpleVar.id), GetBasicType(simpleVar.basic_type),
                            GetAccess(simpleVar.access), parent);
                        DataModelNamespace ns = dmn.GetNamespace();
                        IdFactory.AddName(ns.Namespace, IdFactory.NameSimpleVar, simpleVar.name);
                        NodeIdBase.AddVarNodeId(ns.Namespace, dmn.NodeId);
                    }
                    else
                    {
                        if (n.Item is OpcUaCfg.nodeArray_var arrayVar)
                        {
                            dmn = new DataModelArrayVariable(arrayVar.name, NodeIdBase.GetNodeIdBase(arrayVar.id), GetBasicType(arrayVar.basic_type),
                                GetAccess(arrayVar.access), (int)arrayVar.length, parent);
                            DataModelNamespace ns = dmn.GetNamespace();
                            IdFactory.AddName(ns.Namespace, IdFactory.NameArrayVar, arrayVar.name);
                            NodeIdBase.AddVarNodeId(ns.Namespace, dmn.NodeId);
                        }
                        else
                        {
                            if (n.Item is OpcUaCfg.nodeObject_type objectType)
                            {
                                dmn = new DataModelObjectType(objectType.name, NodeIdBase.GetNodeIdBase(objectType.id), parent);
                                DataModelNamespace ns = dmn.GetNamespace();
                                IdFactory.AddName(ns.Namespace, IdFactory.NameObjectType, objectType.name);
                                //                                IdFactory.AddNumericId(ns.Namespace, objectType.id);
                            }
                            else
                            {
                                if (n.Item is OpcUaCfg.nodeObject_var objectVar)
                                {
                                    dmn = new DataModelObjectVariable(objectVar.name, NodeIdBase.GetNodeIdBase(objectVar.id), objectVar.object_type_name,
                                        parent);
                                    DataModelNamespace ns = dmn.GetNamespace();
                                    IdFactory.AddName(ns.Namespace, IdFactory.NameArrayVar, objectVar.name);
                                    //                                    IdFactory.AddNumericId(ns.Namespace, objectVar.id);
                                }
                            }
                        }
                    }
                }
            }
            if (parent == null)
            {
                dataModel.Add(dmn);
            }
            else
            {
                parent.AddChildren(dmn);
            }
            if (n.sub_nodes != null)
            {
                foreach (OpcUaCfg.node child in n.sub_nodes)
                {
                    LoadNode(dmn, child, dataModel);
                }
            }
        }

        private void LoadXml(string pName, WpfControlLibrary.ViewModel.OpcUaViewModel mvm)
        {
            if (File.Exists(pName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(OpcUaCfg.OPCUAParametersType));
                using (TextReader tr = new StreamReader(pName))
                {
                    OpcUaCfg.OPCUAParametersType treeNodes = (OpcUaCfg.OPCUAParametersType)serializer.Deserialize(tr);
                    foreach (OpcUaCfg.node node in treeNodes.nodes)
                    {
                        LoadNode(null, node, mvm.DataModel);
                    }
                    /*                    foreach (OpcUaCfg.connectionsConnection connection in treeNodes.connections)
                                        {
                                            WpfControlLibrary.Client.ClientConnection cc = new WpfControlLibrary.Client.ClientConnection();
                                            cc.Crypto = connection.encryption;
                                            cc.IpAddress = connection.ip_address;
                                            cc.Service = GetClientService(connection.service);
                                            if (connection.var != null)
                                            {
                                                foreach (OpcUaCfg.connectionsConnectionVar vt in connection.var)
                                                {
                                                    cc.AddVar(vt.ns, vt.id, GetBasicType(vt.basic_type), vt.alias);
                                                }
                                            }
                                            mvm.Connections.Add(cc);
                                        }*/
                }
            }
            else
            {
                mvm.DataModelNamespace0 = new DataModelNamespace(0);
                mvm.DataModelNamespace1 = new DataModelNamespace(1);
                mvm.DataModelNamespace2 = new DataModelNamespace(2);
                mvm.DataModel.Add(mvm.DataModelNamespace0);
                mvm.DataModel.Add(mvm.DataModelNamespace1);
                mvm.DataModel.Add(mvm.DataModelNamespace2);
            }
        }

        private void CreateFlags(OpcUaCfg.node n, StringBuilder path, PortsNode pn)
        {
            string nodeText = String.Empty;
            if (n.Item is OpcUaCfg.nodeNamespace ns)
            {
                path.Append($"{ns.index}");
                nodeText = $"Ns{ns.index}";
            }
            else
            {
                if (n.Item is OpcUaCfg.nodeFolder nf)
                {
                    path.Append($".{nf.name}");
                    nodeText = $"{nf.name}";
                }
                else
                {
                    if (n.Item is OpcUaCfg.nodeSimple_var nsv)
                    {
                        if (_ptxBasicTypes.TryGetValue(GetBasicType(nsv.basic_type), out char basicTypeChar))
                        {
                            if (nsv.access == access.Read || nsv.access == access.ReadWrite)
                            {
                                pn.Add($"O.OPCUA.{basicTypeChar}.{path}.{nsv.name}");
                            }
                            else
                            {
                                pn.Add($"I.OPCUA.{basicTypeChar}.{path}.{nsv.name}");
                            }
                        }
                        return;
                    }
                    else
                    {
                        if (n.Item is OpcUaCfg.nodeArray_var nav)
                        {
                            if (_ptxBasicTypes.TryGetValue(GetBasicType(nav.basic_type), out char basicTypeChar))
                            {
                                for (uint i = 0; i < nav.length; i++)
                                {
                                    if (nav.access == access.Read || nav.access == access.ReadWrite)
                                    {
                                        pn.Add($"O.OPCUA.{basicTypeChar}.{path}.{nav.name}.{i}");
                                    }
                                    else
                                    {
                                        pn.Add($"I.OPCUA.{basicTypeChar}.{path}.{nav.name}.{i}");
                                    }
                                }
                            }
                            return;
                        }
                    }
                }
            }
            PortsNode childNode = pn.Add(nodeText);
            if (n.sub_nodes != null)
            {
                foreach (OpcUaCfg.node child in n.sub_nodes)
                {
                    CreateFlags(child, path, childNode);
                }
            }
        }
        public override void LoadConfig(string pName)
        {
            Debug.Print($"LoadConfig= {pName}");
            if (File.Exists(pName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(OpcUaCfg.OPCUAParametersType));
                using (TextReader tr = new StreamReader(pName))
                {
                    OpcUaCfg.OPCUAParametersType cfg = (OpcUaCfg.OPCUAParametersType)serializer.Deserialize(tr);
                    _ports.Clear();
                    StringBuilder sb = new StringBuilder();
                    foreach (OpcUaCfg.node treeNode in cfg.nodes)
                    {
                        PortsNode portsNode = null;
                        if (treeNode.Item is OpcUaCfg.nodeNamespace ns)
                        {
                            sb.Clear();
                            sb.Append($"{ns.index}");
                            portsNode = new PortsNode($"Ns{ns.index}");
                            if (treeNode.sub_nodes != null)
                            {
                                foreach (OpcUaCfg.node child in treeNode.sub_nodes)
                                {
                                    CreateFlags(child, sb, portsNode);
                                }
                            }
                            _ports.Add(portsNode);
                        }
                    }
                }
            }
            /*            _objects.Clear();
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
                        }*/
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
            /*            foreach (WpfControlLibrary.OpcObject oo in _objects)
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
                        }*/
        }

        public override void MakeConfig(System.Windows.Forms.IWin32Window hWnd, string pName)
        {
            WpfControlLibrary.View.OpcUaMainWindow mainWindow = new WpfControlLibrary.View.OpcUaMainWindow();
            if (mainWindow.DataContext is WpfControlLibrary.ViewModel.OpcUaViewModel mvm)
            {
                LoadXml(pName, mvm);
                if ((bool)mainWindow.ShowDialog())
                {
                    Debug.Print("3");
                    SaveConfiguration(pName, mvm);
                    Debug.Print("4");
                }
            }
        }
        /*        public override void MakeConfig(System.Windows.Forms.IWin32Window hWnd, string pName)
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
                }*/

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug.Print($"Vm_PropertyChanged {e.PropertyName}, {sender}");
            /*            if (e.PropertyName == "SubscriberPath")
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
                        }*/
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
        /*        private ObjectTypeType GetObjectFromPublisher(string path, string name, ushort id, out ushort publisherId)
                {
                    publisherId = 0;
                    if (File.Exists(path))
                    {
                        XmlSerializer serializerOpc = new XmlSerializer(typeof(OpcUaCfg.OPCUAParametersType));
                        using (TextReader tr = new StreamReader(path))
                        {
                            OpcUaCfg.OPCUAParametersType parsOpc = (OpcUaCfg.OPCUAParametersType)serializerOpc.Deserialize(tr);
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
                }*/

        private static OpcUaCfg.access GetAccess(string s)
        {
            switch (s)
            {
                case "Read":
                    return OpcUaCfg.access.Read;
                case "Write":
                    return OpcUaCfg.access.Write;
                case "ReadWrite":
                    return OpcUaCfg.access.ReadWrite;
            }
            return OpcUaCfg.access.Unknown;
        }

        private static string GetAccess(OpcUaCfg.access access)
        {
            switch (access)
            {
                case OpcUaCfg.access.Read:
                    return "Read";
                case OpcUaCfg.access.Write:
                    return "Write";
                case OpcUaCfg.access.ReadWrite:
                    return "ReadWrite";
            }
            return "Unknown";
        }
        private static OpcUaCfg.basic_type GetBasicType(string s)
        {
            switch (s)
            {
                case "Boolean":
                    return OpcUaCfg.basic_type.Boolean;
                case "UInt8":
                    return OpcUaCfg.basic_type.UInt8;
                case "Int8":
                    return OpcUaCfg.basic_type.Int8;
                case "UInt16":
                    return OpcUaCfg.basic_type.UInt16;
                case "Int16":
                    return OpcUaCfg.basic_type.Int16;
                case "UInt32":
                    return OpcUaCfg.basic_type.UInt32;
                case "Int32":
                    return OpcUaCfg.basic_type.Int32;
                case "Float":
                    return OpcUaCfg.basic_type.Float;
                case "Double":
                    return OpcUaCfg.basic_type.Double;
            }
            return OpcUaCfg.basic_type.Unknown;
        }
        private static string GetBasicType(OpcUaCfg.basic_type s)
        {
            switch (s)
            {
                case OpcUaCfg.basic_type.Boolean:
                    return "Boolean";
                case OpcUaCfg.basic_type.UInt8:
                    return "UInt8";
                case OpcUaCfg.basic_type.Int8:
                    return "Int8";
                case OpcUaCfg.basic_type.UInt16:
                    return "UInt16";
                case OpcUaCfg.basic_type.Int16:
                    return "Int16";
                case OpcUaCfg.basic_type.UInt32:
                    return "UInt32";
                case OpcUaCfg.basic_type.Int32:
                    return "Int32";
                case OpcUaCfg.basic_type.Float:
                    return "Float";
                case OpcUaCfg.basic_type.Double:
                    return "Double";
            }
            return "Unknown";
        }

        private static string GetClientService(OpcUaCfg.client_service service)
        {
            switch (service)
            {
                case OpcUaCfg.client_service.Read:
                    return "Read";
                case OpcUaCfg.client_service.Write:
                    return "Write";
            }
            return "Unknown";

        }
        private static OpcUaCfg.client_service GetClientService(string service)
        {
            switch (service)
            {
                case "Read":
                    return OpcUaCfg.client_service.Read;
                case "Write":
                    return OpcUaCfg.client_service.Write;
            }
            return OpcUaCfg.client_service.Unknown;

        }

        private static OpcUaCfg.nodeFolder GetFolder(DataModelFolder dmFolder)
        {
            OpcUaCfg.nodeFolder folder = new OpcUaCfg.nodeFolder();
            folder.name = dmFolder.Name;
            folder.id = dmFolder.NodeId.GetNodeName();
            return folder;
        }

        private static OpcUaCfg.nodeSimple_var GetSimpleVar(DataModelSimpleVariable dmSimple)
        {
            OpcUaCfg.nodeSimple_var simple = new OpcUaCfg.nodeSimple_var();
            simple.name = dmSimple.Name;
            simple.access = GetAccess(dmSimple.VarAccess);
            simple.basic_type = GetBasicType(dmSimple.VarType);
            simple.id = dmSimple.NodeId.GetNodeName();
            return simple;
        }

        private static OpcUaCfg.nodeArray_var GetArrayVar(DataModelArrayVariable dmArray)
        {
            OpcUaCfg.nodeArray_var array = new OpcUaCfg.nodeArray_var();
            array.name = dmArray.Name;
            array.access = GetAccess(dmArray.VarAccess);
            array.basic_type = GetBasicType(dmArray.BasicType);
            array.id = dmArray.NodeId.GetNodeName();
            array.length = (uint)dmArray.ArrayLength;
            return array;
        }

        private static OpcUaCfg.nodeObject_type GetObjectType(DataModelObjectType dmObjectType)
        {
            OpcUaCfg.nodeObject_type ot = new OpcUaCfg.nodeObject_type();
            ot.name = dmObjectType.Name;
            //            ot.id = $"{dmObjectType.GetNamespace().Namespace}:{dmObjectType.NodeId.GetIdentifier()}";
            return ot;
        }

        private static OpcUaCfg.nodeObject_var GetObjectvar(DataModelObjectVariable dmObjectVar)
        {
            OpcUaCfg.nodeObject_var objectVar = new OpcUaCfg.nodeObject_var();
            objectVar.name = dmObjectVar.Name;
            objectVar.object_type_name = dmObjectVar.ObjectTypeName;
            //            objectVar.id = $"{dmObjectVar.GetNamespace().Namespace}:{dmObjectVar.NodeId.GetIdentifier()}";
            return objectVar;
        }

        private static void SaveTreeNode(DataModelNode node, OpcUaCfg.node tn)
        {
            Debug.Print($"SaveTreeNode {node}");
            if (node is DataModelNamespace dmNs)
            {
                OpcUaCfg.nodeNamespace objNamespace = new OpcUaCfg.nodeNamespace();
                objNamespace.index = dmNs.Namespace;
                tn.Item = objNamespace;
                tn.node_type = node_type.Namespace;
            }
            else
            {
                if (node is DataModelFolder dmFolder)
                {
                    tn.Item = GetFolder(dmFolder);
                    tn.node_type = node_type.Folder;
                }
                else
                {
                    if (node is DataModelSimpleVariable dmSimple)
                    {
                        tn.Item = GetSimpleVar(dmSimple);
                        tn.node_type = node_type.SimpleVariable;
                    }
                    else
                    {
                        if (node is DataModelArrayVariable dmArray)
                        {
                            tn.Item = GetArrayVar(dmArray);
                            tn.node_type = node_type.ArrayVariable;
                        }
                        else
                        {
                            if (node is DataModelObjectType dmObjectType)
                            {
                                tn.Item = GetObjectType(dmObjectType);
                                tn.node_type = node_type.ObjectType;
                            }
                            else
                            {
                                if (node is DataModelObjectVariable dmObjectVar)
                                {
                                    tn.Item = GetObjectvar(dmObjectVar);
                                    tn.node_type = node_type.ObjectVariable;
                                }
                            }
                        }
                    }
                }
            }
            if (node.Children.Count != 0)
            {
                tn.sub_nodes = new OpcUaCfg.node[node.Children.Count];
                int index = 0;
                foreach (DataModelNode child in node.Children)
                {
                    tn.sub_nodes[index] = new OpcUaCfg.node();
                    SaveTreeNode(child, tn.sub_nodes[index]);
                    ++index;
                }
            }
            else
            {
                tn.sub_nodes = new node[] { };
            }
        }

        private void SaveConnection(WpfControlLibrary.Client.ClientConnection connection, OpcUaCfg.connectionsConnection ct)
        {
            ct.ip_address = connection.IpAddress;
            ct.encryption = connection.Crypto;
            ct.period = connection.Period;
            ct.service = GetClientService(connection.Service);
        }

        private void SaveClientVar(WpfControlLibrary.Client.ClientVar var, OpcUaCfg.connectionsConnectionVar var_Type)
        {
            var_Type.ns = 0;
            var_Type.id = string.Empty;
            string[] items = var.Identifier.Split(':');
            if (items.Length == 2)
            {
                if (ushort.TryParse(items[0], out ushort nsIndex))
                {
                    var_Type.ns = nsIndex;
                    var_Type.id = items[1];
                }
            }
            var_Type.basic_type = GetBasicType(var.SelectedBasicType);
            var_Type.alias = var.Alias;
        }
        private void SaveConfiguration(string fileName, WpfControlLibrary.ViewModel.OpcUaViewModel mvm)
        {
            Debug.Print($"SaveConfiguration {fileName}");

            OpcUaCfg.OPCUAParametersType cfg = new OpcUaCfg.OPCUAParametersType();
            cfg.settings = new OpcUaCfg.settings();
            cfg.settings.local_ip = mvm.LocalIpAddress;
            cfg.settings.multicast_ip = mvm.MulticastIpAddress;

            cfg.server = new OPCUAParametersTypeServer();
            cfg.server.encryption = false;

            List<OpcUaCfg.node> nodes = new List<OpcUaCfg.node>();
            foreach (WpfControlLibrary.DataModel.DataModelNode modelNode in mvm.DataModel)
            {
                OpcUaCfg.node tn = new OpcUaCfg.node();
                SaveTreeNode(modelNode, tn);
                nodes.Add(tn);
            }
            cfg.nodes = nodes.ToArray();

            List<OpcUaCfg.connectionsConnection> connections = new List<OpcUaCfg.connectionsConnection>();
            foreach (WpfControlLibrary.Client.ClientConnection connection in mvm.Connections)
            {
                OpcUaCfg.connectionsConnection ct = new OpcUaCfg.connectionsConnection();
                SaveConnection(connection, ct);

                List<OpcUaCfg.connectionsConnectionVar> vars = new List<OpcUaCfg.connectionsConnectionVar>();
                foreach (WpfControlLibrary.Client.ClientVar var in connection.Vars)
                {
                    OpcUaCfg.connectionsConnectionVar var_Type = new OpcUaCfg.connectionsConnectionVar();
                    SaveClientVar(var, var_Type);
                    vars.Add(var_Type);
                }
                ct.var = vars.ToArray();
                connections.Add(ct);
            }
            //            cfg.connections = connections.ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(OpcUaCfg.OPCUAParametersType));
            using (TextWriter tw = new StreamWriter(fileName))
            {
                serializer.Serialize(tw, cfg);
                Debug.Print("Po serialize");
            }
        }
    }
}

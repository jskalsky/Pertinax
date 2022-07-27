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
using WpfControlLibrary.Client;
using WpfControlLibrary.DataModel;
using WpfControlLibrary.ViewModel;
using WpfControlLibrary.View;

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
        private readonly List<VmFlagNode> _ports;
        private readonly List<WpfControlLibrary.Client.ClientConnection> _connections;
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
            _ports = new List<VmFlagNode>();
            _connections = new List<ClientConnection>();
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

            bw.Write(tc);
            bw.Write(IntelMotorola(flag.ArrayIndex));

            int idx = 0;
            string s = flag.Text;
            for (int i = 0; i < 3; ++i)
            {
                idx = s.IndexOf('.');
                if (idx > 0)
                {
                    s = s.Remove(0, idx + 1);
                }
            }

            if (flag.IsArray)
            {
                idx = s.LastIndexOf('.');
                if (idx > 0)
                {
                    s = s.Remove(idx, s.Length - idx);
                }
            }
            bw.Write(IntelMotorola((ushort)s.Length));
            bw.Write(Encoding.ASCII.GetBytes(s));
            bw.Close();
            return ms.ToArray();
        }

        public override string CreatePort(System.Windows.Forms.IWin32Window hWnd)
        {
            Debug.Print($"CreatePort");
            PortDialog pd = new WpfControlLibrary.View.PortDialog();
            if (pd.DataContext is WpfControlLibrary.ViewModel.ViewModelFlags vmp)
            {
                vmp.Load();
            }
            bool? result = pd.ShowDialog();
            if (result != null && result == true)
            {
                if (pd.SelectedPort != null)
                {
                    Debug.Print($"pd.SelectedPort= {pd.SelectedPort}");
                    return pd.SelectedPort;
                }
            }

            return string.Empty;
        }


        private void LoadNode(DataModelNode parent, OpcUaCfg.node n, ObservableCollection<DataModelNode> dataModel, ref int nrErrors)
        {
            Debug.Print($"LoadNode {n.node_type}, parent= {parent}");
            DataModelNode dmn = null;
            if (n.Item is OpcUaCfg.nodeFolder folder)
            {
                Debug.Print($"LoadNode FOLDER= {folder.name}, {folder.id}");
                NodeIdBase nodeId = NodeIdBase.GetNodeIdBase(folder.id);
                dmn = new DataModelFolder(folder.name, nodeId, parent);
                /*                if (nodeId.NamespaceIndex == 1)
                                {
                                    Debug.Print($"nodeId.NamespaceIndex= {nodeId.NamespaceIndex}");
                                    switch (folder.name)
                                    {
                                        case "Z2Xx":
                                            DefaultDataModel.FolderZ2Xx = new DataModelFolder(folder.name, nodeId, parent);
                                            dmn = DefaultDataModel.FolderZ2Xx;
                                            break;
                                        case "Objects":
                                            DefaultDataModel.FolderObjects = new DataModelFolder(folder.name, nodeId, parent);
                                            dmn = DefaultDataModel.FolderObjects;
                                            break;
                                        case "ObjectTypes":
                                            DefaultDataModel.FolderObjectTypes = new DataModelFolder(folder.name, nodeId, parent);
                                            dmn = DefaultDataModel.FolderObjectTypes;
                                            break;
                                        case "Variables":
                                            DefaultDataModel.FolderVariables = new DataModelFolder(folder.name, nodeId, parent);
                                            dmn = DefaultDataModel.FolderVariables;
                                            break;
                                    }

                                }
                                else
                                {
                                    dmn = new DataModelFolder(folder.name, nodeId, parent);
                                }*/
                Debug.Print($"Folder dmn= {dmn}");
                if (dmn != null)
                {
                    DataModelNamespace ns = dmn.GetNamespace();
                    IdFactory.AddName(ns.Namespace, IdFactory.NameFolder, folder.name);
                    if (!NodeIdBase.AddSystemNodeId(ns.Namespace, dmn.NodeId))
                    {
                        ++nrErrors;
                        WpfControlLibrary.ViewModel.OpcUaViewModel.AddStatusMessage(WpfControlLibrary.ViewModel.StatusMsg._messageTypes[0],
                            $"NodeId {dmn.NodeId.GetNodeName()} již existuje", dmn);
                    }
                }
            }
            else
            {
                if (n.Item is OpcUaCfg.nodeNamespace nodeNamespace)
                {
                    Debug.Print($"LoadNode NS= {nodeNamespace.index}");
                    switch (nodeNamespace.index)
                    {
                        case 0:
                            DefaultDataModel.DataModelNamespace0 = new DataModelNamespace(nodeNamespace.index);
                            dmn = DefaultDataModel.DataModelNamespace0;
                            break;
                        case 1:
                            DefaultDataModel.DataModelNamespace1 = new DataModelNamespace(nodeNamespace.index);
                            dmn = DefaultDataModel.DataModelNamespace1;
                            break;
                        case 2:
                            DefaultDataModel.DataModelNamespace2 = new DataModelNamespace(nodeNamespace.index);
                            dmn = DefaultDataModel.DataModelNamespace2;
                            break;
                    }
                    Debug.Print($"1000, {dmn}");
                }
                else
                {
                    if (n.Item is OpcUaCfg.nodeSimple_var simpleVar)
                    {
                        Debug.Print($"LoadNode SIMPLE= {simpleVar.name}");
                        dmn = new DataModelSimpleVariable(simpleVar.name, NodeIdBase.GetNodeIdBase(simpleVar.id), GetBasicType(simpleVar.basic_type),
                            GetAccess(simpleVar.access), parent);
                        DataModelNamespace ns = dmn.GetNamespace();
                        IdFactory.AddName(ns.Namespace, IdFactory.NameSimpleVar, simpleVar.name);
                        if (!NodeIdBase.AddVarNodeId(ns.Namespace, dmn.NodeId))
                        {
                            ++nrErrors;
                            WpfControlLibrary.ViewModel.OpcUaViewModel.AddStatusMessage(WpfControlLibrary.ViewModel.StatusMsg._messageTypes[0],
                                $"NodeId {dmn.NodeId.GetNodeName()} již existuje", dmn);
                        }
                    }
                    else
                    {
                        if (n.Item is OpcUaCfg.nodeArray_var arrayVar)
                        {
                            Debug.Print($"LoadNode ARRAY= {arrayVar.name}");
                            dmn = new DataModelArrayVariable(arrayVar.name, NodeIdBase.GetNodeIdBase(arrayVar.id), GetBasicType(arrayVar.basic_type),
                                GetAccess(arrayVar.access), (int)arrayVar.length, parent);
                            DataModelNamespace ns = dmn.GetNamespace();
                            IdFactory.AddName(ns.Namespace, IdFactory.NameArrayVar, arrayVar.name);
                            if (!NodeIdBase.AddVarNodeId(ns.Namespace, dmn.NodeId))
                            {
                                ++nrErrors;
                                WpfControlLibrary.ViewModel.OpcUaViewModel.AddStatusMessage(WpfControlLibrary.ViewModel.StatusMsg._messageTypes[0],
                                    $"NodeId {dmn.NodeId.GetNodeName()} již existuje", dmn);
                            }
                        }
                        else
                        {
                            if (n.Item is OpcUaCfg.nodeObject_type objectType)
                            {
                                Debug.Print($"OT {objectType.id}");
                                dmn = new DataModelObjectType(objectType.name, NodeIdBase.GetNodeIdBase(objectType.id), parent);
                                DataModelNamespace ns = dmn.GetNamespace();
                                IdFactory.AddName(ns.Namespace, IdFactory.NameObjectType, objectType.name);
                                if (!NodeIdBase.AddSystemNodeId(ns.Namespace, dmn.NodeId))
                                {
                                    ++nrErrors;
                                    WpfControlLibrary.ViewModel.OpcUaViewModel.AddStatusMessage(WpfControlLibrary.ViewModel.StatusMsg._messageTypes[0],
                                        $"NodeId {dmn.NodeId.GetNodeName()} již existuje", dmn);
                                }
                            }
                            else
                            {
                                if (n.Item is OpcUaCfg.nodeObject_var objectVar)
                                {
                                    Debug.Print($"LoadNode 4");
                                    dmn = new DataModelObjectVariable(objectVar.name, NodeIdBase.GetNodeIdBase(objectVar.id), objectVar.object_type_name,
                                        parent);
                                    DataModelNamespace ns = dmn.GetNamespace();
                                    IdFactory.AddName(ns.Namespace, IdFactory.NameObjectVar, objectVar.name);
                                    if (!NodeIdBase.AddSystemNodeId(ns.Namespace, dmn.NodeId))
                                    {
                                        ++nrErrors;
                                        WpfControlLibrary.ViewModel.OpcUaViewModel.AddStatusMessage(WpfControlLibrary.ViewModel.StatusMsg._messageTypes[0],
                                            $"NodeId {dmn.NodeId.GetNodeName()} již existuje", dmn);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Debug.Print($"parent= {parent}, dmn= {dmn}");
            if (parent == null)
            {
                Debug.Print("5");
                dataModel.Add(dmn);
                Debug.Print("6");
            }
            else
            {
                Debug.Print("7");
                parent.AddChildren(dmn);
                Debug.Print("8");
            }
            Debug.Print("100");
            if (n.sub_nodes != null)
            {
                Debug.Print($"101, {n.sub_nodes}, {n.sub_nodes.Length}");
                foreach (OpcUaCfg.node child in n.sub_nodes)
                {
                    Debug.Print($"9 {child}, {dmn}");
                    Debug.Flush();
                    LoadNode(dmn, child, dataModel, ref nrErrors);
                    Debug.Print("10");
                }
            }
        }

        private void LoadXml(string pName, WpfControlLibrary.ViewModel.OpcUaViewModel mvm)
        {
            Debug.Print($"LoadXml {pName}, {mvm}");
            try
            {
                if (File.Exists(pName))
                {
                    Debug.Print($"Existed");
                    XmlSerializer serializer = new XmlSerializer(typeof(OpcUaCfg.OPCUAParametersType));
                    using (TextReader tr = new StreamReader(pName))
                    {
                        OpcUaCfg.OPCUAParametersType cfg = (OpcUaCfg.OPCUAParametersType)serializer.Deserialize(tr);
                        mvm.LocalIpAddress = cfg.settings.local_ip;
                        mvm.MulticastIpAddress = cfg.settings.multicast_ip;
                        int nrErrors = 0;
                        foreach (OpcUaCfg.node node in cfg.nodes)
                        {
                            LoadNode(null, node, mvm.DataModel, ref nrErrors);
                        }

                        WpfControlLibrary.ViewModel.OpcUaViewModel.AddStatusMessage(
                            WpfControlLibrary.ViewModel.StatusMsg._messageTypes[2],
                            nrErrors == 0
                                ? $"Načtení konfigurace bez chyb"
                                : $"Načtení konfigurace, počet chyb = {nrErrors}");
                        if (cfg.connections != null)
                        {
                            if (cfg.connections.Length != 0)
                            {
                                foreach (OpcUaCfg.connectionsConnection connection in cfg.connections)
                                {
                                    Debug.Flush();
                                    WpfControlLibrary.Client.ClientConnection cc =
                                        new WpfControlLibrary.Client.ClientConnection(connection.ip_address,
                                            connection.encryption);
                                    if (connection.group != null)
                                    {
                                        foreach (OpcUaCfg.connectionsConnectionGroup group in connection.group)
                                        {
                                            WpfControlLibrary.Client.Group gr = new Group(group.period, group.service == client_service.Read ? "Read" : "Write");
                                            cc.Groups.Add(gr);
                                            if (group.var != null)
                                            {
                                                foreach (OpcUaCfg.connectionsConnectionGroupVar groupVar in group.var)
                                                {
                                                    WpfControlLibrary.Client.ClientVar clientVar =
                                                        new ClientVar(gr, groupVar.id,
                                                            GetBasicType(groupVar.basic_type), groupVar.alias);
                                                    gr.Vars.Add(clientVar);
                                                }
                                            }
                                        }
                                    }
                                    mvm.Connections.Add(cc);
                                }
                            }
                        }
                    }
                }
                else
                {
                    WpfControlLibrary.DataModel.DefaultDataModel.Setup(mvm.DataModel);
                }
                DefaultDataModel.DataModelNamespace1.IsExpanded = true;
            }
            catch (Exception e)
            {
                Debug.Print($"LoadXml Exception: {e.Message}");
                StackTrace stackTrace = new StackTrace(e, true);
                for (int i = 0; i < stackTrace.FrameCount; ++i)
                {
                    Debug.WriteLine($"  {stackTrace.GetFrame(i).GetFileName()}, {stackTrace.GetFrame(i).GetFileLineNumber()} : {stackTrace.GetFrame(i).GetMethod().Name}");
                }
            }
        }

        public override void LoadConfig(string pName)
        {
            Debug.Print($"LoadConfig= {pName}");
            if (File.Exists(pName))
            {
                try
                {
                    WpfControlLibrary.Model.ModFlagsTree.LoadXml(pName);
                }
                catch (Exception e)
                {
                    Debug.Print($"Exception: {e.Message}");
                    StackTrace stackTrace = new StackTrace(e, true);
                    for (int i = 0; i < stackTrace.FrameCount; ++i)
                    {
                        Debug.WriteLine($"  {stackTrace.GetFrame(i).GetFileName()}, {stackTrace.GetFrame(i).GetFileLineNumber()} : {stackTrace.GetFrame(i).GetMethod().Name}");
                    }
                }
            }
            Debug.Print("LoadConfig Ok");
        }

        public override void MakeConfig(System.Windows.Forms.IWin32Window hWnd, string pName)
        {
            try
            {
                WpfControlLibrary.View.MainWindow mainWindow = new WpfControlLibrary.View.MainWindow();
                if (mainWindow.DataContext is WpfControlLibrary.ViewModel.MainWindowViewModel mvm)
                {
                    mvm.LoadXml(pName);
                    if ((bool)mainWindow.ShowDialog())
                    {
                        mvm.SaveXml(pName);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Print($"Exception: {e.Message}");
                StackTrace stackTrace = new StackTrace(e, true);
                for (int i = 0; i < stackTrace.FrameCount; ++i)
                {
                    Debug.WriteLine($"  {stackTrace.GetFrame(i).GetFileName()}, {stackTrace.GetFrame(i).GetFileLineNumber()} : {stackTrace.GetFrame(i).GetMethod().Name}");
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
            ot.id = dmObjectType.NodeId.GetNodeName();
            return ot;
        }

        private static OpcUaCfg.nodeObject_var GetObjectvar(DataModelObjectVariable dmObjectVar)
        {
            OpcUaCfg.nodeObject_var objectVar = new OpcUaCfg.nodeObject_var();
            objectVar.name = dmObjectVar.Name;
            objectVar.object_type_name = dmObjectVar.ObjectTypeName;
            objectVar.id = dmObjectVar.NodeId.GetNodeName();
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
            List<OpcUaCfg.connectionsConnectionGroup> groups = new List<connectionsConnectionGroup>();
            foreach (Group group in connection.Groups)
            {
                OpcUaCfg.connectionsConnectionGroup g = new connectionsConnectionGroup();
                g.period = group.Period;
                g.service = (group.Service == "Read") ? OpcUaCfg.client_service.Read : OpcUaCfg.client_service.Write;
                List<OpcUaCfg.connectionsConnectionGroupVar> groupVars = new List<connectionsConnectionGroupVar>();
                foreach (ClientVar clientVar in group.Vars)
                {
                    OpcUaCfg.connectionsConnectionGroupVar groupVar = new connectionsConnectionGroupVar();
                    groupVar.id = clientVar.Identifier;
                    groupVar.basic_type = GetBasicType(clientVar.SelectedBasicType);
                    groupVar.alias = clientVar.Alias;
                    groupVars.Add(groupVar);
                }

                g.var = groupVars.ToArray();
                groups.Add(g);
            }

            ct.group = groups.ToArray();
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
                connections.Add(ct);
            }
            cfg.connections = connections.ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(OpcUaCfg.OPCUAParametersType));
            using (TextWriter tw = new StreamWriter(fileName))
            {
                serializer.Serialize(tw, cfg);
                Debug.Print("Po serialize");
            }
        }
    }
}

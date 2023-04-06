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
        private readonly List<WpfControlLibrary.Client.ClientConnection> _connections;
        private readonly WpfControlLibrary.Model.ModOpcUa _opcua;
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
            _connections = new List<ClientConnection>();
            _publisherId = 1;
            _serverEncryption = false;
            _opcua = new WpfControlLibrary.Model.ModOpcUa();
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
            if (!WpfControlLibrary.Model.ModFlagsCollection.ModFlags.TryGetValue(pLabel, out WpfControlLibrary.Model.ModFlag flag))
            {
                throw new ApplicationException($"Unknown label {pLabel}");
            }
            string[] items = flag.Flag.Split('.');
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

            int idx = 0;
            string s = flag.Flag;
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
                    string sub = s.Substring(idx + 1);
                    ushort index = 0;
                    if(ushort.TryParse(sub, out ushort arrayIndex))
                    {
                        index = arrayIndex;
                    }
                    bw.Write(IntelMotorola(index));
                    s = s.Remove(idx, s.Length - idx);
                }
            }
            else
            {
                bw.Write(IntelMotorola((ushort)0));

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
            if (pd.DataContext is WpfControlLibrary.ViewModel.MainWindowViewModel vmp)
            {
                vmp.LoadXml(_opcua);
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

        public override void LoadConfig(string pName)
        {
            Debug.Print($"LoadConfig= {pName}");
            if (File.Exists(pName))
            {
                try
                {
                    _opcua.ReadXml(pName);
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
                    mvm.LoadXml(_opcua);
                    if ((bool)mainWindow.ShowDialog())
                    {
                        mvm.SaveXml(_opcua,pName);
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
    }
}

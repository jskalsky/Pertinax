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
        private readonly List<WpfControlLibrary.PublisherItem> _publisherItems;
        private readonly List<WpfControlLibrary.SubscriberItem> _subscriberItems;

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
            _publisherItems = new List<WpfControlLibrary.PublisherItem>();
            _subscriberItems = new List<WpfControlLibrary.SubscriberItem>();
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
            if(!_allPorts.TryGetValue(pLabel, out string port))
            {
                throw new ApplicationException($"Unknown label {pLabel}");
            }
            string[] items = port.Split('.');
            for(int i=0;i<items.Length;++i)
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
            if(items[3] == "Pub")
            {
                compiledType = 2;
            }
            else
            {
                if(items[3] == "Sub")
                {
                    compiledType = 3;
                }
            }
            ushort compiledIndex = 0;
            if(items[4].Contains("Sub") || items[4].Contains("Pub"))
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
                foreach(WpfControlLibrary.PortsNode pn in _ports)
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

        public override void LoadConfig(string pName)
        {
            Debug.Print($"LoadConfig= {pName}");
            if (File.Exists(pName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(OPCUAParametersType));
                using (TextReader tr = new StreamReader(pName))
                {
                    OPCUAParametersType pars = (OPCUAParametersType)serializer.Deserialize(tr);

                    if (pars.UsePublisher)
                    {
                        _publisherItems.Clear();
                        _publisherId = (int)pars.PublisherId;
                        Debug.Print($"_publisherId= {_publisherId}");
                        string[] ips = pars.Subscriber[0].Description.Split(';');
                        if(ips.Length==2)
                        {
                            _localIpAddress = ips[0];
                            _groupAddress = ips[1];
                            Debug.Print($"local= {_localIpAddress}, {_groupAddress}");
                        }
                        foreach(SubscriberType st in pars.Subscriber)
                        {
                            _publisherItems.Add(new WpfControlLibrary.PublisherItem(st.PublisherRootType, st.SendPeriod));
                        }
                    }
                    if(pars.UseSubscriber)
                    {
                        _subscriberItems.Clear();
                        foreach(PublisherType pt in pars.Publisher)
                        {
                            _subscriberItems.Add(new WpfControlLibrary.SubscriberItem(pt.SubscriberRootType, pt.PublisherId));
                        }
                    }
                    _objects.Clear();
                    foreach (ObjectTypeType ott in pars.ObjectType)
                    {
                        if(ott.Variables == null)
                        {
                            continue;
                        }
                        WpfControlLibrary.OpcObject oo = new WpfControlLibrary.OpcObject(ott.Name);
                        Debug.Print($"object= {oo.Name}");
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
                        _objects.Add(oo);
                    }
                    Debug.Print("1");
                }
                Debug.Print("2");
                CreatePorts();
                Debug.Print("3");
            }
            /*            Debug.Print($"LoadConfig {pName}");
                        if (File.Exists(pName))
                        {
                            File.WriteAllText("c:\\Workzat13.log", $"File exists");
                            XmlSerializer serializer = new XmlSerializer(typeof(OpcConfiguration));
                            File.WriteAllText("c:\\Workzat14.log", $"serializer {serializer}");
                            using (TextReader tr = new StreamReader(pName))
                            {
                                File.WriteAllText("c:\\Workzat11.log", $"tr {tr}");
                                OpcConfiguration opc = (OpcConfiguration)serializer.Deserialize(tr);
                                File.WriteAllText("c:\\Workzat12.log", $"opc {opc}, _vm= {_vm}");
                                _vm.GroupAddressString = opc.GroupIpAddress;
                                _vm.LocalIpAddressString = opc.LocalIpAddress;
                                _vm.Objects.Clear();
                                foreach (OpcConfigurationObject oco in opc.Objects)
                                {
                                    OpcObject oo = new OpcObject(oco.Name);
                                    oo.PublishingInterval = oco.PublishingInterval;
                                    oo.Pub = oco.Pub;
                                    oo.Sub = oco.Sub;
                                    foreach (OpcConfigurationObjectItem ocoi in oco.Items)
                                    {
                                        OpcObjectItem ooi = new OpcObjectItem(ocoi.Name);
                                        ooi.SelectedAccess = ocoi.Access;
                                        ooi.SelectedBasicType = ocoi.BasicType;
                                        ooi.SelectedRank = ocoi.Rank;
                                        ooi.ArraySizeValue = ocoi.ArraySize;
                                        oo.AddItem(ooi);
                                    }
                                    _vm.Objects.Add(oo);
                                }
                                _vm.SelectedOpcObject = _vm.Objects[0];
                            }
                        }
                        File.WriteAllText("c:\\Workzat11.log", $"LoadConfig end");*/
        }

        private void CreatePorts()
        {
            _ports.Clear();
            WpfControlLibrary.PortsNode outputs = new WpfControlLibrary.PortsNode("Outputs");
            WpfControlLibrary.PortsNode inputs = new WpfControlLibrary.PortsNode("Inputs");
            _ports.Add(outputs);
            _ports.Add(inputs);
            foreach (WpfControlLibrary.PublisherItem pi in _publisherItems)
            {
                foreach(WpfControlLibrary.OpcObject oo in _objects)
                {
                    if(pi.ObjectName == oo.Name)
                    {
                        WpfControlLibrary.PortsNode obj = outputs.Add(oo.Name);
                        foreach(WpfControlLibrary.OpcObjectItem ooi in oo.Items)
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
                }
            }
            foreach (WpfControlLibrary.SubscriberItem si in _subscriberItems)
            {
                foreach (WpfControlLibrary.OpcObject oo in _objects)
                {
                    if (si.ObjectName == oo.Name)
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
                    }
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
                if (!string.IsNullOrEmpty(_localIpAddress))
                {
                    Debug.Print("10\n");
                    vm.GroupAddressString = _groupAddress;
                    vm.LocalIpAddressString = _localIpAddress;
                }
                vm.Objects.Clear();
                Debug.Print($"Objects= {_objects.Count}");
                foreach (WpfControlLibrary.OpcObject oo in _objects)
                {
                    WpfControlLibrary.OpcObject opcObject = new WpfControlLibrary.OpcObject(oo);
                    vm.Objects.Add(opcObject);
                    foreach (WpfControlLibrary.OpcObjectItem ooi in oo.Items)
                    {
                        opcObject.AddItem(new WpfControlLibrary.OpcObjectItem(ooi));
                    }
                }
                if (vm.Objects.Count > 0)
                {
                    Debug.Print($"20");
                    vm.SelectedOpcObject = vm.Objects[0];
                }
                foreach(WpfControlLibrary.PublisherItem pi in _publisherItems)
                {
                    Debug.Print($"21");
                    vm.PublisherObjects.Add(pi);
                }
                if(vm.PublisherObjects.Count != 0)
                {
                    Debug.Print($"22");
                    vm.SelectedPublisherItem = vm.PublisherObjects[0];
                }
                vm.PublisherId = _publisherId;

                foreach(WpfControlLibrary.SubscriberItem si in _subscriberItems)
                {
                    vm.SubscriberObjects.Add(si);
                }
                if(vm.SubscriberObjects.Count != 0)
                {
                    vm.SelectedSubscriberItem = vm.SubscriberObjects[0];
                }
                Debug.Print($"23");
            }

            if ((bool)mainWindow.ShowDialog())
            {
                if (mainWindow.DataContext is WpfControlLibrary.MainViewModel mvm)
                {
                    SaveConfiguration(pName, mvm);
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
        private void SaveConfiguration(string fileName, WpfControlLibrary.MainViewModel mvm)
        {
            OPCUAParametersType pars = new OPCUAParametersType();
            pars.ObjectTypeCount = (ushort)mvm.Objects.Count;
            pars.UsePublisher = (mvm.PublisherObjects.Count != 0) ? true : false;
            pars.UseSubscriber = (mvm.SubscriberObjects.Count != 0) ? true : false;
//            pars.UseServer = true;
            pars.PublisherId = (ushort)mvm.PublisherId;

            List<SubscriberType> sts = new List<SubscriberType>();
            foreach (WpfControlLibrary.PublisherItem publisherItem in mvm.PublisherObjects)
            {
                SubscriberType st = new SubscriberType();
                st.SendPeriod = (ushort)publisherItem.PublishingInterval;
                st.PublisherRootType = publisherItem.ObjectName;
                st.Description = $"{mvm.LocalIpAddressString};{mvm.GroupAddressString}";
                sts.Add(st);
                pars.Subscriber = sts.ToArray();
            }
            pars.SubscribersCount = (ushort)mvm.PublisherObjects.Count;

            List<PublisherType> pts = new List<PublisherType>();
            foreach(WpfControlLibrary.SubscriberItem subscriberItem in mvm.SubscriberObjects)
            {
                PublisherType pt = new PublisherType();
                pt.PublisherId = (ushort)subscriberItem.PublisherId;
                pt.SubscriberRootType = subscriberItem.ObjectName;
                pt.Description = $"{mvm.LocalIpAddressString};{mvm.GroupAddressString}";
                pts.Add(pt);
                pars.Publisher = pts.ToArray();
            }

            /*            List<SubscriberType> subscribers = new List<SubscriberType>();
                        SubscriberType st = new SubscriberType();
                        IPAddress ipAddress = IPAddress.Parse(mvm.GroupAddressString);
                        st.IpAddress = IntelMotorola(BitConverter.ToUInt32(ipAddress.GetAddressBytes(), 0));
                        IPAddress localAddress = IPAddress.Parse(mvm.LocalIpAddressString);
                        st.LocalAddress = IntelMotorola(BitConverter.ToUInt32(localAddress.GetAddressBytes(), 0));
                        st.PublisherRootType = "Pertinax";
                        st.SendPeriod = sendPeriod;
                        subscribers.Add(st);
                        pars.Subscriber = subscribers.ToArray();*/

            List<ObjectTypeType> objects = new List<ObjectTypeType>();
            ushort id = 1;
            List<VariablesType> variables = new List<VariablesType>();
            foreach (WpfControlLibrary.OpcObject oo in mvm.Objects)
            {
                ObjectTypeType ott = new ObjectTypeType();
                ott.Id = id++;
                ott.Name = oo.Name;
                ott.VariablesCount = (ushort)oo.Items.Count;
                variables.Clear();
                foreach (WpfControlLibrary.OpcObjectItem ooi in oo.Items)
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
                objects.Add(ott);
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

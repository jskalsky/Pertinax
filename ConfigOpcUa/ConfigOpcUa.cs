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
        private readonly Dictionary<string, byte> _basicTypes = new Dictionary<string, byte>() { { "Boolean", 0 }, { "UInt8", 1 }, { "UInt16", 2 }, { "UInt32", 3 }, { "Int8", 4 }, { "Int16", 5 },
            {"Int32", 6 }, {"Float", 7 }, {"Double", 8 } };
        private readonly Dictionary<string, byte> _access = new Dictionary<string, byte>() { { "Read", 0 }, { "Write", 1 }, { "ReadWrite", 2 } };
        private readonly List<WpfControlLibrary.OpcObject> _objects;
        private string _localIpAddress;
        private string _groupAddress;
        public ConfigOpcUa()
        {
            lName = "OpcUa";
            lDescription = "OpcUa";
            lVersion = "1.0.0.0";
            /*            string appFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "Pertinax";
                        if (!Directory.Exists(appFolder)) Directory.CreateDirectory(appFolder);
            #if DEBUG
                        FileStream myTraceLog = new FileStream(appFolder + System.IO.Path.DirectorySeparatorChar + "ConfigOpcUa.deb", FileMode.Create, FileAccess.Write, FileShare.Write);
                        TextWriterTraceListener myListener = new TextWriterTraceListener(myTraceLog);
                        Debug.Listeners.Add(myListener);
                        Debug.AutoFlush = true;
                        Debug.WriteLine("Start");
            #endif*/
            _objects = new List<WpfControlLibrary.OpcObject>();
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
            return null;
        }

        public override string CreatePort(System.Windows.Forms.IWin32Window hWnd)
        {
            Debug.Print($"CreatePort");
            WpfControlLibrary.PortDialog pd = new WpfControlLibrary.PortDialog();
            if(pd.DataContext is WpfControlLibrary.ViewModelPorts vmp)
            {
                foreach (WpfControlLibrary.OpcObject oo in _objects)
                {
                    if (oo.Pub)
                    {
                        WpfControlLibrary.PortsNode outputs = new WpfControlLibrary.PortsNode("Outputs");
                        foreach (WpfControlLibrary.OpcObjectItem ooi in oo.Items)
                        {
                            outputs.Add($"O.OPCUA.R.{oo.Name}.{ooi.Name}");
                        }
                        vmp.RootNodes.Add(outputs);
                    }
                }
            }
            if ((bool)pd.ShowDialog())
            {

            }
            return string.Empty;
        }

        public override void LoadConfig(string pName)
        {
            if (File.Exists(pName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(OPCUAParametersType));
                using (TextReader tr = new StreamReader(pName))
                {
                    OPCUAParametersType pars = (OPCUAParametersType)serializer.Deserialize(tr);
                    ushort sendPeriod = pars.Subscriber[0].SendPeriod;
                    byte[] groupAddressBytes = BitConverter.GetBytes(pars.Subscriber[0].IpAddress);
                    IPAddress groupAddress = new IPAddress(groupAddressBytes);
                    _groupAddress = groupAddress.ToString();
                    byte[] localAddressBytes = BitConverter.GetBytes((uint)pars.Subscriber[0].LocalAddress);
                    IPAddress localAddress = new IPAddress(localAddressBytes);
                    _localIpAddress = localAddress.ToString();
                    foreach (ObjectTypeType ott in pars.ObjectType)
                    {
                        WpfControlLibrary.OpcObject oo = new WpfControlLibrary.OpcObject(ott.Name);
                        if (sendPeriod != 0)
                        {
                            oo.Pub = true;
                        }
                        oo.PublishingInterval = sendPeriod;
                        foreach (VariablesType vt in ott.Variables)
                        {
                            WpfControlLibrary.OpcObjectItem ooi = new WpfControlLibrary.OpcObjectItem(vt.Name);
                            ooi.SelectedBasicType = GetBasicType(vt.BasicType);
                            ooi.SelectedAccess = GetAccess(vt.AccessType);
                            ooi.ArraySizeValue = vt.ArraySize;
                            ooi.SelectedRank = (vt.Type == 0) ? "SimpleVariable" : "Array";
                            oo.AddItem(ooi);
                        }
                        _objects.Add(oo);
                    }
                }
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

        public override void MakeConfig(System.Windows.Forms.IWin32Window hWnd, string pName)
        {
            //            Debug.Print($"MakeConfig {pName}");
            WpfControlLibrary.MainWindow mainWindow = new WpfControlLibrary.MainWindow();
            if (mainWindow.DataContext is WpfControlLibrary.MainViewModel vm)
            {
                if (!string.IsNullOrEmpty(_localIpAddress))
                {
                    vm.GroupAddressString = _groupAddress;
                    vm.LocalIpAddressString = _localIpAddress;
                }
                vm.Objects.Clear();
                foreach (WpfControlLibrary.OpcObject oo in _objects)
                {
                    WpfControlLibrary.OpcObject opcObject = new WpfControlLibrary.OpcObject(oo);
                    vm.Objects.Add(opcObject);
                    foreach (WpfControlLibrary.OpcObjectItem ooi in oo.Items)
                    {
                        opcObject.AddItem(new WpfControlLibrary.OpcObjectItem(ooi));
                    }
                }
                vm.SelectedOpcObject = vm.Objects[0];
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
            pFiles = null;
        }

        public override void StartupLines(out string pLinesW, out string pLines, string pName, int typ)
        {
            Debug.Print($"StartupLines {pName}, {typ}");
            pLines = null;
            pLinesW = null;
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
            pars.UsePublisher = true;
            pars.SubscribersCount = 1;
            pars.PublisherId = 1;

            ushort sendPeriod = 0;

            foreach (WpfControlLibrary.OpcObject oo in mvm.Objects)
            {
                if (oo.Pub)
                {
                    List<PublisherType> pts = new List<PublisherType>();
                    PublisherType pt = new PublisherType();
                    pt.PublisherId = 1;
                    pt.SubscriberRootType = "Pertinax";
                    pts.Add(pt);
                    pars.Publisher = pts.ToArray();
                    sendPeriod = (ushort)oo.PublishingInterval;
                    break;
                }
            }

            List<SubscriberType> subscribers = new List<SubscriberType>();
            SubscriberType st = new SubscriberType();
            IPAddress ipAddress = IPAddress.Parse(mvm.GroupAddressString);
            st.IpAddress = BitConverter.ToUInt32(ipAddress.GetAddressBytes(), 0);
            IPAddress localAddress = IPAddress.Parse(mvm.LocalIpAddressString);
            st.LocalAddress = BitConverter.ToUInt32(localAddress.GetAddressBytes(), 0);
            st.PublisherRootType = "Pertinax";
            st.SendPeriod = sendPeriod;
            subscribers.Add(st);
            pars.Subscriber = subscribers.ToArray();

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

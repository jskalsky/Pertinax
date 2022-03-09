using ConfigOpcUaNet;
using OpcUaPars;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConfigOpcUa
{
    public class ConfigOpcUa : ConfigPtx.CfgPtx
    {
        private readonly Dictionary<string, byte> _basicTypes = new Dictionary<string, byte>() { { "Boolean", 0 }, { "UInt8", 1 }, { "UInt16", 2 }, { "UInt32", 3 }, { "Int8", 4 }, { "Int16", 5 },
            {"Int32", 6 }, {"Float", 7 }, {"Double", 8 } };
        private ViewModel _vm;
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
            _vm = new ViewModel();
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
            return string.Empty;
        }

        public override void LoadConfig(string pName)
        {
            Debug.Print($"LoadConfig {pName}");
            if (File.Exists(pName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(OpcConfiguration));
                using (TextReader tr = new StreamReader(pName))
                {
                    OpcConfiguration opc = (OpcConfiguration)serializer.Deserialize(tr);
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
        }

        public override void MakeConfig(System.Windows.Forms.IWin32Window hWnd, string pName)
        {
            Debug.Print($"MakeConfig {pName}");
            MainWindow mainWindow = new MainWindow();
            mainWindow.DataContext = _vm;
            if ((bool)mainWindow.ShowDialog())
            {
                //                    SaveConfiguration("c:\\Work\\ExportCfg.xml", vm);
                SaveConfiguration(pName, _vm);
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

        private void SaveConfiguration(string fileName, ViewModel vm)
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
        }
        private void ExportToXml(string fileName)
        {
            /*            OPCUAParametersType pars = new OPCUAParametersType();
                        pars.ObjectTypeCount = 1;
                        pars.UsePublisher = true;
                        pars.SubscribersCount = 1;
                        pars.PublisherId = 1;

                        List<SubscriberType> subscribers = new List<SubscriberType>();
                        SubscriberType st = new SubscriberType();
                        st.IpAddress = 168431100;
                        st.LocalAddress = 723406057539371008;
                        st.PublisherRootType = "Pertinax";
                        subscribers.Add(st);
                        pars.Subscriber = subscribers.ToArray();

                        List<ObjectTypeType> objects = new List<ObjectTypeType>();
                        ushort id = 1;
                        List<VariablesType> variables = new List<VariablesType>();
                        foreach(OpcObject oo in _viewModel.Objects)
                        {
                            ObjectTypeType ott = new ObjectTypeType();
                            ott.Id = id++;
                            ott.Name = oo.Name;
                            ott.VariablesCount = (ushort)oo.Items.Count;
                            variables.Clear();
                            foreach(OpcObjectItem ooi in oo.Items)
                            {
                                if(_basicTypes.TryGetValue(ooi.BasicType, out byte typeCode))
                                {
                                    VariablesType vt = new VariablesType();
                                    vt.Name = ooi.Name;
                                    vt.BasicType = typeCode;
                                    variables.Add(vt);
                                }
                            }
                            ott.Variables = variables.ToArray();
                            objects.Add(ott);
                        }
                        pars.ObjectType = objects.ToArray();

                        XmlSerializer serializer = new XmlSerializer(typeof(OPCUAParametersType));
                        using(TextWriter tw = new StreamWriter(fileName))
                        {
                            serializer.Serialize(tw, pars);
                        }*/
        }
    }
}

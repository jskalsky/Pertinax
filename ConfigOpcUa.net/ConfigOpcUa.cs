using OpcUaPars;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConfigOpcUaNet
{
    public class ConfigOpcUa : ConfigPtx.CfgPtx
    {
        private readonly Dictionary<string, byte> _basicTypes = new Dictionary<string, byte>() { { "Boolean", 0 }, { "UInt8", 1 }, { "UInt16", 2 }, { "UInt32", 3 }, { "Int8", 4 }, { "Int16", 5 },
            {"Int32", 6 }, {"Float", 7 }, {"Double", 8 } };
        public ConfigOpcUa()
        {
        }
        public override void CheckProject(string pName, string pErrName)
        {
        }

        public override byte[] CheCoLabel(int mod, string pLabel)
        {
            return null;
        }

        public override string CreatePort(System.Windows.Forms.IWin32Window hWnd)
        {
            return string.Empty;
        }

        public override void LoadConfig(string pName)
        {
        }

        public override void MakeConfig(System.Windows.Forms.IWin32Window hWnd, string pName)
        {
            MainWindow mainWindow = new MainWindow();
            if((bool)mainWindow.ShowDialog())
            {
                if(mainWindow.DataContext is ViewModel vm)
                {
                    SaveConfiguration("c:\\Work\\ExportCfg.xml", vm);
                }
            }
        }

        public override void OS9Files(out string pFiles, string pName, int typ)
        {
            pFiles = string.Empty;
        }

        public override void StartupLines(out string pLinesW, out string pLines, string pName, int typ)
        {
            pLinesW = string.Empty;
            pLines = string.Empty;
        }

        public override void UnLoadConfig()
        {
        }

        private void SaveConfiguration(string fileName, ViewModel vm)
        {
            opcConfigurationType opc = new opcConfigurationType();
            opc.GroupIpAddress = vm.GroupAddressString;
            List<objectType> objects = new List<objectType>();
            List<objectItemType> items = new List<objectItemType>();
            foreach(OpcObject oo in vm.Objects)
            {
                objectType ot = new objectType();
                ot.Name = oo.Name;
                ot.PublishingInterval = (ushort)oo.PublishingInterval;
                items.Clear();
                foreach(OpcObjectItem ooi in oo.Items)
                {
                    objectItemType oit = new objectItemType();
                    oit.Name = ooi.Name;
                    oit.ArraySize = (ushort)ooi.ArraySizeValue;
                    oit.Access = ooi.SelectedAccess;
                    oit.BasicType = ooi.SelectedBasicType;
                    oit.Rank = ooi.SelectedRank;
                    items.Add(oit);
                }
                ot.Items = items.ToArray();
                objects.Add(ot);
            }
            opc.Objects = objects.ToArray();
            XmlSerializer serializer = new XmlSerializer(typeof(opcConfigurationType));
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

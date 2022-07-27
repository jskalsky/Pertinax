using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WpfControlLibrary.Model
{
    public class ModOpcUa
    {
        public static readonly string[] BasicTypes = new string[] { "Boolean", "UInt8", "Int8", "UInt16", "Int16", "UInt32", "Int32", "Float", "Double" };
        public static readonly string[] VarAccess = new string[] { "Read", "Write", "ReadWrite" };
        public static readonly string[] ClientService = new string[] { "Read", "Write" };
        public static readonly string[] NodeIdType = new string[] { "UInt32", "String", "Guid", "ByteString" };

        private List<ModNode> _nodes;
        public ModOpcUa()
        {
            _nodes = new List<ModNode>();
        }
        public string LocalIpAddress { get; set; }
        public string MulticastIpAddress { get; set; }
        public IList<ModNode> Nodes => _nodes;
        public void GenerateTest()
        {
            ModNodeServer server = new ModNodeServer("Server", false);
            _nodes.Add(server);
            ModNodeNs ns0 = new ModNodeNs("Ns0", 0);
            ModNodeNs ns1 = new ModNodeNs("Ns1", 1);
            ModNodeNs ns2 = new ModNodeNs("Ns2", 2);
            server.AddSubNode(ns0);
            server.AddSubNode(ns1);
            server.AddSubNode(ns2);
            ModNodeFolder folderObjects = new ModNodeFolder("Objects", new ModNodeIdNumeric(1, 1000));
            ModNodeFolder folderVariables = new ModNodeFolder("Variables", new ModNodeIdNumeric(1, 1001));
            ns1.AddSubNode(folderObjects);
            ns1.AddSubNode(folderVariables);
            for (int i = 0; i < 10; ++i)
            {
                folderVariables.AddSubNode(new ModNodeVariable($"Var{i + 1}", new ModNodeIdNumeric(1, (uint)(5000 + i)), basic_type.Float, access.Read));
            }
        }
        public static access GetAccess(string s)
        {
            switch (s)
            {
                case "Read":
                    return access.Read;
                case "Write":
                    return access.Write;
                case "ReadWrite":
                    return access.ReadWrite;
            }
            return access.Unknown;
        }

        public static string GetAccess(access access)
        {
            switch (access)
            {
                case access.Read:
                    return "Read";
                case access.Write:
                    return "Write";
                case access.ReadWrite:
                    return "ReadWrite";
            }
            return "Unknown";
        }
        public static basic_type GetBasicType(string s)
        {
            switch (s)
            {
                case "Boolean":
                    return basic_type.Boolean;
                case "UInt8":
                    return basic_type.UInt8;
                case "Int8":
                    return basic_type.Int8;
                case "UInt16":
                    return basic_type.UInt16;
                case "Int16":
                    return basic_type.Int16;
                case "UInt32":
                    return basic_type.UInt32;
                case "Int32":
                    return basic_type.Int32;
                case "Float":
                    return basic_type.Float;
                case "Double":
                    return basic_type.Double;
            }
            return basic_type.Unknown;
        }
        public static string GetBasicType(basic_type s)
        {
            switch (s)
            {
                case basic_type.Boolean:
                    return "Boolean";
                case basic_type.UInt8:
                    return "UInt8";
                case basic_type.Int8:
                    return "Int8";
                case basic_type.UInt16:
                    return "UInt16";
                case basic_type.Int16:
                    return "Int16";
                case basic_type.UInt32:
                    return "UInt32";
                case basic_type.Int32:
                    return "Int32";
                case basic_type.Float:
                    return "Float";
                case basic_type.Double:
                    return "Double";
            }
            return "Unknown";
        }

        public static string GetClientService(client_service cs)
        {
            switch (cs)
            {
                case client_service.Read:
                    return "Read";
                case client_service.Write:
                    return "Write";
            }
            return "Unknown";
        }
        public static client_service GetClientService(string s)
        {
            switch (s)
            {
                case "Read":
                    return client_service.Read;
                case "Write":
                    return client_service.Write;
            }
            return client_service.Unknown;
        }
        private void SaveTreeNode(ModNode node, node tn)
        {
            Debug.Print($"SaveTreeNode {node}");
            if (node is ModNodeNs modNs)
            {
                nodeNamespace objNamespace = new nodeNamespace();
                objNamespace.index = modNs.NsIndex;
                tn.Item = objNamespace;
                tn.node_type = node_type.Namespace;
            }
            else
            {
                if (node is ModNodeFolder modFolder)
                {
                    nodeFolder folder = new nodeFolder();
                    folder.name = modFolder.Name;
                    folder.id = modFolder.NodeId.GetText();
                    tn.Item = folder;
                    tn.node_type = node_type.Folder;
                }
                else
                {
                    if (node is ModNodeVariable modVariable)
                    {
                        nodeSimple_var simple = new nodeSimple_var();
                        simple.name = modVariable.Name;
                        simple.access = modVariable.Access;
                        simple.basic_type = modVariable.Type;
                        simple.id = modVariable.NodeId.GetText();
                        tn.Item = simple;
                        tn.node_type = node_type.SimpleVariable;
                    }
                    else
                    {
                        if (node is ModNodeArrayVariable modArrayVariable)
                        {
                            nodeArray_var array = new nodeArray_var();
                            array.name = modArrayVariable.Name;
                            array.access = modArrayVariable.Access;
                            array.basic_type = modArrayVariable.Type;
                            array.length = modArrayVariable.ArrayLength;
                            tn.Item = array;
                            tn.node_type = node_type.ArrayVariable;
                        }
                        else
                        {
                            if (node is ModNodeObjectType modObjectType)
                            {
                                nodeObject_type ot = new nodeObject_type();
                                ot.name = modObjectType.Name;
                                ot.id = modObjectType.NodeId.GetText();
                                tn.Item = ot;
                                tn.node_type = node_type.ObjectType;
                            }
                            else
                            {
                                if (node is ModNodeObject modObject)
                                {
                                    nodeObject_var o = new nodeObject_var();
                                    o.name = modObject.Name;
                                    o.object_type_name = modObject.ObjectType;
                                    o.id = modObject.NodeId.GetText();
                                    tn.Item = o;
                                    tn.node_type = node_type.ObjectVariable;
                                }
                            }
                        }
                    }
                }
            }
            if (node.SubNodes.Count != 0)
            {
                tn.sub_nodes = new node[node.SubNodes.Count];
                int index = 0;
                foreach (ModNode child in node.SubNodes)
                {
                    tn.sub_nodes[index] = new node();
                    SaveTreeNode(child, tn.sub_nodes[index]);
                    ++index;
                }
            }
            else
            {
                tn.sub_nodes = new node[] { };
            }
        }
        private void LoadNode(ModNode parent, node n, ref int nrErrors)
        {
            Debug.Print($"LoadNode {n.node_type}, parent= {parent}");
            ModNode modNode = null;
            if (n.Item is nodeFolder folder)
            {
                Debug.Print($"LoadNode FOLDER= {folder.name}, {folder.id}");
                ModNodeId nodeId = ModNodeId.GetModNodeId(folder.id);
                modNode = new ModNodeFolder(folder.name, nodeId);
            }
            else
            {
                if (n.Item is nodeNamespace nodeNamespace)
                {
                    Debug.Print($"LoadNode NS= {nodeNamespace.index}");
                    modNode = new ModNodeNs($"Ns{nodeNamespace.index}", nodeNamespace.index);
                }
                else
                {
                    if (n.Item is nodeSimple_var simpleVar)
                    {
                        Debug.Print($"LoadNode SIMPLE= {simpleVar.name}");
                        modNode = new ModNodeVariable(simpleVar.name, ModNodeId.GetModNodeId(simpleVar.id),
                            simpleVar.basic_type, simpleVar.access);
                    }
                    else
                    {
                        if (n.Item is nodeArray_var arrayVar)
                        {
                            Debug.Print($"LoadNode ARRAY= {arrayVar.name}");
                            modNode = new ModNodeArrayVariable(arrayVar.name, ModNodeId.GetModNodeId(arrayVar.id),
                                arrayVar.basic_type, arrayVar.access, (int)arrayVar.length);
                        }
                        else
                        {
                            if (n.Item is nodeObject_type objectType)
                            {
                                Debug.Print($"OT {objectType.id}");
                                modNode = new ModNodeObjectType(objectType.name, ModNodeId.GetModNodeId(objectType.id));
                            }
                            else
                            {
                                if (n.Item is nodeObject_var objectVar)
                                {
                                    Debug.Print($"LoadNode 4");
                                    modNode = new ModNodeObject(objectVar.name, ModNodeId.GetModNodeId(objectVar.id),
                                        objectVar.object_type_name);
                                }
                            }
                        }
                    }
                }
            }
            Debug.Print($"parent= {parent}, modNode= {modNode}");
            if (parent == null || modNode == null)
            {
                Debug.Print($"Hruba chyba");
                return;
            }
            parent.AddSubNode(modNode);
            Debug.Print("100");
            if (n.sub_nodes != null)
            {
                Debug.Print($"101, {n.sub_nodes}, {n.sub_nodes.Length}");
                foreach (node child in n.sub_nodes)
                {
                    LoadNode(modNode, child, ref nrErrors);
                }
            }
        }

        public void WriteXml(string fileName)
        {
            OPCUAParametersType cfg = new OPCUAParametersType();
            cfg.settings = new settings();
            cfg.settings.local_ip = LocalIpAddress;
            cfg.settings.multicast_ip = MulticastIpAddress;
            List<node> nodes = new List<node>();
            List<OPCUAParametersTypeClient> clients = new List<OPCUAParametersTypeClient>();
            foreach (ModNode modNode in _nodes)
            {
                if (modNode is ModNodeServer modServer)
                {
                    cfg.server = new OPCUAParametersTypeServer();
                    cfg.server.encryption = modServer.Encrypt;
                    foreach (ModNode sub in modServer.SubNodes)
                    {
                        node tn = new node();
                        SaveTreeNode(sub, tn);
                        nodes.Add(tn);
                    }
                }
                else
                {
                    if (modNode is ModNodeClient modClient)
                    {
                        OPCUAParametersTypeClient client = new OPCUAParametersTypeClient();
                        client.name = modClient.Name;
                        client.encryption = modClient.Encrypt;
                        List<OPCUAParametersTypeClientGroup> groups = new List<OPCUAParametersTypeClientGroup>();
                        foreach (ModNodeClientGroup group in modClient.SubNodes)
                        {
                            OPCUAParametersTypeClientGroup groupGroup = new OPCUAParametersTypeClientGroup();
                            groupGroup.name = group.Name;
                            groupGroup.period = group.Period;
                            groupGroup.service = group.Service;
                            groups.Add(groupGroup);
                            List<OPCUAParametersTypeClientGroupVar> vars = new List<OPCUAParametersTypeClientGroupVar>();
                            foreach (ModNodeClientVar clientVar in group.SubNodes)
                            {
                                OPCUAParametersTypeClientGroupVar groupVar = new OPCUAParametersTypeClientGroupVar();
                                groupVar.name = clientVar.Name;
                                groupVar.id = clientVar.NodeId;
                                groupVar.basic_type = GetBasicType(clientVar.Type);
                                vars.Add(groupVar);
                            }
                            groupGroup.var = vars.ToArray();
                        }
                        client.group = groups.ToArray();
                        clients.Add(client);
                    }
                }
            }
            cfg.nodes = nodes.ToArray();
            cfg.client = clients.ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(OPCUAParametersType));
            using (TextWriter tw = new StreamWriter(fileName))
            {
                serializer.Serialize(tw, cfg);
            }
        }
        public void ReadXml(string fileName)
        {
            if (File.Exists(fileName))
            {
                Debug.Print($"Existed");
                XmlSerializer serializer = new XmlSerializer(typeof(OPCUAParametersType));
                using (TextReader tr = new StreamReader(fileName))
                {
                    OPCUAParametersType cfg = (OPCUAParametersType)serializer.Deserialize(tr);
                    LocalIpAddress = cfg.settings.local_ip;
                    MulticastIpAddress = cfg.settings.multicast_ip;
                    if (cfg.server != null)
                    {
                        ModNodeServer modServer = new ModNodeServer("Server", cfg.server.encryption);
                        _nodes.Add(modServer);
                        int nrErrors = 0;
                        foreach (node n in cfg.nodes)
                        {
                            LoadNode(modServer, n, ref nrErrors);
                        }
                    }
                    if (cfg.client != null)
                    {
                        foreach (OPCUAParametersTypeClient client in cfg.client)
                        {
                            ModNodeClient modClient = new ModNodeClient(client.name, client.encryption, client.ip_address);
                            _nodes.Add(modClient);
                            if (client.group != null)
                            {
                                foreach (OPCUAParametersTypeClientGroup group in client.group)
                                {
                                    ModNodeClientGroup clientGroup = new ModNodeClientGroup(group.name, group.period, group.service);
                                    modClient.AddSubNode(clientGroup);
                                    if (group.var != null)
                                    {
                                        foreach (OPCUAParametersTypeClientGroupVar groupVar in group.var)
                                        {
                                            ModNodeClientVar clientvar = new ModNodeClientVar(groupVar.name, groupVar.id,
                                                GetBasicType(groupVar.basic_type));
                                            clientGroup.AddSubNode(clientvar);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                ModNodeServer modServer = new ModNodeServer("Server", false);
                ModNodeNs ns0 = new ModNodeNs("Ns0", 0);
                ModNodeNs ns1 = new ModNodeNs("Ns1", 1);
                ModNodeNs ns2 = new ModNodeNs("Ns2", 2);
                modServer.AddSubNode(ns0);
                modServer.AddSubNode(ns1);
                modServer.AddSubNode(ns2);
                _nodes.Add(modServer);
                DefaultDataModel.Setup(ns0);
            }
        }
    }
}

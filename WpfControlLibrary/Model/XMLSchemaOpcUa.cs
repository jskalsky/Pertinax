﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Tento kód byl generován nástrojem.
//     Verze modulu runtime:4.0.30319.42000
//
//     Změny tohoto souboru mohou způsobit nesprávné chování a budou ztraceny,
//     dojde-li k novému generování kódu.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.7.3081.0.
// 
namespace WpfControlLibrary.Model {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.zat.cz/OPCUAParameters")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.zat.cz/OPCUAParameters", IsNullable=false)]
    public partial class settings {
        
        private string local_ipField;
        
        private string multicast_ipField;
        
        /// <remarks/>
        public string local_ip {
            get {
                return this.local_ipField;
            }
            set {
                this.local_ipField = value;
            }
        }
        
        /// <remarks/>
        public string multicast_ip {
            get {
                return this.multicast_ipField;
            }
            set {
                this.multicast_ipField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.zat.cz/OPCUAParameters")]
    public partial class node {
        
        private node_type node_typeField;
        
        private object itemField;
        
        private node[] sub_nodesField;
        
        /// <remarks/>
        public node_type node_type {
            get {
                return this.node_typeField;
            }
            set {
                this.node_typeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("array_var", typeof(nodeArray_var))]
        [System.Xml.Serialization.XmlElementAttribute("folder", typeof(nodeFolder))]
        [System.Xml.Serialization.XmlElementAttribute("namespace", typeof(nodeNamespace))]
        [System.Xml.Serialization.XmlElementAttribute("object_type", typeof(nodeObject_type))]
        [System.Xml.Serialization.XmlElementAttribute("object_var", typeof(nodeObject_var))]
        [System.Xml.Serialization.XmlElementAttribute("simple_var", typeof(nodeSimple_var))]
        public object Item {
            get {
                return this.itemField;
            }
            set {
                this.itemField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("sub_node", IsNullable=false)]
        public node[] sub_nodes {
            get {
                return this.sub_nodesField;
            }
            set {
                this.sub_nodesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.zat.cz/OPCUAParameters")]
    public enum node_type {
        
        /// <remarks/>
        Unknown,
        
        /// <remarks/>
        Namespace,
        
        /// <remarks/>
        Folder,
        
        /// <remarks/>
        ObjectType,
        
        /// <remarks/>
        ObjectVariable,
        
        /// <remarks/>
        SimpleVariable,
        
        /// <remarks/>
        ArrayVariable,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.zat.cz/OPCUAParameters")]
    public partial class nodeArray_var {
        
        private string nameField;
        
        private basic_type basic_typeField;
        
        private access accessField;
        
        private string idField;
        
        private int lengthField;
        
        /// <remarks/>
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public basic_type basic_type {
            get {
                return this.basic_typeField;
            }
            set {
                this.basic_typeField = value;
            }
        }
        
        /// <remarks/>
        public access access {
            get {
                return this.accessField;
            }
            set {
                this.accessField = value;
            }
        }
        
        /// <remarks/>
        public string id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public int length {
            get {
                return this.lengthField;
            }
            set {
                this.lengthField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.zat.cz/OPCUAParameters")]
    public enum basic_type {
        
        /// <remarks/>
        Unknown,
        
        /// <remarks/>
        Boolean,
        
        /// <remarks/>
        UInt8,
        
        /// <remarks/>
        Int8,
        
        /// <remarks/>
        UInt16,
        
        /// <remarks/>
        Int16,
        
        /// <remarks/>
        UInt32,
        
        /// <remarks/>
        Int32,
        
        /// <remarks/>
        Float,
        
        /// <remarks/>
        Double,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.zat.cz/OPCUAParameters")]
    public enum access {
        
        /// <remarks/>
        Unknown,
        
        /// <remarks/>
        Read,
        
        /// <remarks/>
        Write,
        
        /// <remarks/>
        ReadWrite,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.zat.cz/OPCUAParameters")]
    public partial class nodeFolder {
        
        private string nameField;
        
        private string idField;
        
        /// <remarks/>
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public string id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.zat.cz/OPCUAParameters")]
    public partial class nodeNamespace {
        
        private ushort indexField;
        
        /// <remarks/>
        public ushort index {
            get {
                return this.indexField;
            }
            set {
                this.indexField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.zat.cz/OPCUAParameters")]
    public partial class nodeObject_type {
        
        private string nameField;
        
        private string idField;
        
        /// <remarks/>
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public string id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.zat.cz/OPCUAParameters")]
    public partial class nodeObject_var {
        
        private string nameField;
        
        private string idField;
        
        private string object_type_nameField;
        
        /// <remarks/>
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public string id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public string object_type_name {
            get {
                return this.object_type_nameField;
            }
            set {
                this.object_type_nameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.zat.cz/OPCUAParameters")]
    public partial class nodeSimple_var {
        
        private string nameField;
        
        private basic_type basic_typeField;
        
        private access accessField;
        
        private string idField;
        
        /// <remarks/>
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public basic_type basic_type {
            get {
                return this.basic_typeField;
            }
            set {
                this.basic_typeField = value;
            }
        }
        
        /// <remarks/>
        public access access {
            get {
                return this.accessField;
            }
            set {
                this.accessField = value;
            }
        }
        
        /// <remarks/>
        public string id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.zat.cz/OPCUAParameters")]
    [System.Xml.Serialization.XmlRootAttribute("OPCUAParameters", Namespace="http://www.zat.cz/OPCUAParameters", IsNullable=false)]
    public partial class OPCUAParametersType {
        
        private settings settingsField;
        
        private OPCUAParametersTypeServer serverField;
        
        private node[] nodesField;
        
        private OPCUAParametersTypeClient[] clientField;
        
        /// <remarks/>
        public settings settings {
            get {
                return this.settingsField;
            }
            set {
                this.settingsField = value;
            }
        }
        
        /// <remarks/>
        public OPCUAParametersTypeServer server {
            get {
                return this.serverField;
            }
            set {
                this.serverField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("nodes")]
        public node[] nodes {
            get {
                return this.nodesField;
            }
            set {
                this.nodesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("client")]
        public OPCUAParametersTypeClient[] client {
            get {
                return this.clientField;
            }
            set {
                this.clientField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.zat.cz/OPCUAParameters")]
    public partial class OPCUAParametersTypeServer {
        
        private bool encryptionField;
        
        /// <remarks/>
        public bool encryption {
            get {
                return this.encryptionField;
            }
            set {
                this.encryptionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.zat.cz/OPCUAParameters")]
    public partial class OPCUAParametersTypeClient {
        
        private string nameField;
        
        private string ip_addressField;
        
        private bool encryptionField;
        
        private OPCUAParametersTypeClientGroup[] groupField;
        
        /// <remarks/>
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public string ip_address {
            get {
                return this.ip_addressField;
            }
            set {
                this.ip_addressField = value;
            }
        }
        
        /// <remarks/>
        public bool encryption {
            get {
                return this.encryptionField;
            }
            set {
                this.encryptionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("group")]
        public OPCUAParametersTypeClientGroup[] group {
            get {
                return this.groupField;
            }
            set {
                this.groupField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.zat.cz/OPCUAParameters")]
    public partial class OPCUAParametersTypeClientGroup {
        
        private string nameField;
        
        private ushort periodField;
        
        private client_service serviceField;
        
        private OPCUAParametersTypeClientGroupVar[] varField;
        
        /// <remarks/>
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public ushort period {
            get {
                return this.periodField;
            }
            set {
                this.periodField = value;
            }
        }
        
        /// <remarks/>
        public client_service service {
            get {
                return this.serviceField;
            }
            set {
                this.serviceField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("var")]
        public OPCUAParametersTypeClientGroupVar[] var {
            get {
                return this.varField;
            }
            set {
                this.varField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.zat.cz/OPCUAParameters")]
    public enum client_service {
        
        /// <remarks/>
        Unknown,
        
        /// <remarks/>
        Read,
        
        /// <remarks/>
        Write,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.zat.cz/OPCUAParameters")]
    public partial class OPCUAParametersTypeClientGroupVar {
        
        private string nameField;
        
        private string idField;
        
        private basic_type basic_typeField;
        
        /// <remarks/>
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public string id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public basic_type basic_type {
            get {
                return this.basic_typeField;
            }
            set {
                this.basic_typeField = value;
            }
        }
    }
}

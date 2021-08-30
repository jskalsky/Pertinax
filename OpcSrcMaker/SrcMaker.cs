using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcSrcMaker
{
    public class SrcMaker
    {
        private Dictionary<string, NodeId> Nids;
        private Dictionary<string, NodeId> MaxSubindex;

        public SrcMaker()
        {
            Nids = new Dictionary<string, NodeId>();
            MaxSubindex = new Dictionary<string, NodeId>();
        }

        private void GetVariables(DataSet cfg, int colNodeId, int colDescription, bool hasHeader)
        {
            foreach (DataTable dt in cfg.Tables)
            {
                int startRow = (hasHeader) ? 1 : 0;
                for (int row = startRow; row < dt.Rows.Count; ++row)
                {
                    if (dt.Rows[row][colNodeId] != null)
                    {
                        string description = (colDescription == -1 || dt.Rows[row][colDescription] == null) ? string.Empty : (string)dt.Rows[row][colDescription];
                        string nodeId = (string)dt.Rows[row][colNodeId];
                        if (Nids.ContainsKey(nodeId))
                        {
                            Debug.Print($"DataTable uz obsahuje {nodeId}");
                        }
                        else
                        {
                            NodeId ni = NodeId.Create(nodeId, description);
                            if (ni != null)
                            {
                                Nids[nodeId] = ni;
                            }
                            else
                            {
                                Debug.Print($"NodeId.Create chyba {dt.TableName}, {row} {nodeId}");
                            }
                        }
                    }
                }
            }
        }

        private void GetMaxSubindex()
        {
            foreach(KeyValuePair<string,NodeId> pair in Nids)
            {
                if(MaxSubindex.TryGetValue(pair.Value.Name, out NodeId max))
                {
                    if(pair.Value.Subindex > max.Subindex)
                    {
                        MaxSubindex[pair.Value.Name] = pair.Value;
                    }
                }
                else
                {
                    MaxSubindex[pair.Value.Name] = pair.Value;
                }
            }
        }

        private void AddTab(StreamWriter sw, int nrSpaces)
        {
            StringBuilder sb = new StringBuilder();
            for(int i=0;i<nrSpaces;++i)
            {
                sb.Append(' ');
            }
            sw.Write(sb.ToString());
        }

        private void AddString(StreamWriter sw, int nrSpaces, string text, bool addNewLine = true)
        {
            AddTab(sw, nrSpaces);
            if(addNewLine)
            {
                sw.WriteLine(text);
            }
            else
            {
                sw.Write(text);
            }
        }

        private void AddConstInt(StreamWriter sw, int nrSpaces, string name, int val)
        {
            AddTab(sw, nrSpaces);
            sw.WriteLine($"static const int {name} = {val};");
        }
        private void AddPointer(StreamWriter sw, int nrSpaces, string name, string valType)
        {
            AddTab(sw, nrSpaces);
            sw.WriteLine($"{valType} *{name};");
        }

        private void AddSetFunction(StreamWriter sw, int nrSpaces, string className, string name, string valType)
        {
            sw.WriteLine($"void {className}::Set{name}(int index, {valType} val)");
            sw.WriteLine("{");
            AddString(sw, nrSpaces * 2, $"{name}[index] = val;");
            sw.WriteLine("}");
            sw.WriteLine();
        }

        private void AddTestFunction(StreamWriter sw, int nrSpaces, string className)
        {
            sw.WriteLine($"void {className}::Test()");
            sw.WriteLine("{");

            int floatValue = 1;
            bool booleanValue = false;
            foreach(NodeId nodeId in Nids.Values)
            {
                if(nodeId.ValueType == 15)
                {
                    string bv = (booleanValue) ? "true" : "false";
                    AddString(sw, nrSpaces * 2, $"{nodeId.Name}[{nodeId.Subindex}] = {bv};");
                    booleanValue = !booleanValue;
                }
                else
                {
                    AddString(sw, nrSpaces * 2, $"{nodeId.Name}[{nodeId.Subindex}] = {floatValue};");
                    ++floatValue;
                }
            }
            sw.WriteLine("}");
            sw.WriteLine();
        }

        private void AddOpenFunction(StreamWriter sw, int nrSpaces, string className)
        {
            sw.WriteLine($"UA_StatusCode {className}::Open()");
            sw.WriteLine("{");
            AddString(sw, nrSpaces, $"UA_StatusCode sc = ServerOpc::Open();");
            AddString(sw, nrSpaces, $"if(sc != UA_STATUSCODE_GOOD)");
            AddString(sw, nrSpaces, "{");
            AddString(sw, nrSpaces * 2, "return sc;");
            AddString(sw, nrSpaces, "}");

            foreach (KeyValuePair<string, NodeId> pair in MaxSubindex)
            {
                int valType = (pair.Value.ValueType == 15) ? 0 : 9;
                AddString(sw, nrSpaces, $"sc = AddArrayVariable(\"{pair.Key}\", {valType}, {pair.Key}, {pair.Key}_Length, &{pair.Key}NodeId);");
                AddString(sw, nrSpaces, $"if(sc != UA_STATUSCODE_GOOD)");
                AddString(sw, nrSpaces, "{");
                AddString(sw, nrSpaces * 2, "return sc;");
                AddString(sw, nrSpaces, "}");
            }

            AddString(sw, nrSpaces, $"return UA_Server_run_startup(Server);");
            sw.WriteLine("}");
            sw.WriteLine();
        }

        private void AddIterateFunction(StreamWriter sw, int nrSpaces, string className)
        {
            sw.WriteLine($"unsigned short int {className}::Iterate()");
            sw.WriteLine("{");
            AddString(sw, nrSpaces, $"return UA_Server_run_iterate(Server, false);");
            sw.WriteLine("}");
            sw.WriteLine();
        }
        private void MakeHeader(string fileName, int nrSpaces)
        {
            using(StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine($"#ifndef {Path.GetFileNameWithoutExtension(fileName).ToUpper()}_H_");
                sw.WriteLine($"#define {Path.GetFileNameWithoutExtension(fileName).ToUpper()}_H_");
                sw.WriteLine();
                sw.WriteLine($"#include \"open62541.h\"");
                sw.WriteLine($"#include \"ServerOpc.h\"");
                sw.WriteLine();

                sw.WriteLine($"class {Path.GetFileNameWithoutExtension(fileName)} : public ServerOpc");
                sw.WriteLine("{");

                sw.WriteLine("public:");
                AddString(sw, nrSpaces, Path.GetFileNameWithoutExtension(fileName) + "(unsigned short port);");
                AddString(sw, nrSpaces, "virtual " + '~' + Path.GetFileNameWithoutExtension(fileName) + "();");
                foreach (KeyValuePair<string, NodeId> pair in MaxSubindex)
                {
                    string valType = (pair.Value.ValueType == 15) ? "bool" : "float";
                    AddString(sw, nrSpaces, $"void Set{pair.Key}(int index, {valType} val);");
                }
                sw.WriteLine();
                AddString(sw, nrSpaces, $"virtual UA_StatusCode Open();");
//                AddString(sw, nrSpaces, $"virtual void Close();");
                AddString(sw, nrSpaces, $"void Test();");
                AddString(sw, nrSpaces, $"unsigned short int Iterate();");

                sw.WriteLine("private:");
                foreach (KeyValuePair<string, NodeId> pair in MaxSubindex)
                {
                    AddConstInt(sw, nrSpaces, $"{pair.Key}_Length", pair.Value.Subindex + 1);
                }
                sw.WriteLine();
                foreach (KeyValuePair<string,NodeId> pair in MaxSubindex)
                {
                    string valType = (pair.Value.ValueType == 15) ? "bool" : "float";
                    AddPointer(sw, nrSpaces, pair.Key, valType);
                }
                foreach (KeyValuePair<string, NodeId> pair in MaxSubindex)
                {
                    AddString(sw, nrSpaces, $"UA_NodeId {pair.Key}NodeId;");
                }

                sw.WriteLine("};");
                sw.WriteLine();
                sw.WriteLine("#endif");
            }
        }

        private void MakeCpp(string fileName, int nrSpaces)
        {
            string name = Path.GetFileNameWithoutExtension(fileName);
            using(StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine($"#include \"{name}.h\"");
                sw.WriteLine();

                foreach (KeyValuePair<string, NodeId> pair in MaxSubindex)
                {
                    sw.WriteLine($"const int {name}::{pair.Key}_Length;");
                }
                sw.WriteLine();

                sw.WriteLine($"{name}::{name}(unsigned short port) : ServerOpc(port)");
                sw.WriteLine("{");
                foreach (KeyValuePair<string, NodeId> pair in MaxSubindex)
                {
                    string valType = (pair.Value.ValueType == 15) ? "bool" : "float";
                    AddString(sw, nrSpaces, $"{pair.Key} = new {valType}[{pair.Key}_Length];");
                }
                sw.WriteLine("}");
                sw.WriteLine();
                sw.WriteLine($"{name}::~{name}()");
                sw.WriteLine("{");
                foreach (KeyValuePair<string, NodeId> pair in MaxSubindex)
                {
                    AddString(sw, nrSpaces, $"delete[] {pair.Key};");
                }
                sw.WriteLine("}");
                sw.WriteLine();
                foreach (KeyValuePair<string, NodeId> pair in MaxSubindex)
                {
                    string valType = (pair.Value.ValueType == 15) ? "bool" : "float";
                    AddSetFunction(sw, nrSpaces, name, pair.Key, valType);
                }
                AddOpenFunction(sw, nrSpaces, name);
                AddTestFunction(sw, nrSpaces, name);
                AddIterateFunction(sw, nrSpaces, name);
            }
        }
        public void Make(DataSet cfg, int colNodeId, int colDescription = -1, bool hasHeader = true, string className = "OpcUaZ1xx", int tab = 4)
        {
            GetVariables(cfg, colNodeId, colDescription, hasHeader);
            GetMaxSubindex();

            string dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "Pertinax";
            string headerFileName = dir + Path.DirectorySeparatorChar + className + ".h";
            string cppFileName = dir + Path.DirectorySeparatorChar + className + ".cpp";

            MakeHeader(headerFileName, tab);
            MakeCpp(cppFileName, tab);
        }
    }
}

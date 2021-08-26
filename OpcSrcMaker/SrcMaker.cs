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
            AddString(sw, nrSpaces, $"void {className}::Set{name}(int index, {valType} val);");
            AddString(sw, nrSpaces, "{");
            AddString(sw, nrSpaces * 2, $"{name}[index] = val;");
            AddString(sw, nrSpaces, "}");
            sw.WriteLine();
        }

        private void AddTestFunction(StreamWriter sw, int nrSpaces, string className)
        {
            AddString(sw, nrSpaces, $"void {className}::Test(float startValue);");
            AddString(sw, nrSpaces, "{");

            AddString(sw, nrSpaces * 2, $"float vf = startValue;");
            AddString(sw, nrSpaces * 2, $"bool vb = false;");
            foreach(NodeId nodeId in Nids.Values)
            {
                if(nodeId.ValueType == 15)
                {
                    AddString(sw, nrSpaces * 2, $"{nodeId.Name}[{nodeId.Subindex}] = vb~;");
                }
                else
                {
                    AddString(sw, nrSpaces * 2, $"{nodeId.Name}[{nodeId.Subindex}] = vf++;");
                }
            }
            AddString(sw, nrSpaces, "}");
            sw.WriteLine();
        }
        private void MakeHeader(string fileName, int nrSpaces)
        {
            using(StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine($"#ifndef {Path.GetFileNameWithoutExtension(fileName).ToUpper()}_H_");
                sw.WriteLine($"#define {Path.GetFileNameWithoutExtension(fileName).ToUpper()}_H_");
                sw.WriteLine();

                sw.WriteLine($"class {Path.GetFileNameWithoutExtension(fileName)}");
                sw.WriteLine("{");

                sw.WriteLine("public:");
                AddString(sw, nrSpaces, Path.GetFileNameWithoutExtension(fileName) + "();");
                AddString(sw, nrSpaces, "virtual " + '~' + Path.GetFileNameWithoutExtension(fileName) + "();");
                foreach (KeyValuePair<string, NodeId> pair in MaxSubindex)
                {
                    string valType = (pair.Value.ValueType == 15) ? "bool" : "float";
                    AddString(sw, nrSpaces, $"Set{pair.Key}(int index, {valType} val);");
                }

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
                    string valType = (pair.Value.ValueType == 15) ? "bool" : "float";
                    sw.WriteLine($"const {valType} {name}::{pair.Key}_Length;");
                }
                sw.WriteLine();

                sw.WriteLine($"{name}::{name}()");
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

                AddTestFunction(sw, nrSpaces, name);
            }
        }
        public void Make(DataSet cfg, int colNodeId, int colDescription = -1, bool hasHeader = true, string className = "OpcUa", int tab = 4)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcSrcMaker
{
    public class NodeId
    {
        protected NodeId(string name, int valueType, int subindex, string description)
        {
            Name = name;
            ValueType = valueType;
            Subindex = subindex;
            Description = description;
        }
        public static NodeId Create(string name, string description)
        {
            int state = 0;
            StringBuilder sbA = new StringBuilder();
            StringBuilder sbT = new StringBuilder();
            StringBuilder sbSub = new StringBuilder();
            for (int i = 0; i < name.Length; ++i)
            {
                switch(state)
                {
                    case 0:
                        if(name[i] != 'A')
                        {
                            return null;
                        }
                        state = 1;
                        break;
                    case 1:
                        if(name[i] >= '0' && name[i] <= '9')
                        {
                            sbA.Append(name[i]);
                        }
                        else
                        {
                            if(name[i] == 'T')
                            {
                                state = 2;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        break;
                    case 2:
                        if (name[i] >= '0' && name[i] <= '9')
                        {
                            sbT.Append(name[i]);
                        }
                        else
                        {
                            if (name[i] == 'S')
                            {
                                state = 3;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        break;
                    case 3:
                        if (name[i] >= '0' && name[i] <= '9')
                        {
                            sbSub.Append(name[i]);
                        }
                        else
                        {
                            return null;
                        }
                        break;
                }
            }
            if(sbA.Length == 0 || sbT.Length == 0 || sbSub.Length == 0)
            {
                return null;
            }
            return new NodeId('A' + sbA.ToString() + 'T' + sbT, int.Parse(sbT.ToString()), int.Parse(sbSub.ToString()), description);
        }
        public string Name { get; }
        public int ValueType { get; }
        public int Subindex { get; }
        public string Description { get; }
    }
}

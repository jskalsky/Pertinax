using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControlLibrary.Model;

namespace WpfControlLibrary.ViewModel
{
    public class ViewModelFlags : ViewModelBase
    {
        public ViewModelFlags()
        {

        }
        public ObservableCollection<VmFlagNode> Flags { get; }
        private ushort GetArrayIndex(string text)
        {
            int i = text.LastIndexOf('.');
            if (i < 0)
            {
                return 0;
            }

            string index = text.Substring(i + 1);
            if (ushort.TryParse(index, out ushort result))
            {
                return result;
            }

            return 0;
        }

        private void LoadFlagNode(ModFlagNode mfNode, VmFlagNode parent)
        {
            VmFlagNode vmFN = null;
            if (mfNode is  ModFlagNode modFlagNode)
            {
                vmFN = new VmFlagNode(modFlagNode.Name, false);
                parent.Add(vmFN);
                parent.IsExpanded = true;
            }
            else
            {
                if(mfNode is ModFlagNodeFlag modFlag)
                {
                    vmFN = new VmFlagNodeFlag(modFlag.Name, false);
                    parent.Add(vmFN);
                    parent.IsExpanded = true;
                    return;
                }
            }
            if(vmFN != null)
            {
                foreach(ModFlagNode mfn in mfNode.SubNodes)
                {
                    LoadFlagNode(mfn,vmFN);
                }
            }
        }
        public void Load()
        {
            foreach(ModFlagNode mfn in Model.ModFlagsTree.FlagsTree)
            {
                VmFlagNode vmFlagNode = new VmFlagNode(mfn.Name, false);
                Flags.Add(vmFlagNode);
                foreach(ModFlagNode mfn2 in mfn.SubNodes)
                {
                    LoadFlagNode(mfn2,vmFlagNode);
                }
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public class VmNodeSimpleVariable : VmNodeVariable
    {
        public VmNodeSimpleVariable(string name, Model.ModNodeId nodeId, string type, string access, bool isExpanded = false, bool isEditable = false) :
            base(name, nodeId, type, access, isExpanded, isEditable)
        {
            ImagePath = "pack://application:,,,/WpfControlLibrary;component/Icons/Constant_495.png";
        }
    }
}

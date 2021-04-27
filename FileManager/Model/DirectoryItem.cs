﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Model
{
    public abstract class DirectoryItem
    {
        public DirectoryItem(string name)
        {
            Name = name;
        }
        public string Name { get; }
    }
}

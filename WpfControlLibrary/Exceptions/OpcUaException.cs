using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.Exceptions
{
    public class OpcUaException : ApplicationException
    {
        public OpcUaException(string message) : base(message)
        {

        }
    }
}

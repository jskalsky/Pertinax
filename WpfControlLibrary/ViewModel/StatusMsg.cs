using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary.ViewModel
{
    public class StatusMsg
    {
        public static string[] _messageTypes = new string[] { "Error", "Warning", "Info" };
        public StatusMsg(string messageType, string message, object tag = null)
        {
            MessageType = messageType;
            Message = message;
            Tag = tag;
        }

        public string MessageType { get; }
        public string Message { get; }
        public object Tag { get; }
    }
}

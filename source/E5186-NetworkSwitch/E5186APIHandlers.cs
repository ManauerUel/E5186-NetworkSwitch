using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E5186_NetworkSwitch
{
    public delegate void MessageHandler(String message, MessageType type);
    public enum MessageType
    {
        MESSAGE, ERROR
    }

    
    public delegate void ConnectionHandler(ConnectionType type);
    public enum ConnectionType
    {
        CONNECTED, DISCONNECTED
    }
}

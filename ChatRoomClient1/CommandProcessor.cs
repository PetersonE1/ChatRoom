using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ChatRoomClient
{
    public static class CommandProcessor
    {
        public static void ProcessCommand(Dispatcher dispatcher, Message message)
        {
            try
            {
                switch (message.Id)
                {
                    case "PRINT": PrintDebug(message.Body, dispatcher); break;
                    case "REMOVE": RemoveMessage(message.Body, dispatcher); break;
                    default: break;
                }
            }
            catch (IndexOutOfRangeException)
            {
                Debug.WriteLine("Missing arguments from server");
                return;
            }
        }

        private static void PrintDebug(string message, Dispatcher dispatcher)
        {
            dispatcher.Invoke(() => MainWindow._Instance.LogCommand(message));
        }

        private static void RemoveMessage(string messageID, Dispatcher dispatcher)
        {
            dispatcher.Invoke(() => MainWindow._Instance.ClearMessageInternal(messageID));
        }
    }
}

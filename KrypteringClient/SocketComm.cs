using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace KrypteringClient
{
    internal class SocketComm
    {
        public static string RecvMsg(NetworkStream tcpStream)
        {
            byte[] bMsg = new byte[256];
            int bReadSize = tcpStream.Read(bMsg, 0, bMsg.Length);
            string msg = "";
            for (int i = 0; i < bReadSize; i++)
                msg += Convert.ToChar(bMsg[i]);

            return msg;
        }

        public static void SendMsg(NetworkStream tcpStream, string message)
        {
            byte[] bMessage = Encoding.UTF8.GetBytes(message);
            tcpStream.Write(bMessage, 0, bMessage.Length);

        }
        // used for acuiring other user information, chat room information or chat logs
        public static List<string> RecvListOfStrings(NetworkStream tcpStream)
        {
            List<string> Content = new List<string>();
            string newContent = RecvMsg(tcpStream);
            while (newContent != "end")
            {
                Content.Add(newContent);
                newContent = RecvMsg(tcpStream);
            }
            return Content;
        } 
    }
}

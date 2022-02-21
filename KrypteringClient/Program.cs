using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace KrypteringClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string address = "127.0.0.1";
            int port = 8001;
            TcpClient tcpClient;
            NetworkStream tcpStream;

            Console.WriteLine("Connecting to server...");
            tcpClient = new TcpClient();
            tcpClient.Connect(address, port);
            Console.WriteLine("Connected to server!");
            tcpStream = tcpClient.GetStream();

            bool connected = true;
            while (connected)
            {
                Console.WriteLine("--------------------------------------------------------------");
                Console.WriteLine("please enter the number of what action you wish to take:");
                Console.WriteLine("1: create a new account");
                Console.WriteLine("2: login with existing account");
                Console.WriteLine("3: exit the application");
                Console.WriteLine("--------------------------------------------------------------");
                int chosenAction = ParseInt();
                switch (chosenAction)
                {
                    case 1:
                        SocketComm.SendMsg(tcpStream, "create account");
                        break;

                    case 2:
                        SocketComm.SendMsg(tcpStream, "login");
                        break;

                    case 3:
                        SocketComm.SendMsg(tcpStream, "exit");
                        connected = false;
                        break;

                    default:
                        Console.WriteLine("ERROR! invalid option, please try again");
                        break;
                }

            }

        }

        public static void login(TcpClient tcpClient, NetworkStream tcpStream)
        {

        }
        public static int ParseInt()
        {
            
            while (true)
            { 
                try 
                { 
                    int input = int.Parse(Console.ReadLine());
                    return input;
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR! " + e.Message);
                }
            }

        }
    }
}

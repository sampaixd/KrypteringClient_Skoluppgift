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

            Console.WriteLine("Connecting to server...");
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(address, port);
            Console.WriteLine("Connected to server!");
            NetworkStream tcpStream = tcpClient.GetStream();

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
                        SocketComm.SendMsg(tcpStream, "quit");
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
            bool loggingIn = true;
            bool insertingPassword = false;
            while (loggingIn)
            { 
                Console.WriteLine("Please enter your username, or 'back' if you wish to go back to main menu");
                string username = Console.ReadLine();
                SocketComm.SendMsg(tcpStream, username);
                if (username == "back")
                    loggingIn = false;
                else
                {
                    bool foundUser = ServerGetTrueOrFalseResponse(tcpStream);
                    if (foundUser)
                    {
                        loggingIn = false;
                        insertingPassword = true;
                    }
                    else
                        Console.WriteLine("Could not find user, please try again");
                }
            }
            int passwordAttempts = 0;
            while (insertingPassword)
            {
                Console.WriteLine("PLease enter your password");
                string password = Console.ReadLine();
                SocketComm.SendMsg(tcpStream, password);
                bool correctPassword = ServerGetTrueOrFalseResponse(tcpStream);
                if (correctPassword)
                { }//TODO here
                else
                {
                    Console.WriteLine("Incorrect password");
                    passwordAttempts++;
                    if (passwordAttempts > 2)
                    { 
                        insertingPassword = false;
                        Console.WriteLine("failed inserting the correct password 3 times, returning to main menu...");
                    }
                }
            }
        }

        public static bool ServerGetTrueOrFalseResponse(NetworkStream tcpStream)
        {
            string serverResponse = SocketComm.RecvMsg(tcpStream);
            if (serverResponse == "accepted")
                return true;
            else
                return false;
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

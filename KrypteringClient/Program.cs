using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

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
                        CreateNewUser(tcpStream);
                        break;

                    case 2:
                        SocketComm.SendMsg(tcpStream, "login");
                        Login(tcpClient, tcpStream);
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

        static void CreateNewUser(NetworkStream tcpStream)
        {
            bool creatingUser = true;
            while(creatingUser)
            { 
                Console.WriteLine("Please enter your desired username or 'back' if you wish to go back to main menu");
                string username = Console.ReadLine();
                if (username == "back")
                {
                    SocketComm.SendMsg(tcpStream, "exit");
                    return;
                }
                SocketComm.SendMsg(tcpStream, username);
                bool nameIsAvalible = ServerGetTrueOrFalseResponse(tcpStream);
                if (nameIsAvalible)
                {
                    Console.WriteLine("Please enter your password");
                    string password = Console.ReadLine();
                    SocketComm.SendMsg(tcpStream, password);
                    Console.WriteLine();
                }
                else
                    Console.WriteLine("Name was taken, please enter a different name");
            }
        }

        static void Login(TcpClient tcpClient, NetworkStream tcpStream)
        {
            bool loggingIn = true;
            bool insertingPassword = false;
            string username  = "";
            while (loggingIn)
            { 
                Console.WriteLine("Please enter your username, or 'back' if you wish to go back to main menu");
                username = Console.ReadLine();
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
                {
                    insertingPassword = false;
                    LoggedInMenu(tcpClient, tcpStream, username);
                }
                else
                {
                    Console.WriteLine("Incorrect password");
                    passwordAttempts++;
                    if (passwordAttempts >= 3)
                    { 
                        insertingPassword = false;
                        Console.WriteLine("failed inserting the correct password 3 times, returning to main menu...");
                    }
                }
            }
        }

        static void LoggedInMenu(TcpClient tcpClient, NetworkStream tcpStream, string username)
        {
            Console.Clear();
            List<string> userInfo = SocketComm.RecvListOfStrings(tcpStream);
            List<string> chatRooms = SocketComm.RecvListOfStrings(tcpStream);
            bool loggedIn = true;
            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine($"Welcome {username}! please enter the action you wish to take");
            Console.WriteLine("1. View a list of all users");
            Console.WriteLine("2. View chatrooms and select a chat room to join");
            Console.WriteLine("3. Refresh list of users and chatrooms");
            Console.WriteLine("4. Logout");
            Console.WriteLine("--------------------------------------------------------------");
            while (loggedIn)
            {
                int selectedOption = ParseInt();
                switch(selectedOption)
                {
                    case 1:
                        ViewAllUserStatus(userInfo);
                        break;

                    case 2:
                        ViewAllChatRooms(chatRooms);
                        SelectChatRoom(tcpClient, tcpStream, chatRooms);
                        break;

                    case 3:
                        userInfo = SocketComm.RecvListOfStrings(tcpStream);
                        chatRooms = SocketComm.RecvListOfStrings(tcpStream);
                        break;

                    case 4:
                        SocketComm.SendMsg(tcpStream, "logout");
                        loggedIn = false;
                        break;

                    default:
                        Console.WriteLine("Invalid command, please try again");
                        break;
                }
            }
        }

        static void ViewAllUserStatus(List<string> userInfo)
        {
            foreach (string user in userInfo)
            {
                string username = user.Substring(0, user.IndexOf("|"));
                string onlinestatus = user.Substring(user.IndexOf("|") + 1, user.Length - user.IndexOf("|") - 1);
                if (onlinestatus == "False")
                    onlinestatus = "offline";
                else
                    onlinestatus = "online";
                Console.WriteLine($"{username} - {onlinestatus}");
            }
        }

        static void ViewAllChatRooms(List<string> chatRooms)
        {
            Console.WriteLine("0 - create a new chat room");
            foreach (string chatRoom in chatRooms)
            {
                string roomId = chatRoom.Substring(0, chatRoom.IndexOf("|"));
                string connectedUsers = chatRoom.Substring(chatRoom.IndexOf("|") + 1, chatRoom.Length - chatRoom.IndexOf("|") - 1);
                Console.WriteLine($"Chat id: {roomId + 1} - connected users: {connectedUsers}");
            }
        }

        static void SelectChatRoom(TcpClient tcpClient, NetworkStream tcpStream, List<string> chatRooms)
        {
            Console.WriteLine("Please enter the number of the chat ID that you wish to join, or enter -1 if you wish to go back");
            while (true)
            { 
                int selectedChatRoom = ParseInt() - 1;
                if (selectedChatRoom == -2)
                    return;
                else if (selectedChatRoom == -1)
                {
                    SocketComm.SendMsg(tcpStream, Convert.ToString(selectedChatRoom));
                    Console.WriteLine("new chat room created!");
                    chatRooms.Add($"{chatRooms.Count}|{0}");
                }
                SocketComm.SendMsg(tcpStream, Convert.ToString(selectedChatRoom));

                bool foundChatRoom = ServerGetTrueOrFalseResponse(tcpStream);
                if (foundChatRoom)
                    ConnectedToChatRoom();
                else
                    Console.WriteLine("Could not find chat room, please try again");
            }
        }

        static void ConnectedToChatRoom()   // TODO
        {
            Console.WriteLine("Connecting...");
            Console.WriteLine("Loading chat logs...");
        }

        static void ListenForIncomingChatMessages(NetworkStream tcpStream)
        {
            string newMessage = SocketComm.RecvMsg(tcpStream);

        }

        

        static bool ServerGetTrueOrFalseResponse(NetworkStream tcpStream)
        {
            string serverResponse = SocketComm.RecvMsg(tcpStream);
            if (serverResponse == "accepted")
                return true;
            else
                return false;
        }
        static int ParseInt()
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

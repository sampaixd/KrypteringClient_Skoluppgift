using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace KrypteringClient
{
    internal class Program
    {
        static bool loadingMessage = false; // best solution I could find for not being able to change ownMessage list while looping through it
        static void Main(string[] args)
        {
            string address = "127.0.0.1";
            int port = 8001;
            bool connected = true;
            Console.WriteLine("Connecting to server...");
            TcpClient tcpClient = new TcpClient();
            try
            {
                tcpClient = ConnectToServer(tcpClient, address, port);
            }
            catch (ServerUnavalibleException)
            {
                Console.WriteLine("Server is currently unavalible, please try again at a later date");
                connected = false;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                connected = false;
            }
            Console.WriteLine("Connected to server!");
            NetworkStream tcpStream = tcpClient.GetStream();

            
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
                        Login(tcpStream);
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

        static TcpClient ConnectToServer(TcpClient tcpClient, string address, int port)
        {
            int attempts = 0;
            while (attempts < 10)
            {
                try
                {
                    tcpClient.Connect(address, port);
                    return tcpClient;
                }
                catch (Exception)
                {
                    Console.WriteLine("Could not connect to server, trying again in 10 seconds...");
                    Thread.Sleep(10000);
                }
            }
            throw new ServerUnavalibleException();
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
                try
                {
                    bool nameIsAvalible = ServerGetTrueOrFalseResponse(tcpStream);
                    if (nameIsAvalible)
                    {
                        Console.WriteLine("Please enter your password");
                        string password = Console.ReadLine();
                        SocketComm.SendMsg(tcpStream, password);
                        Console.WriteLine("account created!");
                        return;
                    }
                    else
                        Console.WriteLine("Name was taken, please enter a different name");
                }
                catch(InvalidServerResponseException)
                {
                    Console.WriteLine("ERROR! Invalid response from the server");
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        static void Login(NetworkStream tcpStream)
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
                    try
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
                    catch (InvalidServerResponseException)
                    {
                        Console.WriteLine("ERROR! Invalid response from the server");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            int passwordAttempts = 0;
            while (insertingPassword)
            {
                Console.WriteLine("Please enter your password");
                string password = Console.ReadLine();
                SocketComm.SendMsg(tcpStream, password);
                try
                { 
                    bool correctPassword = ServerGetTrueOrFalseResponse(tcpStream);
                    if (correctPassword)
                    {
                        insertingPassword = false;
                        LoggedInMenu(tcpStream, username);
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
                catch (InvalidServerResponseException)
                {
                    Console.WriteLine("ERROR! Invalid response from the server");
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        static void LoggedInMenu(NetworkStream tcpStream, string username)
        {
            Console.Clear();
            List<string> userInfo = SocketComm.RecvListOfStrings(tcpStream);
            List<string> chatRooms = SocketComm.RecvListOfStrings(tcpStream);
            for (int i = 0; i < chatRooms.Count; i++)
                ChatLogManager.AddNewChatLog();
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
                        SelectChatRoom(tcpStream, chatRooms, username);
                        break;

                    case 3:
                        SocketComm.SendMsg(tcpStream, "refresh");
                        userInfo = SocketComm.RecvListOfStrings(tcpStream);
                        chatRooms = SocketComm.RecvListOfStrings(tcpStream);
                        Console.WriteLine("users and chatrooms updated!");
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
                Console.WriteLine($"Chat id: {Convert.ToInt32(roomId) + 1} - connected users: {connectedUsers}");
            }
        }

        static void SelectChatRoom(NetworkStream tcpStream, List<string> chatRooms, string name)
        {
            SocketComm.SendMsg(tcpStream, "chatroom");
            Console.WriteLine("Please enter the number of the chat ID that you wish to join, or enter -1 if you wish to go back");
            while (true)
            { 
                int selectedChatRoom = ParseInt() - 1;  // since option 0 is to create a new chat room, the chat rooms ID got pushed forward by 1, although they have
                if (selectedChatRoom == -2)             // a starting index of 0 in the database
                {
                    SocketComm.SendMsg(tcpStream, Convert.ToString(selectedChatRoom));
                    return;
                }
                else if (selectedChatRoom == -1)
                {
                    SocketComm.SendMsg(tcpStream, Convert.ToString(selectedChatRoom));
                    Console.WriteLine("new chat room created!");
                    chatRooms.Add($"{chatRooms.Count}|{0}");
                    ChatLogManager.AddNewChatLog();
                    continue;
                }
                SocketComm.SendMsg(tcpStream, Convert.ToString(selectedChatRoom));
                try
                {
                    bool foundChatRoom = ServerGetTrueOrFalseResponse(tcpStream);
                    if (foundChatRoom)
                    {
                        ConnectedToChatRoom(tcpStream, selectedChatRoom, name);
                        break;
                    }
                    else
                        Console.WriteLine("Could not find chat room, please try again");
                }
                catch (InvalidServerResponseException)
                {
                    Console.WriteLine("ERROR! Invalid response from the server");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        static void ConnectedToChatRoom(NetworkStream tcpStream, int chatRoomId, string name)   // TODO
        {
            Console.WriteLine("Connecting...");
            Console.WriteLine("Loading chat logs...");
            List<string> messages = SocketComm.RecvListOfStrings(tcpStream);
            ChatLogManager.AddMultipleNewMessages(chatRoomId, messages);
            List<string> chatLog = ChatLogManager.GetAllMessages(chatRoomId);
            foreach (string message in chatLog)
            {
                string sender = message.Substring(0, message.IndexOf("|"));
                string msgContent = message.Substring(message.IndexOf("|") + 1, message.Length - message.IndexOf("|") - 1);
                if (sender == name)
                    Console.WriteLine($"you: {msgContent}");
                else
                    Console.WriteLine($"{sender}: {msgContent}");
            }
            Console.WriteLine();
            Console.Write("Your message: ");
            bool connected = true;
            List<char> ownMessage = new List<char>();
            Thread thread = new Thread(() => ListenForIncomingChatMessages(tcpStream, ownMessage, chatRoomId));
            thread.Start();
            
            while (connected)
            {
                while (!loadingMessage)
                { 
                    ConsoleKeyInfo newChar = Console.ReadKey();
                    if (newChar.KeyChar == '\b' && ownMessage.Count > 0)
                    {
                        Console.Write(" \b");
                        ownMessage.RemoveAt(ownMessage.Count - 1);
                    }
                    else if (newChar.KeyChar == '\r' && ownMessage.Count > 0)
                    {
                        string message = "";
                        for (int i = ownMessage.Count - 1; i >= 0; i--)
                            message += ownMessage[i];   // AddNewMessage automatically reverses message, therefore we reverse the message beforehand
                        if (message == "evael/")
                        {
                            SocketComm.SendMsg(tcpStream, message);
                            connected = false;
                            loadingMessage = true;
                            Console.WriteLine("You have disconnected from the chat room");
                        }
                        else
                        { 
                            string formattedMsg = $"{ChatLogManager.ChatMessageCount(chatRoomId)}|{message}|{name}";
                            ChatLogManager.AddNewMessage(chatRoomId, formattedMsg);
                            SocketComm.SendMsg(tcpStream, formattedMsg);
                            WriteNewMessage($"you: {new string(ownMessage.ToArray())}");
                            ownMessage.Clear();
                            WriteOwnMessage(ownMessage);
                        }
                    }
                    else
                        ownMessage.Add(newChar.KeyChar);
                }
                loadingMessage = false;
            }
            thread.Abort();

        }

        static void ListenForIncomingChatMessages(NetworkStream tcpStream, List<char> ownMessage, int chatRoomId)
        {
            while(true)
            { 
                string newMessage = SocketComm.RecvMsg(tcpStream);
                ChatLogManager.AddNewMessage(chatRoomId, newMessage);
                string formattedMessage = ChatLogManager.GetLastMessage(chatRoomId);
                WriteNewMessage(formattedMessage);
                WriteOwnMessage(ownMessage);
            }
        }

        static void WriteNewMessage(string newMsg)
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine(newMsg);
            ClearCurrentConsoleLine();
        }

        static void WriteOwnMessage(List<char> ownMessage)
        {
            loadingMessage = true;
            Thread.Sleep(10);   // could sometimes allow items to be added to list without a small timeout
            string ownMessageString = new string(ownMessage.ToArray());
            Console.WriteLine();
            Console.Write($"Your message: {ownMessageString}");
            loadingMessage = false;
        }

        static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }


        static bool ServerGetTrueOrFalseResponse(NetworkStream tcpStream)
        {
            string serverResponse = SocketComm.RecvMsg(tcpStream);
            if (serverResponse == "accepted")
                return true;
            else if (serverResponse == "denied")
                return false;
            else
                throw new InvalidServerResponseException();
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

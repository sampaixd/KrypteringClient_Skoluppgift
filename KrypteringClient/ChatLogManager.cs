using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrypteringClient
{
    internal class ChatLogManager
    {
        static List<ChatLog> chatLogs;

        static ChatLogManager()
        {
            chatLogs = new List<ChatLog>();
        }
        static public int ChatLogCount { get { return chatLogs.Count; } }

        public static void AddNewChatLog()
        {
            chatLogs.Add(new ChatLog(chatLogs.Count));
        }


        public static void AddMultipleNewMessages(int chatId, List<string> newMessages)
        {
            chatLogs[chatId].AddMultipleNewMessages(newMessages);
        }

        public static void AddNewMessage(int chatId, string newMessage)
        {
            chatLogs[chatId].AddNewMessage(newMessage);
        }

        public static int ChatMessageCount(int chatId)
        {
            return chatLogs[chatId].ChatMessageCount;
        }

        public static List<string> GetAllMessages(int roomId)
        {
            return chatLogs[roomId].GetAllMessages();
        }
        public static string GetLastMessage(int roomId)
        {
            return chatLogs[roomId].GetLastMessage();
        }




    }
}

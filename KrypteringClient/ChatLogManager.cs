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


        public static void AddMultipleNewMessages(int ChatId, List<string> newMessages)
        {
            foreach (ChatLog chatLog in chatLogs)
            {
                if (chatLog.ChatId == ChatId)
                { 
                    chatLog.AddMultipleNewMessages(newMessages);
                    break;
                }
            }
        }

        public static void AddNewMessage(int chatId, string newMessage)
        {
            foreach (ChatLog chatLog in chatLogs)
            {
                if (chatLog.ChatId == chatId)
                {
                    chatLog.AddNewMessage(newMessage);
                    break;
                }
            }
        }

        public static int ChatMessageCount(int chatId)
        {
            foreach (ChatLog chatLog in chatLogs)
            {
                if (chatLog.ChatId == chatId)
                    return chatLog.ChatMessageCount;
            }
            return 0;
        }

        public List<string> GetAllMessages(int roomId)
        {
            foreach (ChatLog chatLog in chatLogs)
            {
                if (chatLog.ChatId == roomId)
                    return chatLog.GetAllMessages();
            }
            return null;
        }
        public string GetLastMessage(int roomId)
        {
            foreach (ChatLog chatLog in chatLogs)
            {
                if (chatLog.ChatId == roomId)
                    return chatLog.GetLastMessage();
            }
            return "";
        }




    }
}

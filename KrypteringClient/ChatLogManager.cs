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

        public static void AddNewChatLog(int chatLogId)
        {
            chatLogs.Add(new ChatLog(chatLogId));
        }

        public static void AddNewMessages(int findChatId, List<string> newMessages)
        {
            foreach (ChatLog chatLog in chatLogs)
            {
                if (chatLog.ChatId == findChatId)
                    chatLog.AddNewMessages(newMessages)
            }
        }

    }
}

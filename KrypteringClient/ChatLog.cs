using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrypteringClient
{
    internal class ChatLog
    {
        List<Message> chatMessages;
        int chatId;
        public ChatLog(int chatId)
        {
            this.chatId = chatId;
            this.chatMessages = new List<Message>();
        }

        public void AddNewMessages(List<string> newMessages)
        {
            foreach (string formattedMessage in newMessages)
            {
                char[] formattedMessageChar = formattedMessage.ToCharArray();
                
            }
        }

        public int ChatId { get { return chatId; } }
    }
}

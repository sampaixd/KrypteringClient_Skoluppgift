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
                int msgId = GetMsgId(formattedMessageChar);
                int msgIdLength = Convert.ToString(msgId).Length + 1;   // used for upcoming methods
                string msgContent = GetMsgContent(formattedMessageChar, msgIdLength);
                string msgSender = GetMsgSender(formattedMessageChar,msgIdLength + msgContent.Length + 1);
                chatMessages.Add(new Message(msgSender, msgContent, msgIdLength));
                chatMessages.Last().ReverseMsg();
            }
        }

        public int GetMsgId(char[] formattedMessageChar)
        {
            string msgIdString = "";
            int currentChar = 0;
            while (formattedMessageChar[currentChar] != '|')
                msgIdString += formattedMessageChar[currentChar++];
            return Convert.ToInt32(msgIdString);
        }

        public string GetMsgContent(char[] formattedMessageChar, int charStartPosition)
        {
            string msgContent = "";
            int currentChar = charStartPosition;
            while (formattedMessageChar[currentChar] != '|')
                msgContent += formattedMessageChar[currentChar++];
            return msgContent;
        }

        public string GetMsgSender(char[] formattedMessageChar, int charStartPosition)
        { 
            string msgSender = "";
            for(int i = charStartPosition; i < formattedMessageChar.Length; i++)
                msgSender += formattedMessageChar[i];
            return msgSender;
        }

        public int ChatId { get { return chatId; } }
    }
}

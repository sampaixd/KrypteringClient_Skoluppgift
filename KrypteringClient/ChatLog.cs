using System;
using System.Collections.Generic;
using System.Linq;

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

        public void AddMultipleNewMessages(List<string> newMessages)
        {
            foreach (string formattedMessage in newMessages)
            {
                AddNewMessage(formattedMessage);
            }
        }
        public void AddNewMessage(string newMessage)
        {
            char[] newMessageChar = newMessage.ToCharArray();
            int msgId = GetMsgId(newMessageChar);
            int msgIdLength = Convert.ToString(msgId).Length + 1;   // used for upcoming methods
            string msgContent = GetMsgContent(newMessageChar, msgIdLength);
            string msgSender = GetMsgSender(newMessageChar, msgIdLength + msgContent.Length + 1);
            chatMessages.Add(new Message(msgSender, msgContent, msgIdLength));
            chatMessages.Last().ReverseMsg();
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

        public List<string> GetAllMessages()
        {
            List<string> messages = new List<string>();
            foreach (Message msg in chatMessages)
                messages.Add($"{msg.SenderName}|{msg.MessageContent}");
            return messages;
        }

        public string GetLastMessage()
        {
            Message msg = chatMessages.Last();
            return $"{msg.SenderName}: {msg.MessageContent}";
        }


        public int ChatId { get { return chatId; } }

        public int ChatMessageCount { get{ return chatMessages.Count; } }
    }
}


namespace KrypteringClient
{
    internal class Message
    {
        int messageId;
        string messageContent;
        string senderName;

        public Message(string senderName, string messageContent, int messageId)
        {
            this.messageId = messageId;
            this.messageContent = messageContent;
            this.senderName = senderName;
        }
        public int MessageId { get { return messageId; } }
        public string MessageContent { get { return messageContent; } }
        public string SenderName { get { return senderName; } }

        public string ConvertInfoToString() // used for sending data via socket
        {
            return $"{messageId}|{messageContent}|{senderName}";
        }

        public void ReverseMsg()
        {
            char[] charMsg = messageContent.ToCharArray();
            messageContent = "";    // clears the string before giving it its reversed value
            for (int i = charMsg.Length - 1; i >= 0; i--)
                messageContent += charMsg[i];

        }
    }
}

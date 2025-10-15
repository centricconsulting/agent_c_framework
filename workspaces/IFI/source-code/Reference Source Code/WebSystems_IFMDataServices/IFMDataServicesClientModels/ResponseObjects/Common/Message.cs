namespace IFM.DataServices.API.ResponseObjects.Common
{
    [System.Serializable]
    public class Message
    {
        public string MessageGroupText { get; set; }
        public Enums.MessageType MessageType { get; set; }
        public Enums.MessageSeverityType MessageSeverityType { get; set; }
        public string MessageText { get; set; }
        public string MessageCode { get; set; }

        public Message() { }

        protected internal Message(string msg, Enums.MessageType msgType, Enums.MessageSeverityType msgSevType, string msgGroupText)
        {
            this.MessageText = msg;
            this.MessageType = msgType;
            this.MessageSeverityType = msgSevType;
            this.MessageGroupText = msgGroupText;
        }
    }
}
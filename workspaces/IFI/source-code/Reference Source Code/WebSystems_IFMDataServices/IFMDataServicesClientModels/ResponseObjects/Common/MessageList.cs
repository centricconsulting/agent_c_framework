using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace IFM.DataServices.API.ResponseObjects.Common
{
    [System.Serializable]
    public class MessagesList : List<Message>
    {
        public string GroupName { get; set; }
        public void CreateGeneralMessage(string msgText)
        {
            this.Add(new Message(msgText, Enums.MessageType.GeneralMessage, Enums.MessageSeverityType.None, ""));
        }

        public void CreateErrorMessage(string errorText)
        {
            this.Add(new Message(errorText, Enums.MessageType.ValidationMessage, Enums.MessageSeverityType.StandardError, ""));
        }

        public void CreateFullStopErrorMessage(string errorText)
        {
            this.Add(new Message(errorText, Enums.MessageType.ValidationMessage, Enums.MessageSeverityType.FullStopError, ""));
        }

        public void CreateGeneralWarning(string warningText)
        {
            this.Add(new Message(warningText, Enums.MessageType.GeneralMessage, Enums.MessageSeverityType.Warning, ""));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var m in this)
            {
                if (m.MessageType == Enums.MessageType.ValidationMessage)
                    sb.AppendLine("Error:" + m.MessageText);
                if (m.MessageType == Enums.MessageType.GeneralMessage)
                    sb.AppendLine("General Msg:" + m.MessageText);
            }
            return sb.ToString();
        }
    }
}
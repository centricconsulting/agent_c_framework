using IFM.DataServices.API.ResponseObjects.Common;
using System.Collections.Generic;

namespace IFM.DataServices.API.ResponseObjects.Payments
{
    [System.Serializable]
    public class BulkPaymentResult
    {
        public bool PaymentCompleted { get; set; }
        public string PaymentConfirmationNumber { get; set; }
        public string PolicyNumber { get; set; }
        public int PolicyId { get; set; }
        public int PolicyImageNum { get; set; }
        public string AccountBillNumber { get; set; }
        private MessagesList _messages { get; set; } = new MessagesList();
        public MessagesList Messages { get { return _messages; } }
        private MessagesList _detailedErrorMessages { get; set; } = new MessagesList();
        public MessagesList DetailedErrorMessages { get { return _detailedErrorMessages; } }
        public void SetMessages(MessagesList messages)
        {
            _messages = messages;
        }
        public void SetDetailedErrorMessages(MessagesList detailedErrorMessages)
        {
            _detailedErrorMessages = detailedErrorMessages;
        }
    }
}
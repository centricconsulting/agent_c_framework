using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IFM.DataServices.API.ResponseObjects.Common;

namespace IFM.DataServices.API.ResponseObjects.Payments
{
    [System.Serializable]
    public class PaymentServiceResult : ServiceResult
    {
        public bool PaymentCompleted { get; set; }
        public string PaymentConfirmationNumber { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IFM.DataServices.API;

namespace IFM.DataServices.API.RequestObjects.Payments.Echeck
{
    [System.Serializable]
    public class BankNameLookup : BaseRequest
    {
        public string RoutingNumber { get; set; }

        public BankNameLookup(string APIAddress) : base(APIAddress)
        {
            API_Path = "IFM/Payment/Echeck";
        }

        public ResponseObjects.Common.ServiceResult<string> GetBankNameFromRoutingNumber()
        {
            TestRequiredVariable(RoutingNumber, nameof(RoutingNumber));

            API_Endpoint = $"routingnumbers/{this.RoutingNumber}";
            return Get<ResponseObjects.Common.ServiceResult<string>>();
        }

        public ResponseObjects.Common.ServiceResult<string> GetBankNameFromRoutingNumber(string routingNumber)
        {
            this.RoutingNumber = routingNumber;
            return GetBankNameFromRoutingNumber();
        }
    }
}
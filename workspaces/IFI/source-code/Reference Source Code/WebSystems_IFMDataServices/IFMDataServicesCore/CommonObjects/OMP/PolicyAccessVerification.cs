using System;
using IFM.PrimitiveExtensions;
using static IFM.DataServicesCore.CommonObjects.Enums.Enums;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class PolicyAccessVerification
    {
        public PolicyInformationVerificationLevel VerificationTypeId { get; set; }
        public string PolicyNumber { get; set; }
        public string AccountNumber { get; set; }
        public int OnlinePaymentNumber { get; set; }
        public string Name { get; set; }
        public string Zip { get; set; }
        public string DLN { get; set; }
        public string DOB { get; set; }
        public string FEIN { get; set; }        
    }
}
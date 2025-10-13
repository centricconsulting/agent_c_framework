using System;
using static IFM.DataServicesCore.CommonObjects.Enums.Enums;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class ClaimReport
    {
        public string IpAddress { get; set; }
        public string PolicyNumber { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneExtension { get; set; }
        public string EmailAddress { get; set; }
        public ClaimLossType LossType { get; set; }
        public string LossDescription { get; set; }
        public bool InjuriesExist { get; set; }
        public DateTime LossDateTime { get; set; }
    }
}
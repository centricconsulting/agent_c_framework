using IFM.DataServicesCore.BusinessLogic;
using IFM.PrimitiveExtensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.CommonObjects.OnBase
{
    [System.Serializable]
    public class OnBaseClaimInformation
    {

        public string ClaimNumber { get; set; }
        public string[] ClaimantName { get; set; }
        public string[] PolicyHolderName { get; set; }
        public string PolicyNumber { get; set; }
        public string OfficeAccount { get; set; }
        public string ClaimAdjuster { get; set; }
        public string ClaimAdjusterUserName { get; set; }
        public string DateOfLoss { get; set; }

        public OnBaseClaimInformation()
        {
        }

        public OnBaseClaimInformation(string claimNumber)
        {
            this.ClaimNumber = claimNumber;
        }

    }
}
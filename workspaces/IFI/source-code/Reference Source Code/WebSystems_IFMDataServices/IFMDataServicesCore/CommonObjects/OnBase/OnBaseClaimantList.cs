using IFM.DataServicesCore.BusinessLogic;
using IFM.PrimitiveExtensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace IFM.DataServicesCore.CommonObjects.OnBase
{
    [System.Serializable]
    public class OnBaseClaimantInformation
    {
        public string ClaimantName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public OnBaseClaimantInformation()
        {
        }

    }
    [System.Serializable]
    public class OnBaseClaimantList
    {

        public List<OnBaseClaimantInformation> ClaimantList;
        public OnBaseClaimantList()
        {
        }

    }
}
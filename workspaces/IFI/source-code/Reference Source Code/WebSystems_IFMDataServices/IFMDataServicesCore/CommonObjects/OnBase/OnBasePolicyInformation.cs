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
    public class OnBasePolicyInformation
    {

        public string PolicyNumber { get; set; }
        public string PolicyType { get; set; }
        public string PolicyState { get; set; }
        public string[] PolicyholderName { get; set; }
        public string PolicyholderDBA { get; set; }
        public string AgencyCode { get; set; }
        public string AgencyName { get; set; }
        public string AgencyState { get; set; }
        public string AgencyDBA { get; set; }
        public string AgencyGroupCode { get; set; }
        public string AgencyLocationCode { get; set; }
        public string AgencyTerritory { get; set; }
        public string[] QuoteNumber { get; set; }
        public string OfficeAccount { get; set; }

        public OnBasePolicyInformation()
        {
        }

        public OnBasePolicyInformation(string policyNumber)
        {
            this.PolicyNumber = policyNumber;
        }

    }
}
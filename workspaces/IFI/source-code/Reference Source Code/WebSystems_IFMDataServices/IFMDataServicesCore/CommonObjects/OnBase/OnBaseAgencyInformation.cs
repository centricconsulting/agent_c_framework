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
    public class OnBaseAgencyInformation 
    {

        public int AgencyId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public string DBA { get; set; }
        public string GroupCode { get; set; }
        public string LocationCode { get; set; }
        public string CommercialLinesTerritory { get; set; }
        public string PersonalLinesTerritory { get; set; }

        public OnBaseAgencyInformation()
        {
        }

        public OnBaseAgencyInformation(string groupCode, string locationCode )
        {
            this.GroupCode = groupCode;
            this.LocationCode = locationCode;
        }

    }
}
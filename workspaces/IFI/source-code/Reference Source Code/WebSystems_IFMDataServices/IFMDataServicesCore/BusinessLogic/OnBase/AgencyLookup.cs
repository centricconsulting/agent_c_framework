using IFM.DataServicesCore.CommonObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using DCO = Diamond.Common.Objects;
using IDS = Insuresoft.DiamondServices;
using IFM.DataServicesCore.CommonObjects.OnBase;
using IFM.DataServicesCore.BusinessLogic.Diamond;

#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.BusinessLogic.OnBase
{
    public class AgencyLookup : ModelBase
    {
        public OnBaseAgencyInformation LoadAgency(int agencyId)
        {
            return GetAgencyInfo(agencyId, null);
        }

        public OnBaseAgencyInformation LoadAgency(string code)
        {
            return GetAgencyInfo(null, code);
        }

        public OnBaseAgencyInformation LoadAgency(int policyId, int policyImageNum)
        {
            var agencyId = AgencyInformation.GetAgencyIDByPolicyIdAndImageNum(policyId, policyImageNum);
            if (agencyId > 0)
            {
                return GetAgencyInfo(agencyId, null);
            }
            return null;
        }

        private OnBaseAgencyInformation GetAgencyInfo(int? agencyId, string agencyCode)
        {            

            using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetOnBaseAgencyInfo", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@agencyId", agencyId ?? 0);
                    cmd.Parameters.AddWithValue("@agencyCode", agencyCode ?? string.Empty);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            OnBaseAgencyInformation onBaseAgency = new OnBaseAgencyInformation();
                            onBaseAgency.AgencyId = reader.GetInt32(0);
                            onBaseAgency.Code = reader.GetString(1);
                            onBaseAgency.DBA = reader.GetString(2);
                            onBaseAgency.Name = reader.GetString(3);
                            onBaseAgency.State = reader.GetString(4);
                            onBaseAgency.CommercialLinesTerritory = reader.IsDBNull(5) ? string.Empty : ParseCommercialTerritory(reader.GetString(5).Trim());
                            onBaseAgency.PersonalLinesTerritory = reader.IsDBNull(6) ? string.Empty : ParsePersonalTerritory(reader.GetString(6).Trim());
                            onBaseAgency.GroupCode = (onBaseAgency.Code.Length >= 4) ? onBaseAgency.Code.Substring(0, 4) : string.Empty;
                            onBaseAgency.LocationCode = (onBaseAgency.Code.Length >= 9) ? onBaseAgency.Code.Substring(5, 4) : string.Empty;
                            return onBaseAgency;
                        }                        
                    }
                }
            }
            return null;
        }

        

        private string ParseCommercialTerritory(string commercialLinesTerritory)
        {
            string returnValue = "";
            commercialLinesTerritory = commercialLinesTerritory.ToUpper();
            int pos = commercialLinesTerritory.IndexOf("TERR");
            if (pos != -1 && commercialLinesTerritory.Length >= pos + 3)
            {
                returnValue = commercialLinesTerritory.Substring(pos + 5, 2);
            }
            return returnValue;
        }

        private string ParsePersonalTerritory(string personalLinesTerritory)
        {
            string returnValue = "";
            personalLinesTerritory = personalLinesTerritory.ToUpper();
            int pos = personalLinesTerritory.IndexOf("TERRITORY");
            if (pos != -1 && personalLinesTerritory.Length >= pos + 11)
            {
                returnValue = personalLinesTerritory.Substring(pos + 9, 2);
            }
            return returnValue;
        }

        //public String GetAgencyCode(int agencyId)
        //{
        //    string sql = @"                            
        //                  SELECT ag.[code] 
        //                  FROM [Diamond].[dbo].[Agency] AS ag with (NoLock)
        //                  WHERE ag.agency_id = @id";

        //    using (var conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings["connDiamond"]))
        //    {
        //        conn.Open();
        //        using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
        //        {
        //            cmd.Parameters.AddWithValue("@id", agencyId);

        //            using (var reader = cmd.ExecuteReader())
        //            {
        //                if (reader.HasRows)
        //                {
        //                    reader.Read();
        //                    return reader.GetString(0);
        //                }
        //            }
        //        }
        //    }
        //    return null;
        //}

    }
}
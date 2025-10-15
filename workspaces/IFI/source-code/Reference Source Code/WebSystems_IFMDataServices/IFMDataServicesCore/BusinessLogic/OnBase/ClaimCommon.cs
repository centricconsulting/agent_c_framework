using Diamond.Common.Objects.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFM.DataServicesCore.BusinessLogic.OnBase
{
    class ClaimCommon
    {
        internal static int GetClaimControlId(string claimNumber)
        {
            int claimControlId = 0;
         
            using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetClaimControlIdForClaimNumber", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@claimNumber", claimNumber ?? string.Empty);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            claimControlId = (int)reader[0];
                        }
                    }
                }
            }

            return claimControlId;
        }

        internal static IFM.DataServicesCore.CommonObjects.Diamond.BasicClaimInfo GetBasicClaimInfo(int claimControlId)
        {
            if(claimControlId > 0)
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetBasicClaimInfo", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@claimControlId", claimControlId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                return LoadBasicClaimInfo(reader);
                            }
                        }
                    }
                }
            }
            return null;
        }

        internal static IFM.DataServicesCore.CommonObjects.Diamond.BasicClaimInfo GetBasicClaimInfo(string claimNumber)
        {
            if (claimNumber.HasValue())
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetBasicClaimInfo", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@claimNumber", claimNumber ?? string.Empty);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                return LoadBasicClaimInfo(reader);
                            }
                        }
                    }
                }
            }
            return null;
        }

        private static IFM.DataServicesCore.CommonObjects.Diamond.BasicClaimInfo LoadBasicClaimInfo(System.Data.SqlClient.SqlDataReader reader)
        {
            if (reader.HasRows)
            {
                reader.Read();
                {
                    var basicClaimInfo = new IFM.DataServicesCore.CommonObjects.Diamond.BasicClaimInfo()
                    {
                        PolicyNumber = reader["policy"].TryToGetString(),
                        PolicyId = reader["policy_id"].TryToGetInt32(),
                        PolicyImageNum = reader["policyimage_num"].TryToGetInt32(),
                        ClaimNumber = reader["claim_number"].TryToGetString(),
                        ClaimControlId = reader["claimcontrol_id"].TryToGetInt32(),
                        LossDate = reader["loss_date"].TryToGetDateTime(),
                        ReportedDate = reader["reported_date"].TryToGetDateTime(),
                        RequestedAmount = reader["claim_amount_requested"].TryToGetDouble(),
                        ClaimTypeId = reader["claim_type_id"].TryToGetInt32(),
                        ClaimType = reader["claim_type"].TryToGetString(),
                        ClaimSeverityId = reader["claimseverity_id"].TryToGetInt32(),
                        ClaimSeverity = reader["claimseverity"].TryToGetString(),
                        ClaimCloseReasonId = reader["claimclosereason_id"].TryToGetInt32(),
                        ClaimCloseReason = reader["claimclosereason"].TryToGetString(),
                        ClaimCloseIssueTypeId = reader["claimcloseissuetype_id"].TryToGetInt32(),
                        ClaimCloseIssueType = reader["claimcloseissuetype"].TryToGetString(),
                        ClaimControlStatusId = reader["claimcontrolstatus_id"].TryToGetInt32(),
                        ClaimControlStatus = reader["claimcontrolstatus"].TryToGetString(),
                        Description = reader["dscr"].TryToGetString()
                    };
                    return basicClaimInfo;
                }
            }
            return null;
        }

        internal static IEnumerable<int> ClaimantNumbers(int claimControlId)
        {
            if (DiamondLogin.OnBaseLogin())
            {
                using (var DS = Insuresoft.DiamondServices.ClaimsService.LoadClaimantList())
                {
                    DS.RequestData.ClaimControlId = claimControlId;
                    DS.RequestData.VehicleNum = -1;
                    var response = DS.Invoke()?.DiamondResponse;
                    var claimantList = response?.ResponseData?.ClaimantList;
                    // claimant numbers are not always sequential so you can't just return a count or something like that
                    if (claimantList?.Any() ?? false)
                    {
                        return from c in claimantList select c.ClaimantNum;
                    }
                }
            }
            return new List<int>();
        }

    }
}

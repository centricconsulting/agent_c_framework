using IFM.DataServicesCore.CommonObjects.Diamond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.Utils.Drawing.Helpers.NativeMethods;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.BusinessLogic.Diamond.Policy
{
    public static class PolicyInfoLookup
    {
        public static DCO.Policy.QuickLookup GetPolicyIDAndImageNumByPolicyNumber(string policyNum)
        {
            var policy = new DCO.Policy.QuickLookup();
            if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {
                using (var DS = Insuresoft.DiamondServices.PolicyService.GetPolicyIdAndNumForPolicyNumber())
                {
                    DS.RequestData.PolicyNumber = policyNum;
                    var invoke = DS.Invoke();
                    var diamondResponse = invoke.DiamondResponse;
                    if (diamondResponse?.ResponseData?.Policies?.Count > 0)
                    {
                        policy = diamondResponse.ResponseData.Policies[0];
                    }
                }
            }

            return policy;
        }

        public static int GetVersionIdByPolicyNumber(string policyNum)
        {
            if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {
                using (var DS = Insuresoft.DiamondServices.PolicyService.GetVersionIdByPolicyNumber())
                {
                    DS.RequestData.PolicyNumber = policyNum;
                    var invoke = DS.Invoke();
                    var diamondResponse = invoke.DiamondResponse;
                    if (diamondResponse?.ResponseData?.VersionId > 0)
                    {
                        return diamondResponse.ResponseData.VersionId;
                    }
                }
            }

            return 0;
        }

        public static int GetVersionIdByPolicyId(int policyId)
        {
            if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {
                using (var DS = Insuresoft.DiamondServices.PolicyService.GetVersionIdByPolicyId())
                {
                    DS.RequestData.PolicyId = policyId;
                    var invoke = DS.Invoke();
                    var diamondResponse = invoke.DiamondResponse;
                    if (diamondResponse?.ResponseData?.VersionId > 0)
                    {
                        return diamondResponse.ResponseData.VersionId;
                    }
                }
            }

            return 0;
        }

        public static string GetPolicyNumberByPolicyId(int policyId)
        {
            if (policyId > 0)
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetPolicyNumberForPolicyId", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@PolicyId", policyId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    return reader["current_policy"].TryToGetString();
                                }
                            }
                        }
                    }
                }
            }
            return "";
        }

        public static List<string> GetPolicyholdersRelatedToPolicy(int policyId, int policyImageNum)
        {
            var Policyholders = new List<string>();

            if (policyId > 0 && policyImageNum > 0)
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetPolicyholdersRelatedToPolicy", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@PolicyId", policyId);
                        cmd.Parameters.AddWithValue("@PolicyImageNum", policyImageNum);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Policyholders.Add(reader["display_name"].TryToGetString());
                                }
                            }
                        }
                    }
                }
            }
            return Policyholders;
        }

        public static List<string> GetPolicyholdersDoingBusinessAsNames(int policyId, int policyImageNum)
        {
            var DoingBusinessAsNames = new List<string>();

            if (policyId > 0 && policyImageNum > 0)
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetPolicyholdersDoingBusinessAsNames", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@PolicyId", policyId);
                        cmd.Parameters.AddWithValue("@PolicyImageNum", policyImageNum);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    DoingBusinessAsNames.Add(reader["doing_business_as"].TryToGetString());
                                }
                            }
                        }
                    }
                }
            }
            return DoingBusinessAsNames;
        }

        public static VersionInfo GetPolicyVersionInfo(int policyId, int policyImageNum)
        {
            var versionInfo = new VersionInfo();
            // could use the static data file if it was updated
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
            {
                conn.Open();
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetPolicyVersionInfo", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@PolicyId", policyId);
                    cmd.Parameters.AddWithValue("@PolicyImageNum", policyImageNum);
                    using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            versionInfo.LobId = reader["lob_id"].TryToGetInt32();
                            versionInfo.LobName = reader["lobname"].TryToGetString();
                            versionInfo.LobAbbreviation = reader["lob_abbr"].TryToGetString();
                            versionInfo.CompanyId = reader["company_id"].TryToGetInt32();
                            versionInfo.CompanyName = reader["company_name"].TryToGetString();
                            versionInfo.State = reader["statename"].TryToGetString();
                            versionInfo.StateAbbreviation = reader[name: "state"].TryToGetString();
                            versionInfo.StateId = reader["state_id"].TryToGetInt32();
                            versionInfo.VersionId = reader["version_id"].TryToGetInt32();
                        }
                    }
                }
            }
            return versionInfo;
        }

        public static string GetPolicyQuoteNumber(int policyId, int policyImageNum)
        {
            var qNumber = "";
            // could use the static data file if it was updated
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetQuoteNumberByPolicyIdAndPolicyImageNum", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@policyId", policyId);
                    cmd.Parameters.AddWithValue("@policyImageNum", policyImageNum);
                    qNumber = cmd.ExecuteScalar().TryToGetString();
                }
            }
            return qNumber;
        }
    }
}

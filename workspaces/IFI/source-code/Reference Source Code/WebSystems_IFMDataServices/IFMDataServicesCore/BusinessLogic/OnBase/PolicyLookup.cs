using IFM.DataServicesCore.CommonObjects.OnBase;
using IFM.PrimitiveExtensions;
using System;
using System.Linq;
using IBO = IFM.DataServicesCore.BusinessLogic.OnBase;
using Constants = IFM.DataServicesCore.CommonObjects.Constants.Constants;
using System.Data.SqlClient;
using IFM.DataServicesCore.BusinessLogic.Diamond.Policy;
using System.Threading.Tasks;
using System.Web;

namespace IFM.DataServicesCore.BusinessLogic.OnBase
{
    public class PolicyLookup : BusinessLogicBase
    {
        public OnBasePolicyInformation LoadPolicy(string PolicyNumber)
        {
            if (!string.IsNullOrWhiteSpace(PolicyNumber))
            {
                Diamond.Policy.PolicyImage lookup = (PolicyNumber[0] == 'Q') ? new Diamond.Policy.QuoteImage(PolicyNumber) : new Diamond.Policy.PolicyImage(PolicyNumber);
                if (lookup.Image != null)
                {
                    OnBasePolicyInformation onBasePolicy = new OnBasePolicyInformation();
                    onBasePolicy.PolicyNumber = PolicyNumber;
                    SetLob(lookup.Image.VersionId, out var lobid, out var lobName);
                    onBasePolicy.PolicyType = GetPolicyPrefix(PolicyNumber);
                    onBasePolicy.PolicyholderName = PolicyholderNameLookup.GatherPolicyholderNames(lookup?.Image).ToArray();
                    onBasePolicy.PolicyholderDBA = PolicyholderNameLookup.GatherPolicyDBANames(lookup?.Image).FirstOrDefault();
                    OnBaseAgencyInformation agencyInfo = new OnBaseAgencyInformation();
                    IBO.AgencyLookup agencyAccess = new IBO.AgencyLookup();
                    agencyInfo = agencyAccess.LoadAgency(lookup.Image.Agency.AgencyId);
                    onBasePolicy.AgencyCode = agencyInfo.Code;
                    onBasePolicy.AgencyName = agencyInfo.Name;
                    onBasePolicy.AgencyState = agencyInfo.State;
                    onBasePolicy.AgencyDBA = agencyInfo.DBA;
                    onBasePolicy.AgencyGroupCode = agencyInfo.GroupCode;
                    onBasePolicy.AgencyLocationCode = agencyInfo.LocationCode;
                    onBasePolicy.AgencyTerritory = getTerritory(lobName, agencyInfo.CommercialLinesTerritory, agencyInfo.PersonalLinesTerritory);
                    onBasePolicy.QuoteNumber = new string[] { lookup.Image.Quote };
                    onBasePolicy.PolicyState = lookup.Image.PolicyHolder.Address.StateAbbreviation;
                    onBasePolicy.OfficeAccount = (agencyInfo.Code == Constants.OfficeAccountAgencyCode) ? Constants.OfficeAccountTrue : Constants.OfficeAccountFalse;
                    return onBasePolicy;
                }
            }

            return null;
        }

        public OnBasePolicyInformation LoadPolicyV2(string PolicyNumber)
        {
            if (!string.IsNullOrWhiteSpace(PolicyNumber))
            {
                var policy = PolicyInfoLookup.GetPolicyIDAndImageNumByPolicyNumber(PolicyNumber);
                if (policy != null && policy.PolicyId > 0 && policy.PolicyImageNum > 0)
                {
                    return LoadPolicyV2(policy.PolicyId, policy.PolicyImageNum, PolicyNumber);
                }
            }

            return null;
        }

        public OnBasePolicyInformation LoadPolicyV2(int policyID, int policyImageNum)
        {
            string polNum = PolicyInfoLookup.GetPolicyNumberByPolicyId(policyID);
            return LoadPolicyV2(policyID, policyImageNum, polNum);
        }

        public OnBasePolicyInformation LoadPolicyV2(int policyID, int policyImageNum, string policyNumber)
        {
            var currentContext = HttpContext.Current;
            if (policyID > 0 && policyImageNum > 0)
            {
                OnBasePolicyInformation onBasePolicy = new OnBasePolicyInformation();
                CommonObjects.Diamond.VersionInfo versionInfo = new CommonObjects.Diamond.VersionInfo();
                OnBaseAgencyInformation agencyInfo = new OnBaseAgencyInformation();
                IBO.AgencyLookup agencyAccess = new IBO.AgencyLookup();
                string quoteNum = "";

                var t1 = Task.Run(() =>
                {
                    quoteNum = PolicyInfoLookup.GetPolicyQuoteNumber(policyID, policyImageNum);
                });
                var t2 = Task.Run(() =>
                {
                    versionInfo = PolicyInfoLookup.GetPolicyVersionInfo(policyID, policyImageNum);
                });
                var t3 = Task.Run(() =>
                {
                    onBasePolicy.PolicyholderName = PolicyInfoLookup.GetPolicyholdersRelatedToPolicy(policyID, policyImageNum).ToArray();
                });
                var t4 = Task.Run(() =>
                {
                    onBasePolicy.PolicyholderDBA = PolicyInfoLookup.GetPolicyholdersDoingBusinessAsNames(policyID, policyImageNum).FirstOrDefault();
                });
                var t5 = Task.Run(() =>
                {
                    HttpContext.Current = currentContext;
                    agencyInfo = agencyAccess.LoadAgency(policyID, policyImageNum);
                });
                Task.WaitAll(t1, t2, t3, t4, t5);

                onBasePolicy.PolicyNumber = policyNumber;
                onBasePolicy.PolicyType = versionInfo.LobAbbreviation;
                onBasePolicy.AgencyCode = agencyInfo.Code;
                onBasePolicy.AgencyName = agencyInfo.Name;
                onBasePolicy.AgencyState = agencyInfo.State;
                onBasePolicy.AgencyDBA = agencyInfo.DBA;
                onBasePolicy.AgencyGroupCode = agencyInfo.GroupCode;
                onBasePolicy.AgencyLocationCode = agencyInfo.LocationCode;
                onBasePolicy.AgencyTerritory = getTerritory(versionInfo.LobName, agencyInfo.CommercialLinesTerritory, agencyInfo.PersonalLinesTerritory);
                onBasePolicy.QuoteNumber = new string[] { quoteNum };
                onBasePolicy.PolicyState = versionInfo.StateAbbreviation;
                onBasePolicy.OfficeAccount = (agencyInfo.Code == Constants.OfficeAccountAgencyCode) ? Constants.OfficeAccountTrue : Constants.OfficeAccountFalse;
                return onBasePolicy;
            }

            return null;
        }

        private string GetPolicyPrefix(string policyNumber)
        {
            if (!string.IsNullOrWhiteSpace(policyNumber) && policyNumber.Length > 3)
            {
                policyNumber = policyNumber.ToUpper();
                return (policyNumber[0] == 'Q') ? policyNumber.Substring(1, 3) : policyNumber.Substring(0, 3);
            }
            return string.Empty;
        }

        private void SetLob(Int32 versionId, out Int32 lobId, out string lobName)
        {
            lobId = 0;
            lobName = string.Empty;
            // could use the static data file if it was updated
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
            {
                conn.Open();
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetLobInfoByVersion", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                                        cmd.Parameters.AddWithValue("@versionId", versionId);
                    using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            lobId = reader["lob_id"].TryToGetInt32();
                            lobName = reader["lobname"].TryToGetString();
                        }
                    }
                }
            }
        }

        private string getTerritory(string lob, string commercialTerritory, string personalTerritory)
        {
            // you'd think there would be a better way to do this but this is about as good as it gets.
            if (lob.ToUpper().Contains("FARM")) // farm can be personal or commercial only way to tell them apart is the type of policyholder
            {
                return personalTerritory; // requested change by Tracy 6-3-2019
                //return (policyHolder1NameTypeId.GetValueOrDefault() == 1) ? personalTerritory : commercialTerritory;
            }
            return lob.ToUpper().Contains("COM") ? commercialTerritory : personalTerritory;
        }

    }
}
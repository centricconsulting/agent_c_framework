using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFM.DataServicesCore.CommonObjects.OnBase;
using IBO = IFM.DataServicesCore.BusinessLogic.OnBase;
using DCO = Diamond.Common.Objects;
using System.Web;

namespace IFM.DataServicesCore.BusinessLogic.OnBase
{
    public class ClaimLookup : BusinessLogicBase
    {

        public OnBaseClaimInformation LoadClaim(string claimNumber)
        {            
            if (DiamondLogin.OnBaseLogin())
            {
                if (ClaimCommon.GetClaimControlId(claimNumber) != 0)
                {
                    using (var DS = Insuresoft.DiamondServices.ClaimsService.LoadClaimDetailInfo())
                    {
                        DS.RequestData.ClaimNumber = claimNumber;
                        var claimDetail = DS.Invoke()?.DiamondResponse?.ResponseData?.ClaimDetailInfo;
                        if (claimDetail != null)
                        {
                            OnBaseClaimInformation onBaseClaim = new OnBaseClaimInformation();
                            onBaseClaim.ClaimNumber = claimDetail.ClaimControl.ClaimNumber;
                            onBaseClaim.PolicyNumber = claimDetail.ClaimControl.PolicyNumber;
                            onBaseClaim.ClaimantName = LoadClaimantNames(claimDetail.ClaimControl.ClaimControlId).ToArray();
                            IBO.PolicyLookup onBasePolicyLookup = new IBO.PolicyLookup();
                            OnBasePolicyInformation onBasePolicy = new OnBasePolicyInformation();
                            onBasePolicy = onBasePolicyLookup.LoadPolicy(claimDetail.ClaimControl.PolicyNumber);
                            onBaseClaim.PolicyHolderName = onBasePolicy?.PolicyholderName ?? new string[] { "Not Available" }; 
                            onBaseClaim.OfficeAccount = onBasePolicy?.OfficeAccount ?? "no";

                            string claimAdjuster, claimAdjusterUserName = null;
                            GetClaimAdjuster(claimNumber, out claimAdjuster, out claimAdjusterUserName);
                            onBaseClaim.ClaimAdjuster = claimAdjuster;
                            onBaseClaim.ClaimAdjusterUserName = claimAdjusterUserName;
                            

                            DateTime.TryParse(claimDetail.ClaimControl.LossDate.ToString(), out var dt);
                            onBaseClaim.DateOfLoss = dt.ToShortDateString();                            
                            return onBaseClaim;
                        }
                    }
                }                
            }
            return null;
        }

        public OnBaseClaimInformation LoadClaimV2(string claimNumber)
        {
            if (DiamondLogin.OnBaseLogin())
            {
                var basicClaimInfo = ClaimCommon.GetBasicClaimInfo(claimNumber);
                if (basicClaimInfo != null)
                {
                    var currentContext = HttpContext.Current;
                    string claimAdjuster = null;
                    string claimAdjusterUserName = null;
                    var t1 = Task.Run(() =>
                    {
                        GetClaimAdjuster(claimNumber, out claimAdjuster, out claimAdjusterUserName);
                    });

                    OnBaseClaimInformation onBaseClaim = new OnBaseClaimInformation();
                    IBO.PolicyLookup onBasePolicyLookup = new IBO.PolicyLookup();
                    OnBasePolicyInformation onBasePolicy = new OnBasePolicyInformation();

                    onBaseClaim.ClaimNumber = basicClaimInfo.ClaimNumber;
                    onBaseClaim.PolicyNumber = basicClaimInfo.PolicyNumber;

                    var t2 = Task.Run(() =>
                    {
                        HttpContext.Current = currentContext;
                        onBaseClaim.ClaimantName = LoadClaimantNames(basicClaimInfo.ClaimControlId).ToArray();
                    });
                    var t3 = Task.Run(() =>
                    {
                        HttpContext.Current = currentContext;
                        onBasePolicy = onBasePolicyLookup.LoadPolicyV2(basicClaimInfo.PolicyNumber);
                    });

                    Task.WaitAll(t1, t2, t3);

                    onBaseClaim.PolicyHolderName = onBasePolicy?.PolicyholderName ?? new string[] { "Not Available" };
                    onBaseClaim.OfficeAccount = onBasePolicy?.OfficeAccount ?? "no";

                    onBaseClaim.ClaimAdjuster = claimAdjuster;
                    onBaseClaim.ClaimAdjusterUserName = claimAdjusterUserName;

                    onBaseClaim.DateOfLoss = basicClaimInfo.LossDate.ToShortDateString();
                    return onBaseClaim;
                }
            }
            return null;
        }

        private IEnumerable<string> LoadClaimantNames(int claimControlId)
        {
            var claimantNames = new List<string>();
            if (DiamondLogin.OnBaseLogin())
            {
                var claimantNumbersAvailable = IBO.ClaimCommon.ClaimantNumbers(claimControlId);
                if (claimantNumbersAvailable?.Any() ?? false)
                {
                    foreach (var number in claimantNumbersAvailable)
                    {
                        using (var DS = Insuresoft.DiamondServices.ClaimsService.LoadClaimantInfo())
                        {
                            DS.RequestData.ClaimantNum = number;
                            DS.RequestData.ClaimControlId = claimControlId;
                            var response = DS.Invoke()?.DiamondResponse;
                            GatherClaimantNames(response?.ResponseData?.Claimant.Name, ref claimantNames);
                        }
                    }
                }
            }
            //remove duplicates before returning
            return claimantNames.Distinct().ToList();
        }

        private void GetClaimAdjuster(string claimNumber, out string claimcontrolrep, out string claimcontrolrep_login)
        {
            claimcontrolrep = string.Empty;
            claimcontrolrep_login = string.Empty;
            using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
            {
                var result = conn.QueryFirst("dbo.usp_GetClaimAdjusterInfo", new { claimNumber }, commandType: CommandType.StoredProcedure);
                claimcontrolrep = result?.claimcontrolrep?.ToUpper() ?? string.Empty;
                claimcontrolrep_login = result?.claimcontrolrep_login?.ToUpper() ?? string.Empty;
            }
        }

     
               
        private List<string> GatherClaimantNames(DCO.Name claimantName, ref List<string> claimantNames)
        {

            if (claimantName != null)
            {
                string name = "";
                name = (claimantName.FirstName + " " + claimantName.LastName).Trim();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    claimantNames.Add(name);
                }
                name = claimantName.CommercialName1;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    claimantNames.Add(name);
                }
                name = claimantName.CommercialName2;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    claimantNames.Add(name);
                }

            }
            return claimantNames;
        }
    }
}
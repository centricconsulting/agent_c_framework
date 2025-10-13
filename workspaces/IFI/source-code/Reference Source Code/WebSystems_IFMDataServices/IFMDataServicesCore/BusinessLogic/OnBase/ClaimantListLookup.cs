using Dapper;
using IFM.DataServicesCore.CommonObjects.OnBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCO = Diamond.Common.Objects;
using IBO = IFM.DataServicesCore.BusinessLogic.OnBase;
using Constants = IFM.DataServicesCore.CommonObjects.Constants.Constants;

namespace IFM.DataServicesCore.BusinessLogic.OnBase
{
    public class ClaimantListLookup : BusinessLogicBase
    {
        public List<OnBaseClaimantInformation> LoadClaimantList(string claimNumber)
        {
            if (DiamondLogin.OnBaseLogin())
            {
                int claimControlId = ClaimCommon.GetClaimControlId(claimNumber);

                if (claimControlId != 0)
                {
                    return LoadClaimants(claimControlId);
                }
            }
            return null;
        }

        private List<OnBaseClaimantInformation> LoadClaimants(int claimControlId)
        {
            var claimantList = new List<OnBaseClaimantInformation>();
            if (DiamondLogin.OnBaseLogin())
            {
                var claimantNumbersAvailable = IBO.ClaimCommon.ClaimantNumbers(claimControlId);
                if (claimantNumbersAvailable?.Any() ?? false)
                {
                    foreach (var number in claimantNumbersAvailable)
                    {
                        using (var DS = Insuresoft.DiamondServices.ClaimsService.LoadClaimantInfo())
                        {
                            OnBaseClaimantInformation onBaseClaimant = new OnBaseClaimantInformation();
                            DS.RequestData.ClaimantNum = number;
                            DS.RequestData.ClaimControlId = claimControlId;
                            var response = DS.Invoke()?.DiamondResponse;
                            GatherClaimantInformation(response?.ResponseData?.Claimant?.Name, claimControlId, number, claimantList);
                        }
                    }
                }
            }
            return claimantList;
        }

        private void GatherClaimantInformation(DCO.Name claimantName, int claimControlId, int claimantNum, List<OnBaseClaimantInformation> claimantInformation)
        {
            if (claimantName != null)
            {
                OnBaseClaimantInformation onBaseClaimant = new OnBaseClaimantInformation();
                if (!string.IsNullOrWhiteSpace(claimantName.CommercialName1))
                {
                    onBaseClaimant.ClaimantName = claimantName.CommercialName1.Trim();
                }
                else
                {
                    onBaseClaimant.ClaimantName = $"{claimantName.FirstName} {claimantName.LastName}".Trim();
                }
                GetClaimantAddress(claimControlId, claimantNum, ref onBaseClaimant);
                if (!(string.IsNullOrWhiteSpace(onBaseClaimant.Address1) && string.IsNullOrWhiteSpace(onBaseClaimant.Address2)))
                {
                    claimantInformation.Add(onBaseClaimant);
                }
            }
        }

        private void GetClaimantAddress(int claimControlId, int claimantNum, ref OnBaseClaimantInformation onBaseClaimant)
        {
            //string sql = @"                            
            //    SELECT house_num, street_name, pobox, city, s.state, zip
            //        FROM [Diamond].[dbo].[Address] AS ad with (NoLock) 
            //        JOIN Diamond.dbo.State AS s WITH (NoLock) ON ad.state_id = s.state_id
            //        WHERE address_id = 
		          //      (SELECT TOP 1 address_id 
			         //       FROM ClaimantAddressLink 
			         //       WHERE claimcontrol_id = @claimcontrol_id
			         //         AND claimant_num = @claimant_num
            //                  AND nameaddresssource_id = @nameaddresssource_id
			         //       ORDER BY last_modified_date DESC)";
            using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetClaimantAddress", conn) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@claimcontrol_id", claimControlId);
                    cmd.Parameters.AddWithValue("@claimant_num", claimantNum);
                    cmd.Parameters.AddWithValue("@nameaddresssource_id", Constants.Claimant_NameAddressSource_Id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            onBaseClaimant.Address1 = $"{reader.GetString(0).Trim()} {reader.GetString(1).Trim()}".Trim();
                            onBaseClaimant.Address2 = FormatPOBox(reader.GetString(2).Trim());
                            onBaseClaimant.City = reader.GetString(3).Trim();
                            onBaseClaimant.State = reader.GetString(4).Trim();
                            onBaseClaimant.Zip =  FormatZip(reader.GetString(5).Trim());
                        }
                    }
                }
            }
        }

        //add p.o. box if just a number
        private static string FormatPOBox(string poBox)
        {
            poBox = poBox ?? string.Empty;
            if (poBox.Length > 0)
            {
                if (poBox[0].ToString().ToUpper() != "P")
                {
                    poBox = $"P.O. Box {poBox}";
                }
            }
            return poBox;
        }

        //strip plus 4 on zip if not present
        private static string FormatZip(string zipCode)
        {
            zipCode = zipCode ?? string.Empty;
            if (zipCode.Length == 10)
            {
                if (zipCode.Substring(5, 5) == "-0000")
                {
                    zipCode = zipCode.Substring(0, 5);
                }
            }
            return zipCode;
        }

    }
}
using IFM.DataServicesCore.CommonObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using DCO = Diamond.Common.Objects;
using DCS = Diamond.Common.Services;
using IDS = Insuresoft.DiamondServices;
using IFM.DataServicesCore.CommonObjects.Diamond;
using Diamond.Common.Objects.EFT;

#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.BusinessLogic.Diamond
{
    public static class AgencyInformation
    {

        public static IFM.DataServicesCore.CommonObjects.BasicAgencyInfo GetAgencyInformationByAgencyCode(string agencyCode)
        {
            var agency = new DCO.Policy.Agency.Agency();
            var basicAgencyInfo = new IFM.DataServicesCore.CommonObjects.BasicAgencyInfo();

            var agencyID = GetAgencyIDByCode(agencyCode);
            if(agencyID > 0)
            {
                using (var DS = Insuresoft.DiamondServices.AdministrationService.LoadAgency())
                {
                    DS.RequestData.AgencyId = agencyID;
                    agency = DS.Invoke()?.DiamondResponse?.ResponseData?.Agency;
                    if(agency != null)
                    {
                        basicAgencyInfo.AgencyId = agency.AgencyId;
                        basicAgencyInfo.Code = agency.Code;
                        basicAgencyInfo.Email = agency.Emails[0].Address;
                        basicAgencyInfo.Name = agency.Name.CommercialName2;
                    }
                }
            }

            return basicAgencyInfo;
        }

        public static IFM.DataServicesCore.CommonObjects.BasicAgencyInfo GetAgencyInformationByAgencyID(int agencyID)
        {
            var agency = new DCO.Policy.Agency.Agency();
            var basicAgencyInfo = new IFM.DataServicesCore.CommonObjects.BasicAgencyInfo();

            if (agencyID > 0)
            {
                using (var DS = Insuresoft.DiamondServices.AdministrationService.LoadAgency())
                {
                    DS.RequestData.AgencyId = agencyID;
                    agency = DS.Invoke()?.DiamondResponse?.ResponseData?.Agency;
                    if (agency != null)
                    {
                        basicAgencyInfo.AgencyId = agency.AgencyId;
                        basicAgencyInfo.Code = agency.Code;
                        basicAgencyInfo.Email = agency.Emails[0].Address;
                        basicAgencyInfo.Name = agency.Name.CommercialName2;
                    }
                }
            }

            return basicAgencyInfo;
        }

        public static string GetAgencyCodeByAgencyID(int agencyID)
        {
            var agencyInfo = GetAgencyInformationByAgencyID(agencyID);
            if(agencyInfo != null)
            {
                return agencyInfo.Code;
            }
            return "";
        }

        public static IFM.DataServicesCore.CommonObjects.BasicAgencyInfo GetAgencyInformationByPolicyNumber(string policyNumber)
        {
            var agency = new DCO.Policy.Agency.Agency();
            var basicAgencyInfo = new IFM.DataServicesCore.CommonObjects.BasicAgencyInfo();

            var agencyID = GetAgencyIDByPolicyNumber(policyNumber);
            if (agencyID > 0)
            {
                using (var DS = Insuresoft.DiamondServices.AdministrationService.LoadAgency())
                {
                    DS.RequestData.AgencyId = agencyID;
                    agency = DS.Invoke()?.DiamondResponse?.ResponseData?.Agency;
                    if (agency != null)
                    {
                        basicAgencyInfo.AgencyId = agency.AgencyId;
                        basicAgencyInfo.Code = agency.Code;
                        basicAgencyInfo.Email = agency.Emails[0].Address;
                        basicAgencyInfo.Name = agency.Name.CommercialName2;
                    }
                }
            }
            return basicAgencyInfo;
        }

        public static int GetAgencyIDByCode(string agencyCode)
        {
            int agencyID = 0;

            using (var DS = Insuresoft.DiamondServices.AgencyAdministrationService.GetAgencyWorkflowInfoByCode())
            {
                DS.RequestData.AgencyCode = agencyCode;
                var info = DS.Invoke()?.DiamondResponse?.ResponseData?.AgencyId;
                agencyID = info.GetValueOrDefault();
            }

            return agencyID;
        }

        public static int GetAgencyIDByPolicyIdAndImageNum(int policyID, int policyImageNum)
        {
            int agencyID = 0;

            if (global::IFM.DataServicesCore.BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {
                using (var DS = Insuresoft.DiamondServices.AgencyAdministrationService.GetAgencyIDByPolicyIDAndImageNum())
                {
                    if (policyID > 0 && policyImageNum > 0)
                    {
                        DS.RequestData.PolicyID = policyID;
                        DS.RequestData.PolicyImageNumID = policyImageNum;

                        DCS.Messages.AgencyAdministrationService.GetAgencyIDByPolicyIDAndImageNum.ResponseData resData = DS.Invoke()?.DiamondResponse?.ResponseData;
                        if (resData?.PolicyImage?.Count > 0)
                        {
                            agencyID = resData.PolicyImage[0].AgencyId;
                        }
                        else
                        {
                            if (1 == 1)
                            {

                            }
                        }
                    }
                }
            }

            if (agencyID > 0)
            {
                return agencyID;
            }
            else
            {
                return 0;
            }
        }

        public static int GetAgencyIDByPolicyNumber(string policyNumber)
        {
            var policyInfo = Policy.PolicyInfoLookup.GetPolicyIDAndImageNumByPolicyNumber(policyNumber);
            if (policyInfo != null && policyInfo.PolicyId > 0 && policyInfo.PolicyImageNum > 0)
            {
                var agencyInfo = GetAgencyEFTInformation(policyInfo.PolicyId, policyInfo.PolicyImageNum);
                if (agencyInfo != null && agencyInfo.AgencyId > 0)
                {
                    return agencyInfo.AgencyId;
                }
            }
            return 0;
        }

        public static AgencyEFTInfo GetAgencyEFTInformation(int policyId, int policyImageNum)
        {
            AgencyEFTInfo agencyInfo = new AgencyEFTInfo();
            using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetAgencyEFTInfoByPolicyId", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@PolicyId", policyId);
                    if (policyImageNum > 0)
                    {
                        cmd.Parameters.AddWithValue("@PolicyImageNum", policyImageNum);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                agencyInfo.AgencyId = reader["agency_id"].TryToGetInt32();
                                agencyInfo.EFTAccountId = reader["eftaccount_id"].TryToGetInt32();
                                agencyInfo.LegacyAgencyId = reader["LegacyAgencyId"].TryToGetInt32();
                            }
                        }
                    }
                }
            }
            return agencyInfo;
        }
    }
}

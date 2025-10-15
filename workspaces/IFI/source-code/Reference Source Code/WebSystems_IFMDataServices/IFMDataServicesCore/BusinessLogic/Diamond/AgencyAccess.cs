using IFM.DataServicesCore.CommonObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using DCO = Diamond.Common.Objects;
using DCE = Diamond.Common.Enums;
using DCS = Diamond.Common.Services;
using IDS = Insuresoft.DiamondServices;

#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.BusinessLogic.Diamond
{
    public static class AgencyAccess
    {
        public static List<BasicAgencyInfo> GetMyAgencies()
        {
            var userid = BusinessLogic.Diamond.Login.GetUserId();
            if (userid > 0)
            {
                using (var DSGetAgencies = IDS.SecurityService.GetViewableAgencies())
                {
                    DSGetAgencies.RequestData.UsersId = userid;
                    var agencies = DSGetAgencies.Invoke()?.DiamondResponse?.ResponseData?.Agencies;
                    if (agencies != null)
                    {
                        List<BasicAgencyInfo> MyAgencies = new List<BasicAgencyInfo>();
                        foreach (var agency in agencies)
                        {
                            using (var DSAgencyInfo = IDS.LookupService.GetAgencyDataForAgencyId())
                            {
                                DSAgencyInfo.RequestData.AgencyId = agency.AgencyId;
                                using (var agInfo = DSAgencyInfo.Invoke()?.DiamondResponse?.ResponseData?.Agency)
                                {
                                    if (agInfo != null)
                                    {
                                        MyAgencies.Add(new BasicAgencyInfo
                                        {
                                            AgencyId = agInfo.AgencyId,
                                            Code = agInfo.Code,
                                            Name = agInfo.Name
                                        });
                                    }
                                }
                            }
                        }
                        return MyAgencies;
                    }
                }
            }
            else
            {
#if DEBUG
                Debugger.Break();
#else
                        global::IFM.IFMErrorLogging.LogIssue("No Diamond User.", "IFMDATASERVICES -> AgencyAccess.cs -> Function GetMyAgencies");
#endif
            }
            return null;
        }

        public static bool HaveAgencyAccess(Int32 agencyId)
        {
            var agencyInfo = GetMyAgencies();
            if (agencyInfo != null)
            {
                return (from a in agencyInfo where a.AgencyId == agencyId select a).Any();
            }
            return false;
        }

        public static IFM.DataServicesCore.CommonObjects.Diamond.AssignAgencyResult AssignAgencyToUser(DCO.Administration.UserAgencyLink newUAL, DCO.Administration.Users userRecord, DCO.InsCollection<DCO.Administration.UserAgencyLink> UALs, DCO.InsCollection<DCO.Administration.UserLinkBase> ULBs)
        {
            IFM.DataServicesCore.CommonObjects.Diamond.AssignAgencyResult AAR = new CommonObjects.Diamond.AssignAgencyResult();
            int DiamondUserId = ULBs[0].UsersID;

            if(newUAL != null && userRecord != null && UALs?.Count > 0 && UALs?.Count > 0 && ULBs?.Count > 0)
            {
                using (var DS = IDS.AllServices.AdministrationService_SaveUser())
                {
                    DS.RequestData.UsersRecord = userRecord;
                    DS.RequestData.UserLinkRecords = ULBs;
                    DS.RequestData.UserLinkRecords.Add(newUAL); //Needs to be added here to?
                    DS.RequestData.AgencyUserLinkRecords = UALs;
                    DS.RequestData.AgencyUserLinkRecords.Add(newUAL);

                    using (var i = DS.Invoke())
                    {
                        AAR.agencyID = newUAL.AgencyId;
                        AAR.success = false;
                        if (i?.ex != null)
                        {
                            AAR.APIException = i.ex;
                        }
                        if (i?.DiamondResponse?.DiamondValidation.ValidationItems?.Count > 0)
                        {
                            AAR.DiamondValidation = i.DiamondResponse.DiamondValidation;
                        }
                        if (i?.DiamondResponse?.ResponseData != null)
                        {
                            AAR.success = i.DiamondResponse.ResponseData.Success;
                        }
                    }
                }
            }
            else
            {
                AAR.ErrorMessage = "All of the passed information must not be null and contain information.";
            }
            return AAR;
        }

        public static List<IFM.DataServicesCore.CommonObjects.Diamond.AssignAgencyResult> AssignMissingAgenciesToUserAsSecondaryAgencies(int DiamondUserID)
        {
            DCO.InsCollection<DCO.Administration.UserAgencyLink> myUserAgencyLinks = new DCO.InsCollection<DCO.Administration.UserAgencyLink>();
            DCO.InsCollection<DCO.Administration.UserLinkBase> myUserLinks = new DCO.InsCollection<DCO.Administration.UserLinkBase>();
            DCO.Administration.Users myUserRecord = new DCO.Administration.Users();
            List<IFM.DataServicesCore.CommonObjects.Diamond.AssignAgencyResult> AARs = new List<CommonObjects.Diamond.AssignAgencyResult>();
            AARs.Add(new CommonObjects.Diamond.AssignAgencyResult()); //Add a result item to the list so that we can add initial API errors if they occur.
            DCO.InsCollection<DCO.Policy.Agency.Agency> allAgencies = null;

            using (var DS = IDS.AllServices.AdministrationService_LoadAllAgencies())
            {
                using (var i = DS.Invoke())
                {
                    if (i?.DiamondResponse?.ResponseData != null)
                    {
                        allAgencies = i.DiamondResponse.ResponseData.Agency;
                    }
                }
            }

            using (var DS = IDS.AllServices.LookupService_GetUserAgencyByUser())
            {
                DS.RequestData.UsersId = DiamondUserID;
                using (var i = DS.Invoke())
                {
                    if (i?.ex != null)
                    {
                        AARs[0].APIException = i.ex;
                    }
                    if (i?.DiamondResponse?.DiamondValidation.ValidationItems?.Count > 0)
                    {
                        AARs[0].DiamondValidation = i.DiamondResponse.DiamondValidation;
                    }
                    if (i?.DiamondResponse?.ResponseData != null)
                    {
                        var agencies = i.DiamondResponse.ResponseData.UserAgencies;
                        if(agencies != null)
                        {
                            foreach (var agency in agencies)
                            {
                                DCO.Administration.UserAgencyLink thisAgency = new DCO.Administration.UserAgencyLink();
                                thisAgency.AgencyId = agency.AgencyId;
                                thisAgency.UserAgencyRelationTypeId = (DCE.UserAgencyRelationType)agency.UserAgencyRelationTypeId;
                                thisAgency.UsersID = agency.UsersId;
                                myUserAgencyLinks.Add(thisAgency);
                            }
                        }
                    }
                }
            }

            using (var DS = IDS.AllServices.AdministrationService_LoadUserLinks())
            {
                DS.RequestData.UserID = DiamondUserID;
                DS.RequestData.UserCategoryID = DCE.UserCategory.UserCategory_Agency;
                using (var i = DS.Invoke())
                {
                    if (i?.ex != null)
                    {
                        AARs[0].APIException = i.ex;
                    }
                    if (i?.DiamondResponse?.DiamondValidation.ValidationItems?.Count > 0)
                    {
                        AARs[0].DiamondValidation = i.DiamondResponse.DiamondValidation;
                    }
                    if (i?.DiamondResponse?.ResponseData != null)
                    {
                        myUserLinks = i.DiamondResponse.ResponseData.UserLinkRecords;
                    }
                }
            }

            using (var DS = IDS.AdministrationService.LoadvUser())
            {
                DS.RequestData.UserID = DiamondUserID;
                using (var i = DS.Invoke())
                {
                    if (i?.ex != null)
                    {
                        AARs[0].APIException = i.ex;
                    }
                    if (i?.DiamondResponse?.DiamondValidation.ValidationItems?.Count > 0)
                    {
                        AARs[0].DiamondValidation = i.DiamondResponse.DiamondValidation;
                    }
                    if (i?.DiamondResponse?.ResponseData != null)
                    {
                        myUserRecord = i.DiamondResponse.ResponseData.vUsersRecords[0];
                    }
                }
            }

            if (AARs[0].APIException == null && allAgencies?.Count > 0 && myUserAgencyLinks?.Count > 0)
            {
                List<DCO.Policy.Agency.Agency> agenciesToAdd = allAgencies.Where(x => myUserAgencyLinks.Find(y => x.AgencyId == y.AgencyId) == null).ToList();
                if (agenciesToAdd != null && agenciesToAdd?.Count > 0)
                {
                    foreach (var agency in agenciesToAdd)
                    {
                        DCO.Administration.UserAgencyLink newUAL = new DCO.Administration.UserAgencyLink();
                        newUAL.AgencyId = agency.AgencyId;
                        newUAL.UserAgencyRelationTypeId = DCE.UserAgencyRelationType.UserAgencyRelationType_SECONDARY;
                        newUAL.IsAgencyAdministrator = false;
                        newUAL.UsersID = DiamondUserID;
                        AARs.Add(AssignAgencyToUser(newUAL, myUserRecord, myUserAgencyLinks, myUserLinks));
                    }
                }
                else
                {
                    AARs[0].success = true; // No missing Agencies to add
                }
            }
            else
            {
                if(AARs[0].APIException != null)
                {
                    IFM.IFMErrorLogging.LogException(AARs[0].APIException, $"IFMDATASERVICES -> CORE -> BusinessLogic -> Diamond -> AgencyAccess -> AssignMissingAgenciesToUserAsSecondaryAgencies; DiamondUserID={DiamondUserID}");
                }
            }
            return AARs;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCS = Diamond.Common.Services;
using DCSDM = Diamond.Common.StaticDataManager;
using DUSDM = Diamond.UI.StaticDataManager;
using DUU = Diamond.UI.Utility;
using DCO = Diamond.Common.Objects;
using DCE = Diamond.Common.Enums;
using System.Runtime.Remoting;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI;
using System.IO;
using System.Collections;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using IFM.PrimitiveExtensions;
using IFM.DataServices.API.RequestObjects.OnBase;

namespace IFM.DataServicesCore.BusinessLogic.Diamond
{
   public class Claim
    {
        public static List<string> Errors { get; set; }
        private const string ClassName = "FNOLCommonLibrary";

      //public static (DCS.Messages.ClaimsService.SaveClaimControlVehicles.Response,DCS.Messages.ClaimsService.SaveClaimControlProperties.Response) SubmitClaim(DataServicesCore.CommonObjects.OMP.FNOL fnol)//, DataServicesCore.CommonObjects.Enums.Enums.FNOL_LOB_Enum FNOLType)//, ref FNOLResponseData_Struct ReturnData)
      public static FNOLResponseData SubmitClaim(DataServicesCore.CommonObjects.OMP.FNOL fnol) 
        {
            var responseAddVehicles= new DCS.Messages.ClaimsService.SaveClaimControlVehicles.Response();
                var responseProperty =new DCS.Messages.ClaimsService.SaveClaimControlProperties.Response();
            FNOLResponseData returnData = new FNOLResponseData();
            Diamond.Login.LoginNow(AppConfig.PrintUserName, AppConfig.PrintUserPassword);
            if (Diamond.Login.IsLoggedIN() == true)
            {
                try
                {
                    DCS.Messages.ClaimsService.SubmitLossNotice.Request request = new DCS.Messages.ClaimsService.SubmitLossNotice.Request();
                    DCS.Messages.ClaimsService.SubmitLossNotice.Response response = new DCS.Messages.ClaimsService.SubmitLossNotice.Response();
                    request.RequestData.Attempt = fnol.SaveAttempt;
                    request.RequestData.ClaimLnStatusTypeId = fnol.StatusType;
                    
                    if (request.RequestData.User == null)
                    {
                        request.RequestData.User = new DCO.User();
                        if(fnol.EntryUser =="MemberPortal")
                        request.RequestData.User.UsersId = Login.GetUserId();
                        else
                            request.RequestData.User.UsersId =fnol.UserID;
                    }
                    request.RequestData.IgnorePersonnel = false;
                    request.RequestData.PolicyId = fnol.PolicyID;
                    if (request.RequestData.LossNoticeData == null)
                    {
                        request.RequestData.LossNoticeData = new DCO.Claims.LossNotice.LossNoticeData();
                        request.RequestData.LossNoticeData.Personnel.ClaimOfficeId = Convert.ToInt32(AppConfig.FNOLClaimOfficeID);
                        //request.RequestData.LossNoticeData.Personnel.InsideAdjusterId = Convert.ToInt32(fnol.InsideAdjusterId);
                        //request.RequestData.LossNoticeData.Personnel.AdministratorId = Convert.ToInt32(fnol.AdministratorId);
                        request.RequestData.LossNoticeData.ReportedBy.ClaimReportedByMethodId = 5;
                        request.RequestData.LossNoticeData.ReportedBy.ClaimReportedById = 1;
                    }
                    if (fnol.Witnesses != null && fnol.Witnesses.Count > 0)
                    {
                        if (request.RequestData.LossNoticeData.Witnesses == null)
                            request.RequestData.LossNoticeData.Witnesses = new DCO.InsCollection<DCO.Claims.LossNotice.Witness>();
                        foreach (IFM.DataServicesCore.CommonObjects.OMP.Witness w in fnol.Witnesses)
                        {
                            DCO.Claims.LossNotice.Witness diaWit = new DCO.Claims.LossNotice.Witness();
                            diaWit.Name = AddName(w.Name);
                            diaWit.Address = AddAddress(w.Address);
                            diaWit.Phones = AddPhones(w.Phone);
                            diaWit.Emails = AddEmails(w.Email);
                            diaWit.Remarks = w.Remarks;
                            if (w.RelationshipId != "" && string.IsNullOrEmpty(w.RelationshipId) == true)
                                diaWit.RelationshipId = Convert.ToInt32(w.RelationshipId);
                            request.RequestData.LossNoticeData.Witnesses.Add(diaWit);
                        }
                    }

                    //string reportedDateDebugInfo = "AppConfig.DiamondClaimsFNOL_ReportedDate_UseLossDate: " + AppConfig.DiamondClaimsFNOL_ReportedDate_UseLossDate.ToString();
                    if (AppConfig.DiamondClaimsFNOL_ReportedDate_UseLossDate == true)
                    {
                        DateTime ld = fnol.LossDate > DateTime.Today ? DateTime.Today : fnol.LossDate; //similar to lossDate logic below
                        request.RequestData.LossNoticeData.LnImage.ReportedDate = (DCO.InsDateTime)ld;
                        request.RequestData.LossNoticeData.ReportedBy.ReportedDate = (DCO.InsDateTime)ld;
                        //reportedDateDebugInfo += "<br />" + "ld: " + ld.ToString();
                        //reportedDateDebugInfo += "<br />" + "request.RequestData.LossNoticeData.LnImage.ReportedDate: " + request.RequestData.LossNoticeData.LnImage.ReportedDate.ToString();
                    }
                    else
                    {
                        request.RequestData.LossNoticeData.LnImage.ReportedDate = (DCO.InsDateTime)DateTime.Now;
                        request.RequestData.LossNoticeData.ReportedBy.ReportedDate = (DCO.InsDateTime)DateTime.Now;
                        //reportedDateDebugInfo += "<br />" + "request.RequestData.LossNoticeData.LnImage.ReportedDate: " + request.RequestData.LossNoticeData.LnImage.ReportedDate.ToString();
                    }
                    //SendEmail("MPAPI FNOL ReportedDate Debug Info", "N/A", reportedDateDebugInfo);
                    
                    if (fnol.LossAddress != null)
                        request.RequestData.LossNoticeData.LossAddress = AddAddress(fnol.LossAddress);

                    request.RequestData.LossNoticeData.LnImage.LossDate = fnol.LossDate > fnol.ReportedDate ? (DCO.InsDateTime)fnol.ReportedDate : (DCO.InsDateTime)fnol.LossDate;
                    request.RequestData.LossNoticeData.LnImage.LossTimeGiven = false;
                    if(fnol.PolicyStatus == "Cancelled" || fnol.PolicyStatus =="Future")
                        request.RequestData.LossNoticeData.LnImage.ClaimTypeId = (int)DCE.Claims.ClaimType.RecordOnly;
                    else
                        request.RequestData.LossNoticeData.LnImage.ClaimTypeId = (int)DCE.Claims.ClaimType.Normal;
                    if (fnol.ClaimSeverity_Id == CommonObjects.Enums.Enums.SeverityLoss.MinorLoss && fnol.MedicalAttention == true)
                         request.RequestData.LossNoticeData.LnImage.ClaimSeverityId = (int)CommonObjects.Enums.Enums.SeverityLoss.MinorInjury;
                    else if (fnol.ClaimSeverity_Id == CommonObjects.Enums.Enums.SeverityLoss.ModerateLoss && fnol.MedicalAttention == true)
                        request.RequestData.LossNoticeData.LnImage.ClaimSeverityId = (int)CommonObjects.Enums.Enums.SeverityLoss.ModerateInjury;
                    else if (fnol.ClaimSeverity_Id == CommonObjects.Enums.Enums.SeverityLoss.SevereLoss && fnol.MedicalAttention == true)
                        request.RequestData.LossNoticeData.LnImage.ClaimSeverityId = (int)CommonObjects.Enums.Enums.SeverityLoss.SevereInjury;
                    else
                        request.RequestData.LossNoticeData.LnImage.ClaimSeverityId = (int)fnol.ClaimSeverity_Id;
                    PolicyNumberObject policyNumberObject = PopulatePolicyInfo(fnol.PolicyNumber, fnol.LossDate.ToString());
                    if(policyNumberObject.PolicyID.HasValue())
                    request.RequestData.LossNoticeData.PolicyId = Convert.ToInt32(policyNumberObject.PolicyID);
                    if (policyNumberObject.DiamondPolicyImageNum.HasValue())
                        request.RequestData.LossNoticeData.PolicyImageNum = Convert.ToInt32(policyNumberObject.DiamondPolicyImageNum);
                    if (policyNumberObject.PolicyID.HasValue())
                        request.RequestData.LossNoticeData.LnImage.PolicyId =Convert.ToInt32(policyNumberObject.PolicyID);
                    if (policyNumberObject.DiamondPolicyImageNum.HasValue())
                        request.RequestData.LossNoticeData.LnImage.PolicyImageNum = Convert.ToInt32(policyNumberObject.DiamondPolicyImageNum);
                    if (policyNumberObject.DiamondVersionId.HasValue() || fnol.VersionId > 0)
                    {
                        request.RequestData.LossNoticeData.LnImage.VersionId = fnol.VersionId > 0 ? Convert.ToInt32(fnol.VersionId) : Convert.ToInt32(policyNumberObject.DiamondVersionId);
                    }
                   
                    if (fnol.packagePartType.HasValue())
                        request.RequestData.LossNoticeData.LnImage.PackagePartNum =  fnol.packagePartType;

                    SetLnImageNotePropsIfNeeded(request.RequestData.LossNoticeData.LnImage);

                    #region Claimants
                    if (fnol.Claimants != null && fnol.Claimants.Count > 0)
                    {
                        if (request.RequestData.LossNoticeData.Parties == null)
                            request.RequestData.LossNoticeData.Parties = new DCO.InsCollection<DCO.Claims.LossNotice.ThirdParty>();
                        
                        for (int i = 0; i <= fnol.Claimants.Count - 1; i++)
                        {
                            if (IsInsuredClaimant(fnol.Claimants[i]) == false || DiamondClaimsFNOL_OmitInsuredClaimants() == false)
                            {
                                request.RequestData.LossNoticeData.Parties.Add(new DCO.Claims.LossNotice.ThirdParty());
                                {
                                    request.RequestData.LossNoticeData.Parties[i].Name = AddName(fnol.Claimants[i].Name);
                                    request.RequestData.LossNoticeData.Parties[i].Address = AddAddress(fnol.Claimants[i].Address);
                                    request.RequestData.LossNoticeData.Parties[i].Phones = AddPhones(fnol.Claimants[i].Phone);
                                    request.RequestData.LossNoticeData.Parties[i].Emails = AddEmails(fnol.Claimants[i].Email);
                                    if (fnol.Claimants[i].claimantTypeID > 0)
                                        request.RequestData.LossNoticeData.Parties[i].ClaimantTypeId = fnol.Claimants[i].claimantTypeID;
                                }
                            }
                        }
                    }
                    #endregion
                    #region Insured
                    if (DiamondClaimsFNOL_OmitInsuredClaimants() == false)
                    {
                         DCO.Claims.LossNotice.ThirdParty insured1Party = GetInsuredThirdPartyFromList(request.RequestData.LossNoticeData.Parties, CommonObjects.Enums.Enums.Insured1orInsured2.Insured1, true); // manually setting returnNewAndAddToListIf1or2AndNotFound to True in case default value ever changes
                        if (insured1Party != null)
                        {
                            if (fnol.Insured != null)
                            {
                                insured1Party.Name = AddName(fnol.Insured.Name);
                                insured1Party.Address = AddAddress(fnol.Insured.Address);
                                insured1Party.Phones = AddPhones(fnol.Insured.Phone);
                                insured1Party.Emails = AddEmails(fnol.Insured.Email);
                            }
                            if (fnol.SecondInsured != null) 
                            {
                                DCO.Claims.LossNotice.ThirdParty insured2Party = GetInsuredThirdPartyFromList(request.RequestData.LossNoticeData.Parties, CommonObjects.Enums.Enums.Insured1orInsured2.Insured2, true); // manually setting returnNewAndAddToListIf1or2AndNotFound to True in case default value ever changes
                                if (insured2Party != null)
                                {

                                    insured2Party.Name = AddName(fnol.SecondInsured.Name);
                                    insured2Party.Address = AddAddress(fnol.SecondInsured.Address);
                                    insured2Party.Phones = AddPhones(fnol.SecondInsured.Phone);
                                    insured2Party.Emails = AddEmails(fnol.SecondInsured.Email);

                                }
                            }
                        }
                    }
                    #endregion
                    #region SubmitLossNotice

                    request.RequestData.LossNoticeData.LnLossInfo.ClaimLossTypeId = (int)fnol.ClaimLossType;
                    request.RequestData.LossNoticeData.LnLossInfo.Description = fnol.Description;
                    request.RequestData.LossNoticeData.LnLossInfo.ClaimFaultId = (int)DataServicesCore.CommonObjects.Enums.Enums.ClaimFaultType.Undetermined;
                    request.RequestData.LossNoticeData.LnLossInfo.AccidentLocationText = fnol.ClaimLocation;
                    if ((fnol.FNOLType == CommonObjects.Enums.Enums.FNOL_LOB_Enum.Auto || fnol.FNOLType == CommonObjects.Enums.Enums.FNOL_LOB_Enum.CommercialAuto) && fnol.Vehicles.Count > 0)
                    {
                        int i = 1;
                        foreach (var vehicle in fnol.Vehicles)
                        {
                            request.RequestData.LossNoticeData.Vehicles = new DCO.InsCollection<DCO.Claims.LossNotice.LnVehicle>();
                            request.RequestData.LossNoticeData.Vehicles.Add(new DCO.Claims.LossNotice.LnVehicle()
                            {
                                ClaimLnVehicleNum = new DCO.IdValue(i),
                                DamageDescription = vehicle.DamageDescription,
                                License = vehicle.LicensePlate,
                                Make = vehicle.Make,
                                Model = vehicle.Model,
                                VIN = vehicle.VIN,
                                Year = vehicle.Year,
                                LnVehicleOwner = new DCO.Claims.LossNotice.LnVehicleOwner
                                {
                                    Name = AddName(vehicle.LossVehicleOwnerName),
                                    Address = AddAddress(vehicle.LossAddress)
                                },
                                LnVehicleOperator = new DCO.Claims.LossNotice.LnVehicleOperator { Name = AddName(vehicle.LossVehicleOperatorName) },
                                DrivableId = vehicle.DrivableId,
                                AirbagsDeployedTypeId = vehicle.AirbagsDeployedTypeId,
                                CCCEstimateQualification = new DCO.ThirdParty.CCC.CCCEstimateQualification
                                {
                                    Question1YesNoId = vehicle.CCCEstimateQualificationId,
                                    Phone = !string.IsNullOrEmpty(vehicle.CCCphone) ? String.Format("{0:(###)###-####}", Convert.ToInt64(vehicle.CCCphone)) : vehicle.CCCphone,
                                    SendQuickEstimate = vehicle.CCCEstimateQualificationId == 1 ? true : false,
                                    LanguageTypeId = 1
                                },
                                LocationOfAccident = new DCO.Claims.ClaimEntity
                                {
                                    Address = AddAddress(vehicle.LossAddress)
                                }


                            });
                            i++;
                        }
                    }
                    using (DCS.Proxies.ClaimsServiceProxy proxy = new DCS.Proxies.ClaimsServiceProxy())
                    {
                        JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
                        {
                            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                        };


                        request.RequestData.ClaimLnStatusTypeId = DCE.Claims.ClaimLnStatusType.Submitted;
                        string json = JsonConvert.SerializeObject(request, microsoftDateFormatSettings);
                        response = proxy.SubmitLossNotice(request);
                        if (response.ResponseData.LossNoticeData.Personnel.InsideAdjusterId > 0 || response.ResponseData.LossNoticeData.Personnel.OutsideAdjusterId > 0)
                        {
                            returnData.AdjusterName = "";
                            returnData.AssignedBy = "Diamond Automatic";
                            returnData.AssignedSuccessfully = true;
                            returnData.CAT = false;
                            returnData.DateAssigned = DateTime.Now.ToShortDateString();
                            // Adjuster ID
                            if (response.ResponseData.LossNoticeData.Personnel.InsideAdjusterId > 0)
                                returnData.DiamondAdjuster_Id = response.ResponseData.LossNoticeData.Personnel.InsideAdjusterId.ToString();
                            else if (response.ResponseData.LossNoticeData.Personnel.OutsideAdjusterId > 0)
                                returnData.DiamondAdjuster_Id = response.ResponseData.LossNoticeData.Personnel.OutsideAdjusterId.ToString();

                            returnData.FNOLAdjusterID = "";
                            returnData.FNOL_ID = 0;
                        }
                        else
                        {
                            // No Adjuster ID returned
                            returnData = new FNOLResponseData();
                            returnData.AssignedSuccessfully = false;
                        }
                    }
                    #endregion
                    if (response != null && response.DiamondValidation.ValidationItems.Count == 0)
                    {
                        if (fnol.FNOLType == CommonObjects.Enums.Enums.FNOL_LOB_Enum.Liability || fnol.FNOLType == CommonObjects.Enums.Enums.FNOL_LOB_Enum.Property)
                        //   else if (fnol.ClaimLossType == CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityAllOther || fnol.ClaimLossType == CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityPropertyDamage)
                        {
                            // begin property, liability

                            if (fnol.Properties.Count > 0)
                            {
                                try
                                {
                                    DCS.Messages.ClaimsService.SaveClaimControlProperties.Request reqProp = new DCS.Messages.ClaimsService.SaveClaimControlProperties.Request();
                                    DCS.Messages.ClaimsService.SaveClaimControlProperties.Response resProp = new DCS.Messages.ClaimsService.SaveClaimControlProperties.Response();

                                    {
                                        reqProp.RequestData.ClaimControlId = response.ResponseData.ClaimControlId;

                                        if (reqProp.RequestData.ClaimControlProperties == null)
                                            reqProp.RequestData.ClaimControlProperties = new DCO.InsCollection<DCO.Claims.ClaimControl.ClaimControlProperty>();

                                        for (int i = 0; i <= fnol.Properties.Count - 1; i++)
                                        {
                                            reqProp.RequestData.ClaimControlProperties.Add(new DCO.Claims.ClaimControl.ClaimControlProperty());
                                            {
                                                if (reqProp.RequestData.ClaimControlProperties[i].Location == null)
                                                    reqProp.RequestData.ClaimControlProperties[i].Location = new DCO.Address();
                                                reqProp.RequestData.ClaimControlProperties[i].DamageDescription = fnol.Properties[i].DamageDescription;
                                                reqProp.RequestData.ClaimControlProperties[i].Location.HouseNumber = fnol.Properties[i].Location.HouseNumber;
                                                reqProp.RequestData.ClaimControlProperties[i].Location.StreetName = fnol.Properties[i].Location.StreetName;
                                                reqProp.RequestData.ClaimControlProperties[i].Location.City = fnol.Properties[i].Location.City;

                                                if (fnol.Properties[i].Location.ApartmentNumber != "")
                                                    reqProp.RequestData.ClaimControlProperties[i].Location.ApartmentNumber = fnol.Properties[i].Location.ApartmentNumber;

                                                if (fnol.Properties[i].Location.POBox != "") reqProp.RequestData.ClaimControlProperties[i].Location.POBox = fnol.Properties[i].Location.POBox;

                                                if (fnol.Properties[i].Location.Other != "") reqProp.RequestData.ClaimControlProperties[0].Location.Other = fnol.Properties[i].Location.Other;

                                                reqProp.RequestData.ClaimControlProperties[0].Location.StateId = (int)fnol.Properties[i].Location.State;
                                                reqProp.RequestData.ClaimControlProperties[0].Location.Zip = fnol.Properties[i].Location.Zip;
                                                reqProp.RequestData.ClaimControlProperties[0].EstimatedAmount = fnol.Properties[i].EstimatedAmount;
                                                if (fnol.Properties[i].Address != null)
                                                    reqProp.RequestData.ClaimControlProperties[0].Location = AddAddress(fnol.Properties[i].Address);
                                            }
                                        }
                                    }

                                    using (DCS.Proxies.ClaimsServiceProxy proxy = new DCS.Proxies.ClaimsServiceProxy())
                                    {
                                        resProp = proxy.SaveClaimControlProperties(reqProp);
                                        responseProperty = resProp;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    SendEmail("save property EXCEPTION! " + fnol.Properties.Count, "", ex.ToString());
                                }
                            }
                        }
                        else
                            SendEmail("fnol NO LOSS TYPE! ", "", "type " + fnol.ClaimType + " veh " + fnol.Vehicles.Count + " prop " + fnol.Properties.Count);
                    }
                    else
                    {
                        foreach (DCO.ValidationItem vItem in response.DiamondValidation.ValidationItems)
                        {
                            // Errors.Add(vItem.Message);
                            returnData.ErrMsg = vItem.Message;
                            SendEmail("ValidationItem", "", vItem.Message);
                            return returnData;
                        }
                    }
                    if (response.ResponseData.ClaimNumber != null)
                    {
                        
                        List<string> AttachmentList = CreateDECFiles(fnol.PolicyNumber);
                        if (AttachmentList != null && AttachmentList.Count > 0)
                        {
                            foreach (var a in AttachmentList)
                            {
                                if (fnol.FNOLAttachements == null)
                                {
                                    fnol.FNOLAttachements = new List<CommonObjects.OMP.FNOLAttachement>();
                                    fnol.FNOLAttachements.Add(new CommonObjects.OMP.FNOLAttachement()
                                    {
                                        FileName = System.IO.Path.GetFileName(a),
                                        FileBytes = System.IO.File.ReadAllBytes(a)
                                    });
                                }
                                else
                                {
                                    fnol.FNOLAttachements.Add(new CommonObjects.OMP.FNOLAttachement()
                                    {
                                        FileName = System.IO.Path.GetFileName(a),
                                        FileBytes = System.IO.File.ReadAllBytes(a)
                                    });
                                }
                            }
                        }
                     InsertFNOL(response.ResponseData.ClaimNumber, returnData.DiamondAdjuster_Id, AttachmentList, returnData.AssignedSuccessfully, ref returnData, fnol, fnol.FNOLType);
                        returnData.CliamNumber = Convert.ToInt32(response.ResponseData.ClaimNumber);
                        if (returnData.AssignedSuccessfully)
                        {
                            var claim = new Claim();
                            returnData.AdjusterEmail = claim.GetAdjusterEmailAddress(returnData.DiamondAdjuster_Id, "");
                            //returnData.AdjusterName =
                        }
                    }
                }
                catch (Exception ex)
                {
                    Errors.Add(ex.ToString());
                    HandleError(ClassName, "Error Submit Claim", ex, null);
                }
            }
            return returnData;//responseAddVehicles,responseProperty);
        }

        public static void SendEmail(string subject, string polnum, string bodyString)
        {
            using (EmailObject em = new EmailObject())
            {
                em.MailHost = System.Configuration.ConfigurationManager.AppSettings["mailhost"];
                em.EmailSubject = subject + " " + polnum;
                em.EmailFromAddress = "diamondClaims@IndianaFarmers.com";
                em.EmailToAddress = System.Configuration.ConfigurationManager.AppSettings["diamondWebClassTO"];
                em.EmailBody = bodyString;
                em.SendEmail();
            }
        }
        public bool SendEmail(ref EmailInfo_Structure_FNOLCA InRec, ref string err, List<string> AttachmentFileList = null)
        {
            ArrayList attachments = new ArrayList();
            System.Net.Mail.Attachment messAtt = null;
            FileInfo info = null;
            string systememail = null;
            string LossType = null;

            try
            {
                using (EmailObject objMail = new EmailObject())
                {
                    // Attach attachment(s) to email
                    if (AttachmentFileList != null)
                    {
                        foreach (string fn in AttachmentFileList)
                        {
                            messAtt = new System.Net.Mail.Attachment(fn);
                            attachments.Add(messAtt);
                        }
                    }
                    if (attachments != null && attachments.Count > 0)
                    {
                        objMail.EmailAttachments = attachments;
                        attachments = null/* TODO Change to default(_) if this is not a reference type */;
                    }
                    messAtt = null;

                    // FROM address - used passed value or default
                    if (InRec.FromAddress_OPTIONAL != null && InRec.FromAddress_OPTIONAL != "")
                        objMail.EmailFromAddress = InRec.FromAddress_OPTIONAL;
                    else
                        objMail.EmailFromAddress = "LossReporting@IndianaFarmers.com";

                    if (InRec.ToAddress != string.Empty)
                        objMail.EmailToAddress = InRec.ToAddress;
                    else
                        throw new Exception("Missing TO email address");

                    if (InRec.CCAddress != null && InRec.CCAddress != string.Empty)
                        objMail.EmailCCAddress = InRec.CCAddress;

                    // If there is a value in the subjectline field, use it, otherwise use the line shown below
                    if (InRec.SubjectLine_OPTIONAL != null && InRec.SubjectLine_OPTIONAL != "")
                        objMail.EmailSubject = InRec.SubjectLine_OPTIONAL;
                    else
                        objMail.EmailSubject = LossType + " for " + InRec.PolicyNumber + " - " + InRec.PolicyHolderFirstName + " " + InRec.PolicyHolderLastName;

                    // Set the body of the email
                    objMail.EmailBody = InRec.Body;

                    // If there is a value in the mailhost field, use it, otherwise use the mail host set in the config file
                    if (InRec.MailHost_OPTIONAL != null && InRec.MailHost_OPTIONAL != "")
                        objMail.MailHost = InRec.MailHost_OPTIONAL;
                    else
                        objMail.MailHost = AppConfig.mailhost;

                    objMail.SendEmail();

                    if (objMail.hasError)

                        throw new Exception("Email transmission to " + objMail.EmailToAddress + " failed: " + objMail.errorMsg);
                }

                return true;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                HandleError(ClassName, "SendMail", ex,null);
                return false;
            }
        }

        public static bool IsInsuredClaimant(CommonObjects.OMP.Claimant claimant)
        {
            bool isInsured = false;

            if (claimant != null && IsInsuredClaimantTypeId(claimant.claimantTypeID) == true)
                isInsured = true;

            return isInsured;
        }
        public static bool IsInsuredClaimantTypeId(int typeId)
        {
            bool isInsured = false;

            if (typeId == insuredClaimantTypeId())
                isInsured = true;

            return isInsured;
        }

        public static int insuredClaimantTypeId()
        {
            return 1; 
        }

        public static bool HasNameInfo(CommonObjects.OMP.Name name)
        {
            bool hasInfo = false;

            if (name != null)
            {
                {
                    if (string.IsNullOrEmpty(name.FirstName) == false || string.IsNullOrEmpty(name.LastName) == false || string.IsNullOrEmpty(name.CommercialName) == false)
                        hasInfo = true;
                }
            }

            return hasInfo;
        }
        public static bool HasAddressInfo(CommonObjects.OMP.Address addr, bool poBoxQualifies = true, bool stateQualifies = true, bool zipQualifies = true)
        {
            bool hasInfo = false;

            if (addr != null)
            {
                {
                    if (string.IsNullOrEmpty(addr.HouseNumber) == false || string.IsNullOrEmpty(addr.StreetName) == false 
                        || (poBoxQualifies == true && string.IsNullOrEmpty(addr.PoBox) == false) || string.IsNullOrEmpty(addr.City) == false 
                        || (stateQualifies == true && string.IsNullOrEmpty(addr.StateId.ToString()) == false) 
                        || (zipQualifies == true && string.IsNullOrEmpty(addr.Zip.ToString()) == false))
                        hasInfo = true;
                }
            }

            return hasInfo;
        }
        public static bool HasFullAddressInfo(CommonObjects.OMP.Address addr, bool poBoxQualifies = true)
        {
            bool hasInfo = false;

            if (addr != null)
            {
                {
                    if (((string.IsNullOrEmpty(addr.HouseNumber) == false && string.IsNullOrEmpty(addr.StreetName) == false) 
                        || (poBoxQualifies == true && string.IsNullOrEmpty(addr.PoBox) == false)) && string.IsNullOrEmpty(addr.City) == false 
                        && string.IsNullOrEmpty(addr.StateId.ToString()) == false  && string.IsNullOrEmpty(addr.Zip.ToString()) == false)
                        hasInfo = true;
                }
            }

            return hasInfo;
        }
        private static (DCO.Name,DCO.Address, DCO.InsCollection<DCO.Phone>, DCO.InsCollection<DCO.Email>) AddPerson(CommonObjects.OMP.Person person)//, ref DCO.Name diaName, ref DCO.Address diaAddress, ref DCO.InsCollection<DCO.Phone> diaPhones, ref DCO.InsCollection<DCO.Email> diaEmails)
        {
              DCO.Name name = new DCO.Name();
                DCO.Address address = new DCO.Address();
                DCO.InsCollection<DCO.Phone> phone = new DCO.InsCollection<DCO.Phone>();
                DCO.InsCollection<DCO.Email> email = new DCO.InsCollection<DCO.Email>();
            if (person is object)
            {
                name = AddName(person.Name);
                address=AddAddress(person.Address);
                //foreach (var ph in person.Phone)
                //{
                    phone = AddPhones(person.Phone);
                //}
                //foreach (var em in person.Email)
                    email = AddEmails(person.Email);
            }
            return (name, address, phone, email);
        }
        private static DCO.Name AddName(CommonObjects.OMP.Name name)
        {
           DCO.Name diaName = new DCO.Name();
            if (name != null)
            { 
                if (name.FirstName != "" || name.LastName != "" || name.CommercialName != "")
                {
                      
                    diaName.FirstName = name.FirstName;
                    diaName.MiddleName = name.MiddleName;
                    diaName.LastName = name.LastName;
                    diaName.CommercialName1 = name.CommercialName;
                    diaName.DoingBusinessAs = name.DbaName;

                        if (string.IsNullOrWhiteSpace(diaName.LastName) == true && 
                            (string.IsNullOrWhiteSpace(diaName.CommercialName1) == false || string.IsNullOrWhiteSpace(diaName.DoingBusinessAs) == false))
                        {
                            diaName.TypeId = 2; // Commercial
                        }
                        else
                        {
                            diaName.TypeId = 1; // Personal Name
                        }
                    
                }
            }
            return diaName;
        }
        private static DCO.Address AddAddress(CommonObjects.OMP.Address address)
        {
            DCO.Address diaAddress = new DCO.Address();
            if (address != null)
            {
                diaAddress.HouseNumber = address.HouseNumber;
                diaAddress.StreetName = address.StreetName;
                diaAddress.City = address.City;
                if (address.StateId >0)
                    diaAddress.StateId = address.StateId;
                diaAddress.Zip = address.Zip;
                diaAddress.County = address.County;
                diaAddress.Other = address.AddressOther;
                diaAddress.POBox = address.PoBox;
                diaAddress.ApartmentNumber = address.AptNum;
                   
                
            }
            return diaAddress;
        }
        private static DCO.InsCollection<DCO.Phone> AddPhones(List<CommonObjects.OMP.Phone> phone) 
        {
            DCO.InsCollection<DCO.Phone> diaPhones = new DCO.InsCollection<DCO.Phone>();
            foreach (var ph in phone)
            {
                if (ph != null)
                {
                    DCO.Phone p = new DCO.Phone();
                    switch (ph.TypeId)
                    {
                            case 1:
                                p.TypeId = DCE.PhoneType.Home;
                                p.Number = ph.Number;
                                break;
                            case 2:
                                p.TypeId = DCE.PhoneType.Business;
                                p.Number = ph.Number;
                                break;
                            case 3:
                                p.TypeId = DCE.PhoneType.Fax;
                                p.Number = ph.Number;
                                break;
                            case 4:
                                p.TypeId = DCE.PhoneType.Cellular;
                                p.Number = ph.Number;
                                break;
                        }
                    diaPhones.Add(p);
                }
            }
            return diaPhones;
        }

        private static DCO.InsCollection<DCO.Email> AddEmails(List<CommonObjects.OMP.Email> email) 
        {
            DCO.InsCollection<DCO.Email> diaEmails = new DCO.InsCollection<DCO.Email>();
            if (email != null)
            {
                foreach (var em in email)
                {
                    if (em != null)
                    {

                        DCO.Email e = new DCO.Email();
                        switch (em.TypeID)
                        {
                            case 1:
                                e.TypeId = DCE.EMailType.Other;
                                e.Address = em.EmailId;
                                break;
                            case 2:
                                e.TypeId = DCE.EMailType.Home;
                                e.Address = em.EmailId;
                                break;
                            case 3:
                                e.TypeId = DCE.EMailType.Business;
                                e.Address = em.EmailId;
                                break;
                        }

                        diaEmails.Add(e);
                    }
                }
            }
            return diaEmails;
        }
        public static bool DiamondClaimsFNOL_OmitInsuredClaimants()
        {
            bool omit = false;

            if (AppConfig.DiamondClaimsFNOL_OmitInsuredClaimants != null && string.IsNullOrWhiteSpace(AppConfig.DiamondClaimsFNOL_OmitInsuredClaimants) == false 
                && AppConfig.DiamondClaimsFNOL_OmitInsuredClaimants.ToUpper() == "YES")
                omit = true;

            return omit;
        }
       
        public static DCO.Claims.LossNotice.ThirdParty GetInsuredThirdPartyFromList(DCO.InsCollection<DCO.Claims.LossNotice.ThirdParty> diaParties,
            CommonObjects.Enums.Enums.Insured1orInsured2 insured1or2 = CommonObjects.Enums.Enums.Insured1orInsured2.Any, 
            bool returnNewAndAddToListIf1or2AndNotFound = true, CommonObjects.Enums.Enums.FirstOrLast firstOrLastItem = CommonObjects.Enums.Enums.FirstOrLast.First)
        {
            DCO.Claims.LossNotice.ThirdParty insParty = null;
            int allCount = 0;
            int unknownCount = 0;
            int insured1Count = 0;
            int insured2Count = 0;
           

            DCO.InsCollection<DCO.Claims.LossNotice.ThirdParty> insParties = GetInsuredThirdPartiesFromList(diaParties, insured1or2, allCount,  
                                                                                unknownCount,  insured1Count,  insured2Count); // sending any here to return all; will filter later
            if (insParties != null && insParties.Count > 0)
            {
                if (firstOrLastItem == CommonObjects.Enums.Enums.FirstOrLast.Last && insParties.Count > 1)
                {
                    int v = insParties.Count - 1;
                    insParty = insParties.ElementAt(v);
                }
                else
                    insParty = insParties.ElementAt(0);
            }
            else
                // check for generic
            if (allCount > 0 && (insured1or2 == CommonObjects.Enums.Enums.Insured1orInsured2.Insured1 || 
                insured1or2 == CommonObjects.Enums.Enums.Insured1orInsured2.Insured2) && unknownCount > 0)
            {
                // have to assume insured1 will be used before insured2 and you'll never have insured2 w/o insured1
                if (insured1or2 == CommonObjects.Enums.Enums.Insured1orInsured2.Insured2)
                {
                    // insured2
                    if (insured1Count > 0)
                    {
                        // can take 1st unknown since insured1 is already set
                        insParties = GetInsuredThirdPartiesFromList(diaParties, CommonObjects.Enums.Enums.Insured1orInsured2.UnknownOnly);
                        if (insParties != null && insParties.Count > 0)
                        {
                            insParty = insParties.ElementAt(0);
                            // now update to specific
                            insParty.Insured2 = true;
                            insParty.Insured1 = false; 
                            insParty.RelationshipId = 5;
                        }
                    }
                    else if (insured1Count == 0 & unknownCount > 1)
                    {
                        // should take 2nd or last unknown since 1st unknown should be insured1
                        insParties = GetInsuredThirdPartiesFromList(diaParties, CommonObjects.Enums.Enums.Insured1orInsured2.UnknownOnly);
                        if (insParties != null && insParties.Count > 1)
                        {
                            insParty = insParties.ElementAt(1); 
                            insParty.Insured2 = true;
                            insParty.Insured1 = false; 
                            insParty.RelationshipId = 5; 
                        }
                    }
                }
                else
                {
                    // insured1
                    // should be able to take 1st unknown regardless of insured2 already being there or not, though insured2 shouldn't be there... or shouldn't be used until after insured1
                    insParties = GetInsuredThirdPartiesFromList(diaParties, CommonObjects.Enums.Enums.Insured1orInsured2.UnknownOnly);
                    if (insParties != null && insParties.Count > 0)
                    {
                        insParty = insParties.ElementAt(0);
                        insParty.Insured1 = true;
                        insParty.Insured2 = false; 
                        insParty.RelationshipId = 8;
                    }
                }
            }

            if (returnNewAndAddToListIf1or2AndNotFound == true && insParty == null && 
                (insured1or2 == CommonObjects.Enums.Enums.Insured1orInsured2.Insured1 || insured1or2 == CommonObjects.Enums.Enums.Insured1orInsured2.Insured2))
            {
                insParty = new DCO.Claims.LossNotice.ThirdParty();

                insParty.ClaimantTypeId = insuredClaimantTypeId(); 
                if (insured1or2 == CommonObjects.Enums.Enums.Insured1orInsured2.Insured2)
                {
                    insParty.Insured2 = true;
                    insParty.Insured1 = false; 
                    insParty.RelationshipId = 5;
                }
                else
                {
                    insParty.Insured1 = true;
                    insParty.Insured2 = false; 
                    insParty.RelationshipId = 8; 
                }
                if (diaParties == null)
                    diaParties = new DCO.InsCollection<DCO.Claims.LossNotice.ThirdParty>();
                diaParties.Add(insParty);
            }

            return insParty;
        }
        public static DCO.InsCollection<DCO.Claims.LossNotice.ThirdParty>  GetInsuredThirdPartiesFromList(DCO.InsCollection<DCO.Claims.LossNotice.ThirdParty> diaParties, CommonObjects.Enums.Enums.Insured1orInsured2 insured1or2 = CommonObjects.Enums.Enums.Insured1orInsured2.Any, int allCount=0, int unknownCount=0, int insured1Count=0, int insured2Count=0)
            {
                DCO.InsCollection<DCO.Claims.LossNotice.ThirdParty> insParties = null/* TODO Change to default(_) if this is not a reference type */;
                allCount = 0;
                unknownCount = 0;
                insured1Count = 0;
                insured2Count = 0;

                if (diaParties != null && diaParties.Count > 0)
                {
                    foreach (DCO.Claims.LossNotice.ThirdParty diaParty in diaParties)
                    {
                         if (diaParty != null && IsInsuredClaimantTypeId(diaParty.ClaimantTypeId) == true)
                        {
                            bool okayToInclude = false;
                            allCount += 1;
                            if (insured1or2 == CommonObjects.Enums.Enums.Insured1orInsured2.Any)
                                okayToInclude = true;
                            switch (diaParty.RelationshipId)
                            {
                                case 8 // Policyholder
                               :
                                    {
                                        insured1Count += 1;
                                        diaParty.Insured1 = true;
                                        diaParty.Insured2 = false;
                                        if (insured1or2 == CommonObjects.Enums.Enums.Insured1orInsured2.Insured1)
                                            okayToInclude = true;
                                        break;
                                    }

                                case 5 // Policyholder #2
                         :
                                    {
                                        insured2Count += 1;
                                        diaParty.Insured2 = true;
                                        diaParty.Insured1 = false;
                                        if (insured1or2 == CommonObjects.Enums.Enums.Insured1orInsured2.Insured2)
                                            okayToInclude = true;
                                        break;
                                    }

                                default:
                                    {
                                        if (diaParty.Insured1 == true && diaParty.Insured2 == false)
                                        {
                                            insured1Count += 1;
                                            if (insured1or2 == CommonObjects.Enums.Enums.Insured1orInsured2.Insured1)
                                                okayToInclude = true;
                                            diaParty.RelationshipId = 8; // Policyholder
                                        }
                                        else if (diaParty.Insured2 == true && diaParty.Insured1 == false)
                                        {
                                            insured2Count += 1;
                                            if (insured1or2 == CommonObjects.Enums.Enums.Insured1orInsured2.Insured2)
                                                okayToInclude = true;
                                           diaParty.RelationshipId = 5; // Policyholder #2
                                        }
                                        else
                                        {
                                            unknownCount += 1;
                                            if (insured1or2 == CommonObjects.Enums.Enums.Insured1orInsured2.UnknownOnly)
                                                okayToInclude = true;
                                        }

                                        break;
                                    }
                            }
                            if (okayToInclude == true)
                            {
                                if (insParties == null)
                                    insParties = new DCO.InsCollection<DCO.Claims.LossNotice.ThirdParty>();
                                insParties.Add(diaParty);
                            }
                        }
                    }
                }

                return insParties;
            }

        public CommonObjects.Enums.Enums.FNOLClaimLossType ClaimLossTypeForSelectedValue(int losstype,string selectedValue,  CommonObjects.Enums.Enums.FNOL_Type fnolType, string packagePartType,string polNum = "")
        {
            CommonObjects.Enums.Enums.FNOLClaimLossType lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.NotAvailable;

            int intForString = IntegerForString(selectedValue);
            if (intForString >0)
            {
                // note: this logic will check against 2 diff enums, though anything from ClaimLossType_All should now be in ClaimLossType, so if it's valid in ClaimLossType_All, shouldn't really need to check against ClaimLossType
                bool looksValid = false;
                bool definitelyValid = false;
                if (System.Enum.IsDefined(typeof(CommonObjects.Enums.Enums.FNOLClaimLossType), intForString) == true)
                    looksValid = true;
                if (System.Enum.IsDefined(typeof(CommonObjects.Enums.Enums.ClaimLossType_AllValid), intForString) == true)
                    definitelyValid = true;

                CommonObjects.Enums.Enums.PolicyLobType lobType = ApplicableLobTypeForCriteria(fnolType, polNum,packagePartType);

                if (looksValid == true)
                {
                    if (definitelyValid == true)
                    {
                        // set it
                        lossType = (CommonObjects.Enums.Enums.FNOLClaimLossType)intForString;
                        ValidateLossType(lossType, lobType: lobType);
                    }
                    else
                    {
                        // try to find alternate value in ClaimLossType that's also in ClaimLossType
                        CommonObjects.Enums.Enums.FNOLClaimLossType tempLossType = (CommonObjects.Enums.Enums.FNOLClaimLossType)intForString;
                      
                        lossType = ReplacementLossType(tempLossType, lobType: lobType);
                    }
                }
                else if (definitelyValid == true)
                {
                    // this shouldn't ever happen if looksValid is false, but... set it anyway
                    lossType = (CommonObjects.Enums.Enums.FNOLClaimLossType)intForString;

                    // double-check a few values
                    ValidateLossType(lossType, lobType: lobType);
                }
                else
                                // appears to be invalid but positive #; check to see if it's a valid id in Diamond's table
                                if (System.Enum.IsDefined(typeof(CommonObjects.Enums.Enums.ClaimLossType_All), intForString) == true)
                    lossType = (CommonObjects.Enums.Enums.FNOLClaimLossType)intForString;
            }

            return lossType;
        }
        public CommonObjects.Enums.Enums.PolicyLobType ApplicableLobTypeForCriteria(CommonObjects.Enums.Enums.FNOL_Type fnolType, string polNum = "", string packagePartType = "")
        {
            CommonObjects.Enums.Enums.PolicyLobType lobType = LobTypeForPolicyNumber(polNum);

            if (lobType == CommonObjects.Enums.Enums.PolicyLobType.CommercialPackage && string.IsNullOrWhiteSpace(packagePartType) == false)
                lobType = LobTypeForPackagePartType(packagePartType, defaultToCPP: true);

            if (lobType == CommonObjects.Enums.Enums.PolicyLobType.CommercialPackage)
            {
                switch (fnolType)
                {
                    case CommonObjects.Enums.Enums.FNOL_Type.AutoFNOL:
                        {
                            lobType = CommonObjects.Enums.Enums.PolicyLobType.CommercialGarage;
                            break;
                        }

                    case CommonObjects.Enums.Enums.FNOL_Type.LiabilityFNOL:
                        {
                            lobType = CommonObjects.Enums.Enums.PolicyLobType.CommercialGeneralLiability;
                            break;
                        }

                    case CommonObjects.Enums.Enums.FNOL_Type.PropertyFNOL:
                        {
                            lobType = CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty;
                            break;
                        }
                }
            }
            else if (lobType == CommonObjects.Enums.Enums.PolicyLobType.NotAssigned && fnolType > 0 )
            {
                switch (fnolType)
                {
                    case CommonObjects.Enums.Enums.FNOL_Type.AutoFNOL:
                        {
                            lobType = CommonObjects.Enums.Enums.PolicyLobType.AutoPersonal;
                            break;
                        }

                    case CommonObjects.Enums.Enums.FNOL_Type.LiabilityFNOL:
                        {
                            lobType = CommonObjects.Enums.Enums.PolicyLobType.CommercialGeneralLiability;
                            break;
                        }

                    case CommonObjects.Enums.Enums.FNOL_Type.PropertyFNOL:
                        {
                            lobType = CommonObjects.Enums.Enums.PolicyLobType.HomePersonal;
                            break;
                        }
                }
            }

            return lobType;
        }
        public CommonObjects.Enums.Enums.PolicyLobType LobTypeForPolicyNumber(string polNum)
        {
            CommonObjects.Enums.Enums.PolicyLobType lobType = CommonObjects.Enums.Enums.PolicyLobType.NotAssigned;

            if (string.IsNullOrWhiteSpace(polNum) == false)
            {
                string polNumToUse =  polNum.Replace("Q", "");
                if (polNumToUse.Length >= 3)
                {
                    switch (polNumToUse.Substring(0,3))
                    {
                        case "PPA":
                            {
                                lobType = CommonObjects.Enums.Enums.PolicyLobType.AutoPersonal;
                                break;
                            }

                        case "CAP":
                            {
                                lobType = CommonObjects.Enums.Enums.PolicyLobType.CommercialAuto;
                                break;
                            }

                        case "BOP":
                            {
                                lobType = CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP;
                                break;
                            }

                        case "CRM":
                            {
                                lobType = CommonObjects.Enums.Enums.PolicyLobType.CommercialCrime;
                                break;
                            }

                        case "GAR":
                            {
                                lobType = CommonObjects.Enums.Enums.PolicyLobType.CommercialGarage;
                                break;
                            }

                        case "CGL":
                            {
                                lobType = CommonObjects.Enums.Enums.PolicyLobType.CommercialGeneralLiability;
                                break;
                            }

                        case "CIM":
                            {
                                lobType = CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine;
                                break;
                            }

                        case "CPP":
                            {
                                lobType = CommonObjects.Enums.Enums.PolicyLobType.CommercialPackage;
                                break;
                            }

                        case "CPR":
                            {
                                lobType = CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty;
                                break;
                            }

                        case "CUP":
                            {
                                lobType = CommonObjects.Enums.Enums.PolicyLobType.CommercialUmbrella;
                                break;
                            }

                        case "DFR":
                            {
                                lobType = CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal;
                                break;
                            }

                        case "FAR":
                            {
                                lobType = CommonObjects.Enums.Enums.PolicyLobType.Farm;
                                break;
                            }

                        case "HOM":
                            {
                                lobType = CommonObjects.Enums.Enums.PolicyLobType.HomePersonal;
                                break;
                            }

                        case "PIM":
                            {
                                lobType = CommonObjects.Enums.Enums.PolicyLobType.InlandMarinePersonal;
                                break;
                            }

                        case "PUP":
                        case "FUP":
                            {
                                lobType = CommonObjects.Enums.Enums.PolicyLobType.UmbrellaPersonal;
                                break;
                            }

                        case "WCP":
                            {
                                lobType = CommonObjects.Enums.Enums.PolicyLobType.WorkersComp;
                                break;
                            }
                    }
                }
            }

            return lobType;
        }

        public CommonObjects.Enums.Enums.PolicyLobType LobTypeForPackagePartType(string packagePartType, bool defaultToCPP = true)
        {
            CommonObjects.Enums.Enums.PolicyLobType lobType = CommonObjects.Enums.Enums.PolicyLobType.NotAssigned;

            if (defaultToCPP == true)
                lobType = CommonObjects.Enums.Enums.PolicyLobType.CommercialPackage;

            if (string.IsNullOrWhiteSpace(packagePartType) == false)
            {
                switch (packagePartType)
                {
                    case "PROPERTY":
                        {
                            lobType = CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty;
                            break;
                        }

                    case "GENERAL LIABILITY":
                        {
                            lobType = CommonObjects.Enums.Enums.PolicyLobType.CommercialGeneralLiability;
                            break;
                        }

                    case "INLAND MARINE":
                        {
                            lobType = CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine;
                            break;
                        }

                    case "CRIME":
                        {
                            lobType = CommonObjects.Enums.Enums.PolicyLobType.CommercialCrime;
                            break;
                        }

                    case "GARAGE":
                        {
                            lobType = CommonObjects.Enums.Enums.PolicyLobType.CommercialGarage;
                            break;
                        }
                }
            }

            return lobType;
        }

        public int IntegerForString(string str) 
        {
            int number;
            bool success = int.TryParse(str, out number);
            if (success)
            {
                return number;
            }
            //if (str != null && str != "" && IsNumeric(str) == true)
            //        return System.Convert.ToInt32(str);
            else
                return 0;
        }
        public void ValidateLossType(CommonObjects.Enums.Enums.FNOLClaimLossType lossType, CommonObjects.Enums.Enums.PolicyLobType lobType = CommonObjects.Enums.Enums.PolicyLobType.NotAssigned)
        {
            switch (lossType)
            {
                case CommonObjects.Enums.Enums.FNOLClaimLossType.Collapse:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.Collapse2:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.HomePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.Collapse;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.Collapse2;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.DebrisRemoval:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.DebrisRemoval2:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.HomePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.DebrisRemoval;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.DebrisRemoval2;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.Earthquake:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.Earthquake2:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty:
                            case CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.HomePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.Earthquake;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.AutoPersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialAuto:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialGarage:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.Earthquake2;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.EmployersLiability:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.EmployersLiability2:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.EmployersLiability;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.WorkersComp:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.EmployersLiability2;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.FallingObjects:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.FallingObjects2:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.HomePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.FallingObjects;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.FallingObjects2;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.FalseAlarm:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.FalseAlarm2:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.HomePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.FalseAlarm;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.FalseAlarm2;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.Fire:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.Fire2:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty:
                            case CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.HomePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.InlandMarinePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.Fire;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.AutoPersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialAuto:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialGarage:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.Fire2;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.Hail:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.Hail2:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty:
                            case CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.HomePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.Hail;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.AutoPersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialAuto:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialGarage:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.Hail2;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityDamageToPropertyOfOthers:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityDamageToPropertyOfOthers2:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialGarage:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialGeneralLiability:
                            case CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.HomePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityDamageToPropertyOfOthers;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityDamageToPropertyOfOthers2;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityPersonalInjury:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityPersonalInjury2:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialGarage:
                            case CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.HomePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityPersonalInjury;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialGeneralLiability:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityPersonalInjury2;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.OtherNOC:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.OtherNOC2:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.OtherNOC3:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.HomePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.InlandMarinePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.OtherNOC;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.AutoPersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialAuto:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialGarage:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.OtherNOC2;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialCrime:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.OtherNOC3;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.RoadbedCollision:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.RoadbedCollision2:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.RoadbedCollision;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.RoadbedCollision2;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.WeightOfIceSnowOrSleet:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.WeightOfIceSnowOrSleet2:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.HomePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.WeightOfIceSnowOrSleet;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.WeightOfIceSnowOrSleet2;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.AnimalDamage:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.AnimalDamage2:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.AnimalDamage3:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.AnimalDamage3;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.HomePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.InlandMarinePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.AnimalDamage2;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.CollisionUpsetOverturn:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.CollisionUpsetOrOverturn:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.CollisionUpsetOrOverturn2:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.CollisionUpsetOrOverturn2;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.InlandMarinePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.CollisionUpsetOrOverturn;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.EquipmentBreakdown:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.EquipmentBreakdown2:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.EquipmentBreakdown3:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.EquipmentBreakdown3;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty:
                            case CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.HomePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.EquipmentBreakdown2;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.Explosion:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.Explosion2:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.Explosion3:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.AutoPersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialAuto:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialGarage:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.Explosion3;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty:
                            case CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.HomePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.Explosion2;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.GlassBreakage:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.GlassBreakage2:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.GlassBreakage3:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.GlassBreakage3;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty:
                            case CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.HomePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.GlassBreakage2;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.DogBiteLiability:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityDogBite:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityPetBite:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityPetBite2:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialGeneralLiability:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityPetBite2;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialGarage:
                            case CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.HomePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityPetBite;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.MineSubsidence:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.MineSubsidence2:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.MineSubsidence3:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.MineSubsidence3;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty:
                            case CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.HomePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.MineSubsidence2;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.PowerInterruptionOffPremise:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.PowerInterruptionOnPremise:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.PowerInterruption:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.PowerInterruption2:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.PowerInterruption2;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty:
                            case CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.HomePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.PowerInterruption;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.RiotOrCivilCommotion:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.RiotOrCivilCommotion2:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.RiotOrCivilCommotion3:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.AutoPersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialAuto:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialGarage:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.RiotOrCivilCommotion3;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine:
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty:
                            case CommonObjects.Enums.Enums.PolicyLobType.DwellingFirePersonal:
                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                            case CommonObjects.Enums.Enums.PolicyLobType.HomePersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.RiotOrCivilCommotion2;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.WorkersCompensation:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.WorkersCompensation2:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.WorkersCompensation3:
                case CommonObjects.Enums.Enums.FNOLClaimLossType.WorkComp:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.WorkersComp:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.WorkComp;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.WorkersCompensation2;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialUmbrella:
                            case CommonObjects.Enums.Enums.PolicyLobType.UmbrellaPersonal:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.WorkersCompensation3;
                                    break;
                                }
                        }

                        break;
                    }
            }
        }
        public CommonObjects.Enums.Enums.FNOLClaimLossType ReplacementLossType(CommonObjects.Enums.Enums.FNOLClaimLossType invalidLossType, CommonObjects.Enums.Enums.PolicyLobType lobType = CommonObjects.Enums.Enums.PolicyLobType.NotAssigned)
        {
            CommonObjects.Enums.Enums.FNOLClaimLossType lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.NotAvailable;

            switch (invalidLossType)
            {
                case CommonObjects.Enums.Enums.FNOLClaimLossType.AircraftDamage:
                    {
                        lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.Aircraft;
                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.AnimalDamage:
                    {
                        switch (lobType)
                        {
                            case   CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                            case   CommonObjects.Enums.Enums.PolicyLobType.CommercialInlandMarine:
                            case   CommonObjects.Enums.Enums.PolicyLobType.CommercialProperty:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.AnimalDamage3;
                                    break;
                                }

                            default:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.AnimalDamage2;
                                    break;
                                }
                        }

                        break;
                    }

                case CommonObjects.Enums.Enums.FNOLClaimLossType.CollisionUpsetOverturn:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.CollisionUpsetOrOverturn2;
                                    break;
                                }

                            default:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.CollisionUpsetOrOverturn;
                                    break;
                                }
                        }

                        break;
                    }

                case  CommonObjects.Enums.Enums.FNOLClaimLossType.EquipmentBreakdown:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.EquipmentBreakdown3;
                                    break;
                                }

                            default:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.EquipmentBreakdown2;
                                    break;
                                }
                        }

                        break;
                    }

                case  CommonObjects.Enums.Enums.FNOLClaimLossType.Explosion:
                    {
                        switch (lobType)
                        {
                            case  CommonObjects.Enums.Enums.PolicyLobType.AutoPersonal:
                            case  CommonObjects.Enums.Enums.PolicyLobType.CommercialAuto:
                            case  CommonObjects.Enums.Enums.PolicyLobType.CommercialGarage:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.Explosion3;
                                    break;
                                }

                            default:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.Explosion2;
                                    break;
                                }
                        }

                        break;
                    }

                case  CommonObjects.Enums.Enums.FNOLClaimLossType.Flood:
                    {
                        lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.Flood2;
                        break;
                    }

                case  CommonObjects.Enums.Enums.FNOLClaimLossType.GlassBreakage:
                    {
                        switch (lobType)
                        {
                            case CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.GlassBreakage3;
                                    break;
                                }

                            default:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.GlassBreakage2;
                                    break;
                                }
                        }

                        break;
                    }

                case  CommonObjects.Enums.Enums.FNOLClaimLossType.IdentityTheft:
                    {
                        lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.IdentityTheft2;
                        break;
                    }

                case  CommonObjects.Enums.Enums.FNOLClaimLossType.IntakeofForeignObject:
                    {
                        lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.IntakeOfForeignObjects;
                        break;
                    }

                case  CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityAnimalOrLivestock:
                    {
                        lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityAnimalOrLivestock2;
                        break;
                    }

                case  CommonObjects.Enums.Enums.FNOLClaimLossType.DogBiteLiability:
                case  CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityDogBite:
                    {
                        switch (lobType)
                        {
                            case  CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                            case  CommonObjects.Enums.Enums.PolicyLobType.CommercialGeneralLiability:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityPetBite2;
                                    break;
                                }

                            default:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityPetBite;
                                    break;
                                }
                        }

                        break;
                    }

                case  CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityEnvironmental:
                case  CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityEnvironmentalMiscellaneous:
                case  CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityEnvironmentalMold:
                case  CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityEnvironmentalPetroleum:
                case  CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityEnvironmentalSolidWaste:
                    {
                        lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilityEnvironmental2;
                        break;
                    }

                case   CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilitySlipandFall:
                    {
                        lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.LiabilitySlipAndFall2;
                        break;
                    }

                case   CommonObjects.Enums.Enums.FNOLClaimLossType.MineSubsidence:
                    {
                        switch (lobType)
                        {
                            case   CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.MineSubsidence3;
                                    break;
                                }

                            default:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.MineSubsidence2;
                                    break;
                                }
                        }

                        break;
                    }

                case   CommonObjects.Enums.Enums.FNOLClaimLossType.MysteriousDisappearanceInvolvingScheduledProperty:
                    {
                        lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.MysteriousDisappearance;
                        break;
                    }

                case   CommonObjects.Enums.Enums.FNOLClaimLossType.PowerInterruptionOffPremise:
                case   CommonObjects.Enums.Enums.FNOLClaimLossType.PowerInterruptionOnPremise:
                    {
                        switch (lobType)
                        {
                            case   CommonObjects.Enums.Enums.PolicyLobType.CommercialBOP:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.PowerInterruption2;
                                    break;
                                }

                            default:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.PowerInterruption;
                                    break;
                                }
                        }

                        break;
                    }

                case   CommonObjects.Enums.Enums.FNOLClaimLossType.RiotOrCivilCommotion:
                    {
                        switch (lobType)
                        {
                            case   CommonObjects.Enums.Enums.PolicyLobType.AutoPersonal:
                            case   CommonObjects.Enums.Enums.PolicyLobType.CommercialAuto:
                            case   CommonObjects.Enums.Enums.PolicyLobType.CommercialGarage:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.RiotOrCivilCommotion3;
                                    break;
                                }

                            default:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.RiotOrCivilCommotion2;
                                    break;
                                }
                        }

                        break;
                    }

                case   CommonObjects.Enums.Enums.FNOLClaimLossType.TheftAutoPAPDOrOTC:
                case   CommonObjects.Enums.Enums.FNOLClaimLossType.TheftInvolvingScheduledProperty:
                case   CommonObjects.Enums.Enums.FNOLClaimLossType.TheftofMoney:
                case   CommonObjects.Enums.Enums.FNOLClaimLossType.TheftofMoneyandSecurities:
                case   CommonObjects.Enums.Enums.FNOLClaimLossType.TheftOrBurglary:
                case   CommonObjects.Enums.Enums.FNOLClaimLossType.TheftOther:
                    {
                        switch (lobType)
                        {
                            case   CommonObjects.Enums.Enums.PolicyLobType.AutoPersonal:
                            case   CommonObjects.Enums.Enums.PolicyLobType.CommercialAuto:
                            case   CommonObjects.Enums.Enums.PolicyLobType.CommercialGarage:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.TheftPartsOrContents;
                                    break;
                                }

                            default:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.Theft;
                                    break;
                                }
                        }

                        break;
                    }

                case   CommonObjects.Enums.Enums.FNOLClaimLossType.VehicleDamage:
                    {
                        lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.VehicleDamage2;
                        break;
                    }

                case   CommonObjects.Enums.Enums.FNOLClaimLossType.WaterDamage:
                    {
                        lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.WaterDamageOther;
                        break;
                    }

                case   CommonObjects.Enums.Enums.FNOLClaimLossType.WeatherRelatedWaterDamage:
                    {
                        lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.WaterDamageWindDriven;
                        break;
                    }

                case   CommonObjects.Enums.Enums.FNOLClaimLossType.WorkersCompensation:
                    {
                        switch (lobType)
                        {
                            case   CommonObjects.Enums.Enums.PolicyLobType.WorkersComp:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.WorkComp;
                                    break;
                                }

                            case CommonObjects.Enums.Enums.PolicyLobType.Farm:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.WorkersCompensation2;
                                    break;
                                }

                            default:
                                {
                                    lossType = CommonObjects.Enums.Enums.FNOLClaimLossType.WorkersCompensation3;
                                    break;
                                }
                        }

                        break;
                    }

                default:
                    {
                        // just use it
                        lossType = invalidLossType;
                        break;
                    }
            }

            return lossType;
        }
        public static bool InsertFNOL(string DiamondClaimNumber, string ClaimPersonnel_Id, List<string> FileList, bool ClaimWasAutoAssigned,ref FNOLResponseData AutoAssignInfo, DataServicesCore.CommonObjects.OMP.FNOL fnol, DataServicesCore.CommonObjects.Enums.Enums.FNOL_LOB_Enum FNOLType)
        {
            int rtn = -1;
            SqlTransaction txn = null/* TODO Change to default(_) if this is not a reference type */;
            object FNOLId = null;
            //int FNOLId;
            string FNOLTypeId = null;
            string PersonTypeID = null;
            // Dim FileList As Array = Nothing
            try {
                 using ( System.Data.SqlClient.SqlConnection connFNOL = new System.Data.SqlClient.SqlConnection(AppConfig.ConnFNOL))
                {
                      connFNOL.Open();
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    txn = connFNOL.BeginTransaction();
                    cmd.Connection = connFNOL;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "sp_Insert_tbl_FNOL";
                    cmd.Transaction = txn;
                        switch (fnol.FNOLType)
                        {
                            case CommonObjects.Enums.Enums.FNOL_LOB_Enum.Auto:
                                FNOLTypeId = "1";
                                break;
                            case CommonObjects.Enums.Enums.FNOL_LOB_Enum.CommercialAuto:
                                FNOLTypeId = "2";
                                break;
                            case CommonObjects.Enums.Enums.FNOL_LOB_Enum.Liability:
                                FNOLTypeId = "3";
                                break;
                            case CommonObjects.Enums.Enums.FNOL_LOB_Enum.Property:
                                FNOLTypeId = "4";
                                break;
                        }
                        cmd.Parameters.AddWithValue("@FNOLType_Id", FNOLTypeId);
                        if (DiamondClaimNumber.HasValue())
                        cmd.Parameters.AddWithValue("@DiamondClaimNumber", DiamondClaimNumber);
                        cmd.Parameters.AddWithValue("@Assigned", 0);
                        cmd.Parameters.AddWithValue("@EntryDate", DateTime.Now);
                        if (fnol.EntryUser.HasValue()) cmd.Parameters.AddWithValue("@EntryUser", fnol.EntryUser);//FNOLCommon.GetDiamondClaimEntryUser(this));
                        if (fnol.PolicyNumber.HasValue()) cmd.Parameters.AddWithValue("@PolicyNumber", fnol.PolicyNumber);
                        cmd.Parameters.AddWithValue("@LossType", fnol.ClaimLossType.ToString());
                        cmd.Parameters.AddWithValue("@ClaimLossType_Id", fnol.ClaimLossType);
                        if (fnol.ReportedDate != null) cmd.Parameters.AddWithValue("@ReportDate", "");//fnol.ReportedDate.ToShortDateString());
                        if (fnol.LossDate != null) cmd.Parameters.AddWithValue("@LossDate", fnol.LossDate.ToShortDateString());
                        if (fnol.AgencyName != null) cmd.Parameters.AddWithValue("@AgencyName", fnol.AgencyName);
                        if (fnol. AgencyPhone!= null) cmd.Parameters.AddWithValue("@AgencyPhone", fnol.AgencyPhone);
                        if (fnol.AgencyFax != null) cmd.Parameters.AddWithValue("@AgencyFAX", fnol.AgencyFax);
                        cmd.Parameters.AddWithValue("@DeductibleAmount", ""); //DB VALUES (NULL,SEE POLICY)
                        cmd.Parameters.AddWithValue("@PackagePart",fnol.packagePartType);
                      
                        if (fnol.OtherContactInfo != null && fnol.OtherContactInfo.Name != null)
                        {
                            cmd.Parameters.AddWithValue("@PrimaryContact", fnol.OtherContactInfo.Name.FirstName + " " + fnol.OtherContactInfo.Name.LastName);
                        }
                        else if (fnol.Insured.Name.FirstName.HasValue())
                            cmd.Parameters.AddWithValue("PrimaryContact", fnol.Insured.Name.FirstName + " " + fnol.Insured.Name.LastName);
                        else
                        cmd.Parameters.AddWithValue("PrimaryContact", "");
                        if (fnol.Insured.Name.FirstName.HasValue())
                            cmd.Parameters.AddWithValue("@InsuredFirst", fnol.Insured.Name.FirstName);
                        else
                            cmd.Parameters.AddWithValue("@InsuredFirst", "");
                        if (fnol.Insured.Name.LastName.HasValue())
                            cmd.Parameters.AddWithValue("@InsuredLast", fnol.Insured.Name.LastName);
                        else if (fnol.Insured.Name.CommercialName.HasValue())
                            cmd.Parameters.AddWithValue("@InsuredLast", fnol.Insured.Name.CommercialName);
                        else
                            cmd.Parameters.AddWithValue("@InsuredLast", "");
                        if (fnol.Insured.Phone != null)
                        {
                            foreach (var ph in fnol.Insured.Phone)
                            {

                                switch (ph.TypeId)
                                {
                                    case 1:
                                        cmd.Parameters.AddWithValue("@InsuredHomePhone", ph.Number);
                                        break;
                                    case 2:
                                        cmd.Parameters.AddWithValue("@InsuredBusinessPhone", ph.Number);
                                        break;
                                    case 3:
                                        cmd.Parameters.AddWithValue("@InsuredFAX", ph.Number);
                                        break;
                                    case 4:
                                        cmd.Parameters.AddWithValue("@InsuredCellPhone", ph.Number);
                                        break;
                                }
                            }
                        }
                        if (fnol.Insured.Email != null)
                        {
                             foreach (var e in fnol.Insured.Email)
                            {
                                if (e.EmailId != null)
                                    cmd.Parameters.AddWithValue("@InsuredEmail", e.EmailId);
                            }
                        }
                        if (fnol.Insured.Address != null)
                        {
                            cmd.Parameters.AddWithValue("@InsuredAddress", fnol.Insured.Address.HouseNumber+" " + fnol.Insured.Address.StreetName);

                            if (fnol.Insured.Address.City.HasValue()) cmd.Parameters.AddWithValue("@InsuredCity", fnol.Insured.Address.City);
                            if (fnol.Insured.Address.State.HasValue()) cmd.Parameters.AddWithValue("@InsuredState", fnol.Insured.Address.State);
                            if (fnol.Insured.Address.Zip.HasValue()) cmd.Parameters.AddWithValue("@InsuredZip", fnol.Insured.Address.Zip);
                        }
                        if (fnol.OtherContactInfo != null && fnol.OtherContactInfo.Phone != null)
                        {
                            foreach (var cph in fnol.OtherContactInfo.Phone)
                            {
                                switch (cph.TypeId)
                                {
                                    case 1:
                                        cmd.Parameters.AddWithValue("@ContactHomePhone", cph.Number);
                                        break;

                                    case 2:
                                        cmd.Parameters.AddWithValue("@ContactBusinessPhone", cph.Number);
                                        break;
                                    case 3:
                                        cmd.Parameters.AddWithValue("@ContactFaxPhone", cph.Number);
                                        break;
                                    case 4:
                                        cmd.Parameters.AddWithValue("@ContactCellPhone", cph.Number);
                                        break;
                                }
                            }
                        }
                        if (fnol.VehicleOwner != null)
                        {
                            if (fnol.VehicleOwner.Phone != null)
                            {
                                foreach (var voph in fnol.VehicleOwner.Phone)
                                {
                                    switch (voph.TypeId)
                                    {
                                        case 1:
                                            cmd.Parameters.AddWithValue("@InsVehOwnerHomePhone", voph.Number);
                                            break;
                                        case 2:
                                            cmd.Parameters.AddWithValue("@InsVehOwnerBusinessPhone", voph.Number);
                                            break;
                                    }
                                }
                            }
                            cmd.Parameters.AddWithValue("@InsVehOwnerAddress", fnol.VehicleOwner.Address.HouseNumber + fnol.VehicleOwner.Address.StreetName);
                            cmd.Parameters.AddWithValue("@InsVehOwnerCity", fnol.VehicleOwner.Address.City);
                            cmd.Parameters.AddWithValue("@InsVehOwnerState", fnol.VehicleOwner.Address.State);
                            cmd.Parameters.AddWithValue("@InsVehOwnerZip", fnol.VehicleOwner.Address.Zip);
                            cmd.Parameters.AddWithValue("@InsVehOwnerFirst", fnol.VehicleOwner.Name.FirstName);
                            cmd.Parameters.AddWithValue("@InsVehOwnerMiddle", fnol.VehicleOwner.Name.MiddleName);
                            cmd.Parameters.AddWithValue("@InsVehOwnerLast", fnol.VehicleOwner.Name.LastName);
                        }
                        foreach (var a in fnol.Claimants)
                        {
                            if (a != null && a.claimantTypeID == 7)
                            {
                                cmd.Parameters.AddWithValue("@InsVehDriverFirst", a.Name.FirstName);
                                cmd.Parameters.AddWithValue("@InsVehDriverMiddle", a.Name.MiddleName);
                                cmd.Parameters.AddWithValue("@InsVehDriverLast", a.Name.LastName);
                                if (a.Phone != null)//(fnol.VehicleDriver.Phone != null)
                                {
                                    foreach (var vdph in a.Phone)//fnol.VehicleDriver.Phone)
                                    {
                                        switch (vdph.TypeId)
                                        {
                                            case 1:
                                                cmd.Parameters.AddWithValue("@InsVehDriverHomePhone", vdph.Number);
                                                break;
                                            case 2:
                                                cmd.Parameters.AddWithValue("@InsVehDriverBusinessPhone", vdph.Number);
                                                break;
                                        }
                                    }
                                }
                                if (a.Address != null)
                                {
                                    cmd.Parameters.AddWithValue("@InsVehDriverAddress", a.Address.HouseNumber + a.Address.StreetName);
                                    cmd.Parameters.AddWithValue("@InsVehDriverCity", a.Address.City);
                                    cmd.Parameters.AddWithValue("@InsVehDriverState", a.Address.State);
                                    cmd.Parameters.AddWithValue("@InsVehDriverZip", a.Address.Zip);
                                }
                            }
                           
                        }
                        if (fnol.ClaimLocation != null) cmd.Parameters.AddWithValue("@LossLocation", fnol.ClaimLocation);
                        if (fnol.LossAddress != null)
                        {
                            if (fnol.LossAddress.HouseNumber != null) cmd.Parameters.AddWithValue("@LossAddress", fnol.LossAddress.HouseNumber+" " + fnol.LossAddress.StreetName);
                            if (fnol.LossAddress.City != null) cmd.Parameters.AddWithValue("@LossCity", fnol.LossAddress.City);
                           
                            if (fnol.LossAddress.State != null)
                                cmd.Parameters.AddWithValue("@LossState", fnol.LossAddress.StateAbbrev);
                            if (fnol.LossAddress.Zip != null) cmd.Parameters.AddWithValue("@LossZip", fnol.LossAddress.Zip);
                        }
                        if (fnol.Description != null) cmd.Parameters.AddWithValue("@LossDescription", fnol.Description);
                        cmd.Parameters.AddWithValue("@LossKindCategory", fnol.ClaimLossType.ToString());
                        if (fnol.Vehicles != null)
                        {
                            foreach (var v in fnol.Vehicles)
                            {
                                if(v.Make.HasValue())cmd.Parameters.AddWithValue("@InsVehicleMake", v.Make);
                                if(v.Model.HasValue())cmd.Parameters.AddWithValue("@InsVehicleModel", v.Model);
                                if(v.Year.HasValue())cmd.Parameters.AddWithValue("@InsVehicleYear", v.Year);
                                if (v.VIN.HasValue()) cmd.Parameters.AddWithValue("@InsVehicleVIN", v.VIN);
                                if (v.LicensePlate.HasValue()) cmd.Parameters.AddWithValue("@InsVehiclePlateNumber", v.LicensePlate);
                                if (v.VehicleDamage.HasValue()) cmd.Parameters.AddWithValue("@InsVehicleDamage", v.VehicleDamage); 
                                if(v.VehicleDamage.HasValue())cmd.Parameters.AddWithValue("@InsVehicleDamageAmount", v.VehicleDamageAmt); 
                                //if(v.LocationOfAccidentAddress!= null)cmd.Parameters.AddWithValue("@InsVehicleLocation", (v.LocationOfAccidentAddress.Line1 + v.LocationOfAccidentAddress.Line2)); 
                            }
                        }

                        cmd.Parameters.AddWithValue("@ManualDraft", 0);
                        if (fnol.ReportedBy.HasValue())cmd.Parameters.AddWithValue("@ReportedBy", fnol.ReportedBy);
                        if (fnol.WitnessRemarks.HasValue())cmd.Parameters.AddWithValue("@Comments_AddlInfo", fnol.WitnessRemarks);
                          FNOLId = cmd.ExecuteScalar();
                    int fnolid;
                     
                    if (FNOLId == null || !int.TryParse(Convert.ToString(FNOLId),out fnolid))
                        throw new Exception("Error inserting FNOL record");
                        #region Other Vehicle Drivers
                        foreach (var ovd in fnol.Claimants)
                        {
                            if (ovd != null && ovd.claimantTypeID == 14)
                            {
                                FNOLData_Structure pr = new FNOLData_Structure();
                                if (ovd.Name != null)
                                {
                                    pr.InsuredFirst = ovd.Name.FirstName;
                                    pr.InsuredLast = ovd.Name.LastName;
                                    pr.InsuredMiddle = ovd.Name.MiddleName;
                                }
                                if (ovd.Phone != null)//(fnol.VehicleDriver.Phone != null)
                                {
                                    foreach (var ovdp in ovd.Phone)//fnol.VehicleDriver.Phone)
                                    {
                                        switch (ovdp.TypeId)
                                        {
                                            case 1:
                                                pr.PhoneHome = ovdp.Number;
                                                break;
                                            case 2:
                                                pr.PhoneBusiness = ovdp.Number;
                                                break;
                                            case 3:
                                                pr.PhoneHome = ovdp.Number;
                                                break;
                                            case 4:
                                                pr.PhoneBusiness = ovdp.Number;
                                                break;
                                        }
                                    }
                                }
                                if (ovd.Address != null)
                                {
                                    pr.Address = ovd.Address.HouseNumber + ovd.Address.StreetName;
                                    pr.InsuredAddressCity = ovd.Address.City;
                                    pr.InsuredAddressState = ovd.Address.State;
                                    pr.InsuredAddressZip = ovd.Address.Zip;
                                }
                                if (!InsertPersonsRecord(pr, FNOLId.ToString(), (int)CommonObjects.Enums.Enums.PersonType_Enum.OtherVehicleDriver, fnol.PolicyNumber, connFNOL, txn))
                                    throw new Exception("Error adding other vehicle driver record");
                            }


                        }
                        #endregion
                        #region NEED TO INSERT DATA FOR AGENT PORT

                        // //cmd.Parameters.AddWithValue("@PoliceContacted", 0);
                        // //else if (rbPoliceYes.Checked)
                        // //{
                        // //    cmd.Parameters.AddWithValue("@PoliceContacted", 1);
                        // //    if (txtDepartmentName.Text.Trim != string.Empty)
                        // //        cmd.Parameters.AddWithValue("@DepartmentName", txtDepartmentName.Text);
                        // //    if (txtReportNumber.Text.Trim != string.Empty)
                        // //        cmd.Parameters.AddWithValue("@ReportNumber", txtReportNumber.Text);
                        // //}

                        //     //cmd.Parameters.AddWithValue("@PropertyInjuryDescription", txtInjuryDesc2.Text);
                        //     //cmd.Parameters.AddWithValue("@PropertyType", txtPremisesType.Text);
                        //     //cmd.Parameters.AddWithValue("@PropertyDescription", txtPropertyDesc.Text);
                        //     //cmd.Parameters.AddWithValue("@PropertyDamage", txtPropertyDamage.Text);
                        //     //cmd.Parameters.AddWithValue("@PropertyDamageAmount", txtPropertyDamageAmt.Text);
                        //     //cmd.Parameters.AddWithValue("@PropertyLocation", txtWherePropertyDamage.Text);

                        //     //cmd.Parameters.AddWithValue("@OtherLiabilityDescription", txtOtherLiabDesc.Text);
                        //     //cmd.Parameters.AddWithValue("@OtherLiabilityClaimantName", txtOtherLiabName.Text);
                        //     //cmd.Parameters.AddWithValue("@OtherLiabilityClaimantAddress", txtOtherLiabAdd.Text);
                        //     //cmd.Parameters.AddWithValue("@OtherLiabilityClaimantContactNumbers", txtOtherLiabContacts.Text);

                        //     //cmd.Parameters.AddWithValue("@OtherLiability2Description", txtOtherLiabDesc2.Text);
                        //     //cmd.Parameters.AddWithValue("@OtherLiability2ClaimantName", txtOtherLiabName2.Text);
                        //     //cmd.Parameters.AddWithValue("@OtherLiability2ClaimantAddress", txtOtherLiabAdd2.Text);
                        //     //cmd.Parameters.AddWithValue("@OtherLiability2ClaimantContactNumbers", txtOtherLiabContacts2.Text);

                        //     //cmd.Parameters.AddWithValue("@OtherLiability3Description", txtOtherLiabDesc3.Text);
                        //     //cmd.Parameters.AddWithValue("@OtherLiability3ClaimantName", txtOtherLiabName3.Text);
                        //     //cmd.Parameters.AddWithValue("@OtherLiability3ClaimantAddress", txtOtherLiabAdd3.Text);
                        //     //cmd.Parameters.AddWithValue("@OtherLiability3ClaimantContactNumbers", txtOtherLiabContacts3.Text);

                        //     //cmd.Parameters.AddWithValue("@OtherPartyPolicyNumber", txtOtherPolNum.Text);
                        //     //cmd.Parameters.AddWithValue("@OtherPartyInsurer", txtOtherInsureComp.Text);

                        // //if (chkMail.Checked)
                        // //    cmd.Parameters.AddWithValue("@AddlDocoMail", 1);
                        // //else
                        // //    cmd.Parameters.AddWithValue("@AddlDocoMail", 0);
                        // //if (chkFax.Checked)
                        // //    cmd.Parameters.AddWithValue("@AddlDocoFAX", 1);
                        // //else
                        // //    cmd.Parameters.AddWithValue("@AddlDocoFAX", 0);
                        // //if (chkEmail.Checked)
                        // //    cmd.Parameters.AddWithValue("@AddlDocoEmail", 1);
                        // //else
                        // //    cmd.Parameters.AddWithValue("@AddlDocoEmail", 0);


                        //// cmd.Parameters.AddWithValue("@ConfirmEmailAddress", txtConfirmEmail.Text);
                        ///

                        // *******************
                        // Injureds
                        // *******************
                        //if (ctlInjureds.PersonTable != null && ctlInjureds.PersonTable.Rows.Count > 0)
                        //{
                        //    PersonTypeID = FNOLCommon.Get_PersonType_Id(DiamondWebClass.FNOLCommonLibrary.PersonsType_enum.Injured, this, txtPolnum.Text, err);
                        //    if (PersonTypeID == null)
                        //    {
                        //        if (err != null && err != string.Empty)
                        //            throw new Exception("Error getting person type id: " + err);
                        //        else
                        //            throw new Exception("Unknown error getting person type id!");
                        //    }
                        //    foreach (DataRow inj in ctlInjureds.PersonTable.Rows)
                        //    {
                        //        DiamondWebClass.FNOLCommonLibrary.PersonsRecord_structure pr = new DiamondWebClass.FNOLCommonLibrary.PersonsRecord_structure();
                        //        pr.FirstName = inj("FirstName").ToString;
                        //        if (!IsDBNull(inj("MiddleName")))
                        //            pr.MiddleName = inj("MiddleName").ToString();
                        //        pr.LastName = inj("LastName").ToString();
                        //        if (!IsDBNull(inj("HomePhone")))
                        //            pr.HomePhone = inj("HomePhone").ToString();
                        //        if (!IsDBNull(inj("BusinessPhone")))
                        //            pr.BusinessPhone = inj("BusinessPhone").ToString();
                        //        if (!IsDBNull(inj("CellPhone")))
                        //            pr.CellPhone = inj("CellPhone").ToString();
                        //        if (!IsDBNull(inj("FAX")))
                        //            pr.FAX = inj("FAX").ToString();
                        //        if (!IsDBNull(inj("Address")))
                        //            pr.Address = inj("Address").ToString();
                        //        if (!IsDBNull(inj("City")))
                        //            pr.City = inj("City").ToString();
                        //        // If Not IsDBNull(inj("State")) Then pr.State = inj("State").ToString()
                        //        // updated 2/11/2019
                        //        if (!IsDBNull(inj("State")))
                        //        {
                        //            string stId = inj("State").ToString();
                        //            if (Information.IsNumeric(stId) == false)
                        //                stId = FNOLCommon.GetDiamondStateID(stId, this, txtPolnum.Text, lblMsg);
                        //            pr.State = stId;
                        //        }
                        //        if (!IsDBNull(inj("Zip")))
                        //            pr.Zip = inj("Zip").ToString();
                        //        if (!IsDBNull(inj("Email")))
                        //            pr.Email = inj("Email").ToString();
                        //        if (!IsDBNull(inj("InjuryType")))
                        //            pr.InjuryType = inj("InjuryType").ToString();
                        //        if (!IsDBNull(inj("InjuredAge")))
                        //            pr.InjuredAge = inj("InjuredAge").ToString();
                        //        if (!IsDBNull(inj("InjuredOccupation")))
                        //            pr.InjuredOccupation = inj("InjuredOccupation").ToString();
                        //        if (!IsDBNull(inj("InjuredDoing")))
                        //            pr.InjuredDoing = inj("InjuredDoing").ToString();
                        //        if (!IsDBNull(inj("InjuredTaken")))
                        //            pr.InjuredTaken = inj("InjuredTaken").ToString();
                        //        if (!IsDBNull(inj("InjuryDescription")))
                        //            pr.InjuryDescription = inj("InjuryDescription").ToString();
                        //        if (!FNOLCommon.InsertPersonsRecord(pr, FNOLId, PersonTypeID, conn, txn, this, txtPolnum.Text, err))
                        //        {
                        //            if (err != null && err != string.Empty)
                        //                throw new Exception("Error adding injured record: " + err);
                        //            else
                        //                throw new Exception("Unknown error adding injured record!");
                        //        }
                        //    }
                        //}

                        //// *******************
                        //// Other Vehicle Owners
                        //// *******************
                        //if (ctlOtherVehicleOwners.PersonTable != null && ctlOtherVehicleOwners.PersonTable.Rows.Count > 0)
                        //{
                        //    PersonTypeID = FNOLCommon.Get_PersonType_Id(DiamondWebClass.FNOLCommonLibrary.PersonsType_enum.OtherVehicleOwner, this, txtPolnum.Text, err);
                        //    if (PersonTypeID == null)
                        //    {
                        //        if (err != null && err != string.Empty)
                        //            throw new Exception("Error getting person type id: " + err);
                        //        else
                        //            throw new Exception("Unknown error getting person type id!");
                        //    }
                        //    foreach (DataRow ovo in ctlOtherVehicleOwners.PersonTable.Rows)
                        //    {
                        //        DiamondWebClass.FNOLCommonLibrary.PersonsRecord_structure pr = new DiamondWebClass.FNOLCommonLibrary.PersonsRecord_structure();
                        //        pr.FirstName = ovo("FirstName").ToString;
                        //        if (!IsDBNull(ovo("MiddleName")))
                        //            pr.MiddleName = ovo("MiddleName").ToString();
                        //        pr.LastName = ovo("LastName").ToString();
                        //        if (!IsDBNull(ovo("HomePhone")))
                        //            pr.HomePhone = ovo("HomePhone").ToString();
                        //        if (!IsDBNull(ovo("BusinessPhone")))
                        //            pr.BusinessPhone = ovo("BusinessPhone").ToString();
                        //        if (!IsDBNull(ovo("CellPhone")))
                        //            pr.CellPhone = ovo("CellPhone").ToString();
                        //        if (!IsDBNull(ovo("FAX")))
                        //            pr.FAX = ovo("FAX").ToString();
                        //        if (!IsDBNull(ovo("Address")))
                        //            pr.Address = ovo("Address").ToString();
                        //        if (!IsDBNull(ovo("City")))
                        //            pr.City = ovo("City").ToString();
                        //        // If Not IsDBNull(ovo("State")) Then pr.State = ovo("State").ToString()
                        //        // updated 2/11/2019
                        //        if (!IsDBNull(ovo("State")))
                        //        {
                        //            string stId = ovo("State").ToString();
                        //            if (Information.IsNumeric(stId) == false)
                        //                stId = FNOLCommon.GetDiamondStateID(stId, this, txtPolnum.Text, lblMsg);
                        //            pr.State = stId;
                        //        }
                        //        if (!IsDBNull(ovo("Zip")))
                        //            pr.Zip = ovo("Zip").ToString();
                        //        if (!IsDBNull(ovo("Email")))
                        //            pr.Email = ovo("Email").ToString();
                        //        if (!IsDBNull(ovo("InjuryType")))
                        //            pr.InjuryType = ovo("InjuryType").ToString();
                        //        if (!IsDBNull(ovo("InjuredAge")))
                        //            pr.InjuredAge = ovo("InjuredAge").ToString();
                        //        if (!IsDBNull(ovo("InjuryDescription")))
                        //            pr.InjuryDescription = ovo("InjuryDescription").ToString();
                        //        if (!FNOLCommon.InsertPersonsRecord(pr, FNOLId, PersonTypeID, conn, txn, this, txtPolnum.Text, err))
                        //        {
                        //            if (err != null && err != string.Empty)
                        //                throw new Exception("Error adding other vehicle owner record: " + err);
                        //            else
                        //                throw new Exception("Unknown error adding other vehicle owner record!");
                        //        }
                        //    }
                        //}



                        //// *******************
                        //// Witnesses
                        //// *******************
                        //if (ctlWitness.PersonTable != null && ctlWitness.PersonTable.Rows.Count > 0)
                        //{
                        //    PersonTypeID = FNOLCommon.Get_PersonType_Id(DiamondWebClass.FNOLCommonLibrary.PersonsType_enum.Witness, this, txtPolnum.Text, err);
                        //    if (PersonTypeID == null)
                        //    {
                        //        if (err != null && err != string.Empty)
                        //            throw new Exception("Error getting person type id: " + err);
                        //        else
                        //            throw new Exception("Unknown error getting person type id!");
                        //    }
                        //    foreach (DataRow w in ctlWitness.PersonTable.Rows)
                        //    {
                        //        DiamondWebClass.FNOLCommonLibrary.PersonsRecord_structure pr = new DiamondWebClass.FNOLCommonLibrary.PersonsRecord_structure();
                        //        pr.FirstName = w("FirstName").ToString;
                        //        if (!IsDBNull(w("MiddleName")))
                        //            pr.MiddleName = w("MiddleName").ToString();
                        //        pr.LastName = w("LastName").ToString();
                        //        if (!IsDBNull(w("HomePhone")))
                        //            pr.HomePhone = w("HomePhone").ToString();
                        //        if (!IsDBNull(w("BusinessPhone")))
                        //            pr.BusinessPhone = w("BusinessPhone").ToString();
                        //        if (!IsDBNull(w("CellPhone")))
                        //            pr.CellPhone = w("CellPhone").ToString();
                        //        if (!IsDBNull(w("FAX")))
                        //            pr.FAX = w("FAX").ToString();
                        //        if (!IsDBNull(w("Address")))
                        //            pr.Address = w("Address").ToString();
                        //        if (!IsDBNull(w("City")))
                        //            pr.City = w("City").ToString();
                        //        // If Not IsDBNull(w("State")) Then pr.State = w("State").ToString()
                        //        // updated 2/11/2019
                        //        if (!IsDBNull(w("State")))
                        //        {
                        //            string stId = w("State").ToString();
                        //            if (Information.IsNumeric(stId) == false)
                        //                stId = FNOLCommon.GetDiamondStateID(stId, this, txtPolnum.Text, lblMsg);
                        //            pr.State = stId;
                        //        }
                        //        if (!IsDBNull(w("Zip")))
                        //            pr.Zip = w("Zip").ToString();
                        //        if (!IsDBNull(w("Email")))
                        //            pr.Email = w("Email").ToString();
                        //        if (!IsDBNull(w("InjuryType")))
                        //            pr.InjuryType = w("InjuryType").ToString();
                        //        if (!IsDBNull(w("InjuredAge")))
                        //            pr.InjuredAge = w("InjuredAge").ToString();
                        //        if (!IsDBNull(w("InjuryDescription")))
                        //            pr.InjuryDescription = w("InjuryDescription").ToString();
                        //        if (!FNOLCommon.InsertPersonsRecord(pr, FNOLId, PersonTypeID, conn, txn, this, txtPolnum.Text, err))
                        //        {
                        //            if (err != null && err != string.Empty)
                        //                throw new Exception("Error adding Witness record: " + err);
                        //            else
                        //                throw new Exception("Unknown error adding Witness record!");
                        //        }
                        //    }
                        //}
                        #endregion


                        #region Attachments
                        // Attachments - Insert each file in the passed list into the attachments table and link it to the 
                        // FNOL record by FNOL_Id
                        if (fnol!= null && fnol.FNOLAttachements !=null && fnol.FNOLAttachements.Count >0 )
                            {  
                               foreach(var s in fnol.FNOLAttachements)
                                {
                                    if (s.FileName != null && s.FileBytes != null) // WS-949 
                                        {
                                            SqlCommand cmmd = new SqlCommand();
                                            cmmd.Connection = connFNOL;
                                            cmmd.Transaction = txn;
                                            cmmd.CommandType = System.Data.CommandType.StoredProcedure;
                                            cmmd.CommandText = "sp_Insert_tbl_FNOLDocuments";
                                            cmmd.Parameters.AddWithValue("@FNOL_Id", FNOLId);
                                            cmmd.Parameters.AddWithValue("@DocumentName", s.FileName);
                                            cmmd.Parameters.AddWithValue("@DocumentImage", s.FileBytes);
                                            cmmd.Parameters.AddWithValue("@DateInserted", DateTime.Now);
                                            rtn = cmmd.ExecuteNonQuery();
                                            if (rtn != 1)
                                                throw new Exception("Error inserting document " + s.FileName);
                                        }
                                }
                        }
                        #endregion
                        txn.Commit();
                    // Done Inserting the FNOL Record. Commit changes
                   

                    // '''''''''''''''''''''''''''''''''''''''
                    // ! AUTOMATIC CLAIM ASSIGNMENT LOGIC !
                    // '''''''''''''''''''''''''''''''''''''''
                    // If the claim was automatically assigned by Diamond:
                    // - Send Adjuster email
                    // - Update the FNOL record with the assignment info.
                    // - Update the adjuster claim count.
                    // - Send documents to Onbase.
                    // - Write the assignment log.
                    if (ClaimWasAutoAssigned)
                    {
                        List<string> ProcessMessages = new List<string>();
                        List<string> WarningMessages = new List<string>();
                        List<string> ErrorMessages = new List<string>();

                        // Get Adjuster Info from the FNOL db
                        string AdjId = null;
                        string AdjName = null;
                        string GroupId = null;
                        string AdjEmail = null;
                        string err = null;
                            var claim = new Claim();
                            if (claim.Get_FNOLAdjusterInfo(AutoAssignInfo.DiamondAdjuster_Id,ref AdjId,ref AdjName,ref GroupId,fnol.PolicyNumber,err)) 
                        {
                            ProcessMessages.Add("*" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                            ProcessMessages.Add("* Claim was automatically assigned by Diamond to Adjuster " + AdjName + ".");
                                if (fnol.EntryUser != "MemberPortal")
                                {
                                    // 1. Send the Adjuster Email
                                    AdjEmail = claim.GetAdjusterEmailAddress(AutoAssignInfo.DiamondAdjuster_Id, err);
                                    if (AdjEmail == null || AdjEmail.Trim() == string.Empty)
                                    {
                                        if (err != null && err.Trim() != string.Empty)
                                            WarningMessages.Add("* Error getting adjuster email (GetAdjusterEmailAddress: " + err + "), no email was sent!!  Please notify the adjuster that this claim was assigned.");
                                        else
                                            WarningMessages.Add("* Error getting adjuster email, no email was sent!!  Please notify the adjuster that this claim was assigned.");
                                    }
                                    else
                                    {
                                        ProcessMessages.Add("* Adjuster email address found: " + AdjEmail);
                                        if (claim.SendAssignmentEmail(AdjEmail, FNOLId.ToString(), DiamondClaimNumber, ref err))
                                            ProcessMessages.Add("* Adjuster email sent to " + AdjEmail + ".");
                                        else if (err == null || err.Trim() == string.Empty)
                                            WarningMessages.Add("* Error sending the adjuster email!");
                                        else
                                            WarningMessages.Add("* Error sending the adjuster email: " + err);
                                    }
                                }
                            // 2. Update FNOL Record with the assignment info and update the FNOL assigned flag
                            AutoAssignInfo.AdjusterName = AdjName;
                            AutoAssignInfo.FNOLAdjusterID = AdjId;
                            AutoAssignInfo.FNOL_ID = Convert.ToInt32(FNOLId);
                            AutoAssignInfo.Group_Id = GroupId;
                            if (claim.UpdateFNOLToAssigned(AutoAssignInfo))
                                ProcessMessages.Add("* Successfully updated FNOL Assigned Flag to True.");
                            else
                                WarningMessages.Add("* Error updating FNOL assigned flag, please Notify IT.");

                            // 3. Update adjuster claim count
                            if (UpdateAdjusterCount(AutoAssignInfo.FNOLAdjusterID, AutoAssignInfo.Group_Id, DateTime.Today.ToShortDateString(), CommonObjects.Enums.Enums.AdjusterCountUpdateIndicator.Increase))
                                ProcessMessages.Add("* Updated adjuster claim count for " + AdjName + ".");
                            else
                                WarningMessages.Add("* Error updating the adjuster claim count.  Please adjust the count manually.");
                           
                            // 4. Set up documents in OnBase
                            if (InsertFNOLDocumentsIntoOnBaseFromFNOLRecord(FNOLId.ToString(),ref err))
                                ProcessMessages.Add("* Successfully added claim documents to OnBase.");
                            else if (err != null && err != string.Empty)
                            {
                                if (err.Contains("GetEmailBody failed"))
                                    WarningMessages.Add("* Error Creating the FNOL Notice for insertion to OnBase, Please update OnBase Manually with the FNOL notice. " + "(" + err + ")");
                                else
                                    WarningMessages.Add("* Error (" + err + ") Creating the OnBase folder(s) and/or inserting the claim documents.  Please update OnBase Manually.");
                            }
                            else
                                WarningMessages.Add("* Unknown error Creating the OnBase folder(s) and/or inserting the claim documents.  Please update OnBase Manually.");
                        }
                        else
                            // Unable to get the FNOL Adjuster ID, group, and name.  Unable to proceed.
                            if (err != null && err.Trim() != string.Empty)
                            ErrorMessages.Add("* There was an error getting the required FNOL data for the assigned adjuster.  The claim was assigned in Diamond, but the assignment will not be reflected in the FNOL Claim Assignment site.  Please contact IT for assistance. " + "Error Message: " + err);
                        else
                            ErrorMessages.Add("* There was an error getting the required FNOL data for the assigned adjuster.  The claim was assigned in Diamond, but the assignment will not be reflected in the FNOL Claim Assignment site.  Please contact IT for assistance.");

                        // 5. If there were any warnings or messages we need to send an email to the claims admin so they can fix anything required.
                        if ((WarningMessages != null && WarningMessages.Count > 0) || (ErrorMessages != null && ErrorMessages.Count > 0))
                        {
                            if (!SendClaimsAdminWarningAndErrorEmail(FNOLId.ToString(), DiamondClaimNumber, WarningMessages, ErrorMessages,ref err))
                            {
                                if (err != null && err.Trim() != "")
                                    ErrorMessages.Add("Error sending the Warning and error email to the claims admin: " + err + ".");
                                else
                                    ErrorMessages.Add("Error sending the Warning and error email to the claims admin.");
                            }
                        }
                        if(AdjName!=null)
                        // LAST THING:  Update claim assignment log
                        if (!claim.WriteClaimAssignmentLog(fnol.PolicyNumber, FNOLId.ToString(), AdjName, ClaimPersonnel_Id, DiamondClaimNumber, DateTime.Today.ToShortDateString(), fnol.LossDate.ToString(), fnol.AgencyName, fnol.Description, WarningMessages, ErrorMessages, ProcessMessages))
                            throw new Exception("* Error writing the claim assignment log, please notify IT.");
                    }
                }
            }
            }
            catch (Exception ex)
            {
                HandleError(ClassName, "InsertFNOL", ex, null);
                Errors.Add(ex.ToString());
               
            }
            return true;
        }
        public static bool InsertFNOLDocumentsIntoOnBaseFromFNOLRecord(string FNOL_Id, ref string err)
        {
            DataRow FNOLRec = null/* TODO Change to default(_) if this is not a reference type */;
            FNOLData_Structure FNOLDetails = new FNOLData_Structure();
            string terr = null;
            DataTable FNOLDocs = null/* TODO Change to default(_) if this is not a reference type */;
            string fn = null;
            string docTitle = null;
            string SubFolder = null;
            byte[] filebytes = null;
            FileStream oFs = null;
            string OnBaseStep = string.Empty;
            var claim = new Claim();

            try
            {
                // Get the FNOL record
                OnBaseStep = "Get FNOL Record";
                FNOLRec = GetFNOLRecordById(FNOL_Id);
                if (FNOLRec == null)
                {
                    if (err != null && err != string.Empty)
                        throw new Exception(err);
                    else
                        throw new Exception("Unknown error getting FNOL Record");
                }

                // Load up the FNOL Details structure for the create
                OnBaseStep = "Load FNOL Details Structure";
                string ft = claim.GetFNOLTypeName(FNOLRec["FNOLType_Id"].ToString());
                FNOLDetails.LOB = ft.ToUpper().Trim();

                if (FNOLRec["AdjusterName"]!=null)
                    FNOLDetails.AdjusterName = FNOLRec["AdjusterName"].ToString();
                if (FNOLRec["AgencyName"]!=null)
                    FNOLDetails.AgencyName = FNOLRec["AgencyName"].ToString();
                if ( FNOLRec["EntryDate"] != null)
                    FNOLDetails.ClaimDate =(DateTime)FNOLRec["EntryDate"];
                if (FNOLRec["LossDescription"] != null)
                    FNOLDetails.Description = FNOLRec["LossDescription"].ToString();
                if (FNOLRec["DiamondClaimNumber"] != null)
                    FNOLDetails.DiamondClaimNumber = FNOLRec["DiamondClaimNumber"].ToString();
                if ( FNOLRec["InsuredCity"] != null)
                    FNOLDetails.InsuredAddressCity = FNOLRec["InsuredCity"].ToString();
                if (FNOLRec["InsuredState"] != null)
                    FNOLDetails.InsuredAddressState = FNOLRec["InsuredState"].ToString();
                if (FNOLRec["InsuredAddress"] != null)
                    FNOLDetails.InsuredAddressStreet = FNOLRec["InsuredAddress"].ToString();
                if (FNOLRec["InsuredZip"] != null)
                    FNOLDetails.InsuredAddressZip = FNOLRec["InsuredZip"].ToString();
                if (FNOLRec["InsuredFirst"] != null)
                    FNOLDetails.InsuredFirst = FNOLRec["InsuredFirst"].ToString();
                if (FNOLRec["InsuredLast"] != null)
                    FNOLDetails.InsuredLast = FNOLRec["InsuredLast"].ToString();
                // FNOLInfo.LOB = "" ' Not really needed
                if (FNOLRec["LossDate"] != null)
                    FNOLDetails.LossDate = Convert.ToDateTime(FNOLRec["LossDate"]);//(DateTime)FNOLRec["LossDate"];
                if ( FNOLRec["InsuredBusinessPhone"] != null)
                    FNOLDetails.PhoneBusiness = FNOLRec["InsuredBusinessPhone"].ToString();
                if (FNOLRec["InsuredCellPhone"] != null)
                    FNOLDetails.PhoneCell = FNOLRec["InsuredCellPhone"].ToString();
                if (FNOLRec["InsuredHomePhone"] != null)
                    FNOLDetails.PhoneHome = FNOLRec["InsuredHomePhone"].ToString();
                if (FNOLRec["PrimaryContact"] != null)
                    FNOLDetails.PrimaryContact = FNOLRec["PrimaryContact"].ToString();
                if (FNOLRec["PolicyNumber"] != null)
                {
                    FNOLDetails.PolicyNumber = FNOLRec["PolicyNumber"].ToString();
                    terr = claim.GetPolicyTerritoryNumber(FNOLRec["PolicyNumber"].ToString(),ref err);
                    if (terr == null)
                    {
                        if (err != null && err != string.Empty)
                            throw new Exception(err);
                        else
                            throw new Exception("Error getting territory number");
                    }
                    FNOLDetails.TerritoryNumber = terr;
                }
                else
                    throw new Exception("Policy Number is empty!");

                if (System.Convert.ToBoolean(FNOLRec["CAT"]))
                    FNOLDetails.CAT = true;
                else
                    FNOLDetails.CAT = false;

                // Insert the documents into the folder if there are any
                OnBaseStep = "Insert Documents";
                err = null;
                FNOLDocs = claim.GetFNOLDocuments(FNOL_Id,ref err);
                if (FNOLDocs == null || FNOLDocs.Rows.Count <= 0)
                {
                    if (err != null && err != string.Empty)
                        // Error getting the documents
                        throw new Exception(err);
                    else
                    {
                    }
                }
                else
                    // Documents found, insert them into onbase
                    foreach (DataRow doc in FNOLDocs.Rows)
                    {
                        docTitle = doc["DocumentName"].ToString();
                        filebytes = (byte[])doc["DocumentImage"];
                        claim.UploadToOnbase(filebytes, docTitle, FNOLDetails.DiamondClaimNumber);
                    }

                // Add a copy of the FNOL Email to the documents folder MGB 5/25/16
                OnBaseStep = "Add FNOL Copy";
                string FNOLEmail = claim.GetFNOLEmailBody(FNOL_Id,ref err);
                if (FNOLEmail != null && FNOLEmail != string.Empty)
                {
                    docTitle = "FNOLDetails_" + FNOLDetails.PolicyNumber + "_" + FNOLDetails.DiamondClaimNumber + ".html";
                    filebytes = System.Text.Encoding.Unicode.GetBytes(FNOLEmail);
                   claim.UploadToOnbase(filebytes, docTitle, FNOLDetails.DiamondClaimNumber);
                }
                else if (err != null)
                    throw new Exception("GetEmailBody failed: " + err);
                else
                    throw new Exception("GetEmailBody failed.");

                return true;
            }
            catch (Exception ex)
            {
                err = ex.Message + " (step: " + OnBaseStep + ")";
                HandleError(ClassName, "InsertFNOLDocumentsIntoAcrosoftFromFNOLRecord (step: " + OnBaseStep + ")",ex,null);
                return false;
            }
        }
        public static bool SendClaimsAdminWarningAndErrorEmail(string FNOL_Id, string DiamondClaimNumber, List<string> WarningMessages, List<string> ErrorMessages, ref string err)
        {
            EmailInfo_Structure_FNOLCA EmailInfo = new EmailInfo_Structure_FNOLCA();
            DataTable atts = new DataTable();
            List<string> ListOfFiles = null;
            bool CAT = false;
            DataRow FNOLRec = null/* TODO Change to default(_) if this is not a reference type */;
            string AgencyEmail = "";
            var claim = new Claim();

            try
            {
                FNOLRec = GetFNOLRecordById(FNOL_Id);
                if (FNOLRec == null)
                    throw new Exception("Error getting FNOL Record: " + err);
                CAT = System.Convert.ToBoolean(FNOLRec["CAT"]);

                EmailInfo.Body = claim.GetFNOLEmailBody(FNOL_Id,ref err, ErrorMessages, WarningMessages);
                if (EmailInfo.Body == null)
                    throw new Exception(err);

                // The admin email address should be the LssReportingErrorEmail address
                if (AppConfig.LossReportingErrorEmail != null)
                    EmailInfo.ToAddress = AppConfig.LossReportingErrorEmail;
                else
                    throw new Exception("Config key 'LossReportingErrorEmail' is missing!!");

                EmailInfo.SubjectLine_OPTIONAL = "*** ERRORS OR WARNINGS ON CLAIM ASSIGNMENT - " + FNOLRec["PolicyNumber"].ToString() + " - " + DiamondClaimNumber;

                // Attachments - Save each one to the temp folder, attach, then delete 
                atts = claim.GetFNOLDocuments(FNOL_Id,ref err);

                if (atts != null && atts.Rows.Count > 0)
                {
                    // Create the temp files and add them to the file list
                    ListOfFiles = new List<string>();
                    foreach (DataRow dr in atts.Rows)
                    {
                        string filepath = AppConfig.DECFolder + dr["DocumentName"].ToString();
                        byte[] fileimage = (byte[])dr["DocumentImage"];
                        if (File.Exists(filepath))
                            File.Delete(filepath);
                        FileStream fs = new FileStream(filepath, FileMode.Create);
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            bw.Write(fileimage);
                            bw.Flush();
                        }
                        fs.Close();
                        ListOfFiles.Add(filepath);
                    }
                }

                // Send the email to the admin
                if (!claim.SendEmail(ref EmailInfo,ref err, ListOfFiles))
                    throw new Exception("Error sending assignment email: " + err);

                return true;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                HandleError(ClassName, "SendClaimsAdminWarningAndErrorEmail", ex,null);
                return false;
            }
            finally
            {
                if (ListOfFiles != null)
                    claim.DeleteOldClaimFiles(ListOfFiles);
            }
        }
        public bool WriteClaimAssignmentLog(string PolicyNumber, string FNOL_Id, string AdjName, string ClaimPersonnel_Id, string DiamondClaimNumber, string ClaimDate, string LossDate, string AgencyName, string LossDesc, List<string> WarningMessages = null, List<string> ErrorMessages = null, List<string> ProcessMessages = null)
        {
            SqlConnection conn = new SqlConnection(AppConfig.ConnFNOL);
            SqlCommand cmd = new SqlCommand();
            int rtn = -1;
            int ndx = -1;
            string wm = null;

            try
            {
                conn.Open();
                
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_Insert_tbl_Assignment_Log";
                cmd.Parameters.AddWithValue("@FNOL_Id", System.Convert.ToInt32(FNOL_Id));
                cmd.Parameters.AddWithValue("@LogDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@AssignedByUser", "Diamond Automatic");
                cmd.Parameters.AddWithValue("@AssignedToAdjusterName", AdjName);
                cmd.Parameters.AddWithValue("@AssignedToAdjuster_Claimpersonnel_Id", ClaimPersonnel_Id);
                cmd.Parameters.AddWithValue("@AssignedToManagerName", "N/A");
                cmd.Parameters.AddWithValue("@AssignedToManager_Claimpersonnel_Id", "N/A");
                cmd.Parameters.AddWithValue("@DiamondClaimNumber", DiamondClaimNumber);
                cmd.Parameters.AddWithValue("@ClaimDate", ClaimDate);
                cmd.Parameters.AddWithValue("@PolicyNumber", PolicyNumber);
                cmd.Parameters.AddWithValue("@LossDate", LossDate);
                cmd.Parameters.AddWithValue("@AgencyName", AgencyName);
                cmd.Parameters.AddWithValue("@LossDescription", LossDesc);

                // Add messages lists as comma delimited fields
                if (WarningMessages != null && WarningMessages.Count > 0)
                {
                    ndx = -1;
                    foreach (string w in WarningMessages)
                    {
                        ndx += 1;
                        wm += w;
                        if (ndx != WarningMessages.Count - 1)
                            wm += "|";
                    }
                    cmd.Parameters.AddWithValue("@WarningMessages", wm);
                }
                if (ErrorMessages != null && ErrorMessages.Count > 0)
                {
                    wm = string.Empty;
                    ndx = -1;
                    foreach (string w in ErrorMessages)
                    {
                        ndx += 1;
                        wm += w;
                        if (ndx != ErrorMessages.Count - 1)
                            wm += "|";
                    }
                    cmd.Parameters.AddWithValue("@ErrorMessages", wm);
                }
                if (ProcessMessages != null && ProcessMessages.Count > 0)
                {
                    wm = string.Empty;
                    ndx = -1;
                    foreach (string w in ProcessMessages)
                    {
                        ndx += 1;
                        wm += w;
                        if (ndx != ProcessMessages.Count - 1)
                            wm += "|";
                    }
                    cmd.Parameters.AddWithValue("@ProcessingMessages", wm);
                }

                rtn = cmd.ExecuteNonQuery();
                if (!int.TryParse(rtn.ToString(),out int r) || rtn != 1)
                    throw new Exception("Error inserting record into Assignment Log");

                return true;
            }
            catch (Exception ex)
            {
                HandleError(ClassName, "WriteClaimAssignmentLog", ex,null);
                return false;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Dispose();
                cmd.Dispose();
            }
        }

        private string GetPolicyTerritoryNumber(string PolicyNumber, ref string err)
        {
            PolicyNumberObject po = null/* TODO Change to default(_) if this is not a reference type */;
            try
            {
                po = new PolicyNumberObject(PolicyNumber);
                if (po.hasError)
                {
                    if (po.errorMsg != null && po.errorMsg != string.Empty)
                        throw new Exception(po.errorMsg);
                    else
                        throw new Exception("Unable to load policy number object");
                }
                else
                {
                    po.GetAllAgencyInfo = true;
                    po.GetPolicyInfo();
                    if (po.hasPolicyInfo)
                    {
                        if (po.AgencyInfo != null && po.AgencyInfo.AgencyTerritory != null && po.AgencyInfo.AgencyTerritory != string.Empty)
                            return po.AgencyInfo.AgencyTerritory.PadLeft(2, '0');
                        else
                            throw new Exception("Territory was not on policy object (AgencyTerritory)");
                    }
                    else if (po.hasPolicyInfoError)
                    {
                        if (po.PolicyInfoError != null && po.PolicyInfoError != string.Empty)
                            throw new Exception(po.PolicyInfoError);
                        else
                            throw new Exception("Unable to load policy info object");
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                HandleError(ClassName, "GetPolicyTerritoryNumber", ex,null);
                return null;
            }
            finally
            {
                if (po != null)
                    po.Dispose();
            }
        }
        public string Get_FNOLType_Id(string FNOLType, ref Page pg, string PolNum, ref string err)
        {
            SqlConnection conn = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            object id = null;

            try
            {
                conn.ConnectionString = AppConfig.ConnFNOL;
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT FNOLType_Id FROM tbl_FNOLType WHERE FNOLType = '" + FNOLType + "'";
                id = cmd.ExecuteScalar();

                if (id == null)
                    throw new Exception("Error getting FNOLType Id");
                else
                    return id.ToString();
            }
            catch (Exception ex)
            {
                err = ex.Message;
                HandleError(ClassName, "Get_FNOLType_Id", ex,null);
                return null;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Dispose();
                cmd.Dispose();
            }
        }

        public bool UpdateFNOLToAssigned(FNOLResponseData AutoAssignmentDetails)
        {
            SqlConnection conn = new SqlConnection(AppConfig.ConnFNOL);
            SqlCommand cmd = new SqlCommand();
            int rtn = -1;

            try
            { 
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_UpdateFNOLToAssigned";
                cmd.Parameters.AddWithValue("@FNOL_ID", AutoAssignmentDetails.FNOL_ID);
                cmd.Parameters.AddWithValue("@DiamondAdjusterID", AutoAssignmentDetails.DiamondAdjuster_Id);
                cmd.Parameters.AddWithValue("@AdjusterName", AutoAssignmentDetails.AdjusterName);
                cmd.Parameters.AddWithValue("@DateAssignedToAdjuster", AutoAssignmentDetails.DateAssigned);
                cmd.Parameters.AddWithValue("@AssignedBy", AutoAssignmentDetails.AssignedBy);
                cmd.Parameters.AddWithValue("@FNOLClaimAssignAdjuster_Id", AutoAssignmentDetails.FNOLAdjusterID);
                if (AutoAssignmentDetails.CAT)
                    cmd.Parameters.AddWithValue("@CAT", 1);
                else
                    cmd.Parameters.AddWithValue("@CAT", 0);
                cmd.Parameters.AddWithValue("@GroupId", AutoAssignmentDetails.Group_Id);

                
                rtn = cmd.ExecuteNonQuery();

                if (rtn > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                // FNOLCommon.HandleError(ClassName, "UpdateFNOLToAssigned", ex, Me, txtPolnum.Text, lblMsg)
                HandleError(ClassName, "UpdateFNOLToAssigned", ex,null);
                return false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Dispose();
                cmd.Dispose();
            }
        }
        public static bool UpdateAdjusterCount(string AdjusterID, string GroupID, string CountDate, CommonObjects.Enums.Enums.AdjusterCountUpdateIndicator Ind)
        {
            SqlConnection conn = new SqlConnection(AppConfig.ConnFNOL);
            SqlCommand cmd = new SqlCommand();
            object rtn = new object();
            int NewVal =0;
            int OldVal = -1;
            DataRow drCount = null/* TODO Change to default(_) if this is not a reference type */;
            var tries = 0;
            var claim = new Claim();

            try
            {
                conn.Open();

            // Get the count record
            getcount:
                ;
                drCount = claim.GetDailyCountRecord(AdjusterID, GroupID, CountDate);
                if (drCount == null)
                {
                    if (tries == 0)
                    {
                        // No Daily count record found.  Create a new one.
                        if (!claim.CreateNewCountRecord(AdjusterID, GroupID, CountDate))
                            throw new Exception("Error creating count record!");
                        tries = 1;
                        goto getcount;
                    }
                    else
                        // We tried to create a count record but we still can't find it.  Error!
                        throw new Exception("Error getting count record!");
                }
                OldVal = (int)drCount["count"];

                // Calculate the new value
                switch (Ind)
                {
                    case CommonObjects.Enums.Enums.AdjusterCountUpdateIndicator.Decrease:
                        {
                            NewVal = OldVal - 1;
                            break;
                        }

                    case CommonObjects.Enums.Enums.AdjusterCountUpdateIndicator.Increase:
                        {
                            NewVal = OldVal + 1;
                            break;
                        }
                }
                
                // Update the record with the new count.  Use the ID from the count record
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE Counts SET [Count] = " + NewVal + " WHERE Counts_Id = " + drCount["Counts_Id"].ToString();
                rtn = cmd.ExecuteNonQuery();
                if (int.TryParse(rtn.ToString(),out int r))
                {
                    if (System.Convert.ToInt32(rtn) == 1)
                        return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                // FNOLCommon.HandleError(ClassName, "UpdateAdjusterCounts", ex, Me, txtPolnum.Text, lblMsg)
                HandleError(ClassName, "UpdateAdjusterCounts",ex,null);
                return false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Dispose();
                cmd.Dispose();
            }
        }
        public DataRow GetDailyCountRecord(string AdjusterId, string GroupId, string CountDate)
        {
            SqlConnection conn = new SqlConnection(AppConfig.ConnFNOL);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable tbl = new DataTable();
            string sql = null;
            bool breakout = false;

            try
            {
                conn.Open();

            tryquery:
                ;
                sql = "SELECT * FROM Counts WHERE FNOLClaimAssignAdjuster_ID = " + AdjusterId + " AND Groups_Id = " + GroupId + " AND CountDate = '" + CountDate + "'";
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                da.SelectCommand = cmd;
                da.Fill(tbl);

                if (tbl == null || tbl.Rows.Count <= 0)
                {
                    // No daily count record found.  Create it.
                    if (CreateNewCountRecord(AdjusterId, GroupId, CountDate) & !breakout)
                    {
                        // If we created a new record we need to run the query again to get it.
                        breakout = true; // Only use the goto once to avoid an infinite loop!
                        goto tryquery;
                    }
                    else
                        throw new Exception("Error updating count record");
                }
                else
                    return tbl.Rows[0];
            }
            catch (Exception ex)
            {
                HandleError(ClassName, "GetDailyCountRecord", ex,null);
                return null/* TODO Change to default(_) if this is not a reference type */;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Dispose();
                cmd.Dispose();
            }
        }
        public bool CreateNewCountRecord(string AdjusterID, string GroupID, string CountDate)
        {
            SqlConnection conn = new SqlConnection(AppConfig.ConnFNOL);
            SqlCommand cmd = new SqlCommand();
            object rtn = new object();

            try
            {
                conn.Open();

                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_InsertNewCountRecord";
                cmd.Parameters.AddWithValue("@UserID", AdjusterID);
                cmd.Parameters.AddWithValue("@GroupID", GroupID);
                cmd.Parameters.AddWithValue("@Date", CountDate);
                rtn = cmd.ExecuteNonQuery();

                if (int.TryParse(rtn.ToString(),out int r))
                {
                    if (System.Convert.ToInt32(rtn) == 1)
                        return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                HandleError(ClassName, "CreateNewCountRecord", ex,null);
                return false;
            }
        }

        public bool Get_FNOLAdjusterInfo(string ClaimPersonnel_Id,ref string FNOLAdjID,ref string AdjName,ref string AdjGroupId, string PolNum,String err)
        {
            System.Data.SqlClient.SqlConnection connFNOL = new System.Data.SqlClient.SqlConnection(AppConfig.ConnFNOL);
            SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            DataTable tbl = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                // Get Adjuster ID and Name
                cmd.Connection = connFNOL;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM FNOLClaimAssign_Adjusters WHERE claimpersonnel_id = " + ClaimPersonnel_Id;
                da.SelectCommand = cmd;
                da.Fill(tbl);
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    DataRow dataRow = tbl.Rows[0];
                    FNOLAdjID = dataRow["FNOLClaimAssignAdjuster_Id"].ToString();
                    AdjName = dataRow["Display_Name"].ToString();
                }
                else
                    throw new Exception("Error gettig Adjuster info!");

                // Get adjuster Group ID
                cmd = new SqlCommand();
                tbl = new DataTable();
                cmd.Connection = connFNOL;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_GetAdjusterGroups";
                cmd.Parameters.AddWithValue("@AdjusterId", FNOLAdjID);
                da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(tbl);
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    // AdjGroupId = tbl.Rows(0)("Groups_Id").ToString();""
                    DataRow dataRow = tbl.Rows[0];
                    AdjGroupId = dataRow["Groups_Id"].ToString();
                }
                else
                    throw new Exception("Error getting adjuster group!!");

                return true;
            }
            catch (Exception ex)
            {
                HandleError(ClassName, "Get_FNOLAdjusterInfo", ref ex,PolNum,null);
                err = ex.Message;
                return false;
            }
            finally
            {
                if (connFNOL != null && connFNOL.State == ConnectionState.Open)
                    connFNOL.Close();
                if (connFNOL != null)
                    connFNOL.Dispose();
            }
        }
        public string GetAdjusterEmailAddress(string Adjuster_Claimpersonnel_ID, string err)
        {
            SqlConnection conn = new SqlConnection(AppConfig.ConnDiamond);
            SqlCommand cmd = new SqlCommand();
            object rtn = null;

            try
            {
                // MGB 8-20-2019 don't bother performing the check if the passed id is invalid.  Added this logic to avoid unnecessary errors in the Error Log.
                if (Adjuster_Claimpersonnel_ID == null || Adjuster_Claimpersonnel_ID.Trim() == string.Empty)
                    return null;

                conn.Open();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT US.user_emailaddr FROM ClaimPersonnel CP JOIN users US ON US.Users_id = CP.users_id WHERE CP.claimpersonnel_id = " + Adjuster_Claimpersonnel_ID;
                rtn = cmd.ExecuteScalar();
                if (rtn == null)
                    throw new Exception("Adjuster Email Address not found!");
                if (rtn.ToString().Trim() == string.Empty)
                    throw new Exception("Adjuster does not have an email address set in Diamond.");

                return rtn.ToString();
            }
            catch (Exception ex)
            {
                err = ex.Message;
              //  HandleError(ClassName, "GetAdjusterEmailAddress", ex);
                return null;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Dispose();
                cmd.Dispose();
            }
        }
        public bool SendAssignmentEmail(string AdjusterEmailAddress, string FNOL_Id, string DiamondClaimNumber,ref string err)
        {
            EmailInfo_Structure_FNOLCA EmailInfo = new EmailInfo_Structure_FNOLCA();
            DataTable atts = new DataTable();
            List<string> ListOfFiles = null;
            bool CAT = false;
            DataRow FNOLRec = null/* TODO Change to default(_) if this is not a reference type */;
            string AgencyEmail = "";

            try
            {
                FNOLRec = GetFNOLRecordById(FNOL_Id);
                if(FNOLRec == null)
                    throw new Exception("Error getting FNOL Record: " + err);
                CAT = System.Convert.ToBoolean(FNOLRec["CAT"]);

                EmailInfo.Body = GetFNOLEmailBody(FNOL_Id,ref err);
                if (EmailInfo.Body == null)
                    throw new Exception(err);

                if (AppConfig.TestOrProd == "PROD")
                {
                    EmailInfo.ToAddress = AdjusterEmailAddress;
                    // CC CSU Group if CAT loss
                    if (CAT)
                        EmailInfo.CCAddress = AppConfig.FNOLCA_CSU_Email;
                }
                else
                {
                    // CC CSU Group if CAT loss - in test we just cc the LossReportingEmailAddress
                    EmailInfo.ToAddress = AppConfig.FNOLClaimAssign_LossReportingEmailAddress;
                    if (CAT)
                        EmailInfo.CCAddress = AppConfig.FNOLClaimAssign_LossReportingEmailAddress;
                }

                // AgencyEmail = FNOLRec["ConfirmEmailAddress").ToString()

                // Send a copy of the adjuster email to the FNOLSubmissions@IndianaFarmers.com
                if (AppConfig.FNOLCA_SendCopyOfAssignmentEmail != null)
                {
                    if (AppConfig.FNOLCA_SendCopyOfAssignmentEmail.ToUpper() == "TRUE")
                    {
                        if (FNOLRec["ConfirmEmailAddress"]!=null)
                            EmailInfo.CCAddress = AppConfig.FNOLCA_CopyOfAssignment_EmailAddress + ";" + FNOLRec["ConfirmEmailAddress"].ToString();
                        else
                            EmailInfo.CCAddress = AppConfig.FNOLCA_CopyOfAssignment_EmailAddress;
                    }
                }

                EmailInfo.SubjectLine_OPTIONAL = "CLAIM ASSIGNMENT - " + FNOLRec["PolicyNumber"].ToString() + " - " + DiamondClaimNumber;

                // Attachments - Save each one to the temp folder, attach, then delete 
                atts = GetFNOLDocuments(FNOL_Id,ref err);

                if (atts != null && atts.Rows.Count > 0)
                {
                    // Create the temp files and add them to the file list
                    ListOfFiles = new List<string>();
                    foreach (DataRow dr in atts.Rows)
                    {
                        string filepath = AppConfig.DECFolder + dr["DocumentName"].ToString();
                        byte[] fileimage = (byte[])dr["DocumentImage"];
                        if (File.Exists(filepath))
                            File.Delete(filepath);
                        FileStream fs = new FileStream(filepath, FileMode.Create);
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            bw.Write(fileimage);
                            bw.Flush();
                        }
                        fs.Close();
                        ListOfFiles.Add(filepath);
                    }
                }

                // Send the assignment email to the adjuster
                if (!SendEmail(ref EmailInfo, ref err, ListOfFiles))
                    throw new Exception("Error sending assignment email: " + err);

                // If the CAT email is turned on and this is a CAT claim, send a copy of the assignment email to the CAT handling company
                 if (AppConfig.TestOrProd.ToUpper() == "PROD" && CAT)
                {
                    if (AppConfig.FNOLCA_EnableCATThirdPartyEmails  != null && AppConfig.FNOLCA_EnableCATThirdPartyEmails.ToUpper() == "TRUE")
                    {
                        DataRow CompanyRec = GetCATCompanyRecord(FNOLRec["tbl_FNOL_CATCompany_Id"].ToString());
                        if (CompanyRec != null)
                        {
                            string CompanyName = CompanyRec["CompanyName"].ToString();
                            string CompanyEmail = CompanyRec["ClaimAssignmentNotificationEmailAddress"].ToString();

                            EmailInfo.ToAddress = CompanyEmail;
                            EmailInfo.SubjectLine_OPTIONAL = "INDIANA FARMERS MUTUAL INSURANCE NOTICE OF CLAIM ASSIGNMENT - " + FNOLRec["PolicyNumber"].ToString() + " - " + DiamondClaimNumber + " (" + CompanyName.ToUpper() + ")";

                            if (EmailInfo.ToAddress != null)
                            {
                                if (!SendEmail(ref EmailInfo, ref err, ListOfFiles))
                                    throw new Exception("Error sending CAT email to " + CompanyName + ": " + err);
                            }
                        }
                        else
                            throw new Exception("There was an error trying to send the CAT Claim email to the CAT company: Company record does not exist.");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                HandleError(ClassName, "SendAssignmentEmail", ex,null);
                return false;
            }
            finally
            {
                if (ListOfFiles != null)
                    DeleteOldClaimFiles(ListOfFiles);
            }
        }
        public DataRow GetCATCompanyRecord(string id)
        {
            SqlConnection conn = new SqlConnection(AppConfig.ConnFNOL);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable tbl = new DataTable();

            try
            {
                if (id == null || int.TryParse(id, out int n))
                    throw new Exception("invalid id passed!");

                conn.Open();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM tbl_FNOL_CATCompany WHERE tbl_FNOL_CATCompany_Id = " + id;
                da.SelectCommand = cmd;
                da.Fill(tbl);

                if (tbl != null && tbl.Rows.Count > 0)
                    return tbl.Rows[0];
                else
                    return null/* TODO Change to default(_) if this is not a reference type */;
            }
            catch (Exception ex)
            {
                HandleError(ClassName, "GetCATCompanyRecord", ex,null);
                return null/* TODO Change to default(_) if this is not a reference type */;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Dispose();
                cmd.Dispose();
                da.Dispose();
                tbl.Dispose();
            }
        }

        public bool DeleteOldClaimFiles(List<string> ListOfFiles = null)
        {
            try
            {
                // Delete files 24 hours old or older
                string[] dirfiles = Directory.GetFiles(AppConfig.DECFolder);
                if (dirfiles != null && dirfiles.Count() > 0)
                {
                    foreach (string s in dirfiles)
                    {
                        FileInfo fi = new FileInfo(s);
                        if (fi.CreationTime < DateTime.Now.AddDays(-1))
                        {
                            try
                            {
                                File.Delete(s);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }

                // Delete all of the files in the  passed list
                if (ListOfFiles != null && ListOfFiles.Count > 0)
                {
                    foreach (string f in ListOfFiles)
                    {
                        try
                        {
                            System.IO.File.Delete(f);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                HandleError(ClassName, "DeleteOldClaimFiles", ex,null);
                return false;
            }
        }
        public static DataRow GetFNOLRecordById(string id)
        {
            SqlConnection conn = new SqlConnection(AppConfig.ConnFNOL);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable tbl = new DataTable();
            string err;
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM tbl_FNOL WHERE FNOL_Id = " + id;
                da.SelectCommand = cmd;
                da.Fill(tbl);
                DataRow dataRow = tbl.Rows[0];
                if (tbl == null || tbl.Rows.Count <= 0)
                    throw new Exception("Record not found");

                return tbl.Rows[0];
            }
            catch (Exception ex)
            {
                err = ex.Message;
                HandleError(ClassName, "GetFNOLRecordById", ex,null);
                return null;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Dispose();
                cmd.Dispose();
                da.Dispose();
                tbl.Dispose();
            }
        }
        private string GetFNOLEmailBody(string FNOL_Id,ref string err, List<string> ErrorMessages = null, List<string> WarningMessages = null)
        {
            string strBody = "";
            int ndx = -1;
            string str = null;
            string FNOLType = null;
            DataTable PersonsTable = null/* TODO Change to default(_) if this is not a reference type */;
            string ct = "NO";

            try
            {
                // Get the FNOL record
                DataRow FNOLRec = GetFNOLRecordById(FNOL_Id);
                if (FNOLRec == null)
                    throw new Exception(err);

                // Get the FNOL Type
                FNOLType = GetFNOLTypeName(FNOLRec["FNOLType_Id"].ToString()).ToUpper();
                if (FNOLType == null || FNOLType == string.Empty)
                    throw new Exception("Error getting FNOL Type!");

                // Build the email body HTML

                strBody = strBody + "<html><head><style type='text/css'>.headline{font-family:Verdana;font-size:medium;font-weight:bold;}.subheadline{font-family:Verdana;font-size:small;font-weight:bold;}.normaltext{font-family:Verdana;font-size:smaller;}a:link {color: #093F70;text-decoration:none;font-size: 12px; font-family: Verdana;font-weight:bold;}a:visited {color: #093F70;text-decoration: none; font-size: 12px; font-family: Verdana;font-weight:bold;}a:hover {color: #990000;text-decoration: none; font-size: 12px; font-family: Verdana;font-weight:bold;}a:active {color: #093F70;text-decoration: none; font-size: 12px; font-family: Verdana;font-weight:bold;}</style></head><body><form><table align='center' width='600' id='tblLoss' cellpadding='4' border='1' style='border-color:inherit'>"  ;

                // ERRORS AND WARNINGS
                // Errors
                if (ErrorMessages != null && ErrorMessages.Count > 0)
                {
                    strBody += "<tr>" ;
                    strBody += "<td class='subheadline' colspan='2' align='left' style='textdecoration=underline;'>ERRORS:</td>"  ;
                    strBody += "</tr>"  ;
                    foreach (string errmsg in ErrorMessages)
                    {
                        strBody += "<tr>"  ;
                        strBody += "<td colspan=2 class='normaltext' align='left' style='Vertical-Align:top;font-weight:700;color:red;'>" + "* " + errmsg + "</td>"  ;
                        strBody += "</tr>"  ;
                    }
                    strBody += "<tr><td colspan=2>&nbsp;</td></tr>"  ;
                }
                // Warnings
                if (WarningMessages != null && WarningMessages.Count > 0)
                {
                    strBody += "<tr>"  ;
                    strBody += "<td class='subheadline' colspan='2' align='left' style='textdecoration=underline;'>WARNINGS:</td>"  ;
                    strBody += "</tr>"  ;
                    foreach (string wmsg in WarningMessages)
                    {
                        strBody += "<tr>"  ;
                        strBody += "<td colspan=2 class='normaltext' align='left' style='Vertical-Align:top;font-weight:700;color:red;'>" + "* " + wmsg + "</td>"  ;
                        strBody += "</tr>"  ;
                    }
                    strBody += "<tr><td colspan=2>&nbsp;</td></tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "Comments"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Notes To Adjuster</td>"  ;
                    strBody = strBody + "</tr>"  ;
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right' style='Vertical-Align:top;font-weight:700;color:red;'>Comments</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left' style='font-weight:700;color:red;'>" + ScrubText(FNOLRec["Comments"].ToString()) + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                // General Information - All this data should be there, it's required
                strBody = strBody + "<tr>"  ;
                strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Basic Information</td>"  ;
                strBody = strBody + "</tr>"  ;

                // Only show Diamond Claim number if we successfully generated one
                if (DataFieldHasValue(FNOLRec, "DiamondClaimNumber"))
                    strBody = strBody + "<tr><td class='normaltext' width='50%' align='right'>Diamond Claim Number</td><td class='normaltext' width='50%' align='left'>" + FNOLRec["diamondClaimNumber"].ToString() + "</td></tr>";

                if ((FNOLRec["CAT"])!=null && System.Convert.ToBoolean(FNOLRec["CAT"]))
                {
                    // If CAT, make the text RED BOLD
                    ct = "YES";
                    strBody = strBody + "<tr style=color:red;font-weight:700;>";
                }
                else
                {
                    ct = "NO";
                    strBody = strBody + "<tr>"  ;
                }
                strBody = strBody + "<td class='normaltext' width='50%' align='right'>CAT</td>"  ;
                strBody = strBody + "<td class='normaltext' width='50%' align='left'>" + ct + "</td>"  ;
                strBody = strBody + "</tr>"  ;

                if (DataFieldHasValue(FNOLRec, "EntryDate"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' width='50%' align='right'>Date</td>"  ;
                    strBody = strBody + "<td class='normaltext' width='50%' align='left'>" + (DateTime)FNOLRec["EntryDate"]+ "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "PolicyNumber"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Policy Number</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["PolicyNumber"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "LossDate"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Loss Date</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["LossDate"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "AdjusterName"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Assigned Adjuster</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["AdjusterName"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "AssignedBy"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Assigned By</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["AssignedBy"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "DateAssignedToAdjuster"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Date Assigned</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["DateAssignedToAdjuster"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "AgencyName"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Agency Name</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["AgencyName"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "AgencyPhone"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Agency Phone</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["AgencyPhone"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "AgencyFAX"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Agency Fax</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["AgencyFAX"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                // Auto Deductibles
                if (DataFieldHasValue(FNOLRec, "CompDed"))
                {
                    strBody = strBody + "<tr id='CompDeductRow'>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Comprehensive Deductible</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["CompDed"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "CollDed"))
                {
                    strBody = strBody + "<tr id='CollDeductRow'>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Collision Deductible</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["CollDed"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "RentDed"))
                {
                    strBody = strBody + "<tr id='RentDeductRow'>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Rental Deductible</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["RentDed"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "DeductibleAmount"))
                {
                    strBody = strBody + "<tr id='PropGenDeductRow'>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Deductible Amount</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["DeductibleAmount"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                // Package Part
                if (DataFieldHasValue(FNOLRec, "PackagePart"))
                {
                    strBody = strBody + "<tr id='PackageRow'>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Package Part</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["PackagePart"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                // Insured Info
                strBody = strBody + "<tr>"  ;
                strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Insured Information</td>"  ;
                strBody = strBody + "</tr>"  ;

                if (DataFieldHasValue(FNOLRec, "PrimaryContact"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Primary Contact</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["PrimaryContact"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "InsuredFirst"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Insured First</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsuredFirst"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "InsuredLast"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Insured Last</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsuredLast"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "InsuredHomePhone"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Insured Home Phone</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsuredHomePhone"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "InsuredBusinessPhone"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Insured Business Phone</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsuredBusinessPhone"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "InsuredCellPhone"))
                {
                    strBody = strBody + "<tr>"  ; 
                    strBody = strBody + "<td class='normaltext' align='right'>Insured Cell Phone</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsuredCellPhone"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "InsuredFAX"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Insured FAX</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsuredFAX"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "InsuredEmail"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Insured Email</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsuredEmail"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "InsuredAddress"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Insured Address</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsuredAddress"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "InsuredCity"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Insured City</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsuredCity"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "InsuredState"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Insured State</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsuredState"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "InsuredZip"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Insured Zip</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsuredZip"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                // Contact Info
                if (DataFieldHasValue(FNOLRec, "ContactHomePhone") || !DataFieldHasValue(FNOLRec, "ContactBusinessPhone") || !DataFieldHasValue(FNOLRec, "ContactCellPhone") || !DataFieldHasValue(FNOLRec, "ContactTime"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Contact (If different than Insured)</td>"  ;
                    strBody = strBody + "</tr>"  ;

                    if (DataFieldHasValue(FNOLRec, "ContactHomePhone"))
                    {
                        strBody = strBody + "<tr>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Home Phone</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["ContactHomePhone"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }
                    if (DataFieldHasValue(FNOLRec, "ContactBusinessPhone"))
                    {
                        strBody = strBody + "<tr>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Business Phone</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["ContactBusinessPhone"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }
                    if (DataFieldHasValue(FNOLRec, "ContactCellPhone"))
                    {
                        strBody = strBody + "<tr>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Cell Phone</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["ContactCellPhone"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }
                    if (DataFieldHasValue(FNOLRec, "ContactTime"))
                    {
                        strBody = strBody + "<tr>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Best Time to Contact</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["ContactTime"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }
                }

                // Insured Vehicle Owner/Insured Vehicle Driver/Other Vehicle Driver only applies to auto policies
                if (FNOLType == "AUTO" | FNOLType == "COMMERCIAL AUTO")
                {
                    // Insured Vehicle Owner
                    if (DataFieldHasValue(FNOLRec, "InsVehOwnerFirst") && DataFieldHasValue(FNOLRec, "InsVehOwnerLast"))
                    {
                        strBody = strBody + "<tr id='InsVehOwnerRow'>"  ;
                        strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Insured Vehicle Owner</td>"  ;
                        strBody = strBody + "</tr>"  ;

                        strBody = strBody + "<tr id='InsVehOwnerNameRow'>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Owner Name</td>"  ;
                        if (DataFieldHasValue(FNOLRec, "InsVehOwnerMiddle"))
                            strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehOwnerFirst"].ToString() + " " + FNOLRec["InsVehOwnerMiddle"].ToString() + " " + FNOLRec["InsVehOwnerLast"].ToString() + "</td>"  ;
                        else
                            strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehOwnerFirst"].ToString() + " " + FNOLRec["InsVehOwnerLast"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;

                        if (DataFieldHasValue(FNOLRec, "InsVehOwnerAddress"))
                        {
                            strBody = strBody + "<tr id='InsVehOwnerAddRow'>"  ;
                            strBody = strBody + "<td class='normaltext' align='right'>Owner Address</td>"  ;
                            strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehOwnerAddress"].ToString() + "</td>"  ;
                            strBody = strBody + "</tr>"  ;
                        }

                        if (DataFieldHasValue(FNOLRec, "InsVehOwnerCity"))
                        {
                            strBody = strBody + "<tr id='InsVehOwnerCityRow'>"  ;
                            strBody = strBody + "<td class='normaltext' align='right'>Owner City</td>"  ;
                            strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehOwnerCity"].ToString() + "</td>"  ;
                            strBody = strBody + "</tr>"  ;
                        }

                        if (DataFieldHasValue(FNOLRec, "InsVehOwnerState"))
                        {
                            strBody = strBody + "<tr id='InsVehOwnerStateRow'>"  ;
                            strBody = strBody + "<td class='normaltext' align='right'>Owner State</td>"  ;
                            strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehOwnerState"].ToString() + "</td>"  ;
                            strBody = strBody + "</tr>"  ;
                        }

                        if (DataFieldHasValue(FNOLRec, "InsVehOwnerZip"))
                        {
                            strBody = strBody + "<tr id='InsVehOwnerZipRow'>"  ;
                            strBody = strBody + "<td class='normaltext' align='right'>Owner Zip</td>"  ;
                            strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehOwnerZip"].ToString() + "</td>"  ;
                            strBody = strBody + "</tr>"  ;
                        }
                    }

                    // Insured Vehicle Driver
                    strBody = strBody + "<tr id='InsVehDriverRow'>"  ;
                    strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Insured Vehicle Driver</td>"  ;
                    strBody = strBody + "</tr>"  ;

                    if (DataFieldHasValue(FNOLRec, "InsVehDriverFirst"))
                    {
                        strBody = strBody + "<tr id='InsVehDriverFirstRow'>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Driver First</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehDriverFirst"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "InsVehDriverMiddle"))
                    {
                        strBody = strBody + "<tr id='InsVehDriverMiddleRow'>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Driver Middle</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehDriverMiddle"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "InsVehDriverLast"))
                    {
                        strBody = strBody + "<tr id='InsVehDriverLastRow'>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Driver Last</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehDriverLast"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "InsVehDriverHomePhone"))
                    {
                        strBody = strBody + "<tr id='InsVehDriverHPRow'>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Driver Home Phone</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehDriverHomePhone"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "InsVehDriverBusinessPhone"))
                    {
                        strBody = strBody + "<tr id='InsVehDriverBPRow'>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Driver Business Phone</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehDriverBusinessPhone"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "InsVehDriverCellPhone"))
                    {
                        strBody = strBody + "<tr id='InsVehDriverCellRow'>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Driver Cell Phone</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehDriverCellPhone"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "InsVehDriverAddress"))
                    {
                        strBody = strBody + "<tr id='InsVehDriverAddRow'>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Driver Address</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehDriverAddress"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "InsVehDriverCity"))
                    {
                        strBody = strBody + "<tr id='InsVehDriverCityRow'>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Driver City</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehDriverCity"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "InsVehDriverState"))
                    {
                        strBody = strBody + "<tr id='InsVehDriverStateRow'>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Driver State</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehDriverState"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "InsVehDriverZip"))
                    {
                        strBody = strBody + "<tr id='InsVehDriverZipRow'>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Driver Zip</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehDriverZip"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    // Other Vehicle Owner
                    PersonsTable = null/* TODO Change to default(_) if this is not a reference type */;
                    PersonsTable = GetFNOLPersonsByType(FNOL_Id, CommonObjects.Enums.Enums.PersonType_Enum.OtherVehicleOwner ,ref err);
                    if (PersonsTable != null && PersonsTable.Rows.Count > 0)
                    {
                        ndx = 0;
                        foreach (DataRow ovo in PersonsTable.Rows)
                        {
                            ndx += 1;
                            // Header Row
                            strBody = strBody + "<tr id='OthVehOwner" + ndx.ToString() + "Row'>"  ;
                            if (PersonsTable.Rows.Count == 1)
                                strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Other Vehicle Owner</td>"  ;
                            else
                                strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Other Vehicle Owner " + ndx.ToString() + "</td>"  ;
                            strBody = strBody + "</tr>"  ;

                            // Format the persons string for display
                            string personStr = BuildPersonDisplayString(ovo,ref err);
                            if (personStr == null)
                                throw new Exception(err);

                            // Data Row
                            strBody = strBody + "<tr id='OthVehOwner" + ndx.ToString() + "FirstRow'>"  ;
                            strBody = strBody + "<td colspan='2' class='normaltext' align='left'>" + personStr + "</td>"  ;
                            strBody = strBody + "</tr>"  ;
                        }
                    }
                    else
                    {
                        // NO DATA
                        // Header Row
                        strBody = strBody + "<tr id='OthVehOwnerRow'>"  ;
                        strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Other Vehicle Owners</td>"  ;
                        strBody = strBody + "</tr>"  ;

                        // Data Row
                        strBody = strBody + "<tr id='OthVehOwnerFirstRow'>"  ;
                        strBody = strBody + "<td colspan='2' class='normaltext' align='center'>" + "** No Other Vehicle Owners found **" + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    // Other Vehicle Drivers
                    PersonsTable = null/* TODO Change to default(_) if this is not a reference type */;
                    PersonsTable = GetFNOLPersonsByType(FNOL_Id, CommonObjects.Enums.Enums.PersonType_Enum.OtherVehicleDriver , ref err);
                    if (PersonsTable != null && PersonsTable.Rows.Count > 0)
                    {
                        ndx = 0;
                        foreach (DataRow drv in PersonsTable.Rows)
                        {
                            ndx += 1;
                            // Header Row
                            strBody = strBody + "<tr id='OthVehDriver" + ndx.ToString() + "Row'>"  ;
                            if (PersonsTable.Rows.Count == 1)
                                strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Other Vehicle Driver</td>"  ;
                            else
                                strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Other Vehicle Driver " + ndx.ToString() + "</td>"  ;
                            strBody = strBody + "</tr>"  ;

                            // Format the persons string for display
                            string personStr = BuildPersonDisplayString(drv,ref err);
                            if (personStr == null)
                                throw new Exception(err);

                            // Data Row
                            strBody = strBody + "<tr id='OthVehDriver" + ndx.ToString() + "FirstRow'>"  ;
                            strBody = strBody + "<td colspan='2' class='normaltext' align='left'>" + personStr + "</td>"  ;
                            strBody = strBody + "</tr>"  ;
                        }
                    }
                    else
                    {
                        // NO DATA
                        // Header Row
                        strBody = strBody + "<tr id='OthVehDriverRow'>"  ;
                        strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Other Vehicle Drivers</td>"  ;
                        strBody = strBody + "</tr>"  ;

                        // Data Row
                        strBody = strBody + "<tr id='OthVehDriverFirstRow'>"  ;
                        strBody = strBody + "<td colspan='2' class='normaltext' align='center'>" + "** No Other Vehicle Drivers found **" + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }
                }

                // Loss Information
                strBody = strBody + "<tr>"  ;
                strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Loss Information</td>"  ;
                strBody = strBody + "</tr>"  ;

                if (DataFieldHasValue(FNOLRec, "LossLocation"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Loss Location</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["LossLocation"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "LossAddress"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Loss Address</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["LossAddress"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "LossCity"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Loss City</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["LossCity"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "LossState"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Loss State</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["LossState"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "LossZip"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Loss Zip</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["LossZip"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "LossDescription"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Loss Description</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["LossDescription"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                // Loss kind only applies to property
                if (FNOLType == "PROPERTY" && DataFieldHasValue(FNOLRec, "LossKindDescription"))
                {
                    strBody = strBody + "<tr id='LossKindRow'>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Kind of Loss</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["LossKindDescription"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (DataFieldHasValue(FNOLRec, "LossAmount"))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Estimated Loss Amount</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["LossAmount"].ToString() + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                if (System.Convert.ToBoolean(FNOLRec["PoliceContacted"]))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='normaltext' align='right'>Police Contacted</td>"  ;
                    strBody = strBody + "<td class='normaltext' align='left'>Yes</td>"  ;
                    strBody = strBody + "</tr>"  ;
                    if (DataFieldHasValue(FNOLRec, "DepartmentName"))
                    {
                        strBody = strBody + "<tr>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Department Name</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["DepartmentName"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }
                    if (DataFieldHasValue(FNOLRec, "ReportNumber"))
                    {
                        strBody = strBody + "<tr>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Report Number</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["ReportNumber"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }
                }

                // Vehicle Info
                // Only applies to auto policies
                if (FNOLType == "AUTO" || FNOLType == "COMMERCIAL AUTO")
                {
                    strBody = strBody + "<tr id='VehInfoRow'>"  ;
                    strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Vehicle Information</td>"  ;
                    strBody = strBody + "</tr>"  ;

                    if (DataFieldHasValue(FNOLRec, "InsVehicleMake"))
                    {
                        strBody = strBody + "<tr id='VehMakeRow'>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Vehicle Make</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehicleMake"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "InsVehicleModel"))
                    {
                        strBody = strBody + "<tr id='VehModelRow'>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Vehicle Model</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehicleModel"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "InsVehicleYear"))
                    {
                        strBody = strBody + "<tr id='VehYearRow'>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Vehicle Year</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehicleYear"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "InsVehicleVIN"))
                    {
                        strBody = strBody + "<tr id='VehVINRow'>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Vehicle VIN</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehicleVIN"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "InsVehiclePlateNumber"))
                    {
                        strBody = strBody + "<tr id='VehPlateRow'>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Vehicle Plate Number</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehiclePlateNumber"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "InsVehicleDamage"))
                    {
                        strBody = strBody + "<tr id='VehDamageRow'>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Vehicle Damage</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehicleDamage"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "InsVehicleDamageAmount"))
                    {
                        strBody = strBody + "<tr id='VehDamageAmtRow'>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Vehicle Damage Amount</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehicleDamageAmount"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "InsVehicleLocation"))
                    {
                        strBody = strBody + "<tr id='VehWhereDamageRow'>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Location of Damaged Vehicle</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["InsVehicleLocation"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }
                }

                // Claimant/Injured
                // Does not apply to Property claims
                if (FNOLType != "PROPERTY")
                {
                    PersonsTable = null/* TODO Change to default(_) if this is not a reference type */;
                    PersonsTable = GetFNOLPersonsByType(FNOL_Id, CommonObjects.Enums.Enums.PersonType_Enum.Injured ,ref err);
                    if (PersonsTable != null && PersonsTable.Rows.Count > 0)
                    {
                        ndx = 0;
                        foreach (DataRow clm in PersonsTable.Rows)
                        {
                            ndx += 1;
                            // Header Row
                            strBody = strBody + "<tr id='Claimant" + ndx.ToString() + "Row'>"  ;
                            if (PersonsTable.Rows.Count == 1)
                            {
                                if (FNOLType == "LIABILITY")
                                    strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Claimant Information</td>"  ;
                                else
                                    strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Injured Information </td>"  ;
                            }
                            else if (FNOLType == "LIABILITY")
                                strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Claimant " + ndx.ToString() + "</td>"  ;
                            else
                                strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Injured " + ndx.ToString() + "</td>"  ;
                            strBody = strBody + "</tr>"  ;

                            // Format the persons string for display
                            string personStr = BuildPersonDisplayString(clm,ref err);
                            if (personStr == null)
                                throw new Exception(err);

                            // Data Row
                            strBody = strBody + "<tr id='Claimant" + ndx.ToString() + "FirstRow'>"  ;
                            strBody = strBody + "<td colspan='2' class='normaltext' align='left'>" + personStr + "</td>"  ;
                            strBody = strBody + "</tr>"  ;
                        }
                    }
                    else
                    {
                        // NO DATA
                        // Header Row
                        ndx += 1;
                        strBody = strBody + "<tr id='ClaimantInfoRow'>"  ;
                        if (FNOLType == "LIABILITY")
                            // For LIABILITY, we show CLAIMANT info
                            strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Claimant Information</td>"  ;
                        else
                            // For AUTO we show INJURED info
                            strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Injured Information</td>"  ;
                        strBody = strBody + "</tr>"  ;

                        // Data Row
                        strBody = strBody + "<tr id='ClaimantFirstRow'>"  ;
                        if (FNOLType == "LIABILITY")
                            // For LIABILITY, we show CLAIMANT info
                            strBody = strBody + "<td colspan='2' class='normaltext' align='center'>" + "** No Claimants found **" + "</td>"  ;
                        else
                            // For AUTO we show INJURED info
                            strBody = strBody + "<td colspan='2' class='normaltext' align='center'>" + "** No Claimants found **" + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }
                }

                // Property Damage/Property Information
                if (DataFieldHasValue(FNOLRec, "PropertyDescription") || !DataFieldHasValue(FNOLRec, "PropertyDamage") || !DataFieldHasValue(FNOLRec, "PropertyDamageAmount"))
                {
                    strBody = strBody + "<tr>"  ;
                    if (FNOLType == "LIABILITY")
                        // For LIABILITY the section is PROPERTY DAMAGE
                        strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Property Damage / Injury Information</td>"  ;
                    else
                        // For AUTO and PROPERTY the section is PROPERTY INFORMATION
                        strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Property Information</td>"  ;
                    strBody = strBody + "</tr>"  ;

                    // Injury description only applies to LIABILITY
                    if (FNOLType == "LIABILITY")
                    {
                        strBody = strBody + "<tr id='InjuredDescRow2'>";
                        strBody = strBody + "<td class='normaltext' align='right'>Injury Description (if applicable)</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["PropertyInjuryDescription"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "PropertyDescription"))
                    {
                        strBody = strBody + "<tr>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Property Description</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["PropertyDescription"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "PropertyDamage"))
                    {
                        strBody = strBody + "<tr>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Property Damage</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["PropertyDamage"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "PropertyDamageAmount".ToString()))
                    {
                        strBody = strBody + "<tr>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Property Damage Amount</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["PropertyDamageAmount"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "PropertyLocation"))
                    {
                        strBody = strBody + "<tr>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Where Property Damage</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["PropertyLocation"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    // Other Liability
                    // Only applies to LIABILITY
                    if (FNOLType == "LIABILITY")
                    {
                        if (DataFieldHasValue(FNOLRec, "OtherLiabilityDescription"))
                        {
                            strBody = strBody + "<tr id='OtherLiabDescRow'>"  ;
                            strBody = strBody + "<td class='normaltext' align='right'>Other Liability Description</td>"  ;
                            strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["OtherLiabilityDescription"].ToString() + "</td>"  ;
                            strBody = strBody + "</tr>"  ;
                        }

                        if (DataFieldHasValue(FNOLRec, "OtherLiabilityClaimantName"))
                        {
                            strBody = strBody + "<tr id='OtherLiabNameRow'>"  ;
                            strBody = strBody + "<td class='normaltext' align='right'>Other Liability Claimant Name</td>"  ;
                            strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["OtherLiabilityClaimantName"].ToString() + "</td>"  ;
                            strBody = strBody + "</tr>"  ;
                        }

                        if (DataFieldHasValue(FNOLRec, "OtherLiabilityClaimantAddress"))
                        {
                            strBody = strBody + "<tr id='OtherLiabAddRow'>"  ;
                            strBody = strBody + "<td class='normaltext' align='right'>Other Liability Claimant Address</td>"  ;
                            strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["OtherLiabilityClaimantAddress"].ToString() + "</td>"  ;
                            strBody = strBody + "</tr>"  ;
                        }

                        if (DataFieldHasValue(FNOLRec, "OtherLiabilityClaimantContactNumbers"))
                        {
                            strBody = strBody + "<tr id='OtherLiabContactsRow'>"  ;
                            strBody = strBody + "<td class='normaltext' align='right'>Other Liability Claimant Contact #s</td>"  ;
                            strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["OtherLiabilityClaimantContactNumbers"].ToString() + "</td>"  ;
                            strBody = strBody + "</tr>"  ;
                        }

                        // Other claimant 2
                        if (DataFieldHasValue(FNOLRec, "OtherLiability2Description"))
                        {
                            strBody = strBody + "<tr id='OtherLiabDescRow'>"  ;
                            strBody = strBody + "<td class='normaltext' align='right'>Other Liability Description</td>"  ;
                            strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["OtherLiability2Description"].ToString() + "</td>"  ;
                            strBody = strBody + "</tr>"  ;
                        }

                        if (DataFieldHasValue(FNOLRec, "OtherLiability2ClaimantName"))
                        {
                            strBody = strBody + "<tr id='OtherLiabNameRow'>"  ;
                            strBody = strBody + "<td class='normaltext' align='right'>Other Liability Claimant Name</td>"  ;
                            strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["OtherLiability2ClaimantName"].ToString() + "</td>"  ;
                            strBody = strBody + "</tr>"  ;
                        }

                        if (DataFieldHasValue(FNOLRec, "OtherLiability2ClaimantAddress"))
                        {
                            strBody = strBody + "<tr id='OtherLiabAddRow'>"  ;
                            strBody = strBody + "<td class='normaltext' align='right'>Other Liability Claimant Address</td>"  ;
                            strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["OtherLiability2ClaimantAddress"].ToString() + "</td>"  ;
                            strBody = strBody + "</tr>"  ;
                        }

                        if (DataFieldHasValue(FNOLRec, "OtherLiability2ClaimantContactNumbers"))
                        {
                            strBody = strBody + "<tr id='OtherLiabContactsRow'>"  ;
                            strBody = strBody + "<td class='normaltext' align='right'>Other Liability Claimant Contact #s</td>"  ;
                            strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["OtherLiability2ClaimantContactNumbers"].ToString() + "</td>"  ;
                            strBody = strBody + "</tr>"  ;
                        }

                        // Other claimant 3
                        if (DataFieldHasValue(FNOLRec, "OtherLiability3Description"))
                        {
                            strBody = strBody + "<tr id='OtherLiabDescRow'>"  ;
                            strBody = strBody + "<td class='normaltext' align='right'>Other Liability Description</td>"  ;
                            strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["OtherLiabilityDescription"].ToString() + "</td>"  ;
                            strBody = strBody + "</tr>"  ;
                        }

                        if (DataFieldHasValue(FNOLRec, "OtherLiability3ClaimantName"))
                        {
                            strBody = strBody + "<tr id='OtherLiabNameRow'>"  ;
                            strBody = strBody + "<td class='normaltext' align='right'>Other Liability Claimant Name</td>"  ;
                            strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["OtherLiabilityClaimantName"].ToString() + "</td>"  ;
                            strBody = strBody + "</tr>"  ;
                        }

                        if (DataFieldHasValue(FNOLRec, "OtherLiability3ClaimantAddress"))
                        {
                            strBody = strBody + "<tr id='OtherLiabAddRow'>"  ;
                            strBody = strBody + "<td class='normaltext' align='right'>Other Liability Claimant Address</td>"  ;
                            strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["OtherLiabilityClaimantAddress"].ToString() + "</td>"  ;
                            strBody = strBody + "</tr>"  ;
                        }

                        if (DataFieldHasValue(FNOLRec, "OtherLiability3ClaimantContactNumbers"))
                        {
                            strBody = strBody + "<tr id='OtherLiabContactsRow'>"  ;
                            strBody = strBody + "<td class='normaltext' align='right'>Other Liability Claimant Contact #s</td>"  ;
                            strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["OtherLiabilityClaimantContactNumbers"].ToString() + "</td>"  ;
                            strBody = strBody + "</tr>"  ;
                        }
                    }

                    if (DataFieldHasValue(FNOLRec, "OtherPartyPolicyNumber"))
                    {
                        strBody = strBody + "<tr>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Other Party Policy Number</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["OtherPartyPolicyNumber"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "OtherPartyInsurer"))
                    {
                        strBody = strBody + "<tr>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Other Party Insurer</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["OtherPartyInsurer"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }
                }

                // Witness
                // Does not apply to Liability  ** YES IT DOES MGB 4/5/16
                // Other Vehicle Drivers
                PersonsTable = null/* TODO Change to default(_) if this is not a reference type */;
                PersonsTable = GetFNOLPersonsByType(FNOL_Id, CommonObjects.Enums.Enums.PersonType_Enum.Witness,ref err);
                if (PersonsTable != null && PersonsTable.Rows.Count > 0)
                {
                    ndx = 0;
                    foreach (DataRow wit in PersonsTable.Rows)
                    {
                        ndx += 1;
                        // Header Row
                        strBody = strBody + "<tr id='Witness" + ndx.ToString() + "Row'>"  ;
                        if (PersonsTable.Rows.Count == 1)
                            strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Witness</td>"  ;
                        else
                            strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Witness " + ndx.ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;

                        // Format the persons string for display
                        string personStr = BuildPersonDisplayString(wit,ref err);
                        if (personStr == null)
                            throw new Exception(err);

                        // Data Row
                        strBody = strBody + "<tr id='Witness" + ndx.ToString() + "FirstRow'>"  ;
                        strBody = strBody + "<td colspan='2' class='normaltext' align='left'>" + personStr + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }
                }
                else
                {
                    // NO DATA
                    // Header Row
                    strBody = strBody + "<tr id='WitnessRow'>"  ;
                    strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Witnesses</td>"  ;
                    strBody = strBody + "</tr>"  ;

                    // Data Row
                    strBody = strBody + "<tr id='WitnessFirstRow'>"  ;
                    strBody = strBody + "<td colspan='2' class='normaltext' align='center'>" + "** No Witnesses found **" + "</td>"  ;
                    strBody = strBody + "</tr>"  ;
                }

                // Misc Info
                if (System.Convert.ToBoolean(FNOLRec["ManualDraft"]) || DataFieldHasValue(FNOLRec, "ReportedBy") || DataFieldHasValue(FNOLRec, "Comments_AddlInfo") || DataFieldHasValue(FNOLRec, "ConfirmEmailAddress") || (System.Convert.ToBoolean(FNOLRec["AddlDocoMail"])) || System.Convert.ToBoolean(FNOLRec["AddlDocoFAX"]) || System.Convert.ToBoolean(FNOLRec["AddlDocoEmail"]))
                {
                    strBody = strBody + "<tr>"  ;
                    strBody = strBody + "<td class='subheadline' colspan='2' align='center'>Misc Information</td>"  ;
                    strBody = strBody + "</tr>"  ;

                    if (System.Convert.ToBoolean(FNOLRec["ManualDraft"]))
                    {
                        if (DataFieldHasValue(FNOLRec, "ManualDraftPayee") && DataFieldHasValue(FNOLRec, "ManualDraftCheckAmount") && DataFieldHasValue(FNOLRec, "ManualDraftCheckNumber"))
                        {
                            strBody = strBody + "<tr><td class='normaltext' align='right'>Manual Draft</td>"  ;
                            strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["ManualDraftPayee"].ToString() + " #" + FNOLRec["ManualDraftCheckNumber"].ToString() + " " + FNOLRec["ManualDrafCheckAmount"].ToString() + "</td></tr>"  ;
                        }
                    }

                    if (DataFieldHasValue(FNOLRec, "ReportedBy"))
                    {
                        strBody = strBody + "<tr>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Reported By</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["ReportedBy"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "Comments_AddlInfo"))
                    {
                        strBody = strBody + "<tr>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Comments / Additional Information</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["Comments_AddlInfo"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (System.Convert.ToBoolean(FNOLRec["AddlDocoMail"]) || System.Convert.ToBoolean(FNOLRec["AddlDocoFAX"]) || System.Convert.ToBoolean(FNOLRec["AddlDocoMail"]))
                    {
                        strBody = strBody + "<tr>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Additional Documents to be sent by</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + GetBitValues(FNOLRec) + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }

                    if (DataFieldHasValue(FNOLRec, "ConfirmEmailAddress"))
                    {
                        strBody = strBody + "<tr>"  ;
                        strBody = strBody + "<td class='normaltext' align='right'>Confirmation Email Address</td>"  ;
                        strBody = strBody + "<td class='normaltext' align='left'>" + FNOLRec["ConfirmEmailAddress"].ToString() + "</td>"  ;
                        strBody = strBody + "</tr>"  ;
                    }
                }

                // DEC disclaimer
                strBody = strBody + "<tr><td colspan='2'>&nbsp;</td></tr>"  ;
                strBody = strBody + "<tr>"  ;
                strBody = strBody + "<td class='normaltext' align='center' colspan='2'>NOTE: If this loss was incurred on a previous policy image, you may need to obtain an older version of the Declaration for the policy image that was in force when the loss occurred.</td>"  ;
                strBody = strBody + "</tr>"  ;

                strBody = strBody + "</table></form></body></html>";

                return strBody;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                HandleError(ClassName, "GetFNOLEmailBody", ex,null);
                return null;
            }
        }
        public string GetBitValues(DataRow dr)
        {
            string ForwardDocs = null;

            try
            {
                if (System.Convert.ToBoolean(dr["AddlDocoMail"]))
                    ForwardDocs = "Mail";
                if (System.Convert.ToBoolean(dr["AddlDocoFax"]))
                {
                    if (ForwardDocs == "")
                        ForwardDocs = "Fax";
                    else
                        ForwardDocs = ForwardDocs + ", Fax";
                }
                if (System.Convert.ToBoolean(dr["AddlDocoEmail"]))
                {
                    if (ForwardDocs == "")
                        ForwardDocs = "Email";
                    else
                        ForwardDocs = ForwardDocs + ", Email";
                }
                return ForwardDocs;
            }
            catch (Exception ex)
            {
                HandleError(ClassName, "GetBitValues", ex,null);
                return "";
            }
        }
        public string BuildPersonDisplayString(DataRow personsRow, ref string err)
        {
            string str = string.Empty;
            try
            {
                // Name
                if (personsRow["FirstName"]!= null  || (personsRow["MiddleName"]!=null || personsRow["LastName"]!=null))
                {
                    str += "Name: ";
                    if (personsRow["FirstName"]!=null)
                        str += personsRow["FirstName"] + " ";
                    if (personsRow["MiddleName"]!= null)
                        str += personsRow["MiddleName"] + " ";
                    if (personsRow["LastName"] != null)
                        str += personsRow["LastName"] + "<br />";
                    str += "<br />";
                }

                // Phones
                if (personsRow["HomePhone"] !=null || personsRow["BusinessPhone"]!=null|| personsRow["CellPhone"]!=null || personsRow["FAX"]!=null)
                {
                    str += "Phone Numbers:" + "<br />" + "----------------------" + "<br />";

                    if (personsRow["HomePhone"] !=null)
                        str += "Home: " + personsRow["HomePhone"].ToString() + "<br />";
                    if (personsRow["BusinessPhone"]!=null)
                        str += "Business: " + personsRow["BusinessPhone"].ToString() + "<br />";
                    if (personsRow["CellPhone"] != null)
                        str += "Cell: " + personsRow["CellPhone"].ToString() + "<br />";
                    if (personsRow["FAX"] != null)
                        str += "FAX: " + personsRow["FAX"].ToString() + "<br />";
                    str += "<br />";
                }

                // Address
                if (personsRow["Address"] != null || personsRow["City"] != null || personsRow["State"] != null || personsRow["Zip"] != null)
                {
                    str +=  "Address: " + "<br />" + "-------------" + "<br />";
                    if (personsRow["Address"] != null)
                        str += personsRow["Address"].ToString() + "<br />";
                    if (personsRow["City"] != null)
                        str += personsRow["City"].ToString();
                    if (personsRow["State"] != null)
                    {
                        if (personsRow["City"] != null)
                            str += ", ";
                        str += GetDiamondStateAbbrev(personsRow["State"].ToString());
                    }
                    if (personsRow["Zip"] != null)
                    {
                        if (personsRow["City"] != null || personsRow["State"] != null)
                            str += " ";
                        str += personsRow["Zip"].ToString() + "<br />";
                    }
                    str += "<br />";
                }

                // Email
                if (personsRow["Email"] != null)
                {
                    str += "Email: " + personsRow["Email"].ToString() + "<br />";
                    str += "<br />";
                }

                // Injury Info
                if (personsRow["InjuryType"] != null && personsRow["InjuredAge"]!=null && personsRow["InjuryDescription"]!=null && personsRow["InjuryDescription"] != null)
                {
                    str += "Injury Information" + "<br />";
                    str += "--------------------" + "<br />";

                    if (personsRow["InjuryType"] != null)
                        str += "Injury Type: " + personsRow["InjuryType"].ToString() + "<br />";
                    if (personsRow["InjuredAge"] != null)
                        str += "Injured Age: " + personsRow["InjuredAge"].ToString() + "<br />";
                    if (personsRow["InjuryDescription"] != null)
                        str += "Injury Description: " + personsRow["InjuryDescription"].ToString() + "<br />";
                }

                return str;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                HandleError(ClassName, "BuildPersonDisplayString", ex,null);
                return null;
            }
        }
        public string GetDiamondStateAbbrev(string StateId)
        {
            SqlConnection conn = new SqlConnection(AppConfig.ConnFNOL);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable tbl = new DataTable();

            try
            {
                conn.Open();
                cmd.Connection = conn;
                 SQLselectObject sqsel = null/* TODO Change to default(_) if this is not a reference type */;
                sqsel = new SQLselectObject(conn.ToString(), "SELECT [state] FROM [State] WHERE state_id = " + StateId);
                DataTable dt = sqsel.GetDataTable();
                DataRow dataRow = tbl.Rows[0];

                if (sqsel.hasError)
                    throw new Exception("GetDiamondStateAbbrev Error: " + sqsel.errorMsg);
                else if (dt.Rows.Count > 0)
                    return dataRow["state"].ToString();
                else
                    return "err";
            }
            catch (Exception ex)
            {
                // HandleError(ClassName, "GetDiamondStateAbbrev", ex, null/* TODO Change to default(_) if this is not a reference type */);
                return "err";
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Dispose();
                cmd.Dispose();
                da.Dispose();
                tbl.Dispose();
            }
            
        }
        public DataTable GetFNOLPersonsByType(string FNOL_Id, CommonObjects.Enums.Enums.PersonType_Enum PersonType, ref string err)
        {
            SqlConnection conn = new SqlConnection(AppConfig.ConnFNOL);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable tbl = new DataTable();
            object rtn = null;

            try
            {
                conn.Open();

                // Get the person type ID
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                switch (PersonType)
                {
                    case CommonObjects.Enums.Enums.PersonType_Enum.Injured:
                        {
                            cmd.CommandText = "SELECT PersonType_Id FROM tbl_PersonType WHERE PersonType = 'Injured'";
                            break;
                        }

                    case CommonObjects.Enums.Enums.PersonType_Enum.OtherVehicleDriver:
                        {
                            cmd.CommandText = "SELECT PersonType_Id FROM tbl_PersonType WHERE PersonType = 'Other Vehicle Driver'";
                            break;
                        }

                    case CommonObjects.Enums.Enums.PersonType_Enum.OtherVehicleOwner:
                        {
                            cmd.CommandText = "SELECT PersonType_Id FROM tbl_PersonType WHERE PersonType = 'Other Vehicle Owner'";
                            break; 
                        }

                    case CommonObjects.Enums.Enums.PersonType_Enum.Witness:
                        {
                            cmd.CommandText = "SELECT PersonType_Id FROM tbl_PersonType WHERE PersonType = 'Witness'";
                            break; 
                        }

                    default:
                        {
                            throw new Exception("Invalid person type passed");
                            break;
                        }
                }
                rtn = cmd.ExecuteScalar();
                if (rtn == null)
                    throw new Exception("Error getting person type ID");

                // Get the desired records
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM tbl_Persons WHERE FNOL_Id = " + FNOL_Id + " AND PersonType_Id = " + rtn.ToString() + " ORDER BY Person_Id ASC";
                da.SelectCommand = cmd;
                da.Fill(tbl);

                if (tbl == null || tbl.Rows.Count <= 0)
                    return null/* TODO Change to default(_) if this is not a reference type */;

                return tbl;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                HandleError(ClassName, "GetFNOLPersonsByType", ex,null);
                return null/* TODO Change to default(_) if this is not a reference type */;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Dispose();
                cmd.Dispose();
                da.Dispose();
                tbl.Dispose();
            }
        }
        public string ScrubText(string TextValue)
        {
            string tmp = null;
            try
            {
                // All quotation marks must be removed
                tmp = TextValue.Replace("'", "");
                tmp = tmp.Replace("\"", "");

                // Remove special characters that will be flagged as illegal
                tmp = tmp.Replace("<", "");
                tmp = tmp.Replace("/>", "");
                tmp = tmp.Replace(">", "");
                tmp = tmp.Replace(":", "");

                return tmp;
            }
            catch (Exception ex)
            {
               // HandleError(ClassName, "ScrubText", ex, null/* TODO Change to default(_) if this is not a reference type */);
                return TextValue;
            }
        }
        public DataTable GetFNOLDocuments(string FNOL_Id, ref string err)
        {
            SqlConnection conn = new SqlConnection(AppConfig.ConnFNOL);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable tbl = new DataTable();

            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM tbl_FNOLDocuments WHERE FNOL_Id = " + FNOL_Id + " ORDER BY FNOL_Documents_Id ASC";
                da.SelectCommand = cmd;
                da.Fill(tbl);

                // If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Throw New Exception("No Documents Found!")
                if (tbl == null || tbl.Rows.Count <= 0)
                    return null/* TODO Change to default(_) if this is not a reference type */;

                return tbl;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                //HandleError(ClassName, "GetFNOLDocuments", ex);
                return null/* TODO Change to default(_) if this is not a reference type */;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Dispose();
                cmd.Dispose();
                da.Dispose();
                tbl.Dispose();
            }
        }
        public static List<string> CreateDECFiles(string PolicyNumber)
        {
            DiamondWebClass.DiamondPrinting prt = new DiamondWebClass.DiamondPrinting();
            DataTable dt = null/* TODO Change to default(_) if this is not a reference type */;
            byte[] DECByte = null;
            SqlConnection cn = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable tbl = new DataTable();
            int TopId = 0;
            DataRow[] drArray = null;
            int i = 0;
            string fname = null;
            string utc = null;
            List<string> filenames = new List<string>();
            string path = null;

            try
            {
                // First, get all of the DECS for the policy
                cn.ConnectionString = AppConfig.Conn;
                cn.Open();
                cmd.Connection = cn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_PolicyNumberDataCollection_Diamond";
                cmd.Parameters.AddWithValue("@collectionType", "Declarations");
                cmd.Parameters.AddWithValue("@DiamondPolicy", PolicyNumber);
                da.SelectCommand = cmd;
                da.Fill(tbl);
                // Next find the DEC id with the highest value
                foreach (DataRow dr in tbl.Rows)
                {
                    if (TopId == 0)
                        TopId = System.Convert.ToInt32(dr["id"]);
                    if (System.Convert.ToInt32(dr["id"]) > TopId)
                        TopId = System.Convert.ToInt32(dr["id"]);
                }

                // Set the path and check it
                path = AppConfig.DECFolder;
                if (System.IO.Directory.Exists(path))
                {
                    // If output folder exists, clean up any existing FNOL DEC files older than 1 hour
                    string[] dirfiles = System.IO.Directory.GetFiles(path, "*_FNOL_DEC_*.pdf");
                    if (dirfiles != null && dirfiles.Count() > 0)
                    {
                        foreach (string fn in dirfiles)
                        {
                            System.IO.FileInfo fi = new System.IO.FileInfo(fn);
                            //int MinDiff = DateTime.DateDiff(DateInterval.Minute, DateTime.Now, fi.CreationTime);
                            //if (MinDiff <= -60)
                            TimeSpan ts = DateTime.Now - fi.CreationTime;
                            if(ts.TotalMinutes <= -60)
                                fi.Delete();
                        }
                    }
                }
                else
                    // If output folder does not exist, create it
                    System.IO.Directory.CreateDirectory(path);

                utc = DateTime.Now.ToUniversalTime().ToString().Replace("/", "-").Replace(":", "");

                // Get all records with the top id
                drArray = tbl.Select("id = " + TopId);
                foreach (DataRow row in drArray)
                {
                    i += 1;
                    // Get the PDF byte array
                    DECByte = null;
                    DECByte = prt.printDec(null, Convert.ToInt32(row["Policy ID"]), TopId, row["Form Description"].ToString());

                    // Build the output file name: <policy>_FNOL_DEC_<#>_<timestamp>.pdf
                    fname = path + PolicyNumber + "_FNOL_DEC_" + i.ToString() + "_" + utc + ".pdf";

                    // Check for existing path and duplicate file name
                    if (System.IO.File.Exists(fname))
                        System.IO.File.Delete(fname);
                    filenames.Add(fname);

                    // Now that we have the Byte Array, convert it to a pdf file
                    FileStream fs = new FileStream(fname, System.IO.FileMode.Create);
                    fs.Write(DECByte, 0, DECByte.Length);
                    fs.Close();
                }
                return filenames;
            }
            catch (Exception ex)
            {
                HandleError(ClassName, "CreateDECFiles",ex,null);
                // Dim a As String = ex.Message
                return null;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                    cn.Close();
                cn.Dispose();
                cmd.Dispose();
            }
        }

        public string GetFNOLTypeName(string FNOLType_Id)
        {
            SqlConnection conn = new SqlConnection(AppConfig.ConnFNOL);
            SqlCommand cmd = new SqlCommand();
            object rtn = null;

            try
            {
               conn.Open();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT FNOLType from tbl_FNOLType WHERE FNOLType_Id = " + FNOLType_Id;
                rtn = cmd.ExecuteScalar();

                if (rtn == null)
                    return null;
                else
                    return rtn.ToString();
            }
            catch (Exception ex)
            {
              //  HandleError(ClassName, "GetFNOLTypeName", ex);
                return null;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn.Dispose();
                cmd.Dispose();
            }
        }
        public bool DataFieldHasValue(DataRow dr, string FieldName)
        {
            try
            {
                //if (dr[FieldName] != null)
                //    return false;
                //if (dr[FieldName].ToString() == string.Empty)
                //    return false;
                if (DBNull.Value.Equals(dr[FieldName]))//IsDBNull(
                    return false;
                if (dr[FieldName].ToString() =="")
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                //HandleError(ClassName, "DataFieldHasValue", ex);
                return false;
            }
        }
        private Int32 UploadToOnbase(byte[] data, string filename, string claimNumber)
        {
            CommonObjects.OnBase.UploadPayload package = new IFM.DataServicesCore.CommonObjects.OnBase.UploadPayload();
            package.SetBase64Payload(data);
            package.SetExtensionFromFullFileName(filename);

            package.SourceSystem = DocumentUpload.SourceSystems.FNOLAssignment;
            package.Keys.Add(new IFM.DataServicesCore.CommonObjects.OnBase.OnBaseKey() { Key = DocumentUpload.KeyTypes.claimNumber, Values = new[] { claimNumber } });
            package.Keys.Add(new IFM.DataServicesCore.CommonObjects.OnBase.OnBaseKey() { Key = DocumentUpload.KeyTypes.description, Values = new[] { "FNOL intranet upload." } });

            using (IFM.JsonProxyClient.ProxyClient proxy = new JsonProxyClient.ProxyClient(AppConfig.IFMDataServices_EndPointBaseUrl))
            {
                var response = proxy.Post("OnBase/Document/Upload", package);
                var responseText = proxy.GetResponsePayload(response);
                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Int32 documentId = 0;
                    Int32.TryParse(responseText, out documentId);
                    return documentId;
                }
                return 0;
            }
        }
        public static void HandleError(string ClassName, string RoutineName, Exception ex,Label msglabel)
        {
            global::IFM.IFMErrorLogging.ErrLog_Parameters_Structure rec = new IFM.IFMErrorLogging.ErrLog_Parameters_Structure();
            string err = null;
            try
            {
                // If a label was passed, set it's text
                if (msglabel != null)
                    msglabel.Text = ClassName + "(" + RoutineName + "): " + ex.Message;

                // IT Error Logging
                rec.ApplicationName = "FNOL Claim Assignment";
                rec.ClassName = "MemberPortal";
                rec.ErrorMessage = ex.Message;
                rec.LogDate = DateTime.Now;
                rec.RoutineName = RoutineName;
                rec.StackTrace = ex.StackTrace;
                IFM.IFMErrorLogging.WriteErrorLogRecord(rec,ref err);

                return;
            }
            catch (Exception ex1)
            {
                return;
            }
            return;
        }
        public void HandleError(string strClassName, string strRoutineName, ref Exception exc, string PolicyNum, Label MessageLabel,bool? blSendErrorEmail = false)
        {
          //  string strScript = "<script language=JavaScript>";
            string message = null;
            global::IFM.IFMErrorLogging.ErrLog_Parameters_Structure rec = new IFM.IFMErrorLogging.ErrLog_Parameters_Structure();
            string err = null;

            try
            {
                // MGB 4/6/16 Ignore thread abort errors
                if (exc.Message.ToUpper().Contains("THREAD WAS BEING ABORTED"))
                    return;

                // Build the message string
                message = "Error Detected in " + strClassName + "(" + strRoutineName + "): " + exc.Message;

                // Update the message label text if one was passed
                if (MessageLabel != null)
                    MessageLabel.Text = message;

                // Write the error log
                rec.ApplicationName = "IFM Data Services";
                rec.ClassName = strClassName;
                rec.ErrorMessage = exc.Message;
                rec.LogDate = DateTime.Now;
                rec.RoutineName = strRoutineName;
                rec.StackTrace = exc.StackTrace;
                IFM.IFMErrorLogging.WriteErrorLogRecord(rec,ref err);

                if ((bool)blSendErrorEmail)
                    SendErrorEmail("Error in IFM DataServices", PolicyNum, GetErrorString(ClassName, strRoutineName,ref exc, PolicyNum, MessageLabel), PolicyNum, MessageLabel);

                return;
            }
            catch (Exception ex)
            {
                // This will cause an infinite loop if the email server fails, so don't do this!
                // SendErrorEmail("FNOL Error", "none", GetErrorString(ClassName, "HandleError", ex))
                return;
            }
        }
        public void SendErrorEmail(string subject, string polnum, string bodyString, string policynum, Label errlabel)
        {
            try
            {
                using (EmailObject em = new EmailObject())
                {
                    em.MailHost = AppConfig.mailhost;
                    em.EmailSubject = subject + " " + polnum;
                    if (AppConfig.TestOrProd.ToUpper()== "TEST")
                        em.EmailFromAddress = "TEST-FNOLconfirmation@IndianaFarmers.com";
                    else
                        em.EmailFromAddress = "FNOLconfirmation@IndianaFarmers.com";
                    em.EmailToAddress = AppConfig.LossReportingErrorEmail;
                    em.EmailBody = bodyString;
                    em.SendEmail();
                }

                return;
            }
            catch (Exception ex)
            {
                // Not much to do here if the error email failed
                // If we tried to put the error handler here it would cause an infinite loop
                return;
            }
        }
        public string GetErrorString(string strClassName, string strRoutineName, ref Exception exc,  string policynum, Label errlabel)
        {
            try
            {
                return "Error detected in " + strClassName + "(" + strRoutineName + "): " + exc.Message;
            }
            catch (Exception ex)
            {
                HandleError(ClassName, "GetErrorString",ref ex,  policynum, errlabel);
                return null;
            }
        }
        public static bool InsertPersonsRecord(FNOLData_Structure rec, string FNOL_Id, int PersonsType_Id, string polnum, SqlConnection conn, SqlTransaction txn)
        {
            SqlCommand cmd = new SqlCommand();
            int rtn = -1;

            try
            {
                if (conn == null || conn.State != ConnectionState.Open)
                    throw new Exception("Connection string has not been initialized");

                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_Insert_tbl_persons";
                cmd.Transaction = txn;

                cmd.Parameters.AddWithValue("@PersonType_Id", PersonsType_Id);
                cmd.Parameters.AddWithValue("@FNOL_Id", FNOL_Id);
                cmd.Parameters.AddWithValue("@FirstName", rec.InsuredFirst);
                if (rec.InsuredMiddle != null)
                    cmd.Parameters.AddWithValue("@MiddleName", rec.InsuredMiddle);
                cmd.Parameters.AddWithValue("@LastName", rec.InsuredLast);
                if (rec.PhoneHome != null)
                    cmd.Parameters.AddWithValue("@HomePhone", rec.PhoneHome);
                if (rec.PhoneBusiness != null)
                    cmd.Parameters.AddWithValue("@BusinessPhone", rec.PhoneBusiness);
                if (rec.PhoneCell != null)
                    cmd.Parameters.AddWithValue("@CellPhone", rec.PhoneCell);
                if (rec.FAX != null)
                    cmd.Parameters.AddWithValue("@FAX", rec.FAX);
                //if (rec.Address != null)
                //    cmd.Parameters.AddWithValue("@Address", rec.Address);
                if (rec.InsuredAddressCity != null)
                    cmd.Parameters.AddWithValue("@City", rec.InsuredAddressCity);
                if (rec.InsuredAddressState != null)
                    cmd.Parameters.AddWithValue("@State", rec.InsuredAddressState);
                if (rec.InsuredAddressZip != null)
                    cmd.Parameters.AddWithValue("@Zip", rec.InsuredAddressZip);
                if (rec.Email != null)
                    cmd.Parameters.AddWithValue("@Email", rec.Email);
                rtn = cmd.ExecuteNonQuery();

                if (rtn == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                HandleError(ClassName, "InsertPersonsRecord", ex, null);
                return false;
            }
        }
        public static PolicyNumberObject PopulatePolicyInfo(string PolicyNumber, string LossDate)//, PolicyNumberObject PolicyInfoObject, string err)
        {
            PolicyNumberObject PolicyInfoObject = new PolicyNumberObject(PolicyNumber, AppConfig.Conn);
               
            try
            {
                //// The passed policy number object is populated.  If the policy number is the same as what was passed
                //// we don't need to repopulate the object
                //if (PolicyInfoObject != null && (!PolicyInfoObject.Equals(new PolicyNumberObject())))
                //{
                //    if (PolicyInfoObject.DiamondPolicyNumber == PolicyNumber)
                //        return true;
                //}

                // Either the policy number changed or the passed policy number object is empty, 
                // make the call to populate the passed policy number object
                // PolicyInfoObject = New PolicyNumberObject(PolicyNumber, strConn) '2/20/2019 note: this is using the connection string for the FNOL db (connFNOL) instead of the ifmtester db (conn key); only works because of other keys to use connDiamondReports
                // updated 2/20/2019 to fix legacy connection string... would cause an error if lookup fails in Diamond, policy has legacyNum, and logic needs to go back and look for active in Legacy
                if (PolicyInfoObject.hasError)
                    throw new Exception("This policy number is in an invalid format.");

                // set evaluation date to loss date to get info for that specific policy term
                PolicyInfoObject.EvaluationDateOrLossDate = LossDate;

                PolicyInfoObject.GetAllInsuredInfo = true;
                PolicyInfoObject.GetAllAgencyInfo = true;
                PolicyInfoObject.GetAllFNOLInfo = true;
                PolicyInfoObject.GetPolicyInfo();

                // Check to see that policy exists
                if (!PolicyInfoObject.hasPolicyInfo)
                {
                    if (PolicyInfoObject.hasPolicyInfoError)
                        throw new Exception("There was a problem locating this policy's information.");
                    else
                        throw new Exception("This policy could not be located.");
                }


                
            }
            catch (Exception ex)
            {
                HandleError(ClassName, "PopulatePolicyInfo", ex,null);
               
            }
            return PolicyInfoObject;
        }

        private static void SetLnImageNotePropsIfNeeded(DCO.Claims.LossNotice.LnImage lnImg)
        {
            if (lnImg != null)
            {
                if (AppConfig.DiamondClaimsFNOL_SetLnImageNoteProps == true)
                {
                    SetLnImageNotePropsToDefaultValues(lnImg);
                }
            }
        }
        private static void SetLnImageNotePropsToDefaultValues(DCO.Claims.LossNotice.LnImage lnImg)
        {
            if (lnImg != null)
            {
                bool onlyIfKeyExists = AppConfig.DiamondClaimsFNOL_SetLnImageNoteProps_OnlyIfKeyExists;

                bool notesKeyExists = false;
                string notes = AppConfig.DiamondClaimsFNOL_LnImageNotesDefault(ref notesKeyExists);
                if (notesKeyExists == true || onlyIfKeyExists == false)
                {
                    lnImg.Notes = notes;
                }

                bool notesTypeIdKeyExists = false;
                int notesTypeId = AppConfig.DiamondClaimsFNOL_LnImageNotesTypeIdDefault(ref notesTypeIdKeyExists);
                if (notesTypeIdKeyExists == true || onlyIfKeyExists == false)
                {
                    lnImg.NotesTypeId = notesTypeId;
                }

                bool noteTitleKeyExists = false;
                string noteTitle = AppConfig.DiamondClaimsFNOL_LnImageNoteTitleDefault(ref noteTitleKeyExists);
                if (noteTitleKeyExists == true || onlyIfKeyExists == false)
                {
                    lnImg.NoteTitle = noteTitle;
                }
            }
        }
    }
    public class FNOLResponseData
    {
        public bool AssignedSuccessfully;
        public string DiamondAdjuster_Id;
        public string AdjusterName;
        public string DateAssigned;
        public string AssignedBy;
        public string FNOLAdjusterID;
        public bool CAT;
        public int FNOL_ID;
        public string Group_Id;
        public string ErrMsg;
        public string AdjusterEmail;
        public int CliamNumber;
    }
    public struct EmailInfo_Structure_FNOLCA
    {
        public string ToAddress;
        public string CCAddress;
        public string PolicyHolderFirstName;
        public string PolicyHolderLastName;
        public string PolicyNumber;
        public string Body;
        public string SubjectLine_OPTIONAL;
        public string FromAddress_OPTIONAL;
        public string MailHost_OPTIONAL;
    }
    public struct FNOLData_Structure
    {
        public string DiamondClaimNumber;
        public string AdjusterName;
        public string TerritoryNumber;
        public DateTime ClaimDate;
        public string PolicyNumber;
        public DateTime LossDate;
        public string AgencyName;
        public string PrimaryContact;
        public string InsuredFirst;
        public string InsuredLast;
        public string InsuredMiddle;
        public string PhoneHome;
        public string PhoneBusiness;
        public string PhoneCell;
        public string InsuredAddressStreet;
        public string InsuredAddressCity;
        public string InsuredAddressState;
        public string InsuredAddressZip;
        public string Description;
        public string LOB;
        public bool CAT;
        public string FAX;
        public string Email;
        public String Address;
    }

}

using Dapper;
using IFM.DataServicesCore.CommonObjects.OMP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.BusinessLogic.OMP
{
    public class ClaimsManager : BusinessLogicBase
    {
        public bool SubmitClaim(ClaimReport claimData)
        {
            try
            {
                using (IDbConnection conn = OpenConnection(AppConfig.Conn))
                {
                    conn.Execute("Insert into tbl_m_claimqueue (IsActive,IpAddress,PolicyNumber,SourceSystem,LossDateTime,FullName,LossType,Phone,Email,Description,InjuriesExist,ProcessedClaim)" +
                        " values (1,@IpAddress,@PolicyNumber,2,@LossDateTime,@Name,@LossType,@PhoneNumber,@EmailAddress,@LossDescription,@InjuriesExist,0) ;",
                        claimData, commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#else
                global::IFM.IFMErrorLogging.LogException(ex,"IFMDATASERVICES -> ClaimsManager.cs -> Function SubmitClaim - Error submitting claim.",claimData);
#endif
            }
            //want to send email even if writing to database was to fail
            return SendNewClaimEmail(claimData);
        }

        private bool SendNewClaimEmail(ClaimReport claimData)
        {
            string emailBody = GenerateEmailFromFNOL_Item(claimData);
            var chc = new CommonHelperClass();
            var ccAddresses = chc.GetApplicationXMLSetting("ClaimsSettings_SubmitClaim_CCAddresses", "ClaimsSettings.xml");
            return General.SendEmail(AppConfig.ClaimSubmittedEmailNotificationEmail, AppConfig.NoReplyEmailAddress, "Claim Reported on Public website", emailBody, ccAddresses, "");
        }

        private string GenerateEmailFromFNOL_Item(ClaimReport claimData)
        {
            StringBuilder sb = new StringBuilder();

            Action<string, string> CreateLabelValueRow = (string title, string val) =>
             {
                 sb.AppendLine($"<tr><td style=\"font-weight:bold;\">{title}</td><td>{val}</td></tr>");
             };

            sb.AppendLine("<table style=\"font-family:Calibri;font-size:10pt;\">");

            sb.AppendLine("<tr>");
            sb.AppendLine("<td colspan=\"2\" style=\"font-weight:bold;font-size:16pt;\">");
            sb.AppendLine("Member Portal Reported Claim");
            sb.AppendLine("</td>");
            sb.AppendLine("</tr>");

            if (claimData != null)
            {
                //CreateLabelValueRow("View Link", $"<a href=""{AppConfig.MobileClaimsIntranetPageUrl}"" > Follow Link To View</a>")

                CreateLabelValueRow("Policy Number", claimData.PolicyNumber.Trim());
                CreateLabelValueRow("Full Name", claimData.Name.Trim());
                CreateLabelValueRow("Phone Number", claimData.PhoneNumber.Trim());
                CreateLabelValueRow("Email", claimData.EmailAddress);

                CreateLabelValueRow("Loss Date", claimData.LossDateTime.ToString());
                CreateLabelValueRow("Do injuries exist or was there any medical treatment?", claimData.InjuriesExist.ToString());
                CreateLabelValueRow("Source System", "memberPortal");
                CreateLabelValueRow("Ip Address", claimData.IpAddress);

                CreateLabelValueRow("Form Submitted", DateTime.Now.ToString());

                sb.AppendLine("<table style=\"font-family:Calibri;font-size:10pt;text-align: center\">");
                sb.AppendLine("<tr style=\"font-weight:bold;font-size:16pt;\">");
                sb.AppendLine("<td>");
                sb.AppendLine("Claim Description");
                sb.AppendLine("</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr>");
                sb.AppendLine("<td>");
                sb.AppendLine(claimData.LossDescription);
                sb.AppendLine("</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");

            return sb.ToString();
        }

        public List<ClaimInformation> LoadClaims(string policyNUmber)
        {
            IEnumerable<VeryBasicClaimInformation> pendingClaims = null;
            var t1 = Task.Factory.StartNew(()=>{
                pendingClaims = GetPendingClaims(policyNUmber);
            });

            List<ClaimInformation> claimList = new List<ClaimInformation>();
            if (DiamondLogin.OMPLogin())
            {
                using (var DS = Insuresoft.DiamondServices.ClaimsService.LoadClaimsListForPolicyNumber())
                {
                    DS.RequestData.PolicyNumber = policyNUmber;
                    DS.RequestData.LoadClaimsFromRewrittenPolicy = true; // set to true 5/1/2018 Matt A
                    var claimsList = DS.Invoke()?.DiamondResponse?.ResponseData?.ClaimsList;
                    if (claimsList != null)
                    {
                        foreach (var c in claimsList)
                        {
                            claimList.Add(new ClaimInformation(c));
                            c.Dispose();
                        }
                    }

                    t1.Wait();
                    foreach (var pc in pendingClaims)
                    {
                        //only show pending claims that don't already exist as a Diamond Claim
                        if ((from c in claimList where c.LossdateShort == pc.LossdateShort select c).Any() == false)
                        {
                            claimList.Add(new ClaimInformation(pc));
                        }
                    }
                }
            }
            return claimList;
        }

        private IEnumerable<VeryBasicClaimInformation> GetPendingClaims(string PolicyNumber)
        {
            uint excludeAfterNDays = 7;
            // don't show them at all if it is more than n days old because it should have been replaced by a real claim by then
            try
            {
                using (IDbConnection connection = OpenConnection(AppConfig.Conn))
                {
                    return connection.Query<VeryBasicClaimInformation>("select IsPendingClaim = 1, [ClaimNumber] = 'P_' + LTRIM(STR(id)),LossDateTime as [Lossdate]," +
                "PolicyNumber,[DateTime] as ReportedDate,FullName as [PolicyHoldername] from [tbl_m_claimqueue]" +
                " where LTRIM(RTRIM(PolicyNumber)) = @PolNum and DateTime > DATEADD(day,-" + excludeAfterNDays.ToString() + ",GetDate());", new { PolNum = PolicyNumber.Trim().ToUpper() }, commandType: CommandType.Text);
                }
            }
            catch (Exception ex)
            {
#if !DEBUG
                global::IFM.IFMErrorLogging.LogException(ex,$"IFMDATASERVICES -> ClaimsManager.cs -> Function: GetPendingClaims - Error getting pending claims - PolicyNumber '{PolicyNumber}'");
#else
                Debugger.Break();
#endif
            }
            return new List<VeryBasicClaimInformation>();
        }

    }
}
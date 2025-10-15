using IFM.DataServicesCore.BusinessLogic;
using IFM.PrimitiveExtensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using DCO = Diamond.Common.Objects;
using DCS = Diamond.Common.Services;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class VeryBasicClaimInformation : ModelBase
    {

        public bool IsPendingClaim { get; set; }
        public string PolicyHoldername { get; set; }
        public string PolicyNumber { get; set; }
        public string ClaimNumber { get; set; }
        public string ReportedDate { get; set; }
        public string Lossdate { get; set; }

        public string LossdateShort
        {
            get
            {
                if (Lossdate.IsNullEmptyOrWhitespace() == false && DateTime.TryParse(Lossdate, out DateTime dt))
                {
                    return dt.ToShortDateString();
                }
                return string.Empty;
            }
        }

        public string ReportedDateShort
        {
            get
            {
                if (ReportedDate.IsNullEmptyOrWhitespace() == false && DateTime.TryParse(ReportedDate, out DateTime dt))
                {
                    return dt.ToShortDateString();
                }
                return string.Empty;
            }
        }

        public string Closedate { get; set; }

        public string ClosedateShort
        {
            get
            {
                if (Closedate.IsNullEmptyOrWhitespace() == false && DateTime.TryParse(Closedate, out DateTime dt))
                {
                    return dt.ToShortDateString();
                }
                return string.Empty;
            }
        }

        public string LossStatus { get; set; }

        public VeryBasicClaimInformation()
        {
        }

        public VeryBasicClaimInformation(VeryBasicClaimInformation info)
        {
            this.IsPendingClaim = info.IsPendingClaim;
            this.PolicyHoldername = info.PolicyHoldername;
            this.PolicyNumber = info.PolicyNumber;
            this.ClaimNumber = info.ClaimNumber;
            this.ReportedDate = info.ReportedDate;
            this.Lossdate = info.Lossdate;
            this.Closedate = info.Closedate;
            this.LossStatus = info.LossStatus;
        }
    }

    [System.Serializable]
    public class ClaimInformation : VeryBasicClaimInformation
    {
        public int PolicyId { get; set; }
        public int PolicyImageNum { get; set; }
        public int ClaimControlId { get; set; }
        public string ClaimStatus { get; set; }
        public string ClaimRep { get; set; }
        public string ClaimRep_Phone { get; set; }
        public string ClaimRep_Email { get; set; }
        public string ClaimLossType { get; set; }
        public string ClaimCloseReason { get; set; }

        public List<ClaimActivityItem> ClaimActivityItems { get; set; }

        public ClaimInformation() { }

        internal ClaimInformation(DCO.Claims.List.ClaimListData claimData)
        {
            this.PolicyNumber = claimData.PolicyNumber.ToUpper().Trim();
            this.IsPendingClaim = false;
            this.PolicyHoldername = "";
            // TODO Find PolicyHolderName
            this.PolicyNumber = claimData.PolicyNumber;
            this.ClaimNumber = claimData.ClaimNumber;
            this.ReportedDate = claimData.ReportedDate.ToString();
            this.Lossdate = claimData.LossDate.ToString();
            this.Closedate = "";
            // TODO Find CloseDate
            this.PolicyId = claimData.PolicyId;
            this.PolicyImageNum = claimData.PolicyImageNum;
            this.ClaimControlId = claimData.ClaimControlId;
            this.ClaimStatus = ClosedateShort.IsNullEmptyOrWhitespace() ? "Open" : "Closed";
            // claimData.StatusDescription
            this.ClaimRep = claimData.InsideAdjuster;
            this.ClaimRep_Email = "";
            // TODO Find Rep EmailAddress
            this.ClaimRep_Phone = claimData.InsideAdjusterPhoneNum;
            this.ClaimLossType = claimData.LossType;

            if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {
                using (var DS = Insuresoft.DiamondServices.ClaimsService.LoadActivity())
                {
                    DS.RequestData.ClaimControlId = claimData.ClaimControlId;
                    var ClaimActivity = DS.Invoke()?.DiamondResponse?.ResponseData?.DataItems;
                    if (ClaimActivity != null && ClaimActivity.Any())
                    {
                        this.ClaimActivityItems = new List<ClaimActivityItem>();
                        foreach (var i in ClaimActivity)
                        {
                            this.ClaimActivityItems.Add(new ClaimActivityItem(i));
                        }
                    }
                }
            }
            SupplementData();
        }

        private void SupplementData()
        {
            //string sql = "SELECT ClaimControl.claimcontrol_id, ClaimControl.claim_number, ClaimControl.reported_date, ClaimControlStatus.dscr AS claimcontrolstatus, ClaimControl.loss_date, coalesce(claimcontrol.claimclosereason_id,0), ISNULL(ClaimControlName.display_name, '') AS claimcontrolrep ,coalesce(P.phone_num,'800-666-5776') as [claimcontrolrep_phone] ,coalesce(ClaimControlUsers.user_emailaddr,'') as [claimcontrolrep_email], CCR.dscr as [closedescription] FROM [Diamond].[dbo].ClaimControl with (nolock) INNER JOIN [Diamond].[dbo].ClaimControlStatus AS ClaimControlStatus with (nolock) ON ClaimControl.claimcontrolstatus_id = ClaimControlStatus.claimcontrolstatus_id LEFT OUTER JOIN [Diamond].[dbo].ClaimControlPersonnel with (nolock) ON ClaimControlPersonnel.claimcontrol_id = ClaimControl.claimcontrol_id AND ClaimControlPersonnel.claimpersonneltype_id = 3 AND ClaimControlPersonnel.claimadjustertype_id = 1 LEFT OUTER JOIN [Diamond].[dbo].ClaimPersonnel AS ClaimControlClaimPersonnel with (nolock) ON ClaimControlClaimPersonnel.claimpersonnel_id = ClaimControlPersonnel.claimpersonnel_id LEFT OUTER JOIN [Diamond].[dbo].Users AS ClaimControlUsers with (nolock) ON ClaimControlUsers.users_id = ClaimControlClaimPersonnel.users_id LEFT OUTER JOIN [Diamond].[dbo].UserEmployeeLink AS ClaimControlUserEmployeeLink with (nolock) ON ClaimControlUserEmployeeLink.users_id = ClaimControlUsers.users_id LEFT OUTER JOIN [Diamond].[dbo].Employee AS ClaimControlEmployee ON ClaimControlEmployee.employee_id = ClaimControlUserEmployeeLink.employee_id LEFT OUTER JOIN [Diamond].[dbo].Name AS ClaimControlName with (nolock) ON ClaimControlEmployee.name_id = ClaimControlName.name_id Left join Diamond.dbo.EmployeePhoneLink EPL with (nolock) on EPL.employee_id = ClaimControlEmployee.employee_id Left Join Diamond.dbo.Phone P with (nolock) on P.phone_id = EPL.phone_id Left join [Diamond].[dbo].[ClaimCloseReason] CCR with (nolock) on CCR.[claimclosereason_id] = claimcontrol.claimclosereason_id where claim_number = '{Me.ClaimNumber}'";
            //updated 3/20/2018
            //string sql = "SELECT ClaimControl.claimcontrol_id, ClaimControl.claim_number, ClaimControl.reported_date, ClaimControlStatus.dscr AS claimcontrolstatus, ClaimControl.loss_date, coalesce(claimcontrol.claimclosereason_id,0), ISNULL(ClaimControlName.display_name, '') AS claimcontrolrep ,coalesce(P.phone_num,'800-666-5776') as [claimcontrolrep_phone] ,coalesce(ClaimControlUsers.user_emailaddr,'') as [claimcontrolrep_email], CCR.dscr as [closedescription] FROM [Diamond].[dbo].ClaimControl with (nolock) INNER JOIN [Diamond].[dbo].ClaimControlStatus AS ClaimControlStatus with (nolock) ON ClaimControl.claimcontrolstatus_id = ClaimControlStatus.claimcontrolstatus_id LEFT OUTER JOIN [Diamond].[dbo].ClaimControlPersonnel with (nolock) ON ClaimControlPersonnel.claimcontrol_id = ClaimControl.claimcontrol_id AND ClaimControlPersonnel.claimpersonneltype_id = 3 AND ClaimControlPersonnel.claimadjustertype_id = 1 LEFT OUTER JOIN [Diamond].[dbo].ClaimPersonnel AS ClaimControlClaimPersonnel with (nolock) ON ClaimControlClaimPersonnel.claimpersonnel_id = ClaimControlPersonnel.claimpersonnel_id LEFT OUTER JOIN [Diamond].[dbo].Users AS ClaimControlUsers with (nolock) ON ClaimControlUsers.users_id = ClaimControlClaimPersonnel.users_id LEFT OUTER JOIN [Diamond].[dbo].UserEmployeeLink AS ClaimControlUserEmployeeLink with (nolock) ON ClaimControlUserEmployeeLink.users_id = ClaimControlUsers.users_id LEFT OUTER JOIN [Diamond].[dbo].Employee AS ClaimControlEmployee ON ClaimControlEmployee.employee_id = ClaimControlUserEmployeeLink.employee_id LEFT OUTER JOIN [Diamond].[dbo].Name AS ClaimControlName with (nolock) ON ClaimControlEmployee.name_id = ClaimControlName.name_id Left join Diamond.dbo.EmployeePhoneLink EPL with (nolock) on EPL.employee_id = ClaimControlEmployee.employee_id Left Join Diamond.dbo.Phone P with (nolock) on P.phone_id = EPL.phone_id Left join [Diamond].[dbo].[ClaimCloseReason] CCR with (nolock) on CCR.[claimclosereason_id] = claimcontrol.claimclosereason_id where claim_number = '" + this.ClaimNumber + "'";

            using (SqlConnection conn = new SqlConnection(AppConfig.ConnDiamondReports))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("dbo.usp_GetSupplementalClaimInformation", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    //cmd.Connection = conn;
                    //cmd.CommandText = sql; usp_GetSupplementalClaimInformation
                    cmd.Parameters.AddWithValue("@claimNumber", this.ClaimNumber);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();

                            this.ClaimStatus = reader["claimcontrolstatus"].ToString();
                            this.ClaimRep = reader["claimcontrolrep"].ToString();
                            this.ClaimRep_Phone = reader["claimcontrolrep_phone"].ToString();
                            this.ClaimRep_Email = reader["claimcontrolrep_email"].ToString();
                            this.ClaimCloseReason = reader["closedescription"].ToString();
                        }
                    }
                }
            }
        }

        internal ClaimInformation(VeryBasicClaimInformation claimData) : base(claimData)
        {
            this.IsPendingClaim = true;
        }
    }

    [System.Serializable]
    public class ClaimActivityItem : ModelBase
    {
        public DateTime AddedDate { get; set; }
        public string Description { get; set; }
        public string ItemDescription { get; set; }

        public string UserName { get; set; }
        public Int32 ItemNumber { get; set; }

        public ClaimActivityItem() {
            // for serialization
        }
        public ClaimActivityItem(DCS.Messages.ClaimsService.LoadActivity.DataItem item)
        {
            if (item != null)
            {
                this.AddedDate = item.Added;
                this.Description = item.CodeDscr;
                this.ItemDescription = item.ItemDscr;
                this.UserName = item.Username;
                this.ItemNumber = item.ItemNumber;
            }
            else
            {
#if !DEBUG
                IFMErrorLogging.LogIssue("DataItem was null.");
#else
                Debugger.Break();
#endif
            }
        }
    }
}
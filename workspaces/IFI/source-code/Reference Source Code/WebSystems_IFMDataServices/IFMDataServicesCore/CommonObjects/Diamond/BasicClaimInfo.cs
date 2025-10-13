using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFM.DataServicesCore.CommonObjects.Diamond
{
    public class BasicClaimInfo
    {
        public string PolicyNumber { get; set; }
        public int PolicyId { get; set; }
        public int PolicyImageNum { get; set; }
        public string ClaimNumber { get; set; }
        public int ClaimControlId { get; set; }
        public DateTime LossDate { get; set; }
        public DateTime ReportedDate { get; set; }
        public double RequestedAmount { get; set; }
        public int ClaimTypeId { get; set; }
        public string ClaimType { get; set; }
        public int ClaimSeverityId { get; set; }
        public string ClaimSeverity { get; set; }
        public int ClaimCloseReasonId { get; set; }
        public string ClaimCloseReason { get; set; }
        public int ClaimCloseIssueTypeId { get; set; }
        public string ClaimCloseIssueType { get; set; }
        public int ClaimControlStatusId { get; set; }
        public string ClaimControlStatus { get; set; }
        public string Description { get; set; }
    }
}

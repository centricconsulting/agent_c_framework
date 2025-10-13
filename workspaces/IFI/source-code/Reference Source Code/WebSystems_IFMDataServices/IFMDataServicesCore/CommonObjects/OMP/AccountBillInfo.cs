using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    public class AccountBillInfo
    {
        public int BillingAccountId { get; set; }
        public string AccountNumber { get; set; }
        public decimal AccountBalance { get; set; }
        public decimal AccountPayInFull { get; set; }
        public decimal PreviousAccountBalance { get; set; }
        public int AccountStatusCode { get; set; }
        public int ClientId { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime PriorOutstandingLastUpdated { get; set; }
        public List<AccountBillPoliciesInAccount> PoliciesInAccount { get; set; }
    }
}

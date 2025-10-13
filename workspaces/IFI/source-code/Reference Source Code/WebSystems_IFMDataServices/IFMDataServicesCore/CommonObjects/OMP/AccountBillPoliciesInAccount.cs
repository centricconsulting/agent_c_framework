namespace IFM.DataServicesCore.CommonObjects.OMP
{
    public class AccountBillPoliciesInAccount
    {
        public string PolicyNumber { get; set; }
        public int PolicyId { get; set; }
        public int PolicyImageNum { get; set; }
        public decimal PolicyBalance { get; set; }
        public decimal PayInFull { get; set; }
        public int PreferredAccountBillPolicyOrder { get; set; }
        public int PolicyStatusCodeId { get; set; }
        public string PolicyStatusCode { get; set; }
    }
}

namespace IFM.DataServicesCore.CommonObjects.Payments
{
    public class GetCurrentPayPlanOptions
    {
        public int PayPlanTypeId { get; set; }
        public int PolicyTermId { get; set; }
        public int BillMethodId { get; set; }
        public int CompanyID { get; set; }
        public int LobId { get; set; }
        public int StateId { get; set; }
        public int PayPlanId { get; set; }
        public string PayPlanName { get; set; }
        public bool GetExpiredPayPlans { get; set; }
        public System.DateTime EffectiveDate { get; set; }
        public bool IsRenewal { get; set; }
    }
}


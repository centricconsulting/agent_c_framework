namespace IFM.DataServicesCore.CommonObjects.Payments
{
    public class GetCurrentPayPlanObject
    {
        public int BillingPayPlanId { get; set; }
        public string BillingPayPlan { get; set; }
        public int BillingPayPlanTypeId { get; set; }
        public string BillingPayPlanType { get; set; }
        public int BillMethodId { get; set; }
        public string BillMethod { get; set; }
    }
}

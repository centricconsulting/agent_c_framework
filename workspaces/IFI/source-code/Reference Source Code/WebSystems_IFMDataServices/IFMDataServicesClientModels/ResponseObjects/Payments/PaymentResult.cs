namespace IFM.DataServices.API.ResponseObjects.Payments
{
    [System.Serializable]
    public class PaymentResult
    {
        public bool PaymentCompleted { get; set; }
        public string PaymentConfirmationNumber { get; set; }
    }
}
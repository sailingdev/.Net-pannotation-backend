namespace Pannotation.Models.InternalModels
{
    public class PaymentData
    {
        public string Number { get; set; }

        public string CVV { get; set; }

        public string ExpiryDate { get; set; }

        public decimal Amount { get; set; }

        public string OrderId { get; set; }
    }
}

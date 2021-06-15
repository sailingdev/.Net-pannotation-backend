namespace Pannotation.Models.RequestModels.Payment
{
    public class Returned3DS
    {
        public string MerId { get; set; }
        public string AcqID { get; set; }
        public string OrderID { get; set; }
        public string ResponseCode { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonCodeDesc { get; set; }
        public string ReferenceNo { get; set; } = null;
        public string PaddedCardNo { get; set; } = null;
        public string AuthCode { get; set; } = null;
        public string CVV2Result { get; set; } = null;
        public string AuthenticationResult { get; set; } = null;
        public string CAVVValue { get; set; } = null;
        public string ECIIndicator { get; set; } = null;
        public string TransactionStain { get; set; } = null;
        public string OriginalResponseCode { get; set; } = null;
        public string Signature { get; set; } = null;
        public string SignatureMethod { get; set; } = null;

        // Test variable
        public bool Test { get; set; } = false;
    }
}
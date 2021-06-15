using Newtonsoft.Json;
using Pannotation.Models.ResponseModels.Songsheet;
using System.Collections.Generic;

namespace Pannotation.Models.ResponseModels.Payment
{
    public class SuccessfulPaymentResponseModel
    {
        [JsonProperty("orderId")]
        public int OrderId { get; set; }

        [JsonProperty("orderAmount")]
        public decimal OrderAmount { get; set; }

        [JsonProperty("successMessage")]
        public string SuccessMessage { get; set; }

        [JsonProperty("billingAddress")]
        public AddressResponseModel BillingAddress { get; set; }

        [JsonProperty("paymentMethod")]
        public PaymentMethodResponseModel PaymentMethod { get; set; }

        [JsonProperty("songsheets")]
        public List<CartSongsheetResponseModel> Songsheets { get; set; }
    }
}

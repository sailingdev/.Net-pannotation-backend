using Newtonsoft.Json;
using Pannotation.Models.Enums;
using Pannotation.Models.ResponseModels.Payment;
using Pannotation.Models.ResponseModels.Songsheet;
using System.Collections.Generic;

namespace Pannotation.Models.ResponseModels.Order
{
    public class OrderDetailsResponseModel
    {
        [JsonProperty("orderId")]
        public int OrderId { get; set; }

        [JsonProperty("orderAmount")]
        public decimal OrderAmount { get; set; }

        [JsonProperty("paymentMethod")]
        public PaymentMethodResponseModel PaymentMethod { get; set; }

        [JsonProperty("songsheets")]
        public List<CartSongsheetResponseModel> Songsheets { get; set; }

        [JsonProperty("status")]
        public TransactionStatus? Status { get; set; }
    }
}

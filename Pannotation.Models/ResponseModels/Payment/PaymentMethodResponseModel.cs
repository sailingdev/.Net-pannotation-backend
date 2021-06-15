using Newtonsoft.Json;

namespace Pannotation.Models.ResponseModels.Payment
{
    public class PaymentMethodResponseModel
    {
        [JsonProperty("cardMask")]
        public string CardMask { get; set; }

        [JsonProperty("cardType")]
        public string CardType { get; set; }
    }
}

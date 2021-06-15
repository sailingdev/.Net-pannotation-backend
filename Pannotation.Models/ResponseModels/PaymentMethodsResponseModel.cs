using Newtonsoft.Json;
using System.Collections.Generic;

namespace Pannotation.Models.ResponseModels
{
    public class PaymentMethodsResponseModel
    {
        [JsonProperty("paymentMethods")]
        public List<string> PaymentMethods { get; set; }
    }
}

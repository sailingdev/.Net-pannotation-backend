using Newtonsoft.Json;
using Pannotation.Models.ResponseModels.Songsheet;
using System.Collections.Generic;

namespace Pannotation.Models.ResponseModels
{
    public class CartResponseModel
    {
        [JsonProperty("totalAmount")]
        public decimal TotalAmount { get; set; }

        [JsonProperty("items", NullValueHandling = NullValueHandling.Ignore)]
        public List<CartSongsheetResponseModel> Items { get; set; }
    }
}

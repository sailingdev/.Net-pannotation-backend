using Newtonsoft.Json;
using Pannotation.Models.ResponseModels.Payment;
using Pannotation.Models.ResponseModels.Songsheet;
using System.Collections.Generic;

namespace Pannotation.Models.ResponseModels.Order
{
    public class AdminOrderDetailsResponseModel : OrderItemListResponseModel
    {
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; }

        [JsonProperty("songsheets")]
        public new List<SongsheetShortResponseModel> Songsheets { get; set; }

        [JsonProperty("transactionInfo")]
        public TransactionInfoResponseModel TransactionInfo { get; set; }
    }
}

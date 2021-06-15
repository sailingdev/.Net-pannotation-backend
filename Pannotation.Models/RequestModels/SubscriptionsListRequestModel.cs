using Newtonsoft.Json;
using Pannotation.Models.Enums;
using System;

namespace Pannotation.Models.RequestModels
{
    public class SubscriptionsListRequestModel : PaginationRequestModel<SubscriptionsSortingKey>
    {
        [JsonProperty("dateFrom")]
        public DateTime? DateFrom { get; set; }

        [JsonProperty("dateTo")]
        public DateTime? DateTo { get; set; }
    }
}

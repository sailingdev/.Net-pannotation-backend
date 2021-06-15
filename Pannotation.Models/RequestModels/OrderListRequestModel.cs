using Newtonsoft.Json;
using Pannotation.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pannotation.Models.RequestModels
{
    public class OrderListRequestModel : PaginationRequestModel<OrderListKey>
    {
        [JsonProperty("dateFrom")]
        public DateTime? DateFrom { get; set; }

        [JsonProperty("dateTo")]
        public DateTime? DateTo { get; set; }
    }
}

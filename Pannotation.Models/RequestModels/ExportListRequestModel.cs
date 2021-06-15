using Newtonsoft.Json;
using Pannotation.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels
{
    public class ExportListRequestModel<T>
    {
        [JsonProperty("order")]
        public OrderingRequestModel<T, SortingDirection> Order { get; set; }

        [JsonProperty("search")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Length of search query must be from 3 to 50 characters")]
        public string Search { get; set; }

        [JsonProperty("dateFrom")]
        public DateTime? DateFrom { get; set; }

        [JsonProperty("dateTo")]
        public DateTime? DateTo { get; set; }
    }
}

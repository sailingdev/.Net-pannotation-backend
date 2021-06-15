using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels
{
    public class PaginationBaseRequestModel
    {
        [JsonProperty("limit")]
        [Range(1, 50, ErrorMessage = "Limit is invalid")]
        public int Limit { get; set; } = 10;

        [JsonProperty("offset")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Offset is invalid")]
        public int Offset { get; set; } = 0;
    }
}

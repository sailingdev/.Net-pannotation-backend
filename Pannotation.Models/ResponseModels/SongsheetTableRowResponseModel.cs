using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pannotation.Models.ResponseModels
{
    public class SongsheetTableRowResponseModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("isTop")]
        public bool IsTop { get; set; }

        [JsonProperty("image")]
        public ImageResponseModel Image { get; set; }
    }
}

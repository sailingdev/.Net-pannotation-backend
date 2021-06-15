using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pannotation.Models.ResponseModels.Order
{
    public class OrderSongsheetResponseModel : IdResponseModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("artistName")]
        public string ArtistName { get; set; }

        [JsonProperty("arranger")]
        public string Arranger { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("file")]
        public FileResponseModel File { get; set; }
    }
}

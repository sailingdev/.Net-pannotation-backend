using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pannotation.Models.ResponseModels.Songsheet
{
    public class SongsheetListItemResponseModel : IdResponseModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("artistName")]
        public string ArtistName { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }

        [JsonProperty("image")]
        public ImageResponseModel Image { get; set; }

        [JsonProperty("instruments")]
        public List<string> Instruments { get; set; }

        [JsonProperty("genres")]
        public List<string> Genres { get; set; }
    }
}

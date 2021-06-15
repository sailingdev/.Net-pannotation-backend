using Newtonsoft.Json;

namespace Pannotation.Models.ResponseModels.Songsheet
{
    public class SongsheetShortResponseModel : IdResponseModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}
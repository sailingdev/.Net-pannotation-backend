using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels
{
    public class SongsheetIdRequestModel
    {
        [JsonProperty("songsheetId")]
        [Required(ErrorMessage = "Songsheet id is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Songsheet id is invalid")]
        public int SongsheetId { get; set; }
    }
}

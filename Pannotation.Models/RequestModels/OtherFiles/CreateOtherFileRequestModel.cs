using Newtonsoft.Json;
using Pannotation.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels.OtherFiles
{
    public class CreateOtherFileRequestModel : EditOtherFileRequestModel
    {
        [JsonProperty("fileType")]
        [Required(ErrorMessage = "File type is required")]
        public OtherFileType? FileType { get; set; }
    }
}

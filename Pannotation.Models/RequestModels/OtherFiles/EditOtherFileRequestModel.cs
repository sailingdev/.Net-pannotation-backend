using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels.OtherFiles
{
    public class EditOtherFileRequestModel
    {
        [JsonProperty("name")]
        [Required(ErrorMessage = "Name is required")]
        [StringLength(250, ErrorMessage = "Name must be from 3 to 250 symbols", MinimumLength = 3)]
        [RegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "Name cannot contain spaces only")]
        public string Name { get; set; }

        [JsonProperty("description")]
        [StringLength(50000, ErrorMessage = "Description must be from 1 to 50000 symbols", MinimumLength = 0)]
        [RegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES_WITH_LINE_BREAKS, ErrorMessage = "Description cannot contain spaces only")]
        public string Description { get; set; }

        [JsonProperty("previewId")]
        [Required(ErrorMessage = "File preview is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Preview id is invalid")]
        public int PreviewId { get; set; }

        [JsonProperty("fileId")]
        [Required(ErrorMessage = "File is required")]
        [Range(1, int.MaxValue, ErrorMessage = "File id is invalid")]
        public int FileId { get; set; }
    }
}

using Newtonsoft.Json;
using Pannotation.Models.Enums;
using System.Collections.Generic;

namespace Pannotation.Models.RequestModels.OtherFiles
{
    public class OtherFilesRequestModel : PaginationRequestModel<OtherFilesOrderKey>
    {
        [JsonProperty("fileTypes")]
        public List<OtherFileType> FileTypes { get; set; } = null;
    }
}

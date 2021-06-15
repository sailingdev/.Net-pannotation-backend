using Newtonsoft.Json;
using Pannotation.Models.Enums;
using System.Collections.Generic;

namespace Pannotation.Models.RequestModels
{
    public class FilesListRequestModel : PaginationRequestModel<FilesListOrderKey>
    {
        [JsonProperty("fileTypes")]
        public List<SearchFileType> FileTypes { get; set; } = null;

        [JsonProperty("genres")]
        public List<int> Genres { get; set; }
    }
}

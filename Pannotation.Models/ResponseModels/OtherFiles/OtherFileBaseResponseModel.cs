using Newtonsoft.Json;
using Pannotation.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pannotation.Models.ResponseModels.OtherFiles
{
    public class OtherFileBaseResponseModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fileType")]
        public OtherFileType FileType { get; set; }

        [JsonProperty("preview")]
        public ImageResponseModel Preview { get; set; }
    }
}

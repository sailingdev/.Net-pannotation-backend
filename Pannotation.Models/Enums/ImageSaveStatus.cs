using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pannotation.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ImageSaveStatus
    {
        InvalidLength,
        InvalidFormat,
        InvalidSize,
        InvalidDimension,
        Saved
    }
}

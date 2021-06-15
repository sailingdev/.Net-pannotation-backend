using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Pannotation.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ImageType
    {
        Avatar,
        Image
    }
}

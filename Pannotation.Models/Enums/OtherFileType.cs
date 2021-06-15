using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Pannotation.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OtherFileType
    {
        AcademicPublication,
        NonAcademicArticle,
        AudioPublication,
        Steelband,
        Other,
        LeadSheet
    }
}

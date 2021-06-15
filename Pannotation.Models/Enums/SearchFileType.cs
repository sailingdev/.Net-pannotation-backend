using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Pannotation.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SearchFileType
    {
        Songsheet,
        AcademicPublication,
        NonAcademicArticle,
        AudioPublication,
        Steelband,
        Other,
        LeadSheet
    }
}

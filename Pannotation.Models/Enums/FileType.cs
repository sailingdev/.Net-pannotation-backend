using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Pannotation.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FileType
    {
        SongsheetPreview,
        Songsheet,
        AudioTrack,         // for songsheet
        OtherFile,
        OtherFileAudio      // for other file
    }
}
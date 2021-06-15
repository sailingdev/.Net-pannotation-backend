using Pannotation.Models.Enums;

namespace Pannotation.Models.InternalModels
{
    public class SongsheetsFiles
    {
        public ContentType ContentType { get; set; }

        public byte[] Content { get; set; }

        public string SingleSongsheetName { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Pannotation.Models.Export
{
    public class SongsheetExportModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public string ArtistName { get; set; }
        
        public string Producer { get; set; }
        
        public string Arranger { get; set; }
        
        public string YouTubeLink { get; set; }
        
        public string Description { get; set; }
        
        public decimal Price { get; set; }

        public List<string> Instruments { get; set; }

        public List<string> Genres { get; set; }
    }
}

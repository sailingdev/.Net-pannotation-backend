using System.ComponentModel.DataAnnotations.Schema;

namespace Pannotation.Domain.Entities
{
    public class SongsheetGenre
    {
        #region Properties

        public int SongsheetId { get; set; }
        
        public int GenreId { get; set; }

        #endregion

        #region Navigation Properties

        [ForeignKey("SongsheetId")]
        [InverseProperty("Genres")]
        public virtual Songsheet Songsheet { get; set; }

        [ForeignKey("GenreId")]
        [InverseProperty("Songsheets")]
        public virtual Genre Genre { get; set; }

        #endregion
    }
}

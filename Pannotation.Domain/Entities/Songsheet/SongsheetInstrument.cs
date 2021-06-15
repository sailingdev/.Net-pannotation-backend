using System.ComponentModel.DataAnnotations.Schema;

namespace Pannotation.Domain.Entities
{
    public class SongsheetInstrument
    {
        #region Properties

        public int SongsheetId { get; set; }
        
        public int InstrumentId { get; set; }

        #endregion

        #region Navigation Properties

        [ForeignKey("SongsheetId")]
        [InverseProperty("Instruments")]
        public virtual Songsheet Songsheet { get; set; }

        [ForeignKey("InstrumentId")]
        [InverseProperty("Songsheets")]
        public virtual Instrument Instrument { get; set; }

        #endregion
    }
}

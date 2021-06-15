using Pannotation.Common.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pannotation.Domain.Entities
{
    public class Instrument : IEntity
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        #endregion

        #region Navigation Properties

        [InverseProperty("Instrument")]
        public virtual ICollection<SongsheetInstrument> Songsheets { get; set; }

        #endregion

        #region Ctors

        public Instrument()
        {
            Songsheets = Songsheets.Empty();
        }

        #endregion
    }
}

using Pannotation.Common.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pannotation.Domain.Entities
{
    public class Genre : IEntity
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        #endregion

        #region Navigation Properties

        [InverseProperty("Genre")]
        public virtual ICollection<SongsheetGenre> Songsheets { get; set; }

        #endregion

        #region Ctors

        public Genre()
        {
            Songsheets = Songsheets.Empty();
        }

        #endregion
    }
}

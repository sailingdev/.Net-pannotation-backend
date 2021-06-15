using Pannotation.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pannotation.Domain.Entities.Order
{
    public class CartItem
    {
        #region Properties

        public int UserId { get; set; }

        public int SongsheetId { get; set; }

        #endregion

        #region Navigation Properties

        [InverseProperty("CartItems")]
        public virtual ApplicationUser User { get; set; }

        [InverseProperty("CartItems")]
        public virtual Songsheet Songsheet { get; set; }

        #endregion
    }
}

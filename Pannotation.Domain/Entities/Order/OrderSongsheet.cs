using System.ComponentModel.DataAnnotations.Schema;

namespace Pannotation.Domain.Entities.Order
{
    public class OrderSongsheet
    {
        #region Properties

        public int OrderId { get; set; }

        public int SongsheetId { get; set; }

        [Column(TypeName = "decimal(8, 2)")]
        public decimal PriceAtPurchaseMoment { get; set; }

        #endregion

        #region Navigation Properties

        [ForeignKey("OrderId")]
        [InverseProperty("Songsheets")]
        public virtual Order Order { get; set; }

        [ForeignKey("SongsheetId")]
        [InverseProperty("Orders")]
        public virtual Songsheet Songsheet { get; set; }

        #endregion
    }
}

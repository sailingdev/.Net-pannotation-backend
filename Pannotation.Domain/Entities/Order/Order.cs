using Pannotation.Common.Extensions;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Domain.Entities.Payment;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pannotation.Domain.Entities.Order
{
    public class Order : IEntity
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "decimal(8, 2)")]
        public decimal Amount { get; set; }

        public int UserId { get; set; }

        #endregion

        #region Navigation Properties

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [InverseProperty("Order")]
        public virtual ICollection<OrderSongsheet> Songsheets { get; set; }

        [InverseProperty("Order")]
        public virtual ICollection<Transaction> Transactions { get; set; }

        #endregion

        #region Ctors

        public Order()
        {
            Songsheets = Songsheets.Empty();
            Transactions = Transactions.Empty();
        }

        #endregion
    }
}

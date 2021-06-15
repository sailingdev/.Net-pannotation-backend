using Pannotation.Domain.Entities.Identity;
using Pannotation.Domain.Entities.Payment;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pannotation.Common.Extensions;
using System;

namespace Pannotation.Domain.Entities.Subscription
{
    public class Subscription : IEntity
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime? PurchasedAt { get; set; }

        public DateTime? NextPaymentDate { get; set; }

        #endregion

        #region Navigation Properties

        [ForeignKey("UserId")]
        [InverseProperty("Subscriptions")]
        public virtual ApplicationUser User { get; set; }

        [InverseProperty("Subscription")]
        public virtual ICollection<Transaction> Transactions { get; set; }

        #endregion

        #region Ctors

        public Subscription()
        {
            Transactions = Transactions.Empty();
        }

        #endregion
    }
}

using Pannotation.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pannotation.Domain.Entities.Payment
{
    public class Transaction : IEntity
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        public int? OrderId { get; set; }

        public int? SubscriptionId { get; set; }

        public TransactionStatus? TransactionStatus { get; set; }

        public string PaymentOrderId { get; set; }

        [Column(TypeName = "decimal(8, 2)")]
        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CountryName { get; set; }

        public string CardMask { get; set; }

        public string ExpirationDate { get; set; }

        public string CardType { get; set; }

        public string CardholderName { get; set; }

        #endregion

        #region Navigation Properties

        [ForeignKey("OrderId")]
        [InverseProperty("Transactions")]
        public virtual Order.Order Order { get; set; }

        [ForeignKey("SubscriptionId")]
        [InverseProperty("Transactions")]
        public virtual Subscription.Subscription Subscription { get; set; }

        #endregion
    }
}

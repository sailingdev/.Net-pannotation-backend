using Microsoft.AspNetCore.Identity;
using Pannotation.Common.Extensions;
using Pannotation.Domain.Entities.Order;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pannotation.Domain.Entities.Identity
{
    public partial class ApplicationUser : IdentityUser<int>, IEntity
    {
        #region Properties

        [Key]
        public override int Id { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        [DataType("DateTime")]
        public DateTime RegistratedAt { get; set; }

        [DataType("DateTime")]
        public DateTime? DeletedAt { get; set; }

        [DataType("DateTime")]
        public DateTime? LastVisitAt { get; set; }

        public bool IsSubscribed { get; set; }

        public bool ShouldCancelSubscription { get; set; }

        /// <summary>
        /// Difference between server and user timezones in hours 
        /// </summary>
        public double TimeZoneOffset { get; set; }

        #endregion

        #region Navigation Properties

        [InverseProperty("User")]
        public virtual Profile Profile { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<CartItem> CartItems { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserToken> Tokens { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Subscription.Subscription> Subscriptions { get; set; }

        #endregion

        #region Additional Properties

        [NotMapped]
        public DateTime ClientTime
        {
            get
            {
                return DateTime.UtcNow.AddHours(TimeZoneOffset);
            }
        }

        #endregion

        #region Ctors

        public ApplicationUser()
        {
            Tokens = Tokens.Empty();
            UserRoles = UserRoles.Empty();
            CartItems = CartItems.Empty();
            Subscriptions = Subscriptions.Empty();
        }

        #endregion
    }
}
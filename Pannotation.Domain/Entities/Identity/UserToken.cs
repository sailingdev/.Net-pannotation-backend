using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pannotation.Models.Enums;

namespace Pannotation.Domain.Entities.Identity
{
    public class UserToken: IEntity
    {
        #region Properties

        public int Id { get; set; }

        public int UserId { get; set; }

        [MaxLength(200)]
        public string TokenHash { get; set; }

        public DateTime ExpiresDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? DisposedAt { get; set; }

        public bool IsActive { get; set; }

        public TokenType Type { get; set; }

        #endregion

        #region Navigation Properties

        [ForeignKey("UserId")]
        [InverseProperty("Tokens")]
        public virtual ApplicationUser User { get; set; }
        
        #endregion
    }
}

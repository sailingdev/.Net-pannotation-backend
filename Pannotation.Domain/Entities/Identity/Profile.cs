using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pannotation.Domain.Entities.Identity
{
    public class Profile : IEntity<int>
    {
        #region Properties

        public int Id { get; set; }

        public int UserId { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string MobileNumber { get; set; }

        public string Country { get; set; }

        [MaxLength(50)]
        public string State { get; set; }

        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(50)]
        public string Address { get; set; }

        [MaxLength(15)]
        public string Zip { get; set; }

        public bool IsComposer { get; set; }

        [MaxLength(50)]
        public string IdCode { get; set; }

        public int? AvatarId { get; set; }

        #endregion

        #region Navigation properties

        [InverseProperty("Profile")]
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("AvatarId")]
        public virtual Image Avatar { get; set; }

        #endregion

        #region Additional Properties

        [NotMapped]
        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
                    return $"{FirstName} {LastName}";
                else
                    return string.Empty;
            }
        }

        #endregion
    }
}

using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels
{
    public class UserProfileRequestModel : AddressRequestModel
    {
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "First Name must be from 1 to 50 symbols", MinimumLength = 1)]
        [RegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "First Name cannot contain spaces only")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Last Name must be from 1 to 50 symbols", MinimumLength = 1)]
        [RegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "Last Name cannot contain spaces only")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone Number field is empty")]
        [RegularExpression(ModelRegularExpression.REG_PHONE, ErrorMessage = "Phone number is not in valid format")]
        [StringLength(16, ErrorMessage = "Phone number is not in valid format", MinimumLength = 6)]
        public string PhoneNumber { get; set; }

        [RegularExpression(ModelRegularExpression.REG_PHONE, ErrorMessage = "Mobile Number is not in valid format")]
        [StringLength(16, ErrorMessage = "Mobile Number is not in valid format", MinimumLength = 6)]
        public string MobileNumber { get; set; }

        public bool IsComposer { get; set; } = false;

        [StringLength(50, ErrorMessage = "Id Code must be from 1 to 50 symbols", MinimumLength = 1)]
        [RegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "Id Code cannot contain spaces only")]
        public string IdCode { get; set; }

        public int? ImageId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels
{
    public class AddressRequestModel
    {
        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }

        [StringLength(50, ErrorMessage = "State must be from 1 to 50 symbols", MinimumLength = 1)]
        [RegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "State cannot contain spaces only")]
        public string State { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(50, ErrorMessage = "City must be from 1 to 50 symbols", MinimumLength = 1)]
        [RegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "City cannot contain spaces only")]
        public string City { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(50, ErrorMessage = "Address must be from 1 to 50 symbols", MinimumLength = 1)]
        [RegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "Address cannot contain spaces only")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Zip is required")]
        [StringLength(15, ErrorMessage = "Zip must be from 1 to 15 symbols", MinimumLength = 1)]
        [RegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "Zip cannot contain spaces only")]
        public string Zip { get; set; }
    }
}

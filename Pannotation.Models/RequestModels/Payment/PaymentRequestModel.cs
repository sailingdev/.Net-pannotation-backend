using Pannotation.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels.Payment
{
    public class PaymentRequestModel : PaymentBaseRequest
    {
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(30, ErrorMessage = "First Name must be from 1 to 30 symbols length", MinimumLength = 1)]
        [CustomRegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "First Name cannot contain spaces only")]
        [CustomRegularExpression(ModelRegularExpression.REG_ALPHANUMERIC_PUNCTUATION, ErrorMessage = "First Name can contain only alphanumeric and punctuation characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(30, ErrorMessage = "Last Name must be from 1 to 30 symbols length", MinimumLength = 1)]
        [CustomRegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "Last Name cannot contain spaces only")]
        [CustomRegularExpression(ModelRegularExpression.REG_ALPHANUMERIC_PUNCTUATION, ErrorMessage = "Last Name can contain only alphanumeric and punctuation characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Country Code is required")]
        [StringLength(3, ErrorMessage = "Country Code must be 3 symbols length", MinimumLength = 3)]
        [CustomRegularExpression(ModelRegularExpression.REG_NUMERIC, ErrorMessage = "Country Code can contain only numeric characters")]
        public string CountryCode { get; set; }

        [Required(ErrorMessage = "Country name is required")]
        [CustomRegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "Country Code can contain only numeric characters")]
        public string CountryName { get; set; }

        //[StringLength(5, ErrorMessage = "State must be from 2 to 5 symbols length", MinimumLength = 2)]
        //[CustomRegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "State cannot contain spaces only")]
        //[CustomRegularExpression(ModelRegularExpression.REG_ALPHANUMERIC, ErrorMessage = "State can contain only alphanumeric characters")]
        [StateValidationAttribute(ErrorMessage ="State is invalid. Should be a valid US state 2 symbols long")]
        public string State { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(30, ErrorMessage = "City must be from 1 to 30 symbols length", MinimumLength = 1)]
        [CustomRegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "City cannot contain spaces only")]
        [CustomRegularExpression(ModelRegularExpression.REG_ALPHANUMERIC, ErrorMessage = "City can contain only alphanumeric characters")]
        public string City { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(50, ErrorMessage = "Address must be from 1 to 50 symbols length", MinimumLength = 1)]
        [CustomRegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "Address cannot contain spaces only")]
        [CustomRegularExpression(ModelRegularExpression.REG_ALPHANUMERIC_PUNCTUATION, ErrorMessage = "Address can contain only alphanumeric and punctuation characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Zip is required")]
        [StringLength(10, ErrorMessage = "Zip must be from 1 to 15 symbols", MinimumLength = 1)]
        [CustomRegularExpression(ModelRegularExpression.REG_ALPHANUMERIC_WITHOUT_SPACES, ErrorMessage = "Zip can contain only numeric characters")]
        public string Zip { get; set; }
    }
}

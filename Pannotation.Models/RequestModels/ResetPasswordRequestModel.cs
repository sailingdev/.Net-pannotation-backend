using Pannotation.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels
{
    public class ResetPasswordRequestModel : EmailRequestModel
    {
        [Required(ErrorMessage = "Token is empty")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, ErrorMessage = "Password must be from 6 to 50 characters", MinimumLength = 6)]
        [CustomRegularExpression(ModelRegularExpression.REG_ONE_LATER_DIGIT_CAPITAL_WITH_SPEC, ErrorMessage = "Password should contain at least one letter, one digit and one capital letter")]
        [CustomRegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "Password cannot contain spaces only")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password field is empty")]
        [Compare("Password", ErrorMessage = "Confirm Password isn’t the same as Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}

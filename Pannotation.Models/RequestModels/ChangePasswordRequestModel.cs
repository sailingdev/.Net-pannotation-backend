using Pannotation.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels
{
    public class ChangePasswordRequestModel
    {
        [Required(ErrorMessage = "Current Password is required")]
        [CustomRegularExpression(ModelRegularExpression.REG_ONE_LATER_DIGIT_CAPITAL_WITH_SPEC, ErrorMessage = "Current Password should contain at least one letter, one digit and one capital letter")]
        [CustomRegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "Current Password cannot contain spaces only")]
        [StringLength(50, ErrorMessage = "Current Password must be from 6 to 50 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [CustomRegularExpression(ModelRegularExpression.REG_ONE_LATER_DIGIT_CAPITAL_WITH_SPEC, ErrorMessage = "Password should contain at least one letter, one digit and one capital letter")]
        [CustomRegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "Password cannot contain spaces only")]
        [StringLength(50, ErrorMessage = "Password should be from 6 to 50 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password field is empty")]
        [Compare("Password", ErrorMessage = "Confirm Password isn’t the same as Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}

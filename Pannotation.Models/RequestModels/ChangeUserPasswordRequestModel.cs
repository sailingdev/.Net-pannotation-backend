using Pannotation.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels
{
    public class ChangeUserPasswordRequestModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Id should be greater than 0")]
        public int Id { get; set; }

        [CustomRegularExpression(ModelRegularExpression.REG_ONE_LATER_DIGIT, ErrorMessage = "Password should contain at least one letter and one digit")]
        [CustomRegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "Password can’t contain spaces only")]
        [Required(ErrorMessage = "Password field is empty")]
        [StringLength(50, ErrorMessage = "Password should be from 6 to 50 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

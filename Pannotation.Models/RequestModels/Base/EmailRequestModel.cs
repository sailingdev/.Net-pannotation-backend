using Pannotation.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels
{
    public class EmailRequestModel
    {
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email is not in valid format")]
        [StringLength(129, ErrorMessage = "Email is not in valid format", MinimumLength = 3)]
        [CustomRegularExpression(ModelRegularExpression.REG_EMAIL, ErrorMessage = "Email is not in valid format")]
        [CustomRegularExpression(ModelRegularExpression.REG_EMAIL_DOMAINS, ErrorMessage = "Email is not in valid format")]
        public string Email { get; set; }
    }
}

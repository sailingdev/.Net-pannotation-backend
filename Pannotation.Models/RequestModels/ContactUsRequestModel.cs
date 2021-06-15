using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels
{
    public class ContactUsRequestModel : EmailRequestModel
    {
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "First Name must be from 1 to 50 symbols", MinimumLength = 1)]
        [RegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "First Name cannot contain spaces only")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Last Name must be from 1 to 50 symbols", MinimumLength = 1)]
        [RegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "Last Name cannot contain spaces only")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        [StringLength(50, ErrorMessage = "Subject must be from 1 to 50 symbols", MinimumLength = 0)]
        [RegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "Subject cannot contain spaces only")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [StringLength(750, ErrorMessage = "Message must be from 1 to 750 symbols", MinimumLength = 0)]
        public string Message { get; set; }
    }
}

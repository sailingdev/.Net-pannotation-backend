using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels
{
    public class ConfirmEmailRequestModel : EmailRequestModel
    {
        [Required(ErrorMessage = "Token field is empty")]
        public string Token { get; set; }
    }

}

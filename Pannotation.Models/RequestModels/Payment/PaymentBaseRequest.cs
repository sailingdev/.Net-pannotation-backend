using Pannotation.Common.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels.Payment
{
    public class PaymentBaseRequest
    {
        [Required(ErrorMessage = "CSC/CVV field is empty")]
        [StringLength(4, ErrorMessage = "CSC/CVV doesn’t meet length validation criteria", MinimumLength = 3)]
        [CustomRegularExpression(ModelRegularExpression.REG_NUMERIC, ErrorMessage = "CSC/CVV can contain only numeric characters")]
        public string CVV { get; set; }

        [Required(ErrorMessage = "Number field is empty")]
        [StringLength(19, ErrorMessage = "Number field doesn’t meet length validation criteria", MinimumLength = 14)]
        [CustomRegularExpression(ModelRegularExpression.REG_NUMERIC, ErrorMessage = "Number field can contain only numeric characters")]
        public string Number { get; set; }

        [Required(ErrorMessage = "Card Type is required")]
        public string CardType { get; set; }

        [Required(ErrorMessage = "Expiration date field is empty")]
        [StringLength(5, ErrorMessage = "Expiration date doesn’t meet length validation criteria", MinimumLength = 5)]
        [CustomRegularExpression(ModelRegularExpression.REG_EXPIRATION_DATE, ErrorMessage = "Expiration date is invalid")]
        public string ExpirationDate { get; set; }
    }
}

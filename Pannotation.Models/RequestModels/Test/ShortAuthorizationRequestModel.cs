using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels.Test
{
    public class ShortAuthorizationRequestModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "{0} is invalid")]
        public int? Id { get; set; }

        public string UserName { get; set; }
    }
}

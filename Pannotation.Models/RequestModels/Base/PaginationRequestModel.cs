using Newtonsoft.Json;
using Pannotation.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Pannotation.Models.RequestModels
{
    public class PaginationRequestModel<T> : PaginationBaseRequestModel where T : struct
    {
        private string _search;

        [JsonProperty("search")]
        [StringLength(50, ErrorMessage = "Length of search query must be from 3 to 50 characters", MinimumLength = 3)]
        public string Search { get => _search; set => _search = HttpUtility.UrlDecode(value); }

        [JsonProperty("order")]
        public OrderingRequestModel<T, SortingDirection> Order { get; set; }
    }
}

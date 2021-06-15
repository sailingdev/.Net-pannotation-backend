namespace Pannotation.Models.ResponseModels
{
    public class SearchPaginationResponseModel
    {
        public int TotalCount { get; set; }

        public SearchResultInfo Data { get; set; }
    }
}

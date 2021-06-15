using Newtonsoft.Json;
using Pannotation.Models.Enums;
using System.Collections.Generic;
using Pannotation.Common.Extensions;

namespace Pannotation.Models.ResponseModels
{
    public class SearchResultInfo
    {
        [JsonProperty("searchString")]
        public string SearchString { get; set; }

        [JsonProperty("items")]
        public List<SearchFileItem> Items { get; set; }

        [JsonProperty("itemsCount")]
        public List<SearchResultCountItem> ItemsCount { get; set; }
    }

    public class SearchFileItem : IdResponseModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("artistName")]
        public string ArtistName { get; set; }

        [JsonProperty("price")]
        public decimal? Price { get; set; }

        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }

        [JsonProperty("image")]
        public ImageResponseModel Image { get; set; }

        [JsonProperty("instruments")]
        public List<string> Instruments { get; set; }

        [JsonProperty("genres")]
        public List<string> Genres { get; set; }

        [JsonProperty("fileType")]
        public SearchFileType FileType { get; set; }
    }

    public class SearchResultCountItem
    {
        [JsonProperty("type")]
        public SearchFileType Type { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        public SearchResultCountItem(SearchFileType type, int count)
        {
            Type = type;
            Count = count;
        }
    }
}

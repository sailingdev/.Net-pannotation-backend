using Pannotation.Common.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels
{
    public class CreateSongsheetRequestModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name must be from 3 to 50 symbols", MinimumLength = 3)]
        [RegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "Name cannot contain spaces only")]
        public string Name { get; set; }

        [StringLength(750, ErrorMessage = "Description must be from 1 to 750 symbols", MinimumLength = 0)]
        [RegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "Description cannot contain spaces only")]
        public string Description { get; set; }

        [StringLength(100, ErrorMessage = "Artist Name must be from 1 to 100 symbols", MinimumLength = 1)]
        [RegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "Artist Name cannot contain spaces only")]
        public string ArtistName { get; set; }

        [StringLength(100, ErrorMessage = "Composer must be from 1 to 100 symbols", MinimumLength = 1)]
        [RegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "Composer cannot contain spaces only")]
        public string Producer { get; set; }

        [StringLength(100, ErrorMessage = "Arranger must be from 1 to 100 symbols", MinimumLength = 1)]
        [RegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES, ErrorMessage = "Arranger cannot contain spaces only")]
        public string Arranger { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.00, 100000.00, ErrorMessage = "Price must be from 0 to 100000")]
        public decimal Price { get; set; }

        [RegularExpression(ModelRegularExpression.REG_YOUTUBE_LINK, ErrorMessage = "YouTube link is invalid")]
        public string YouTubeLink { get; set; }

        public bool IsTop { get; set; }

        [Required(ErrorMessage = "Image is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Image id is invalid")]
        public int ImageId { get; set; }

        [Required(ErrorMessage = "Sheet preview is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Preview id is invalid")]
        public int PreviewId { get; set; }

        [Required(ErrorMessage = "Songsheet file is required")]
        [Range(1, int.MaxValue, ErrorMessage = "File id is invalid")]
        public int FileId { get; set; }

        public int? TrackId { get; set; }

        public List<int> Instruments { get; set; }

        public List<int> Genres { get; set; }
    }
}
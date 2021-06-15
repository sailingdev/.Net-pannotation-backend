using Pannotation.Models.Enums;

namespace Pannotation.Models.ResponseModels
{
    public class MultipleImagesResponseModel
    {
        public ImageResponseModel Image { get; set; }
                
        public ImageSaveStatus? Status { get; set; }

        public string Name { get; set; }
    }
}

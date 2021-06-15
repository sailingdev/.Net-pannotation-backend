using System.ComponentModel.DataAnnotations;

namespace Pannotation.Domain.Entities
{
    public class Image : IEntity
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        public string OriginalPath { get; set; }

        public string CompactPath { get; set; }

        public bool IsActive { get; set; }

        #endregion
    }
}

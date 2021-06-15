using Pannotation.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pannotation.Domain.Entities.OtherFiles
{
    public class OtherFile : IEntity
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        [MaxLength(250)]
        public string Name { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public OtherFileType FileType { get; set; }

        public bool IsActive { get; set; }

        public int PreviewId { get; set; }

        public int FileId { get; set; }

        #endregion

        #region Navigation Properties

        [ForeignKey("PreviewId")]
        public virtual Image Preview { get; set; }

        [ForeignKey("FileId")]
        public virtual File File { get; set; }

        #endregion
    }
}

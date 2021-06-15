using Pannotation.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Domain.Entities
{
    public class File : IEntity
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        public string Path { get; set; }

        public FileType Type { get; set; }

        #endregion
    }
}

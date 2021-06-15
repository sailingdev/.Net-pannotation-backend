using Pannotation.Common.Extensions;
using Pannotation.Domain.Entities.Order;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pannotation.Domain.Entities
{
    public class Songsheet : IEntity
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string ArtistName { get; set; }

        [MaxLength(100)]
        public string Producer { get; set; }

        [MaxLength(100)]
        public string Arranger { get; set; }

        [MaxLength(300)]
        public string YouTubeLink { get; set; }

        [MaxLength(750)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(8, 2)")]
        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public bool IsTop { get; set; }

        public int ImageId { get; set; }

        public int PreviewId { get; set; }

        public int FileId { get; set; }

        public int? TrackId { get; set; }

        #endregion

        #region Navigation Properties

        [ForeignKey("ImageId")]
        public virtual Image Image { get; set; }

        [ForeignKey("PreviewId")]
        public virtual File Preview { get; set; }

        [ForeignKey("FileId")]
        public virtual File File { get; set; }

        [ForeignKey("TrackId")]
        public virtual File Track { get; set; }

        [InverseProperty("Songsheet")]
        public virtual ICollection<SongsheetInstrument> Instruments { get; set; }

        [InverseProperty("Songsheet")]
        public virtual ICollection<SongsheetGenre> Genres { get; set; }

        [InverseProperty("Songsheet")]
        public virtual ICollection<OrderSongsheet> Orders { get; set; }

        [InverseProperty("Songsheet")]
        public virtual ICollection<CartItem> CartItems { get; set; }

        #endregion

        #region Ctors

        public Songsheet()
        {
            Instruments = Instruments.Empty();
            Genres = Genres.Empty();
            Orders = Orders.Empty();
            CartItems = CartItems.Empty();
        }

        #endregion
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using Pannotation.Models.Enums;

namespace Pannotation.Domain.Entities.Logging
{
    public class EmailLog : IEntity
    {
        #region Properties

        public int Id { get; set; }

        [MaxLength(129)]
        public string Sender { get; set; }

        [MaxLength(129)]
        public string Recipient { get; set; }

        [MaxLength(8000)]
        public string EmailBody { get; set; }

        public SendingStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        #endregion
    }
}

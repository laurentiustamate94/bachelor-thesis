using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace BachelorThesis.Database.Models
{
    [Table("Feedback")]
    public partial class Feedback
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long? LoggingId { get; set; }

        public long? UsersId { get; set; }

        [Required]
        public string RawText { get; set; }

        public virtual Logging Logging { get; set; }

        public virtual Users Users { get; set; }
    }
}
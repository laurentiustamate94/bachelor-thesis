using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace BachelorThesis.Database.Models
{
    [Table("KnowledgeBase")]
    public partial class KnowledgeBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string Answer { get; set; }

        [Required]
        public string Analysis { get; set; }

        [Required]
        [StringLength(64)]
        public string PairChecksum { get; set; }

        [StringLength(256)]
        public string Intent { get; set; }

        public int Hits { get; set; }
    }
}
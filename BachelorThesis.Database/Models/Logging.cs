using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace BachelorThesis.Database.Models
{
    [Table("Logging")]
    public partial class Logging
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Logging()
        {
            Feedback = new HashSet<Feedback>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string MessageId { get; set; }

        [Required]
        public string RawText { get; set; }

        public string TranslateJson { get; set; }

        public string QnAMakerJson { get; set; }

        public string LuisJson { get; set; }

        public string AnalysisJson { get; set; }

        public string CustomJson { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Feedback> Feedback { get; set; }
    }
}
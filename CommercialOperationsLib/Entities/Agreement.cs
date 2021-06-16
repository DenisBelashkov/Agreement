using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Agreement : BaseEntity
    {
        [ForeignKey("CustomerUser")]
        public long CustomerUserId { get; set; }
        
        [ForeignKey("ContractorUser")]
        public long ContractorUserId { get; set; }
        public virtual User CustomerUser { get; set; }
        public virtual User ContractorUser { get; set; }

        public float Cost { get; set; }
        
        [Required]
        public DateTime ConclusionDate { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public virtual IList<Category> Categories { get; set; } = new List<Category>();
        [Required]
        public virtual List<CategoryAgreement> CategoryAgreement { get; set; } = new List<CategoryAgreement>();
    }
}

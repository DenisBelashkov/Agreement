using Core.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Category : BaseEntity
    {
        [Required]
        public string Name { get; set; } //unique
        public virtual IList<Agreement> Agreements { get; set; } = new List<Agreement>();
        public virtual List<CategoryAgreement> CategoryAgreement { get; set; } = new List<CategoryAgreement>();
    }
}

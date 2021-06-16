using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CategoryAgreement : BaseEntity
    {
        public virtual Agreement Agreement { get; set; }
        
        [ForeignKey("AgreementId")]
        public long AgreementId { get; set; }
        
        public virtual Category Category { get; set; }
        [ForeignKey("CategoryId")]
        public long CategoryId { get; set; }
    }
}

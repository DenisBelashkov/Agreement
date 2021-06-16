using Core.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        
        [ForeignKey("UserTypeId")]
        public virtual UserType UserType { get; set; }
        public long UserTypeId { get; set; }
        
        [ForeignKey("BuildingId")]
        public virtual Building Building { get; set; }

        [InverseProperty("CustomerUser")]
        public virtual IList<Agreement> OrderAgreements { get; set; } = new List<Agreement>();

        [InverseProperty("ContractorUser")]
        public virtual IList<Agreement> ContractAgreements { get; set; } = new List<Agreement>();
        public long BuildingId { get; set; }
    }
}

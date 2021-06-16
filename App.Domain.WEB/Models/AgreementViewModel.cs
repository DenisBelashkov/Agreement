using System;
using System.Collections.Generic;

namespace App.Domain.WEB.Models
{
    public class AgreementViewModel
    {
        public long Id { get; set; }
        public UserViewModel CustomerUser { get; set; }
        public UserViewModel ContractorUser { get; set; }
        public float Cost { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public virtual IList<CategoryViewModel> Categories { get; set; } 
        public DateTime ConclusionDate { get; set; }
        
    }
}
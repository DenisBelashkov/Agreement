using System.Collections.Generic;

namespace App.Domain.WEB.Models
{
    public class UserStatsViewModel
    {
        public UserViewModel User { get; set; }
        public List<AgreementViewModel> Agreements { get; set; }

        public float TotalCostForContract { get; set; }
        public int TotalCountForContract { get; set; }
        public float TotalСostForOrder { get; set; }
        public int TotalCountForOrder { get; set; }
        
    }
}
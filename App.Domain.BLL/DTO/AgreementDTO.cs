using System;
using System.Collections.Generic;

namespace App.Domain.BLL.DTO
{
    public class AgreementDTO
    { 
        public long Id { get; set; }

        public IList<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        
        public UserDto CustomerUser { get; set; }
        public UserDto ContractorUser { get; set; }
        public float Cost { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        
        
        public DateTime ConclusionDate { get; set; }
    }
}
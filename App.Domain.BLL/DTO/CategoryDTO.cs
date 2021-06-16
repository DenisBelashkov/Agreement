using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace App.Domain.BLL.DTO
{
    public class CategoryDto
    {
        public string Name { get; set; }
        public IList<AgreementDTO> Agreements { get; set; }
        /*
        public IList<Agreement> Agreements { get; set; }
    */
    }
}

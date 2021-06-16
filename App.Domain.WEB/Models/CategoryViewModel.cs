using System.Collections.Generic;

namespace App.Domain.WEB.Models
{
    public class CategoryViewModel
    {
        public string Name { get; set; }
        public IList<AgreementViewModel> Agreements { get; set; }
    }
}
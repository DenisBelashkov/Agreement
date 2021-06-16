using App.Domain.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.BLL.Interfaces
{
    public interface IAgreementService : IBaseService<AgreementDTO>
    {
        List<AgreementDTO> GetUserCustomerAgreement(UserDto user);
        List<AgreementDTO> GetUserContractorAgreement(UserDto user);
        void Delete(long id);
    }
}

using App.Domain.BLL.DTO;
using System;
using System.Collections.Generic;

namespace App.Domain.BLL.Interfaces
{
    public interface IUserService : IBaseService<UserDto>
    {
        IEnumerable<string> GetAvailableUserTypes();

        void Delete(long id);

        float? GetTotalOrderCostByIdAndDate(long id, DateTime startDate, DateTime endDate);
        float? GetTotalContractCostByIdAndDate(long id, DateTime startDate, DateTime endDate);
        int? GetTotalOrderAmountByIdAndDate(long id, DateTime startDate, DateTime endDate);
        int? GetTotalContractAmountByIdAndDate(long id, DateTime startDate, DateTime endDate);
    }
}

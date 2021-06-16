using System;
using System.Collections.Generic;
using System.Linq;
using App.Domain.WEB.Utils;

namespace App.Domain.WEB.Models
{
    public class UsersFilterViewModel
    {
        [StringInterceptor] public string SortOrder { get; set; } = "";
        [StringInterceptor] public string NameFilter { get; set; } = "";
        [StringInterceptor] public string TypeFilter { get; set; } = "";
        [StringInterceptor] public string AddressFilter { get; set; } = "";

        public int MinOrderAgrAmount { get; set; } = 0;
        public int MaxOrderAgrAmount { get; set; } = int.MaxValue;

        public int MinContractAgrAmount { get; set; } = 0;
        public int MaxContractAgrAmount { get; set; } = int.MaxValue;

        public float MinOrderAgrValue { get; set; } = 0;
        public float MaxOrderAgrValue { get; set; } = float.MaxValue;

        public float MinContractAgrValue { get; set; } = 0;
        public float MaxContractAgrValue { get; set; } = float.MaxValue;

        public DateTime StartDate { get; set; } = DateTime.MinValue;
        public DateTime EndDate { get; set; } = DateTime.MaxValue;


        public void SortUsingOrder(ref List<UserViewModel> usersList)
        {
            usersList = SortOrder switch
            {
                "id_desc" => usersList.AsQueryable().Reverse().ToList(),
                "Name" => usersList.OrderBy(r => r.Name).ToList(),
                "name_desc" => usersList.OrderByDescending(r => r.Name).ToList(),
                "UserType" => usersList.OrderBy(r => r.UserType).ToList(),
                "usertype_desc" => usersList.OrderByDescending(r => r.UserType).ToList(),
                "Address" => usersList.OrderBy(r => r.Address.ToString()).ToList(),
                "address_desc" => usersList.OrderByDescending(r => r.Address.ToString()).ToList(),
                "TotalOrderAmount" => usersList.OrderBy(r => r.TotalOrderAmount).ToList(),
                "total_bought_am_desc" => usersList.OrderByDescending(r => r.TotalOrderAmount).ToList(),
                "TotalContractAmount" => usersList.OrderBy(r => r.TotalContractAmount).ToList(),
                "total_sold_am_desc" => usersList.OrderByDescending(r => r.TotalContractAmount).ToList(),
                "TotalOrderValue" => usersList.OrderBy(r => r.TotalOrderValue).ToList(),
                "total_bought_val_desc" => usersList.OrderByDescending(r => r.TotalOrderValue).ToList(),
                "TotalContractValue" => usersList.OrderBy(r => r.TotalContractValue).ToList(),
                "total_sold_val_desc" => usersList.OrderByDescending(r => r.TotalContractValue).ToList(),
                _ => usersList
            };
        }

        public void ApplyFilter(ref List<UserViewModel> usersList)
        {
            if (NameFilter.Length != 0)
                usersList = usersList
                    .FindAll(user => user
                        .Name.Contains(NameFilter, StringComparison.OrdinalIgnoreCase));

            if (TypeFilter.Length != 0)
                usersList = usersList.FindAll(user => user.UserType.Contains(TypeFilter));

            if (AddressFilter.Length != 0)
                usersList = usersList
                    .FindAll(user => user.Address.ToString()
                        .Contains(AddressFilter, StringComparison.OrdinalIgnoreCase));
        }

        public void FilterByAmounts(ref List<UserViewModel> usersList)
        {
            usersList = usersList
                .Where(user => user.TotalOrderAmount >= MinOrderAgrAmount)
                .Where(user => user.TotalOrderAmount <= MaxOrderAgrAmount)
                .ToList();

            usersList = usersList
                .Where(user => user.TotalContractAmount >= MinContractAgrAmount)
                .Where(user => user.TotalContractAmount <= MaxContractAgrAmount)
                .ToList();

            usersList = usersList
                .Where(user => user.TotalOrderValue >= MinOrderAgrValue)
                .Where(user => user.TotalOrderValue <= MaxOrderAgrValue)
                .ToList();

            usersList = usersList
                .Where(user => user.TotalContractValue >= MinContractAgrValue)
                .Where(user => user.TotalContractValue <= MaxContractAgrValue)
                .ToList();

        }
    }
}
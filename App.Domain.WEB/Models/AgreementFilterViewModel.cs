using System;
using System.Collections.Generic;
using System.Linq;
using App.Domain.WEB.Utils;

namespace App.Domain.WEB.Models
{
    public class AgreementFilterViewModel
    {
        [StringInterceptor] public string SortOrder { get; set; } = "";

        [StringArrayInterceptor] public string[] CatFilter { get; set; } = Array.Empty<string>();

        [StringInterceptor] public string ExecUsrNameFilter { get; set; } = "";
        [StringInterceptor] public string CustomerUsrNameFilter { get; set; } = "";

        public int MinAmount { get; set; } = 0;
        public int MaxAmount { get; set; } = int.MaxValue;

        public float MinValue { get; set; } = 0;
        public float MaxValue { get; set; } = float.MaxValue;

        public DateTime StartDate { get; set; } = DateTime.MinValue;
        public DateTime EndDate { get; set; } = DateTime.MaxValue;
        
        public void SortUsingOrder(ref List<AgreementViewModel> agreementList)
        {
            agreementList = SortOrder switch
            {
                "id_desc" => agreementList.AsQueryable().Reverse().ToList(),
                "AgrCat" => agreementList.OrderBy(r => r.Categories[0].Name).ToList(),
                "cat_name_desc" => agreementList.OrderByDescending(r => r.Categories[0].Name).ToList(),
                "Cost" => agreementList.OrderBy(r => r.Cost).ToList(),
                "cost_desc" => agreementList.OrderByDescending(r => r.Cost).ToList(),
                "ContractorUser" => agreementList.OrderBy(r => r.ContractorUser.Name).ToList(),
                "contractor_usr_desc" => agreementList.OrderByDescending(r => r.ContractorUser.Name).ToList(),
                "CustomerUser" => agreementList.OrderBy(r => r.CustomerUser.Name).ToList(),
                "customerUser_usr_desc" => agreementList.OrderBy(r => r.CustomerUser.Name).ToList(),
                "ConclusionDate" => agreementList.OrderBy(r => r.ConclusionDate).ToList(),
                "date_desc" => agreementList.OrderByDescending(r => r.ConclusionDate).ToList(),
                _ => agreementList
            };
        }
        public void SortByAmount(ref List<AgreementViewModel> agreementList)
        {
            /*agreementList = agreementList
                .Where(it => it.ItemCount <= MaxAmount)
                .Where(it => it.ItemCount >= MinAmount)
                .ToList();*/

            agreementList = agreementList
                .Where(it => it.Cost <= MaxValue)
                .Where(it => it.Cost >= MinValue)
                .ToList();

            agreementList = agreementList
                .Where(it => it.ConclusionDate <= EndDate)
                .Where(it => it.ConclusionDate >= StartDate)
                .ToList();
        }
        public void SortByCatUsr(ref List<AgreementViewModel> agreementList)
        {
            /*if (ItemNameFilter.Length > 0)
                agreementList = agreementList.Where(oper =>
                    ItemNameFilter.Contains(oper.Item.Name)).ToList();*/

            if (CustomerUsrNameFilter.Length > 0)
                agreementList = agreementList.Where(agreement =>
                    agreement.CustomerUser.Name.Contains(CustomerUsrNameFilter)).ToList();

            if (ExecUsrNameFilter.Length != 0)
                agreementList = agreementList.Where(agreement =>
                    agreement.ContractorUser.Name.Contains(ExecUsrNameFilter)).ToList();

            if (CatFilter.Length > 0)
                agreementList = agreementList.Where(agreement =>
                    agreement.Categories.Select(cat => cat.Name)
                        .Intersect(CatFilter).Any()).ToList();
        }
    }
}
using App.Domain.BLL.DTO;
using App.Domain.BLL.Interfaces;
using App.Domain.WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using AutoMapper;
using System.IO;
using ClosedXML.Excel;

namespace App.Domain.WEB.Controllers
{
    public class AgreementController : Controller
    {
        private readonly ILogger<AgreementController> _logger;

        private readonly IAgreementService _agreementService;
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;

        public AgreementController(ILogger<AgreementController> logger,
            IAgreementService agreementService,
            ICategoryService categoryService,
            IUserService userService)
        {
            _logger = logger;
            _agreementService = agreementService;
            _userService = userService;
            _categoryService = categoryService;
        }

        public IActionResult Agreement(AgreementFilterViewModel filterViewModel,
            int pageSize = 5, int pageIndex = 1)
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AgreementDTO, AgreementViewModel>();
                cfg.CreateMap<CategoryDto, CategoryViewModel>();
                cfg.CreateMap<UserDto, UserViewModel>().ForMember(dst => dst.Address, src => src.Ignore());
            }).CreateMapper();

            ViewData["AllUsers"] = mapper.Map<IEnumerable<UserDto>, List<UserViewModel>>(_userService.GetAll());
            ViewData["AllCats"] = mapper.Map<IEnumerable<CategoryDto>, List<CategoryViewModel>>(_categoryService.GetAll());
          
            //ViewData["AllGroups"] = new[] {"Buying User", "Selling User", "Item"};
            
            
            ViewData["SortOrder"] = filterViewModel.SortOrder;
            ViewData["IdSortParam"] = String.IsNullOrEmpty(filterViewModel.SortOrder) ? "id_desc" : "";
            
            //ViewData["ItemNameSortParam"] = filterViewModel.SortOrder == "ItemName" ? "name_desc" : "ItemName";
            
            ViewData["AgrCatSortParam"] = filterViewModel.SortOrder == "AgrCat" ? "cat_name_desc" : "AgrCat";
            //ViewData["ItemCountSortParam"] = filterViewModel.SortOrder == "ItemCount" ? "count_desc" : "ItemCount";
            ViewData["CostSortParam"] = filterViewModel.SortOrder == "Cost" ? "cost_desc" : "Cost";
            
            ViewData["CustomerUserSortParam"] = filterViewModel.SortOrder == "CustomerUser" ? "customerUser_usr_desc" : "CustomerUser";
            ViewData["ContractorUserSortParam"] = filterViewModel.SortOrder == "ContractorUser" ? "contractor_usr_desc" : "ContractorUser";
            ViewData["DateSortParam"] = filterViewModel.SortOrder == "ConclusionDate" ? "date_desc" : "ConclusionDate";

            
           // ViewData["ItemNameFilter"] = filterViewModel.ItemNameFilter;
            ViewData["CatFilter"] = filterViewModel.CatFilter;
            ViewData["CustomerUserFilter"] = filterViewModel.CustomerUsrNameFilter;
            ViewData["ExecUsrFilter"] = filterViewModel.ExecUsrNameFilter;

            ViewData["MinVal"] = filterViewModel.MinValue == 0 ? null : filterViewModel.MinValue;
            ViewData["MaxVal"] = filterViewModel.MaxValue == float.MaxValue ? null : filterViewModel.MaxValue;

            ViewData["MinAmount"] = filterViewModel.MinAmount == 0 ? null : filterViewModel.MinAmount;
            ViewData["MaxAmount"] = filterViewModel.MaxAmount == int.MaxValue ? null : filterViewModel.MaxAmount;

            ViewData["StartDate"] = filterViewModel.StartDate == DateTime.MinValue ? "" : filterViewModel.StartDate.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = filterViewModel.EndDate == DateTime.MaxValue ? "" : filterViewModel.EndDate.ToString("yyyy-MM-dd");

            //ViewData["CurrGroup"] = filterViewModel.Group;
            
            var result = FilterResults(filterViewModel);

            ViewData["TotalAmount"] = result.Count;
            ViewData["TotalValue"] = result.Select(it => it.Cost).Sum();
            
            _logger.LogInformation($"Viewing info about all operations: pageIndex={pageIndex} with pageSize={pageSize}");
            
            return View(PaginatedList<AgreementViewModel>.CreateList(result.AsQueryable(), pageIndex, pageSize));
        }

        public IActionResult Download(AgreementFilterViewModel filter)//TODO
        {
            var result = FilterResults(filter);
            
                using var workbook = new XLWorkbook();

                _logger.LogInformation($"Saving xlsx file for operations");

                var worksheet = workbook.Worksheets.Add("Agreements");
                worksheet.Cell("A1").Value = "Id";
                //worksheet.Cell("B1").Value = "Item Name";
                worksheet.Cell("B1").Value = "Agreement Categories";
                worksheet.Cell("C1").Value = "Agreement Count";
                worksheet.Cell("D1").Value = "Agreement Cost";
                worksheet.Cell("E1").Value = "Contractor User Name";
                worksheet.Cell("F1").Value = "Contractor User Type";
                worksheet.Cell("G1").Value = "Customer User Name";
                worksheet.Cell("H1").Value = "Customer User Type";
                worksheet.Cell("I1").Value = "Agreement ConclusionDate";

                worksheet.Cell("M1").Value = "Total Amount";
                worksheet.Cell("N1").Value = "Total Cost";

                int row = 1;
                foreach (var operation in result)
                {
                    var rowObj = worksheet.Row(++row);
                    rowObj.Cell(1).Value = operation.Id;
                   // rowObj.Cell(2).Value = operation.Item.Name;
                    rowObj.Cell(2).Value = String.Join(",", operation.Categories.Select(cat => cat.Name).ToArray());
                    rowObj.Cell(3).Value = 1;
                    rowObj.Cell(4).Value = operation.Cost;
                    rowObj.Cell(5).Value = operation.ContractorUser.Name;
                    rowObj.Cell(6).Value = operation.ContractorUser.UserType;
                    rowObj.Cell(7).Value = operation.CustomerUser.Name;
                    rowObj.Cell(8).Value = operation.CustomerUser.UserType;
                    rowObj.Cell(9).Value = operation.ConclusionDate.ToString("d", CultureInfo.CreateSpecificCulture("ru-RU"));
                }

                worksheet.Cell("M2").FormulaA1 = $"=SUM($C$2:$C${row})";
                worksheet.Cell("N2").FormulaA1 = $"=SUM($D$2:$D${row})";
                
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = "Agreements.xlsx",
                    Inline = false,
                };

                Response.Headers.Add("Content-Disposition", cd.ToString());
                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Agreements.xlsx");
                }
        }

        private List<AgreementViewModel> FilterResults(AgreementFilterViewModel filter)
        {
            var res = _agreementService.GetAll();

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AgreementDTO, AgreementViewModel>();
                cfg.CreateMap<CategoryDto, CategoryViewModel>();
                cfg.CreateMap<UserDto, UserViewModel>().ForMember(dst => dst.Address, src => src.Ignore());
            }).CreateMapper();

            var result = mapper.Map<IEnumerable<AgreementDTO>, List<AgreementViewModel>>(res);
            
            filter.SortByAmount(ref result);
            filter.SortByCatUsr(ref result);
            filter.SortUsingOrder(ref result);
            
            return result;
        }
        
        [HttpPost]
        public IActionResult Agreement(AgreementViewModel agreement, long buyingUser, long sellingUser, long item,string[] categories)
        {
            
            if (agreement.Id == 0)
                _logger.LogInformation($"Adding new agreement");
            else
                _logger.LogInformation($"Updating agreement with id={agreement.Id}");

            if (buyingUser == sellingUser)
                return RedirectPermanent("~/Agreement/Agreement");

            agreement.CustomerUser = new UserViewModel { Id = buyingUser };
            agreement.ContractorUser = new UserViewModel { Id = sellingUser };
            
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserViewModel, UserDto>();
                cfg.CreateMap<AgreementViewModel, AgreementDTO>();
                cfg.CreateMap<CategoryViewModel, CategoryDto>();
            }).CreateMapper();

            AgreementDTO agreementDto =mapper.Map<AgreementViewModel, AgreementDTO>(agreement);

            agreementDto.Categories = _categoryService.FromStringListIgnoreItems(categories.ToList());
            
            _agreementService.AddOrUpdate(agreementDto);
            return RedirectPermanent("~/Agreement/Agreement");
        }

        public IActionResult RemoveAgreement(long id)
        {
            _logger.LogInformation($"Removing agreement with id={id}");
            _agreementService.Delete(id);
            return RedirectPermanent("~/Agreement/Agreement");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using App.Domain.BLL.DTO;
using App.Domain.BLL.Interfaces;
using App.Domain.WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using System.Threading.Tasks;

namespace App.Domain.WEB.Controllers
{
    public class IndexController : Controller
    {
        private readonly ILogger<IndexController> _logger;
        private readonly IUserService _userService;
        private readonly ICategoryService _categoryService;
        private readonly IAgreementService _agreementService;
        private readonly IAddressService _addressService;

        public IndexController(ILogger<IndexController> logger,
            IUserService userService,
            ICategoryService categoryService,
            IAgreementService agreementService,
            IAddressService addressService)
        {
            _logger = logger;
            _userService = userService;
            _categoryService = categoryService;
            _agreementService = agreementService;
            _addressService = addressService;
        }

        public IActionResult Index()
        {
            var result = new List<string>();

            result.Add($"User Amount: {_userService.GetAll().Count()}");
           // result.Add($"Item Category Amount: {_categoryService.GetAll().Count()}");
            result.Add($"Agreement Amount: {_agreementService.GetAll().Count()}");

            var agreements = _agreementService.GetAll();

            if (!agreements.Any())
                return View(result);

            /*var theMostSellingItemByPrice = agreements
                .GroupBy(op => op.Item.Id)
                .OrderByDescending(gr => gr.Sum(item => item.Cost))
                .FirstOrDefault()!.Select(igr => igr.Item.Name).FirstOrDefault();
            
            var theMostSellingItemByAmount = agreements
                .GroupBy(op => op.Item.Id)
                .OrderByDescending(gr => gr.Sum(item => item.ItemCount))
                .FirstOrDefault()!.Select(igr => igr.Item.Name).FirstOrDefault();*/

            var theMostOrderUser = agreements
                .GroupBy(op => op.CustomerUser.Id)
                .OrderByDescending(gr => gr.Sum(item => item.Cost))
                .FirstOrDefault()!.Select(igr => igr.CustomerUser.Name).FirstOrDefault();
            
            var theMostContractUser = agreements
                .GroupBy(op => op.ContractorUser.Id)
                .OrderByDescending(gr => gr.Sum(item => item.Cost))
                .FirstOrDefault()!.Select(igr => igr.ContractorUser.Name).FirstOrDefault();

            /*result.Add($"The Most Selling Item (By Price) {theMostSellingItemByPrice}");
            result.Add($"The Most Selling Item (By Amount) {theMostSellingItemByAmount}");*/
            result.Add($"User, Who Order The Most Agreement (By Cost) {theMostOrderUser}");
            result.Add($"User, Who Execute The Most Agreement (By Cost) {theMostContractUser}");

            return View(result);
        }

        /*public IActionResult ItemStats(int id)
        {

            _logger.LogInformation($"Showing statst for item with id={id}");
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserDto, UserViewModel>();
                cfg.CreateMap<CategoryDto, CategoryViewModel>();
                cfg.CreateMap<AddressDto, AddressViewModel>();
                cfg.CreateMap<AgreementDTO, AgreementViewModel>();
            }).CreateMapper();

            var item = _itemService.Find(id);
            
            if (item is null)
                return Redirect(Request.Headers["Referer"].ToString());

            var itemStats = new ItemStatsViewModel { Item = mapper.Map<ItemDto, ItemViewModel>(item) };
            
            var opertations = _agreementService.GetAll().Where(op => op.Item.Id == item.Id);
          
            itemStats.Item.TotalAmount = _itemService.GetTotalAmount(itemStats.Item.Id);
            itemStats.Item.TotalValue = _itemService.GetTotalValue(itemStats.Item.Id);

            if (!opertations.Any())
            {
                itemStats.MostPopularCityToSell = mapper.Map<AddressDto, AddressViewModel>(opertations.GroupBy(op =>
                        _addressService.Find(op.ContractorUser.BuildingId))
                            .OrderByDescending(group => group.Count())
                            .Select(grp => grp.Key).First());
                
                itemStats.MostPopularCityToBuy = mapper.Map<AddressDto, AddressViewModel>(opertations.GroupBy(op =>
                        _addressService.Find(op.CustomerUser.BuildingId))
                            .OrderByDescending(group => group.Count())
                            .Select(grp => grp.Key).First());
            }

            return View(itemStats);
        }*/

        public IActionResult UserStats(int id)
        {

            _logger.LogInformation($"Showing stats for user with id={id}");

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserDto, UserViewModel>();
                cfg.CreateMap<CategoryDto, CategoryViewModel>();
                cfg.CreateMap<AddressDto, AddressViewModel>();
                cfg.CreateMap<AgreementDTO, AgreementViewModel>();
            }).CreateMapper();

            var user = _userService.Find(id);

            if (user is null)
                return Redirect(Request.Headers["Referer"].ToString());

            var userStats = new UserStatsViewModel { User = mapper.Map<UserDto, UserViewModel>(user) };

            var agreement = _agreementService.GetAll();

            userStats.TotalCountForContract = agreement.Count(agr => agr.ContractorUser.Id == user.Id);
            userStats.TotalCostForContract = agreement.Where(agr => agr.ContractorUser.Id == user.Id).Select(op => op.Cost).Sum();

            userStats.TotalCountForOrder = agreement.Count(agr => agr.CustomerUser.Id == user.Id);
            userStats.TotalСostForOrder = agreement.Where(agr => agr.CustomerUser.Id == user.Id).Select(op => op.Cost).Sum();

            //userStats.Agreements = mapper.Map<IEnumerable<ItemDto>, List<ItemViewModel>>(agreement.Where(op => op.CustomerUser.Id == user.Id || op.ContractorUser.Id == user.Id).Select(op => op.Item).ToList());
            userStats.Agreements = mapper.Map<IEnumerable<AgreementDTO>, List<AgreementViewModel>>(agreement.Where(op => op.CustomerUser.Id == user.Id || op.ContractorUser.Id == user.Id).ToList());

            return View(userStats);
        }

        public IActionResult Privacy()
        {
            _logger.LogInformation("Viewing privacy information");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

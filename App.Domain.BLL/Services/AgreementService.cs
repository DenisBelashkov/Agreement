using App.Domain.BLL.DTO;
using App.Domain.BLL.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.BLL.Services
{
    public class AgreementService : IAgreementService
    {
        private readonly IRepository<Agreement> _agreementRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<User> _userRepository;
        
        //private readonly IItemService _itemService;
        private readonly IUserService _userService;

        public AgreementService(IRepository<Agreement> agreementRepository, IRepository<Category> categoryRepository, IRepository<User> userRepository, IUserService userService)
        {
            _agreementRepository = agreementRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _userService = userService;
        }
        

        public IList<AgreementDTO> OrderOperations(UserDto user)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<AgreementDTO, Agreement>()).CreateMapper();
            IList<Agreement> agreements = _agreementRepository.FindInclude(usr => usr.ContractorUser.Id == user.Id,  oper => oper.ContractorUser).ToList();
            return mapper.Map<IList<Agreement>, List<AgreementDTO>>(agreements); 
        }

        public long AddOrUpdate(AgreementDTO dto)
        {
            var agreement = _agreementRepository.Find(dto.Id);

            if (agreement is null)
            {
                agreement = new Agreement {Cost = dto.Cost,
                    ConclusionDate = dto.ConclusionDate,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate
                };
            }

            agreement.Cost = dto.Cost;
            agreement.ConclusionDate = dto.ConclusionDate;
            agreement.StartDate = dto.StartDate;
            agreement.EndDate = dto.EndDate;
            
            var customerUser = _userRepository.Find(dto.CustomerUser.Id);
            var contractorUser = _userRepository.Find(dto.ContractorUser.Id);
            var category = _categoryRepository.Find(a => a.Name.Equals(dto.Categories[0].Name)).FirstOrDefault();


            if (customerUser is null)
                customerUser = _userRepository.Find(_userService.AddOrUpdate(dto.CustomerUser));

            if (contractorUser is null)
                contractorUser = _userRepository.Find(_userService.AddOrUpdate(dto.ContractorUser));

            if (category is null) {
                _categoryRepository.CreateOrUpdate(new Category { Name = dto.Categories[0].Name });
                category = _categoryRepository.Find(a => a.Name.Equals(dto.Categories[0].Name)).FirstOrDefault();
            }
            
                

            agreement.CustomerUser = customerUser;
            agreement.ContractorUser = contractorUser;
            agreement.Categories.Add(category);
            
            _agreementRepository.CreateOrUpdate(agreement);
            return agreement.Id;
        }

        public AgreementDTO Find(long id)
        {
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Agreement, AgreementDTO>();
                cfg.CreateMap<User, UserDto>().ForMember(user => user.UserType, dst => dst.MapFrom(userDTO => userDTO.UserType.Type));
                cfg.CreateMap<Category, CategoryDto>();
            }).CreateMapper();

            return mapper.Map<Agreement, AgreementDTO>(_agreementRepository.Find(id));
        }

        public List<AgreementDTO> GetAll()
        {
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Agreement, AgreementDTO>();
                cfg.CreateMap<User, UserDto>().ForMember(user => user.UserType, dst => dst.MapFrom(userDTO => userDTO.UserType.Type));
                cfg.CreateMap<Category, CategoryDto>();
            }).CreateMapper();

            var res = _agreementRepository.GetAll();

            return mapper.Map<IList<Agreement>, List<AgreementDTO>>(res);
        }


        public IList<AgreementDTO> OrderAgreements(UserDto user)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<AgreementDTO, Agreement>()).CreateMapper();
            IList<Agreement> agreements = _agreementRepository.FindInclude(usr => usr.CustomerUser.Id == user.Id, oper => oper.CustomerUser).ToList();
            return mapper.Map<IList<Agreement>, List<AgreementDTO>>(agreements);
        }

        public List<AgreementDTO> GetUserCustomerAgreement(UserDto user)
        {
            return new List<AgreementDTO>();//TODO
        }

        public List<AgreementDTO> GetUserContractorAgreement(UserDto user)
        {
            return new List<AgreementDTO>();//TODO
        }

        public void Delete(long id)
        {
            _agreementRepository.Delete(id);
        }
    }
}

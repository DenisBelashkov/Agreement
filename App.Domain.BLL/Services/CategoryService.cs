using App.Domain.BLL.DTO;
using App.Domain.BLL.Infrastructure;
using App.Domain.BLL.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace App.Domain.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Agreement> _agreementRepository;

        public CategoryService(IRepository<Category> repo, IRepository<Agreement> agreementRepository)
        {
            _categoryRepository = repo;
            _agreementRepository = agreementRepository;
        }


        public long AddOrUpdate(CategoryDto dto)
        {
            var category = _categoryRepository
                .Find(cat => cat.Name.Equals(dto.Name)).FirstOrDefault();

            if (category is null)
                category = new Category();

            foreach (var agreementDto in dto.Agreements)
            {
                var agreement = _agreementRepository.Find(agreementDto.Id);

                if (agreement is null)
                {
                    agreement = new Agreement();
                    _agreementRepository.CreateOrUpdate(agreement);
                }
                
                if (category.Agreements.All(it => it.Id != agreement.Id))
                    category.Agreements.Add(agreement);
            }
            
            _categoryRepository.CreateOrUpdate(category);
            return category.Id;
        }

        public CategoryDto Find(long id)
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Category, CategoryDto>();
                cfg.CreateMap<Agreement, AgreementDTO>();
            }).CreateMapper();

            return mapper.Map<Category, CategoryDto>(_categoryRepository.Find(id));
        }

        public IList<CategoryDto> FromStringListIgnoreItems(IEnumerable<string> categoryNames)
        {
            IList<CategoryDto> categories = new List<CategoryDto>();
            
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Category, CategoryDto>()
                .ForMember(cat => cat.Agreements, dest => dest.Ignore());
            }).CreateMapper();

            foreach(var str in categoryNames)
            {
                if (!_categoryRepository.Exists(item => item.Name.Equals(str)))
                    _categoryRepository.Create(new Category { Name = str });

                var category = _categoryRepository
                    .Find(item => item.Name.Equals(str))
                    .First();
                
                var categoryDto = mapper.Map<Category, CategoryDto>(category);
                categories.Add(categoryDto);
            }

            return categories;
        }

        public List<CategoryDto> GetAll()
        {
            var mapper = new MapperConfiguration(cfg => { 
                cfg.CreateMap<Agreement, AgreementDTO>(); 
                cfg.CreateMap<Category, CategoryDto>();
                cfg.CreateMap<User, UserDto>();
            }).CreateMapper();
            
            return mapper.Map<IList<Category>, List<CategoryDto>>(_categoryRepository.GetAll());
        }

        public void Delete(CategoryDto categoryDto)
        {
            Category category = _categoryRepository.Find(dbItem => 
                dbItem.Name.Equals(categoryDto.Name)).FirstOrDefault();
            
            if(category is not null)
                _categoryRepository.Delete(category.Id);
        }
    }
}

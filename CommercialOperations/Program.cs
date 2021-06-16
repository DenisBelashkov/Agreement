using App.Domain.BLL;
using App.Domain.BLL.DTO;
using App.Domain.BLL.Infrastructure;
using App.Domain.BLL.Interfaces;
using App.Domain.BLL.Services;
using Core.Entities;
using Core.Interfaces;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Core.EF;

namespace CommercialOperations
{
    class Program
    {
        static void Main(string[] args)
        {
            using (MyContext db = new MyContext(new DbContextOptions<MyContext>()))
            {
                Random random = new Random();
                var userRepo = new Repository<User>(db);
                var opRepo = new Repository<Agreement>(db);
                var catRepo = new Repository<Category>(db);
                
                //var allItems = itemRepo.GetAll();
                var allUsers = userRepo.GetAll();
                var allCats = catRepo.GetAll();
                

                for (int i = 0; i < 50; i++)
                {
                    
                    IList<Category> categories = new List<Category>();
                    var contractorUser = allUsers[random.Next(allUsers.Count)];
                    var customerUser = allUsers[random.Next(allUsers.Count)];
                    var category = allCats[random.Next(allCats.Count)];
                    
                    while (customerUser.Id == contractorUser.Id)
                    {
                        customerUser = allUsers[random.Next(allUsers.Count)];
                    }
                    
                   
                    categories.Add(category);
                    var cost = 100000 - random.NextDouble() * 100000;
                    DateTime start = new DateTime(2010, 1, 1);
                    int range = (DateTime.Today - start).Days;           
                    var date = start.AddDays(random.Next(range));

                    var agreement = new Agreement { CustomerUser = customerUser, 
                        ContractorUser = contractorUser,  Cost = (float) cost,
                        Categories =  categories , ConclusionDate = date};
                    
                    opRepo.Create(agreement);
                }
                
            }

            Console.WriteLine("ok");
        }
    }
}

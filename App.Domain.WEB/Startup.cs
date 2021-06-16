using App.Domain.BLL.Interfaces;
using App.Domain.BLL.Services;
using Core.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using App.Domain.WEB.Utils;
using System.Reflection;
using Core.EF;
using Microsoft.EntityFrameworkCore;

namespace App.Domain.WEB
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            //services.AddDbContext<MyContext>(options => options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=mydb;Trusted_Connection=True;"));
            services.AddDbContext<MyContext>(options => options.UseNpgsql("Host=localhost;Database=admin2;Username=postgres;Password=admin;"));

            services.AddAllGenericTypes(typeof(IRepository<>), new[]{typeof(MyContext).GetTypeInfo().Assembly});
           
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<ICategoryService, CategoryService>();// dependency injection 
            services.AddScoped<IAgreementService, AgreementService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "log.txt"));
            var logger = loggerFactory.CreateLogger("FileLogger");

            if (env.IsDevelopment())
            {
                logger.LogInformation("Using developer exception page", null);
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Item/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Index}/{action=Index}/{id?}");
            });



        }
    }
}

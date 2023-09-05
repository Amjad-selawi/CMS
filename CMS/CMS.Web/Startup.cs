using CMS.Domain;
using CMS.Repository.Implementation;
using CMS.Repository.Interfaces;
using CMS.Repository.Repositories;
using CMS.Services.Interfaces;
using CMS.Services.Services;
using CMS.Web.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CMS.Web
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
             
            services.AddScoped<ICarrerOfferService, CarrerOfferService>();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Defultconiction"));
            });


            services.AddScoped(typeof(ICompanyRepository), typeof(CompanyRepository));
            services.AddTransient<ICompanyService, CompanyService>();

            services.AddScoped(typeof(ICountryRepository), typeof(CountryRepository));
            services.AddTransient<ICountryService, CountryService>();

            services.AddScoped(typeof(IPositionRepository), typeof(PositionRepository));
            services.AddTransient<IPositionService, PositionService>();
            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });


            services.AddScoped(typeof(ICarrerOfferRepository), typeof(CarrerOfferRepository));
            services.AddScoped<ICarrerOfferService, CarrerOfferService>();
            services.AddScoped(typeof(ICandidateRepository), typeof(CandidateRepository));
            services.AddScoped<ICandidateService, CandidateService>();

            services.AddScoped(typeof(IAttachmentRepository), typeof(AttachmentRepository));
            services.AddScoped<IAttachmentService, AttachmentService>();









            services.AddScoped(typeof(IInterviewsRepository), typeof(InterviewsRepository));
            services.AddTransient<IInterviewsService, InterviewsService>();


            services.AddScoped(typeof(INotificationsRepository), typeof(NotificationsRepository));
            services.AddTransient<INotificationsService, NotificationsService>();


            services.AddScoped(typeof(ITemplatesRepository), typeof(TemplatesRepository));
            services.AddTransient<ITemplatesService, TemplatesService>();



            services.AddDbContext<ApplicationDbContext>(x =>
            {
                x.UseSqlServer(Configuration.GetConnectionString("SqlCon"));
            });

            services.AddDefaultIdentity<IdentityUser>().AddDefaultTokenProviders().AddRoles<IdentityRole>()
          .AddEntityFrameworkStores<ApplicationDbContext>();





        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
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
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

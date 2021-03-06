﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrPet.Web.Data;
using DrPet.Web.Data.Entities;
using DrPet.Web.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DrPet.Web.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace DrPet.Web
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
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzIxMTA5QDMxMzgyZTMyMmUzMEFhNGRaLzVOL0dWVTU0MGZveFh5VzBmbVBlZ1RKTkhGZ2JZNnJwdks0dEk9");

            services.AddIdentity<User, IdentityRole>(cfg =>
            {
                cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                cfg.SignIn.RequireConfirmedEmail = true;
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequireDigit = false;
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequiredLength = 6;
            })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<DataContext>();

            services.AddDbContext<DataContext>(cfg =>
            {
                cfg.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddTransient<SeedDB>();

            services.AddScoped<IAnimalRepository, AnimalRepository>();
            services.AddScoped<IUserHelper, UserHelper>();
            services.AddScoped<IConverterHelper, ConverterHelper>();
            services.AddScoped<IImageHelper, ImageHelper>();
            services.AddScoped<IMailHelper, MailHelper>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<ISpecializationRepository, SpecializationRepository>();
            services.AddScoped<IDashBoardHelper, DashBoardHelper>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.ConfigureApplicationCookie(options => 
            {
                options.LoginPath = "/Account/NotAuthorized"; //MANDA PARA ESTE CAMINHO EM VEZ DE LOGIN SE NAO ESTIVER LOGADO E FOR NECESSARIO AUTENTICAÇAO
                options.AccessDeniedPath = "/Account/NotAuthorized";
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()); //ADICIONEI POR CAUSA DO CALENDARIO

            services.AddMvc()
    .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
    .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
    .AddJsonOptions(opt => opt.SerializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat)
    .AddJsonOptions(opt => opt.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

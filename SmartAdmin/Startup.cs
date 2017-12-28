using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartAdmin.Services;
using Smart.Data.Context;
using Smart.Core.Interface.Base;
using Smart.Data.Repository;
using Smart.Core.Identity;
using SmartAdmin.Helpers;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Smart.Services.Interfaces;
using Smart.Services.Entity;
using SmartAdmin.Formatters;
using Microsoft.Net.Http.Headers;
using AutoMapper;
using SmartAdmin.Authorization;

namespace SmartAdmin
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        public IConfigurationRoot Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var csvFormatterOptions = new CsvFormatterOptions();
            services.Configure<Settings>(Configuration);
            
            services.AddOptions();
            var appConfig = Configuration.GetSection("Settings").Get<Settings>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            string connection = Configuration["ConnectionStrings:DefaultConnection"].ToString();

            services.AddDbContext<SmartContext>(opt => opt.UseSqlServer(connection));


            

            services.AddIdentity<ApplicationUser, IdentityRole>(options => {
                options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.Cookies.ApplicationCookie.AccessDeniedPath = "/home/access-denied";
                options.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<SmartContext>().AddDefaultTokenProviders();
            


            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IServices<>), typeof(Services<>));
            services.AddScoped<IUser, User>();

            //services.AddScoped<IBusinessEntityService, BusinessEntityService>();
            //services.AddScoped<IUserSettingService, UserSettingService>();
            //services.AddScoped<IUserBusinessEntityService, UserBusinessEntityService>();

            services.AddLocalization(options => 
            {
                options.ResourcesPath = "Resources";
            });
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("pt-BR")
                };
                options.DefaultRequestCulture = new RequestCulture(culture: "pt-BR", uiCulture: "pt-BR");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;


                //options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(async A => { return new ProviderCultureResult("en"); }));



            });


            services.AddMvc(options => 
            {
               // options.SslPort = 44368;
               // options.Filters.Add(new RequireHttpsAttribute());

                options.InputFormatters.Add(new CsvInputFormatter(csvFormatterOptions));
                options.OutputFormatters.Add(new CsvOutputFormatter(csvFormatterOptions));
                options.FormatterMappings.SetMediaTypeMappingForFormat("csv", MediaTypeHeaderValue.Parse("text/csv"));

            }).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix, opts => { opts.ResourcesPath = "Resources"; })
            .AddDataAnnotationsLocalization();
            services.AddAutoMapper();


            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanManageTask", policy => policy.Requirements.Add(new ClaimRequirement("Task", "Write")));
            });

            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromHours(8);
                options.CookieHttpOnly = true;
            });



            //services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.Configure<AuthMessageSenderOptions>(opt =>
            {
                opt.SendGridKey = Configuration["AuthMessageSender:SendGridKey"].ToString();
                opt.SendGridUser = Configuration["AuthMessageSender:SendGridUser"].ToString();

            });



        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

          

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/StatusCode");
            }

            app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");
            app.UseStatusCodePagesWithRedirects("/StatusCode/{0}");
            
            app.UseStaticFiles();
            app.UseIdentity();
            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715
            // IMPORTANT: This session call MUST go before UseMvc()
            app.UseSession();

            //app.UseFacebookAuthentication(new FacebookOptions()
            //{
            //    AppId = Configuration["Authentication:Facebook:AppId"],
            //    AppSecret = Configuration["Authentication:Facebook:AppSecret"]
            //});


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

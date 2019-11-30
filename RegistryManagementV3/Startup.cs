using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RegistryManagementV3.Controllers.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Models.Repository;
using RegistryManagementV3.Services;
using RegistryManagementV3.Services.Notifications;
using RegistryManagementV3.Services.resources;

namespace RegistryManagementV3
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            services.AddDbContext<SecurityDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            var serviceProvider = services.BuildServiceProvider();
            var unitOfWork = new UnitOfWork(serviceProvider.GetRequiredService<SecurityDbContext>());
            services.AddScoped<IUnitOfWork>(provider => unitOfWork);
            services.AddScoped<ICatalogService>(provider => new CatalogService(unitOfWork));
            var tagService = new TagService(unitOfWork);
            services.AddScoped<ITagService>(provider => tagService);
            
            var registryPath = Configuration.GetValue<string>("RegistryPath");
            services.AddScoped<IResourceService>(provider => new ResourceService(unitOfWork, tagService, registryPath));
            
            services.AddScoped<ISearchService>(provider => new SearchService(unitOfWork));
            services.AddScoped<IUserGroupService>(provider => new UserGroupService(unitOfWork));

            var userResourceManagementService = new UserResourceManagementService(unitOfWork);
            var adminResourceManagementService = new AdminResourceManagementService(unitOfWork);

            var awsOptions = Configuration.GetAWSOptions();
            services.AddDefaultAWSOptions(awsOptions);
            var awsAccessKeyId = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            var awsSecretAccessKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
            var amazonSimpleNotificationServiceClient = new AmazonSimpleNotificationServiceClient(new BasicAWSCredentials(awsAccessKeyId, awsSecretAccessKey), awsOptions.Region);
            services.AddScoped<IAmazonSimpleNotificationService>(provider => amazonSimpleNotificationServiceClient);
            var userNotifier =
                new AwsSmsUserNotifier(amazonSimpleNotificationServiceClient);
            
            services.AddScoped<ISmsUserNotifier>(provider => userNotifier);
            services.AddScoped<IUserService>(provider => new UserService(unitOfWork, userNotifier));
            
            services.AddScoped<ResourceManagementService>(provider => userResourceManagementService);
            services.AddScoped<ResourceManagementService>(provider => adminResourceManagementService);
            services.AddScoped<ResourceManagementStrategy>(provider =>
                new ResourceManagementStrategy(adminResourceManagementService, userResourceManagementService));
            
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<SecurityDbContext>()
                .AddDefaultTokenProviders();

            services
                .AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, RegistryManagementClaimsPrincipalFactory>();
            
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            services.AddMvc(options => options.Filters.Add(new AuthorizeFilter()));
            services.AddRouting();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/LogOff";
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("onlyUsersWithApprovedAccountPolicy",
                    x => { x.RequireClaim("accountStatus", AccountStatus.Approved.ToString()); });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            CreateRoles(serviceProvider).Wait();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Static")),
                RequestPath = "/Static"
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else
            {
                app.UseExceptionHandler("/Error");   
            }

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRouting();
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<SecurityDbContext>();
                context.Database.EnsureCreated();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
            });
        }

        private static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = {"Admin", "User", "ResourceAuthor"};

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
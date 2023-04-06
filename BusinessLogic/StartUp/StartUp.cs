using BusinessLogic.ApiClasses;
using Contracts;
using Entities;
using Entities.Models;
using LoggerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Repository;
using System.Text;
using EmailService;
using BusinessLogic.Services;
using Microsoft.AspNetCore.Localization;
using Quartz;
using BusinessLogic.Services.Jobs;
using Entities.Models.CorePushModels;
namespace BusinessLogic.StartUp
{
    public static class StartUp
    {
        public static void ConfigureCors(this IServiceCollection services) =>
                     services.AddCors(options =>
                     {
                         options.AddPolicy("CorsPolicy", builder =>
                         builder.AllowAnyOrigin()
                         .AllowAnyMethod()
                         .AllowAnyHeader()
                         .WithExposedHeaders("X-Pagination"));
                     });
        public static void ConfigureAuthenticationManagerService(this IServiceCollection services) =>
         services.AddScoped<IAuthenticationManager, AuthenticationManager>();
        public static void ConfigureIISIntegration(this IServiceCollection services) =>
                    services.Configure<IISOptions>(options =>{});
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = Environment.GetEnvironmentVariable("SECRET");
            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
                    ValidAudience = jwtSettings.GetSection("validAudience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
        }
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<User>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 10;
                o.User.RequireUniqueEmail = true;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(Role),
           builder.Services);
            builder.AddEntityFrameworkStores<RepositoryContext>()
            .AddDefaultTokenProviders();
            services.AddIdentity<User, Role>();
        }
        public static void ConfigureBusinessLogic(this IServiceCollection services)
        {
            services.AddScoped<HomeBL>();
            services.AddScoped<ImageBL>();
            services.AddScoped<ProductBL>();
            services.AddScoped<UserBL>();
            services.AddScoped<CartBL>();
            services.AddScoped<ImageUploadServices>();
            services.AddScoped<OrderBL>();
            services.AddScoped<LocationTaxBL>();
            services.AddScoped<NewsBL>();
        }
        public static void ConfigureEmailConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            services.AddScoped<IEmailSender, EmailSender>();
        } 
        public static void ConfigureSqlConnection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(
               configuration.GetConnectionString("SqlConnection"),
                  x => x.MigrationsAssembly("EtayfeAdminPanel")));
        }
        public static void ConfigureFcmNotification(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("NotificationSettings");
            services.Configure<FcmNotificationSetting>(appSettingsSection);
        }
        public static void ConfigureLocService(this IServiceCollection services)
        {
            services.AddSingleton<LocService>();
            services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(culture: "ar", uiCulture: "ar");
                options.AddSupportedCultures("ar", "en");
                options.AddSupportedUICultures("ar", "en");
                options.FallBackToParentCultures = true;
                options.ApplyCurrentCultureToResponseHeaders = true;
            });

        }
        public static void ConfigureAddQuartz(this IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();
                q.UseTimeZoneConverter();

                var NotifyjobKey = new JobKey("NotificationJob");
                q.AddJob<NotificationJob>(opts => opts.WithIdentity(NotifyjobKey));
                q.AddTrigger(opts => opts
                    .ForJob(NotifyjobKey)
                    .WithIdentity("NotificationJob-trigger")
                    .WithCronSchedule("0/5 * * * * ?"));
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        } 
        public static void ConfigureNotificationService(this IServiceCollection services)=>
             services.AddScoped<INotificationService, NotificationService>();
        
        public static void ConfigureLoggerService(this IServiceCollection services) =>
                   services.AddScoped<ILoggerManager, LoggerManager>();
        public static void ConfigureRepositoryManager(this IServiceCollection services)=>
            services.AddScoped<IRepositoryManager, RepositoryManager>();
        public static void ConfigureLangaugeService(this IServiceCollection services) =>
           services.AddScoped<ILanguageService, LangaugeService>(); 
        public static void ConfigureSMSService(this IServiceCollection services) =>
           services.AddScoped<ISMSService, SMSService>();
        public static void ConfigurePaymentService(this IServiceCollection services) =>
           services.AddScoped<PaymentService>();
    }
}

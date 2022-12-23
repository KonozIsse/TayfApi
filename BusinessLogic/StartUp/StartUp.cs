using BusinessLogic.ApiClasses
    ;
using BusinessLogic;
using Contracts;
using Entities;
using Entities.Models;
using LoggerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Repository;
using System;
using System.Text;
using EmailService;

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
                         .AllowAnyHeader());
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
                o.Password.RequireDigit = true;
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
        public static void ConfigureSqlContext(this IServiceCollection services,
                  IConfiguration configuration) =>
                   services.AddDbContext<RepositoryContext>(opts =>
                  opts.UseSqlServer(configuration.GetConnectionString("sqlConnection"), b =>
                          b.MigrationsAssembly("WebLayer")));
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<ILoggerManager, LoggerManager>();
            services.AddScoped<IRepositoryManager, RepositoryManager>();
            services.AddScoped<HomeBL>();
            services.AddScoped<ImageBL>();
            services.AddScoped<ProductBL>();
            services.AddScoped<UserBL>();
            services.AddScoped<CartBL>();
            services.AddScoped<ImageUploadServices>();
            services.AddScoped<OrderBL>();
            services.AddScoped<Util>();
            services.AddScoped<LocationTaxBL>();
            services.AddScoped<NewsBL>();
        } 
        public static void ConfigureEmailConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var emailConfig = configuration.GetSection("EmailSettings").Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            services.AddScoped<IEmailSender, EmailSender>();
        }
    }
}

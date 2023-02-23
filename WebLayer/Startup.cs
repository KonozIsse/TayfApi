
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.OpenApi.Models;
using EmailService;
using Contracts;
using Quartz;
using CorePush.Google;
using CorePush.Apple;
using Microsoft.OpenApi.Any;
using Microsoft.Extensions.Options;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using BusinessLogic;
using BusinessLogic.StartUp;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;

namespace WebLayer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCors();
            services.ConfigureIISIntegration();
            services.AddAuthentication();
            services.ConfigureIdentity();
            services.AddHttpContextAccessor();
            services.ConfigureAuthenticationManagerService(); 
            services.ConfigureBusinessLogic(); 
            services.ConfigureSqlConnection(Configuration);
            services.ConfigureEmailConfiguration(Configuration);
            services.ConfigureFcmNotification(Configuration);
            services.ConfigureJWT(Configuration);
            services.ConfigureAddQuartz();
            services.ConfigureLoggerService();
            services.ConfigureRepositoryManager(); 
            services.ConfigureNotificationService();
            services.ConfigureLocService();
            services.ConfigureLangaugeService();
            services.ConfigureSMSService();
            services.ConfigurePaymentService();
            //------------------------------
     
            services.AddAuthentication().AddFacebook(facebook =>
            {
                var facebookSettings = Configuration.GetSection("FacebookSetting");
                facebook.AppId = facebookSettings.GetSection("facebookAppId").Value;
                facebook.AppSecret = facebookSettings.GetSection("facebookAppSecret").Value;
            });
            services.AddAuthentication().AddGoogle(google =>
            {
                var googleSettings = Configuration.GetSection("GoogleSetting");
                google.ClientId = googleSettings.GetSection("googleClientId").Value;
                google.ClientSecret = googleSettings.GetSection("googleClientSecret").Value;
            });
            //------------------------------
            services.AddControllers().AddJsonOptions(
                options =>
              options.JsonSerializerOptions.Converters.Add(new TimeSpanToStringConverter()));
            services.AddControllers();
            //--------------------------------

            // services.Scan(scan => scan
            //.FromAssemblyOf<BaseClassBL>()
            //.AddClasses(classes => classes.InNamespaces("BusinessLogic"))
            //.AsSelf()
            //.WithTransientLifetime());
            //-------------------------------------
            services.AddHttpClient<FcmSender>();
            services.AddHttpClient<ApnSender>();
            //services.AddAutoMapper(typeof(Startup));
            services.AddSingleton(provider => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile(provider.GetService<IHttpContextAccessor>()));
            }).CreateMapper());
            //-------------------------------------------

            services.AddSwaggerGen(c =>
            {
                c.MapType<TimeSpan>(() => new OpenApiSchema
                {
                    Type = "string",
                    Example = new OpenApiString("00:00")
                });

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TayfApi", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                 {
                    {
                          new OpenApiSecurityScheme
                          {
                            Reference = new OpenApiReference
                              {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                              },
                              Scheme = "oauth2",
                              Name = "Bearer",
                              In = ParameterLocation.Header,

                            },
                        new List<string>()
                    }
                 });
            });
            
            services.Configure<QuartzOptions>(options =>
            {
                options.Scheduling.IgnoreDuplicates = true;
                options.Scheduling.OverWriteExistingData = true;
            });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var lockOption = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(lockOption.Value);
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });
            app.UseResponseCaching();
            //app.UseHttpCacheHeaders();
            //app.UseIpRateLimiting();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller= Home}/{action=Index}/{id?}");
            });
            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "Code Maze API v1");
                s.SwaggerEndpoint("/swagger/v2/swagger.json", "Code Maze API v2");
            });
            //---------------------------------------------------
        }

    }
}
 
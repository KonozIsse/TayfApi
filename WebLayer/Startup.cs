using WebLayer.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
using Entities.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Globalization;
using Microsoft.OpenApi.Any;
using Microsoft.Extensions.Localization;
using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Localization;
using System.Threading;
using ResourcesLib;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Entities.Models.CorePushModels;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using WebLayer.Services;
using WebLayer.Services.Jobs;
using BusinessLogic.ApiClasses;
using BusinessLogic;

namespace WebLayer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCors();
            services.ConfigureIISIntegration();
            services.ConfigureLoggerService();
            services.AddControllers().AddJsonOptions(
                options =>
                options.JsonSerializerOptions.Converters.Add(new TimeSpanToStringConverter())
                );
            services.AddControllers();
            //--------------------------------

            services.AddScoped<NewsBL>();
            services.AddScoped<HomeBL>();
            services.AddScoped<ImageBL>();
            services.AddScoped<ProductBL>();
            services.AddScoped<CartBL>();
            services.AddScoped<OrderBL>();
            services.AddScoped<UserBL>();
            services.AddScoped<LocationTaxBL>();
            services.AddScoped<Util>();

           // services.Scan(scan => scan
           //.FromAssemblyOf<BaseClassBL>()
           //.AddClasses(classes => classes.InNamespaces("BusinessLogic"))
           //.AsSelf()
           //.WithTransientLifetime());
            //-------------------------------------
            services.AddSingleton<LocService>();
            services.AddHttpClient<FcmSender>();
            services.AddHttpClient<ApnSender>();
            services.ConfigureRepositoryManager();
            //services.AddAutoMapper(typeof(Startup));
            services.AddSingleton(provider => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile(provider.GetService<IHttpContextAccessor>()));
            }).CreateMapper());

            services.ConfigureSqlContext(Configuration);
            services.AddAuthentication();
            services.ConfigureIdentity();
            services.AddHttpContextAccessor();
            services.ConfigureAuthenticationManagerService();
            //------------------------------

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
            services.ConfigureJWT(Configuration);

            var emailConfig = Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            services.AddScoped<IEmailSender, EmailSender>();


            var appSettingsSection = Configuration.GetSection("FcmNotification");
            services.Configure<FcmNotificationSetting>(appSettingsSection);

            //////Localization//////////////////

            services.AddLocalization();
            //services.AddLocalization(op => op.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(culture: "ar", uiCulture: "ar");
                options.AddSupportedCultures("ar", "en");
                options.AddSupportedUICultures("ar", "en");
                //-------------------------------------
                options.FallBackToParentCultures = true;

                options.ApplyCurrentCultureToResponseHeaders = true;

            });

            services.AddScoped<INotificationService, NotificationService>();

            services.Configure<QuartzOptions>(options =>
            {
                options.Scheduling.IgnoreDuplicates = true;
                options.Scheduling.OverWriteExistingData = true;
            });

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
                    pattern: "{controller= Home}/{action=Index}/{id?}"
                    );
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
//    public class TimeSpanToStringConverter : JsonConverter<TimeSpan>
//    {
//        private readonly string format = @"h\:mm";
//        public override TimeSpan Read(ref Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
//        {
//            var value = reader.GetString();
//            return TimeSpan.ParseExact(value, format, CultureInfo.InvariantCulture);
//        }

//        public override void Write(Utf8JsonWriter writer, TimeSpan value, System.Text.Json.JsonSerializerOptions options)
//        {
//            writer.WriteStringValue(value.ToString(format));
//        }
//    }
//    public class LocService
//    {
//        private readonly IStringLocalizer _localizer;

//        public LocService(IStringLocalizerFactory factory)
//        {
//            var type = typeof(SharedResource);
//            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
//            _localizer = factory.Create("SharedResource", assemblyName.Name);
//        }

//        public LocalizedString GetLocalizedString(string key)
//        {
//            return _localizer[key];
//        }

//        public string GetLocalizedStringValue(string key)
//        {
//            return _localizer[key].Value;
//        }
//    }
}
 

using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using CorePush.Google;
using CorePush.Apple;
using Microsoft.OpenApi.Any;
using Microsoft.Extensions.Options;
using AutoMapper;
using BusinessLogic;
using BusinessLogic.StartUp;
using EtayfeAdminPanel.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Globalization;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        //----------------------------------------------
        builder.Services.ConfigureEmailConfiguration(builder.Configuration);
        builder.Services.ConfigureCors();
        builder.Services.ConfigureIISIntegration();
        builder.Services.ConfigureAuthenticationManagerService();
        builder.Services.ConfigureIdentity();
        builder.Services.ConfigureBusinessLogic();
        builder.Services.ConfigureLoggerService();
        builder.Services.ConfigureFcmNotification(builder.Configuration);
        builder.Services.ConfigureRepositoryManager();
        builder.Services.ConfigureJWT(builder.Configuration);
        builder.Services.ConfigureNotificationService();
        builder.Services.ConfigureLangaugeService();
        builder.Services.ConfigureSqlConnection(builder.Configuration);
        builder.Services.ConfigureLocService();
        builder.Services.ConfigureAddQuartz();
        builder.Services.ConfigureSMSService();

        //----------------------------------------------
        builder.Services.AddHttpClient<FcmSender>();
        builder.Services.AddHttpClient<ApnSender>();

        builder.Services.AddSingleton(provider => new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile(provider.GetService<IHttpContextAccessor>()));
        }).CreateMapper());
        //----------------------
        //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:44360/") });
        builder.Services.AddHttpClient();

        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<JWTAuthenticationProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider, JWTAuthenticationProvider>(
            provider => provider.GetRequiredService<JWTAuthenticationProvider>());
        builder.Services.AddScoped<ILoginService, JWTAuthenticationProvider>(
            provider => provider.GetRequiredService<JWTAuthenticationProvider>());


        builder.Services.AddControllers().AddJsonOptions(
                     options =>
                   options.JsonSerializerOptions.Converters.Add(new TimeSpanToStringConverter()));

        builder.Services.AddControllers();

        builder.Services.AddSwaggerGen(c =>
        {
            c.MapType<TimeSpan>(() => new OpenApiSchema
            {
                Type = "string",
                Example = new OpenApiString("00:00")
            });

            c.SwaggerDoc("v1", new OpenApiInfo { Title = "TayfCPApi", Version = "v1" });
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

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        //app.Use(async (context, next) =>
        //{
        //    if (context.Request.Query.Count() > 0 &&
        //    context.Request.Query["culture"].ToString() != "")
        //    {
        //        System.Threading.Thread.CurrentThread.CurrentCulture =
        //         System.Threading.Thread.CurrentThread.CurrentUICulture
        //        = new CultureInfo(context.Request.Query["culture"].ToString());
        //        //save cuurrent culture in cookie
        //        context.Response.Cookies.Append(
        //            CookieRequestCultureProvider.DefaultCookieName,
        //            CookieRequestCultureProvider.MakeCookieValue
        //            (new RequestCulture(context.Request.Query["culture"].ToString()))
        //            , new CookieOptions() { Expires = DateTime.Now.AddYears(1) }
        //            );
        //    }

        //    await next.Invoke();
        //});


        //var jsInterop = app.Services.GetRequiredService<IJSRuntime>();
        //var result = await jsInterop.InvokeAsync<string>("cultureInfo.get");
        //CultureInfo culture;
        //if (result != null)
        //{
        //    culture = new CultureInfo(result);
        //}
        //else
        //{
        //    culture = new CultureInfo("en-US");
        //    await jsInterop.InvokeVoidAsync("cultureInfo.set", "en-US");
        //}
        //CultureInfo.DefaultThreadCurrentCulture = culture;
        //CultureInfo.DefaultThreadCurrentUICulture = culture;


        var options = ((IApplicationBuilder)app).ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(options.Value);

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCors("CorsPolicy");
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });
        app.UseResponseCaching();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        app.UseSwagger();
        app.UseSwaggerUI(s =>
        {
            s.SwaggerEndpoint("/swagger/v1/swagger.json", "Code Maze API v1");
            s.SwaggerEndpoint("/swagger/v2/swagger.json", "Code Maze API v2");
        });
        app.Run();
    }
}
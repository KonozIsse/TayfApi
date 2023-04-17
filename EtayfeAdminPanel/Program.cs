
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
using Blazored.LocalStorage;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

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
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:44360/") }.EnableIntercept(sp));

        builder.Services.AddHttpClientInterceptor();
        builder.Services.AddScoped<HttpInterceptorService>();
        builder.Services.AddHttpClient();
        builder.Services.AddAuthorizationCore();

        builder.Services.AddScoped<JWTAuthenticationProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider, JWTAuthenticationProvider>(
            provider => provider.GetRequiredService<JWTAuthenticationProvider>());
        builder.Services.AddScoped<ILoginService, JWTAuthenticationProvider>(
            provider => provider.GetRequiredService<JWTAuthenticationProvider>());
        builder.Services.AddScoped<RefreshTokenService>();

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

        var options = ((IApplicationBuilder)app).ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();
        app.UseRequestLocalization(options.Value);

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
            Path.Combine(@"C:\Users\a7ed\source\repos\TayfApi\WebLayer\wwwroot\media_files", "avatars")),
            RequestPath = "/avatars"
        });
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
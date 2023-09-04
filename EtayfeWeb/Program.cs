using AutoMapper;
using BusinessLogic;
using BusinessLogic.StartUp;
using CorePush.Apple;
using CorePush.Google;
using EtayfeWeb;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
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

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TayfWeb", Version = "v1" });
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
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(@"C:\Users\a7ed\source\repos\TayfApi\EtayfeAdminPanel\wwwroot", "media_files")),
    RequestPath = "/media_files"
});
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Test}/{action=Index}/{id?}");
app.UseSwagger();
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Code Maze API v1");
    s.SwaggerEndpoint("/swagger/v2/swagger.json", "Code Maze API v2");
});
app.Run();

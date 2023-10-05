using Capstone.API.Extentions;
using Capstone.Common.Jwt;
using Capstone.DataAccess;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.LoggerService;
using Capstone.Service.UserService;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

static async Task InitializeDatabase(IApplicationBuilder app)
{
    using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
    if (scope != null)
    {
        await scope.ServiceProvider.GetRequiredService<CapstoneContext>().Database.MigrateAsync();
    }
}

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddDbContext<CapstoneContext>(opt =>
{
    opt.UseSqlServer(configuration.GetConnectionString("DBConnString"));
});
// Add services to the container.

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSingleton<ILoggerManager, LoggerManager>();

builder.Services.AddControllers()
                .AddFluentValidation(options =>
                {
                    // Validate child properties and root collection elements
                    options.ImplicitlyValidateChildProperties = true;
                    options.ImplicitlyValidateRootCollectionElements = true;

                    // Automatic registration of validators in assembly
                    options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                });

builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
    build.WithOrigins("http://localhost:port").AllowAnyMethod().AllowAnyHeader();
}));

//add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
           options =>
           {
               options.RequireHttpsMetadata = false;
               options.SaveToken = true;
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidIssuer = JwtConstant.Issuer,
                   ValidAudience = JwtConstant.Audience,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConstant.Key)),
               };
           }
       );

builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseCors("corspolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

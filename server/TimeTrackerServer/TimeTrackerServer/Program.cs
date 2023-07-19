using System.Text;
using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using TimeTrackerServer.Models;
using TimeTrackerServer.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TimeTrackerServer.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TimeTrackerServerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TimeTrackerServerContext") ?? throw new InvalidOperationException("Connection string 'TimeTrackerServerContext' not found.")));
BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

var mongoDbSettings = builder.Configuration.GetSection("TimeTrackerDatabase").Get<TimeTrackerDatabaseSettings>();

// Add services to the container.
var mongoIdentityConfiguration = new MongoDbIdentityConfiguration
{
    MongoDbSettings = new MongoDbSettings
    {
        ConnectionString = mongoDbSettings?.ConnectionString,
        DatabaseName = mongoDbSettings?.DatabaseName
    },
    IdentityOptionsAction = options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        // Lockout settings.
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;

        options.User.RequireUniqueEmail = true;
        
    }
};

builder.Services.AddScoped<PackagesService>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<CycleService>();


builder.Services.AddSingleton<Cycle>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRole, Guid>(mongoIdentityConfiguration)
    .AddUserManager<UserManager<ApplicationUser>>()
    .AddSignInManager<SignInManager<ApplicationUser>>()
    .AddRoleManager<RoleManager<ApplicationRole>>()
    .AddDefaultTokenProviders();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:3000") // Replace with your frontend domain
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});



builder.Services.AddControllers();
builder.Services.Configure<TimeTrackerDatabaseSettings>(
    builder.Configuration.GetSection("TimeTrackerDatabase"));
var app = builder.Build();// 
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.MapControllers();
app.MapGet("/", () => "Hello World!");

app.Run();

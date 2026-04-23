using System.Text;
using BusBookingAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<BusBookingAPI.Services.IAuthService, BusBookingAPI.Services.AuthService>();
builder.Services.AddScoped<BusBookingAPI.Services.IAdminService, BusBookingAPI.Services.AdminService>();
builder.Services.AddScoped<BusBookingAPI.Services.IOperatorService, BusBookingAPI.Services.OperatorService>();
builder.Services.AddScoped<BusBookingAPI.Services.IScheduleService, BusBookingAPI.Services.ScheduleService>();
builder.Services.AddScoped<BusBookingAPI.Services.IBookingService, BusBookingAPI.Services.BookingService>();
builder.Services.AddScoped<BusBookingAPI.Services.IBusService, BusBookingAPI.Services.BusService>();
builder.Services.AddHostedService<BusBookingAPI.Services.SeatHoldCleanupService>();

// Database Configuration
builder.Services.AddDbContext<BusBookingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication & JWT Configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyForJWTAuthentication123456";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "BusBookingAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "BusBookingUsers";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; 
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        NameClaimType = System.Security.Claims.ClaimTypes.Email,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication failed: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            Console.WriteLine("Incoming request to: " + context.Request.Path);
            return Task.CompletedTask;
        }
    };
});

// Swagger/OpenAPI Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BusBookingAPI", Version = "v1" });
    
    // Add JWT Authentication support to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Standard Swagger XML Documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    var authHeader = context.Request.Headers["Authorization"].ToString();
    if (!string.IsNullOrEmpty(authHeader))
    {
        Console.WriteLine($"[DEBUG] Auth Header found: {authHeader.Substring(0, Math.Min(authHeader.Length, 20))}...");
    }
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BusBookingAPI v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

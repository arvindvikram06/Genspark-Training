using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using PolicyAuthDemo.Authorization.Handlers;
using PolicyAuthDemo.Authorization.Requirements;

var builder = WebApplication.CreateBuilder(args);

// ═══════════════════════════════════════════════════════════════════════════
// JWT SECRET
// ═══════════════════════════════════════════════════════════════════════════

var jwtSecret = "super-secret-demo-key-at-least-32-chars!!";

// ═══════════════════════════════════════════════════════════════════════════
// AUTHENTICATION
// ═══════════════════════════════════════════════════════════════════════════

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

// ═══════════════════════════════════════════════════════════════════════════
// AUTHORIZATION
// ═══════════════════════════════════════════════════════════════════════════

builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, DepartmentHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, BadgeEntryHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, TemporaryStickerHandler>();

builder.Services.AddAuthorization(options =>
{
    // POLICY 1
    options.AddPolicy("AtLeast18", policy =>
        policy.Requirements.Add(new MinimumAgeRequirement(18)));

    // POLICY 2
    options.AddPolicy("SeniorEngineer", policy =>
    {
        policy.Requirements.Add(new MinimumAgeRequirement(30));
        policy.Requirements.Add(new DepartmentRequirement("Engineering"));
    });

    // POLICY 3
    options.AddPolicy("CanEnterBuilding", policy =>
        policy.Requirements.Add(new BuildingEntryRequirement()));

    // POLICY 4
    options.AddPolicy("HRDepartment", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                c.Type == "Department" &&
                c.Value == "HR")));
});

// ═══════════════════════════════════════════════════════════════════════════
// CONTROLLERS
// ═══════════════════════════════════════════════════════════════════════════

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// ═══════════════════════════════════════════════════════════════════════════
// SWAGGER + JWT AUTH CONFIGURATION
// ═══════════════════════════════════════════════════════════════════════════

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Policy Authorization Demo API",
        Version = "v1",
        Description = "ASP.NET Core Policy-Based Authorization Demo"
    });

    // JWT AUTH SCHEME
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description =
            "Enter JWT token like this: Bearer your_token"
    });

    // APPLY JWT GLOBALLY IN SWAGGER
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
});

var app = builder.Build();

// ═══════════════════════════════════════════════════════════════════════════
// SWAGGER MIDDLEWARE
// ═══════════════════════════════════════════════════════════════════════════

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "PolicyAuthDemo v1");
    options.RoutePrefix = string.Empty;
});

// ═══════════════════════════════════════════════════════════════════════════
// MIDDLEWARE ORDER
// ═══════════════════════════════════════════════════════════════════════════

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// ═══════════════════════════════════════════════════════════════════════════
// TOKEN GENERATOR
// ═══════════════════════════════════════════════════════════════════════════

app.MapPost("/token", (TokenRequest req) =>
{
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, req.Name)
    };

    if (req.Dob is not null)
        claims.Add(new Claim(ClaimTypes.DateOfBirth, req.Dob));

    if (req.Department is not null)
        claims.Add(new Claim("Department", req.Department));

    if (req.BadgeId is not null)
        claims.Add(new Claim("BadgeId", req.BadgeId));

    if (req.TemporaryBadge is not null)
        claims.Add(new Claim("TemporaryBadge", req.TemporaryBadge));

    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(jwtSecret));

    var creds = new SigningCredentials(
        key,
        SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: creds);

    var tokenString =
        new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(new
    {
        token = tokenString,
        claimsIncluded = claims.Select(c => new
        {
            c.Type,
            c.Value
        }),
        usage = "Authorization: Bearer <token>"
    });
});

app.Run();

// ═══════════════════════════════════════════════════════════════════════════
// DTO
// ═══════════════════════════════════════════════════════════════════════════

record TokenRequest(
    string Name,
    string? Dob,
    string? Department,
    string? BadgeId,
    string? TemporaryBadge
);
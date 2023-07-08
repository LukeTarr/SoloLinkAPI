using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SoloLinkAPI.Services;
using SoloLinkDataAccess;
using Swashbuckle.AspNetCore.Filters;

// Create a Web API and basic services
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Swagger with ability to use a token for authorized routes
builder.Services.AddSwaggerGen(opts =>
{
    opts.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "JSON Web Token in the Bearer scheme, given when logging in.",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    opts.OperationFilter<SecurityRequirementsOperationFilter>();
});

// Add the
builder.Services.AddCors(p =>
    p.AddPolicy("sololink", builder => { builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader(); }));

// Add the database context from the Data Access Class Library, connect with connection string, and set this as the assembly to do migrations
builder.Services.AddDbContext<SoloLinkDatabaseContext>();


// Add the authentication service and configure to check signingkey with the one stored in appsettings
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("POSTGRES_DATABASE")!)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddHttpContextAccessor();

// Register custom services into dependency injection context
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ILinkService, LinkService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IPageViewService, PageViewService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c => { c.DisplayRequestDuration(); });

// Configure middleware and run the server
app.UseCors("sololink");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
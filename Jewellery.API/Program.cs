using MediatR;
using Jewellery.Application.Master.Commands;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Infrastructure.Master.Repositories;
using Microsoft.Data.SqlClient;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Jewellery.Application.Auth.Interfaces;
using Jewellery.API.Filters;
using System.Data;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Infrastructure.Transactions.Repositories;
using Jewellery.Application.Common.Interfaces;
using Jewellery.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ------------------ JWT Token Service ------------------
builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// ------------------ Controllers & Swagger ------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Jewellery API", Version = "v1" });

    // 🔐 JWT Bearer Authorize button
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your JWT token"
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
            new string[] {}
        }
    });
});

// ------------------ ✅ CORS (IMPORTANT) ------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Next.js frontend
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// ------------------ MediatR ------------------
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(MetalMaster_ManageCommand).Assembly));

// ------------------ SQL Connection ------------------
//builder.Services.AddScoped<SqlConnection>(sp =>
//{
//    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
//    return new SqlConnection(cs);
//});
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    return new SqlConnection(cs);
});

// ------------------Start Repository DI ------------------

//--------------------DB----------------------------
builder.Services.AddScoped<IDAL, DAL>();
builder.Services.AddScoped<IErrorLogRepository, ErrorLogRepository>();

//--------------------Auth----------------------------
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<ISignUpRepository, SignUpRepository>();
builder.Services.AddScoped<IGetMenuRepository, GetMenuRepository>();

//--------------------Master-------------------------
builder.Services.AddScoped<IMetalRepository, MetalRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

//--------------------Transactions-------------------------
builder.Services.AddScoped<IStockRepository, StockRepository>();
// ------------------END Repository DI ------------------


// ------------------ Exception Filter ------------------
builder.Services.AddScoped<ExceptionFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
});


// ------------------ JWT Authentication ------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

// ------------------ Build App ------------------
var app = builder.Build();

// ------------------ Middleware ------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Jewellery API V1");
    });
}

app.UseHttpsRedirection();

// ✅ CORS MUST be before Auth
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

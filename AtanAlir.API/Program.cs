using AtanAlir.Core.Interfaces;
using AtanAlir.Infrastructure.Data;
using AtanAlir.Infrastructure.External;
using AtanAlir.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using AspNetCoreRateLimit;
using AtanAlir.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<TokenService>();


builder.Services.AddOptions();


builder.Services.AddMemoryCache();


builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.GeneralRules = new List<RateLimitRule>
    {
        
        new RateLimitRule
        {
            Endpoint = "*",
            Period = "1m",   // 1 dakika
            Limit = 30       // 30 istek
        },
        
        new RateLimitRule
        {
            Endpoint = "*/auth/*",
            Period = "5m",   // 5 dakika
            Limit = 5        // 5 istek
        },
        
        new RateLimitRule
        {
            Endpoint = "*/chatHub/*",
            Period = "10s",  // 10 saniye
            Limit = 20       // 20 mesaj
        }
    };
});

// Rate limit servisleri
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

builder.Services.AddScoped<EmailService>();

//// CORS Policy
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowFrontend", builder =>
//    {
//        builder.WithOrigins("http://localhost:5173")
//               .AllowAnyMethod()
//               .AllowAnyHeader()
//               .AllowCredentials() // SignalR i�in �nemli
//               .SetIsOriginAllowed(_ => true); // Development i�in
//    });
//});
// CORS Policy
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", builder =>
        {
            builder.WithOrigins("http://localhost:5173")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials() // SignalR i�in
                   .SetIsOriginAllowed(_ => true); // Development i�in
        });
    });
}
else
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", builder =>
        {
            builder.WithOrigins(
                
                   )
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials() // SignalR
                   .SetIsOriginAllowed(origin =>
                   {
                       // Production'da izin verilen domainler
                       var allowedOrigins = new[]
                       {
                        "https://example.com",
                       };
                       return allowedOrigins.Contains(origin);
                   });
        });
    });
}

builder.Services.AddSignalR();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AtanAlir API", Version = "v1" });

   
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
            new string[] {}
        }
    });
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };

        // SignalR icin JWT ayarlari
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient<FootballDataApiClient>();



var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "");
        c.RoutePrefix = "swagger";
    });
}

app.UseIpRateLimiting();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication(); // Authentication middleware
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Ok(new { message = " is running", status = "Active", version = "1.0.0" }));
app.MapHub<ChatHub>("/chatHub");

app.Run();
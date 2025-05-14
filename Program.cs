using Calyx_Solutions.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using Microsoft.Extensions.Http;
using Calyx_Solutions.Middlewares;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Net.Http;
using Serilog;




using MySqlConnector;
using System.Security.Cryptography.X509Certificates;

using System.Text.Json;

using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Calyx_Solutions;
using Microsoft.Extensions.Options;
using Google.Protobuf.WellKnownTypes;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";



var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Optional: Use only if required
    });
});

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Jwt configuration starts here
var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:SecurityKey").Get<string>();

/*
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });*/

 bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken tokenToValidate, TokenValidationParameters @param)
{
    if (expires != null)
    {
        return expires > DateTime.UtcNow;      
    }
    else
    {
        Console.WriteLine("Token expired...");
    }
    return false;
}

 void ConfigureServices(IServiceCollection services)
{
    
    services.AddDistributedMemoryCache();
    services.AddRazorPages();
    services.AddSession(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.IdleTimeout = TimeSpan.FromSeconds(10);
    });

    

    services.Configure<HttpClientFactoryOptions>(options =>
    {
        options.HttpClientActions.Add(client =>
            client.DefaultRequestHeaders.ConnectionClose = false);
    });

    // Set TLS version
    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

    //services.AddTransient<IConfiguration, MyRepository>(); // Register your repository
    services.AddTransient<UserLoginController>();

    services.AddMvc(option => option.EnableEndpointRouting = false);
}



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecurityKey"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        LifetimeValidator =  CustomLifetimeValidator ,
        ValidateIssuerSigningKey = true 
    };

    o.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            // Call this to skip the default logic and avoid using the default response
            context.HandleResponse();

            // Write to the response in any way you wish
            context.Response.StatusCode = 401;
            //context.Response.Headers.Append("my-custom-header", "custom-value");
            var jsonData = new { response = false, responseCode = "04", data = "Invalid token." };
            string jsonData1 = JsonConvert.SerializeObject(jsonData);
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(jsonData1);
        }
    };

    // Code for Token Expire
    o.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var jti = context.Principal.FindFirstValue(JwtRegisteredClaimNames.Jti);
            var blacklistService = context.HttpContext.RequestServices.GetRequiredService<ITokenBlacklistService>();

            if (blacklistService.IsTokenBlacklisted(jti))
            {
                context.Fail("Token is expired or revoked.");                
            }

            return Task.CompletedTask;
        }
    };


});
builder.Services.AddAuthorization();
// Add configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddSingleton<ITokenBlacklistService, InMemoryTokenBlacklistService>(); // New code

// Jwt configuration ends here



builder.Services.AddSwaggerGen(opt =>
{
    opt.OperationFilter<CustomSwaggerOperationFilter>();
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Calyx Solutions", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

/*
builder.Services.AddCors( options =>
{
    options.AddDefaultPolicy(
      policy =>
      {
          policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
      });
});*/


// This method gets called by the runtime. Use this method to add services to the container.


builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddLogging();
builder.Services.Configure<LoggingOptions>(builder.Configuration.GetSection("LoggingOptions"));

bool isLoggingEnabled = builder.Configuration.GetValue<bool>("LoggingOptions:EnableLogging");


//builder.Host.UseSerilog();

LogManager.LoadConfiguration("nlog.config");
builder.Logging.ClearProviders(); // Remove default logging providers
builder.Logging.AddNLog(); // Add



var app = builder.Build();

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError("An unhandled exception occurred.");

        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An error occurred.");
    });
});

/*
app.Use(async (context, next) =>
{
    Console.WriteLine("Before IPWhitelistMiddleware");
    await next();
    Console.WriteLine("After IPWhitelistMiddleware");
});
app.UseMiddleware<IPWhitelistMiddleware>(); */

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseCors(MyAllowSpecificOrigins);
app.UseHttpsRedirection();



app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

/*app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();*/

IConfiguration configuration = app.Configuration;
IWebHostEnvironment environment = app.Environment;

app.MapControllers();



app.Run();


// Ip Whitelisting function
public class IPWhitelistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public IPWhitelistMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        var authorizedIPAddresses = _configuration.GetSection("IPAddressWhitelistConfiguration:AuthorizedIPAddresses").Get<List<string>>();
        var clientIp = context.Connection.RemoteIpAddress;
        Console.WriteLine($"Client IP: {clientIp}");

        if (!IsIpAddressAuthorized(clientIp, authorizedIPAddresses))
        {
            context.Response.StatusCode = 403; // Forbidden
            await context.Response.WriteAsync("HTTP Status 403 - Access Forbidden for IP: " + clientIp);
            return;
        }

        await _next(context);
    }

    private bool IsIpAddressAuthorized(IPAddress ipAddress, List<string> authorizedIPAddresses)
    {
        bool chk = false;
        try
        {
            chk = authorizedIPAddresses.Any(allowedIp => IPAddress.Parse(allowedIp).Equals(ipAddress));
            //return authorizedIPAddresses.Any(allowedIp => IPAddress.Parse(allowedIp).Equals(ipAddress));
        }
        catch (Exception ex)
        {
            return false;
        }
        return chk;
    }
}

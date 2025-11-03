using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using QuestPDF.Infrastructure;
using Simulator.Server.Databases.Entities.Identity;
using Simulator.Server.Implementations.Databases;
using Simulator.Server.Implementations.Identities;
using Simulator.Server.Implementations.Storage;
using Simulator.Server.Interfaces.Database;
using Simulator.Server.Interfaces.Identity;
using Simulator.Server.Interfaces.Storage;
using Simulator.Server.Services;
using Simulator.Shared.Constants.Application;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
namespace Simulator.Server.RegisterServices
{
    public static class RegisterServices
    {

        public static WebApplicationBuilder AddServerServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndPoints();
            builder.Services.AddForwarding(builder.Configuration);
            QuestPDF.Settings.License = LicenseType.Community;
            builder.Services.AddCurrentUserService();

            builder.Services.AddDatabase(builder.Configuration);

            builder.Services.AddIdentity();

            builder.Services.AddJwtAuthentication(builder.Services.GetApplicationSettings(builder.Configuration));

           
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddRepositories();
       

            builder.Services.AddControllers();

            builder.Services.AddRazorPages();
            builder.Services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });

            builder.Services.AddTransient<IDatabaseSeeder, DatabaseSeeder>();
            builder.Services.AddLazyCache();
            builder.Services.AddSingleton<ICache, Cache>();
            return builder;
        }
        public static IServiceCollection AddEndPoints(this IServiceCollection service)
        {
            service.AddEndPoints(Assembly.GetExecutingAssembly());
            return service;
        }
        static IServiceCollection AddEndPoints(this IServiceCollection service, Assembly assembly)
        {
            ServiceDescriptor[] serviceDescriptors = assembly
                .DefinedTypes
                .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                type.IsAssignableTo(typeof(IEndPoint)))
                .Select(type => ServiceDescriptor.Transient(typeof(IEndPoint), type)).ToArray();

            service.TryAddEnumerable(serviceDescriptors);
            return service;
        }
        internal static IServiceCollection AddForwarding(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
            var config = applicationSettingsConfiguration.Get<AppConfiguration>();
            if (config!.BehindSSLProxy)
            {
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                    if (!string.IsNullOrWhiteSpace(config.ProxyIP))
                    {
                        var ipCheck = config.ProxyIP;
                        if (IPAddress.TryParse(ipCheck, out var proxyIP))
                            options.KnownProxies.Add(proxyIP);

                    }
                });

                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            builder
                                .AllowCredentials()
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .WithOrigins(config.ApplicationUrl.TrimEnd('/'));
                        });
                });
            }

            return services;
        }
        internal static IServiceCollection AddCurrentUserService(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
           
            return services;
        }
        internal static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Default");
            services.AddSingleton<SoftDeleteInterceptor>();
            services.AddDbContext<BlazorHeroContext>(
                    (sp, options) => options
                        .UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null); // Optional: add specific error numbers to retry
                        })
                        //.EnableSensitiveDataLogging()
                    .AddInterceptors(
                        sp.GetRequiredService<SoftDeleteInterceptor>()));



            services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<BlazorHeroContext>());


            return services;
        }
        internal static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services
                .AddIdentity<BlazorHeroUser, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<BlazorHeroContext>()
                .AddDefaultTokenProviders();

            return services;
        }
        internal static AppConfiguration GetApplicationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
            services.Configure<AppConfiguration>(applicationSettingsConfiguration);

            return applicationSettingsConfiguration.Get<AppConfiguration>()!;
        }
        internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services, AppConfiguration config)
        {
            var key = Encoding.UTF8.GetBytes(config.Secret);
            services
                .AddAuthentication(authentication =>
                {
                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(bearer =>
                {
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RoleClaimType = ClaimTypes.Role,
                        ClockSkew = TimeSpan.Zero
                    };



                    bearer.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments("/signalRHub"))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = c =>
                        {
                            if (c.Exception is SecurityTokenExpiredException)
                            {
                                c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                c.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(Result.Fail("The Token is expired."));
                                return c.Response.WriteAsync(result);
                            }
                            else
                            {
#if DEBUG
                                c.NoResult();
                                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                c.Response.ContentType = "text/plain";
                                return c.Response.WriteAsync(c.Exception.ToString());
#else
                                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                c.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(Result.Fail("An unhandled error has occurred."));
                                return c.Response.WriteAsync(result);
#endif
                            }
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(Result.Fail("You are not Authorized."));
                                return context.Response.WriteAsync(result);
                            }

                            return Task.CompletedTask;
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(Result.Fail("You are not authorized to access this resource."));
                            return context.Response.WriteAsync(result);
                        },
                    };
                });
            services.AddAuthorization(options =>
            {

            });
            return services;
        }

        internal static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailConfiguration>(configuration.GetSection("MailConfiguration"));

            services.AddTransient<IRoleClaimService, RoleClaimService>();
            services.AddTransient<ITokenService, IdentityService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IUserService, UserService>();

            services.AddTransient<IUploadService, UploadService>();
            //services.AddTransient<IMailService, SMTPMailService>();
            services.AddScoped<IExcelService, ExcelService>();
            return services;
        }
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IServerCrudService, ServerCrudService>();
            //services.AddScoped<IQueryRepository, QueryRepository>();
            //services.AddScoped<IRepository, Repository>();
            //services.AddScoped<IIgnoreQueryFilterRepository, IgnoreQueryFilterRepository>();
            return services;
        }
    }
}

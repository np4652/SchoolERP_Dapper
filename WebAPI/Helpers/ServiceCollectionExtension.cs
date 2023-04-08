using Data;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Services.Identity;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Service.Identity;
using FluentMigrator.Runner;
using Infrastructure.Interface;
using Service.CartWishList;
using Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

using Service.PaymentGateway;

using Service.Notify;


namespace WebAPI.Helpers
{
    public static class ServiceCollectionExtension
    {
        public const string corsPolicy = "AllowSpecificOrigin";
        public static void RegisterService(this IServiceCollection services, IConfiguration configuration)
        {
            string dbConnectionString = configuration.GetConnectionString("SqlConnection");
            IConnectionString ch = new ConnectionString { connectionString = dbConnectionString };
            services.AddSingleton<IViewRenderService, ViewRenderService>();
            services.AddScoped<IEmailConfiguration, EmailConfiguration>();
            services.AddSingleton<IConnectionString>(ch);
            services.AddScoped<IDapperRepository, DapperRepository>();
            services.AddScoped<ApplicationDbContext>();
            services.AddScoped<IUserStore<ApplicationUser>, UserStore>();
            services.AddScoped<IRoleStore<ApplicationRole>, RoleStore>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPGCallback, PGCallbackService>();
            services.AddScoped<ICustomeLogger, CustomeLogger>();
            services.AddTransient(typeof(ICustomeLogger<>), typeof(CustomeLogger<>));
            services.AddSingleton<ITokenService, TokenService>();
            services.AddScoped<Data.Migrations.Database>();            
            services.AddScoped<INotifyService, NotifyService>();
            services.AddScoped<IRequestInfo, RequestInfo>();            
            services.AddScoped<IPGDetailService, PGDetailService>();            
            services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();            
            services.AddAutoMapper(typeof(Program));
            services.AddLogging(c => c.AddFluentMigratorConsole())
                .AddFluentMigratorCore()
                .ConfigureRunner(c => c.AddSqlServer2016()
                .WithGlobalConnectionString(configuration.GetConnectionString("SqlConnection"))
                .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations());
            
            JWTConfig jwtConfig = new JWTConfig();
            configuration.GetSection("JWT").Bind(jwtConfig);
            services.AddSingleton(jwtConfig);
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1.1",
                    Title = "API Documentation(v1.1)"
                });
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ApiDoc.xml");
                option.IncludeXmlComments(filePath);
                //option.OperationFilter<AddRequiredHeaderParameter>();
                option.UseAllOfToExtendReferenceSchemas();
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Standard authorization header using the bearer scheme(\"Bearer {token}\")",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                //option.OperationFilter<SecurityRequirementsOperationFilter>();
                option.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            /* End Jwd */
            services.AddControllersWithViews();
            services.AddHsts(option =>
            {
                option = new Microsoft.AspNetCore.HttpsPolicy.HstsOptions
                {
                    IncludeSubDomains = true,
                    Preload = true,
                    MaxAge = TimeSpan.FromDays(7),
                };
            });
            #region Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.AllowedForNewUsers = false;
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.User.RequireUniqueEmail = false;
            }).AddUserStore<UserStore>()
            .AddRoleStore<RoleStore>()
            .AddUserManager<ApplicationUserManager>()
            .AddDefaultTokenProviders();
            services.AddAuthentication(option =>
            {
                option = new Microsoft.AspNetCore.Authentication.AuthenticationOptions
                {
                    DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme,
                    DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme,
                    DefaultScheme = JwtBearerDefaults.AuthenticationScheme
                };
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secretkey"]))
                };
            });
            #endregion
            services.Configure<JWTConfig>(configuration.GetSection("JWT"));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            services.AddCors(options =>
            {
                options.AddPolicy(corsPolicy,
                    builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });
        }
    }
}
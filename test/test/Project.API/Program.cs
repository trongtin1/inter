using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml;
using test.Project.Application.ServiceInterfaces;
using test.Project.Application.Services;
using test.Project.Infrastructure.Data;
using test.Project.Infrastructure.Repositories;
using test.Project.Domain.RepoInterfaces;
using test.Project.API.Hubs;
using test.Project.Domain.Common;
using test.Project.Infrastructure.Common;
using test.Project.Infrastructure.Extensions;
namespace test.Project.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            // Add DbContext
            builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var defaultConnection = configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(defaultConnection);
            });

            // Đăng ký một factory method để tạo DbContext với connection string tùy chọn
            builder.Services.AddScoped<Func<string, ApplicationDbContext>>(provider => (connectionStringName) =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString(connectionStringName);
                optionsBuilder.UseSqlServer(connectionString);
                return new ApplicationDbContext(optionsBuilder.Options, connectionStringName);
            });

            // Cập nhật cách đọc configuration
            builder.Configuration
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Project.API"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            // JWT configuration
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
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration"))
                        )
                    };
                });

            // Thêm cấu hình ClaimTypes tùy chỉnh
            builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.MapInboundClaims = false; // Tắt việc map claims mặc định
            });

            // Định nghĩa Identity options
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.EmailClaimType = "email";
                options.ClaimsIdentity.UserNameClaimType = "username";
                options.ClaimsIdentity.RoleClaimType = "roles";
                
            });

            // Swagger
            builder.Services.AddSwaggerGen( options => 
            {   
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
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
                        new string[] {}
                    }
                });
            });

            builder.Services.AddAuthorization();

            // builder.Services.AddAuthentication("CookieAuth")
            // .AddCookie("CookieAuth", options =>
            // {
            //     options.LoginPath = "/Account/Login"; // Trang login
            //     options.AccessDeniedPath = "/Account/AccessDenied"; // Trang truy cập bị từ chối
            // });

            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            // Add EPPlus license configuration
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Add SignalR services
            builder.Services.AddSignalR();
            //Add Services
            builder.Services.AddScoped<IMailNotificationService, MailNotificationService>();
            builder.Services.AddScoped<IUserModuleNotificationService, UserModuleNotificationService>();
            builder.Services.AddScoped<ISmtpService, SmtpService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IMailService, MailService>();
            builder.Services.AddScoped<IMailExcelService, MailExcelService>();
            builder.Services.AddScoped<IMailStatisticsService, MailStatisticsService>();
            builder.Services.AddScoped<IModuleService, ModuleService>();
            builder.Services.AddScoped<INotiStatisticsService, NotiStatisticsService>();
            builder.Services.AddScoped<IRoleModuleService, RoleModuleService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IUserModuleService, UserModuleService>();
            builder.Services.AddScoped<IUserRoleService, UserRoleService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IPersonalProfileService, PersonalProfileService>();
            builder.Services.AddScoped<IUserOnlineService, UserOnlineService>();
            builder.Services.AddScoped<ITemplateService, TemplateService>();
            //Add Repo
            builder.Services.AddScoped<IMailStatisticsRepository, MailStatisticsRepository>();
            builder.Services.AddScoped<IMailExcelRepository, MailExcelRepository>();
            builder.Services.AddScoped<IMailRepository, MailRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
            builder.Services.AddScoped<INotiStatisticsRepository, NotiStatisticsRepository>();
            builder.Services.AddScoped<IRoleModuleRepository,RoleModuleRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IUserModuleRepository, UserModuleRepository>();
            builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            builder.Services.AddScoped<IUserOnlineRepository, UserOnlineRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IPersonalProfileRepository, PersonalProfileRepository>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IContextService, ContextService>();
            builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            // Set culture cookie
            app.Use(async(context, next) =>
            {
                string cookies = string.Empty;
                if (context.Request.Cookies.TryGetValue("Language", out cookies))
                {
                    Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cookies);
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cookies);
                }
                else
                {
                    Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
                }
                await next.Invoke();
            });
    
            
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = "swagger";
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Add endpoints
            app.MapHub<MailHub>("/mailHub");
            app.MapHub<UserModuleHub>("/userModuleHub");
            app.MapHub<UserOnlineHub>("/userOnlineHub");
            if (app.Environment.IsDevelopment())
            {
                app.Run("http://localhost:5001");
            }
            else
            {
                app.Run();
            }
        }
    }
}

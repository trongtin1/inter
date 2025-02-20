using Microsoft.EntityFrameworkCore;
using test.Services;
using AutoMapper;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using OfficeOpenXml;
using Microsoft.AspNetCore.SignalR;
using test.Hubs;
using test.Attributes;
using test.Services.Hubs;
namespace test
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

            // JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };

                    options.Events = new JwtBearerEvents
                    {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers["Token-Expired"] = "true";
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            var result = JsonSerializer.Serialize(new
                            {
                                status = 401,
                                message = "Token has expired"
                            });
                            return context.Response.WriteAsync(result);
                        }
                        return Task.CompletedTask;
                    }
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
            builder.Services.AddScoped<IMailNotificationService, MailNotificationService>();
            builder.Services.AddScoped<IUserModuleNotificationService, UserModuleNotificationService>();
            builder.Services.AddScoped<IUserOnlineNotificationService, UserOnlineNotificationService>();
            builder.Services.AddScoped<TokenService>();
        
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
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cookies);
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(cookies);
                }
                else
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
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

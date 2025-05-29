using KPISolution.Authorization.Handlers;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using KPISolution.Models.Mappings;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace KPISolution
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/indicator-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.Parse("8.0.0"))
                       .ConfigureWarnings(warnings =>
                            warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                // Tùy chọn đăng nhập
                options.SignIn.RequireConfirmedAccount = true;
                options.SignIn.RequireConfirmedEmail = true;

                // Tùy chọn mật khẩu
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 4;

                // Tùy chọn khóa tài khoản
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Tùy chọn người dùng
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            })
                .AddRoles<IndicatorRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Cấu hình xác thực bên ngoài - Google
            builder.Services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    var googleConfig = builder.Configuration.GetSection("Authentication:Google");
                    googleOptions.ClientId = googleConfig["ClientId"] ?? throw new InvalidOperationException("Google ClientId not found in configuration");
                    googleOptions.ClientSecret = googleConfig["ClientSecret"] ?? throw new InvalidOperationException("Google ClientSecret not found in configuration");

                    // Cấu hình scope cơ bản
                    googleOptions.Scope.Add("email");
                    googleOptions.Scope.Add("profile");

                    // Cấu hình callback URL
                    googleOptions.CallbackPath = "/signin-google";

                    // Lưu token để sử dụng trong tương lai nếu cần
                    googleOptions.SaveTokens = true;
                });

            // Đăng ký và cấu hình các chính sách ủy quyền
            builder.Services.AddIndicatorPolicies();

            // Cấu hình cookie
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(14);
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
                options.Cookie.Name = "Indicator.Identity";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            // Cấu hình xác thực hai yếu tố
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Cấu hình token providers
                options.Tokens.EmailConfirmationTokenProvider = "Email";
                options.Tokens.PasswordResetTokenProvider = "Email";

                // Cấu hình password
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                // Cấu hình lockout
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình user
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            // Cấu hình gửi email
            var emailConfig = builder.Configuration.GetSection("EmailSettings");
            var emailSettings = new SmtpSettings
            {
                Server = emailConfig["MailServer"] ?? "localhost",
                Port = int.Parse(emailConfig["MailPort"] ?? "587"),
                SenderEmail = emailConfig["SenderEmail"] ?? "noreply@indicatorapp.com",
                SenderName = emailConfig["SenderName"] ?? "Indicator System",
                Username = emailConfig["UserName"] ?? "user",
                Password = emailConfig["Password"] ?? "password",
                UseSsl = true
            };
            builder.Services.AddSingleton(Options.Create(emailSettings));

            // Use different email sender implementations for development and production
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddTransient<IEmailSender, DevelopmentEmailSender>();
                builder.Services.AddTransient<IEmailSenderExtended, DevelopmentEmailSender>();

                // Log that we're using the development email sender
                Log.Information("Using development email sender - emails will be logged but not sent");
            }
            else
            {
                builder.Services.AddTransient<IEmailSender, EmailSender>();
                builder.Services.AddTransient<IEmailSenderExtended, EmailSender>();
            }

            // Register Repositories and UnitOfWork
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            // Register Authorization Handlers
            builder.Services.AddScoped<IAuthorizationHandler, IndicatorAuthorizationHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, SuccessFactorAuthorizationHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, SuccessFactorResourceAuthorizationHandler>();

            // Add AutoMapper
            builder.Services.AddAutoMapper(typeof(SuccessFactorMappingProfile).Assembly);

            // Add services with detailed error handling in development
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDatabaseDeveloperPageExceptionFilter();
                builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
                builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
            }
            else
            {
                builder.Services.AddControllersWithViews();
                builder.Services.AddRazorPages();
            }

            // Add case-insensitive routing configuration
            builder.Services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = false;
                options.LowercaseQueryStrings = false;
                options.AppendTrailingSlash = false;
                options.ConstraintMap.Add("ignore-case", typeof(IgnoreCaseParameterTransformer));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Development environment configuration
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();

                // Seed data for development only
                using var scope = app.Services.CreateScope();
                var services = scope.ServiceProvider;

                // Ensure the database schema is created
                var context = services.GetRequiredService<ApplicationDbContext>();
                try
                {
                    await context.Database.MigrateAsync();
                    Log.Information("Database schema created or verified");

                    // Seed initial data
                    await SeedData.InitializeAsync(services);
                    Log.Information("Database seeded with initial data");
                }
                catch (Exception ex)
                {
                    Log.Warning($"Migration error: {ex.Message}");
                    Log.Warning("Continuing without migration");
                }
            }
            else
            {
                // Configure production error handling
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

                // Production environment - run migrations
                using var scope = app.Services.CreateScope();
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                try
                {
                    await context.Database.MigrateAsync();
                    Log.Information("Production database schema updated");

                    // Initialize seed data for production
                    await SeedData.InitializeAsync(services);
                    Log.Information("Production seed data initialized");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Production migration error: {Message}", ex.Message);
                    Log.Warning("Continuing with existing database state");
                }
            }

            // Add global error handling middleware
            app.Use(async (context, next) =>
            {
                try
                {
                    // Log every request for debugging
                    Log.Information("Request to path: {Path}, Method: {Method}", context.Request.Path, context.Request.Method);

                    // Special handling for KeyResultIndicator URLs
                    if (context.Request.Path.Value != null &&
                        context.Request.Path.Value.StartsWith("/KeyResultIndicator", StringComparison.OrdinalIgnoreCase))
                    {
                        Log.Information("Intercepted request to {Path}", context.Request.Path);

                        // Extract ID from the URL if it's a Details request
                        if (context.Request.Path.Value.Contains("/Details/"))
                        {
                            var parts = context.Request.Path.Value.Split('/');
                            if (parts.Length >= 4)
                            {
                                var id = parts[3]; // Get the GUID
                                context.Request.Path = $"/Kri/Details/{id}";
                                Log.Information("Rewriting path to {NewPath}", context.Request.Path);
                            }
                        }
                    }

                    // Special logging for homepage to identify redirects
                    if (context.Request.Path.Value == "/" ||
                        context.Request.Path.Value == "/Home" ||
                        context.Request.Path.Value == "/Home/Index")
                    {
                        Log.Information("Homepage access detected: {Path}", context.Request.Path);
                    }

                    await next();

                    // Log response for debugging
                    Log.Information("Response status code: {StatusCode} for path: {Path}",
                        context.Response.StatusCode, context.Request.Path);

                    // Restore the custom 404 handling block
                    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
                    {
                        // Log the 404 error
                        Log.Warning("404 error occurred for path: {Path}", context.Request.Path);

                        // Rewrite path to error handler
                        context.Request.Path = "/Error/404";
                        await next();
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Log.Error(ex, "Unhandled exception occurred for path: {Path}", context.Request.Path);

                    // Re-throw to be handled by the exception handler middleware
                    throw;
                }
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Re-enable the seed data initialization with error handling
            try
            {
                await SeedData.InitializeAsync(app.Services);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error initializing seed data. Application will continue without full seed data.");
            }

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Landing}/{action=Index}/{id?}");

            // Add custom route for SuccessFactor with case-insensitive routing
            app.MapControllerRoute(
                name: "successfactor",
                pattern: "SuccessFactor/{action=Index}/{id?}",
                defaults: new { controller = "SuccessFactor" });

            // Add routes for all indicator types with case-insensitive routing
            app.MapControllerRoute(
                name: "kri",
                pattern: "KeyResultIndicator/{action=Index}/{id?}",
                defaults: new { controller = "KeyResultIndicator" });

            app.MapControllerRoute(
                name: "ri",
                pattern: "ResultIndicator/{action=Index}/{id?}",
                defaults: new { controller = "ResultIndicator" });

            app.MapControllerRoute(
                name: "kpi",
                pattern: "KeyPerformanceIndicator/{action=Index}/{id?}",
                defaults: new { controller = "KeyPerformanceIndicator" });

            app.MapControllerRoute(
                name: "pi",
                pattern: "PerformanceIndicator/{action=Index}/{id?}",
                defaults: new { controller = "PerformanceIndicator" });

            app.MapRazorPages();

            try
            {
                Log.Information("Starting Indicator Management System");

                // Seed the database - run in all environments for Docker
                // Ensure database schema is created via migrations
                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate(); // Use Migrate() instead of EnsureCreated() when migrations exist
                    Log.Information("Database migrations applied");

                    // Get services first
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IndicatorRole>>();

                    // Khởi tạo các vai trò hệ thống
                    await RoleInitializer.InitializeRolesAsync(app.Services);
                    Log.Information("Roles initialized");

                    // If no admin is found, create a default one
                    string adminEmail = builder.Configuration["AdminSettings:Email"] ?? "admin@indicatorapp.com";

                    var adminUser = await userManager.FindByEmailAsync(adminEmail);

                    if (adminUser == null)
                    {
                        adminUser = new ApplicationUser
                        {
                            UserName = adminEmail,
                            Email = adminEmail,
                            EmailConfirmed = true,
                            FirstName = "System",
                            LastName = "Administrator",
                            IsActive = true
                        };

                        var createResult = await userManager.CreateAsync(adminUser, "Admin@123");
                        if (createResult.Succeeded)
                        {
                            await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
                            Log.Information("Default admin user created: {Email}", adminEmail);
                        }
                        else
                        {
                            Log.Error("Failed to create admin user: {Errors}", string.Join(", ", createResult.Errors.Select(e => e.Description)));
                        }
                    }
                }

                // Comment out the seed data initialization to avoid foreign key constraint errors only in development
                if (app.Environment.IsDevelopment())
                {
                    // await SeedData.InitializeAsync(app.Services);
                }

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Indicator Management System terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}

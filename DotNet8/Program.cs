using System.Text.Json.Serialization;
using Calendarium.Data;
using Calendarium.Endpoints;
using Calendarium.Services;
using Calendarium.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Calendarium
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var strConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(strConnectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
                    options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<IContentManager, ContentManager>();
            builder.Services.AddSingleton(TimeProvider.System);
            builder.Services.AddScoped<CalendarService>();
            builder.Services.AddScoped<RequestHeaderLogger>();

            // Minimal API:
            builder.Services.AddProblemDetails();
            builder.Services.AddValidation();
            builder.Services.ConfigureHttpJsonOptions(options =>
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler();
                app.UseHsts();  //  The default HSTS value is 30 days, see https://aka.ms/aspnetcore-hsts.
            }
            app.UseStatusCodePages();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
                app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            else
                app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=StartPage}/{id?}");

            app.MapRazorPages();

            var api = app.MapGroup("/api").RequireAuthorization();
            api.MapCalendarEndpoints();
            api.MapEventEndpoints();
            api.MapNoteEndpoints();

            if (app.Environment.IsDevelopment())
                app.MapOpenApi();

            Database.CreateDbIfNotExists(app);

            app.Run();
        }
    }
}

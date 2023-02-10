#region Copyright © 2023 Patrick Borger - https: //github.com/Knightmore

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// Author: Patrick Borger
// GitHub: https://github.com/Knightmore
// Created: 21.10.2022
// Modified: 19.01.2023

#endregion

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using RoomReservation.Configuration;
using RoomReservation.Data;
using RoomReservation.IdentityPolicy;
using RoomReservation.Services;

namespace RoomReservation;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Host.ConfigureAppConfiguration((hostingContext, config) => { config.AddJsonFile("customsettings.json", false, true); });

        builder.Services.AddTransient<IUserValidator<AppUser>, CustomEmailPolicy>();
        builder.Services.AddDbContext<ApplicationDbContext>(options => { options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")); });
        builder.Services.AddIdentity<AppUser, IdentityRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders()
               .AddErrorDescriber<LocalizedIdentityErrorDescriber>();

        builder.Services.Configure<IdentityOptions>(options =>
                                                    {
                                                        options.User.RequireUniqueEmail        = true;
                                                        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ, ";

                                                        options.SignIn.RequireConfirmedAccount  = bool.Parse(builder.Configuration["AccountSettings:SignIn:RequireConfirmedAccount"]);
                                                        options.Password.RequiredLength         = int.Parse(builder.Configuration["AccountSettings:Passwords:RequiredLength"]);
                                                        options.Password.RequireLowercase       = bool.Parse(builder.Configuration["AccountSettings:Passwords:RequiredLowercase"]);
                                                        options.Password.RequireUppercase       = bool.Parse(builder.Configuration["AccountSettings:Passwords:RequireUppercase"]);
                                                        options.Password.RequireDigit           = bool.Parse(builder.Configuration["AccountSettings:Passwords:RequireDigit"]);
                                                        options.Password.RequireNonAlphanumeric = bool.Parse(builder.Configuration["AccountSettings:Passwords:RequireNonAlphanumeric"]);

                                                        options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(5);
                                                        options.Lockout.MaxFailedAccessAttempts = 3;
                                                        options.Lockout.AllowedForNewUsers      = true;
                                                    });

        builder.Services.ConfigureApplicationCookie(options =>
                                                    {
                                                        options.Cookie.Name        = "RoomReservation";
                                                        options.Cookie.HttpOnly    = true;
                                                        options.ExpireTimeSpan     = TimeSpan.FromDays(30);
                                                        options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;

                                                        options.LoginPath         = "/Account/Login";
                                                        options.AccessDeniedPath  = "/AccessDenied";
                                                        options.SlidingExpiration = true;
                                                    });

        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
        builder.Services.AddControllersWithViews()
               .AddViewLocalization(LanguageViewLocationExpanderFormat.SubFolder)
               .AddDataAnnotationsLocalization();
        builder.Services.Configure<RequestLocalizationOptions>(options =>
                                                               {
                                                                   var supportedCultures = new[] { "en", "de" };
                                                                   options.SetDefaultCulture(supportedCultures[0])
                                                                          .AddSupportedCultures(supportedCultures)
                                                                          .AddSupportedUICultures(supportedCultures);
                                                               });

        builder.Services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(3));

        builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));
        builder.Services.AddTransient<IMailSender, MailSender>();

        builder.Services.Configure<ReservationSettings>(builder.Configuration.GetSection(nameof(ReservationSettings)));

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddHttpContextAccessor();


        WebApplication app = builder.Build();

        var supportedCultures = new[] { "en", "de" };
        RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                                                                                         .AddSupportedCultures(supportedCultures)
                                                                                         .AddSupportedUICultures(supportedCultures);

        app.UseRequestLocalization(localizationOptions);

        using (IServiceScope scope = app.Services.CreateScope())
        {
            var dataContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dataContext.Database.Migrate();
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Shared/Error");

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
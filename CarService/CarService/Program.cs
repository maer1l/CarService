using CarService.Areas.Identity.Data;
using CarService.Data;
using CarService.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;

namespace CarService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            /*create table clients (client_id serial primary key, document_id int check (document_id between 100000 and 999999), name varchar(100), address varchar(250), phone varchar(27), birthdate date);

            create table categories (category_id serial primary key, category_name varchar(100));

            create table products (product_id serial primary key, category_id int, serial_number int check (serial_number between 100000 and 999999), price money, 
            release_year int check (release_year between 1970 and 2026), brand varchar(100), model varchar(100), 
            CONSTRAINT fk_product_category FOREIGN KEY (category_id) REFERENCES categories (category_id));

            create table orders (order_id serial primary key, product_id int, master_id uuid not null,
            client_id int not null, start_date date, end_date date, price money,
            CONSTRAINT fk_orders_product FOREIGN KEY (product_id) REFERENCES products (product_id),
            CONSTRAINT fk_orders_client FOREIGN KEY (client_id) REFERENCES clients (client_id));

            ALTER TABLE clients ADD CONSTRAINT uq_clients_document UNIQUE (document_id);
            ALTER TABLE products ADD CONSTRAINT uq_product_serial UNIQUE (serial_number);
            ALTER TABLE clients ADD CONSTRAINT uq_clients_phone UNIQUE (phone);*/

            // Scaffold-DbContext "Host=localhost;Database=carservice;Username=postgres;Password=2959912Ars" Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir Models -ContextDir Data -Tables clients,categories,products,orders

            builder.Services.AddDbContext<CarserviceContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("ApplicationDBContextConnection"))
            );

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false; // !@#$%_=
                options.Password.RequireUppercase = false; // Верхний регистр
                options.Password.RequireLowercase = false; // Нижний регистр
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddTransient<IEmailSender, EmailSender>();

            // указание адреса страниц login, logout, accessdenied для сайта, нужно для правильной работы [Authorize]
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            builder.Services.AddRazorPages();

            builder.Services.AddMvc().AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
            });

            // builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.MapRazorPages();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}

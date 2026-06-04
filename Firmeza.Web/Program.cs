using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Firmeza.Web.Data;
using Firmeza.Web.Models;

var builder = WebApplication.CreateBuilder(args);

// Conexión a PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration
           .GetConnectionString("DefaultConnection")));

// Identity con roles
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configurar redirección de login
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/AccesoDenegado";
});

builder.Services.AddRazorPages();
builder.Services.AddScoped<Firmeza.Web.Services.PdfService>();
builder.Services.AddScoped<Firmeza.Web.Services.ExcelService>(

var app = builder.Build();

// Seed de roles y admin
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    if (!await roleManager.RoleExistsAsync("Cliente"))
        await roleManager.CreateAsync(new IdentityRole("Cliente"));

    var admin = await userManager.FindByEmailAsync("admin@firmeza.com");
    if (admin == null)
    {
        admin = new ApplicationUser
        {
            UserName = "admin@firmeza.com",
            Email = "admin@firmeza.com",
            NombreCompleto = "Administrador"
        };
        await userManager.CreateAsync(admin, "Admin123!");
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
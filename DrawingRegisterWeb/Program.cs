using DrawingRegisterWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DrawingRegisterWeb.Utilities;
using Microsoft.AspNetCore.Identity.UI.Services;
using DrawingRegisterWeb.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Add Database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Default Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

// Add Database Initializer
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

// Configure app to support email
builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Invoke Initializer
SeedDatabase();

app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();




void SeedDatabase()
{
	using (var scope = app.Services.CreateScope())
	{
		var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
		dbInitializer.Initialize();
	}
}
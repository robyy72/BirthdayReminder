#region Usings
using Common;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebsiteAdmin;
#endregion

var builder = WebApplication.CreateBuilder(args);

#region Services
// Database
builder.Services.AddDbContext<CoreDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Login";
		options.LogoutPath = "/Logout";
		options.AccessDeniedPath = "/Login";
		options.ExpireTimeSpan = TimeSpan.FromHours(8);
		options.SlidingExpiration = true;
	});

builder.Services.AddAuthorization();

// Application Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SupportService>();
builder.Services.AddScoped<HeartbeatService>();
builder.Services.AddScoped<SubscriptionService>();
builder.Services.AddScoped<TextsService>();

// Razor Pages
builder.Services.AddRazorPages();
#endregion

var app = builder.Build();

#region Database Initialization
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
	db.Database.EnsureCreated();
}
#endregion

#region Middleware
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
#endregion

app.Run();

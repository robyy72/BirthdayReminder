#region Usings
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebsiteAdmin;
#endregion

var builder = WebApplication.CreateBuilder(args);

#region Services
// Database
builder.Services.AddDbContext<AdminDbContext>(options =>
	options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
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
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
		};
	});

builder.Services.AddAuthorization();

// HTTP Client
builder.Services.AddHttpClient();

// Application Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SupportService>();
builder.Services.AddScoped<HeartbeatService>();
builder.Services.AddScoped<SubscriptionService>();

// Controllers and Razor Pages
builder.Services.AddControllers();
builder.Services.AddRazorPages();
#endregion

var app = builder.Build();

#region Database Initialization
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AdminDbContext>();
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
app.MapControllers();
app.MapRazorPages()
   .WithStaticAssets();
#endregion

app.Run();

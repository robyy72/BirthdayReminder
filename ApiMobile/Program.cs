#region Usings
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ApiMobile;
#endregion

var builder = WebApplication.CreateBuilder(args);

#region Services
// Database (shared with WebsiteAdmin)
builder.Services.AddDbContext<ApiDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "birthday-reminder.online";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "birthday-reminder-mobile";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = jwtIssuer,
			ValidAudience = jwtAudience,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
		};
	});

builder.Services.AddAuthorization();

// Application Services
builder.Services.AddScoped<HeartbeatService>();
builder.Services.AddScoped<SupportService>();
builder.Services.AddScoped<TokenService>();

// Controllers
builder.Services.AddControllers();
#endregion

var app = builder.Build();

#region Database Initialization
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
	db.Database.EnsureCreated();
}
#endregion

#region Middleware
if (!app.Environment.IsDevelopment())
{
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
#endregion

app.Run();

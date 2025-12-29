#region Usings
using Common;
using Microsoft.EntityFrameworkCore;
#endregion

namespace WebsiteAdmin;

/// <summary>
/// Aim: Service for tracking app user heartbeats.
/// </summary>
public class HeartbeatService
{
	#region Fields
	private readonly AdminDbContext _db;
	#endregion

	#region Constructor
	public HeartbeatService(AdminDbContext db)
	{
		_db = db;
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// Aim: Record heartbeat from mobile app.
	/// Params: dto - heartbeat data.
	/// Return: True if successful.
	/// </summary>
	public async Task<bool> RecordHeartbeatAsync(HeartbeatDto dto)
	{
		var user = await _db.AppUsers.FindAsync(dto.UserId);

		if (user == null)
		{
			user = new AppUser
			{
				UserId = dto.UserId,
				Email = dto.Email,
				PhoneNumber = dto.PhoneNumber,
				PurchaseToken = dto.PurchaseToken,
				Store = dto.Store,
				PreferredChannel = dto.PreferredChannel,
				LastHeartbeat = DateTime.UtcNow
			};
			_db.AppUsers.Add(user);
		}
		else
		{
			// Update user info
			if (!string.IsNullOrEmpty(dto.Email))
			{
				user.Email = dto.Email;
			}
			if (!string.IsNullOrEmpty(dto.PhoneNumber))
			{
				user.PhoneNumber = dto.PhoneNumber;
			}
			if (!string.IsNullOrEmpty(dto.PurchaseToken))
			{
				user.PurchaseToken = dto.PurchaseToken;
			}
			if (dto.Store != Store.Unknown)
			{
				user.Store = dto.Store;
			}
			if (dto.PreferredChannel != PreferredChannel.NotSet)
			{
				user.PreferredChannel = dto.PreferredChannel;
			}

			user.LastHeartbeat = DateTime.UtcNow;
		}

		await _db.SaveChangesAsync();
		return true;
	}

	/// <summary>
	/// Aim: Get all app users.
	/// Return: List of app users.
	/// </summary>
	public async Task<List<AppUser>> GetAllUsersAsync()
	{
		var users = await _db.AppUsers
			.OrderByDescending(u => u.LastHeartbeat)
			.ToListAsync();

		return users;
	}

	/// <summary>
	/// Aim: Get active users (heartbeat within last N days).
	/// Params: days - number of days.
	/// Return: List of active users.
	/// </summary>
	public async Task<List<AppUser>> GetActiveUsersAsync(int days = 7)
	{
		var cutoff = DateTime.UtcNow.AddDays(-days);

		var users = await _db.AppUsers
			.Where(u => u.LastHeartbeat >= cutoff)
			.OrderByDescending(u => u.LastHeartbeat)
			.ToListAsync();

		return users;
	}

	/// <summary>
	/// Aim: Get user statistics.
	/// Return: User statistics object.
	/// </summary>
	public async Task<UserStats> GetUserStatsAsync()
	{
		var now = DateTime.UtcNow;

		var stats = new UserStats
		{
			TotalUsers = await _db.AppUsers.CountAsync(),
			ActiveLast24h = await _db.AppUsers.CountAsync(u => u.LastHeartbeat >= now.AddHours(-24)),
			ActiveLast7d = await _db.AppUsers.CountAsync(u => u.LastHeartbeat >= now.AddDays(-7)),
			ActiveLast30d = await _db.AppUsers.CountAsync(u => u.LastHeartbeat >= now.AddDays(-30)),
			PlusSubscribers = await _db.AppUsers.CountAsync(u => u.Subscription == SubscriptionTier.Plus),
			ProSubscribers = await _db.AppUsers.CountAsync(u => u.Subscription == SubscriptionTier.Pro)
		};

		return stats;
	}

	/// <summary>
	/// Aim: Get user by ID.
	/// Params: userId - user GUID.
	/// Return: AppUser or null.
	/// </summary>
	public async Task<AppUser?> GetUserByIdAsync(Guid userId)
	{
		var user = await _db.AppUsers
			.Include(u => u.SupportTickets)
			.FirstOrDefaultAsync(u => u.UserId == userId);

		return user;
	}
	#endregion
}

/// <summary>
/// Aim: User statistics model.
/// </summary>
public class UserStats
{
	public int TotalUsers { get; set; }
	public int ActiveLast24h { get; set; }
	public int ActiveLast7d { get; set; }
	public int ActiveLast30d { get; set; }
	public int PlusSubscribers { get; set; }
	public int ProSubscribers { get; set; }
}

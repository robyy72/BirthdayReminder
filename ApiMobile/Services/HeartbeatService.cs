#region Usings
using Common;
using Microsoft.EntityFrameworkCore;
#endregion

namespace ApiMobile;

/// <summary>
/// Aim: Service for tracking app user heartbeats.
/// </summary>
public class HeartbeatService
{
	#region Fields
	private readonly CoreDbContext _db;
	#endregion

	#region Constructor
	public HeartbeatService(CoreDbContext db)
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
		var user = await _db.Customers.FindAsync(dto.UserId);

		if (user == null)
		{
			user = new Customer
			{
				Id = dto.UserId,
				Email = dto.Email,
				PhoneNumber = dto.PhoneNumber,
				PurchaseToken = dto.PurchaseToken,
				Store = dto.Store,
				PreferredChannel = dto.PreferredChannel,
				LastHeartbeat = DateTime.UtcNow
			};
			_db.Customers.Add(user);
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
	/// Aim: Get user by ID.
	/// Params: userId - user GUID.
	/// Return: Customer or null.
	/// </summary>
	public async Task<Customer?> GetUserByIdAsync(Guid userId)
	{
		var user = await _db.Customers
			.FirstOrDefaultAsync(u => u.Id == userId);

		return user;
	}
	#endregion
}

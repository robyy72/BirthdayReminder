#region Usings
using Common;
using Microsoft.EntityFrameworkCore;
#endregion

namespace ApiMobile;

/// <summary>
/// Aim: Service for creating support tickets from mobile app.
/// </summary>
public class SupportService
{
	#region Fields
	private readonly CoreDbContext _db;
	#endregion

	#region Constructor
	public SupportService(CoreDbContext db)
	{
		_db = db;
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// Aim: Create a new support ticket from app.
	/// Params: dto - support ticket data.
	/// Return: Created ticket ID.
	/// </summary>
	public async Task<int> CreateTicketAsync(SupportTicketDto dto)
	{
		// Ensure user exists
		var user = await _db.Customers.FindAsync(dto.UserId);
		if (user == null)
		{
			user = new Customer
			{
				Id = dto.UserId,
				Email = dto.Email,
				PhoneNumber = dto.PhoneNumber,
				PurchaseToken = dto.PurchaseToken,
				Store = dto.Store
			};
			_db.Customers.Add(user);
		}
		else
		{
			// Update user info if provided
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
			if (dto.Store != AppStore.Unknown)
			{
				user.Store = dto.Store;
			}
		}

		var ticket = new SupportTicket
		{
			CustomerId = dto.UserId,
			Message = dto.Message,
			Type = dto.Type,
			Source = dto.Source,
			Status = TicketStatus.Created
		};

		_db.SupportTickets.Add(ticket);
		await _db.SaveChangesAsync();

		return ticket.Id;
	}

	/// <summary>
	/// Aim: Get tickets for a specific user.
	/// Params: userId - user GUID.
	/// Return: List of user's tickets.
	/// </summary>
	public async Task<List<SupportTicket>> GetUserTicketsAsync(Guid userId)
	{
		var tickets = await _db.SupportTickets
			.Where(t => t.CustomerId == userId)
			.OrderByDescending(t => t.CreatedAt)
			.ToListAsync();

		return tickets;
	}
	#endregion
}

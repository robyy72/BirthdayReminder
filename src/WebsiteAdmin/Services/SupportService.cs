#region Usings
using Common;
using Microsoft.EntityFrameworkCore;
#endregion

namespace WebsiteAdmin;

/// <summary>
/// Aim: Service for managing support tickets.
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
	public async Task<int> CreateTicketAsync(TicketDto dto)
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
	/// Aim: Get all open tickets.
	/// Return: List of open support tickets.
	/// </summary>
	public async Task<List<SupportTicket>> GetOpenTicketsAsync()
	{
		var tickets = await _db.SupportTickets
			.Include(t => t.customer)
			.Include(t => t.systemUser)
			.Where(t => t.Status != TicketStatus.Successful && t.Status != TicketStatus.Cancelled)
			.OrderByDescending(t => t.CreatedAt)
			.ToListAsync();

		return tickets;
	}

	/// <summary>
	/// Aim: Get all tickets with optional filter.
	/// Params: status - optional status filter.
	/// Return: List of support tickets.
	/// </summary>
	public async Task<List<SupportTicket>> GetTicketsAsync(TicketStatus? status = null)
	{
		var query = _db.SupportTickets
			.Include(t => t.customer)
			.Include(t => t.systemUser)
			.AsQueryable();

		if (status.HasValue)
		{
			query = query.Where(t => t.Status == status.Value);
		}

		var tickets = await query
			.OrderByDescending(t => t.CreatedAt)
			.ToListAsync();

		return tickets;
	}

	/// <summary>
	/// Aim: Get ticket by ID.
	/// Params: id - ticket ID.
	/// Return: Support ticket or null.
	/// </summary>
	public async Task<SupportTicket?> GetTicketByIdAsync(int id)
	{
		var ticket = await _db.SupportTickets
			.Include(t => t.customer)
			.Include(t => t.systemUser)
			.FirstOrDefaultAsync(t => t.Id == id);

		return ticket;
	}

	/// <summary>
	/// Aim: Update ticket status.
	/// Params: id - ticket ID, status - new status, adminNotes - optional notes.
	/// Return: True if successful.
	/// </summary>
	public async Task<bool> UpdateTicketStatusAsync(int id, TicketStatus status, string? adminNotes = null)
	{
		var ticket = await _db.SupportTickets.FindAsync(id);
		if (ticket == null)
		{
			return false;
		}

		ticket.Status = status;
		ticket.UpdatedAt = DateTimeOffset.UtcNow;

		if (!string.IsNullOrEmpty(adminNotes))
		{
			ticket.AdminNotes = adminNotes;
		}

		if (status == TicketStatus.Successful || status == TicketStatus.Cancelled)
		{
			ticket.ClosedAt = DateTimeOffset.UtcNow;
		}

		await _db.SaveChangesAsync();
		return true;
	}

	/// <summary>
	/// Aim: Assign ticket to admin.
	/// Params: ticketId - ticket ID, adminId - admin user ID.
	/// Return: True if successful.
	/// </summary>
	public async Task<bool> AssignTicketAsync(int ticketId, int adminId)
	{
		var ticket = await _db.SupportTickets.FindAsync(ticketId);
		if (ticket == null)
		{
			return false;
		}

		ticket.SystemUserId = adminId;
		ticket.Status = TicketStatus.Assigned;
		ticket.UpdatedAt = DateTimeOffset.UtcNow;

		await _db.SaveChangesAsync();
		return true;
	}

	/// <summary>
	/// Aim: Get ticket statistics.
	/// Return: Dictionary with status counts.
	/// </summary>
	public async Task<Dictionary<TicketStatus, int>> GetTicketStatsAsync()
	{
		var stats = await _db.SupportTickets
			.GroupBy(t => t.Status)
			.Select(g => new { Status = g.Key, Count = g.Count() })
			.ToDictionaryAsync(x => x.Status, x => x.Count);

		return stats;
	}

	/// <summary>
	/// Aim: Get tickets with filter.
	/// Params: filter - ticket filter options.
	/// Return: Filtered list of tickets.
	/// </summary>
	public async Task<List<SupportTicket>> GetFilteredTicketsAsync(TicketFilter filter)
	{
		var query = _db.SupportTickets
			.Include(t => t.customer)
			.Include(t => t.systemUser)
			.Include(t => t.Entries)
			.AsQueryable();

		if (filter.Status.HasValue)
		{
			query = query.Where(t => t.Status == filter.Status.Value);
		}

		if (filter.Type.HasValue)
		{
			query = query.Where(t => t.Type == filter.Type.Value);
		}

		if (!string.IsNullOrEmpty(filter.SearchText))
		{
			query = query.Where(t =>
				t.Message.Contains(filter.SearchText) ||
				(t.customer != null && t.customer.Email != null && t.customer.Email.Contains(filter.SearchText)));
		}

		if (filter.FromDate.HasValue)
		{
			query = query.Where(t => t.CreatedAt >= filter.FromDate.Value);
		}

		if (filter.ToDate.HasValue)
		{
			query = query.Where(t => t.CreatedAt <= filter.ToDate.Value);
		}

		var tickets = await query
			.OrderByDescending(t => t.CreatedAt)
			.ToListAsync();

		return tickets;
	}

	/// <summary>
	/// Aim: Add entry to ticket conversation.
	/// Params: ticketId - ticket ID, message - entry text, isFromCustomer - sender type, adminName - admin name if from admin.
	/// Return: True if successful.
	/// </summary>
	public async Task<bool> AddTicketEntryAsync(int ticketId, string message, bool isFromCustomer, string? adminName = null)
	{
		var ticket = await _db.SupportTickets.FindAsync(ticketId);
		if (ticket == null)
		{
			return false;
		}

		int? systemUserId = null;
		if (!isFromCustomer && !string.IsNullOrEmpty(adminName))
		{
			var admin = await _db.SystemUsers.FirstOrDefaultAsync(u => u.Email == adminName);
			systemUserId = admin?.Id;
		}

		var entry = new SupportTicketEntry
		{
			SupportTicketId = ticketId,
			Message = message,
			IsFromCustomer = isFromCustomer,
			SystemUserId = systemUserId
		};

		_db.SupportTicketEntries.Add(entry);
		ticket.UpdatedAt = DateTimeOffset.UtcNow;

		await _db.SaveChangesAsync();
		return true;
	}
	#endregion
}

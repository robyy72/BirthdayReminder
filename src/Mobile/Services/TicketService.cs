#region Usings
using Common;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Service for managing support tickets with API sync and offline storage.
/// </summary>
public static class TicketService
{
	#region API Sync Methods
	/// <summary>
	/// Aim: Download tickets from API and populate App.SupportEntries.
	/// Return: True if successful, false if offline or failed.
	/// </summary>
	public static async Task<bool> DownloadTicketsAsync()
	{
		if (!MobileService.HasNetworkAccess())
		{
			return false;
		}

		
		var tickets = await ApiService.GetTicketsAsync();

		if (tickets == null)
		{
			return false;
		}

		App.SupportEntries.Clear();
		foreach (var ticket in tickets)
		{
			var support = ConvertTicketItemToSupport(ticket);
			App.SupportEntries.Add(support);
		}

		return true;
	}

	/// <summary>
	/// Aim: Upload a ticket to API or save to pending if offline.
	/// Params: entry (Support) - The ticket to upload.
	/// Return: True if uploaded, false if saved to pending.
	/// </summary>
	public static async Task<bool> UploadTicketAsync(Support entry)
	{
		entry.CreatedAt = DateTime.Now;

		if (!MobileService.HasNetworkAccess())
		{
			AddToPending(entry);
			return false;
		}

		
		var ticketType = ConvertSupportTypeToTicketType((SupportType)entry.Type);
		var message = FormatMessage(entry.Title, entry.Text);

		var ticketId = await ApiService.SendSupportTicketAsync(message, ticketType);

		if (ticketId > 0)
		{
			entry.Id = ticketId;
			App.SupportEntries.Insert(0, entry);
			return true;
		}

		AddToPending(entry);
		return false;
	}

	/// <summary>
	/// Aim: Sync pending tickets to API.
	/// </summary>
	public static async Task SyncPendingAsync()
	{
		if (!MobileService.HasNetworkAccess())
		{
			return;
		}

		var pending = LoadPending();
		if (pending.Count == 0)
		{
			return;
		}

		
		var uploaded = new List<Support>();

		foreach (var entry in pending)
		{
			var ticketType = ConvertSupportTypeToTicketType((SupportType)entry.Type);
			var message = FormatMessage(entry.Title, entry.Text);

			var ticketId = await ApiService.SendSupportTicketAsync(message, ticketType);
			if (ticketId > 0)
			{
				uploaded.Add(entry);
			}
			else
			{
				break;
			}
		}

		if (uploaded.Count > 0)
		{
			pending.RemoveAll(p => uploaded.Any(u => u.CreatedAt == p.CreatedAt && u.Title == p.Title));
			SavePending(pending);
		}
	}
	#endregion

	#region Pending Tickets (NotUploadedTickets)
	/// <summary>
	/// Aim: Load pending tickets from prefs.
	/// Return: List of pending tickets.
	/// </summary>
	private static List<Support> LoadPending()
	{
		var entries = PrefsHelper.GetValue<List<Support>>(MobileConstants.PREFS_NOT_UPLOADED_TICKETS);
		return entries ?? new List<Support>();
	}

	/// <summary>
	/// Aim: Save pending tickets to prefs.
	/// Params: entries - List of pending tickets.
	/// </summary>
	private static void SavePending(List<Support> entries)
	{
		PrefsHelper.SetValue(MobileConstants.PREFS_NOT_UPLOADED_TICKETS, entries);
	}

	/// <summary>
	/// Aim: Add a ticket to pending list.
	/// Params: entry - The ticket to add.
	/// </summary>
	private static void AddToPending(Support entry)
	{
		var pending = LoadPending();
		pending.Add(entry);
		SavePending(pending);
	}

	/// <summary>
	/// Aim: Check if there are pending tickets.
	/// Return: True if there are pending tickets.
	/// </summary>
	public static bool HasPending()
	{
		var pending = LoadPending();
		return pending.Count > 0;
	}

	/// <summary>
	/// Aim: Get count of pending tickets.
	/// Return: Number of pending tickets.
	/// </summary>
	public static int GetPendingCount()
	{
		var pending = LoadPending();
		return pending.Count;
	}
	#endregion

	#region List Access Methods
	/// <summary>
	/// Aim: Get a support entry by id from App.SupportEntries.
	/// Params: id - The id of the support entry
	/// Return: The support entry or null if not found
	/// </summary>
	public static Support? GetById(int id)
	{
		var entry = App.SupportEntries.FirstOrDefault(s => s.Id == id);
		return entry;
	}

	/// <summary>
	/// Aim: Get all support entries filtered by type.
	/// Params: type - The SupportType to filter by
	/// Return: List of support entries matching the type
	/// </summary>
	public static List<Support> GetByType(SupportType type)
	{
		var entries = App.SupportEntries
			.Where(s => s.Type == (int)type)
			.OrderByDescending(s => s.CreatedAt)
			.ToList();
		return entries;
	}

	/// <summary>
	/// Aim: Get all support entries.
	/// Return: List of all support entries ordered by creation date
	/// </summary>
	public static List<Support> GetAll()
	{
		var entries = App.SupportEntries
			.OrderByDescending(s => s.CreatedAt)
			.ToList();
		return entries;
	}
	#endregion

	#region Type Conversion Helpers
	/// <summary>
	/// Aim: Convert SupportType to TicketType.
	/// Params: supportType - The SupportType to convert.
	/// Return: Corresponding TicketType.
	/// </summary>
	public static TicketType ConvertSupportTypeToTicketType(SupportType supportType)
	{
		return supportType switch
		{
			SupportType.Bug => TicketType.Error,
			SupportType.FeatureRequest => TicketType.FeatureRequest,
			SupportType.Feedback => TicketType.CustomerFeedback,
			_ => TicketType.SupportRequest
		};
	}

	/// <summary>
	/// Aim: Convert TicketType to SupportType.
	/// Params: ticketType - The TicketType to convert.
	/// Return: Corresponding SupportType.
	/// </summary>
	public static SupportType ConvertTicketTypeToSupportType(TicketType ticketType)
	{
		return ticketType switch
		{
			TicketType.Error => SupportType.Bug,
			TicketType.FeatureRequest => SupportType.FeatureRequest,
			TicketType.CustomerFeedback => SupportType.Feedback,
			_ => SupportType.Feedback
		};
	}

	/// <summary>
	/// Aim: Convert TicketItem from API to Support model.
	/// Params: ticket - The TicketItem to convert.
	/// Return: Support model.
	/// </summary>
	private static Support ConvertTicketItemToSupport(TicketItem ticket)
	{
		var (title, text) = ParseMessage(ticket.Message);
		var support = new Support
		{
			Id = ticket.Id,
			Type = (int)ConvertTicketTypeToSupportType(ticket.Type),
			Title = title,
			Text = text,
			CreatedAt = ticket.CreatedAt.LocalDateTime
		};
		return support;
	}

	/// <summary>
	/// Aim: Format title and text into message for API.
	/// Params: title - Ticket title, text - Ticket text.
	/// Return: Combined message string.
	/// </summary>
	private static string FormatMessage(string title, string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return title;
		}
		return $"{title}\n\n{text}";
	}

	/// <summary>
	/// Aim: Parse API message back to title and text.
	/// Params: message - The message to parse.
	/// Return: Tuple of title and text.
	/// </summary>
	private static (string title, string text) ParseMessage(string message)
	{
		if (string.IsNullOrEmpty(message))
		{
			return (string.Empty, string.Empty);
		}

		var parts = message.Split("\n\n", 2);
		if (parts.Length == 2)
		{
			return (parts[0], parts[1]);
		}

		return (message, string.Empty);
	}
	#endregion

	#region Ticket Entries
	/// <summary>
	/// Aim: Load ticket entries from prefs.
	/// </summary>
	public static void LoadEntries()
	{
		var entries = PrefsHelper.GetValue<List<SupportEntry>>(MobileConstants.PREFS_SUPPORT_ENTRIES);
		if (entries != null)
		{
			App.SupportTicketEntries = entries;
		}
		else
		{
			App.SupportTicketEntries = new List<SupportEntry>();
		}
	}

	/// <summary>
	/// Aim: Save ticket entries to prefs.
	/// </summary>
	public static void SaveEntries()
	{
		PrefsHelper.SetValue(MobileConstants.PREFS_SUPPORT_ENTRIES, App.SupportTicketEntries);
	}

	/// <summary>
	/// Aim: Add a ticket entry and save.
	/// Params: entry (SupportEntry) - The entry to add.
	/// </summary>
	public static void AddEntry(SupportEntry entry)
	{
		entry.Id = GetNextEntryId();
		entry.CreatedAt = DateTime.Now;
		App.SupportTicketEntries.Add(entry);
		SaveEntries();
	}

	/// <summary>
	/// Aim: Get all entries for a specific ticket.
	/// Params: ticketId (int) - The ticket id.
	/// Return: List of entries for the ticket.
	/// </summary>
	public static List<SupportEntry> GetEntriesByTicketId(int ticketId)
	{
		var entries = App.SupportTicketEntries
			.Where(e => e.SupportId == ticketId)
			.OrderBy(e => e.CreatedAt)
			.ToList();
		return entries;
	}

	/// <summary>
	/// Aim: Get the next available id for a ticket entry.
	/// Return: The next available id.
	/// </summary>
	private static int GetNextEntryId()
	{
		if (App.SupportTicketEntries.Count == 0)
		{
			return 1;
		}

		var maxId = App.SupportTicketEntries.Max(e => e.Id);
		return maxId + 1;
	}
	#endregion
}

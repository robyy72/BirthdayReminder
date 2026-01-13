#region Usings
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Common;
#endregion

namespace Mobile;

/// <summary>
/// Aim: Static service for API communication with connection pooling and JWT support.
/// </summary>
public static class ApiService
{
	#region Private Fields
	private static readonly SocketsHttpHandler _handler = new()
	{
		PooledConnectionLifetime = TimeSpan.FromMinutes(10),
		PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5)
	};
	private static readonly HttpClient _httpClient = new(_handler);
	private static readonly string _baseUrl;
	private static Func<Task<string?>>? _tokenProvider;
	#endregion

	#region Constructor
	static ApiService()
	{
#if DEBUG
		_baseUrl = CommonHelper.GetApiUrl(isDevelopment: true);
#else
		_baseUrl = CommonHelper.GetApiUrl(isDevelopment: false);
#endif
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// Aim: Configure the JWT token provider.
	/// Params: tokenProvider (Func&lt;Task&lt;string?&gt;&gt;) - Async function returning the JWT token.
	/// </summary>
	public static void ConfigureTokenProvider(Func<Task<string?>> tokenProvider)
	{
		_tokenProvider = tokenProvider;
	}

	/// <summary>
	/// Aim: Perform GET request and deserialize response to type T.
	/// Params: endpoint (string) - API endpoint path.
	/// Return (T?): Deserialized object of type T or default.
	/// </summary>
	public static async Task<T?> GetAsync<T>(string endpoint)
	{
		try
		{
			if (!MobileService.HasNetworkAccess()) return default;

			await AddAuthHeaderAsync();
			var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<T>();
		}
		catch (Exception ex)
		{
			ErrorService.Handle(ex);
			return default;
		}
	}

	/// <summary>
	/// Aim: Perform POST request with data and deserialize response to type T.
	/// Params:
	///   endpoint (string) - API endpoint path.
	///   data (object) - Object to send.
	/// Return (T?): Deserialized object of type T or default.
	/// </summary>
	public static async Task<T?> PostAsync<T>(string endpoint, object data)
	{
		try
		{
			if (!MobileService.HasNetworkAccess()) return default;

			await AddAuthHeaderAsync();
			var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/{endpoint}", data);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<T>();
		}
		catch (Exception ex)
		{
			ErrorService.Handle(ex);
			return default;
		}
	}

	/// <summary>
	/// Aim: Perform PUT request with data and deserialize response to type T.
	/// Params:
	///   endpoint (string) - API endpoint path.
	///   data (object) - Object to send.
	/// Return (T?): Deserialized object of type T or default.
	/// </summary>
	public static async Task<T?> PutAsync<T>(string endpoint, object data)
	{
		try
		{
			if (!MobileService.HasNetworkAccess()) return default;

			await AddAuthHeaderAsync();
			var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{endpoint}", data);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<T>();
		}
		catch (Exception ex)
		{
			ErrorService.Handle(ex);
			return default;
		}
	}

	/// <summary>
	/// Aim: Perform DELETE request.
	/// Params: endpoint (string) - API endpoint path.
	/// Return (bool): True if successful, false otherwise.
	/// </summary>
	public static async Task<bool> DeleteAsync(string endpoint)
	{
		try
		{
			if (!MobileService.HasNetworkAccess()) return false;

			await AddAuthHeaderAsync();
			var response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}");
			return response.IsSuccessStatusCode;
		}
		catch (Exception ex)
		{
			ErrorService.Handle(ex);
			return false;
		}
	}
	#endregion

	#region App-Specific Methods
	/// <summary>
	/// Aim: Send heartbeat to backend.
	/// Return (bool): True if successful.
	/// </summary>
	public static async Task<bool> SendHeartbeatAsync()
	{
		try
		{
			if (!MobileService.HasNetworkAccess()) return false;

			var dto = new HeartbeatDto
			{
				UserId = App.Account.UserId,
				Email = App.Account.Email,
				PhoneNumber = App.Account.PhoneNumber,
				PurchaseToken = App.Account.PurchaseToken,
				Store = App.Account.Store,
				PreferredChannel = App.Account.PreferredChannel
			};

			await AddAuthHeaderAsync();
			var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/heartbeat", dto);

			if (response.IsSuccessStatusCode)
			{
				App.Account.LastHeartbeat = DateTime.UtcNow;
				AccountService.Save();
			}

			return response.IsSuccessStatusCode;
		}
		catch (Exception ex)
		{
			ErrorService.Handle(ex);
			return false;
		}
	}

	/// <summary>
	/// Aim: Get all tickets for current user from backend.
	/// Return (List&lt;TicketItem&gt;?): List of tickets or null if failed.
	/// </summary>
	public static async Task<List<TicketItem>?> GetTicketsAsync()
	{
		try
		{
			if (!MobileService.HasNetworkAccess()) return null;

			await AddAuthHeaderAsync();
			var response = await _httpClient.GetAsync($"{_baseUrl}/api/support");
			if (!response.IsSuccessStatusCode) return null;

			var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
			var tickets = await response.Content.ReadFromJsonAsync<List<TicketItem>>(options);
			return tickets;
		}
		catch (Exception ex)
		{
			ErrorService.Handle(ex);
			return null;
		}
	}

	/// <summary>
	/// Aim: Send support ticket to backend.
	/// Params:
	///   message (string) - Support message.
	///   type (TicketType) - Ticket type.
	/// Return (int): Created ticket ID or -1 if failed.
	/// </summary>
	public static async Task<int> SendTicketAsync(string message, TicketType type = TicketType.SupportRequest)
	{
		try
		{
			if (!MobileService.HasNetworkAccess()) return -1;

			var dto = new TicketDto
			{
				UserId = App.Account.UserId,
				Message = message,
				Email = App.Account.Email,
				PhoneNumber = App.Account.PhoneNumber,
				PurchaseToken = App.Account.PurchaseToken,
				Store = App.Account.Store,
				Type = type
			};

			await AddAuthHeaderAsync();
			var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/support", dto);
			if (!response.IsSuccessStatusCode) return -1;

			var result = await response.Content.ReadFromJsonAsync<TicketResponse>();
			return result?.TicketId ?? -1;
		}
		catch (Exception ex)
		{
			ErrorService.Handle(ex);
			return -1;
		}
	}
	#endregion

	#region Private Methods
	/// <summary>
	/// Aim: Add JWT authorization header to request if token provider is configured.
	/// </summary>
	private static async Task AddAuthHeaderAsync()
	{
		if (_tokenProvider == null) return;

		string? token = await _tokenProvider();
		if (!string.IsNullOrEmpty(token))
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}
	}
	#endregion
}

/// <summary>
/// Aim: Response model for ticket creation.
/// </summary>
internal class TicketResponse
{
	public int TicketId { get; set; }
}

/// <summary>
/// Aim: Model for ticket data returned by API.
/// </summary>
public class TicketItem
{
	public int Id { get; set; }
	public string Message { get; set; } = string.Empty;
	public TicketType Type { get; set; }
	public TicketStatus Status { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset? UpdatedAt { get; set; }
	public DateTimeOffset? ClosedAt { get; set; }
}

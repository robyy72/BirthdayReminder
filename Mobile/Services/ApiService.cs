#region Usings

using System.Net.Http.Json;
using System.Text.Json;
using Common;

#endregion

namespace Mobile;

/// <summary>
/// Aim: Service for API communication with dynamic object support.
/// </summary>
public class ApiService
{
	private readonly HttpClient _httpClient;
	private readonly string _baseUrl;

	public ApiService()
	{
		_httpClient = new HttpClient();
		_baseUrl = "https://" + CommonConstants.API_BASE_URL;
	}

	/// <summary>
	/// Aim: Perform GET request and deserialize response to type T.
	/// Params: endpoint - API endpoint path.
	/// Return: Deserialized object of type T or default.
	/// </summary>
	public async Task<T?> GetAsync<T>(string endpoint)
	{
		try
		{
			if (!MobileService.HasNetworkAccess())
			{
				return default;
			}

			var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<T>();
		}
		catch (Exception ex)
		{
			ErrorService.LogError(ex, endpoint);
			return default;
		}
	}

	/// <summary>
	/// Aim: Perform POST request with data and deserialize response to type T.
	/// Params: endpoint - API endpoint path, data - object to send.
	/// Return: Deserialized object of type T or default.
	/// </summary>
	public async Task<T?> PostAsync<T>(string endpoint, object data)
	{
		try
		{
			if (!MobileService.HasNetworkAccess())
			{
				return default;
			}

			var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/{endpoint}", data);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<T>();
		}
		catch (Exception ex)
		{
			ErrorService.LogError(ex, endpoint);
			return default;
		}
	}

	/// <summary>
	/// Aim: Perform PUT request with data and deserialize response to type T.
	/// Params: endpoint - API endpoint path, data - object to send.
	/// Return: Deserialized object of type T or default.
	/// </summary>
	public async Task<T?> PutAsync<T>(string endpoint, object data)
	{
		try
		{
			if (!MobileService.HasNetworkAccess())
			{
				return default;
			}

			var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{endpoint}", data);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<T>();
		}
		catch (Exception ex)
		{
			ErrorService.LogError(ex, endpoint);
			return default;
		}
	}

	/// <summary>
	/// Aim: Perform DELETE request.
	/// Params: endpoint - API endpoint path.
	/// Return: True if successful, false otherwise.
	/// </summary>
	public async Task<bool> DeleteAsync(string endpoint)
	{
		try
		{
			if (!MobileService.HasNetworkAccess())
			{
				return false;
			}

			var response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}");
			return response.IsSuccessStatusCode;
		}
		catch (Exception ex)
		{
			ErrorService.LogError(ex, endpoint);
			return false;
		}
	}

	#region App-Specific Methods
	/// <summary>
	/// Aim: Send heartbeat to backend.
	/// Return: True if successful.
	/// </summary>
	public async Task<bool> SendHeartbeatAsync()
	{
		try
		{
			if (!MobileService.HasNetworkAccess())
			{
				return false;
			}

			var dto = new HeartbeatDto
			{
				UserId = App.Account.UserId,
				Email = App.Account.Email,
				PhoneNumber = App.Account.PhoneNumber,
				PurchaseToken = App.Account.PurchaseToken,
				Store = App.Account.Store,
				PreferredChannel = App.Account.PreferredChannel
			};

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
			ErrorService.LogError(ex, "heartbeat");
			return false;
		}
	}

	/// <summary>
	/// Aim: Send support ticket to backend.
	/// Params: message - support message, type - ticket type.
	/// Return: Created ticket ID or -1 if failed.
	/// </summary>
	public async Task<int> SendSupportTicketAsync(string message, SupportEntryType type = SupportEntryType.SupportRequest)
	{
		try
		{
			if (!MobileService.HasNetworkAccess())
			{
				return -1;
			}

			var dto = new SupportTicketDto
			{
				UserId = App.Account.UserId,
				Message = message,
				Email = App.Account.Email,
				PhoneNumber = App.Account.PhoneNumber,
				PurchaseToken = App.Account.PurchaseToken,
				Store = App.Account.Store,
				Type = type
			};

			var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/support", dto);
			if (!response.IsSuccessStatusCode)
			{
				return -1;
			}

			var result = await response.Content.ReadFromJsonAsync<TicketResponse>();
			return result?.TicketId ?? -1;
		}
		catch (Exception ex)
		{
			ErrorService.LogError(ex, "support");
			return -1;
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

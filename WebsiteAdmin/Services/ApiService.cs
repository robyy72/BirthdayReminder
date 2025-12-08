#region Usings

using System.Net.Http.Json;
using System.Text.Json;
using Common;

#endregion

namespace WebsiteAdmin;

/// <summary>
/// Aim: Service for API communication with dynamic object support.
/// </summary>
public class ApiService
{
	private readonly HttpClient _httpClient;
	private readonly string _baseUrl;

	public ApiService(HttpClient httpClient)
	{
		_httpClient = httpClient;
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
			var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<T>();
		}
		catch (Exception)
		{
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
			var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/{endpoint}", data);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<T>();
		}
		catch (Exception)
		{
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
			var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{endpoint}", data);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<T>();
		}
		catch (Exception)
		{
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
			var response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}");
			return response.IsSuccessStatusCode;
		}
		catch (Exception)
		{
			return false;
		}
	}
}

namespace Mobile;

/// <summary>
/// Aim: Service for mobile-specific functionality like network access.
/// </summary>
public static class MobileService
{
	/// <summary>
	/// Aim: Check if device has network access.
	/// Return: True if network is available, false otherwise.
	/// </summary>
	public static bool HasNetworkAccess()
	{
		var current = Connectivity.Current.NetworkAccess;
		return current == NetworkAccess.Internet;
	}

	/// <summary>
	/// Aim: Get current network access type.
	/// Return: NetworkAccess enum value.
	/// </summary>
	public static NetworkAccess GetNetworkAccess()
	{
		return Connectivity.Current.NetworkAccess;
	}

	/// <summary>
	/// Aim: Get connection profiles (WiFi, Cellular, etc.).
	/// Return: Collection of connection profiles.
	/// </summary>
	public static IEnumerable<ConnectionProfile> GetConnectionProfiles()
	{
		return Connectivity.Current.ConnectionProfiles;
	}

	/// <summary>
	/// Aim: Check if connected via WiFi.
	/// Return: True if WiFi is available, false otherwise.
	/// </summary>
	public static bool IsConnectedViaWifi()
	{
		return Connectivity.Current.ConnectionProfiles.Contains(ConnectionProfile.WiFi);
	}

	/// <summary>
	/// Aim: Check if connected via cellular.
	/// Return: True if cellular is available, false otherwise.
	/// </summary>
	public static bool IsConnectedViaCellular()
	{
		return Connectivity.Current.ConnectionProfiles.Contains(ConnectionProfile.Cellular);
	}
}

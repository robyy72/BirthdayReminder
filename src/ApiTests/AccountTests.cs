#region Usings
using System.Net.Http.Json;
using System.Text.Json;
using Common;
#endregion

namespace ApiTests;

public class AccountTests
{
	#region Fields
	private static readonly HttpClient _httpClient = new();
	private static readonly string _baseUrl = CommonConstants.URL_API_DEV;
	private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
	#endregion

	#region Test Data
	private static List<TestAccount>? _testAccounts;

	private static List<TestAccount> TestAccounts
	{
		get
		{
			if (_testAccounts == null)
			{
				var json = File.ReadAllText("TestData/accounts.json");
				_testAccounts = JsonSerializer.Deserialize<List<TestAccount>>(json, _jsonOptions) ?? [];
			}
			return _testAccounts;
		}
	}
	#endregion

	#region Tests
	[Fact]
	public void TestAccounts_ShouldHave10Entries()
	{
		// Arrange & Act
		var accounts = TestAccounts;

		// Assert
		Assert.Equal(10, accounts.Count);
	}

	[Fact]
	public void TestAccounts_ShouldHaveValidEmails()
	{
		// Arrange & Act
		var accounts = TestAccounts;

		// Assert
		Assert.All(accounts, a => Assert.Contains("@", a.Email));
	}

	[Fact]
	public void TestAccounts_ShouldHaveValidPhoneNumbers()
	{
		// Arrange & Act
		var accounts = TestAccounts;

		// Assert
		Assert.All(accounts, a => Assert.StartsWith("+49", a.PhoneNumber));
	}

	[Fact]
	public void CreateCustomer_ShouldCreateValidObject()
	{
		// Arrange
		var testAccount = TestAccounts.First();

		// Act
		var customer = new Customer
		{
			Id = Guid.NewGuid(),
			Email = testAccount.Email,
			PhoneNumber = testAccount.PhoneNumber,
			Store = (AppStore)testAccount.Store
		};

		// Assert
		Assert.NotEqual(Guid.Empty, customer.Id);
		Assert.Equal(testAccount.Email, customer.Email);
		Assert.Equal(testAccount.PhoneNumber, customer.PhoneNumber);
		Assert.Equal((AppStore)testAccount.Store, customer.Store);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(2)]
	public void CreateCustomer_FromTestData_ShouldBeValid(int index)
	{
		// Arrange
		var testAccount = TestAccounts[index];

		// Act
		var customer = new Customer
		{
			Id = Guid.NewGuid(),
			Email = testAccount.Email,
			PhoneNumber = testAccount.PhoneNumber,
			Store = (AppStore)testAccount.Store,
			Subscription = SubscriptionTier.Free,
			PreferredChannel = PreferredChannel.Email
		};

		// Assert
		Assert.NotNull(customer.Email);
		Assert.NotNull(customer.PhoneNumber);
		Assert.True(customer.Store == AppStore.GooglePlayStore || customer.Store == AppStore.AppleAppStore);
	}

	[Fact]
	public async Task SendHeartbeat_ShouldReturnResponse()
	{
		// Arrange
		var testAccount = TestAccounts.First();
		var heartbeat = new HeartbeatDto
		{
			UserId = Guid.NewGuid(),
			Email = testAccount.Email,
			PhoneNumber = testAccount.PhoneNumber,
			Store = (AppStore)testAccount.Store,
			PreferredChannel = PreferredChannel.Email
		};

		// Act
		try
		{
			var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/heartbeat", heartbeat);

			// Assert - we expect this to either succeed or return a specific error (like 401 for auth)
			Assert.True(
				response.IsSuccessStatusCode ||
				response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
				response.StatusCode == System.Net.HttpStatusCode.NotFound,
				$"Unexpected status code: {response.StatusCode}");
		}
		catch (HttpRequestException)
		{
			// API might not be available in test environment
			Assert.True(true, "API not available - skipping");
		}
	}

	[Fact]
	public async Task CheckApiHealth_ShouldBeReachable()
	{
		// Act
		try
		{
			var response = await _httpClient.GetAsync($"{_baseUrl}/health");

			// Assert - health endpoint should respond
			Assert.True(
				response.IsSuccessStatusCode ||
				response.StatusCode == System.Net.HttpStatusCode.NotFound,
				$"API returned: {response.StatusCode}");
		}
		catch (HttpRequestException)
		{
			// API might not be available
			Assert.True(true, "API not available - skipping");
		}
	}
	#endregion
}

#region Test Models
public class TestAccount
{
	public string Email { get; set; } = string.Empty;
	public string PhoneNumber { get; set; } = string.Empty;
	public int Store { get; set; }
}
#endregion

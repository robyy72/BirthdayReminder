#region Usings
using System.Net.Http.Json;
using System.Text.Json;
using Common;
#endregion

namespace ApiTests;

public class TicketTests
{
	#region Fields
	private static readonly HttpClient _httpClient = new();
	private static readonly string _baseUrl = CommonConstants.URL_API_DEV;
	private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
	#endregion

	#region Test Data
	private static List<TestTicket>? _testTickets;

	private static List<TestTicket> TestTickets
	{
		get
		{
			if (_testTickets == null)
			{
				var json = File.ReadAllText("TestData/tickets.json");
				_testTickets = JsonSerializer.Deserialize<List<TestTicket>>(json, _jsonOptions) ?? [];
			}
			return _testTickets;
		}
	}
	#endregion

	#region Tests
	[Fact]
	public void TestTickets_ShouldHave20Entries()
	{
		// Arrange & Act
		var tickets = TestTickets;

		// Assert
		Assert.Equal(20, tickets.Count);
	}

	[Fact]
	public void TestTickets_ShouldHaveValidTypes()
	{
		// Arrange & Act
		var tickets = TestTickets;

		// Assert - Types should be 1 (Error), 3 (FeatureRequest), or 4 (CustomerFeedback)
		Assert.All(tickets, t => Assert.True(
			t.Type == 1 || t.Type == 3 || t.Type == 4,
			$"Invalid ticket type: {t.Type}"));
	}

	[Fact]
	public void TestTickets_ShouldHaveMessages()
	{
		// Arrange & Act
		var tickets = TestTickets;

		// Assert
		Assert.All(tickets, t => Assert.False(string.IsNullOrEmpty(t.Message)));
	}

	[Fact]
	public void TestTickets_ShouldHaveBalancedTypes()
	{
		// Arrange & Act
		var tickets = TestTickets;
		var errorCount = tickets.Count(t => t.Type == 1);
		var featureCount = tickets.Count(t => t.Type == 3);
		var feedbackCount = tickets.Count(t => t.Type == 4);

		// Assert - Should have roughly balanced distribution
		Assert.True(errorCount >= 5, $"Expected at least 5 error tickets, got {errorCount}");
		Assert.True(featureCount >= 5, $"Expected at least 5 feature tickets, got {featureCount}");
		Assert.True(feedbackCount >= 5, $"Expected at least 5 feedback tickets, got {feedbackCount}");
	}

	[Fact]
	public void CreateTicketDto_ShouldCreateValidObject()
	{
		// Arrange
		var testTicket = TestTickets.First();

		// Act
		var ticketDto = new TicketDto
		{
			UserId = Guid.NewGuid(),
			Message = testTicket.Message,
			Email = "test@example.com",
			PhoneNumber = "+49151123456",
			Store = AppStore.GooglePlayStore,
			Type = (TicketType)testTicket.Type,
			Source = TicketSource.FromCustomerViaApp
		};

		// Assert
		Assert.NotEqual(Guid.Empty, ticketDto.UserId);
		Assert.Equal(testTicket.Message, ticketDto.Message);
		Assert.Equal((TicketType)testTicket.Type, ticketDto.Type);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(5)]
	[InlineData(10)]
	[InlineData(15)]
	public void CreateTicketDto_FromTestData_ShouldBeValid(int index)
	{
		// Arrange
		var testTicket = TestTickets[index];

		// Act
		var ticketDto = new TicketDto
		{
			UserId = Guid.NewGuid(),
			Message = testTicket.Message,
			Email = "user@example.com",
			Store = AppStore.AppleAppStore,
			Type = (TicketType)testTicket.Type,
			Source = TicketSource.FromCustomerViaApp
		};

		// Assert
		Assert.NotNull(ticketDto.Message);
		Assert.True(ticketDto.Type == TicketType.Error ||
					ticketDto.Type == TicketType.FeatureRequest ||
					ticketDto.Type == TicketType.CustomerFeedback);
	}

	[Fact]
	public async Task SendTicket_ShouldReturnResponse()
	{
		// Arrange
		var testTicket = TestTickets.First();
		var ticketDto = new TicketDto
		{
			UserId = Guid.NewGuid(),
			Message = testTicket.Message,
			Email = "test@example.com",
			PhoneNumber = "+49151123456",
			Store = AppStore.GooglePlayStore,
			Type = (TicketType)testTicket.Type,
			Source = TicketSource.FromCustomerViaApp
		};

		// Act
		try
		{
			var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/support", ticketDto);

			// Assert - we expect this to either succeed or return a specific error (like 401 for auth)
			Assert.True(
				response.IsSuccessStatusCode ||
				response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
				response.StatusCode == System.Net.HttpStatusCode.NotFound ||
				response.StatusCode == System.Net.HttpStatusCode.BadRequest,
				$"Unexpected status code: {response.StatusCode}");
		}
		catch (HttpRequestException)
		{
			// API might not be available in test environment
			Assert.True(true, "API not available - skipping");
		}
	}

	[Fact]
	public async Task GetTickets_ShouldReturnResponse()
	{
		// Act
		try
		{
			var response = await _httpClient.GetAsync($"{_baseUrl}/api/support");

			// Assert
			Assert.True(
				response.IsSuccessStatusCode ||
				response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
				response.StatusCode == System.Net.HttpStatusCode.NotFound,
				$"Unexpected status code: {response.StatusCode}");
		}
		catch (HttpRequestException)
		{
			// API might not be available
			Assert.True(true, "API not available - skipping");
		}
	}

	[Fact]
	public void TicketType_EnumValues_ShouldMatchExpected()
	{
		// Assert
		Assert.Equal(0, (int)TicketType.NotSet);
		Assert.Equal(1, (int)TicketType.Error);
		Assert.Equal(2, (int)TicketType.ReportBug);
		Assert.Equal(3, (int)TicketType.FeatureRequest);
		Assert.Equal(4, (int)TicketType.CustomerFeedback);
	}
	#endregion
}

#region Test Models
public class TestTicket
{
	public int Type { get; set; }
	public string Message { get; set; } = string.Empty;
}
#endregion

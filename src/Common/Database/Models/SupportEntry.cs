namespace Common;

public class SupportEntry
{
	public int Id { get; set; }
	public int SupportId { get; set; }
	public string Text { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; } = DateTime.Now;
}

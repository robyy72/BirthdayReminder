namespace Common;

public class Support
{
	public int Id { get; set; }
	public int Type { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Text { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; } = DateTime.Now;
}

#region Usings
using SQLite;
#endregion

namespace Mobile;

/// <summary>
/// Aim: SQLite database for offline error storage with WAL mode.
/// </summary>
public static class ErrorDatabase
{
	#region Private Fields
	private static SQLiteAsyncConnection? _db;
	private static bool _isInitialized;
	#endregion

	#region Public Methods
	/// <summary>
	/// Aim: Initialize the database connection and create table.
	/// Return (Task): Async task.
	/// </summary>
	public static async Task InitAsync()
	{
		if (_isInitialized) return;

		string dbPath = Path.Combine(FileSystem.AppDataDirectory, MobileConstants.ERROR_DB_FILENAME);
		_db = new SQLiteAsyncConnection(dbPath);

		await _db.ExecuteAsync("PRAGMA journal_mode=WAL;");
		await _db.ExecuteAsync("PRAGMA synchronous=NORMAL;");
		await _db.CreateTableAsync<ErrorLog>();

		_isInitialized = true;
	}

	/// <summary>
	/// Aim: Save an error log to the database.
	/// Params: log (ErrorLog) - The error log to save.
	/// Return (Task of int): The inserted row id.
	/// </summary>
	public static async Task<int> SaveAsync(ErrorLog log)
	{
		if (_db == null) await InitAsync();
		int result = await _db!.InsertAsync(log);
		return result;
	}

	/// <summary>
	/// Aim: Get all unsynced error logs.
	/// Return (Task of List of ErrorLog): List of unsynced error logs.
	/// </summary>
	public static async Task<List<ErrorLog>> GetUnsyncedAsync()
	{
		if (_db == null) await InitAsync();
		var logs = await _db!.Table<ErrorLog>().Where(e => !e.IsSynced).ToListAsync();
		return logs;
	}

	/// <summary>
	/// Aim: Mark an error log as synced.
	/// Params: id (int) - The id of the error log.
	/// Return (Task): Async task.
	/// </summary>
	public static async Task MarkSyncedAsync(int id)
	{
		if (_db == null) await InitAsync();
		await _db!.ExecuteAsync("UPDATE ErrorLog SET IsSynced = 1 WHERE Id = ?", id);
	}

	/// <summary>
	/// Aim: Delete old error logs older than specified days.
	/// Params: daysToKeep (int) - Number of days to keep logs.
	/// Return (Task of int): Number of deleted rows.
	/// </summary>
	public static async Task<int> DeleteOldLogsAsync(int daysToKeep = 30)
	{
		if (_db == null) await InitAsync();
		DateTime cutoff = DateTime.UtcNow.AddDays(-daysToKeep);
		int deleted = await _db!.ExecuteAsync("DELETE FROM ErrorLog WHERE Timestamp < ?", cutoff);
		return deleted;
	}
	#endregion
}

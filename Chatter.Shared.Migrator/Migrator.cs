using Microsoft.Data.SqlClient;

namespace Chatter.Migrator;

public class Migrator(string connectionString, string scriptPath)
{
    public async Task ExecutePendingMigrationsAsync()
    {
        EnsureMigrationTable();

        var executedScripts = GetExecutedScriptNames();
        var sqlFiles = Directory.GetFiles(scriptPath, "*.sql")
            .OrderBy(f => f)
            .ToList();

        foreach (var file in sqlFiles)
        {
            var fileName = Path.GetFileName(file);
            if (executedScripts.Contains(fileName)) continue;

            Console.WriteLine($"Running {fileName}...");

            var script = File.ReadAllText(file);
            using var sqlConnection = new SqlConnection(connectionString);
            await sqlConnection.OpenAsync();

            using var transaction = sqlConnection.BeginTransaction();

            try
            {
                using (var sqlCommand = new SqlCommand(script, sqlConnection, transaction))
                {
                    await sqlCommand.ExecuteNonQueryAsync();
                }

                using (var command = new SqlCommand(
                           "INSERT INTO dbo.MigrationHistory (ScriptName) VALUES (@name)", sqlConnection, transaction))
                {
                    command.Parameters.AddWithValue("@name", fileName);
                    await command.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                Console.WriteLine($"{fileName} executed successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error executing {fileName}: {ex.Message}");
            }
        }

        Console.WriteLine("All pending migrations processed.");
    }

    private void EnsureMigrationTable()
    {
        using var sqlConnection = new SqlConnection(connectionString);
        sqlConnection.Open();

        var cmdText = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='dbo.MigrationHistory' AND xtype='U')
            BEGIN
                CREATE TABLE dbo.MigrationHistory (
                    Id INT IDENTITY PRIMARY KEY,
                    ScriptName NVARCHAR(255) NOT NULL UNIQUE,
                    ExecutedOn DATETIME NOT NULL DEFAULT GETDATE()
                );
            END";
        using var cmd = new SqlCommand(cmdText, sqlConnection);
        cmd.ExecuteNonQuery();
    }

    private HashSet<string> GetExecutedScriptNames()
    {
        var result = new HashSet<string>();
        using var conn = new SqlConnection(connectionString);
        conn.Open();

        var cmd = new SqlCommand("SELECT ScriptName FROM dbo.MigrationHistory", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(reader.GetString(0));
        }
        return result;
    }
}
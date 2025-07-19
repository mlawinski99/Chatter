using Npgsql;

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
            using var sqlConnection = new NpgsqlConnection(connectionString);
            await sqlConnection.OpenAsync();

            using var transaction = sqlConnection.BeginTransaction();

            try
            {
                using (var sqlCommand = new NpgsqlCommand(script, sqlConnection, transaction))
                {
                    await sqlCommand.ExecuteNonQueryAsync();
                }

                using (var command = new NpgsqlCommand(
                           @"INSERT INTO public.""MigrationHistory"" (""ScriptName"") VALUES (@name)", 
                           sqlConnection, transaction))
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
                throw;
            }
        }

        Console.WriteLine("All pending migrations processed.");
    }

    private void EnsureMigrationTable()
    {
        using var sqlConnection = new NpgsqlConnection(connectionString);
        sqlConnection.Open();

        var cmdText = @"
            CREATE TABLE IF NOT EXISTS public.""MigrationHistory"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""ScriptName"" VARCHAR(255) NOT NULL UNIQUE,
                ""ExecutedOn"" TIMESTAMP NOT NULL DEFAULT NOW()
            );
        ";
        using var cmd = new NpgsqlCommand(cmdText, sqlConnection);
        cmd.ExecuteNonQuery();
    }

    private HashSet<string> GetExecutedScriptNames()
    {
        var result = new HashSet<string>();
        using var conn = new NpgsqlConnection(connectionString);
        conn.Open();

        var cmd = new NpgsqlCommand(@"SELECT ""ScriptName"" FROM public.""MigrationHistory""", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(reader.GetString(0));
        }
        return result;
    }
}
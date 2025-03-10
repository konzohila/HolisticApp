using MySqlConnector;
using Microsoft.Extensions.Logging;

namespace HolisticApp.Helpers
{
    public class DbHelper
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public DbHelper(string connectionString, ILogger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<MySqlConnection> GetConnectionAsync()
        {
            try
            {
                var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();
                _logger.LogDebug("Datenbankverbindung erfolgreich geöffnet.");
                return connection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Öffnen der Datenbankverbindung.");
                throw;
            }
        }

        public async Task<int> ExecuteNonQueryAsync(string commandText, Action<MySqlCommand> setupParameters)
        {
            await using var connection = await GetConnectionAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = commandText;
            setupParameters(command);
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<object?> ExecuteScalarAsync(string commandText, Action<MySqlCommand> setupParameters)
        {
            await using var connection = await GetConnectionAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = commandText;
            setupParameters(command);
            return await command.ExecuteScalarAsync();
        }
    }
}
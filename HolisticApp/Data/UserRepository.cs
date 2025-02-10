using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using MySqlConnector;

namespace HolisticApp.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        // SQL-Statements als Konstanten auslagern
        private const string selectAllUsersSql = "SELECT * FROM Users";
        private const string selectUserByIdSql = @"
            SELECT Id, Username, Email, PasswordHash, CurrentComplaint, Age, Gender, Height, Weight, MasterAccountId, Role 
            FROM Users WHERE Id = @id";
        private const string UpdateUserSql = @"
            UPDATE Users
            SET Username = @username, 
                Email = @email, 
                PasswordHash = @passwordHash,
                CurrentComplaint = @currentComplaint,
                Age = @age,
                Gender = @gender,
                Height = @height,
                Weight = @weight,
                Role = @role
            WHERE Id = @id";
        private const string InsertUserSql = @"
            INSERT INTO Users (Username, Email, PasswordHash, CurrentComplaint, Age, Gender, Height, Weight, Role)
            VALUES (@username, @email, @passwordHash, @currentComplaint, @age, @gender, @height, @weight, @role)";
        private const string DeleteUserSql = "DELETE FROM Users WHERE Id = @id";

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Öffnet eine asynchrone Verbindung zur Datenbank.
        /// </summary>
        private async Task<MySqlConnection> GetConnectionAsync()
        {
            var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        // Hilfsmethoden zum Auslesen von Spaltenwerten

        private string GetString(MySqlDataReader reader, string columnName, string defaultValue = "")
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetString(ordinal);
        }

        private int? GetNullableInt(MySqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
        }

        private decimal? GetNullableDecimal(MySqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (decimal?)null : reader.GetDecimal(ordinal);
        }

        private int GetInt(MySqlDataReader reader, string columnName, int defaultValue = 0)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetInt32(ordinal);
        }

        /// <summary>
        /// Erzeugt ein User-Objekt basierend auf den Daten des übergebenen DataReaders.
        /// </summary>
        private User CreateUserFromReader(MySqlDataReader reader)
        {
            return new User
            {
                Id = GetInt(reader, "Id"),
                Username = GetString(reader, "Username"),
                Email = GetString(reader, "Email"),
                PasswordHash = GetString(reader, "PasswordHash"),
                CurrentComplaint = GetString(reader, "CurrentComplaint", "Keine Beschwerden"),
                Age = GetNullableInt(reader, "Age"),
                Gender = GetString(reader, "Gender", "Nicht angegeben"),
                Height = GetNullableDecimal(reader, "Height"),
                Weight = GetNullableDecimal(reader, "Weight"),
                MasterAccountId = GetNullableInt(reader, "MasterAccountId"),
                Role = Enum.TryParse(GetString(reader, "Role", "Patient"), out UserRole parsedRole)
                        ? parsedRole
                        : UserRole.Patient
            };
        }

        /// <summary>
        /// Fügt dem übergebenen MySqlCommand alle Parameter hinzu, die für einen User benötigt werden.
        /// </summary>
        private void AddUserParameters(MySqlCommand command, User user)
        {
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@currentComplaint", user.CurrentComplaint ?? "Keine Beschwerden");
            command.Parameters.AddWithValue("@age", user.Age ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@gender", user.Gender ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@height", user.Height ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@weight", user.Weight ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@role", user.Role.ToString());
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var users = new List<User>();

            using (var connection = await GetConnectionAsync())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = selectAllUsersSql;

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(CreateUserFromReader(reader));
                    }
                }
            }
            return users;
        }

        public async Task<User?> GetUserAsync(int id)
        {
            using (var connection = await GetConnectionAsync())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = selectUserByIdSql;
                command.Parameters.AddWithValue("@id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return CreateUserFromReader(reader);
                    }
                }
            }
            return null;
        }

        public async Task<int> SaveUserAsync(User user)
        {
            using (var connection = await GetConnectionAsync())
            using (var command = connection.CreateCommand())
            {
                if (user.Id != 0)
                {
                    command.CommandText = UpdateUserSql;
                    command.Parameters.AddWithValue("@id", user.Id);
                }
                else
                {
                    command.CommandText = InsertUserSql;
                }

                AddUserParameters(command, user);

                try
                {
                    return await command.ExecuteNonQueryAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public async Task<int> DeleteUserAsync(int id)
        {
            using (var connection = await GetConnectionAsync())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = DeleteUserSql;
                command.Parameters.AddWithValue("@id", id);
                return await command.ExecuteNonQueryAsync();
            }
        }
    }
}
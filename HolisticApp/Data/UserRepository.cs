using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using MySqlConnector;

namespace HolisticApp.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Hilfsmethoden
        private string GetString(MySqlDataReader reader, string columnName, string defaultValue = "")
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetString(ordinal);
        }

        private int? GetNullableInt(MySqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (int?)null : reader.GetInt32(ordinal);
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

        private async Task<MySqlConnection> GetConnectionAsync()
        {
            var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
        #endregion

        public async Task<List<User>> GetUsersAsync()
        {
            var users = new List<User>();
            using (var connection = await GetConnectionAsync())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Users";
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new User
                        {
                            Id = GetInt(reader, "Id"),
                            Username = GetString(reader, "Username"),
                            Email = GetString(reader, "Email"),
                            PasswordHash = GetString(reader, "PasswordHash"),
                            CurrentComplaint = GetString(reader, "CurrentComplaint", "Keine Beschwerden"),
                            Age = GetNullableInt(reader, "Age"),
                            Gender = GetString(reader, "Gender", "Nicht angegeben"),
                            Height = GetNullableDecimal(reader, "Height"),
                            Weight = GetNullableDecimal(reader, "Weight")
                        });
                    }
                }
            }
            return users;
        }

        public async Task<User> GetUserAsync(int id)
        {
            using (var connection = await GetConnectionAsync())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT Id, Username, Email, PasswordHash, CurrentComplaint, Age, Gender, Height, Weight 
                    FROM Users WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
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
                            Weight = GetNullableDecimal(reader, "Weight")
                        };
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
                    command.CommandText = @"
                        UPDATE Users
                        SET Username = @username, 
                            Email = @email, 
                            PasswordHash = @passwordHash,
                            CurrentComplaint = @currentComplaint,
                            Age = @age,
                            Gender = @gender,
                            Height = @height,
                            Weight = @weight
                        WHERE Id = @id";
                    command.Parameters.AddWithValue("@id", user.Id);
                }
                else
                {
                    command.CommandText = @"
                        INSERT INTO Users (Username, Email, PasswordHash, CurrentComplaint, Age, Gender, Height, Weight)
                        VALUES (@username, @email, @passwordHash, @currentComplaint, @age, @gender, @height, @weight)";
                }
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@currentComplaint", user.CurrentComplaint ?? "Keine Beschwerden");
                command.Parameters.AddWithValue("@age", user.Age ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@gender", user.Gender ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@height", user.Height ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@weight", user.Weight ?? (object)DBNull.Value);
                return await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<int> DeleteUserAsync(int id)
        {
            using (var connection = await GetConnectionAsync())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Users WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);
                return await command.ExecuteNonQueryAsync();
            }
        }
    }
}

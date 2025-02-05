using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using HolisticApp.Models;
using MySqlConnector;

namespace HolisticApp.Data
{
    public class UserDatabase
    {
        private readonly string _connectionString;

        public UserDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }

        private async Task<MySqlConnection> GetConnectionAsync()
        {
            var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

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
                            Id = reader.GetInt32("Id"),
                            Username = reader.GetString("Username"),
                            Email = reader.GetString("Email"),
                            PasswordHash = reader.GetString("PasswordHash")
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
                command.CommandText = "SELECT * FROM Users WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new User
                        {
                            Id = reader.GetInt32("Id"),
                            Username = reader.GetString("Username"),
                            Email = reader.GetString("Email"),
                            PasswordHash = reader.GetString("PasswordHash")
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
                    // Update eines bestehenden Benutzers inklusive der neuen Spalte
                    command.CommandText = @"
                UPDATE Users
                SET Username = @username, 
                    Email = @email, 
                    PasswordHash = @passwordHash,
                    CurrentComplaint = @currentComplaint
                WHERE Id = @id";
                    command.Parameters.AddWithValue("@id", user.Id);
                }
                else
                {
                    // Einfügen eines neuen Benutzers
                    command.CommandText = @"
                INSERT INTO Users (Username, Email, PasswordHash, CurrentComplaint)
                VALUES (@username, @email, @passwordHash, @currentComplaint)";
                }

                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@currentComplaint", user.CurrentComplaint ?? "Keine Beschwerden");

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

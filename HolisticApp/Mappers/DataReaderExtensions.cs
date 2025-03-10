using HolisticApp.Models;
using MySqlConnector;
using HolisticApp.Helpers;
using Microsoft.Extensions.Logging;

namespace HolisticApp.Mappers
{
    public static class UserMapper
    {
        public static User CreateUserFromReader(MySqlDataReader reader, ILogger logger)
        {
            try
            {
                return new User
                {
                    Id = reader.GetIntSafe("id"),
                    Username = reader.GetStringSafe("username"),
                    Email = reader.GetStringSafe("email"),
                    PasswordHash = reader.GetStringSafe("password_hash"),
                    CurrentComplaint = reader.GetStringSafe("current_complaint", "Keine Beschwerden"),
                    Age = reader.GetNullableIntSafe("age"),
                    Gender = reader.GetStringSafe("gender", "Nicht angegeben"),
                    Height = reader.GetNullableDecimalSafe("height"),
                    Weight = reader.GetNullableDecimalSafe("weight"),
                    MasterAccountId = reader.GetNullableIntSafe("master_account_id"),
                    Role = Enum.TryParse(reader.GetStringSafe("role", "Patient"), out UserRole parsedRole)
                        ? parsedRole
                        : UserRole.Patient
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fehler beim Erstellen eines Benutzerobjekts aus dem Reader.");
                throw;
            }
        }

        public static void AddUserParameters(MySqlCommand command, User user)
        {
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@password_hash", user.PasswordHash);
            command.Parameters.AddWithValue("@current_complaint", user.CurrentComplaint);
            command.Parameters.AddWithValue("@age", user.Age.HasValue ? (object)user.Age.Value : DBNull.Value);
            command.Parameters.AddWithValue("@gender", user.Gender);
            command.Parameters.AddWithValue("@height", user.Height.HasValue ? (object)user.Height.Value : DBNull.Value);
            command.Parameters.AddWithValue("@weight", user.Weight.HasValue ? (object)user.Weight.Value : DBNull.Value);
            command.Parameters.AddWithValue("@role", user.Role.ToString());
        }
    }
}
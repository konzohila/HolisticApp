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
                    Id = reader.GetIntSafe("Id"),
                    Username = reader.GetStringSafe("Username"),
                    Email = reader.GetStringSafe("Email"),
                    PasswordHash = reader.GetStringSafe("PasswordHash"),
                    CurrentComplaint = reader.GetStringSafe("CurrentComplaint", "Keine Beschwerden"),
                    Age = reader.GetNullableIntSafe("Age"),
                    Gender = reader.GetStringSafe("Gender", "Nicht angegeben"),
                    Height = reader.GetNullableDecimalSafe("Height"),
                    Weight = reader.GetNullableDecimalSafe("Weight"),
                    MasterAccountId = reader.GetNullableIntSafe("MasterAccountId"),
                    Role = Enum.TryParse(reader.GetStringSafe("Role", "Patient"), out UserRole parsedRole)
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
            command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@currentComplaint", user.CurrentComplaint);
            command.Parameters.AddWithValue("@age", user.Age.HasValue ? (object)user.Age.Value : DBNull.Value);
            command.Parameters.AddWithValue("@gender", user.Gender);
            command.Parameters.AddWithValue("@height", user.Height.HasValue ? (object)user.Height.Value : DBNull.Value);
            command.Parameters.AddWithValue("@weight", user.Weight.HasValue ? (object)user.Weight.Value : DBNull.Value);
            command.Parameters.AddWithValue("@role", user.Role.ToString());
        }
    }
}
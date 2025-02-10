using HolisticApp.Data.Interfaces;
using HolisticApp.Models;
using MySqlConnector;
using System;
using System.Threading.Tasks;

namespace HolisticApp.Data
{
    public class InvitationRepository : IInvitationRepository
    {
        private readonly string _connectionString;

        public InvitationRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private async Task<MySqlConnection> GetConnectionAsync()
        {
            var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public async Task<Invitation> CreateInvitationAsync(Invitation invitation)
        {
            using (var connection = await GetConnectionAsync())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO Invitations (Token, MasterAccountId, CreatedAt, ExpiresAt, IsUsed)
                    VALUES (@token, @masterAccountId, @createdAt, @expiresAt, @isUsed);
                    SELECT LAST_INSERT_ID();";
                command.Parameters.AddWithValue("@token", invitation.Token);
                command.Parameters.AddWithValue("@masterAccountId", invitation.MasterAccountId);
                command.Parameters.AddWithValue("@createdAt", invitation.CreatedAt);
                command.Parameters.AddWithValue("@expiresAt", invitation.ExpiresAt);
                command.Parameters.AddWithValue("@isUsed", invitation.IsUsed);

                var result = await command.ExecuteScalarAsync();
                if (result == null)
                    throw new InvalidOperationException("ExecuteScalarAsync returned null.");
                invitation.Id = Convert.ToInt32(result);
                return invitation;
            }
        }

        public async Task<Invitation?> GetInvitationByTokenAsync(string token)
        {
            using (var connection = await GetConnectionAsync())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Invitations WHERE Token = @token";
                command.Parameters.AddWithValue("@token", token);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Invitation
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            // Vermeide mögliche Nullverweiszuweisung:
                            Token = reader["Token"]?.ToString() ?? string.Empty,
                            MasterAccountId = Convert.ToInt32(reader["MasterAccountId"]),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                            ExpiresAt = Convert.ToDateTime(reader["ExpiresAt"]),
                            IsUsed = Convert.ToBoolean(reader["IsUsed"])
                        };
                    }
                }
            }
            return null;
        }

        public async Task MarkInvitationAsUsedAsync(int invitationId)
        {
            using (var connection = await GetConnectionAsync())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Invitations SET IsUsed = TRUE WHERE Id = @id";
                command.Parameters.AddWithValue("@id", invitationId);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}

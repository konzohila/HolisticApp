namespace HolisticApp.Constants;

public class SqlCommands
{
    public const string SelectUserByIdSql = @"
            SELECT Id, Username, Email, PasswordHash, CurrentComplaint, Age, Gender, Height, Weight, MasterAccountId, Role 
            FROM Users WHERE Id = @id";

    public const string SelectUserByEmailOrUsernameSql = @"
            SELECT Id, Username, Email, PasswordHash, CurrentComplaint, Age, Gender, Height, Weight, MasterAccountId, Role 
            FROM Users WHERE Email = @value OR Username = @value";

    public const string InsertUserSql = @"
            INSERT INTO Users 
            (Username, Email, PasswordHash, CurrentComplaint, Age, Gender, Height, Weight, Role)
            VALUES (@username, @email, @passwordHash, @currentComplaint, @age, @gender, @height, @weight, @role)";

    public const string UpdateUserSql = @"
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

    public const string DeleteUserSql = "DELETE FROM Users WHERE Id = @id";

    public const string CountUserByEmailOrUsernameSql = @"
            SELECT COUNT(*) 
            FROM Users 
            WHERE Email = @value OR Username = @value";

    public const string SelectUsersByRoleSql = @"
            SELECT Id, Username, Email, PasswordHash, CurrentComplaint, Age, Gender, Height, Weight, MasterAccountId, Role 
            FROM Users WHERE Role = @role";
}
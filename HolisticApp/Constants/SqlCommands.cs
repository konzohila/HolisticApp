namespace HolisticApp.Constants;

public class SqlCommands
{
    public const string SelectUserByIdSql = @"
            SELECT Id, Username, Email, password_hash, current_complaint, Age, Gender, Height, Weight, master_account_id, Role 
            FROM Users WHERE Id = @id";

    public const string SelectUserByEmailOrUsernameSql = @"
            SELECT Id, Username, Email, password_hash, current_complaint, Age, Gender, Height, Weight, master_account_id, Role 
            FROM Users WHERE Email = @value OR Username = @value";

    public const string InsertUserSql = @"
            INSERT INTO Users 
            (Username, Email, password_hash, current_complaint, Age, Gender, Height, Weight, Role)
            VALUES (@username, @email, @password_hash, @current_complaint, @age, @gender, @height, @weight, @role)";

    public const string UpdateUserSql = @"
            UPDATE Users
            SET Username = @username, 
                Email = @email, 
                password_hash = @password_hash,
                current_complaint = @current_complaint,
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
            SELECT Id, Username, Email, password_hash, current_complaint, Age, Gender, Height, Weight, master_account_id, Role 
            FROM Users WHERE Role = @role";
}
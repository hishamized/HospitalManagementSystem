using Dapper;
using HMS.Application.DTO.User;
using HMS.Application.DTOs;
using HMS.Application.DTOs.Users;
using HMS.Application.Interfaces;
using HMS.Application.ViewModel.User;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using System.Data;

public class UserRepository : IUserRepository
{
    private readonly DapperContext _context;

    public UserRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<UserRoleDto?> GetUserWithRolesByUsernameOrEmailAsync(string usernameOrEmail)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@UsernameOrEmail", usernameOrEmail);

        var userDictionary = new Dictionary<int, UserRoleDto>();

        var result = await connection.QueryAsync<UserRoleDto, string, UserRoleDto>(
            "sp_GetUserWithRolesByUsernameOrEmail", // SP that accepts username or email
            (user, role) =>
            {
                if (!userDictionary.TryGetValue(user.UserId, out var currentUser))
                {
                    currentUser = user;
                    userDictionary.Add(user.UserId, currentUser);
                }
                if (!string.IsNullOrEmpty(role))
                    currentUser.Roles.Add(role);

                return currentUser;
            },
            param: parameters,
            splitOn: "RoleName",
            commandType: CommandType.StoredProcedure
        );

        return userDictionary.Values.FirstOrDefault();
    }


    public async Task<int> CreateAdminAsync(CreateAdminDto admin, string passwordHash)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@FullName", admin.FullName);
        parameters.Add("@Email", admin.Email);
        parameters.Add("@ContactNumber", admin.ContactNumber);
        parameters.Add("@DateOfBirth", admin.DateOfBirth);
        parameters.Add("@Gender", admin.Gender);
        parameters.Add("@Username", admin.Username);
        parameters.Add("@PasswordHash", passwordHash); // ✅ correct name for SP
        parameters.Add("@IsActive", admin.IsActive);
        parameters.Add("@RoleId", admin.RoleId);

        // Call SP
        var result = await connection.ExecuteScalarAsync<int>(
            "sp_CreateAdmin",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }
    public async Task<IEnumerable<AdminListVm>> GetAllAdminsAsync()
    {
        using var connection = _context.CreateConnection();

        // 🧩 Using DynamicParameters (even though SP takes no input params)
        var parameters = new DynamicParameters();
        parameters.AddDynamicParams(new { }); // Keeps consistency for future extensibility

        var result = await connection.QueryAsync<AdminListVm>(
            "sp_GetAllAdmins",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public async Task<UserOtpResultDto?> VerifyEmailPhoneAsync(VerifyEmailPhoneDto dto)
    {
        using var conn = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@Email", dto.Email);
        parameters.Add("@PhoneNumber", dto.PhoneNumber);

        try
        {
            var result = await conn.QueryFirstOrDefaultAsync<UserOtpResultDto>(
                "sp_VerifyEmailPhoneAndGenerateOtp",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }
        catch
        {
            throw; // let higher layer handle/log
        }
    }

    public async Task<UserOtpResultDto?> VerifyOtpAsync(string otpCode)
    {
        try
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@OtpCode", otpCode, DbType.String);

            var result = await connection.QueryFirstOrDefaultAsync<UserOtpResultDto>(
                "sp_VerifyUserOtp",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    // Plan (pseudocode):
    // 1. Generate a strong random password.
    // 2. Hash the password using BCrypt.
    // 3. Execute the stored procedure and immediately request @@ROWCOUNT in the same command text.
    //    - This avoids relying on ExecuteAsync's returned value (which can be -1 when NOCOUNT is ON).
    //    - Use ExecuteScalarAsync<int> with SQL: "EXEC sp_ResetPasswordAfterOtp @UserId, @PasswordHash; SELECT @@ROWCOUNT;".
    // 4. If the returned row count > 0, return the new plain-text password; otherwise return null.
    // 5. Keep method async and dispose the DB connection with 'using'.

    public async Task<string?> ResetPasswordAfterOtpAsync(int userId)
    {
        using var conn = _context.CreateConnection();

        // 1. Generate a strong random password
        string newPassword = GenerateStrongPassword(10); // length = 10

        // 2. Hash it
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

        // 3. Execute the stored procedure then SELECT @@ROWCOUNT in the same command to get affected rows reliably
        var sql = "EXEC sp_ResetPasswordAfterOtp @UserId, @PasswordHash; SELECT @@ROWCOUNT;";
        var rows = await conn.ExecuteScalarAsync<int>(
            sql,
            new { UserId = userId, PasswordHash = passwordHash }
        );

        // 4. Return the new password only if update succeeded
        return rows > 0 ? newPassword : null;
    }

    private string GenerateStrongPassword(int length)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
        var random = new Random();
        return new string(Enumerable.Repeat(validChars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

}

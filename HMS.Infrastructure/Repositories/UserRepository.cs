using Dapper;
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


}

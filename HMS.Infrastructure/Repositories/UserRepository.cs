using Dapper;
using HMS.Application.DTOs;
using HMS.Application.Interfaces;
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


    // Implement IRepository methods (Add, Update, Delete...) if needed
}

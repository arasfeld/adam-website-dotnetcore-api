using Api.Entities;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Auth
{
    public class UserStore : IUserStore<User>, IUserPasswordStore<User>, IUserRoleStore<User>
    {
        private readonly string _connectionString;

        public UserStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                user.UserId = await connection.QuerySingleAsync<int>($@"
                    INSERT INTO [Users] ([UserName], [NormalizedUserName], [PasswordHash])
                    VALUES (@{nameof(User.UserName)}, @{nameof(User.NormalizedUserName)}, @{nameof(User.PasswordHash)});
                    SELECT CAST(SCOPE_IDENTITY() as int)", user);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($@"
                    DELETE FROM [Users] 
                    WHERE [UserId] = @{nameof(User.UserId)}", user);
            }

            return IdentityResult.Success;
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<User>($@"
                    SELECT * FROM [Users]
                    WHERE [UserId] = @{nameof(userId)}", 
                    new { userId });
            }
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<User>($@"
                    SELECT * FROM [Users]
                    WHERE [NormalizedUserName] = @{nameof(normalizedUserName)}", 
                    new { normalizedUserName });
            }
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserId.ToString());
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($@"
                    UPDATE [Users] SET
                        [UserName] = @{nameof(User.UserName)},
                        [NormalizedUserName] = @{nameof(User.NormalizedUserName)},
                        [PasswordHash] = @{nameof(User.PasswordHash)}
                    WHERE [UserId] = @{nameof(User.UserId)}", user);
            }

            return IdentityResult.Success;
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var normalizedName = roleName.ToUpper();
                var roleId = await connection.ExecuteScalarAsync<int?>($@"
                    SELECT [RoleId] 
                    FROM [Roles] 
                    WHERE [NormalizedName] = @{nameof(normalizedName)}", 
                    new { normalizedName });
                if (!roleId.HasValue)
                {
                    roleId = await connection.ExecuteAsync($@"
                        INSERT INTO [Roles] ([Name], [NormalizedName]) 
                        VALUES(@{nameof(roleName)}, @{nameof(normalizedName)})",
                        new { roleName, normalizedName });
                }
                
                await connection.ExecuteAsync($@"
                    IF NOT EXISTS(SELECT 1 FROM [UserRoles] WHERE [UserId] = @userId AND [RoleId] = @{nameof(roleId)}) 
                    INSERT INTO [UserRoles] ([UserId], [RoleId]) 
                    VALUES(@userId, @{nameof(roleId)})",
                    new { userId = user.UserId, roleId });
            }
        }

        public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var roleId = await connection.ExecuteScalarAsync<int?>($@"
                    SELECT [RoleId] 
                    FROM [Roles] 
                    WHERE [NormalizedName] = @normalizedName", 
                    new { normalizedName = roleName.ToUpper() });
                if (!roleId.HasValue)
                {
                    await connection.ExecuteAsync($@"
                        DELETE FROM [UserRoles] 
                        WHERE [UserId] = @userId AND [RoleId] = @{nameof(roleId)}", 
                        new { userId = user.UserId, roleId });
                }
            }
        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var queryResults = await connection.QueryAsync<string>($@"
                    SELECT R.[Name] 
                    FROM [Roles] R 
                        INNER JOIN [UserRoles] UR ON UR.[RoleId] = R.RoleId 
                    WHERE UR.UserId = @userId", 
                    new { userId = user.UserId });

                return queryResults.ToList();
            }
        }

        public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                var roleId = await connection.ExecuteScalarAsync<int?>($@"
                    SELECT [RoleId] 
                    FROM [Roles] 
                    WHERE [NormalizedName] = @normalizedName", 
                    new { normalizedName = roleName.ToUpper() });
                if (roleId == default(int)) return false;
                var matchingRoles = await connection.ExecuteScalarAsync<int>($@"
                    SELECT COUNT(*) 
                    FROM [UserRoles] 
                    WHERE [UserId] = @userId AND [RoleId] = @{nameof(roleId)}",
                    new { userId = user.UserId, roleId });

                return matchingRoles > 0;
            }
        }

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                var queryResults = await connection.QueryAsync<User>($@"
                    SELECT U.* 
                    FROM [Users] U
                        INNER JOIN [UserRoles] UR ON UR.[UserId] = U.[UserId] 
                        INNER JOIN [Roles] R ON R.[RoleId] = UR.[RoleId] 
                    WHERE R.[NormalizedName] = @normalizedName",
                    new { normalizedName = roleName.ToUpper() });

                return queryResults.ToList();
            }
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }
    }
}

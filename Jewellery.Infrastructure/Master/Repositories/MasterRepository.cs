using Dapper;
using Jewellery.Application.Common.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Jewellery.Infrastructure.Master.Repositories
{
    public class MasterRepository : IMasterRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUser;

        public MasterRepository(
            IConfiguration configuration,
            ICurrentUserService currentUser)
        {
            _configuration = configuration;
            _currentUser = currentUser;
        }

        public async Task<dynamic> RoleMaster_ManageAsync(RoleMasterModel roleModel)
        {
            using var connection =
                new SqlConnection(
                    _configuration.GetConnectionString(_currentUser.shopCode));

            var param = new DynamicParameters();

            param.Add("@RoleId", roleModel.RoleId);
            param.Add("@RoleName", roleModel.RoleName);
            param.Add("@RoleDescription", roleModel.RoleDescription);
            param.Add("@IsActive", roleModel.IsActive);
            param.Add("@UserId", _currentUser.UserId);
            param.Add("@TypeId", roleModel.TypeId);
            param.Add("@RoleCode", roleModel.RoleCode);
            return await connection.QueryAsync("Jewellery.Role_Master_Manage", param, commandType: CommandType.StoredProcedure);
        }
        public async Task<dynamic> RoleMenuMapping_ManageAsync(RoleMenuMappingModel model)
        {
            using var connection =
                new SqlConnection(
                    _configuration.GetConnectionString(_currentUser.shopCode));

            var param = new DynamicParameters();

            param.Add("@RoleId", model.RoleId);
            param.Add("@MenuIds", model.MenuIds);
            param.Add("@TypeId", model.TypeId);
            param.Add("@UserId", _currentUser.UserId);
            return await connection.QueryAsync(
                "Jewellery.RoleMenuMapping_Manage",
                param,
                commandType: CommandType.StoredProcedure);
        }
        public async Task<dynamic> ChangePasswordAsync(ChangePasswordModel model)
        {
            using var connection =
                new SqlConnection(
                    _configuration.GetConnectionString(_currentUser.shopCode));

            var param = new DynamicParameters();

            param.Add("@UserId", _currentUser.UserId);
            param.Add("@CurrentPasswordHash", model.CurrentPasswordHash);
            param.Add("@NewPasswordHash", model.NewPasswordHash);
            param.Add("@TypeId", model.TypeId);

            using var result = await connection.QueryMultipleAsync(
                "Jewellery.ChangePassword_Manage",
                param,
                commandType: CommandType.StoredProcedure
            );

            return await result.ReadFirstAsync<dynamic>();
        }
        public async Task<dynamic> User_ManageAsync(UserModel model)
        {
            using var connection =
                new SqlConnection(
                    _configuration.GetConnectionString(_currentUser.shopCode));

            var param = new DynamicParameters();

            param.Add("@LoginId", model.LoginId);
            param.Add("@UserName", model.UserName);
            param.Add("@PasswordHash", model.PasswordHash);
            param.Add("@RoleId", model.RoleId);
            param.Add("@Email", model.Email);
            param.Add("@MobileNo", model.MobileNo);
            param.Add("@IsActive", model.IsActive);
            param.Add("@UserId", _currentUser.UserId);
            param.Add("@TypeId", model.TypeId);

            return await connection.QueryAsync(
                "Jewellery.User_Manage",
                param,
                commandType: CommandType.StoredProcedure);
        }
    }
}
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

            using var result = await connection.QueryMultipleAsync("Jewellery.User_Manage", param,commandType: CommandType.StoredProcedure);

            return await result.ReadAsync<dynamic>();
        }
        public async Task<dynamic> Supplier_ManageAsync(SupplierModel model)
        {
            using var connection =new SqlConnection(_configuration.GetConnectionString(_currentUser.shopCode));

            var param = new DynamicParameters();

            param.Add("@SupplierId", model.SupplierId);
            param.Add("@SupplierName", model.SupplierName);
            param.Add("@Phone", model.Phone);
            param.Add("@GSTIN", model.GSTIN);
            param.Add("@Address", model.Address);
            param.Add("@IsActive", model.IsActive);
            param.Add("@TypeId", model.TypeId);

            using var result = await connection.QueryMultipleAsync("Jewellery.Supplier_Master_Manage",param,commandType: CommandType.StoredProcedure);
            return await result.ReadAsync<dynamic>();
        }
        public async Task<dynamic> ProductMaster_ManageAsync(ProductMasterModel product)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString(_currentUser.shopCode));

            var parameters = new DynamicParameters();

            parameters.Add("@TypeId", product.TypeId);
            parameters.Add("@ProductId", product.ProductId);
            parameters.Add("@ProductCode", product.ProductCode);
            parameters.Add("@ProductName", product.ProductName);
            parameters.Add("@CategoryId", product.CategoryId);
            parameters.Add("@MetalId", product.MetalId);
            parameters.Add("@SupplierId", product.SupplierId);
            parameters.Add("@GrossWeight", product.GrossWeight);
            parameters.Add("@NetWeight", product.NetWeight);
            parameters.Add("@MakingCharge", product.MakingCharge);
            parameters.Add("@MakingChargeType", product.MakingChargeType);
            parameters.Add("@TotalQuantity", product.TotalQuantity);
            parameters.Add("@IsActive", product.IsActive);
            parameters.Add("@AuditBy", _currentUser.UserName);

            using var result = await connection.QueryMultipleAsync("Jewellery.ProductMaster_Manage",parameters,commandType:CommandType.StoredProcedure);

            return await result.ReadAsync<dynamic>();
        }
    }
}
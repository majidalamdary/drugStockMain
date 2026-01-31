using DrugStockWeb.Controllers;
using DrugStockWeb.Models;
using DrugStockWeb.Models.Account;
using DrugStockWeb.Utitlities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DrugStockWeb.Helper
{
    public static class PermissionHelper
    {

        public static MainDbContext Db = new MainDbContext();

        public static bool HasPermission(int permissionValue, string userId)
        {
            Console.WriteLine(@"user=" + userId);
            var user = Db.Users.Include(p => p.Roles).FirstOrDefault(p => p.Id == userId);
            Db.Entry(user).Reload();
            if (user != null)
            {
                var roles = user.Roles.ToList(); // Create a copy to avoid modification issues
                for (int i = 0; i < roles.Count; i++)
                {
                    Db.Entry(roles[i]).Reload();
                }

                string roleId = user.Roles.FirstOrDefault()?.RoleId;
                var test = Db.PermissionInRoles.FirstOrDefault(p => p.RoleId == roleId && p.PermissionId == permissionValue)?.ToString();
                if (test != null || roleId == Define.SuperAdminRoleId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;

        }
        public static async Task<bool> HasPermissionAsync(int permissionValue, string userId)
        {
            Console.WriteLine(@"user=" + userId);
            var user = await Db.Users.Include(p => p.Roles).FirstOrDefaultAsync(p => p.Id == userId);
            if (user != null)
            {
                string roleId = user.Roles.FirstOrDefault()?.RoleId;
                var test = await Db.PermissionInRoles.AnyAsync(p => p.RoleId == roleId && p.PermissionId == permissionValue);
                if (test || user.Id == Define.SuperAdminUserId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;

        }
    }
}
using System.Linq;
using DrugStockWeb.Helper;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DrugStockWeb.Migrations
{
    using DrugStockWeb.Models.Account;
    using DrugStockWeb.Models;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Data.Entity.Migrations;

    public partial class add_admin_user : DbMigration
    {
        public MainDbContext Db = new MainDbContext();
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager { get; set; }
        public override void Up()
        {
            // _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(Db));
            // // ایجاد نقش جدید
            // var role = new IdentityRole(Define.SuperAdminRoleName);
            // role.Id = Define.SuperAdminRoleId;
            // _roleManager.Create(role);
            //
            // User user = new User();
            // user.Id = Define.SuperAdminUserId;
            // user.FirstName = "مجید";
            // user.LastName = "علمداری";
            // user.Mobile = "09141484633";
            // user.DepartmentId = Db.Departments.FirstOrDefault().Id;
            // user.PasswordHash = new PasswordHasher().HashPassword("Food@1024!");
            // user.UserName = "admin";
            // Db.Users.Add(user);
            // Db.SaveChanges();
            //
            //
            // _userManager = new UserManager<User>(new UserStore<User>(Db));
            // user = _userManager.FindByName("admin");
            // // افزودن نقش به کاربر
            // _userManager.AddToRole(user.Id, Define.SuperAdminRoleName);

        }

        public override void Down()
        {
        }
    }
}

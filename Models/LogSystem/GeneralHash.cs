using DrugStockWeb.Helper;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrugStockWeb.Models.LogSystem
{
    public class GeneralHash
    {

        public int Id { get; set; }

        public byte[] UserModel { get; set; }
        public byte[] PermissionModel { get; set; }
        public byte[] UserRoleModel { get; set; }
        public byte[] RoleModel { get; set; }
        public byte[] OnlineUserModel { get; set; }
        public byte[] PermisionInRoleModel { get; set; }
        public byte[] StoreInUserModel { get; set; }
        public byte[] LogModel { get; set; }
        public byte[] LogSettingModel { get; set; }
        public byte[] LogType { get; set; }
        public byte[] OnlineUser { get; set; }
        public byte[] BlackListIp { get; set; }

        public byte[] HashValue { get; set; }

    }
}
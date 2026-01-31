using DrugStockWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DrugStockWeb.Models.Account
{
    public class PermissionInRole
    {
        public PermissionInRole()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }

        public byte[] HashValue { get; set; }
        public int PermissionId { get; set; }
        public Permission Permission { get; set; }
        public string RoleId { get; set; }
        public IdentityRole Role { get; set; }



    }
}
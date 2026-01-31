using DrugStockWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrugStockWeb.Models.Account
{
    public class Permission
    {
        public Permission()
        {
        }
        public Permission(string title)
        {
            this.Title = title;
        }
        public int Id { get; set; }


        public string Title { get; set; }
        public byte[] HashValue { get; set; }

        public ICollection<PermissionInRole> PermissionInRoles { get; set; }

    }
}
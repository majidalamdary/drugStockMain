using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DrugStockWeb.Models.Account
{
    
    public class ApplicationUserRole : IdentityUserRole
    {
        public ApplicationUserRole()
        {

        }
        public byte[] HashValue { get; set; }

    }
}
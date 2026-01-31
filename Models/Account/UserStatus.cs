using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DrugStockWeb.Helper;

namespace DrugStockWeb.Models.Account
{
    public class UserStatus
    {
        public UserStatus()
        {
            
        }
        [Required]
        public int Id { get; set; }

        public string Title { get; set; }

        public virtual ICollection<User> Users { get; set; }

    }
}
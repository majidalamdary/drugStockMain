using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrugStockWeb.Helper;

namespace DrugStockWeb.Models.Account
{
    public class Department
    {
        public Department()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }

        public string Title { get; set; }

        public virtual ICollection<User> Users { get; set; }

    }
}
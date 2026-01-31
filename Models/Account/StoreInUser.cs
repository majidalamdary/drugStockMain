using DrugStockWeb.Helper;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrugStockWeb.Models.Account
{
    public class StoreInUser
    {
        public StoreInUser()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }

        public byte[] HashValue { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
        public Guid StoreId { get; set; }
        public Store.Store Store { get; set; }

    }
}
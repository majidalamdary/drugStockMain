using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.Constancts;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DrugStockWeb.Models.Account
{
    public class OnlineUser 
    {
        public Guid Id { get; set; } = SequentialGuidGenerator.NewSequentialGuid();
        public string UserId { get; set; }

        public DateTime LoginDateTime { get; set; }
        public DateTime LastActivityDateTime { get; set; }
        public DateTime? LastFailedDateTime { get; set; }
        public string Browser { get; set; }
        public string SessionId { get; set; }

        public virtual User User { get; set; }
        public byte[] HashValue { get; set; }

    }
}
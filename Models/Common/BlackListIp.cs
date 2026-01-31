using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.Constancts;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DrugStockWeb.Models.Common
{
    public class BlackListIp
    {
        public BlackListIp()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }
        [Required]
        public string Ip { get; set; }
        public byte[] HashValue { get; set; }

    }
}
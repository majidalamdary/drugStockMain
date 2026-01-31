using System;
using DrugStockWeb.Models.Account;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DrugStockWeb.Helper;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrugStockWeb.Models.LogSystem
{
    public class LogType
    {
        public LogType()
        {
            
        }
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public byte[] HashValue { get; set; }
        public virtual ICollection<Logs> Logs { get; set; }
    }
}
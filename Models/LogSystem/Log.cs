using System;
using DrugStockWeb.Models.Account;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DrugStockWeb.Helper;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrugStockWeb.Models.LogSystem
{
    public class Logs
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        

        public long Id { get; set; }

        [Required]
        public DateTime LogDateTime { get; set; }
        [Required]
        public int LogTypeId { get; set; }
        public byte[] HashValue { get; set; }
        public string Creator { get; set; }

        [Required]
        public bool LogStatus { get; set; }
        [Required]
        public string IPAddress { get; set; }
        public string Description { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public bool IsSeen { get; set; }
        public virtual LogType LogType { get; set; }

    }
}
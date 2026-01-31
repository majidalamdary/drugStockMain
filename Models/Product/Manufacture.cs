using DrugStockWeb.Helper;
using DrugStockWeb.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DrugStockWeb.Models.Product
{
    public class Manufacture
    {
        public Manufacture()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string CreatorUserId { get; set; }
        public virtual User CreatorUser { get; set; }
        
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
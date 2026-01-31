using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.Account;

namespace DrugStockWeb.Models.ProductGroup
{
    public class ProductGroup
    {
        public ProductGroup()
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

        public virtual ICollection<ProductSubGroup> ProductSubGroups { get; set; }

    }
}
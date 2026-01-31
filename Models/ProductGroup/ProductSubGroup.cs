using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.Account;

namespace DrugStockWeb.Models.ProductGroup
{
    public class ProductSubGroup
    {
        public ProductSubGroup()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }


        public Guid ProductGroupId { get; set; }

        public virtual ProductGroup ProductGroup { get; set; }
        public virtual ICollection<Product.Product> Products { get; set; }

        public string CreatorUserId { get; set; }
        public virtual User CreatorUser { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
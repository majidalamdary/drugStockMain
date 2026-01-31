using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.Account;
using DrugStockWeb.Models.ProductGroup;

namespace DrugStockWeb.Models.Product
{
    public class ProductType
    {

        public int Id { get; set; }
        [Required]
        public string Title { get; set; }


        public virtual ICollection<Product> Products { get; set; }

    }
}
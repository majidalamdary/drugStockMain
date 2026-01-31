using DrugStockWeb.Helper;
using DrugStockWeb.Models.ProductGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrugStockWeb.Models.Product
{
    public class ProductTypeInProductSubGroup
    {
        public ProductTypeInProductSubGroup()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }
        public int ProductTypeId { get; set; }
        public virtual ProductType ProductType { get; set; }
        public Guid ProductSubGroupId { get; set; }
        public virtual ProductSubGroup ProductSubGroup { get; set; }
    }
}
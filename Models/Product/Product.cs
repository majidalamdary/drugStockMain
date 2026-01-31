using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DrugStockWeb.Helper;
using DrugStockWeb.Models.Account;
using DrugStockWeb.Models.ProductGroup;

namespace DrugStockWeb.Models.Product
{
    public class Product
    {
        public Product()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string GenericCode { get; set; }
        public string Dose { get; set; }
        [Required]
        public long MinimumCount { get; set; }
        [Required]
        public long? MaximumCount { get; set; }
        
        public long? BuyPrice { get; set; }
        public long? SellPrice { get; set; }


        public Guid StoreId { get; set; }
        public virtual  Store.Store Store { get; set; }
        public Guid? ManufactureId { get; set; }
        public virtual Manufacture Manufacture  { get; set; }

        public int ProductTypeId { get; set; }
        public virtual ProductType ProductType { get; set; }

        public Guid ProductSubGroupId { get; set; }
        public virtual ProductSubGroup ProductSubGroup { get; set; }
        public virtual ICollection<StoreReceipt.StoreReceiptDetail> StoreReceiptDetails { get; set; }
        public virtual ICollection<StoreReceipt.StoreReceiptDetailTemp> StoreReceiptDetailTemps { get; set; }

        public string CreatorUserId { get; set; }
        public virtual User CreatorUser { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
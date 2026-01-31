using DrugStockWeb.Helper;
using DrugStockWeb.Models.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrugStockWeb.Models.StoreReceipt
{
    public class StoreReceiptDetailTemp
    {
        public StoreReceiptDetailTemp()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public virtual Product.Product Product { get; set; }
        public long BuyPrice { get; set; }
        public long SellPrice { get; set; }
        public DateTime ExpireDate { get; set; }
        public string BatchNumber { get; set; }
        public Guid? ManufactureId { get; set; }
        public virtual Manufacture Manufacture { get; set; }

        public Guid StoreReceiptId { get; set; }
        public long Count { get; set; }



    }

}
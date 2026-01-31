
using DrugStockWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrugStockWeb.Models.Product;

namespace DrugStockWeb.Models.StoreReceipt
{
    public class StoreReceiptDetailReturn
    {
        public StoreReceiptDetailReturn()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }


        public Guid StoreReceiptDetailId { get; set; }
        public virtual StoreReceiptDetail StoreReceiptDetail { get; set; }
        public Guid StoreReceiptReturnId { get; set; }
        public virtual StoreReceiptReturn StoreReceiptReturn { get; set; }
        public long Count { get; set; }

        
    }
}
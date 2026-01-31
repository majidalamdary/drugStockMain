
using DrugStockWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrugStockWeb.Models.Product;

namespace DrugStockWeb.Models.StoreReceipt
{
    public class StoreReceiptDetail
    {
        public StoreReceiptDetail()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Product.Product Product { get; set; }
        public long BuyPrice { get; set; }
        public long SellPrice { get; set; }
        public DateTime ExpireDate { get; set; }
        public string BatchNumber { get; set; }

        public Guid StoreReceiptId { get; set; }
        public virtual StoreReceipt StoreReceipt { get; set; }
        public long Count { get; set; }
        public long? ReturnTempCount { get; set; }
        public Guid? ManufactureId { get; set; }
        public virtual Manufacture Manufacture { get; set; }
        public ICollection<Invoice.InvoiceDetail> InvoiceDetails { get; set; }
        public ICollection<StoreReceiptDetailReturn> StoreReceiptDetailReturns { get; set; }
    }
}
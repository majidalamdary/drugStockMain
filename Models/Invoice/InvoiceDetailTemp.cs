using DrugStockWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrugStockWeb.Models.Invoice
{
    public class InvoiceDetailTemp
    {
        public InvoiceDetailTemp()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }



        public Guid StoreReceiptDetailId { get; set; }
        public virtual StoreReceipt.StoreReceiptDetail StoreReceiptDetail { get; set; }

        public long SellPrice { get; set; }
        public Guid InvoiceId { get; set; }

        public long Count { get; set; }
    }
}
using DrugStockWeb.Helper;
using DrugStockWeb.Models.Product;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DrugStockWeb.ViewModels.InvoiceReport
{
    public class InvoiceDetailReportViewModel
    {
        public InvoiceDetailReportViewModel()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }


        public string ProductTitle { get; set; }
        public Guid StoreReceiptDetailId { get; set; }

        public Models.StoreReceipt.StoreReceiptDetail StoreReceiptDetail { get; set; }

        public string InvoiceDate { get; set; }
        public Guid InvoiceId { get; set; }
        public Models.Invoice.Invoice Invoice { get; set; }
        public Guid BusinnessPartnerId { get; set; }
        public Guid StoreId { get; set; }

        public long SellPrice { get; set; }



        public long Count { get; set; }

        public long RemainingCount { get; set; } = 0;
        public long ReturnTempCount { get; set; } = 0;
    }
}
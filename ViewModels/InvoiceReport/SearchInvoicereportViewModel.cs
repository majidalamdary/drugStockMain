using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrugStockWeb.ViewModels.InvoiceReport
{
    public class SearchInvoicereportViewModel
    {
        public Guid StoreId { get; set; }
        public Guid? ProductId { get; set; }
        
        public string InvoiceDateFrom { get; set; }
        public string InvoiceDateTo { get; set; }
        public int Page { get; set; }
    }
}
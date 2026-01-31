using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrugStockWeb.ViewModels.ReceiptReport
{
    public class SearchInvoicereportViewModel
    {
        public Guid StoreId { get; set; }
        public Guid? ProductId { get; set; }
        
        public string ReceiptDateFrom { get; set; }
        public string ReceiptDateTo { get; set; }
        public int Page { get; set; }
    }
}
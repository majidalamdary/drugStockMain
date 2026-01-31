using DrugStockWeb.ViewModels.ReceiptReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DrugStockWeb.ViewModels.InvoiceReport
{
    public class InvoiceReportViewModel
    {

        public List<SelectListItem> StoreList { get; set; }
        public List<InvoiceDetailReportViewModel> InvoiceDetailReportList { get; set; }
        public Guid StoreId { get; set; }
        public Guid ProductId { get; set; }
        public List<SelectListItem> ProductList { get; set; }
    }
}
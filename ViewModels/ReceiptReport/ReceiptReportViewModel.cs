using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DrugStockWeb.ViewModels.ReceiptReport
{
    public class ReceiptReportViewModel
    {

        public List<SelectListItem> StoreList { get; set; }
        public List<ReceiptDetailReportViewModel> ReceiptDetailReportList { get; set; }
        public Guid StoreId { get; set; }
        public Guid ProductId { get; set; }
        public List<SelectListItem> ProductList { get; set; }
    }
}
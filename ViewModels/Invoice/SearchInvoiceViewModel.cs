using System;
using DrugStockWeb.Models.Account;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DrugStockWeb.Helper;
using DrugStockWeb.Models;

namespace DrugStockWeb.ViewModels.Invoice
{
    public class SearchInvoiceViewModel
    {

        [DisplayName("عنوان")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string Title { get; set; }
        public long FactorNumber { get; set; }
        public string BusinnessPartnerId { get; set; }
        public string HamrahName { get; set; }
        public string FactorDateFrom { get; set; }
        public string FactorDateTo { get; set; }
        // public long FactorNumber { get; set; }
        public int Page { get; set; }

    }
}
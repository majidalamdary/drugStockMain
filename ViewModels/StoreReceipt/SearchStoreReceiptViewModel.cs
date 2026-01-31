using System;
using DrugStockWeb.Models.Account;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DrugStockWeb.Helper;
using DrugStockWeb.Models;

namespace DrugStockWeb.ViewModels.StoreReceipt
{
    public class SearchStoreReceiptViewModel
    {



        [DisplayName("عنوان")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string Title { get; set; }
        public string FactorNumber { get; set; }
        public string ReceiptNumber { get; set; }
        public string RequestNumber { get; set; }
        public string BusinnessPartnerId { get; set; }
        public int Page { get; set; }

    }
}
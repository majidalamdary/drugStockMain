using System;
using DrugStockWeb.Models.Account;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DrugStockWeb.Helper;
using DrugStockWeb.Models;

namespace DrugStockWeb.ViewModels.Store
{
    public class SearchStoreViewModel
    {



        [DisplayName("نام انبار")]
        [Required(ErrorMessage = "لطفا {0} را مشخص کنید")]
        public string Title { get; set; }

        [DisplayName("مسئول انبار")]
        public string InCharge { get; set; }

        public int Page { get; set; }

    }
}
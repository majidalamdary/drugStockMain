using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DrugStockWeb.ViewModels.Account
{
    public class SearchUserViewModel
    {

        public string LastName { get; set; }
        public int Page { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrugStockWeb.ViewModels.Home
{
    public class HomePageViewModel
    {

        public HomePageViewModel() { }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string Role { get; set; }
    }
}
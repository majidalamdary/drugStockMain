using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DrugStockWeb.ViewModels.StoreReport
{
    public class StockBalanceViewModel
    {

        public StockBalanceViewModel()
        {

        }
        public List<SelectListItem> StoreList { get; set; }
        public List<StockBalanceProductListViewModel>    ProductList { get; set; }
        public Guid StoreId { get; set; }
        public List<SelectListItem> DisposableStoreList { get; set; }
        public Guid DisposableStoreId { get; set; }
    }
}
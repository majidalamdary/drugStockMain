using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrugStockWeb.ViewModels.StoreReport
{
    public class StockBalanceProductListViewModel
    {

        public string ProductTitle { get; set; }
        public Guid ProductId { get; set; }
        public string ProductGenericCode { get; set; }
        public string ProductGroupTitle { get; set; }
        public string StoreTitle { get; set; }
        public Guid DisposableStoreId { get; set; }
        public long RemainingCount { get; set; }
        public long RemainingPrice { get; set; }
    }
}
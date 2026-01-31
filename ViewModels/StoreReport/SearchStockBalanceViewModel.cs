using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrugStockWeb.ViewModels.StoreReport
{
    public class SearchStockBalanceViewModel
    {
        public Guid StoreId { get; set; }
        public Guid DispoasableStoreId { get; set; }
        public int Page { get; set; }
    }
}
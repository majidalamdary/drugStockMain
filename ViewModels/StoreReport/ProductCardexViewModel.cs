using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrugStockWeb.ViewModels.StoreReport
{
    public class ProductCardexViewModel
    {
        public string Sanad;


        public DateTime  Date { get; set; }
        public string  ShamsiDate { get; set; }
        public string Description { get; set; }
        public string ActionType { get; set; }
        public long Count { get; set; }
        public long Remainig { get; set; }
        public string Type { get; set; }
        public Guid ReferenceId { get; set; }

        public ProductCardexViewModel()
        {

        }
    }
}
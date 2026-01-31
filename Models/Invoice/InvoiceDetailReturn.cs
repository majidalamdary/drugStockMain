using DrugStockWeb.Helper;
using DrugStockWeb.Models.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrugStockWeb.Models.Invoice
{
    public class InvoiceDetailReturn
    {
        public InvoiceDetailReturn()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }


        public Guid InvoiceDetailId { get; set; }
        public virtual InvoiceDetail InvoiceDetail { get; set; }
        public Guid InvoiceReturnId { get; set; }
        public virtual InvoiceReturn InvoiceReturn { get; set; }
        public long Count { get; set; }



    }
}
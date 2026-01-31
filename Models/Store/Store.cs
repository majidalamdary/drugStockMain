using System;
using DrugStockWeb.Models.Account;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DrugStockWeb.Helper;

namespace DrugStockWeb.Models.Store
{
    public class Store
    {
        public Store()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string InCharge { get; set; }

        public bool IsForDisposable { get; set; }
        public bool IsUsagePeriodForce { get; set; }
        public int CityId { get; set; }
        public string CreatorUserId { get; set; }
        public virtual User CreatorUser { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public virtual City Cities { get; set; }
        public virtual ICollection<Product.Product> Products { get; set; }
        public virtual ICollection<StoreInUser> StoreInUsers { get; set; }
        public virtual ICollection<Invoice.Invoice> Invoices { get; set; }
        public virtual ICollection<Invoice.InvoiceReturn> InvoiceReturns { get; set; }
        public virtual ICollection<StoreReceipt.StoreReceipt> StoreReceipts { get; set; }

    }
}
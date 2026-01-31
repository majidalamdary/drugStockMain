using DrugStockWeb.Helper;
using DrugStockWeb.Models.Account;
using DrugStockWeb.Models.BusinessPartner;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DrugStockWeb.Models.Invoice
{
    public class Invoice
    {
        public Invoice()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }
        [Required]
        public long FactorNumber { get; set; }
        public long UsagePeriod { get; set; }
        [Required]
        public DateTime FactorDate { get; set; }


        [Required]
        public Boolean IsConfirmed { get; set; }
        [Required]
        public Boolean IsAccountingConfirmed { get; set; }

        public DateTime? AccountingConfirmTime  { get; set; }
        public string Describe { get; set; }
        public string ReceiverFullName { get; set; }
        public string ReceiverMobile { get; set; }

        public Guid BusinnessPartnerId { get; set; }
        public virtual BusinnessPartner BusinnessPartner { get; set; }
        public Guid StoreId { get; set; }
        public virtual Store.Store Store { get; set; }



        public string CreatorUserId { get; set; }
        public virtual User CreatorUser { get; set; }
        public string ConfirmerUserId { get; set; }
        public virtual User ConfirmerUser { get; set; }
        public string AccountingConfirmerUserId { get; set; }
        public virtual User AccountingConfirmerUser { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public ICollection<InvoiceDetail> InvoiceDetails { get; set; }
        public ICollection<InvoiceReturn> InvoiceReturns{ get; set; }

    }
}
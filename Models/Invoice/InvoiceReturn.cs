using DrugStockWeb.Helper;
using DrugStockWeb.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DrugStockWeb.Models.Invoice
{
    public class InvoiceReturn
    {
        public InvoiceReturn()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }

        [Required]
        public DateTime InvoiceReturnDate { get; set; }


        [Required]
        public Boolean IsConfirmed { get; set; }

        public string Describ { get; set; }

        public string CreatorUserId { get; set; }
        public virtual User CreatorUser { get; set; }

        public string DisposerUserId { get; set; }
        public virtual User DisposerUser { get; set; }
        public string ConfirmerUserId { get; set; }
        public virtual User ConfirmerUser { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public bool IsDisposed { get; set; }
        public DateTime DisposalDate { get; set; }
        public Guid StoreId { get; set; }
        public Store.Store Store { get; set; }

        public Guid InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }
        public ICollection<InvoiceDetailReturn> InvoiceDetailReturns { get; set; }

    }
}
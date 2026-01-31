using DrugStockWeb.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DrugStockWeb.Models.BusinessPartner;
using DrugStockWeb.Models.Account;
using DrugStockWeb.Models.Invoice;

namespace DrugStockWeb.Models.StoreReceipt
{
    public class StoreReceipt
    {
        public StoreReceipt()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }
        
        [Required]
        public long ReceiptNumber { get; set; }
        public string Title { get; set; }

        public string RequestNumber { get; set; }

        [Required]
        public string FactorNumber { get; set; }
        [Required]
        public DateTime FactorDate { get; set; }

        [Required]
        public DateTime ReceiptDate { get; set; }


        [Required]
        public Boolean IsConfirmed { get; set; }

        public string Describ { get; set; }

        public Guid StoreId { get; set; }
        public virtual Store.Store Store { get; set; }


        public Guid BusinnessPartnerId { get; set; }
        public virtual BusinnessPartner BusinnessPartner { get; set; }

        public string CreatorUserId { get; set; }
        public virtual User CreatorUser { get; set; }
        public string ConfirmerUserId { get; set; }
        public virtual User ConfirmerUser { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public virtual ICollection<StoreReceiptDetail> StoreReceiptDetails { get; set; }
        public virtual ICollection<StoreReceiptReturn> StoreReceiptReturns { get; set; }






    }
}
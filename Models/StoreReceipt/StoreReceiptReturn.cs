using DrugStockWeb.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DrugStockWeb.Models.Account;
namespace DrugStockWeb.Models.StoreReceipt
{
    public class StoreReceiptReturn
    {
        public StoreReceiptReturn()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }
        
        [Required]
        public DateTime ReceiptReturnDate { get; set; }


        [Required]
        public Boolean IsConfirmed { get; set; }

        public string Describ { get; set; }

        public string CreatorUserId { get; set; }
        public virtual User CreatorUser { get; set; }
        public string ConfirmerUserId { get; set; }
        public virtual User ConfirmerUser { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public Guid StoreReceiptId { get; set; }
        public virtual StoreReceipt StoreReceipt { get; set; }

        public ICollection<StoreReceiptDetailReturn> StoreReceiptDetailReturns { get; set; }






    }
}
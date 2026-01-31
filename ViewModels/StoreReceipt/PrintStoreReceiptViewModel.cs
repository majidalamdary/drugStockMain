using System;
using DrugStockWeb.Models.Account;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DrugStockWeb.Helper;

namespace DrugStockWeb.ViewModels.StoreReceipt
{
    public class PrintStoreReceiptViewModel
    {
        public PrintStoreReceiptViewModel()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }

        public Guid? Id { get; set; }

        public long ReceiptNumber { get; set; }

        public string Title { get; set; }
        public string RequestNumber { get; set; }
        public string FactorNumber { get; set; }

        public string Describ { get; set; }
        public string FactorDateShamsi { get; set; }
        public DateTime FactorDate { get; set; }

        public int IsNew { get; set; }
        public bool IsConfirmed { get; set; } = false;

        public string ReceiptDateShamsi { get; set; }
        public DateTime ReceiptDate { get; set; }

        public Guid BusinnessPartnerId { get; set; }



        public Guid StoreId { get; set; }
        public string StoreTitle { get; set; }


        public string BusinnessPartnerName { get; set; }
        public string BusinnessPartnerMobile { get; set; }
        public string BusinnessPartnerTel { get; set; }
        public string BusinnessPartnerAddress { get; set; }
        public string BusinnessPartnerMelliCode { get; set; }
        public List<PrintStoreReceiptDetailViewModel> ListStoreReceiptDetail { get; set; }


    }
}
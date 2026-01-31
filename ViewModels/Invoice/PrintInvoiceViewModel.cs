
using System;
using DrugStockWeb.Models.Account;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DrugStockWeb.Helper;

namespace DrugStockWeb.ViewModels.Invoice
{
    public class PrintInvoiceViewModel
    {
        public PrintInvoiceViewModel()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }

        public Guid? Id { get; set; }

        public string Title { get; set; }

        public long FactorNumber { get; set; }

        public long UsagePeriod { get; set; }

        public string Describ { get; set; }



        public string ReceiverFullName { get; set; }
        public string ReceiverMobile { get; set; }



        public string FactorDateShamsi { get; set; }
        public Guid StoreId { get; set; }
        public DateTime FactorDate { get; set; }

        public int IsNew { get; set; }


        public Guid BusinnessPartnerId { get; set; }

        public string BusinnessPartnerName { get; set; }
        public string BusinnessPartnerMobile { get; set; }
        public string BusinnessPartnerTel { get; set; }
        public string BusinnessPartnerAddress { get; set; }
        public string BusinnessPartnerEconomicalCode { get; set; }
        public string BusinnessPartnerMelliCode { get; set; }
        public List<PrintInvoiceDetailViewModel> DetailLists { get; set; }
    }
}
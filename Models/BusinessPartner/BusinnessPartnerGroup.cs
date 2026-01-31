using DrugStockWeb.Helper;
using DrugStockWeb.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DrugStockWeb.Models.BusinessPartner
{
    public class BusinnessPartnerGroup
    {
        public BusinnessPartnerGroup()
        {
            Id = SequentialGuidGenerator.NewSequentialGuid();
        }
        public Guid Id { get; set; }
        
        [Required]
        public string Title { get; set; }

        [Required]
        public BusinnessPartnerStatus BusinnessPartnerStatusId { get; set; }
        [Required]

        public Boolean SellWithBuyPrice { get; set; }
        public string CreatorUserId { get; set; }
        public virtual User CreatorUser { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public virtual ICollection<BusinnessPartner> BusinnessPartners{ get; set; }
        public int BusinnessPartnerLegalTypeId { get; set; }
        public virtual BusinnessPartnerLegalType BusinnessPartnerLegalType { get; set; }

    }
}
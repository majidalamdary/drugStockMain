using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrugStockWeb.Models.BusinessPartner
{
    public class BusinnessPartnerLegalType
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual ICollection<BusinnessPartnerGroup> BusinnessPartnerGroups { get; set; }
    }
}
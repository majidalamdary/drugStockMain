using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DrugStockWeb.Models.BusinessPartner
{
    public enum BusinnessPartnerStatus
    {
        [Display(Name = "خریدار")]
        خریدار = 1,
        [Display(Name = "فروشنده")]
         فروشنده= 2,
    }
}
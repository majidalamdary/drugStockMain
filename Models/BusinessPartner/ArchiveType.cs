using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DrugStockWeb.Models.BusinessPartner
{
    public enum ArchiveType
    {
        [Display(Name = "فعال")]
        Active = 1,
        [Display(Name = "بهبود یافته")]
        Recovered = 2,
        [Display(Name = "فوت شده")]
        PassedAway = 3,
    }
}
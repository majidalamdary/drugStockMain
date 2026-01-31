using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrugStockWeb.ViewModels.Store;

namespace DrugStockWeb.Models.Constancts
{
    public enum UserStatusValue
    {
        Activated = 1,
        UnActivated = 2,
        Blocked = 3,
        TempBlocked = 4
    }
}
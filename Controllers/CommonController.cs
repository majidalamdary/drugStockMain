using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DrugStockWeb.Controllers
{
    public class CommonController : MainController
    {
        // GET: Common
        [HttpPost]
        public JsonResult GetCities(int id)
        {
            var temp = Db.Cities.Where(p => p.ProvinceId == id).OrderBy(p => p.Name).ToList();
            var result = temp.Select(variable => new SelectListItem() { Text = variable.Name, Value = variable.Id.ToString() })
                    .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
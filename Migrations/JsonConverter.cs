using DrugStockWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DrugStockWeb.Models.CityAndProvince;
using Newtonsoft.Json;
namespace DrugStockWeb.Migrations
{
    public static class JsonConverter
    {
        public static IEnumerable<Province> ToOstanList(this string json)
        {
            return JsonConvert.DeserializeObject<IEnumerable<Province>>(json);
        }
        public static IEnumerable<City> ToCityList(this string json)
        {
            return JsonConvert.DeserializeObject<IEnumerable<City>>(json);
        }
    }
}
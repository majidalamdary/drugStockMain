using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DrugStockWeb.Models.Account;
using DrugStockWeb.Models.CityAndProvince;
using Newtonsoft.Json;

namespace DrugStockWeb.Models
{
    public class City
    {
        public City()
        {

        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int ProvinceId { get; set; }

        public virtual Province Province { get; set; }

        [JsonIgnore]
        public virtual ICollection<Store.Store> Stores { get; set; }

        

    }
}
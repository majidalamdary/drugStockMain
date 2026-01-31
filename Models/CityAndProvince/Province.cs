using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DrugStockWeb.Models.Account;

namespace DrugStockWeb.Models.CityAndProvince
{
    public class Province
    {
        public Province()
        {

        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        
        public virtual ICollection<City> Cities { get; set; }

        
    }
}
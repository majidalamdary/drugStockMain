using DrugStockWeb;
using Microsoft.Owin;
using Owin;
using System.Web.Hosting;
using System;
using DrugStockWeb.Models;
using System.Data.Entity;
using DrugStockWeb.Migrations;

[assembly: OwinStartup(typeof(Startup))]
namespace DrugStockWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
             
            ConfigureAuth(app);  ////wefwef//123415623e23123

        }

    }
}

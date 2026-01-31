using System.Web.Optimization;

namespace DrugStockWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {


            BundleTable.EnableOptimizations = false;

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                 "~/Scripts/core.min.js",
                 "~/Scripts/noc.js" // ← تایپ درست
             ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js"
                      )
                );

            bundles.Add(new ScriptBundle("~/bundles/menu").Include(
                      "~/Scripts/menu.js"));

            bundles.Add(new ScriptBundle("~/bundles/MyScripts").Include(
                      "~/Scripts/global.min.js",
                      "~/Scripts/custom.min.js",
                      "~/Scripts/select2/js/select2.js",
                      "~/Scripts/sweetalert2.all.min.js",
                      "~/Scripts/persianDatepicker.js",
                      "~/Scripts/fontAwesome/js/all.js",
                      "~/Scripts/MyScript.js"

                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                       "~/Content/bootstrap.rtl.min.css",
                       "~/Content/select2/css/select2.css",
                       // "~/Content/components.css",
                       "~/Content/sweetalert2.min.css",
                       "~/Content/persianDatepicker-default.css",
                      "~/Content/style-new.css"));
            bundles.Add(new StyleBundle("~/Content/login_css").Include(

                      "~/Content/Account/css/font-awesome.min.css",
                      "~/Content/Account/css/util.css",
                      "~/Content/Account/css/bootstrap.min.css",
            "~/Content/Account/css/main_login.css"));
        }
    }
}

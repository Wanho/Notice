using System.Web;
using System.Web.Optimization;

namespace Notice.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/Jquery/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/Jquery/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/Jquery/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/BootStrap/bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/FontAwesome/all.min.css",
                      "~/Content/Bootstrap/Theme/sb-admin-2.min.css",
                      "~/Content/Bootstrap/Theme/dataTables.bootstrap4.min.css",
                      "~/Content/Page/Site.css"));
        }
    }
}

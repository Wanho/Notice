using System.Web;
using System.Web.Optimization;

namespace Notice.Web
{
    public class BundleConfig
    {
        // 묶음에 대한 자세한 내용은 https://go.microsoft.com/fwlink/?LinkId=301862를 참조하세요.
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/Jquery/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/Jquery/jquery.validate*"));

            // Modernizr의 개발 버전을 사용하여 개발하고 배우십시오. 그런 다음
            // 프로덕션에 사용할 준비를 하고 https://modernizr.com의 빌드 도구를 사용하여 필요한 테스트만 선택하세요.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/Jquery/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/BootStrap/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/vue").Include(
                      "~/Scripts/Vue/vue.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //"~/Content/bootstrap.css",
                      //"~/Content/bootstrap-grid.min.css",
                      //"~/Content/bootstrap-reboot.min.css",
                      "~/Content/FontAwesome/all.min.css",
                      "~/Content/Bootstrap/Theme/sb-admin-2.min.css",
                      "~/Content/Bootstrap/Theme/dataTables.bootstrap4.min.css",
                      "~/Content/Page/Site.css"));
        }
    }
}

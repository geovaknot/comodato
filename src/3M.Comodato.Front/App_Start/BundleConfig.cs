using System.Web;
using System.Web.Optimization;

namespace _3M.Comodato.Front
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //#if DEBUG == false
            //            // Habilita a otimização do Bundle (juntar arquivos) e Minification (diminuir tamanho) quando compilar para RELEASE
            //            BundleTable.EnableOptimizations = true;
            //#endif 
            bundles.Add(new Bundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.moneymask.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js",
                        "~/Scripts/jquery.mask.js",
                        "~/Scripts/jquery.cookie.js",
                        "~/Scripts/jquery.freezeheader.js",
                        "~/Scripts/jquery.freezeheader-Dash.js",
                        "~/Scripts/select2.js",
                        "~/Scripts/bootstrap-datepicker.js",
                        "~/Scripts/locales/bootstrap-datepicker.pt-BR.min.js",
                        "~/Scripts/moment.js",
                        "~/Scripts/chart.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      //"~/Scripts/bootstrap.bundle.js",
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/css/select2.css",
                      "~/Content/bootstrap-datepicker3.css",
                      "~/Content/fontawesome-all.css"));


            
        }
    }
}

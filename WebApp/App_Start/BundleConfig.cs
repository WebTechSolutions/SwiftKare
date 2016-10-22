using System.Web;
using System.Web.Optimization;

namespace WebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/NewCss").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/nprogress.css",
                      "~/Content/css/green.css",
                      "~/Content/css/bootstrap-progressbar-3.3.4.min.css",
                      "~/Content/css/jqvmap.min.css",
                      "~/Content/css/animate.min.css",
                      "~/Content/css/custom.min.css",
                      "~/Content/css/style.css"

                      ));


            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                     "~/Scripts/js/jquery.min.js",

                     "~/Scripts/js/bootstrap.min.js",
                     "~/Scripts/js/fastclick.js",
                     "~/Scripts/js/nprogress.js",
                     "~/Scripts/js/Chart.min.js",
                     "~/Scripts/js/gauge.min.js",
                     "~/Scripts/js/bootstrap-progressbar.min.js",
                     "~/Scripts/js/icheck.min.js",
                     "~/Scripts/js/skycons.js",
                    

                     "~/Scripts/js/jquery.flot.js",
                     "~/Scripts/js/jquery.flot.pie.js",
                     "~/Scripts/js/jquery.flot.time.js",
                     "~/Scripts/js/jquery.flot.stack.js",
                     "~/Scripts/js/jquery.flot.resize.js",


                     "~/Scripts/js/jquery.flot.orderBars.js",

                     "~/Scripts/js/jquery.flot.spline.js",

                     "~/Scripts/js/curvedLines.js",

                     "~/Scripts/js/date.js",
                     "~/Scripts/js/jquery.vmap.js",
                     "~/Scripts/js/jquery.vmap.world.js",
                     "~/Scripts/js/jquery.vmap.sampledata.js",
                     "~/Scripts/js/moment/moment.min.js",
                     "~/Scripts/js/datepicker/daterangepicker.js",
                     
                     "~/Scripts/js/custom.min.js"));




            //-----------------------
            bundles.Add(new ScriptBundle("~/bundles/content/js").Include(
                    "~/Content/js/jquery.min.js",

                    "~/Content/js/bootstrap.min.js",
                    "~/Content/js/fastclick.js",
                    "~/Content/js/nprogress.js",
                    "~/Content/js/Chart.min.js",
                    "~/Content/js/gauge.min.js",
                    "~/Content/js/bootstrap-progressbar.min.js",
                    "~/Content/js/icheck.min.js",
                    "~/Content/js/skycons.js",


                    "~/Content/js/jquery.flot.js",
                    "~/Content/js/jquery.flot.pie.js",
                    "~/Content/js/jquery.flot.time.js",
                    "~/Content/js/jquery.flot.stack.js",
                    "~/Content/js/jquery.flot.resize.js",


                    "~/Content/js/jquery.flot.orderBars.js",

                    "~/Content/js/jquery.flot.spline.js",

                    "~/Content/js/curvedLines.js",

                    "~/Content/js/date.js",
                    "~/Content/js/jquery.vmap.js",
                    "~/Content/js/jquery.vmap.world.js",
                    "~/Content/js/jquery.vmap.sampledata.js",
                    "~/Content/js/moment/moment.min.js",
                    "~/Content/js/datepicker/daterangepicker.js",

                    "~/Content/js/custom.min.js"));




        }
    }
}

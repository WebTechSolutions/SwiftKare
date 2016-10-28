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
                       "~/Scripts/font-awesome/css/font-awesome.min.css",
                      "~/Content/css/nprogress.css",
                      "~/Content/css/green.css",
                      "~/Content/css/bootstrap-progressbar-3.3.4.min.css",
                      "~/Content/css/jqvmap.min.css",
                      "~/Content/css/animate.min.css",
                      "~/Content/css/dataTables.bootstrap.min.css",
                      "~/Content/css/switchery.min.css",
                      "~/Content/css/responsive.bootstrap.min.css",
                      "~/Content/css/buttons.bootstrap.min.css",
                      "~/Content/css/scroller.bootstrap.min.css",
                      "~/Content/css/select2.min.css",
                     "~/Content/css/starrr.css",
                      "~/Content/css/dropzone.min.css",
                      "~/Content/css/custom.min.css",
                      "~/Content/css/style.css",
                      "~/Content/css/bootstrap-material-datetimepicker.css"





                      ));


            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                     "~/Scripts/jquery.min.js",
                     "~/Scripts/bootstrap.min.js",
                     "~/Scripts/fastclick.js",
                     "~/Scripts/nprogress.js",
                     "~/Scripts/Chart.min.js",
                     "~/Scripts/gauge.min.js",
                     "~/Scripts/bootstrap-progressbar.min.js",
                     "~/Scripts/icheck.min.js",
                    "~/Scripts/jquery.dataTables.min.js",
                     "~/Scripts/dataTables.bootstrap.min.js",
                       "~/Scripts/dataTables.buttons.min.js",
                     "~/Scripts/dataTables.responsive.min.js",
                      "~/Scripts/responsive.bootstrap.js",
                    "~/Scripts/dataTables.fixedHeader.min.js",
                      "~/Scripts/dataTables.keyTable.min.js",
                    "~/Scripts/jquery.flot.js",
                     "~/Scripts/jquery.flot.pie.js",
                     "~/Scripts/jquery.flot.time.js",
                     "~/Scripts/jquery.flot.stack.js",
                     "~/Scripts/jquery.flot.resize.js",
                     "~/Scripts/jquery.flot.orderBars.js",
                    "~/Scripts/jquery.flot.spline.js",
                    "~/Scripts/curvedLines.js",
                    "~/Scripts/date.js",
                    "~/Scripts/moment.min.js",
                     "~/Scripts/select2.full.min.js",
                     "~/Scripts/daterangepiker/daterangepicker.js",
                     "~/Scripts/switchery.min.js",
                    "~/Scripts/toggle.js",
                     "~/Scripts/validator.js",
                     "~/Scripts/jquery.smartWizard.js",
                       "~/Scripts/dropzone.min.js",
                   "~/Scripts/jquery.autocomplete.min.js",
                   "~/Scripts/jquery.starrr.js",
                    "~/Scripts/bootstrap-material-datetimepicker.js",
                     "~/Scripts/material.min.js",
                       "~/Scripts/moment-with-locales.min.js",
                    "~/Scripts/custom.min.js"));
            

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

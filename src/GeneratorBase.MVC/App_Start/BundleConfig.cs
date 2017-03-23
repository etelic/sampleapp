using System.Web;
using System.Web.Optimization;
namespace GeneratorBase.MVC
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                       "~/Scripts/jquery.validate*",
                       "~/Scripts/jquery.unobtrusive*"));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*",
                          "~/Scripts/respond.js"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));
            bundles.Add(new ScriptBundle("~/bundles/common1").Include(
           "~/Scripts/Common1/responsive1.js",
           "~/Scripts/Common1/analytics.js",
           "~/Scripts/Common1/html5shiv.js",
           "~/Scripts/Common1/custom-forms.js",
           "~/Scripts/Common1/bootstrap-multiselect.js",
           "~/Scripts/Common1/bwizard.js",
            "~/Scripts/Common1/jquery.cookie.js",
            "~/Scripts/Common1/jquery.are-you-sure.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/common2").Include(
           "~/Scripts/Common2/responsive2.js",
           "~/Scripts/Common2/jquery.are-you-sure.js",
           "~/Scripts/Common2/moment-2.4.0.js",
           "~/Scripts/Common2/bootstrap-datetimepicker.js",
           "~/Scripts/Common2/jquery.metisMenu.js",
            "~/Scripts/Common2/dashboard.js",
            "~/Scripts/Common2/bootstrap-multiselect.js",
            "~/Scripts/Common2/bwizard.js",
 	    "~/Scripts/Common2/jquery.prettyPhoto.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/common3").Include(
            "~/Scripts/Common3/jquery.maskedinput.js",
            "~/Scripts/Common3/jquery.ba-throttle-debounce.min.js",
            "~/Scripts/Common3/chosen.jquery.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap-datetimepicker.min.css",
                      "~/Content/bootstrap-multiselect.css",
                      "~/Content/bootstrap.css",
                       "~/Content/bootstrap.min.css",
                       "~/Content/chosen.css",
                       "~/Content/loading.css",
                       "~/Content/Site.css"));
            //font-awesome
            bundles.Add(new StyleBundle("~/Content/fontawesome").Include(
                      "~/Content/font-awesome/css/*.css"));
            bundles.Add(new StyleBundle("~/Content/csstheme").Include(
                     "~/Content/themes/base/*.css"));
            //BundleTable.EnableOptimizations = true;
        }
    }
}

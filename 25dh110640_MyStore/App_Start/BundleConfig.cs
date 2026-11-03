using System.Web.Optimization;

namespace MyStore
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/Site.css",
                      "~/Content/myStyle/CustomerHome.css",
                      "~/Content/myStyle/CustomerDetailProduct.css"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                      "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/myscript").Include(
                      "~/Scripts/MyStore.js"));

            BundleTable.EnableOptimizations = false;
        }
    }
}

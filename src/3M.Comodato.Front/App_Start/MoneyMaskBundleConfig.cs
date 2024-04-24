using System.Web.Optimization;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(_3M.Comodato.Front.App_Start.MoneyMaskBundleConfig), "RegisterBundles")]

namespace _3M.Comodato.Front.App_Start
{
	public class MoneyMaskBundleConfig
	{
		public static void RegisterBundles()
		{
			BundleTable.Bundles.Add(new ScriptBundle("~/bundles/moneymask").Include("~/Scripts/jquery.moneymask.js"));
		}
	}
}
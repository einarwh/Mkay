using System.Web.Mvc;
using System.Web.Optimization;
using Mkay;

[assembly: WebActivatorEx.PostApplicationStartMethod(
    typeof($rootnamespace$.App_Start.MkayBoot), "Kick")]

namespace $rootnamespace$.App_Start 
{
    public static class MkayBoot 
    {
        public static void Kick() 
        {
            // Create script bundle for Mkay.
            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/mkay").Include("~/Scripts/mkay*"));

            // Hook up MkayAttribute to use its own validator which sets ValidationContext.MemberName.
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(MkayAttribute), typeof(MkayValidator));
        }
    }
}
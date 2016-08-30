using System.Web.Mvc;

namespace Iata.IS.Web.Areas.MiscPayables
{
  public class MiscPayablesAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "MiscPayables";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "MiscPayables_default",
          "MiscPayables/{controller}/{action}/{id}",
          new { action = "Index", id = UrlParameter.Optional }
      );

      context.MapRoute("MiscPayableManageSupportingDocuments", "MiscPayables/{billingType}/MiscPayableSupportingDoc/{action}", new { area = "MiscPayables", controller = "MiscPayableSupportingDoc", billingType = "Payables" });
    }
  }
}

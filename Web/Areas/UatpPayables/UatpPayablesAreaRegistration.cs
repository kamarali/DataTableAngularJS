using System.Web.Mvc;

namespace Iata.IS.Web.Areas.UatpPayables
{
  public class UatpPayablesAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
          return "UatpPayables";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "UatpPayables_default",
          "UatpPayables/{controller}/{action}/{id}",
          new { action = "Index", id = UrlParameter.Optional }
      );

      context.MapRoute("UatpPayableManageSupportingDocuments", "UatpPayables/{billingType}/UatpPayablesSupportingDoc/{action}", new { area = "UatpPayables", controller = "UatpPayablesSupportingDoc", billingType = "Payables" });
    }
  }
}

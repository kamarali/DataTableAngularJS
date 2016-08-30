using System.Web.Mvc;

namespace Iata.IS.Web.Areas.CargoPayables
{
  public class CargoPayablesAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
          return "CargoPayables";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
        //context.MapRoute("CargoPDownLoadFile", "CargoPayables/{billingType}/Invoice/DownloadFile/{id}", new { area = "CargoPayables", billingType = "Payables", action = "DownloadFile", controller = "Invoice" });
      
      context.MapRoute(
          "CargoPayables_default",
          "CargoPayables/{billingType}/{controller}/{action}/{id}",
          new { action = "Index", id = UrlParameter.Optional, billingType = "Payables" }
      );
      context.MapRoute("CGOPInvoiceSearchs", "CargoPayables/{billingType}/PayablesInvoiceSearch/{action}/{id}", new { area = "CargoPayables", billingType = "Payables", controller = "PayablesInvoiceSearch", id = UrlParameter.Optional });
      //context.MapRoute("Cargo_default", "Cargo/{billingType}/{controller}/{action}/{id}", new { area = "Cargo", billingType = "Payables", action = "Index", id = UrlParameter.Optional });
      //context.MapRoute("PayablesManageSupportingDocuments", "CargoPayables/{billingType}/SupportingDoc/{action}", new { area = "CargoPayables", controller = "SupportingDoc", billingType = "Payables" });
    }
  }
}

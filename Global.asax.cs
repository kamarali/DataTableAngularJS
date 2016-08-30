using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Devart.Data.Oracle;
using Iata.IS.Core.DI;
using Iata.IS.Core.Logging;
using Iata.IS.Framework.UserInterface;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Common;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.BillingHistory;
using Iata.IS.Web.Areas.General.Controllers;
using Iata.IS.Web.Util.ModelBinders.Cargo;
using PaxBillingHistory = Iata.IS.Model.Pax.BillingHistory;
using Iata.IS.Model.Pax;
using Iata.IS.Web.Util.ModelBinders;
using Iata.IS.Web.Util.ModelBinders.Misc;
using MiscModelBinders = Iata.IS.Web.Util.ModelBinders.Misc;
using PaxModelBinders = Iata.IS.Web.Util.ModelBinders.Pax;
using CargoModelBinders = Iata.IS.Web.Util.ModelBinders.Cargo;
using log4net;
using Iata.IS.Model.Pax.Sampling;
using MvcSiteMapProvider.Web;
using InvoiceModelBinder = Iata.IS.Web.Util.ModelBinders.InvoiceModelBinder;
using InvoiceSearchModelBinder = Iata.IS.Web.Util.ModelBinders.InvoiceSearchModelBinder;
using Iata.IS.Model.SupportingDocuments;
using RejectionMemo = Iata.IS.Model.Pax.RejectionMemo;
using SearchCriteria = Iata.IS.Model.Pax.SearchCriteria;
using CargoSearchCriteria = Iata.IS.Model.Cargo.SearchCriteria;

namespace Iata.IS.Web
{
  // For instructions on enabling IIS6 or IIS7 classic mode, visit http://go.microsoft.com/?LinkId=9394801
  public class Application : HttpApplication
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("elmah.axd");
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
      routes.IgnoreRoute("{resource}.aspx/{*pathInfo}");
      routes.IgnoreRoute("favicon.ico");

      routes.MapRoute(
        "Default", // Route name
        "{controller}/{action}/{id}", // URL with parameters
        new
        {
          controller = "Account",
          action = "LogOn",
          id = UrlParameter.Optional
        } // Parameter defaults
        );
    }

    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
    }

    private static void RegisterModelBinders()
    {
      ModelBinders.Binders[typeof(PaxInvoice)] = new InvoiceModelBinder();
      ModelBinders.Binders[typeof(SearchCriteria)] = new InvoiceSearchModelBinder();
      ModelBinders.Binders[typeof(CMCoupon)] = new CMCouponModelBinder();
      ModelBinders.Binders[typeof(BMCoupon)] = new BMCouponModelBinder();
      ModelBinders.Binders[typeof(PrimeCoupon)] = new CouponRecordModelBinder();
      ModelBinders.Binders[typeof(CreditMemo)] = new CreditMemoModelBinder();
      ModelBinders.Binders[typeof(BillingMemo)] = new BMModelBinder();
      ModelBinders.Binders[typeof(RejectionMemo)] = new RMModelBinder();
      ModelBinders.Binders[typeof(RMCoupon)] = new RMCouponModelBinder();
      ModelBinders.Binders[typeof(SamplingFormC)] = new FormCModelBinder();
      ModelBinders.Binders[typeof(SamplingFormCRecord)] = new FormCRecordModelBinder();
      ModelBinders.Binders[typeof(SamplingFormDRecord)] = new FormDModelBinder();

      // Misc. Model binders.
      ModelBinders.Binders[typeof(MiscUatpInvoice)] = new MiscModelBinders.InvoiceModelBinder();
      ModelBinders.Binders[typeof(MiscSearchCriteria)] = new MiscModelBinders.InvoiceSearchModelBinder();
      ModelBinders.Binders[typeof(InvoiceSearchCriteria)] = new MiscModelBinders.BillingHistoryModelBinder();
      ModelBinders.Binders[typeof(PaxBillingHistory.InvoiceSearchCriteria)] = new PaxModelBinders.BillingHistoryModelBinder();
      
      ModelBinders.Binders[typeof(MiscCorrespondence)] = new MiscModelBinders.CorrespondenceModelBinder();
      ModelBinders.Binders[typeof(Model.Pax.Correspondence)] = new PaxModelBinders.CorrespondenceModelBinder();
      ModelBinders.Binders[typeof(CargoCorrespondence)] = new CargoCorrespondenceModelBinder();

      ModelBinders.Binders[typeof(LineItem)] = new LineItemModelBinder();
      ModelBinders.Binders[typeof(LineItemDetail)] = new LineItemDetailModelBinder();
      
      // Unlinked Supporting document binder
      ModelBinders.Binders[typeof(UnlinkedSupportingDocumentEx)] = new UnlinkedSupportingDocumentModelBinder();

      ModelBinders.Binders[typeof(SupportingDocSearchCriteria)] = new PaxModelBinders.SupportingDocumentModelBinder();

      //Cargo Model binders.
      ModelBinders.Binders[typeof(CargoInvoice)] = new CargoModelBinders.InvoiceModelBinder();
      ModelBinders.Binders[typeof(CargoSearchCriteria)] = new CargoModelBinders.InvoiceSearchModelBinder();
      ModelBinders.Binders[typeof(PayableSearch)] = new CargoModelBinders.InvoicePayableSearchModelBinder();
      ModelBinders.Binders[typeof(CargoBillingMemo)] = new CargoBMModelBinder();
      ModelBinders.Binders[typeof(CargoBillingMemoAwb)] = new CargoBMAwbModelBinder();
      ModelBinders.Binders[typeof(AwbRecord)] = new CargoModelBinders.CargoAWBModelBinder();
      ModelBinders.Binders[typeof(CargoRejectionMemo)] = new CargoRMModelBinder();
      ModelBinders.Binders[typeof(Model.Cargo.BillingHistory.InvoiceSearchCriteria)] = new CargoModelBinders.BillingHistoryModelBinder();
      ModelBinders.Binders[typeof (RMAwb)] = new CargoModelBinders.RMAwbModelBinder();
      ModelBinders.Binders[typeof(ValidationErrorCorrection)] = new ValidationErrorCorrectionModelBinder();
      ModelBinders.Binders[typeof(CargoCreditMemo)] = new CargoCreditMemoModelBinder();
      ModelBinders.Binders[typeof(CMAirWayBill)] = new CargoCMAwbModelBinder();
    }

    protected void Application_BeginRequest()
    {
        Response.AddHeader("X-Frame-Options", "SAMEORIGIN");
    }

    protected void Application_PreSendRequestHeaders()
    {
        if (ConfigurationManager.AppSettings.Get("DebugSourceCode") == "1") return;
        Response.Headers.Remove("Server");
        Response.Headers.Remove("X-AspNet-Version");
        Response.Headers.Remove("X-AspNetMvc-Version");
    }


    protected void Application_Start()
    {
      // Initialize global context for logging.
      LoggingManager.InitializeGlobalContext();

      AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;

      // Register all the areas in the application.
      AreaRegistration.RegisterAllAreas();
      Logger.Info("Registered all areas.");

      RegisterGlobalFilters(GlobalFilters.Filters);

      // Register the routes for the application.
      RegisterRoutes(RouteTable.Routes);
      Logger.Info("Registered routes.");

      // Register XmlSiteMapController
      XmlSiteMapController.RegisterRoutes(RouteTable.Routes);

      // Register model binders.
      RegisterModelBinders();
      Logger.Info("Registered model binders.");

      // Initialize the inversion of control container.
      Ioc.Initialize();
      Logger.Info("Windsor castle container initialized.");

      // Setup the controller factory to use the Windsor controller factory.
      ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(Assembly.GetExecutingAssembly()));

      Logger.Info("Initialized [WindsorControllerFactory] as the default Controller factory.");


      if (ConfigurationManager.AppSettings.Get("TraceSql") == "1" && Application["dbMonitor"] == null)
      {
        MonitorDB();
      }

    }

    static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      Logger.Fatal("App-domain unhandled exception.", e.ExceptionObject as Exception);
    }

    protected void Application_Error(object sender, EventArgs e)
    {
      var ex = Server.GetLastError();

      Logger.Error("Application Exception Occurred", ex);

      Response.Redirect(AdminSystem.SystemParameters.Instance.General.LogOnURL + @"/Home/Error");
    }

    /// <summary>
    /// SCP237121: Prevention of Unhandled exception found in IS-WEB Log 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Session_End(object sender, EventArgs e)
    {
      Response.Redirect(AdminSystem.SystemParameters.Instance.General.LogOnURL + @"/Account/LogOff"); 
    }

    /// <summary>
    /// Starts monitoring the DB.
    /// </summary>
    private static void MonitorDB(string host = "localhost", int port = 1000)
    {
      var monitor = HttpContext.Current.Application["dbMonitor"] as OracleMonitor;

      if (monitor != null)
      {
        monitor.Host = host;
        monitor.Port = port;
        monitor.IsActive = true;
      }
      else
      {
        HttpContext.Current.Application["dbMonitor"] = new OracleMonitor { IsActive = true, Host = host, Port = port };
      }

      Logger.Info("Initialized Oracle monitor to trace EF queries.");
    }
  }
}

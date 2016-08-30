using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Web.Controllers.Base;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.MiscPayables.Controllers
{
  public class ManageMiscPayablesInvoiceController : ManageMUPayablesControllerBase
  {
    
    public ManageMiscPayablesInvoiceController(IMiscInvoiceManager miscInvoiceManager, ICalendarManager calendarManager):base(miscInvoiceManager, calendarManager)
    {
        AreaText = "MiscPay";
    }

    protected override BillingCategoryType BillingCategory
    {
      get
      {
        return BillingCategoryType.Misc;
      }
      set
      {
        base.BillingCategory = value;
      }
    }

    /// <summary>
    /// CMP #655: IS-WEB Display per Location ID
    /// Desc: Due to URL limitation issue, have seperate out Get and Post method
    /// </summary>
    /// <returns></returns>
   [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Misc.Payables.Search.Query)]
    public ActionResult Index()
    {
      var miscSearchCriteria = new MiscSearchCriteria();
      return base.Index(miscSearchCriteria);
    }

       /// <summary>
       /// CMP #655: IS-WEB Display per Location ID
       /// Desc: Due to URL limitation issue, have seperate out Get and Post method
       /// </summary>
       /// <returns></returns>
    [ValidateAntiForgeryToken]
    [HttpPost]
    [ISAuthorize(Business.Security.Permissions.Misc.Payables.Search.Query)]
    public override ActionResult Index(MiscSearchCriteria miscSearchCriteria)
    {
        return base.Index(miscSearchCriteria);
    }
  }
}

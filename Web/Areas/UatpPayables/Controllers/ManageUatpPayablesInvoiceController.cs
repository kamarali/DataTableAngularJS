using System.Web.Mvc;
using System.Web.Script.Serialization;
using Iata.IS.Business.Common;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Web.Controllers.Base;
using Iata.IS.Web.UIModel.Grid.MUPayables;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.UatpPayables.Controllers
{
  public class ManageUatpPayablesInvoiceController : ManageMUPayablesControllerBase
  {
      public ManageUatpPayablesInvoiceController(IMiscInvoiceManager miscInvoiceManager, ICalendarManager calendarManager)
          : base(miscInvoiceManager, calendarManager)
    {
        AreaText = "UatpPay";
    }

    protected override BillingCategoryType BillingCategory
    {
      get
      {
        return BillingCategoryType.Uatp;
      }
      set
      {
        base.BillingCategory = value;
      }
    }

    [ISAuthorize(Business.Security.Permissions.UATP.Payables.Search.Query)]
    public override ActionResult Index(MiscSearchCriteria miscSearchCriteria)
    {
      return base.Index(miscSearchCriteria);
    }
  }
}

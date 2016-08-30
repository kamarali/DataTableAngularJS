using System;
using System.Web.Mvc;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.Controllers.Base;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Misc.Controllers
{
  public class ManageMiscInvoiceController : ManageMiscUatpControllerBase
  {

    public IMiscInvoiceManager MiscInvoiceManager { get; set; }

    public ManageMiscInvoiceController()
    {
      BillingCategory = BillingCategoryType.Misc;
      AreaText = "Misc";
    }

    protected override IMiscUatpInvoiceManager Manager
    {
      get
      {
        return MiscInvoiceManager;
      }
    }

      /// <summary>
      /// CMP #655: IS-WEB Display per Location ID
      /// Desc: Due to URL limitation issue, have seperate out Get and Post method
      /// </summary>
      /// <returns></returns>
    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Misc.Receivables.Manage.Query)]
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
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Misc.Receivables.Manage.Query)]
    public new ActionResult Index(MiscSearchCriteria miscSearchCriteria)
    {
        return base.Index(miscSearchCriteria);
    }


    protected override RejectionOnValidationFailure GetRejectionOnValidationFailureFlag()
    {
      var rejectionFlag = base.GetRejectionOnValidationFailureFlag();
      var rejectionFlagId = MemberManager.GetMemberConfigurationValue(SessionUtil.MemberId, MemberConfigParameter.MiscRejectionOnValidationFailure);
      if (!string.IsNullOrEmpty(rejectionFlagId))
        rejectionFlag = (RejectionOnValidationFailure)Convert.ToInt32(rejectionFlagId);
      return rejectionFlag;
    }
  }
}

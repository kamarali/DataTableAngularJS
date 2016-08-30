using System.Collections.Generic;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using System;
using Enyim.Caching;
using System.Reflection;
using Iata.IS.Web.Util.Filters;


namespace Iata.IS.Web.Areas.Masters.Controllers
{
  public class ChargeCodeTypeRequirementSetUpController : ISController
  {
    private readonly IChargeCodeManager _chargeCodeManager;
    private readonly IReferenceManager _referenceManager;
    private const String SuccessMessage = "Record saved.";

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="chargeCodeManager"></param>
    /// <param name="referenceManager"></param>
    public ChargeCodeTypeRequirementSetUpController(IChargeCodeManager chargeCodeManager, IReferenceManager referenceManager)
    {
      _chargeCodeManager = chargeCodeManager;
      _referenceManager = referenceManager;
    }

    /// <summary>
    /// Indexes this instance.
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.ChargeCodeTypeReqSetupQuery)]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult Index()
    {
      int ChargeCategoryId = 0;
      int ChargeCodeId = 0;
      var ChargeCodeTypeReqSetupGrid = new ChargeCodeTypeReqSetUpSearch("SearchChargeCodeTypeReqSetUpGrid", Url.Action("ChargeCodeTypeReqSetUpSearchGridData", new { ChargeCategoryId, ChargeCodeId }));
      //Default size of grid.
      ChargeCodeTypeReqSetupGrid.DefaultPageSize = 5;
      ViewData["ChargeCodeTypeReqSetupGrid"] = ChargeCodeTypeReqSetupGrid.Instance;
      return View();
    }

    /// <summary>
    /// Indexes the specified charge code.
    /// </summary>
    /// <param name="chargeCode">The charge code.</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.ChargeCodeTypeReqSetupQuery)]
    public ActionResult Index(ChargeCode chargeCode)
    {
      var ChargeCodeTypeReqSetupGrid = new ChargeCodeTypeReqSetUpSearch("SearchChargeCodeTypeReqSetUpGrid", Url.Action("ChargeCodeTypeReqSetUpSearchGridData", new { ChargeCategoryId = chargeCode.ChargeCategoryId, ChargeCodeId = chargeCode.Id }));
      ViewData["ChargeCodeTypeReqSetupGrid"] = ChargeCodeTypeReqSetupGrid.Instance;
      return View(chargeCode);
    }

    /// <summary>
    /// Charges the code search grid data.
    /// </summary>
    /// <param name="Name">The name.</param>
    /// <param name="ChargeCategoryId">The charge category id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.ChargeCodeTypeReqSetupQuery)]
    public JsonResult ChargeCodeTypeReqSetUpSearchGridData(int ChargeCategoryId, int ChargeCodeId)
    {
      var ChargeCodeTypeReqSetupGrid = new ChargeCodeTypeReqSetUpSearch("SearchChargeCodeTypeReqSetUpGrid", Url.Action("ChargeCodeTypeReqSetUpSearchGridData", new { ChargeCategoryId, ChargeCodeId }));
      var chargeCodes = _chargeCodeManager.GetMiscChargeCode(ChargeCategoryId, ChargeCodeId);
      return ChargeCodeTypeReqSetupGrid.DataBind(chargeCodes.AsQueryable());
    }

    /// <summary>
    /// Creates this instance.
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.ChargeCodeTypeReqSetupEditOrDelete)]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult Create()
    {
      return View();
    }

    /// <summary>
    /// Creates the specified charge code.
    /// </summary>
    /// <param name="chargeCode">The charge code.</param>
    /// <param name="collection">The collection.</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.ChargeCodeTypeReqSetupEditOrDelete)]
    public ActionResult Create(ChargeCode chargeCode)
    {
      try
      {
        _chargeCodeManager.AddChargeCode(chargeCode, SessionUtil.UserId);
        ShowSuccessMessage(SuccessMessage);
        return RedirectToAction("Index");
      }
      catch (ISBusinessException ex)
      {
        ShowErrorMessage(ex.ErrorCode);
        return View(chargeCode);
      }
    }

    /// <summary>
    /// Edits the specified id.
    /// </summary>
    /// <param name="Id">The id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.ChargeCodeTypeReqSetupEditOrDelete)]
    public ActionResult Edit(int Id)
    {
      ChargeCode chargeCode = _chargeCodeManager.GetChargeCodeDetails(Id);
      return View(chargeCode);
    }

    /// <summary>
    /// Edits the specified id.
    /// </summary>
    /// <param name="Id">The id.</param>
    /// <param name="chargeCode">The charge code.</param>
    /// <param name="collection">The collection.</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.ChargeCodeTypeReqSetupEditOrDelete)]
    public ActionResult Edit(ChargeCode chargeCode)
    {
      _chargeCodeManager.UpdateChargeCode(chargeCode, SessionUtil.UserId);
      ShowSuccessMessage(SuccessMessage);
      return RedirectToAction("Index");
    }

    /// <summary>
    /// Deletes the specified id.
    /// </summary>
    /// <param name="Id">The id.</param>
    /// <param name="collection">The collection.</param>
    /// <returns></returns>
    [HttpPost]
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.ChargeCodeTypeReqSetupEditOrDelete)]
    public ActionResult Delete(int Id)
    {
      _chargeCodeManager.DeleteChargeCode(Id, SessionUtil.UserId);
      return RedirectToAction("Index");
    }

    /// <summary>
    /// This function is used to get charge code list based on charge category.
    /// </summary>
    /// <param name="chargeCategoryId"></param>
    /// <returns></returns>
    public ActionResult GetChargeCodeList(int chargeCategoryId)
    {
      IList<ChargeCode> chargeCodeList = new List<ChargeCode>();

      if (chargeCategoryId > 0)
        chargeCodeList = _referenceManager.GetChargeCodeList(chargeCategoryId);

      return new JsonResult() { Data = chargeCodeList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
    }
  }
}

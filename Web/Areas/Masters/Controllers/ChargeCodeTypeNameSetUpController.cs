using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Enyim.Caching;
using System.Reflection;
using Iata.IS.Business.Common;
using System.Web.Mvc;
using Iata.IS.Web.UIModel.Grid.Masters;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
  public class ChargeCodeTypeNameSetUpController : ISController
  {
    private readonly IChargeCodeTypeManager _chargeCodeTypeManager;
    private readonly IChargeCodeManager _chargeCodeManager;
    private readonly IReferenceManager _referenceManager;
    private const String SuccessMessage = "Record saved.";

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="chargeCodeTypeManager">The charge code manager.</param>
    public ChargeCodeTypeNameSetUpController(IChargeCodeTypeManager chargeCodeTypeManager, IChargeCodeManager chargeCodeManager, IReferenceManager referenceManager)
    {
      _chargeCodeTypeManager = chargeCodeTypeManager;
      _chargeCodeManager = chargeCodeManager;
      _referenceManager = referenceManager;
    }

    /// <summary>
    /// Indexes this instance.
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.ChargeCodeTypeNameSetupQuery)]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult Index()
    {
      int chargeCategoryId = 0;
      int chargeCodeId = 0;
      string chargeCodeTypeName = String.Empty;
      var ChargeCodeTypeNameSetupGrid = new ChargeCodeTypeNameSetUpSearch("SearchChargeCodeTypeNameSetUpGrid", Url.Action("ChargeCodeTypeNameSetUpSearchGridData", new { chargeCategoryId = chargeCategoryId, chargeCodeId = chargeCodeId, ChargeCodeTypeName = chargeCodeTypeName }));
      //Default size of grid.
      ChargeCodeTypeNameSetupGrid.DefaultPageSize = 5;
      ViewData["ChargeCodeTypeNameSetupGrid"] = ChargeCodeTypeNameSetupGrid.Instance;
      return View();
    }

    /// <summary>
    /// Indexes the specified charge code.
    /// </summary>
    /// <param name="chargeCodeType">The charge code.</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.ChargeCodeTypeNameSetupQuery)]
    public ActionResult Index(ChargeCodeType chargeCodeType)
    {
      var ChargeCodeTypeNameSetupGrid = new ChargeCodeTypeNameSetUpSearch("SearchChargeCodeTypeNameSetUpGrid", Url.Action("ChargeCodeTypeNameSetUpSearchGridData", new { chargeCategoryId = chargeCodeType.ChargeCategoryId, chargeCodeId = chargeCodeType.ChargeCodeId, chargeCodeTypeName = chargeCodeType.Name }));
      ViewData["ChargeCodeTypeNameSetupGrid"] = ChargeCodeTypeNameSetupGrid.Instance;
      return View(chargeCodeType);
    }

    /// <summary>
    /// Charges the code search grid data.
    /// </summary>
    /// <param name="Name">The name.</param>
    /// <param name="ChargeCategoryId">The charge category id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.ChargeCodeTypeNameSetupQuery)]
    public JsonResult ChargeCodeTypeNameSetUpSearchGridData(int chargeCategoryId, int chargeCodeId, string chargeCodeTypeName)
    {
      var ChargeCodeTypeNameSetupGrid = new ChargeCodeTypeNameSetUpSearch("SearchChargeCodeTypeNameSetUpGrid", Url.Action("ChargeCodeTypeNameSetUpSearchGridData", new { chargeCategoryId, ChargeCodeId = chargeCodeId, chargeCodeTypeName }));
      var chargeCodes = _chargeCodeTypeManager.GetMiscChargeCodeType(chargeCategoryId, chargeCodeId, chargeCodeTypeName != null ? chargeCodeTypeName.Trim() : chargeCodeTypeName);
      return ChargeCodeTypeNameSetupGrid.DataBind(chargeCodes.AsQueryable());
    }

    /// <summary>
    /// Creates this instance.
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.ChargeCodeTypeNameSetupEditOrDelete)]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult Create()
    {
      return View();
    }

    /// <summary>
    /// Creates the specified charge code.
    /// </summary>
    /// <param name="chargeCodeType">The charge code.</param>
    /// <param name="collection">The collection.</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.ChargeCodeTypeNameSetupEditOrDelete)]
    public ActionResult Create(ChargeCodeType chargeCodeType)
    {
      try
      {
        _chargeCodeTypeManager.AddChargeCodeType(chargeCodeType, SessionUtil.UserId);
        ShowSuccessMessage(SuccessMessage);
        return RedirectToAction("Index");
      }
      catch (ISBusinessException ex)
      {
        ShowErrorMessage(ex.ErrorCode);
        return View(chargeCodeType);
      }
    }

    /// <summary>
    /// Edits the specified id.
    /// </summary>
    /// <param name="Id">The id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.ChargeCodeTypeNameSetupEditOrDelete)]
    public ActionResult Edit(int Id)
    {
      ChargeCodeType chargeCodeType = _chargeCodeTypeManager.GetChargeCodeTypeDetails(Id);
      var chargeCodeDetail = _chargeCodeManager.GetChargeCodeDetails(chargeCodeType.ChargeCodeId);
      chargeCodeType.ChargeCategoryId = chargeCodeDetail.ChargeCategoryId;

      return View(chargeCodeType);
    }

    /// <summary>
    /// Edits the specified id.
    /// </summary>
    /// <param name="Id">The id.</param>
    /// <param name="chargeCodeType">The charge code.</param>
    /// <param name="collection">The collection.</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.ChargeCodeTypeNameSetupEditOrDelete)]
    public ActionResult Edit(ChargeCodeType chargeCodeType)
    {
      try
      {
        _chargeCodeTypeManager.UpdateChargeCodeType(chargeCodeType, SessionUtil.UserId);
        ShowSuccessMessage(SuccessMessage);
        return RedirectToAction("Index");
      }
      catch (ISBusinessException ex)
      {
        //Show error message and return to view.
        ShowErrorMessage(ex.ErrorCode);
        return View(chargeCodeType);
      }
    }

    /// <summary>
    /// Deletes the specified id.
    /// </summary>
    /// <param name="Id">The id.</param>
    /// <param name="collection">The collection.</param>
    /// <returns></returns>
    [HttpPost]
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.ChargeCodeTypeNameSetupEditOrDelete)]
    public ActionResult Delete(int Id)
    {
      _chargeCodeTypeManager.DeleteChargeCodeType(Id, SessionUtil.UserId);
      return RedirectToAction("Index");
    }

    /// <summary>
    /// This function is used to get charge code list based on charge category.
    /// </summary>
    /// <param name="chargeCategoryId"></param>
    /// <returns></returns>
    public ActionResult GetChargeCodeList(int chargeCategoryId, bool isActiveChargeCodeTypeReq)
    {
      IList<ChargeCode> chargeCodeList = new List<ChargeCode>();

      if (chargeCategoryId > 0)
        chargeCodeList = _referenceManager.GetChargeCodeListForMstChargeCodeType(chargeCategoryId, isActiveChargeCodeTypeReq);

      return new JsonResult() { Data = chargeCodeList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
    }
  }
}
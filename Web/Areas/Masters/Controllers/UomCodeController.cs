using System.Collections.Generic;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using System;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
  public class UomCodeController : ISController
  {
    private readonly IUomCodeManager _UomCodeManager = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="UomCodeController"/> class.
    /// </summary>
    /// <param name="uomCodeManager">The uom code manager.</param>
    public UomCodeController(IUomCodeManager uomCodeManager)
    {
      _UomCodeManager = uomCodeManager;
    }

    /// <summary>
    /// Indexes this instance.
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.UomCodeQuery)]
    public ActionResult Index()
    {
      try
      {
        const string Description = "";
        const int uomCodeType = 0;
        const string uomCode = "";
        var uomCodeGrid = new UomCodeSearch("SearchUomCodeGrid", Url.Action("UomCodeSearchGridData", "UomCode", new { uomCode, uomCodeType, Description }));
        ViewData["UomCodeGrid"] = uomCodeGrid.Instance;
        return View();
      }
      catch
      {
        return View();
      }
    }

    /// <summary>
    /// Indexes the specified uom code.
    /// </summary>
    /// <param name="uomCode">The uom code.</param>
    /// <returns></returns>
   [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.UomCodeQuery)]
    [HttpPost]
    public ActionResult Index(UomCode uomCode, FormCollection collection)
    {
      uomCode.Type = string.IsNullOrEmpty(collection["Type"].ToString()) ? -1 : Convert.ToInt32(collection["Type"].ToString());
      if (!string.IsNullOrEmpty(uomCode.Description))
      {
        uomCode.Description = uomCode.Description.Trim();
      }
      SessionUtil.CurrentPageSelected = 1;
      var uomCodesGrid = new UomCodeSearch("SearchUomCodeGrid", Url.Action("UomCodeSearchGridData", "UomCode", new { uomCode.Id, uomCode.Type, uomCode.Description }));
      ViewData["UomCodeGrid"] = uomCodesGrid.Instance;
      return View(uomCode);
    }

    /// <summary>
    /// Uoms the code search grid data.
    /// </summary>
    /// <param name="Code">The code.</param>
    /// <param name="Name">The name.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.UomCodeQuery)]
    public JsonResult UomCodeSearchGridData(string Id, int? type, string Description)
    {
      int uomtype = (type != null) ? Convert.ToInt32(type) : -1;
      var uomCodesGrid = new UomCodeSearch("SearchUomCodeGrid", Url.Action("UomCodeSearchGridData", new { Id, uomtype, Description }));
      var uomCodes = _UomCodeManager.GetUomCodeList(Id, uomtype, Description);
      try
      {
        return uomCodesGrid.DataBind(uomCodes.AsQueryable());

      }
      catch (ISBusinessException be)
      {
        ViewData["errorMessage"] = be.ErrorCode;
        return null;
      }
    }

    /// <summary>
    /// Detailses the specified id.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.UomCodeQuery)]
    public ActionResult Details(string id)
    {
      return View();
    }

    /// <summary>
    /// Creates this instance.
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.UomCodeEditOrDelete)]
    public ActionResult Create()
    {
      return View();
    }

    /// <summary>
    /// Creates the specified uom code.
    /// </summary>
    /// <param name="uomCode">The uom code.</param>
    /// <param name="collection">The collection.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.UomCodeEditOrDelete)]
    [HttpPost]
    public ActionResult Create(UomCode uomCode, FormCollection collection)
    {
      try
      {
        uomCode.Id = uomCode.Id.ToUpper();
        if (!string.IsNullOrEmpty(uomCode.Description))
        {
          uomCode.Description = uomCode.Description.Trim();
          if (uomCode.Description.Length > 255)
            uomCode.Description = uomCode.Description.Substring(0, 255);
        }
        if (ModelState.IsValid)
        {
          var createUomCode = _UomCodeManager.AddUomCode(uomCode);
          ShowSuccessMessage(Messages.RecordSaveSuccessful);
          return RedirectToAction("Index");
        }
        
        return View(uomCode);
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
        return View(uomCode);
      }
    }

    /// <summary>
    /// Edits the specified id.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.UomCodeEditOrDelete)]
    public ActionResult Edit(string id)
    {
      UomCode uomCode = _UomCodeManager.GetUomCodeDetails(id);
      // Done to fix an issue.
      uomCode.Code = uomCode.Id;
     
      return View(uomCode);
    }

    /// <summary>
    /// Edits the specified id.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="uomCode">The uom code.</param>
    /// <param name="collection">The collection.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.UomCodeEditOrDelete)]
    [HttpPost]
    public ActionResult Edit(string id, UomCode uomCode, FormCollection collection)
    {
      try
      {
        //uomCode.Id = uomCode.Id.ToUpper();
        // Done to fix an issue.
        uomCode.Id = uomCode.Code.ToUpper();
        if (!string.IsNullOrEmpty(uomCode.Description))
        {
          uomCode.Description = uomCode.Description.Trim();
          if (uomCode.Description.Length > 255)
            uomCode.Description = uomCode.Description.Substring(0, 255);
        }
        
        var UpdateUomCode = _UomCodeManager.UpdateUomCode(uomCode);
        ShowSuccessMessage(Messages.RecordSaveSuccessful);
        return RedirectToAction("Index");
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
        return View(uomCode);
      }
    }

    /// <summary>
    /// Deletes the specified id.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="collection">The collection.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.UomCodeEditOrDelete)]
    [HttpPost]
    public ActionResult Delete(string id, FormCollection collection)
    {
      try
      {
        // TODO: Add delete logic here
        var deleteUomCode = _UomCodeManager.DeleteUomCode(id);
        return RedirectToAction("Index");
      }
      catch (Exception exception)
      {
        return View();
      }
    }
  }
}

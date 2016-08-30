using System.Collections.Generic;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using System;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
  public class RficController : ISController
  {
    private readonly IRficManager _RficManager = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="RficController"/> class.
    /// </summary>
    /// <param name="rficManager">The rfic manager.</param>
    public RficController(IRficManager rficManager)
    {
      _RficManager = rficManager;
    }

    /// <summary>
    /// Indexes this instance.
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.RFICQuery)]
    public ActionResult Index()
    {

        const string rficDescription = "";
        const string rficId = "";
        var rficGrid = new RficSearch("SearchRficGrid", Url.Action("RficSearchGridData", "Rfic", new { rficId, rficDescription }));
      ViewData["RficGrid"] = rficGrid.Instance;
      return View();
    }

    /// <summary>
    /// Indexes the specified rfic.
    /// </summary>
    /// <param name="rfic">The rfic.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.RFICQuery)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Index(Rfic rfic)
    {
        if (!string.IsNullOrEmpty(rfic.Description))
        {
            rfic.Description = rfic.Description.Trim();
        }
        SessionUtil.CurrentPageSelected = 1;
        var rficGrid = new RficSearch("SearchRficGrid", Url.Action("RficSearchGridData", "Rfic", new { rfic.Id,rfic.Description }));
      ViewData["RficGrid"] = rficGrid.Instance;
      return View(rfic);
    }

    /// <summary>
    /// Rfics the search grid data.
    /// </summary>
    /// <param name="Description">The description.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.RFICQuery)]
    public JsonResult RficSearchGridData(string Id,string Description)
    {
      var rficsGrid = new RficSearch("SearchRficGrid", Url.Action("RficSearchGridData", new {Id,Description }));
      var rfics = _RficManager.GetRficList(Id,Description);
      try
      {
        return rficsGrid.DataBind(rfics.AsQueryable());
      }
      catch (ISBusinessException be)
      {
        ViewData["errorMessage"] = be.ErrorCode;
        return null;
      }
    }

    /// <summary>
    /// Creates this instance.
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.RFICEditOrDelete)]
    public ActionResult Create()
    {
      return View();
    }

    /// <summary>
    /// Creates the specified rfic.
    /// </summary>
    /// <param name="rfic">The rfic.</param>
    /// <param name="collection">The collection.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.RFICEditOrDelete)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Rfic rfic, FormCollection collection)
    {
      try
      {
          if (!string.IsNullOrEmpty(rfic.Description))
          {
              rfic.Description = rfic.Description.Trim();
              if (rfic.Description.Length > 255)
                rfic.Description = rfic.Description.Substring(0, 255);
          }
        if (ModelState.IsValid)
        {
          // TODO: Add insert logic here
          var createRfic = _RficManager.AddRfic(rfic);
          ShowSuccessMessage(Messages.RecordSaveSuccessful);
          return RedirectToAction("Index");
        }
        else
        {
          return View(rfic);
        }
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
        return View(rfic);
      }
      catch (Exception ex)
      {
          ShowErrorMessage(string.Format(Messages.RecordSaveException, ex.Message));
          return View(rfic);
      }
    }

    /// <summary>
    /// Edits the specified id.
    /// </summary>
    /// <param name="Id">The id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.RFICEditOrDelete)]
    public ActionResult Edit(string Id)
    {
      Rfic rfic = _RficManager.GetRficDetails(Id);
      return View(rfic);
    }

    /// <summary>
    /// Edits the specified id.
    /// </summary>
    /// <param name="Id">The id.</param>
    /// <param name="rfic">The rfic.</param>
    /// <param name="collection">The collection.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.RFICEditOrDelete)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(string Id, Rfic rfic, FormCollection collection)
    {
      try
      {
        rfic.Id = Id;
        if (!string.IsNullOrEmpty(rfic.Description))
        {
            rfic.Description = rfic.Description.Trim();
            if (rfic.Description.Length > 255)
              rfic.Description = rfic.Description.Substring(0, 255);
        }
        if (ModelState.IsValid)
        {
          // TODO: Add update logic here
          Rfic Updatedrfic = _RficManager.UpdateRfic(rfic);
          ShowSuccessMessage(Messages.RecordSaveSuccessful);
          return RedirectToAction("Index");
        }
        else
        {
          return View(rfic);
        }
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
        return View(rfic);
      }
      catch (Exception ex)
      {
          ShowErrorMessage(string.Format(Messages.RecordSaveException, ex.Message));
          return View(rfic);
      }
    }

    /// <summary>
    /// Deletes the specified id.
    /// </summary>
    /// <param name="Id">The id.</param>
    /// <param name="collection">The collection.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.RFICEditOrDelete)]
    [HttpPost]
    public ActionResult Delete(string Id, FormCollection collection)
    {
      try
      {
        // TODO: Add delete logic here
        var rficDelete = _RficManager.DeleteRfic(Id);
        return RedirectToAction("Index");
      }
      catch (Exception ex)
      {
        return RedirectToAction("Index");
      }
    }


  }
}

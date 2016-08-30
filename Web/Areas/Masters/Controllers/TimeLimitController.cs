using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using System;
using Iata.IS.Web.Util.Filters;


namespace Iata.IS.Web.Areas.Masters.Controllers
{
  public class TimeLimitController : ISController
  {
    private readonly ITimeLimitManager _TimeLimitManager = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeLimitController"/> class.
    /// </summary>
    /// <param name="timeLimitManager">The time limit manager.</param>
    public TimeLimitController(ITimeLimitManager timeLimitManager)
    {
      _TimeLimitManager = timeLimitManager;
    }

    /// <summary>
    /// Indexes this instance.
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.TimeLimitQuery)]
    public ActionResult Index()
    {
      const int SettlementMethodId = 0;
      const int Limit = 0, TransactionTypeId = 0;
      DateTime effectiveFromPeriod, effectiveToPeriod;
      effectiveFromPeriod = effectiveToPeriod = new DateTime(01, 01, 01);
      
      var timeLimitGrid = new TimeLimitSearch("SearchTimeLimitGrid", Url.Action("TimeLimitSearchGridData", "TimeLimit", new { Limit, SettlementMethodId, TransactionTypeId, effectiveFromPeriod, effectiveToPeriod }));
      ViewData["TimeLimitGrid"] = timeLimitGrid.Instance;
      return View();
    }

    /// <summary>
    /// Indexes the specified time limit.
    /// </summary>
    /// <param name="timeLimit">The time limit.</param>
    /// <returns></returns>
   [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.TimeLimitQuery)]
    [HttpPost]
    public ActionResult Index(TimeLimit timeLimit)
    {
      SessionUtil.CurrentPageSelected = 1;
      var timeLimitGrid = new TimeLimitSearch("SearchTimeLimitGrid", Url.Action("TimeLimitSearchGridData", "TimeLimit", new { timeLimit.Limit, timeLimit.SettlementMethodId, timeLimit.TransactionTypeId, timeLimit.EffectiveFromPeriod, timeLimit.EffectiveToPeriod }));
      ViewData["TimeLimitGrid"] = timeLimitGrid.Instance;

      return View();
    }

    /// <summary>
    /// Times the limit search grid data.
    /// </summary>
    /// <param name="Limit">The limit.</param>
    /// <param name="ClearingHouse">The clearing house.</param>
    /// <param name="TransactionTypeId">The transaction type id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.TimeLimitQuery)]
    public JsonResult TimeLimitSearchGridData(int Limit, int SettlementMethodId, int TransactionTypeId, DateTime effectiveFromPeriod, DateTime effectiveToPeriod)
    {
      var timeLimitGrid = new TimeLimitSearch("SearchTimeLimitGrid", Url.Action("TimeLimitSearchGridData", new { Limit, SettlementMethodId, TransactionTypeId, effectiveFromPeriod, effectiveToPeriod }));
      var timeLimits = _TimeLimitManager.GetTimeLimitList(Limit, SettlementMethodId, TransactionTypeId, effectiveFromPeriod, effectiveToPeriod);
      try
      {
        return timeLimitGrid.DataBind(timeLimits.AsQueryable());
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
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.TimeLimitEditOrDelete)]
    public ActionResult Create()
    {
      return View();
    }

    /// <summary>
    /// Creates the specified time limit.
    /// </summary>
    /// <param name="timeLimit">The time limit.</param>
    /// <param name="collection">The collection.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.TimeLimitEditOrDelete)]
    [HttpPost]
    public ActionResult Create(TimeLimit timeLimit, FormCollection collection)
    {
      try
      {
        timeLimit.CalculationMethod = timeLimit.CalculationMethod.ToUpper();
        if (ModelState.IsValid)
        {
          // TODO: Add insert logic here
          var createtimeLimit = _TimeLimitManager.AddTimeLimit(timeLimit);
          ShowSuccessMessage(Messages.RecordSaveSuccessful);
          return RedirectToAction("Index");
        }
        else
        {
          return View(timeLimit);
        }
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        return View(timeLimit);
      }
      catch
      {
        ShowErrorMessage(Messages.RecordSaveException);
        return View(timeLimit);
      }
    }

    /// <summary>
    /// Edits the specified id.
    /// </summary>
    /// <param name="Id">The id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.TimeLimitEditOrDelete)]
    public ActionResult Edit(int Id)
    {
      TimeLimit timeLimit = _TimeLimitManager.GetTimeLimitDetails(Id);
      return View(timeLimit);
    }

    /// <summary>
    /// Edits the specified id.
    /// </summary>
    /// <param name="Id">The id.</param>
    /// <param name="timeLimit">The time limit.</param>
    /// <param name="collection">The collection.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.TimeLimitEditOrDelete)]
    [HttpPost]
    public ActionResult Edit(int Id, TimeLimit timeLimit, FormCollection collection)
    {
      try
      {
        timeLimit.Id = Id;
        //timeLimit.ClearingHouse = timeLimit.ClearingHouse.ToUpper();
        timeLimit.CalculationMethod = timeLimit.CalculationMethod.ToUpper();
        if (ModelState.IsValid)
        {
          // TODO: Add update logic here
          TimeLimit UpdatedtimeLimit = _TimeLimitManager.UpdateTimeLimit(timeLimit);
          ShowSuccessMessage(Messages.RecordUpdateSuccessful);
          return RedirectToAction("Index");
        }
        else
        {
          return View(timeLimit);
        }
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        return View(timeLimit);
      }
      catch
      {
        ShowErrorMessage(Messages.RecordSaveException);
        return View(timeLimit);
      }
    }

    /// <summary>
    /// Deletes the specified id.
    /// </summary>
    /// <param name="Id">The id.</param>
    /// <param name="collection">The collection.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Masters.Masters.TimeLimitEditOrDelete)]
    [HttpPost]
    public ActionResult Delete(int Id, FormCollection collection)
    {
      try
      {
        // TODO: Add delete logic here
        var countryDelete = _TimeLimitManager.DeleteTimeLimit(Id);
        return RedirectToAction("Index");
      }
      catch (Exception ex)
      {
        return RedirectToAction("Index");
      }
    }
  }
}

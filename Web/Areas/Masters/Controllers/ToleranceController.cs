using System;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class ToleranceController : ISController
    {
        private readonly IToleranceManager _ToleranceManager = null;

        public ToleranceController(IToleranceManager toleranceManager)
         {
             _ToleranceManager = toleranceManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ToleranceQuery)]
        public ActionResult Index()
        {
            
            const int billingCategoryId=0;
            const string clearingHouse="";
            const string type ="";
            DateTime effectiveFromPeriod = new DateTime(01, 01, 01);
            DateTime effectiveToPeriod = new DateTime(01, 01, 01); 
            var toleranceGrid = new ToleranceSearch("SearchToleranceGrid", Url.Action("ToleranceSearchGridData", "Tolerance", new { billingCategoryId, clearingHouse, type, effectiveFromPeriod, effectiveToPeriod}));
            ViewData["ToleranceGrid"] = toleranceGrid.Instance;
           
            return View();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ToleranceEditOrDelete)]
        public ActionResult Create()
        {
          return View();
        }

        /// <summary>
        /// Creates the specified Tolerance.
        /// </summary>
        /// <param name="tolerance">Tolerance object</param>
        /// <param name="collection">Tolerance form collection.</param>
        /// <returns></returns>
       [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ToleranceEditOrDelete)]
        [HttpPost]
        public ActionResult Create(Tolerance tolerance, FormCollection collection)
        {
          try
          {
            if (ModelState.IsValid)
            {
              tolerance.ClearingHouse = tolerance.ClearingHouse.ToUpper();
              tolerance.Type = tolerance.Type.ToUpper();
              // Compare Effective From and Effective To period values 
              var isFromPeriodLessThanToPeriod = DateTime.Compare(tolerance.EffectiveFromPeriod, tolerance.EffectiveToPeriod);
              // If Effective From period is greater than Effective To period display an error message, else save the transaction
              if (isFromPeriodLessThanToPeriod > 0)
              {
                ShowErrorMessage(Messages.EffectiveFromAndToPeriodComparison);
                return View(tolerance);
              }
              else
              {
                _ToleranceManager.AddTolerance(tolerance);
                ShowSuccessMessage(Messages.RecordSaveSuccessful);
                return RedirectToAction("Index");
              }
            }
            else
            {
              return View(tolerance);
            }
          }
          catch (ISBusinessException exception)
          {
            ShowErrorMessage(exception.ErrorCode);
            return View(tolerance);
          }
          catch
          {
            ShowErrorMessage(Messages.RecordSaveException);
            return View(tolerance);
          }
        }

        /// <summary>
        /// Indexes the specified tolerance.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns></returns>
       [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ToleranceQuery)]
        [HttpPost]
        public ActionResult Index(Tolerance tolerance)
        {
            SessionUtil.CurrentPageSelected = 1;
             // Compare Effective From and Effective To period values 
              var isFromPeriodLessThanToPeriod = DateTime.Compare(tolerance.EffectiveFromPeriod, tolerance.EffectiveToPeriod);
              // If Effective From period is greater than Effective To period display an error message, else save the transaction
              if (isFromPeriodLessThanToPeriod > 0)
              {
                  ShowErrorMessage(Messages.EffectiveFromAndToPeriodComparison);
                  const int billingCategoryId = 0;
                  const string clearingHouse = "";
                  const string type = "";
                  DateTime effectiveFromPeriod = new DateTime(01, 01, 01);
                  DateTime effectiveToPeriod = new DateTime(01, 01, 01);
                  var toleranceGrid = new ToleranceSearch("SearchToleranceGrid", Url.Action("ToleranceSearchGridData", "Tolerance", new { billingCategoryId, clearingHouse, type, effectiveFromPeriod, effectiveToPeriod }));
                  ViewData["ToleranceGrid"] = toleranceGrid.Instance;
              }
              else
              {

                  var tolerancesGrid = new ToleranceSearch("SearchToleranceGrid",
                                                           Url.Action("ToleranceSearchGridData",
                                                                      new
                                                                          {
                                                                              tolerance.BillingCategoryId,
                                                                              tolerance.ClearingHouse,
                                                                              tolerance.Type,
                                                                              tolerance.EffectiveFromPeriod,
                                                                              tolerance.EffectiveToPeriod
                                                                          }));
                  ViewData["ToleranceGrid"] = tolerancesGrid.Instance;
                 
              }
              return View();
        }

        /// <summary>
        /// Tolerances the search grid data.
        /// </summary>
        /// <param name="billingCategoryId">The billing category id.</param>
        /// <param name="clearingHouse">The clearing house.</param>
        /// <param name="type">The type.</param>
        /// <param name="effectiveFromPeriod"></param>
        /// <param name="effectiveToPeriod"></param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ToleranceQuery)]
        public JsonResult ToleranceSearchGridData(int billingCategoryId, string clearingHouse, string type, System.DateTime effectiveFromPeriod, System.DateTime effectiveToPeriod)
        {

            var tolerancesGrid = new ToleranceSearch("SearchToleranceGrid", Url.Action("ToleranceSearchGridData", new { billingCategoryId, clearingHouse, type, effectiveFromPeriod, effectiveToPeriod}));
            var tolerances = _ToleranceManager.GetToleranceList(billingCategoryId, clearingHouse, type,effectiveFromPeriod ,effectiveToPeriod);
            try
            {
                return tolerancesGrid.DataBind(tolerances.AsQueryable());

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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ToleranceQuery)]
        public ActionResult Details(int id)
        {
            return View();
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
      
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ToleranceEditOrDelete)]
        public ActionResult Edit(int id)
        {
            Tolerance tolerance = _ToleranceManager.GetToleranceDetails(id);
            return View(tolerance);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ToleranceEditOrDelete)]
        [HttpPost]
        public ActionResult Edit(int id,Tolerance tolerance, FormCollection collection)
        {
            try
            {
                tolerance.Id = id;
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    tolerance.ClearingHouse = tolerance.ClearingHouse.ToUpper();
                    tolerance.Type = tolerance.Type.ToUpper();
                    // Compare Effective From and Effective To period values 
                    var isFromPeriodLessThanToPeriod = DateTime.Compare(tolerance.EffectiveFromPeriod, tolerance.EffectiveToPeriod);
                    // If Effective From period is greater than Effective To period display an error message, else update the transaction
                    if (isFromPeriodLessThanToPeriod > 0)
                    {
                      ShowErrorMessage(Messages.EffectiveFromAndToPeriodComparison);
                      return View(tolerance);
                    }
                    else
                    {
                      var UpdateTolerance = _ToleranceManager.UpdateTolerance(tolerance);
                      ShowSuccessMessage(Messages.RecordUpdateSuccessful);
                      return RedirectToAction("Index");
                    }
                }
                else
                {
                    return View(tolerance);
                }
                
            }
            catch (ISBusinessException businessException)
            {
                ShowErrorMessage(businessException.ErrorCode);
                return View(tolerance);
            }
            catch
            {
                ShowErrorMessage(Messages.RecordSaveException);
                return View(tolerance);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ToleranceEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteTolerance = _ToleranceManager.DeleteTolerance(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

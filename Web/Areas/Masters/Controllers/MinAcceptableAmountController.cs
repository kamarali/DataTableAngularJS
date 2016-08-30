using System.Collections.Generic;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using System;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class MinAcceptableAmountController : ISController
    {
        private readonly IMinAcceptableAmountManager _minAcceptableAmountManager = null;
        //private readonly IReasonCodeManager _reasonCodeManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="MinAcceptableAmountController"/> class.
        /// </summary>
        /// <param name="minAcceptableAmountManager">The min acceptable amount manager.</param>
        public MinAcceptableAmountController(IMinAcceptableAmountManager minAcceptableAmountManager)
         {
             _minAcceptableAmountManager = minAcceptableAmountManager;
            
         }
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtQuery)]
        public ActionResult Index()
        {
            const int transactionTypeId = 0;
            const int applicableMinFieldId = 0;
            const string clearinghouse = "";
            const int reasonCodeId = 0;
            const double amount= 0;
            DateTime effectiveFromPeriod = new DateTime(01, 01, 01);
            DateTime effectiveToPeriod = new DateTime(01, 01, 01);
            var minAcceptableAmountGrid = new MinAcceptableAmountSearch("SearchMinAcceptableAmountGrid", Url.Action("MinAcceptableAmountSearchGridData", "MinAcceptableAmount", new { effectiveFromPeriod, effectiveToPeriod, transactionTypeId, clearinghouse, amount, reasonCodeId, applicableMinFieldId }));
            ViewData["MinAcceptableAmountGrid"] = minAcceptableAmountGrid.Instance;
            ViewData["RejectionReasonCode"] = null;
            return View();
        }
        /// <summary>
        /// Indexes the specified min acceptable amount.
        /// </summary>
        /// <param name="minAcceptableAmount">The min acceptable amount.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtQuery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MinAcceptableAmount minAcceptableAmount)
        {
            SessionUtil.CurrentPageSelected = 1;
            try
            {
                // Compare Effective From and Effective To period values 
                var isFromPeriodLessThanToPeriod = DateTime.Compare(minAcceptableAmount.EffectiveFromPeriod, minAcceptableAmount.EffectiveToPeriod);
                // If Effective From period is greater than Effective To period display an error message, else save the transaction
                if (isFromPeriodLessThanToPeriod > 0)
                {
                    ShowErrorMessage(Messages.EffectiveFromAndToPeriodComparison);
                    const int transactionTypeId = 0;
                    const int applicableMinFieldId = 0;
                    const string clearinghouse = "";
                    const int reasonCodeId = 0;
                    const double amount = 0;
                    DateTime effectiveFromPeriod = new DateTime(01, 01, 01);
                    DateTime effectiveToPeriod = new DateTime(01, 01, 01);
                    var minAcceptableAmountGrid = new MinAcceptableAmountSearch("SearchMinAcceptableAmountGrid", Url.Action("MinAcceptableAmountSearchGridData", "MinAcceptableAmount", new { effectiveFromPeriod, effectiveToPeriod, transactionTypeId, clearinghouse, amount, reasonCodeId, applicableMinFieldId }));
                    ViewData["MinAcceptableAmountGrid"] = minAcceptableAmountGrid.Instance;

                    return View();
                }
                else
                {

                    var minAcceptableAmountsGrid = new MinAcceptableAmountSearch("SearchMinAcceptableAmountGrid", Url.Action("MinAcceptableAmountSearchGridData", new { effectiveFromPeriod = minAcceptableAmount.EffectiveFromPeriod, effectiveToPeriod = minAcceptableAmount.EffectiveToPeriod, transactionTypeId = minAcceptableAmount.TransactionTypeId, clearinghouse = minAcceptableAmount.ClearingHouse, amount = minAcceptableAmount.Amount, reasonCode = minAcceptableAmount.RejectionReasonCode, applicableMinFieldId = minAcceptableAmount.ApplicableMinimumFieldId }));
                    ViewData["MinAcceptableAmountGrid"] = minAcceptableAmountsGrid.Instance;

                    ViewData["RejectionReasonCode"] = minAcceptableAmount.RejectionReasonCode;
                    return View();
                }
            }
            catch (ISBusinessException businessException)
            {
                ShowErrorMessage(businessException.ErrorCode);
                return View();
            }
           
        }

        /// <summary>
        /// Mins x acceptable amount search grid data.
        /// </summary>
        /// <param name="applicableMinFieldId">Applicable Amount Field</param>
        /// <param name="effectiveFromPeriod">Effective From Period</param>
        /// <param name="effectiveToPeriod">Effective To Period</param>
        /// <param name="reasonCodeId">Rejection Reasoncode Id</param>
        /// <param name="transactionTypeId">The transaction type id.</param>
        /// <param name="clearinghouse">The clearinghouse.</param>
        /// <param name="amount">The minimum amount.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtQuery)]
        public JsonResult MinAcceptableAmountSearchGridData(DateTime effectiveFromPeriod, DateTime effectiveToPeriod, int transactionTypeId, string clearinghouse, string reasonCode, int applicableMinFieldId, double amount)
        {
            var minAcceptableAmountsGrid = new MinAcceptableAmountSearch("SearchMinAcceptableAmountGrid", Url.Action("MinAcceptableAmountSearchGridData", new { effectiveFromPeriod, effectiveToPeriod, applicableMinFieldId, reasonCode, transactionTypeId, clearinghouse, amount }));            
            try
            {
                var minAcceptableAmounts = _minAcceptableAmountManager.GetMinAcceptableAmountList(effectiveFromPeriod, effectiveToPeriod, applicableMinFieldId, reasonCode, transactionTypeId, clearinghouse, amount);
                return minAcceptableAmountsGrid.DataBind(minAcceptableAmounts.AsQueryable());

            }
            catch (ISBusinessException be)
            {
                ViewData["errorMessage"] = be.ErrorCode;
                ViewData[ViewDataConstants.ErrorMessage] = be.ErrorCode;
                return null;
            }            
        }
        /// <summary>
        /// Detailse the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtQuery)]
        public ActionResult Details(int id)
        {
            return View();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// Creates the specified min acceptable amount.
        /// </summary>
        /// <param name="minAcceptableAmount">The min acceptable amount.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MinAcceptableAmount minAcceptableAmount, FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    minAcceptableAmount.ClearingHouse = minAcceptableAmount.ClearingHouse.ToUpper();
                    // Compare Effective From and Effective To period values 
                    var isFromPeriodLessThanToPeriod = DateTime.Compare(minAcceptableAmount.EffectiveFromPeriod, minAcceptableAmount.EffectiveToPeriod);
                    // If Effective From period is greater than Effective To period display an error message, else save the transaction
                    if (isFromPeriodLessThanToPeriod > 0)
                    {
                      ShowErrorMessage(Messages.EffectiveFromAndToPeriodComparison);
                      return View(minAcceptableAmount);
                    }
                    else
                    {
                      _minAcceptableAmountManager.AddMinAcceptableAmount(minAcceptableAmount);
                      ShowSuccessMessage(Messages.RecordSaveSuccessful);
                      return RedirectToAction("Index");
                    }
                }
                else
                {
                    return View(minAcceptableAmount);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(minAcceptableAmount);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtEditOrDelete)]
        public ActionResult Edit(int id)
        {
            MinAcceptableAmount minAcceptableAmount = _minAcceptableAmountManager.GetMinAcceptableAmountDetails(id);
            return View(minAcceptableAmount);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="minAcceptableAmount">The min max acceptable amount.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, MinAcceptableAmount minAcceptableAmount, FormCollection collection)
        {
            try
            {
                minAcceptableAmount.Id = id;
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    minAcceptableAmount.ClearingHouse = minAcceptableAmount.ClearingHouse.ToUpper();
                    // Compare Effective From and Effective To period values 
                    var isFromPeriodLessThanToPeriod = DateTime.Compare(minAcceptableAmount.EffectiveFromPeriod, minAcceptableAmount.EffectiveToPeriod);
                    // If Effective From period is greater than Effective To period display an error message, else save the transaction
                    if (isFromPeriodLessThanToPeriod > 0)
                    {
                      ShowErrorMessage(Messages.EffectiveFromAndToPeriodComparison);
                      return View(minAcceptableAmount);
                    }
                    else
                    {
                      _minAcceptableAmountManager.UpdateMinAcceptableAmount(minAcceptableAmount);
                      ShowSuccessMessage(Messages.RecordUpdateSuccessful);
                      return RedirectToAction("Index");
                    }
                }
                else
                {
                    return View(minAcceptableAmount);
                }

            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(minAcceptableAmount);
            }
        }


        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteMinAcceptableAmount = _minAcceptableAmountManager.DeleteMinAcceptableAmount(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public virtual JsonResult GetRejectionReasonCodeList(int transactionTypeId)
        {
            var rejectionReasonCodeList =_minAcceptableAmountManager.GetRejectionReasonCodeList(transactionTypeId);
            return Json(rejectionReasonCodeList);
        }
    }
}
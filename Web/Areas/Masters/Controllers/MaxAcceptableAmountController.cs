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
    public class MaxAcceptableAmountController : ISController
    {
        private readonly IMaxAcceptableAmountManager _maxAcceptableAmountManager = null;
        //private readonly IReasonCodeManager _reasonCodeManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxAcceptableAmountController"/> class.
        /// </summary>
        /// <param name="maxAcceptableAmountManager">The min acceptable amount manager.</param>
        public MaxAcceptableAmountController(IMaxAcceptableAmountManager maxAcceptableAmountManager)
        {
            _maxAcceptableAmountManager = maxAcceptableAmountManager;

        }
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtQuery)]
        public ActionResult Index()
        {
            const int transactionTypeId = 0;
            const string clearinghouse = "";
            const double amount = 0;
            DateTime effectiveFromPeriod = new DateTime(01, 01, 01);
            DateTime effectiveToPeriod = new DateTime(01, 01, 01); 
            var maxAcceptableAmountGrid = new MaxAcceptableAmountSearch("SearchMaxAcceptableAmountGrid", Url.Action("MaxAcceptableAmountSearchGridData", "MaxAcceptableAmount", new {effectiveFromPeriod, effectiveToPeriod, transactionTypeId, clearinghouse, amount}));
            ViewData["MaxAcceptableAmountGrid"] = maxAcceptableAmountGrid.Instance;
            return View();
        }
        /// <summary>
        /// Indexes the specified max acceptable amount.
        /// </summary>
        /// <param name="maxAcceptableAmount">The max acceptable amount.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtQuery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MaxAcceptableAmount maxAcceptableAmount)
        {
            SessionUtil.CurrentPageSelected = 1;
            // Compare Effective From and Effective To period values 
            var isFromPeriodLessThanToPeriod = DateTime.Compare(maxAcceptableAmount.EffectiveFromPeriod,
                                                                maxAcceptableAmount.EffectiveToPeriod);
            // If Effective From period is greater than Effective To period display an error message, else save the transaction
            if (isFromPeriodLessThanToPeriod > 0)
            {
                ShowErrorMessage(Messages.EffectiveFromAndToPeriodComparison);
                const int transactionTypeId = 0;
                const string clearinghouse = "";
                const double amount = 0;
                DateTime effectiveFromPeriod = new DateTime(01, 01, 01);
                DateTime effectiveToPeriod = new DateTime(01, 01, 01);
                var maxAcceptableAmountGrid = new MaxAcceptableAmountSearch("SearchMaxAcceptableAmountGrid",
                                                                            Url.Action(
                                                                                "MaxAcceptableAmountSearchGridData",
                                                                                "MaxAcceptableAmount",
                                                                                new
                                                                                    {
                                                                                        effectiveFromPeriod,
                                                                                        effectiveToPeriod,
                                                                                        transactionTypeId,
                                                                                        clearinghouse,
                                                                                        amount
                                                                                    }));
                ViewData["MaxAcceptableAmountGrid"] = maxAcceptableAmountGrid.Instance;
                return View();
            }
            else
            {
                var maxAcceptableAmountsGrid = new MaxAcceptableAmountSearch("SearchMaxAcceptableAmountGrid",
                                                                             Url.Action(
                                                                                 "MaxAcceptableAmountSearchGridData",
                                                                                 new
                                                                                     {
                                                                                         effectiveFromPeriod =
                                                                                     maxAcceptableAmount.
                                                                                     EffectiveFromPeriod,
                                                                                         effectiveToPeriod =
                                                                                     maxAcceptableAmount.
                                                                                     EffectiveToPeriod,
                                                                                         transactionTypeId =
                                                                                     maxAcceptableAmount.
                                                                                     TransactionTypeId,
                                                                                         clearinghouse =
                                                                                     maxAcceptableAmount.ClearingHouse,
                                                                                         amount =
                                                                                     maxAcceptableAmount.Amount
                                                                                     }));
                ViewData["MaxAcceptableAmountGrid"] = maxAcceptableAmountsGrid.Instance;
            }

            return View();
        }

        /// <summary>
        /// Max acceptable amount search grid data.
        /// </summary>
        /// <param name="effectiveFromPeriod">Effective From Period</param>
        /// <param name="effectiveToPeriod">Effective To Period</param>
        /// <param name="transactionTypeId">The transaction type id.</param>
        /// <param name="clearinghouse">The clearinghouse.</param>
        /// <param name="amount">The min.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtQuery)]
        public JsonResult MaxAcceptableAmountSearchGridData(DateTime effectiveFromPeriod, DateTime effectiveToPeriod, int transactionTypeId, string clearinghouse, double amount)
        {
            var maxAcceptableAmountsGrid = new MaxAcceptableAmountSearch("SearchMaxAcceptableAmountGrid", Url.Action("MaxAcceptableAmountSearchGridData", new { transactionTypeId, clearinghouse, amount }));
            var maxAcceptableAmounts = _maxAcceptableAmountManager.GetMaxAcceptableAmountList(effectiveFromPeriod,effectiveToPeriod, transactionTypeId, clearinghouse, amount);
            try
            {
                return maxAcceptableAmountsGrid.DataBind(maxAcceptableAmounts.AsQueryable());

            }
            catch (ISBusinessException be)
            {
                ViewData["errorMessage"] = be.ErrorCode;
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
        /// Creates the specified max acceptable amount.
        /// </summary>
        /// <param name="maxAcceptableAmount">The min acceptable amount.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MaxAcceptableAmount maxAcceptableAmount, FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                  // TODO: Add insert logic here
                  maxAcceptableAmount.ClearingHouse = maxAcceptableAmount.ClearingHouse.ToUpper();
                  // Compare Effective From and Effective To period values 
                  var isFromPeriodLessThanToPeriod = DateTime.Compare(maxAcceptableAmount.EffectiveFromPeriod, maxAcceptableAmount.EffectiveToPeriod);
                  // If Effective From period is greater than Effective To period display an error message, else save the transaction
                  if(isFromPeriodLessThanToPeriod > 0)
                  {
                    ShowErrorMessage(Messages.EffectiveFromAndToPeriodComparison);
                    return View(maxAcceptableAmount);
                  }
                  else
                  {
                    _maxAcceptableAmountManager.AddMaxAcceptableAmount(maxAcceptableAmount);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index"); 
                  }
                }
                else
                {
                    return View(maxAcceptableAmount);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(maxAcceptableAmount);
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
            MaxAcceptableAmount maxAcceptableAmount = _maxAcceptableAmountManager.GetMaxAcceptableAmountDetails(id);
            return View(maxAcceptableAmount);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="maxAcceptableAmount">The max acceptable amount.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, MaxAcceptableAmount maxAcceptableAmount, FormCollection collection)
        {
            try
            {
                maxAcceptableAmount.Id = id;
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    maxAcceptableAmount.ClearingHouse = maxAcceptableAmount.ClearingHouse.ToUpper();
                    // Compare Effective From and Effective To period values 
                    var isFromPeriodLessThanToPeriod = DateTime.Compare(maxAcceptableAmount.EffectiveFromPeriod, maxAcceptableAmount.EffectiveToPeriod);
                    // If Effective From period is greater than Effective To period display an error message, else save the transaction
                    if (isFromPeriodLessThanToPeriod > 0)
                    {
                      ShowErrorMessage(Messages.EffectiveFromAndToPeriodComparison);
                      return View(maxAcceptableAmount);
                    }
                    else
                    {
                      _maxAcceptableAmountManager.UpdateMaxAcceptableAmount(maxAcceptableAmount);
                      ShowSuccessMessage(Messages.RecordUpdateSuccessful);
                      return RedirectToAction("Index");
                    }
                }
                else
                {
                    return View(maxAcceptableAmount);
                }

            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(maxAcceptableAmount);
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
                var deleteMaxAcceptableAmount = _maxAcceptableAmountManager.DeleteMaxAcceptableAmount(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        
    }
}
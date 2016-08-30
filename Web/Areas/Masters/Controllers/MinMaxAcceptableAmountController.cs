using System.Collections.Generic;
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
    public class MinMaxAcceptableAmountController : ISController
    {
        private readonly IMinMaxAcceptableAmountManager _MinMaxAcceptableAmountManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="MinMaxAcceptableAmountController"/> class.
        /// </summary>
        /// <param name="minMaxAcceptableAmountManager">The min max acceptable amount manager.</param>
        public MinMaxAcceptableAmountController(IMinMaxAcceptableAmountManager minMaxAcceptableAmountManager)
         {
             _MinMaxAcceptableAmountManager = minMaxAcceptableAmountManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtQuery)]
        public ActionResult Index()
        {
            
            const int transactionTypeId=0;
            const string clearinghouse="";
            const double min = 0, max = 0;
            var minMaxAcceptableAmountGrid = new MinMaxAcceptableAmountSearch("SearchMinMaxAcceptableAmountGrid", Url.Action("MinMaxAcceptableAmountSearchGridData", "MinMaxAcceptableAmount", new { transactionTypeId, clearinghouse, min, max }));
            ViewData["MinMaxAcceptableAmountGrid"] = minMaxAcceptableAmountGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified min max acceptable amount.
        /// </summary>
        /// <param name="minMaxAcceptableAmount">The min max acceptable amount.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtQuery)]
        [HttpPost]
        public ActionResult Index(MinMaxAcceptableAmount minMaxAcceptableAmount)
        {
            SessionUtil.CurrentPageSelected = 1;
            var minMaxAcceptableAmountsGrid = new MinMaxAcceptableAmountSearch("SearchMinMaxAcceptableAmountGrid", Url.Action("MinMaxAcceptableAmountSearchGridData", new { minMaxAcceptableAmount.TransactionTypeId, minMaxAcceptableAmount.ClearingHouse, minMaxAcceptableAmount.Min, minMaxAcceptableAmount.Max }));
            ViewData["MinMaxAcceptableAmountGrid"] = minMaxAcceptableAmountsGrid.Instance;
            return View();
        }

        /// <summary>
        /// Mins the max acceptable amount search grid data.
        /// </summary>
        /// <param name="transactionTypeId">The transaction type id.</param>
        /// <param name="clearinghouse">The clearinghouse.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtQuery)]
        public JsonResult MinMaxAcceptableAmountSearchGridData(int transactionTypeId, string clearinghouse, double min, double max)
        {

            var minMaxAcceptableAmountsGrid = new MinMaxAcceptableAmountSearch("SearchMinMaxAcceptableAmountGrid", Url.Action("MinMaxAcceptableAmountSearchGridData", new { transactionTypeId, clearinghouse, min, max }));
            var minMaxAcceptableAmounts = _MinMaxAcceptableAmountManager.GetMinMaxAcceptableAmountList(transactionTypeId, clearinghouse, min, max);
            try
            {
                return minMaxAcceptableAmountsGrid.DataBind(minMaxAcceptableAmounts.AsQueryable());

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
        /// Creates the specified min max acceptable amount.
        /// </summary>
        /// <param name="minMaxAcceptableAmount">The min max acceptable amount.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtEditOrDelete)]
        [HttpPost]
        public ActionResult Create(MinMaxAcceptableAmount minMaxAcceptableAmount,FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    minMaxAcceptableAmount.ClearingHouse = minMaxAcceptableAmount.ClearingHouse.ToUpper();
                    var createMinMaxAcceptableAmount = _MinMaxAcceptableAmountManager.AddMinMaxAcceptableAmount(minMaxAcceptableAmount);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(minMaxAcceptableAmount);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(minMaxAcceptableAmount);
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
            MinMaxAcceptableAmount minMaxAcceptableAmount = _MinMaxAcceptableAmountManager.GetMinMaxAcceptableAmountDetails(id);
            return View(minMaxAcceptableAmount);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="minMaxAcceptableAmount">The min max acceptable amount.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtEditOrDelete)]
        [HttpPost]
        public ActionResult Edit(int id,MinMaxAcceptableAmount minMaxAcceptableAmount, FormCollection collection)
        {
            try
            {
                minMaxAcceptableAmount.Id = id;
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    minMaxAcceptableAmount.ClearingHouse = minMaxAcceptableAmount.ClearingHouse.ToUpper();
                    var UpdateMinMaxAcceptableAmount = _MinMaxAcceptableAmountManager.UpdateMinMaxAcceptableAmount(minMaxAcceptableAmount);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(minMaxAcceptableAmount);
                }
                
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(minMaxAcceptableAmount);
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
                var deleteMinMaxAcceptableAmount = _MinMaxAcceptableAmountManager.DeleteMinMaxAcceptableAmount(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

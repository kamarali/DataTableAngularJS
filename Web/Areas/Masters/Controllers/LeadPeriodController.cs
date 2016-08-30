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
    public class LeadPeriodController : ISController
    {
        private readonly ILeadPeriodManager _leadPeriodManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeadPeriodController"/> class.
        /// </summary>
        /// <param name="leadPeriodManager">The lead period manager.</param>
        public LeadPeriodController(ILeadPeriodManager leadPeriodManager)
        {
            _leadPeriodManager = leadPeriodManager;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LeadPeriodQuery)]
        public ActionResult Index()
        {
            const string clearingHouse = null;
            const int period = 0, billingCategoryId = 0;
            DateTime effectiveFromPeriod, effectiveToPeriod;
            effectiveFromPeriod = effectiveToPeriod = new DateTime(01, 01, 01);

            var leadPeriodGrid = new LeadPeriodSearch("SearchLeadPeriodGrid", Url.Action("LeadPeriodSearchGridData", "LeadPeriod", new { period, clearingHouse, billingCategoryId, effectiveFromPeriod, effectiveToPeriod }));
            ViewData["LeadPeriodGrid"] = leadPeriodGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified min acceptable amount.
        /// </summary>
        /// <param name="leadPeriod">The min acceptable amount.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LeadPeriodQuery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LeadPeriod leadPeriod)
        {
            SessionUtil.CurrentPageSelected = 1;
            var leadPeriodGrid = new LeadPeriodSearch("SearchLeadPeriodGrid", Url.Action("LeadPeriodSearchGridData", new { effectiveFromPeriod = leadPeriod.EffectiveFromPeriod, effectiveToPeriod = leadPeriod.EffectiveToPeriod, period = leadPeriod.Period, billingCategoryId=leadPeriod.BillingCategoryId, clearinghouse = leadPeriod.ClearingHouse, }));
            ViewData["LeadPeriodGrid"] = leadPeriodGrid.Instance;
            return View();
        }

        /// <summary>
        /// lead period search grid data.
        /// </summary>
        /// <param name="period">The limit.</param>
        /// <param name="clearingHouse">The clearing house.</param>
        /// <param name="samplingIndicator">The sampling indicator.</param>
        /// <param name="billingCategoryId">The billing category id.</param>
        /// <param name="effectiveFromPeriod">The effective from period.</param>
        /// <param name="effectiveToPeriod">The effective to period.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LeadPeriodQuery)]
        public JsonResult LeadPeriodSearchGridData(int period, string clearingHouse, string samplingIndicator, int billingCategoryId, DateTime effectiveFromPeriod, DateTime effectiveToPeriod)
        {
            var leadPeriodGrid = new LeadPeriodSearch("SearchLeadPeriodGrid", Url.Action("LeadPeriodSearchGridData", "LeadPeriod", new { period, clearingHouse, samplingIndicator, billingCategoryId, effectiveFromPeriod, effectiveToPeriod }));
            var leadPeriods = _leadPeriodManager.GetLeadPeriodList(period, clearingHouse, billingCategoryId, effectiveFromPeriod, effectiveToPeriod);
            try
            {
                return leadPeriodGrid.DataBind(leadPeriods.AsQueryable());
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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LeadPeriodQuery)]
        public ActionResult Details(int id)
        {
            return View();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LeadPeriodEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified Lead Period.
        /// </summary>
        /// <param name="leadPeriod">The Lead Period.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LeadPeriodEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LeadPeriod leadPeriod, FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    leadPeriod.ClearingHouse = leadPeriod.ClearingHouse.ToUpper();
                    var createLeadPeriod = _leadPeriodManager.AddLeadPeriod(leadPeriod);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(leadPeriod);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(leadPeriod);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LeadPeriodEditOrDelete)]
        public ActionResult Edit(int id)
        {
            LeadPeriod leadPeriod = _leadPeriodManager.GetLeadPeriodDetails(id);
            return View(leadPeriod);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="leadPeriod">The min max acceptable amount.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LeadPeriodEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, LeadPeriod leadPeriod, FormCollection collection)
        {
            try
            {
                leadPeriod.Id = id;
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    leadPeriod.ClearingHouse = leadPeriod.ClearingHouse.ToUpper();
                    var updateLeadPeriod = _leadPeriodManager.UpdateLeadPeriod(leadPeriod);
                    ShowSuccessMessage(Messages.RecordUpdateSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(leadPeriod);
                }

            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(leadPeriod);
            }
        }


        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LeadPeriodEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteLeadPeriod = _leadPeriodManager.DeleteLeadPeriod(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}

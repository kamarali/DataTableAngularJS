using System.Collections.Generic;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using System;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class SampleDigitController: ISController
    {
        private readonly ISampleDigitManager _SampleDigitManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleDigitController"/> class.
        /// </summary>
        /// <param name="sampleDigitManager">The sample digit manager.</param>
        public SampleDigitController(ISampleDigitManager sampleDigitManager)
         {
             _SampleDigitManager = sampleDigitManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SampleDigitQuery)]
        public ActionResult Index()
        {
            
            const string billingMonth = "";
            var sampleDigitGrid = new SampleDigitSearch("SearchSampleDigitGrid", Url.Action("SampleDigitSearchGridData", "SampleDigit", new { billingMonth }));
            ViewData["SampleDigitGrid"] = sampleDigitGrid.Instance;
           
            return View();
        }

        /// <summary>
        /// Indexes the specified sample digit.
        /// </summary>
        /// <param name="sampleDigit">The sample digit.</param>
        /// <returns></returns>
         [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SampleDigitQuery)]
        [HttpPost]
        public ActionResult Index(SampleDigit sampleDigit)
        {
            SessionUtil.CurrentPageSelected = 1;
            var sampleDigitsGrid = new SampleDigitSearch("SearchSampleDigitGrid", Url.Action("SampleDigitSearchGridData", new { sampleDigit.ProvisionalBillingMonth }));
            ViewData["SampleDigitGrid"] = sampleDigitsGrid.Instance;
           
            return View();
        }

        /// <summary>
        /// Samples the digit search grid data.
        /// </summary>
        /// <param name="ProvisionalBillingMonth">The Provisional Billing Month.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SampleDigitQuery)]
        public JsonResult SampleDigitSearchGridData(string ProvisionalBillingMonth)
        {

            var sampleDigitsGrid = new SampleDigitSearch("SearchSampleDigitGrid", Url.Action("SampleDigitSearchGridData", new { ProvisionalBillingMonth }));
            var sampleDigits = _SampleDigitManager.GetSampleDigitList(ProvisionalBillingMonth);
            try
            {
                return sampleDigitsGrid.DataBind(sampleDigits.AsQueryable());

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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SampleDigitQuery)]
        public ActionResult Details(int id)
        {
            return View();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SampleDigitEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified sample digit.
        /// </summary>
        /// <param name="sampleDigit">The sample digit.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SampleDigitEditOrDelete)]
        [HttpPost]
        public ActionResult Create(SampleDigit sampleDigit,FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createSampleDigit = _SampleDigitManager.AddSampleDigit(sampleDigit);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(sampleDigit);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(sampleDigit);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SampleDigitEditOrDelete)]
        public ActionResult Edit(int id)
        {
            SampleDigit sampleDigit = _SampleDigitManager.GetSampleDigitDetails(id);
            return View(sampleDigit);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="sampleDigit">The sample digit.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
         [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SampleDigitEditOrDelete)]
        [HttpPost]
        public ActionResult Edit(int id,SampleDigit sampleDigit, FormCollection collection)
        {
            try
            {
                sampleDigit.Id = id;
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    var UpdateSampleDigit = _SampleDigitManager.UpdateSampleDigit(sampleDigit);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(sampleDigit);
                }
                
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(sampleDigit);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SampleDigitEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteSampleDigit = _SampleDigitManager.DeleteSampleDigit(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

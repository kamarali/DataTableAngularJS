using System.Collections.Generic;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using System;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class OnBehalfInvoiceSetupController: ISController
    {
        private readonly IOnBehalfInvoiceSetupManager _OnBehalfInvoiceSetupManager = null;
        private readonly IChargeCodeManager _ChargeCodeManager = null;
        private readonly IChargeCategoryManager _chargeCategoryManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnBehalfInvoiceSetupController"/> class.
        /// </summary>
        /// <param name="onBehalfInvoiceSetupManager">The onBehalfInvoiceSetup manager.</param>
        /// <param name="chargeCategoryManager"> Charge Category Manager</param>
        public OnBehalfInvoiceSetupController(IOnBehalfInvoiceSetupManager onBehalfInvoiceSetupManager, IChargeCodeManager chargeCodeManager, IChargeCategoryManager chargeCategoryManager)
        {
            _ChargeCodeManager = chargeCodeManager;
            _OnBehalfInvoiceSetupManager = onBehalfInvoiceSetupManager;
            _chargeCategoryManager = chargeCategoryManager;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.OnBehalfInvoiceSetupQuery)]
        public ActionResult Index()
        {
            
            const string transmitterCode = "";
            const int billingCategoryId=0,chargeCategoryId=0,chargeCodeId=0;
            var onBehalfInvoiceSetupGrid = new OnBehalfInvoiceSetupSearch("SearchOnBehalfInvoiceSetupGrid", Url.Action("OnBehalfInvoiceSetupSearchGridData", "OnBehalfInvoiceSetup", new { billingCategoryId, transmitterCode,chargeCategoryId,chargeCodeId }));
            ViewData["OnBehalfInvoiceSetupGrid"] = onBehalfInvoiceSetupGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified onBehalfInvoiceSetup.
        /// </summary>
        /// <param name="onBehalfInvoiceSetup">The onBehalfInvoiceSetup.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.OnBehalfInvoiceSetupQuery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(OnBehalfInvoiceSetup onBehalfInvoiceSetup)
        {
            SessionUtil.CurrentPageSelected = 1;
            var onBehalfInvoiceSetupsGrid = new OnBehalfInvoiceSetupSearch("SearchOnBehalfInvoiceSetupGrid", Url.Action("OnBehalfInvoiceSetupSearchGridData", new { onBehalfInvoiceSetup.BillingCategoryId, onBehalfInvoiceSetup.TransmitterCode, onBehalfInvoiceSetup.ChargeCategoryId, onBehalfInvoiceSetup.ChargeCodeId }));
            ViewData["OnBehalfInvoiceSetupGrid"] = onBehalfInvoiceSetupsGrid.Instance;
            return View(onBehalfInvoiceSetup);
        }

        /// <summary>
        /// Currencies the search grid data.
        /// </summary>
        /// <param name="Code">The code.</param>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.OnBehalfInvoiceSetupQuery)]
        public JsonResult OnBehalfInvoiceSetupSearchGridData(int BillingCategoryId, string TransmitterCode,int ChargeCategoryId,int ChargeCodeId)
        {

            var onBehalfInvoiceSetupsGrid = new OnBehalfInvoiceSetupSearch("SearchOnBehalfInvoiceSetupGrid", Url.Action("OnBehalfInvoiceSetupSearchGridData", new { BillingCategoryId, TransmitterCode, ChargeCategoryId, ChargeCodeId }));
            var onBehalfInvoiceSetups = _OnBehalfInvoiceSetupManager.GetOnBehalfInvoiceSetupList(BillingCategoryId, TransmitterCode, ChargeCategoryId, ChargeCodeId);
            try
            {
                return onBehalfInvoiceSetupsGrid.DataBind(onBehalfInvoiceSetups.AsQueryable());

            }
            catch (ISBusinessException be)
            {
                ViewData["errorMessage"] = be.ErrorCode;
                return null;
            }
        }

        [ISAuthorize(Business.Security.Permissions.Masters.Masters.OnBehalfInvoiceSetupQuery)]
        public JsonResult GetChargeCategoryList(string billingCategoryId)
        {
          var intBillingCategoryId = !string.IsNullOrEmpty(billingCategoryId) ? Convert.ToInt32(billingCategoryId) : -1;
          var chargeCategory = _chargeCategoryManager.GetChargeCategoryList("", intBillingCategoryId);
          var result = new JsonResult {Data = chargeCategory, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
          return result;
        }

        [ISAuthorize(Business.Security.Permissions.Masters.Masters.OnBehalfInvoiceSetupQuery)]
        public JsonResult GetChargeCodeList(string ChargeCategoryId)
        {
            var chargeCategoryId = !string.IsNullOrEmpty(ChargeCategoryId) ? Convert.ToInt32(ChargeCategoryId) : 0;
            var ChargeCodes = _ChargeCodeManager.GetChargeCodeList("", chargeCategoryId);
            var selectList = (new SelectList(ChargeCodes, "Id", "Name"));
            var result = new JsonResult();
            result.Data = ChargeCodes;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        /// <summary>
        /// Detailses the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.OnBehalfInvoiceSetupQuery)]
        public ActionResult Details(int id)
        {
            return View();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.OnBehalfInvoiceSetupEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified onBehalfInvoiceSetup.
        /// </summary>
        /// <param name="onBehalfInvoiceSetup">The onBehalfInvoiceSetup.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.OnBehalfInvoiceSetupEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OnBehalfInvoiceSetup onBehalfInvoiceSetup,FormCollection collection)
        {
            try
            {
                onBehalfInvoiceSetup.TransmitterCode = onBehalfInvoiceSetup.TransmitterCode.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createOnBehalfInvoiceSetup = _OnBehalfInvoiceSetupManager.AddOnBehalfInvoiceSetup(onBehalfInvoiceSetup);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(onBehalfInvoiceSetup);
                }
            }
            catch (ISBusinessException be)
            {
                ShowErrorMessage(be.ErrorCode);
                return View(onBehalfInvoiceSetup);
            }
            catch
            {
                ShowErrorMessage(Messages.RecordSaveException);
                return View(onBehalfInvoiceSetup);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.OnBehalfInvoiceSetupEditOrDelete)]
        public ActionResult Edit(int id)
        {
            OnBehalfInvoiceSetup onBehalfInvoiceSetup = _OnBehalfInvoiceSetupManager.GetOnBehalfInvoiceSetupDetails(id);
            return View(onBehalfInvoiceSetup);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="onBehalfInvoiceSetup">The onBehalfInvoiceSetup.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.OnBehalfInvoiceSetupEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,OnBehalfInvoiceSetup onBehalfInvoiceSetup, FormCollection collection)
        {
            try
            {
                onBehalfInvoiceSetup.Id = id;
                onBehalfInvoiceSetup.TransmitterCode = onBehalfInvoiceSetup.TransmitterCode.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    var UpdateOnBehalfInvoiceSetup = _OnBehalfInvoiceSetupManager.UpdateOnBehalfInvoiceSetup(onBehalfInvoiceSetup);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(onBehalfInvoiceSetup);
                }
                
            }
            catch (ISBusinessException be)
            {
                ShowErrorMessage(be.ErrorCode);
                return View(onBehalfInvoiceSetup);
            }
            catch
            {
                ShowErrorMessage(Messages.RecordSaveException);
                return View(onBehalfInvoiceSetup);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.OnBehalfInvoiceSetupEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteOnBehalfInvoiceSetup = _OnBehalfInvoiceSetupManager.DeleteOnBehalfInvoiceSetup(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

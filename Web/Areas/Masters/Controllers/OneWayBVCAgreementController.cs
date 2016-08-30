using System.Collections.Generic;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using System;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Model.Pax;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    /// <summary>
    /// One Way Agreements Not Sent for BVC.
    /// </summary>
    public class OneWayBVCAgreementController : ISController
    {
        private readonly IBvcAgreementManager _bvcAgreementManager;
    
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bvcAgreementManager"></param>
        public OneWayBVCAgreementController(IBvcAgreementManager bvcAgreementManager)
        {
            _bvcAgreementManager = bvcAgreementManager;
        }

        /// <summary>
        /// Default view, landing page
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.BvcAgreementSetupQuery)]
        public ActionResult Index()
        {
            var bvcAgreementGrid = new BvcAgreementSearch("SearchBvcAgreementGrid", Url.Action("BVCAgreementSearchGridData", "OneWayBVCAgreement"));
            ViewData["BvcAgreementGrid"] = bvcAgreementGrid.Instance;
            return View();
        }

        /// <summary>
        /// Returns create view.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.BvcAgreementSetupEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Masters/Bvc/Create

        /// <summary>
        /// Creates the bvc ageement between billing member and billed member.
        /// </summary>
        /// <param name="bvcAgreement">Object to create new bvc agreement, This contains Billing and billed member</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.BvcAgreementSetupEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BvcAgreement bvcAgreement)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bvcAgreement.LastUpdatedBy = SessionUtil.UserId;
                    _bvcAgreementManager.AddBVCAgreement(bvcAgreement);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(string.Format(Messages.RecordSaveException, ex.Message));
            }

            return View(bvcAgreement);
        }

        /// <summary>
        /// Return edit view with particular bvc agreement record
        /// </summary>
        /// <param name="id">bvc mapping id</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.BvcAgreementSetupEditOrDelete)]
        public ActionResult Edit(string id) // string
        {
            var bvcAgreement = _bvcAgreementManager.GetBVCAgreementDetails(id);
            return View(bvcAgreement);
        }

        //
        // POST: /Masters/Bvc Agreement/Edit/
        /// <summary>
        /// Edits the specified bvc agreement.
        /// </summary>
        /// <param name="bvcAgreement">Updated bvc agreement</param>
        /// <returns></returns>
         [ISAuthorize(Business.Security.Permissions.Masters.Masters.BvcAgreementSetupEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BvcAgreement bvcAgreement)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bvcAgreement.LastUpdatedBy = SessionUtil.UserId;
                    _bvcAgreementManager.UpdateBVCAgreement(bvcAgreement);

                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(string.Format(Messages.RecordSaveException, ex.Message));
            }

            return View(bvcAgreement);
        }

        /// <summary>
        /// Indexes the specified bvc agreement.
        /// </summary>
        /// <param name="bvcAgreement"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.BvcAgreementSetupQuery)]
        public ActionResult Index(BvcAgreement bvcAgreement)
        {
            var bvcAgreementGrid = new BvcAgreementSearch("SearchBvcAgreementGrid", Url.Action("BVCAgreementSearchGridData", new { bvcAgreement.BillingMemberId, bvcAgreement.BilledMemberId }));
            ViewData["BvcAgreementGrid"] = bvcAgreementGrid.Instance;
            return View();
        }

        /// <summary>
        /// Returns the search grid data.
        /// </summary>
        /// <param name="billingMemberId">Billing Member ID</param>
        /// <param name="billedMemberId">Billed Member ID</param>
        /// <returns></returns>
        public JsonResult BVCAgreementSearchGridData(int billingMemberId = 0, int billedMemberId = 0)
        {

            var bvcAgreementGrid = new BvcAgreementSearch("SearchBvcAgreementGrid",
                                                          Url.Action("BVCAgreementSearchGridData",
                                                                     new {billingMemberId, billedMemberId}));

            var bvcAgreement = _bvcAgreementManager.GetBVCAgreementList(billingMemberId, billedMemberId);
            try
            {
                return bvcAgreementGrid.DataBind(bvcAgreement.AsQueryable());

            }
            catch (ISBusinessException be)
            {
                ViewData["errorMessage"] = be.ErrorCode;
                return null;
            }
        }

        //
        // GET: /Masters/Bvc Agreement/Delete/
        /// <summary>
        /// Deletes the specified Bvc Agreement.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.BvcAgreementSetupEditOrDelete)]
        [HttpPost]
        public ActionResult Active(string id)
        {
            UIMessageDetail details;
            var _message = string.Empty;
            try
            {
                var isActivated = _bvcAgreementManager.ActiveDeactiveBVCAgreement(id);
                details = new UIMessageDetail { IsFailed = !isActivated, Message = _message };
            }
            catch (ISBusinessException ex)
            {
                details = new UIMessageDetail { IsFailed = true, Message = ShowMessageText(ex.ErrorCode) };
            }
            catch(Exception ex)
            {
                _message = ex.Message;
                details = new UIMessageDetail { IsFailed = true, Message = _message };
            }

            return Json(details);
        }
    }
}
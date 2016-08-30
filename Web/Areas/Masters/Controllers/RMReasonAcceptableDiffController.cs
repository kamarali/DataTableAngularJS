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
using Iata.IS.Core.DI;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class RMReasonAcceptableDiffController : ISController
    {
        private readonly IRMReasonAcceptableDiffManager _RMReasonAcceptableDiffManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="RMReasonAcceptableDiffController"/> class.
        /// </summary>
        /// <param name="rmReasonAcceptableDiffManager">The rm reason acceptable diff manager.</param>
        public RMReasonAcceptableDiffController(IRMReasonAcceptableDiffManager rmReasonAcceptableDiffManager)
         {
             _RMReasonAcceptableDiffManager = rmReasonAcceptableDiffManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.PaxRMReasonQuery)]
        public ActionResult Index()
        {
            
            const int ReasonCodeId = 0;
            const int TransactionTypeId = 0;
            const string EffectiveFrom = "", EffectiveTo= "";
            var rmReasonAcceptableDiffGrid = new RMReasonAcceptableDiffSearch("SearchRMReasonAcceptableDiffGrid", Url.Action("RMReasonAcceptableDiffSearchGridData", "RMReasonAcceptableDiff", new { ReasonCodeId,TransactionTypeId, EffectiveFrom, EffectiveTo }));
            ViewData["RMReasonAcceptableDiffGrid"] = rmReasonAcceptableDiffGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified rm reason acceptable diff.
        /// </summary>
        /// <param name="rmReasonAcceptableDiff">The rm reason acceptable diff.</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.PaxRMReasonQuery)]
        [HttpPost]
        public ActionResult Index(RMReasonAcceptableDiff rmReasonAcceptableDiff)
        {
            SessionUtil.CurrentPageSelected = 1;
            var rmReasonAcceptableDiffsGrid = new RMReasonAcceptableDiffSearch("SearchRMReasonAcceptableDiffGrid", Url.Action("RMReasonAcceptableDiffSearchGridData", new { rmReasonAcceptableDiff.ReasonCodeId, rmReasonAcceptableDiff.TransactionTypeId, rmReasonAcceptableDiff.EffectiveFrom, rmReasonAcceptableDiff.EffectiveTo }));
            ViewData["RMReasonAcceptableDiffGrid"] = rmReasonAcceptableDiffsGrid.Instance;

            return View(rmReasonAcceptableDiff);
        }

        /// <summary>
        /// RMs the reason acceptable diff search grid data.
        /// </summary>
        /// <param name="ReasonCodeId">The reason code id.</param>
        /// <param name="EffectiveFrom">The effective from.</param>
        /// <param name="EffectiveTo">The effective to.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.PaxRMReasonQuery)]
        public JsonResult RMReasonAcceptableDiffSearchGridData(int ReasonCodeId, int TransactionTypeId, string EffectiveFrom, string EffectiveTo)
        {

            var rmReasonAcceptableDiffsGrid = new RMReasonAcceptableDiffSearch("SearchRMReasonAcceptableDiffGrid", Url.Action("RMReasonAcceptableDiffSearchGridData", new { ReasonCodeId,TransactionTypeId, EffectiveFrom, EffectiveTo }));
            var rmReasonAcceptableDiffs = _RMReasonAcceptableDiffManager.GetRMReasonAcceptableDiffList(ReasonCodeId,TransactionTypeId, EffectiveFrom, EffectiveTo);
            try
            {
                return rmReasonAcceptableDiffsGrid.DataBind(rmReasonAcceptableDiffs.AsQueryable());

            }
            catch (ISBusinessException be)
            {
                ViewData["errorMessage"] = be.ErrorCode;
                return null;
            }
        }

        /// <summary>
        /// Gets the reason code list.
        /// </summary>
        /// <param name="TransactionTypeId">The transaction type id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.PaxRMReasonQuery)]
        public JsonResult GetReasonCodeList(string TransactionTypeId)
        {
            int transactionTypeId = 0;
            List<ReasonCode> ReasonCodeList = new List<ReasonCode>();
            if (!string.IsNullOrEmpty(TransactionTypeId))
            {
                transactionTypeId = Convert.ToInt32(TransactionTypeId);
                ReasonCodeList = Ioc.Resolve<IReasonCodeManager>(typeof(IReasonCodeManager)).GetReasonCodeList("", transactionTypeId).OrderBy(r=>r.Code).ToList();
            }
            var result = new JsonResult();
            result.Data = ReasonCodeList;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        /// <summary>
        /// Detailses the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.PaxRMReasonQuery)]
        public ActionResult Details(int id)
        {
            return View();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.PaxRMReasonEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified rm reason acceptable diff.
        /// </summary>
        /// <param name="rmReasonAcceptableDiff">The rm reason acceptable diff.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.PaxRMReasonEditOrDelete)]
        [HttpPost]
        public ActionResult Create(RMReasonAcceptableDiff rmReasonAcceptableDiff,FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    if (Convert.ToInt32(rmReasonAcceptableDiff.EffectiveFrom) > Convert.ToInt32(rmReasonAcceptableDiff.EffectiveTo) )
                    {
                        throw new ISBusinessException(ErrorCodes.InvalidEffectiveFromDate);
                    }
                    var createRMReasonAcceptableDiff = _RMReasonAcceptableDiffManager.AddRMReasonAcceptableDiff(rmReasonAcceptableDiff);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(rmReasonAcceptableDiff);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(rmReasonAcceptableDiff);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.PaxRMReasonEditOrDelete)]
        public ActionResult Edit(int id)
        {
            RMReasonAcceptableDiff rmReasonAcceptableDiff = _RMReasonAcceptableDiffManager.GetRMReasonAcceptableDiffDetails(id);
            var reasonCode = Ioc.Resolve<IReasonCodeManager>(typeof(IReasonCodeManager)).GetReasonCodeDetails(rmReasonAcceptableDiff.ReasonCodeId);
            if (reasonCode != null)
                rmReasonAcceptableDiff.TransactionTypeId = reasonCode.TransactionTypeId;
            return View(rmReasonAcceptableDiff);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="rmReasonAcceptableDiff">The rm reason acceptable diff.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.PaxRMReasonEditOrDelete)]
        [HttpPost]
        public ActionResult Edit(int id,RMReasonAcceptableDiff rmReasonAcceptableDiff, FormCollection collection)
        {
            try
            {
                rmReasonAcceptableDiff.Id = id;
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    if (Convert.ToInt32(rmReasonAcceptableDiff.EffectiveFrom) > Convert.ToInt32(rmReasonAcceptableDiff.EffectiveTo))
                    {
                        throw new ISBusinessException(ErrorCodes.InvalidEffectiveFromDate);
                    }
                    var UpdateRMReasonAcceptableDiff = _RMReasonAcceptableDiffManager.UpdateRMReasonAcceptableDiff(rmReasonAcceptableDiff);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(rmReasonAcceptableDiff);
                }
                
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(rmReasonAcceptableDiff);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.PaxRMReasonEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteRMReasonAcceptableDiff = _RMReasonAcceptableDiffManager.DeleteRMReasonAcceptableDiff(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

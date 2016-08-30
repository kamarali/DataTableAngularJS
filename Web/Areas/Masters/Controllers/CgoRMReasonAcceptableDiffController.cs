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
    public class CgoRMReasonAcceptableDiffController : ISController
    {
        private readonly ICgoRMReasonAcceptableDiffManager _CgoRMReasonAcceptableDiffManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CgoRMReasonAcceptableDiffController"/> class.
        /// </summary>
        /// <param name="cgoRMReasonAcceptableDiffManager">The cgoRMReasonAcceptableDiff manager.</param>
        public CgoRMReasonAcceptableDiffController(ICgoRMReasonAcceptableDiffManager cgoRMReasonAcceptableDiffManager)
         {
             _CgoRMReasonAcceptableDiffManager = cgoRMReasonAcceptableDiffManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CgoRMReasonQuery)]
        public ActionResult Index()
        {
            
            const int ReasonCodeId =0;
            const int TransactionTypeId = 0;
            const string EffectiveFrom="", EffectiveTo= "";
            var cgoRMReasonAcceptableDiffGrid = new CgoRMReasonAcceptableDiffSearch("SearchCgoRMReasonAcceptableDiffGrid", Url.Action("CgoRMReasonAcceptableDiffSearchGridData", "CgoRMReasonAcceptableDiff", new { ReasonCodeId,TransactionTypeId, EffectiveFrom, EffectiveTo }));
            ViewData["CgoRMReasonAcceptableDiffGrid"] = cgoRMReasonAcceptableDiffGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified cgoRMReasonAcceptableDiff.
        /// </summary>
        /// <param name="cgoRMReasonAcceptableDiff">The cgoRMReasonAcceptableDiff.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CgoRMReasonQuery)]
        [HttpPost]
        public ActionResult Index(CgoRMReasonAcceptableDiff cgoRMReasonAcceptableDiff)
        {
            SessionUtil.CurrentPageSelected = 1;
            var cgoRMReasonAcceptableDiffsGrid = new CgoRMReasonAcceptableDiffSearch("SearchCgoRMReasonAcceptableDiffGrid", Url.Action("CgoRMReasonAcceptableDiffSearchGridData", new { cgoRMReasonAcceptableDiff.ReasonCodeId, cgoRMReasonAcceptableDiff.TransactionTypeId, cgoRMReasonAcceptableDiff.EffectiveFrom, cgoRMReasonAcceptableDiff.EffectiveTo }));
            ViewData["CgoRMReasonAcceptableDiffGrid"] = cgoRMReasonAcceptableDiffsGrid.Instance;
            return View(cgoRMReasonAcceptableDiff);
        }

        /// <summary>
        /// Currencies the search grid data.
        /// </summary>
        /// <param name="Code">The code.</param>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CgoRMReasonQuery)]
        public JsonResult CgoRMReasonAcceptableDiffSearchGridData(int ReasonCodeId,int TransactionTypeId, string EffectiveFrom, string EffectiveTo)
        {

            var cgoRMReasonAcceptableDiffsGrid = new CgoRMReasonAcceptableDiffSearch("SearchCgoRMReasonAcceptableDiffGrid", Url.Action("CgoRMReasonAcceptableDiffSearchGridData", new { ReasonCodeId, EffectiveFrom, EffectiveTo }));
            var cgoRMReasonAcceptableDiffs = _CgoRMReasonAcceptableDiffManager.GetCgoRMReasonAcceptableDiffList(ReasonCodeId,TransactionTypeId, EffectiveFrom, EffectiveTo);
            try
            {
                return cgoRMReasonAcceptableDiffsGrid.DataBind(cgoRMReasonAcceptableDiffs.AsQueryable());

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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CgoRMReasonQuery)]
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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CgoRMReasonQuery)]
        public ActionResult Details(int id)
        {
            return View();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified cgoRMReasonAcceptableDiff.
        /// </summary>
        /// <param name="cgoRMReasonAcceptableDiff">The cgoRMReasonAcceptableDiff.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CgoRMReasonEditOrDelete)]
        [HttpPost]
        public ActionResult Create(CgoRMReasonAcceptableDiff cgoRMReasonAcceptableDiff,FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    if (Convert.ToInt32(cgoRMReasonAcceptableDiff.EffectiveFrom) > Convert.ToInt32(cgoRMReasonAcceptableDiff.EffectiveTo))
                    {
                        throw new ISBusinessException(ErrorCodes.InvalidEffectiveFromDate);
                    }
                    var createCgoRMReasonAcceptableDiff = _CgoRMReasonAcceptableDiffManager.AddCgoRMReasonAcceptableDiff(cgoRMReasonAcceptableDiff);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(cgoRMReasonAcceptableDiff);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(cgoRMReasonAcceptableDiff);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CgoRMReasonEditOrDelete)]
        public ActionResult Edit(int id)
        {
            CgoRMReasonAcceptableDiff cgoRMReasonAcceptableDiff = _CgoRMReasonAcceptableDiffManager.GetCgoRMReasonAcceptableDiffDetails(id);
            var reasonCode = Ioc.Resolve<IReasonCodeManager>(typeof(IReasonCodeManager)).GetReasonCodeDetails(cgoRMReasonAcceptableDiff.ReasonCodeId);
            if (reasonCode != null)
                cgoRMReasonAcceptableDiff.TransactionTypeId = reasonCode.TransactionTypeId;
            return View(cgoRMReasonAcceptableDiff);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="cgoRMReasonAcceptableDiff">The cgoRMReasonAcceptableDiff.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CgoRMReasonEditOrDelete)]
        [HttpPost]
        public ActionResult Edit(int id,CgoRMReasonAcceptableDiff cgoRMReasonAcceptableDiff, FormCollection collection)
        {
            try
            {
                cgoRMReasonAcceptableDiff.Id = id;
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    if (Convert.ToInt32(cgoRMReasonAcceptableDiff.EffectiveFrom) > Convert.ToInt32(cgoRMReasonAcceptableDiff.EffectiveTo))
                    {
                        throw new ISBusinessException(ErrorCodes.InvalidEffectiveFromDate);
                    }
                    var UpdateCgoRMReasonAcceptableDiff = _CgoRMReasonAcceptableDiffManager.UpdateCgoRMReasonAcceptableDiff(cgoRMReasonAcceptableDiff);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(cgoRMReasonAcceptableDiff);
                }
                
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(cgoRMReasonAcceptableDiff);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CgoRMReasonEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteCgoRMReasonAcceptableDiff = _CgoRMReasonAcceptableDiffManager.DeleteCgoRMReasonAcceptableDiff(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

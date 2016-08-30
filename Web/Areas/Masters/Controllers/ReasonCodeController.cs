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
    public class ReasonCodeController : ISController
    {
        private readonly IReasonCodeManager _ReasonCodeManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReasonCodeController"/> class.
        /// </summary>
        /// <param name="reasonCodeManager">The reason code manager.</param>
        public ReasonCodeController(IReasonCodeManager reasonCodeManager)
         {
             _ReasonCodeManager = reasonCodeManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ReasonCodeQuery)]
        public ActionResult Index()
        {
            
            const int TransactionTypeId = 0;
            const string Code = "";
            var reasonCodeGrid = new ReasonCodeSearch("SearchReasonCodeGrid", Url.Action("ReasonCodeSearchGridData", "ReasonCode", new { Code, TransactionTypeId }));
            ViewData["ReasonCodeGrid"] = reasonCodeGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified reason code.
        /// </summary>
        /// <param name="reasonCode">The reason code.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ReasonCodeQuery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ReasonCode reasonCode)
        {
            if (!string.IsNullOrEmpty(reasonCode.Description))
            {
              reasonCode.Description = reasonCode.Description.Trim();
            }

            SessionUtil.CurrentPageSelected = 1;
            var reasonCodesGrid = new ReasonCodeSearch("SearchReasonCodeGrid", Url.Action("ReasonCodeSearchGridData", new { reasonCode.Code, reasonCode.TransactionTypeId }));
            ViewData["ReasonCodeGrid"] = reasonCodesGrid.Instance;
            return View();
        }

        /// <summary>
        /// Reasons the code search grid data.
        /// </summary>
        /// <param name="Code">The code.</param>
        /// <param name="TransactionTypeId">The transaction type id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ReasonCodeQuery)]
        public JsonResult ReasonCodeSearchGridData(string Code, int TransactionTypeId)
        {

            var reasonCodesGrid = new ReasonCodeSearch("SearchReasonCodeGrid", Url.Action("ReasonCodeSearchGridData", new { Code, TransactionTypeId }));
            var reasonCodes = _ReasonCodeManager.GetReasonCodeList(Code, TransactionTypeId);
            try
            {
                return reasonCodesGrid.DataBind(reasonCodes.AsQueryable());

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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ReasonCodeQuery)]
        public ActionResult Details(int id)
        {
            ReasonCode reasonCode = _ReasonCodeManager.GetAllReasonCodeList().Where(r=>r.Id==id).FirstOrDefault();
            return View(reasonCode);
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
         [ISAuthorize(Business.Security.Permissions.Masters.Masters.ReasonCodeEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified reason code.
        /// </summary>
        /// <param name="reasonCode">The reason code.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
         [ISAuthorize(Business.Security.Permissions.Masters.Masters.ReasonCodeEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReasonCode reasonCode, FormCollection collection)
        {
            try
            {
                if (!string.IsNullOrEmpty(reasonCode.Description))
                {
                  reasonCode.Description = reasonCode.Description.Trim();
                  if (reasonCode.Description.Length > 255)
                    reasonCode.Description = reasonCode.Description.Substring(0, 255);
                }
                reasonCode.Code = reasonCode.Code.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createReasonCode = _ReasonCodeManager.AddReasonCode(reasonCode);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(reasonCode);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(reasonCode);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(string.Format(Messages.RecordSaveException, ex.Message));
                return View(reasonCode);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
         [ISAuthorize(Business.Security.Permissions.Masters.Masters.ReasonCodeEditOrDelete)]
        public ActionResult Edit(int id)
        {
            ReasonCode reasonCode = _ReasonCodeManager.GetReasonCodeDetails(id);
            return View(reasonCode);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="reasonCode">The reason code.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ReasonCodeEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ReasonCode reasonCode, FormCollection collection)
        {
            try
            {
                reasonCode.Id = id;
                reasonCode.Code = reasonCode.Code.ToUpper();
                if (!string.IsNullOrEmpty(reasonCode.Description))
                {
                  reasonCode.Description = reasonCode.Description.Trim();
                  if (reasonCode.Description.Length > 255)
                    reasonCode.Description = reasonCode.Description.Substring(0, 255);
                }
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    var UpdateReasonCode = _ReasonCodeManager.UpdateReasonCode(reasonCode);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(reasonCode);
                }

            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(reasonCode);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(string.Format(Messages.RecordSaveException, ex.Message));
                return View(reasonCode);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ReasonCodeEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteReasonCode = _ReasonCodeManager.DeleteReasonCode(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

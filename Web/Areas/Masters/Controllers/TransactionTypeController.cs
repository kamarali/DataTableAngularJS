using System.Collections.Generic;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using System;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class TransactionTypeController : ISController
    {
        private readonly ITransactionTypeManager _TransactionTypeManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionTypeController"/> class.
        /// </summary>
        /// <param name="transactionTypeManager">The transaction type manager.</param>
        public TransactionTypeController(ITransactionTypeManager transactionTypeManager)
         {
             _TransactionTypeManager = transactionTypeManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TransactionTypeQuery)]
        public ActionResult Index()
        {
            
            const int BillingCategoryCode=0;
            const string Description = "";
            const string Name = "";
            var transactionTypeGrid = new TransactionTypeSearch("SearchTransactionTypeGrid", Url.Action("TransactionTypeSearchGridData", "TransactionType", new { BillingCategoryCode,Description,Name }));
            ViewData["TransactionTypeGrid"] = transactionTypeGrid.Instance;
           
            return View();
        }

        /// <summary>
        /// Indexes the specified transaction type.
        /// </summary>
        /// <param name="transactionType">Type of the transaction.</param>
        /// <returns></returns>
         [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TransactionTypeQuery)]
        [HttpPost]
        public ActionResult Index(TransactionType transactionType)
        {
            if (!string.IsNullOrEmpty(transactionType.Description))
            {
              transactionType.Description = transactionType.Description.Trim();
            }

            SessionUtil.CurrentPageSelected = 1;
            var transactionTypesGrid = new TransactionTypeSearch("SearchTransactionTypeGrid", Url.Action("TransactionTypeSearchGridData", new { transactionType.BillingCategoryCode, transactionType.Description, transactionType.Name }));
            ViewData["TransactionTypeGrid"] = transactionTypesGrid.Instance;
           
            return View();
        }

        /// <summary>
        /// Transactions the type search grid data.
        /// </summary>
        /// <param name="BillingCategoryCode">The billing category code.</param>
        /// <param name="Description">The description.</param>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TransactionTypeQuery)]
        public JsonResult TransactionTypeSearchGridData(int BillingCategoryCode,string Description, string Name)
        {

            var transactionTypesGrid = new TransactionTypeSearch("SearchTransactionTypeGrid", Url.Action("TransactionTypeSearchGridData", new { BillingCategoryCode, Description, Name }));
            var transactionTypes = _TransactionTypeManager.GetTransactionTypeList(BillingCategoryCode, Description, Name);
            try
            {
                return transactionTypesGrid.DataBind(transactionTypes.AsQueryable());

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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TransactionTypeQuery)]
        public ActionResult Details(int id)
        {
            TransactionType transactionType = _TransactionTypeManager.GetTransactionTypeDetails(id);
            return View(transactionType);
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TransactionTypeEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified transaction type.
        /// </summary>
        /// <param name="transactionType">Type of the transaction.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TransactionTypeEditOrDelete)]
        [HttpPost]
        public ActionResult Create(TransactionType transactionType, FormCollection collection)
        {
            try
            {
                if (!string.IsNullOrEmpty(transactionType.Description))
                {
                  transactionType.Description = transactionType.Description.Trim();
                  if (transactionType.Description.Length > 255)
                    transactionType.Description = transactionType.Description.Substring(0, 255);
                }
                transactionType.Name = transactionType.Name.ToUpper();
                transactionType.Description = transactionType.Description.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createTransactionType = _TransactionTypeManager.AddTransactionType(transactionType);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(transactionType);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(transactionType);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(string.Format(Messages.RecordSaveException, ex.Message));
                return View(transactionType);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TransactionTypeEditOrDelete)]
        public ActionResult Edit(int id)
        {
            TransactionType transactionType = _TransactionTypeManager.GetTransactionTypeDetails(id);
            return View(transactionType);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="transactionType">Type of the transaction.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TransactionTypeEditOrDelete)]
        [HttpPost]
        public ActionResult Edit(int id, TransactionType transactionType, FormCollection collection)
        {
            try
            {
                if (!string.IsNullOrEmpty(transactionType.Description))
                {
                  transactionType.Description = transactionType.Description.Trim();
                  if (transactionType.Description.Length > 255)
                    transactionType.Description = transactionType.Description.Substring(0, 255);
                }
                transactionType.Id = id;
                transactionType.Name = transactionType.Name.ToUpper();
                transactionType.Description = transactionType.Description.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    var UpdateTransactionType = _TransactionTypeManager.UpdateTransactionType(transactionType);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(transactionType);
                }

            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(transactionType);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(string.Format(Messages.RecordSaveException, ex.Message));
                return View(transactionType);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TransactionTypeEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteTransactionType = _TransactionTypeManager.DeleteTransactionType(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

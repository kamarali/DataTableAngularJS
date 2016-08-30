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

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class BillingCategoryController : ISController
    {
        private readonly IBillingCategoryManager _BillingCategoryManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BillingCategoryController"/> class.
        /// </summary>
        /// <param name="billingCategoryManager">The billing category manager.</param>
        public BillingCategoryController(IBillingCategoryManager billingCategoryManager)
         {
             _BillingCategoryManager = billingCategoryManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            const string description = "";
            const string codeIsxml = "";
            var billingCategoryGrid = new BillingCategorySearch("SearchBillingCategoryGrid", Url.Action("BillingCategorySearchGridData", "BillingCategory", new { codeIsxml, description }));
            ViewData["BillingCategoryGrid"] = billingCategoryGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified billing category.
        /// </summary>
        /// <param name="billingCategory">The billing category.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(BillingCategory billingCategory)
        {
            SessionUtil.CurrentPageSelected = 1;
            var billingCategorysGrid = new BillingCategorySearch("SearchBillingCategoryGrid", Url.Action("BillingCategorySearchGridData", new { billingCategory.CodeIsxml,billingCategory.Description }));
            ViewData["BillingCategoryGrid"] = billingCategorysGrid.Instance;
            return View();
        }

        /// <summary>
        /// Billings the category search grid data.
        /// </summary>
        /// <param name="CodeIsxml">The code isxml.</param>
        /// <param name="Description">The description.</param>
        /// <returns></returns>
        public JsonResult BillingCategorySearchGridData(string CodeIsxml, string Description)
        {

            var billingCategorysGrid = new BillingCategorySearch("SearchBillingCategoryGrid", Url.Action("BillingCategorySearchGridData", new { CodeIsxml, Description }));
            var billingCategorys = _BillingCategoryManager.GetBillingCategoryList(CodeIsxml, Description);
            try
            {
                return billingCategorysGrid.DataBind(billingCategorys.AsQueryable());

            }
            catch (ISBusinessException be)
            {
                ViewData["errorMessage"] = be.ErrorCode;
                return null;
            }
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
        /// Creates the specified billing category.
        /// </summary>
        /// <param name="billingCategory">The billing category.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(BillingCategory billingCategory,FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createBillingCategory = _BillingCategoryManager.AddBillingCategory(billingCategory);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(billingCategory);
                }
            }
            catch
            {
                return View(billingCategory);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            BillingCategory billingCategory = _BillingCategoryManager.GetBillingCategoryDetails(id);
            return View(billingCategory);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="billingCategory">The billing category.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id,BillingCategory billingCategory, FormCollection collection)
        {
            try
            {
                billingCategory.Id = id;
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    var UpdateBillingCategory = _BillingCategoryManager.UpdateBillingCategory(billingCategory);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(billingCategory);
                }
                
            }
            catch
            {
                return View(billingCategory);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteBillingCategory = _BillingCategoryManager.DeleteBillingCategory(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

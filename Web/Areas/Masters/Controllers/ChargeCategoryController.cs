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

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class ChargeCategoryController : ISController
    {
        private readonly IChargeCategoryManager _ChargeCategoryManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChargeCategoryController"/> class.
        /// </summary>
        /// <param name="chargeCategoryManager">The charge category manager.</param>
        public ChargeCategoryController(IChargeCategoryManager chargeCategoryManager)
         {
             _ChargeCategoryManager = chargeCategoryManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            const string Name = "";
            const int BillingCategoryId = 0;
            var chargeCategoryGrid = new ChargeCategorySearch("SearchChargeCategoryGrid", Url.Action("ChargeCategorySearchGridData", "ChargeCategory", new { Name, BillingCategoryId }));
            ViewData["ChargeCategoryGrid"] = chargeCategoryGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified charge category.
        /// </summary>
        /// <param name="chargeCategory">The charge category.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(ChargeCategory chargeCategory)
        {
            SessionUtil.CurrentPageSelected = 1;
            var chargeCategoryGrid = new ChargeCategorySearch("SearchChargeCategoryGrid", Url.Action("ChargeCategorySearchGridData", "ChargeCategory", new {  chargeCategory.Name, chargeCategory.BillingCategoryId }));
            ViewData["ChargeCategoryGrid"] = chargeCategoryGrid.Instance;
            return View();
        }

        /// <summary>
        /// Charges the category search grid data.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="BillingCategoryId">The billing category id.</param>
        /// <returns></returns>
        public JsonResult ChargeCategorySearchGridData(string Name, int BillingCategoryId)
        {
            var chargeCategoryGrid = new ChargeCategorySearch("SearchChargeCategoryGrid", Url.Action("ChargeCategorySearchGridData", new { Name, BillingCategoryId }));
            var chargeCategorys = _ChargeCategoryManager.GetChargeCategoryList(Name, BillingCategoryId);
            try
            {
                return chargeCategoryGrid.DataBind(chargeCategorys.AsQueryable());
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
        /// Creates the specified charge category.
        /// </summary>
        /// <param name="chargeCategory">The charge category.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(ChargeCategory chargeCategory,FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createchargeCategory = _ChargeCategoryManager.AddChargeCategory(chargeCategory);
                    ShowSuccessMessage("Charge Category details saved successfully");
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(chargeCategory);
                }
            }
            catch
            {
                return View(chargeCategory);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        public ActionResult Edit(int Id)
        {
            ChargeCategory chargeCategory = _ChargeCategoryManager.GetChargeCategoryDetails(Id);
            return View(chargeCategory);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="chargeCategory">The charge category.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int Id, ChargeCategory chargeCategory, FormCollection collection)
        {
            try
            {
                chargeCategory.Id = Id;
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    ChargeCategory UpdatedchargeCategory = _ChargeCategoryManager.UpdateChargeCategory(chargeCategory);
                    ShowSuccessMessage("Charge Category details saved successfully");
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(chargeCategory);
                }
            }
            catch
            {
                return View(chargeCategory);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int Id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var countryDelete = _ChargeCategoryManager.DeleteChargeCategory(Id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
    }
}

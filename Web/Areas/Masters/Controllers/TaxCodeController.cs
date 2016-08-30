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
    public class TaxCodeController : ISController
    {
        private readonly ITaxCodeManager _TaxCodeManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxCodeController"/> class.
        /// </summary>
        /// <param name="taxCodeManager">The tax code manager.</param>
        public TaxCodeController(ITaxCodeManager taxCodeManager)
        {
            _TaxCodeManager = taxCodeManager;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TaxCodeQuery)]
        public ActionResult Index()
        {
            const string TaxCodeId = "";
            const string Description = "";
            var taxCodeGrid = new TaxCodeSearch("SearchTaxCodeGrid", Url.Action("TaxCodeSearchGridData", "TaxCode", new { TaxCodeId, Description }));
            ViewData["TaxCodeGrid"] = taxCodeGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified tax code.
        /// </summary>
        /// <param name="taxCode">The tax code.</param>
        /// <returns></returns>
       [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TaxCodeQuery)]
        [HttpPost]
        public ActionResult Index(TaxCode taxCode)
        {
            if (!string.IsNullOrEmpty(taxCode.Description))
            {
                taxCode.Description = taxCode.Description.Trim();
            }
            SessionUtil.CurrentPageSelected = 1;
            var taxCodeGrid = new TaxCodeSearch("SearchTaxCodeGrid", Url.Action("TaxCodeSearchGridData", "TaxCode", new { taxCode.Id, taxCode.Description }));
            ViewData["TaxCodeGrid"] = taxCodeGrid.Instance;

            return View(taxCode);
        }

        /// <summary>
        /// Taxes the code search grid data.
        /// </summary>
        /// <param name="TaxCodeTypeId">The tax code type id.</param>
        /// <param name="Description">The description.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TaxCodeQuery)]
        public JsonResult TaxCodeSearchGridData(string Id, string Description)
        {
            var taxCodeGrid = new TaxCodeSearch("SearchTaxCodeGrid", Url.Action("TaxCodeSearchGridData", new { Id, Description }));
            var taxCodes = _TaxCodeManager.GetTaxCodeList(Id, Description);
            try
            {
                return taxCodeGrid.DataBind(taxCodes.AsQueryable());
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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TaxCodeEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified tax code.
        /// </summary>
        /// <param name="taxCode">The tax code.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
       [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TaxCodeEditOrDelete)]
        [HttpPost]
        public ActionResult Create(TaxCode taxCode, FormCollection collection)
        {
            try
            {
                taxCode.Id = taxCode.Id.ToUpper();
                taxCode.TaxCodeTypeId = taxCode.TaxCodeTypeId.ToUpper();
                if (!string.IsNullOrEmpty(taxCode.Description))
                {
                    taxCode.Description = taxCode.Description.Trim();
                    if (taxCode.Description.Length > 255)
                      taxCode.Description = taxCode.Description.Substring(0, 255);
                }
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createCountry = _TaxCodeManager.AddTaxCode(taxCode);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(taxCode);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(taxCode);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TaxCodeEditOrDelete)]
        public ActionResult Edit(string Id)
        {
            TaxCode taxCode = _TaxCodeManager.GetTaxCodeDetails(Id);
            return View(taxCode);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="taxCode">The tax code.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
       [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TaxCodeEditOrDelete)]
        [HttpPost]
        public ActionResult Edit(string Id, TaxCode taxCode, FormCollection collection)
        {
            try
            {
                taxCode.Id = Id.ToUpper();
                taxCode.TaxCodeTypeId = taxCode.TaxCodeTypeId.ToUpper();
                if (!string.IsNullOrEmpty(taxCode.Description))
                {
                    taxCode.Description = taxCode.Description.Trim();
                    if (taxCode.Description.Length > 255)
                      taxCode.Description = taxCode.Description.Substring(0, 255);
                }
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    TaxCode UpdatedtaxCode = _TaxCodeManager.UpdateTaxCode(taxCode);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(taxCode);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(taxCode);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TaxCodeEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(string Id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var countryDelete = _TaxCodeManager.DeleteTaxCode(Id);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}

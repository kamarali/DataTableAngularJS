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
    public class VatIdentifierController : ISController
    {
        private readonly IVatIdentifierManager _VatIdentifierManager = null;

        public VatIdentifierController(IVatIdentifierManager vatIdentifierManager)
        {
            _VatIdentifierManager = vatIdentifierManager;
        }

        [ISAuthorize(Business.Security.Permissions.Masters.Masters.VatIdentifierQuery)]
        public ActionResult Index()
        {
            const int BillingCategoryCode = 0;
            const string Identifier = "", Description = "";
            var vatIdentifierGrid = new VatIdentifierSearch("SearchVatIdentifierGrid", Url.Action("VatIdentifierSearchGridData", "VatIdentifier", new { Identifier, BillingCategoryCode, Description }));
            ViewData["VatIdentifierGrid"] = vatIdentifierGrid.Instance;
            return View();
        }

       [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.VatIdentifierQuery)]
        [HttpPost]
        public ActionResult Index(VatIdentifier vatIdentifier)
        {
            if (!string.IsNullOrEmpty(vatIdentifier.Description))
            {
                vatIdentifier.Description = vatIdentifier.Description.Trim();
            }
            SessionUtil.CurrentPageSelected = 1;
            var vatIdentifiersGrid = new VatIdentifierSearch("SearchVatIdentifierGrid", Url.Action("VatIdentifierSearchGridData", new { vatIdentifier.Identifier, vatIdentifier.BillingCategoryCode, vatIdentifier.Description }));
            ViewData["VatIdentifierGrid"] = vatIdentifiersGrid.Instance;
            return View(vatIdentifier);
        }

        [ISAuthorize(Business.Security.Permissions.Masters.Masters.VatIdentifierQuery)]
        public JsonResult VatIdentifierSearchGridData(string Identifier, int BillingCategoryCode, string Description)
        {
            var vatIdentifiersGrid = new VatIdentifierSearch("SearchVatIdentifierGrid", Url.Action("VatIdentifierSearchGridData", new { Identifier, BillingCategoryCode, Description }));
            var vatIdentifiers = _VatIdentifierManager.GetVatIdentifierList(Identifier, BillingCategoryCode, Description);
            try
            {
                return vatIdentifiersGrid.DataBind(vatIdentifiers.AsQueryable());

            }
            catch (ISBusinessException be)
            {
                ViewData["errorMessage"] = be.ErrorCode;
                return null;
            }
        }

        [ISAuthorize(Business.Security.Permissions.Masters.Masters.VatIdentifierQuery)]
        public ActionResult Details(int id)
        {
            return View();
        }

        [ISAuthorize(Business.Security.Permissions.Masters.Masters.VatIdentifierEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

       [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.VatIdentifierEditOrDelete)]
        [HttpPost]
        public ActionResult Create(VatIdentifier vatIdentifier, FormCollection collection)
        {
            try
            {
                if (!string.IsNullOrEmpty(vatIdentifier.Description))
                {
                    vatIdentifier.Description = vatIdentifier.Description.Trim();
                    if (vatIdentifier.Description.Length > 255)
                      vatIdentifier.Description = vatIdentifier.Description.Substring(0, 255);
                }
                vatIdentifier.Identifier = vatIdentifier.Identifier.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createVatIdentifier = _VatIdentifierManager.AddVatIdentifier(vatIdentifier);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(vatIdentifier);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(vatIdentifier);
            }
        }

        [ISAuthorize(Business.Security.Permissions.Masters.Masters.VatIdentifierEditOrDelete)]
        public ActionResult Edit(int id)
        {
            VatIdentifier vatIdentifier = _VatIdentifierManager.GetVatIdentifierDetails(id);
            return View(vatIdentifier);
        }

       [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.VatIdentifierEditOrDelete)]
        [HttpPost]
        public ActionResult Edit(int id, VatIdentifier vatIdentifier, FormCollection collection)
        {
            try
            {                
                vatIdentifier.Id = id;
                if (!string.IsNullOrEmpty(vatIdentifier.Description))
                {
                    vatIdentifier.Description = vatIdentifier.Description.Trim();
                    if (vatIdentifier.Description.Length > 255)
                      vatIdentifier.Description = vatIdentifier.Description.Substring(0, 255);
                }
                vatIdentifier.Identifier = vatIdentifier.Identifier.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    var UpdateVatIdentifier = _VatIdentifierManager.UpdateVatIdentifier(vatIdentifier);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(vatIdentifier);
                }

            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(vatIdentifier);
            }
        }

        [ISAuthorize(Business.Security.Permissions.Masters.Masters.VatIdentifierEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteVatIdentifier = _VatIdentifierManager.DeleteVatIdentifier(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

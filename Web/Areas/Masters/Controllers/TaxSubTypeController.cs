using System.Collections.Generic;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using System;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class TaxSubTypeController: ISController
    {
        private readonly ITaxSubTypeManager _TaxSubTypeManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxSubTypeController"/> class.
        /// </summary>
        /// <param name="taxSubTypeManager">The tax sub type manager.</param>
        public TaxSubTypeController(ITaxSubTypeManager taxSubTypeManager)
         {
             _TaxSubTypeManager = taxSubTypeManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TaxSubTypeQuery)]
        public ActionResult Index()
        {
            
            const string taxsubtype = "";
            const string type = "";
            var taxSubTypeGrid = new TaxSubTypeSearch("SearchTaxSubTypeGrid", Url.Action("TaxSubTypeSearchGridData", "TaxSubType", new { taxsubtype, type }));
            ViewData["TaxSubTypeGrid"] = taxSubTypeGrid.Instance;
           
            return View();
        }

        /// <summary>
        /// Indexes the specified tax sub type.
        /// </summary>
        /// <param name="taxSubType">Type of the tax sub.</param>
        /// <returns></returns>
       [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TaxSubTypeQuery)]
        [HttpPost]
        public ActionResult Index(TaxSubType taxSubType)
        {
            SessionUtil.CurrentPageSelected = 1;
            var taxSubTypesGrid = new TaxSubTypeSearch("SearchTaxSubTypeGrid", Url.Action("TaxSubTypeSearchGridData", new {taxSubType.SubType,taxSubType.Type }));
            ViewData["TaxSubTypeGrid"] = taxSubTypesGrid.Instance;
           
            return View();
        }

        /// <summary>
        /// Taxes the sub type search grid data.
        /// </summary>
        /// <param name="taxsubtype">The taxsubtype.</param>
        /// <param name="taxtype">The taxtype.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TaxSubTypeQuery)]
        public JsonResult TaxSubTypeSearchGridData(string SubType, string Type)
        {

            var taxSubTypesGrid = new TaxSubTypeSearch("SearchTaxSubTypeGrid", Url.Action("TaxSubTypeSearchGridData", new { SubType, Type }));
            var taxSubTypes = _TaxSubTypeManager.GetTaxSubTypeList(SubType, Type);
            try
            {
                return taxSubTypesGrid.DataBind(taxSubTypes.AsQueryable());

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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TaxSubTypeQuery)]
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
        /// Creates the specified tax sub type.
        /// </summary>
        /// <param name="taxSubType">Type of the tax sub.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
       [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TaxSubTypeEditOrDelete)]
        [HttpPost]
        public ActionResult Create(TaxSubType taxSubType,FormCollection collection)
        {
            try
            {
                taxSubType.SubType = taxSubType.SubType.ToUpper();
                taxSubType.Type = taxSubType.Type.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createTaxSubType = _TaxSubTypeManager.AddTaxSubType(taxSubType);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(taxSubType);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(taxSubType);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TaxSubTypeEditOrDelete)]
        public ActionResult Edit(int id)
        {
            TaxSubType taxSubType = _TaxSubTypeManager.GetTaxSubTypeDetails(id);
            return View(taxSubType);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="taxSubType">Type of the tax sub.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
       [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TaxSubTypeEditOrDelete)]
        [HttpPost]
        public ActionResult Edit(int id,TaxSubType taxSubType, FormCollection collection)
        {
            try
            {
                taxSubType.Id = id;
                taxSubType.SubType = taxSubType.SubType.ToUpper();
                taxSubType.Type = taxSubType.Type.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    var UpdateTaxSubType = _TaxSubTypeManager.UpdateTaxSubType(taxSubType);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(taxSubType);
                }
                
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(taxSubType);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.TaxSubTypeEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteTaxSubType = _TaxSubTypeManager.DeleteTaxSubType(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

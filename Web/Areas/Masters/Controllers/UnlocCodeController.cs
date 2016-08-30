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
using Iata.IS.Web.Util.Filters;


namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class UnlocCodeController : ISController
    {
        private readonly IUnlocCodeManager _UnlocCodeManager = null;

        public UnlocCodeController(IUnlocCodeManager unlocCodeManager)
         {
             _UnlocCodeManager = unlocCodeManager;
         }

        //
        // GET: /Masters/UnlocCode/

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.UNLocCodeQuery)]
        public ActionResult Index()
        {
            const string Id = "";
            const string Name = "";
            var unlocCodeGrid = new UnlocCodeSearch("SearchUnlocCodeGrid", Url.Action("UnlocCodeSearchGridData", "UnlocCode", new {Id,Name }));
            ViewData["UnlocCodeGrid"] = unlocCodeGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified unloc code.
        /// </summary>
        /// <param name="unlocCode">The unloc code.</param>
        /// <returns></returns>
       [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.UNLocCodeQuery)]
        [HttpPost]
        public ActionResult Index(UnlocCode unlocCode)
        {
            SessionUtil.CurrentPageSelected = 1;
            var unlocCodeGrid = new UnlocCodeSearch("SearchUnlocCodeGrid", Url.Action("UnlocCodeSearchGridData", "UnlocCode", new { unlocCode.Id, unlocCode.Name }));
            ViewData["UnlocCodeGrid"] = unlocCodeGrid.Instance;
           
            return View();
        }

        /// <summary>
        /// Unlocs the code search grid data.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.UNLocCodeQuery)]
        public JsonResult UnlocCodeSearchGridData( string Id,string Name)
        {
            var unlocCodeGrid = new UnlocCodeSearch("SearchUnlocCodeGrid", Url.Action("UnlocCodeSearchGridData", new { Id, Name }));
            var unlocCodes = _UnlocCodeManager.GetUnlocCodeList(Id, Name);
            try
            {
                return unlocCodeGrid.DataBind(unlocCodes.AsQueryable());
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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.UNLocCodeEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        } 

        /// <summary>
        /// Creates the specified unloc code.
        /// </summary>
        /// <param name="unlocCode">The unloc code.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
       [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.UNLocCodeEditOrDelete)]
        [HttpPost]
        public ActionResult Create(UnlocCode unlocCode,FormCollection collection)
        {
            try
            {
                unlocCode.Id = unlocCode.Id.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createCountry = _UnlocCodeManager.AddUnlocCode(unlocCode);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(unlocCode);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(unlocCode);
            }
        }
        
        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.UNLocCodeEditOrDelete)]
        public ActionResult Edit(string Id)
        {
            UnlocCode unlocCode = _UnlocCodeManager.GetUnlocCodeDetails(Id);
            return View(unlocCode);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="unlocCode">The unloc code.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
       [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.UNLocCodeEditOrDelete)]
        [HttpPost]
        public ActionResult Edit(string Id, UnlocCode unlocCode, FormCollection collection)
        {
            try
            {
                unlocCode.Id = Id;
                unlocCode.Id = unlocCode.Id.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    UnlocCode UpdatedunlocCode = _UnlocCodeManager.UpdateUnlocCode(unlocCode);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(unlocCode);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(unlocCode);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.UNLocCodeEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(string Id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var countryDelete = _UnlocCodeManager.DeleteUnlocCode(Id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
    }
}

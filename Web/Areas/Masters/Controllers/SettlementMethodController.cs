using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using System;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class SettlementMethodController:ISController
    {
        private readonly ISettlementMethodManager _settlementMethodManager = null;

        public  SettlementMethodController(ISettlementMethodManager settlementMethodManager)
        {
            _settlementMethodManager = settlementMethodManager;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SettlementMethodSetupQuery)]
        public ActionResult Index()
        {
            const string settlementMethodName = "";
            var settlementMethodtGrid = new SettlementMethodSearch("SearchSettlementMethodGrid", Url.Action("SettlementMethodSearchGridData", "SettlementMethod", new { settlementMethodName }));
            ViewData["SettlementMethodGrid"] = settlementMethodtGrid.Instance;
            return View();
        }
        /// <summary>
        /// Indexes the specified settlement method.
        /// </summary>
        /// <param name="settlementMethod">The settlement method.</param>
        /// <returns></returns>
         [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SettlementMethodSetupQuery)]
        [HttpPost]
        public ActionResult Index(SettlementMethod settlementMethod)
        {
            SessionUtil.CurrentPageSelected = 1;
            var settlementMethodtGrid = new SettlementMethodSearch("SearchSettlementMethodGrid", Url.Action("SettlementMethodSearchGridData", new { settlementMethodName = settlementMethod.Name }));
            ViewData["SettlementMethodGrid"] = settlementMethodtGrid.Instance;
            return View();
        }

        /// <summary>
        /// Settlement method search grid data.
        /// </summary>
        ///<param name="settlementMethodName">The settlmentMethod name</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SettlementMethodSetupQuery)]
        public JsonResult SettlementMethodSearchGridData(string settlementMethodName)
        {
            var settlementMethodtGrid = new SettlementMethodSearch("SearchSettlementMethodGrid", Url.Action("SettlementMethodSearchGridData", "SettlementMethod", new { settlementMethodName }));
            var settlementMethod = _settlementMethodManager.GetSettlementMethodList(settlementMethodName);
            try
            {
                return settlementMethodtGrid.DataBind(settlementMethod.AsQueryable());

            }
            catch (ISBusinessException be)
            {
                ViewData["errorMessage"] = be.ErrorCode;
                return null;
            }
        }
        /// <summary>
        /// Detailse the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SettlementMethodSetupQuery)]
        public ActionResult Details(int id)
        {
            return View();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SettlementMethodSetupEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// Creates the specified settlement method.
        /// </summary>
        /// <param name="settlementMethod">The settlement method.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SettlementMethodSetupEditOrDelete)]
        [HttpPost]
        public ActionResult Create(SettlementMethod settlementMethod, FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    settlementMethod.Name = settlementMethod.Name.ToUpper();
                    var createSettlementMethod = _settlementMethodManager.AddSettlementMethod(settlementMethod);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(settlementMethod);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(settlementMethod);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SettlementMethodSetupEditOrDelete)]
        public ActionResult Edit(int id)
        {
            SettlementMethod settlementMethod  = _settlementMethodManager.GetSettlementMethodDetails(id);
            return View(settlementMethod);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="settlementMethod">The min max acceptable amount.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SettlementMethodSetupEditOrDelete)]
        [HttpPost]
        public ActionResult Edit(int id, SettlementMethod settlementMethod, FormCollection collection)
        {
            try
            {
                settlementMethod.Id = id;
                if (ModelState.IsValid)
                {
                    
                    settlementMethod.Name = settlementMethod.Name.ToUpper();
                    var updateSettlementMethod = _settlementMethodManager.UpdateSettlementMethod(settlementMethod);
                    ShowSuccessMessage(Messages.RecordUpdateSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(settlementMethod);
                }

            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(settlementMethod);
            }
        }


        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SettlementMethodSetupEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteSettlementMethod = _settlementMethodManager.DeleteSettlementMethod(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
    }
}
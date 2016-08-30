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
    public class CurrencyController : ISController
    {
        private readonly ICurrencyManager _CurrencyManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyController"/> class.
        /// </summary>
        /// <param name="currencyManager">The currency manager.</param>
        public CurrencyController(ICurrencyManager currencyManager)
         {
             _CurrencyManager = currencyManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CurrencyQuery)]
        public ActionResult Index()
        {
            
            const string Code = "";
            const string Name = "";
            var currencyGrid = new CurrencySearch("SearchCurrencyGrid", Url.Action("CurrencySearchGridData", "Currency", new { Code,Name }));
            ViewData["CurrencyGrid"] = currencyGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified currency.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CurrencyQuery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Currency currency)
        {
            SessionUtil.CurrentPageSelected = 1;
            var currencysGrid = new CurrencySearch("SearchCurrencyGrid", Url.Action("CurrencySearchGridData", new {currency.Code,currency.Name }));
            ViewData["CurrencyGrid"] = currencysGrid.Instance;
            return View();
        }

        /// <summary>
        /// Currencies the search grid data.
        /// </summary>
        /// <param name="Code">The code.</param>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CurrencyQuery)]
        public JsonResult CurrencySearchGridData(string Code,string Name)
        {

            var currencysGrid = new CurrencySearch("SearchCurrencyGrid", Url.Action("CurrencySearchGridData", new { Code,Name }));
            var currencys = _CurrencyManager.GetCurrencyList(Code,Name);
            try
            {
                return currencysGrid.DataBind(currencys.AsQueryable());

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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CurrencyQuery)]
        public ActionResult Details(int id)
        {
            return View();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CurrencyEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified currency.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CurrencyEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Currency currency,FormCollection collection)
        {
            try
            {
                currency.Code = currency.Code.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createCurrency = _CurrencyManager.AddCurrency(currency);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(currency);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(currency);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CurrencyEditOrDelete)]
        public ActionResult Edit(int id)
        {
            Currency currency = _CurrencyManager.GetCurrencyDetails(id);
            return View(currency);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="currency">The currency.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CurrencyEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,Currency currency, FormCollection collection)
        {
            try
            {
                currency.Id = id;
                currency.Code = currency.Code.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    var UpdateCurrency = _CurrencyManager.UpdateCurrency(currency);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(currency);
                }
                
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(currency);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CurrencyEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteCurrency = _CurrencyManager.DeleteCurrency(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

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
    public class ExchangeRateController : ISController
    {
        private readonly IExchangeRateManager _ExchangeRateManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeRateController"/> class.
        /// </summary>
        /// <param name="exchangeRateManager">The exchange rate manager.</param>
        public ExchangeRateController(IExchangeRateManager exchangeRateManager)
         {
             _ExchangeRateManager = exchangeRateManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ExchangeRateQuery)]
        public ActionResult Index()
        {
            
            const int CurrencyId = 0;
            const string Fromdate="";
            const string Todate = "";
            var exchangeRateGrid = new ExchangeRateSearch("SearchExchangeRateGrid", Url.Action("ExchangeRateSearchGridData", "ExchangeRate", new {CurrencyId,Fromdate,Todate }));
            ViewData["ExchangeRateGrid"] = exchangeRateGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified exchange rate.
        /// </summary>
        /// <param name="exchangeRate">The exchange rate.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ExchangeRateQuery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ExchangeRate exchangeRate)
        {
            SessionUtil.CurrentPageSelected = 1;
             int CurrencyId = Convert.ToInt32(exchangeRate.CurrencyId);
             DateTime Fromdate = exchangeRate.EffectiveFromDate;
             DateTime Todate = exchangeRate.EffectiveToDate;
             var exchangeRatesGrid = new ExchangeRateSearch("SearchExchangeRateGrid", Url.Action("ExchangeRateSearchGridData", new { CurrencyId, Fromdate, Todate }));
            ViewData["ExchangeRateGrid"] = exchangeRatesGrid.Instance;
            return View();
        }

        /// <summary>
        /// Exchanges the rate search grid data.
        /// </summary>
        /// <param name="CurrencyId">The currency id.</param>
        /// <param name="Fromdate">The fromdate.</param>
        /// <param name="Todate">The todate.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ExchangeRateQuery)]
        public JsonResult ExchangeRateSearchGridData(int CurrencyId, DateTime? Fromdate, DateTime? Todate)
        {

            var exchangeRatesGrid = new ExchangeRateSearch("SearchExchangeRateGrid", Url.Action("ExchangeRateSearchGridData", new { CurrencyId, Fromdate, Todate }));
            var exchangeRates = _ExchangeRateManager.GetExchangeRateList(CurrencyId, Fromdate, Todate);
            try
            {
                return exchangeRatesGrid.DataBind(exchangeRates.AsQueryable());

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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ExchangeRateEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified exchange rate.
        /// </summary>
        /// <param name="exchangeRate">The exchange rate.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ExchangeRateEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ExchangeRate exchangeRate,FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    if (exchangeRate.EffectiveFromDate > exchangeRate.EffectiveToDate)
                    {
                        throw new ISBusinessException(ErrorCodes.InvalidEffectiveFromDate);
                    }
                    var createExchangeRate = _ExchangeRateManager.AddExchangeRate(exchangeRate);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(exchangeRate);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(exchangeRate);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ExchangeRateEditOrDelete)]
        public ActionResult Edit(int id)
        {
            ExchangeRate exchangeRate = _ExchangeRateManager.GetExchangeRateDetails(id);
            return View(exchangeRate);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="exchangeRate">The exchange rate.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ExchangeRateEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,ExchangeRate exchangeRate, FormCollection collection)
        {
            try
            {
                exchangeRate.Id = id;
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    if (exchangeRate.EffectiveFromDate > exchangeRate.EffectiveToDate)
                    {
                        throw new ISBusinessException(ErrorCodes.InvalidEffectiveFromDate);
                    }
                    var UpdateExchangeRate = _ExchangeRateManager.UpdateExchangeRate(exchangeRate);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(exchangeRate);
                }
                
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(exchangeRate);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.ExchangeRateEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteExchangeRate = _ExchangeRateManager.DeleteExchangeRate(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

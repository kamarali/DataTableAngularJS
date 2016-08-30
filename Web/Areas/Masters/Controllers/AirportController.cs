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
    public class AirportController : ISController
    {
        private readonly IAirportManager _AirportManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AirportController"/> class.
        /// </summary>
        /// <param name="airportManager">The airport manager.</param>
        public AirportController(IAirportManager airportManager)
         {
             _AirportManager = airportManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AirportICAOQuery)]
        public ActionResult Index()
        {
            const string Id = "";
            const string Name = "";
            const string CountryCode = "";
            var airportGrid = new AirportSearch("SearchAirportGrid", Url.Action("AirportSearchGridData", "Airport", new {Id,Name,  CountryCode }));
            ViewData["AirportGrid"] = airportGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified airport.
        /// </summary>
        /// <param name="airport">The airport.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AirportICAOQuery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Airport airport)
        {
            SessionUtil.CurrentPageSelected = 1;
            //Get Airport present in database
            var airportGrid = new AirportSearch("SearchAirportGrid", Url.Action("AirportSearchGridData", "Airport", new { airport.Id, airport.Name, airport.CountryCode }));
            ViewData["AirportGrid"] = airportGrid.Instance;
            //Display Airport on UI
            return View();
        }

        /// <summary>
        /// Airports the search grid data.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="Name">The name.</param>
        /// <param name="CountryCode">The country code.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AirportICAOQuery)]
        public JsonResult AirportSearchGridData( string Id,string Name,string  CountryCode)
        {
            var airportGrid = new AirportSearch("SearchAirportGrid", Url.Action("AirportSearchGridData", new { Id, Name, CountryCode }));
            var airports = _AirportManager.GetAirportList(Id, Name, CountryCode);
            try
            {
                return airportGrid.DataBind(airports.AsQueryable());
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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AirportICAOEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified airport.
        /// </summary>
        /// <param name="airport">The airport.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AirportICAOEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Airport airport,FormCollection collection)
        {
            try
            {
                airport.Id = airport.Id.ToUpper();
                airport.CountryCode = airport.CountryCode.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createCountry = _AirportManager.AddAirport(airport);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(airport);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(airport);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AirportICAOEditOrDelete)]
        public ActionResult Edit(string Id)
        {
            Airport airport = _AirportManager.GetAirportDetails(Id);
            return View(airport);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="airport">The airport.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AirportICAOEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string Id, Airport airport, FormCollection collection)
        {
            try
            {
                airport.Id = Id.ToUpper();
                airport.CountryCode = airport.CountryCode.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    Airport Updatedairport = _AirportManager.UpdateAirport(airport);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(airport);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(airport);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AirportICAOEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(string Id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var countryDelete = _AirportManager.DeleteAirport(Id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
    }
}

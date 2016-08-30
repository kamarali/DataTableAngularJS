using System.Collections.Generic;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using System;
using Iata.IS.Web.Util.Filters;


namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class CityAirportController : ISController
    {
        private readonly ICityAirportManager _CityAirportManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CityAirportController"/> class.
        /// </summary>
        /// <param name="cityAirportManager">The city airport manager.</param>
        public CityAirportController(ICityAirportManager cityAirportManager)
         {
             _CityAirportManager = cityAirportManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CityAndAirportQuery)]
        public ActionResult Index()
        {
            const string Id = "";
            const string Name = "";
            const string CountryId = "";
            var cityAirportGrid = new CityAirportSearch("SearchCityAirportGrid", Url.Action("CityAirportSearchGridData", "CityAirport", new { Id, Name, CountryId }));
            ViewData["CityAirportGrid"] = cityAirportGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified city airport.
        /// </summary>
        /// <param name="cityAirport">The city airport.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CityAndAirportQuery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(CityAirport cityAirport)
        {
            //SCP205281:Error when searching City/Airport Master Table 
            //AUX is reserved word, so if comes as an input in URL, throws an exception, to avopid the exception replaced it with _1 and while seraching remove it again.
            string Id = string.Format("{0}_1", cityAirport.Id);
            SessionUtil.CurrentPageSelected = 1;
            var cityAirportGrid = new CityAirportSearch("SearchCityAirportGrid", Url.Action("CityAirportSearchGridData", "CityAirport", new { Id, cityAirport.Name, cityAirport.CountryId }));
            ViewData["CityAirportGrid"] = cityAirportGrid.Instance;
            return View();
        }

        /// <summary>
        /// Cities the airport search grid data.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="Name">The name.</param>
        /// <param name="CountryId">The country id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CityAndAirportQuery)]
        public JsonResult CityAirportSearchGridData(string Id, string Name, string CountryId)
        {
            //SCP205281:Error when searching City/Airport Master Table 
            //AUX is reserved word, so if comes as an input in URL, throws an exception, to avopid the exception replaced it with _1 and while seraching remove it again.
            var cityAirportGrid = new CityAirportSearch("SearchCityAirportGrid", Url.Action("CityAirportSearchGridData", new { Id, Name, CountryId }));
            var cityAirports = _CityAirportManager.GetCityAirportList(string.IsNullOrEmpty(Id) ? Id : Id.Replace("_1", string.Empty), Name, CountryId);
            cityAirports.ForEach(a => a.Id = a.Id + "_1");
            try
            {
                return cityAirportGrid.DataBind(cityAirports.AsQueryable());
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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CityAndAirportEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified city airport.
        /// </summary>
        /// <param name="cityAirport">The city airport.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CityAndAirportEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CityAirport cityAirport,FormCollection collection)
        {
            try
            {
                cityAirport.Id = cityAirport.Id.ToUpper();
                cityAirport.CountryId = cityAirport.CountryId.ToUpper();
                cityAirport.MainCity = cityAirport.MainCity.ToUpper();
                if (ModelState.IsValid)
                {
                    var createcityAirport = _CityAirportManager.AddCityAirport(cityAirport);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(cityAirport);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(cityAirport);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CityAndAirportEditOrDelete)]
        public ActionResult Edit(string Id)
        {
            //SCP205281:Error when searching City/Airport Master Table 
            Id = string.IsNullOrEmpty(Id) ? Id : Id.Replace("_1", string.Empty);
            CityAirport cityAirport = _CityAirportManager.GetCityAirportDetails(Id);
            return View(cityAirport);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="cityAirport">The city airport.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CityAndAirportEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string Id, CityAirport cityAirport, FormCollection collection)
        {
            try
            {
                //SCP205281:Error when searching City/Airport Master Table 
                Id = string.IsNullOrEmpty(Id) ? Id : Id.Replace("_1", string.Empty);
                cityAirport.Id = Id;
                cityAirport.Id = cityAirport.Id.ToUpper();
                cityAirport.CountryId = cityAirport.CountryId.ToUpper();
                cityAirport.MainCity = cityAirport.MainCity.ToUpper();
                if (ModelState.IsValid)
                {
                    string Message = string.Empty;
                    CityAirport UpdatedcityAirport = _CityAirportManager.UpdateCityAirport(cityAirport);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(cityAirport);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(cityAirport);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CityAndAirportEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(string Id, FormCollection collection)
        {
            try
            {
                //SCP205281:Error when searching City/Airport Master Table 
                Id = string.IsNullOrEmpty(Id) ? Id : Id.Replace("_1", string.Empty);
                var countryDelete = _CityAirportManager.DeleteCityAirport(Id);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}

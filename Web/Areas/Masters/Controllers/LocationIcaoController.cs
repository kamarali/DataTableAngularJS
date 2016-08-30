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
    public class LocationIcaoController : ISController
    {
        private readonly ILocationIcaoManager _LocationIcaoManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationIcaoController"/> class.
        /// </summary>
        /// <param name="locationIcaoManager">The locationIcao manager.</param>
        public LocationIcaoController(ILocationIcaoManager locationIcaoManager)
        {
            _LocationIcaoManager = locationIcaoManager;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LocationICAOQuery)]
        public ActionResult Index()
        {

            const string Id = "";
            const string CountryCode = "";
            const string Description = "";
            var locationIcaoGrid = new LocationIcaoSearch("SearchLocationIcaoGrid", Url.Action("LocationIcaoSearchGridData", "LocationIcao", new { Id, CountryCode, Description }));
            ViewData["LocationIcaoGrid"] = locationIcaoGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified locationIcao.
        /// </summary>
        /// <param name="locationIcao">The locationIcao.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LocationICAOQuery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LocationIcao locationIcao)
        {
            if (!string.IsNullOrEmpty(locationIcao.Description))
            {
                locationIcao.Description = locationIcao.Description.Trim();
            }
            SessionUtil.CurrentPageSelected = 1;
            var locationIcaosGrid = new LocationIcaoSearch("SearchLocationIcaoGrid", Url.Action("LocationIcaoSearchGridData", new { locationIcao.Id, locationIcao.CountryCode, locationIcao.Description }));
            ViewData["LocationIcaoGrid"] = locationIcaosGrid.Instance;
            return View(locationIcao);
        }

        /// <summary>
        /// Currencies the search grid data.
        /// </summary>
        /// <param name="Code">The code.</param>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LocationICAOQuery)]
        public JsonResult LocationIcaoSearchGridData(string Id, string CountryCode, string Description)
        {

            var locationIcaosGrid = new LocationIcaoSearch("SearchLocationIcaoGrid", Url.Action("LocationIcaoSearchGridData", new { Id, CountryCode, Description }));
            var locationIcaos = _LocationIcaoManager.GetLocationIcaoList(Id, CountryCode, Description);
            try
            {
                return locationIcaosGrid.DataBind(locationIcaos.AsQueryable());

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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LocationICAOQuery)]
        public ActionResult Details(string id)
        {
            return View();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LocationICAOEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified locationIcao.
        /// </summary>
        /// <param name="locationIcao">The locationIcao.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LocationICAOEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LocationIcao locationIcao, FormCollection collection)
        {
            try
            {
                locationIcao.Id = locationIcao.Id.ToUpper();
                if (!string.IsNullOrEmpty(locationIcao.Description))
                {
                    locationIcao.Description = locationIcao.Description.Trim();
                }
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createLocationIcao = _LocationIcaoManager.AddLocationIcao(locationIcao);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(locationIcao);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(locationIcao);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LocationICAOEditOrDelete)]
        public ActionResult Edit(string id)
        {
            LocationIcao locationIcao = _LocationIcaoManager.GetLocationIcaoDetails(id);
            return View(locationIcao);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="locationIcao">The locationIcao.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LocationICAOEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, LocationIcao locationIcao, FormCollection collection)
        {
            try
            {
                locationIcao.Id = locationIcao.Id.ToUpper();
                if (!string.IsNullOrEmpty(locationIcao.Description))
                {
                    locationIcao.Description = locationIcao.Description.Trim();
                }
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    var UpdateLocationIcao = _LocationIcaoManager.UpdateLocationIcao(locationIcao);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(locationIcao);
                }

            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(locationIcao);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LocationICAOEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteLocationIcao = _LocationIcaoManager.DeleteLocationIcao(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

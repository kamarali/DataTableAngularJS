using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class AircraftTypeController : ISController
    {
        private readonly IAircraftTypeManager _AircraftTypeManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AircraftTypeController"/> class.
        /// </summary>
        /// <param name="aircraftTypeManager">The aircraft type manager.</param>
        public AircraftTypeController(IAircraftTypeManager aircraftTypeManager)
        {
            _AircraftTypeManager = aircraftTypeManager;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AircraftTypeQuery)]
        public ActionResult Index()
        {
            //Get AircraftType present in database
            const string id = "", description = "";
            var aircraftTypeGrid = new AircraftTypeSearch("SearchAircraftTypeGrid", Url.Action("AircraftTypeSearchGridData", "AircraftType", new { id, description }));
            ViewData["AircraftTypeGrid"] = aircraftTypeGrid.Instance;
            //Display AircraftType on UI
            return View();
        }

        /// <summary>
        /// Indexes the specified aircraft type.
        /// </summary>
        /// <param name="aircraftType">Type of the aircraft.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AircraftTypeQuery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(AircraftType aircraftType)
        {
            if (!string.IsNullOrEmpty(aircraftType.Description))
            {
                aircraftType.Description = aircraftType.Description.Trim();
            }
            SessionUtil.CurrentPageSelected = 1;
            var aircraftTypeGrid = new AircraftTypeSearch("SearchAircraftTypeGrid", Url.Action("AircraftTypeSearchGridData", "AircraftType", new { aircraftType.Id, aircraftType.Description }));
            ViewData["AircraftTypeGrid"] = aircraftTypeGrid.Instance;
            return View(aircraftType);
        }

        /// <summary>
        /// Aircrafts the type search grid data.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="AircraftTypeCodeIcao">The aircraft type code icao.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AircraftTypeQuery)]
        public JsonResult AircraftTypeSearchGridData(string id, string description)
        {
            var aircraftTypesGrid = new AircraftTypeSearch("SearchAircraftTypeGrid", Url.Action("AircraftTypeSearchGridData", new { id, description }));
            var aircraftTypes = _AircraftTypeManager.GetAircraftTypeList(id, description);
            try
            {
                return aircraftTypesGrid.DataBind(aircraftTypes.AsQueryable());
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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AircraftTypeEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified aircraft type.
        /// </summary>
        /// <param name="aircraftType">Type of the aircraft.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AircraftTypeEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AircraftType aircraftType, FormCollection collection)
        {
            try
            {
                aircraftType.Id = aircraftType.Id.ToUpper();
                if (!string.IsNullOrEmpty(aircraftType.Description))
                {
                    aircraftType.Description = aircraftType.Description.Trim();
                    if (aircraftType.Description.Length > 255)
                    aircraftType.Description = aircraftType.Description.Substring(0, 255);
                }
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createAircraftType = _AircraftTypeManager.AddAircraftType(aircraftType);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(aircraftType);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(aircraftType);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AircraftTypeEditOrDelete)]
        public ActionResult Edit(string Id)
        {
            AircraftType aircraftType = _AircraftTypeManager.GetAircraftTypeDetails(Id);
            return View(aircraftType);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="aircraftType">Type of the aircraft.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AircraftTypeEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string Id, AircraftType aircraftType, FormCollection collection)
        {
            try
            {
                aircraftType.Id = aircraftType.Id.ToUpper();
                if (!string.IsNullOrEmpty(aircraftType.Description))
                {
                    aircraftType.Description = aircraftType.Description.Trim();
                    if (aircraftType.Description.Length > 255)
                    aircraftType.Description = aircraftType.Description.Substring(0, 255);
                }
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    AircraftType UpdatedaircraftType = _AircraftTypeManager.UpdateAircraftType(aircraftType);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(aircraftType);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(aircraftType);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AircraftTypeEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(string Id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var aircraftTypeDelete = _AircraftTypeManager.DeleteAircraftType(Id);
                return RedirectToAction("Index");
            }
            catch 
            {
                return RedirectToAction("Index");
            }
        }


    }
}

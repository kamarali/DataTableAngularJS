using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using Iata.IS.Web.Util.Filters;


namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class AircraftTypeIcaoController : ISController
    {
        private readonly IAircraftTypeIcaoManager _AircraftTypeIcaoManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AircraftTypeIcaoController"/> class.
        /// </summary>
        /// <param name="aircraftTypeIcaoManager">The aircraftTypeIcao manager.</param>
        public AircraftTypeIcaoController(IAircraftTypeIcaoManager aircraftTypeIcaoManager)
        {
            _AircraftTypeIcaoManager = aircraftTypeIcaoManager;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AircraftTypeICAOQuery)]
        public ActionResult Index()
        {
            const string Id = "";
            const string Description = "";
            var aircraftTypeIcaoGrid = new AircraftTypeIcaoSearch("SearchAircraftTypeIcaoGrid", Url.Action("AircraftTypeIcaoSearchGridData", "AircraftTypeIcao", new { Id, Description }));
            ViewData["AircraftTypeIcaoGrid"] = aircraftTypeIcaoGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified aircraftTypeIcao.
        /// </summary>
        /// <param name="aircraftTypeIcao">The aircraftTypeIcao.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AircraftTypeICAOQuery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(AircraftTypeIcao aircraftTypeIcao)
        {
            if (!string.IsNullOrEmpty(aircraftTypeIcao.Description))
            {
                aircraftTypeIcao.Description = aircraftTypeIcao.Description.Trim();
            }
            SessionUtil.CurrentPageSelected = 1;
            //Get AircraftType present in database
            var aircraftTypeIcaosGrid = new AircraftTypeIcaoSearch("SearchAircraftTypeIcaoGrid", Url.Action("AircraftTypeIcaoSearchGridData", new { aircraftTypeIcao.Id, aircraftTypeIcao.Description }));
            ViewData["AircraftTypeIcaoGrid"] = aircraftTypeIcaosGrid.Instance;
            //Display AircraftType on UI
            return View(aircraftTypeIcao);
        }

        /// <summary>
        /// Currencies the search grid data.
        /// </summary>
        /// <param name="Code">The code.</param>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AircraftTypeICAOQuery)]
        public JsonResult AircraftTypeIcaoSearchGridData(string Id, string Description)
        {

            var aircraftTypeIcaosGrid = new AircraftTypeIcaoSearch("SearchAircraftTypeIcaoGrid", Url.Action("AircraftTypeIcaoSearchGridData", new { Id, Description }));
            var aircraftTypeIcaos = _AircraftTypeIcaoManager.GetAircraftTypeIcaoList(Id, Description);
            try
            {
                return aircraftTypeIcaosGrid.DataBind(aircraftTypeIcaos.AsQueryable());

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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AircraftTypeICAOQuery)]
        public ActionResult Details(string id)
        {
            return View();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AircraftTypeICAOEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified aircraftTypeIcao.
        /// </summary>
        /// <param name="aircraftTypeIcao">The aircraftTypeIcao.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AircraftTypeICAOEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AircraftTypeIcao aircraftTypeIcao, FormCollection collection)
        {
            try
            {
                aircraftTypeIcao.Id = aircraftTypeIcao.Id.ToUpper();
                if (!string.IsNullOrEmpty(aircraftTypeIcao.Description))
                {
                    aircraftTypeIcao.Description = aircraftTypeIcao.Description.Trim();
                    if (aircraftTypeIcao.Description.Length > 255)
                        aircraftTypeIcao.Description = aircraftTypeIcao.Description.Substring(0, 255);
                }
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createAircraftTypeIcao = _AircraftTypeIcaoManager.AddAircraftTypeIcao(aircraftTypeIcao);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(aircraftTypeIcao);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(aircraftTypeIcao);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AircraftTypeICAOEditOrDelete)]
        public ActionResult Edit(string id)
        {
            AircraftTypeIcao aircraftTypeIcao = _AircraftTypeIcaoManager.GetAircraftTypeIcaoDetails(id);
            return View(aircraftTypeIcao);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="aircraftTypeIcao">The aircraftTypeIcao.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AircraftTypeICAOEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, AircraftTypeIcao aircraftTypeIcao, FormCollection collection)
        {
            try
            {
                aircraftTypeIcao.Id = aircraftTypeIcao.Id.ToUpper();
                if (!string.IsNullOrEmpty(aircraftTypeIcao.Description))
                {
                    aircraftTypeIcao.Description = aircraftTypeIcao.Description.Trim();
                    if (aircraftTypeIcao.Description.Length > 255)
                        aircraftTypeIcao.Description = aircraftTypeIcao.Description.Substring(0, 255);
                }
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    var UpdateAircraftTypeIcao = _AircraftTypeIcaoManager.UpdateAircraftTypeIcao(aircraftTypeIcao);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(aircraftTypeIcao);
                }

            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(aircraftTypeIcao);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AircraftTypeICAOEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteAircraftTypeIcao = _AircraftTypeIcaoManager.DeleteAircraftTypeIcao(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

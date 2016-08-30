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

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class IchZoneController : ISController
    {
        private readonly IIchZoneManager _IchZoneManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="IchZoneController"/> class.
        /// </summary>
        /// <param name="IchZoneManager">The ich zone manager.</param>
        public IchZoneController(IIchZoneManager IchZoneManager)
         {
             _IchZoneManager = IchZoneManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            
            const string Zone = "",ClearanceCurrency= "",Description= "";
            var IchZoneGrid = new IchZoneSearch("SearchIchZoneGrid", Url.Action("IchZoneSearchGridData", "IchZone", new { Zone, ClearanceCurrency, Description }));
            ViewData["IchZoneGrid"] = IchZoneGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified ich zone.
        /// </summary>
        /// <param name="IchZone">The ich zone.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(IchZone IchZone)
        {
            SessionUtil.CurrentPageSelected = 1;
            var IchZonesGrid = new IchZoneSearch("SearchIchZoneGrid", Url.Action("IchZoneSearchGridData", new { IchZone.Zone, IchZone.ClearanceCurrency, IchZone.Description }));
            ViewData["IchZoneGrid"] = IchZonesGrid.Instance;
            return View();
        }

        /// <summary>
        /// Iches the zone search grid data.
        /// </summary>
        /// <param name="Code">The code.</param>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        public JsonResult IchZoneSearchGridData(string Zone, string ClearanceCurrency, string Description)
        {

            var IchZonesGrid = new IchZoneSearch("SearchIchZoneGrid", Url.Action("IchZoneSearchGridData", new { Zone, ClearanceCurrency, Description }));
            var IchZones = _IchZoneManager.GetIchZoneList(Zone, ClearanceCurrency, Description);
            try
            {
                return IchZonesGrid.DataBind(IchZones.AsQueryable());

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
        /// Creates the specified ich zone.
        /// </summary>
        /// <param name="IchZone">The ich zone.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(IchZone IchZone,FormCollection collection)
        {
            try
            {
                IchZone.Zone = IchZone.Zone.ToUpper();
                IchZone.ClearanceCurrency = IchZone.ClearanceCurrency.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createIchZone = _IchZoneManager.AddIchZone(IchZone);
                    ShowSuccessMessage("Ich Zone details saved successfully");
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(IchZone);
                }
            }
            catch
            {
                return View(IchZone);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            IchZone IchZone = _IchZoneManager.GetIchZoneDetails(id);
            return View(IchZone);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="IchZone">The ich zone.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id,IchZone IchZone, FormCollection collection)
        {
            try
            {
                IchZone.Id = id;
                IchZone.Zone = IchZone.Zone.ToUpper();
                IchZone.ClearanceCurrency = IchZone.ClearanceCurrency.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    var UpdateIchZone = _IchZoneManager.UpdateIchZone(IchZone);
                    ShowSuccessMessage("Ich Zone details saved successfully");
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(IchZone);
                }
                
            }
            catch
            {
                return View(IchZone);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteIchZone = _IchZoneManager.DeleteIchZone(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

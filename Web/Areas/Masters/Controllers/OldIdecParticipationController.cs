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
    public class OldIdecParticipationController : ISController
    {
        private readonly IOldIdecParticipationManager _OldIdecParticipationManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="OldIdecParticipationController"/> class.
        /// </summary>
        /// <param name="oldIdecParticipationManager">The old idec participation manager.</param>
        public OldIdecParticipationController(IOldIdecParticipationManager oldIdecParticipationManager)
         {
             _OldIdecParticipationManager = oldIdecParticipationManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            
            const int MemberId = 0;
            var oldIdecParticipationGrid = new OldIdecParticipationSearch("SearchOldIdecParticipationGrid", Url.Action("OldIdecParticipationSearchGridData", "OldIdecParticipation", new { MemberId }));
            ViewData["OldIdecParticipationGrid"] = oldIdecParticipationGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified old idec participation.
        /// </summary>
        /// <param name="oldIdecParticipation">The old idec participation.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(OldIdecParticipation oldIdecParticipation)
        {
            SessionUtil.CurrentPageSelected = 1;
            var oldIdecParticipationsGrid = new OldIdecParticipationSearch("SearchOldIdecParticipationGrid", Url.Action("OldIdecParticipationSearchGridData", new {oldIdecParticipation.MemberId}));
            ViewData["OldIdecParticipationGrid"] = oldIdecParticipationsGrid.Instance;
            return View();
        }

        /// <summary>
        /// Olds the idec participation search grid data.
        /// </summary>
        /// <param name="MemberId">The member id.</param>
        /// <returns></returns>
        public JsonResult OldIdecParticipationSearchGridData(int MemberId)
        {

            var oldIdecParticipationsGrid = new OldIdecParticipationSearch("SearchOldIdecParticipationGrid", Url.Action("OldIdecParticipationSearchGridData", new { MemberId }));
            var oldIdecParticipations = _OldIdecParticipationManager.GetOldIdecParticipationList(MemberId);
            try
            {
                return oldIdecParticipationsGrid.DataBind(oldIdecParticipations.AsQueryable());

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
        /// Creates the specified old idec participation.
        /// </summary>
        /// <param name="oldIdecParticipation">The old idec participation.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(OldIdecParticipation oldIdecParticipation,FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createOldIdecParticipation = _OldIdecParticipationManager.AddOldIdecParticipation(oldIdecParticipation);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(oldIdecParticipation);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(oldIdecParticipation);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            OldIdecParticipation oldIdecParticipation = _OldIdecParticipationManager.GetOldIdecParticipationDetails(id);

            return View(oldIdecParticipation);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="oldIdecParticipation">The old idec participation.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id,OldIdecParticipation oldIdecParticipation, FormCollection collection)
        {
            try
            {
                oldIdecParticipation.Id = id;
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    var UpdateOldIdecParticipation = _OldIdecParticipationManager.UpdateOldIdecParticipation(oldIdecParticipation);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(oldIdecParticipation);
                }
                
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(oldIdecParticipation);
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
                var deleteOldIdecParticipation = _OldIdecParticipationManager.DeleteOldIdecParticipation(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

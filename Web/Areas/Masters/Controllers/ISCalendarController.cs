using System.Collections.Generic;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Model.Calendar;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using System;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class ISCalendarController : ISController
    {
        private readonly ICalendarManager _ISCalendarManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ISCalendarController"/> class.
        /// </summary>
        /// <param name="isCalendarManager">The is calendar manager.</param>
        public ISCalendarController(ICalendarManager isCalendarManager)
         {
             _ISCalendarManager = isCalendarManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            const int Month = 0;
            const int Period = 0;
            const string EventCategory = "";
            var isCalendarGrid = new ISCalendarSearch("SearchISCalendarGrid", Url.Action("ISCalendarSearchGridData", "ISCalendar", new { Month, Period, EventCategory }));
            ViewData["ISCalendarGrid"] = isCalendarGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified is calendar.
        /// </summary>
        /// <param name="isCalendar">The is calendar.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(ISCalendar isCalendar)
        {
            SessionUtil.CurrentPageSelected = 1;
            var isCalendarGrid = new ISCalendarSearch("SearchISCalendarGrid", Url.Action("ISCalendarSearchGridData", "ISCalendar", new { isCalendar.Month, isCalendar.Period, isCalendar.EventCategory }));
            ViewData["ISCalendarGrid"] = isCalendarGrid.Instance;
            return View();
        }

        /// <summary>
        /// ISs the calendar search grid data.
        /// </summary>
        /// <param name="Month">The month.</param>
        /// <param name="Period">The period.</param>
        /// <param name="EventCategory">The event category.</param>
        /// <returns></returns>
        public JsonResult ISCalendarSearchGridData( int Month, int Period, string EventCategory )
        {
            var isCalendarGrid = new ISCalendarSearch("SearchISCalendarGrid", Url.Action("ISCalendarSearchGridData", new { Month, Period, EventCategory }));
            var isCalendars = _ISCalendarManager.GetISCalendarList(Month, Period, EventCategory);
            try
            {
                return isCalendarGrid.DataBind(isCalendars.AsQueryable());
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
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified is calendar.
        /// </summary>
        /// <param name="isCalendar">The is calendar.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(ISCalendar isCalendar,FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createCountry = _ISCalendarManager.AddISCalendar(isCalendar);
                    ShowSuccessMessage("ISCalendar details saved successfully");
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(isCalendar);
                }
            }
            catch
            {
                return View(isCalendar);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        public ActionResult Edit(int Id)
        {
            ISCalendar isCalendar = _ISCalendarManager.GetISCalendarDetails(Id);
            return View(isCalendar);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="isCalendar">The is calendar.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int Id, ISCalendar isCalendar, FormCollection collection)
        {
            try
            {
                isCalendar.Id = Id;
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    ISCalendar UpdatedisCalendar = _ISCalendarManager.UpdateISCalendar(isCalendar);
                    ShowSuccessMessage("ISCalendar details saved successfully");
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(isCalendar);
                }
            }
            catch
            {
                return View(isCalendar);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int Id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var countryDelete = _ISCalendarManager.DeleteISCalendar(Id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
    }
}
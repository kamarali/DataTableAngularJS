using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using System;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class RfiscController : ISController
    {
        private readonly IRfiscManager _RfiscManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="RfiscController"/> class.
        /// </summary>
        /// <param name="rfiscManager">The rfisc manager.</param>
        public RfiscController(IRfiscManager rfiscManager)
        {
            _RfiscManager = rfiscManager;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.RFISCQuery)]
        public ActionResult Index()
        {

            const string Id = "", RficId = "", GroupName = "", CommercialName = "";
            var rfiscGrid = new RfiscSearch("SearchRfiscGrid", Url.Action("RfiscSearchGridData", "Rfisc", new { Id, RficId, GroupName, CommercialName }));
            ViewData["RfiscGrid"] = rfiscGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified rfisc.
        /// </summary>
        /// <param name="rfisc">The rfisc.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.RFISCQuery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Rfisc rfisc)
        {
            SessionUtil.CurrentPageSelected = 1;
            var rfiscGrid = new RfiscSearch("SearchRfiscGrid", Url.Action("RfiscSearchGridData", "Rfisc", new { rfisc.Id, rfisc.RficId, rfisc.GroupName, rfisc.CommercialName }));
            ViewData["RfiscGrid"] = rfiscGrid.Instance;
            return View();
        }

        /// <summary>
        /// Rfiscs the search grid data.
        /// </summary>
        /// <param name="RficId">The rfic id.</param>
        /// <param name="GroupName">Name of the group.</param>
        /// <param name="CommercialName">Name of the commercial.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.RFISCQuery)]
        public JsonResult RfiscSearchGridData(string Id, string RficId, string GroupName, string CommercialName)
        {
            var rfiscsGrid = new RfiscSearch("SearchRfiscGrid", Url.Action("RfiscSearchGridData", new { Id,RficId, GroupName, CommercialName }));
            var rfiscs = _RfiscManager.GetRfiscList(Id, RficId, GroupName, CommercialName);
            try
            {
                return rfiscsGrid.DataBind(rfiscs.AsQueryable());
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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.RFISCEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified rfisc.
        /// </summary>
        /// <param name="rfisc">The rfisc.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.RFISCEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Rfisc rfisc, FormCollection collection)
        {
            try
            {
                rfisc.Id = rfisc.Id.ToUpper();
                rfisc.CommercialName = rfisc.CommercialName.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createRfisc = _RfiscManager.AddRfisc(rfisc);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(rfisc);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(rfisc);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.RFISCEditOrDelete)]
        public ActionResult Edit(string Id)
        {
            Rfisc rfisc = _RfiscManager.GetRfiscDetails(Id);
            return View(rfisc);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="rfisc">The rfisc.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.RFISCEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string Id, Rfisc rfisc, FormCollection collection)
        {
            try
            {
                rfisc.Id = Id.ToUpper();
                rfisc.CommercialName = rfisc.CommercialName.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    Rfisc Updatedrfisc = _RfiscManager.UpdateRfisc(rfisc);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(rfisc);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(rfisc);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.RFISCEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(string Id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var rfiscDelete = _RfiscManager.DeleteRfisc(Id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
    }
}

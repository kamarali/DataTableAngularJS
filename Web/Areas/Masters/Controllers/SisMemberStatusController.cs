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
    public class SisMemberStatusController : ISController
    {
        private readonly ISisMemberStatusManager _SisMemberStatusManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SisMemberStatusController"/> class.
        /// </summary>
        /// <param name="sisMemberStatusManager">The sisMemberStatus manager.</param>
        public SisMemberStatusController(ISisMemberStatusManager sisMemberStatusManager)
         {
             _SisMemberStatusManager = sisMemberStatusManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SisMembershipStatusQuery)]
        public ActionResult Index()
        {
            
            const string MemberStatus = "";
            const string Description = "";
            var sisMemberStatusGrid = new SisMemberStatusSearch("SearchSisMemberStatusGrid", Url.Action("SisMemberStatusSearchGridData", "SisMemberStatus", new { MemberStatus, Description }));
            ViewData["SisMemberStatusGrid"] = sisMemberStatusGrid.Instance;
           
            return View();
        }

        /// <summary>
        /// Indexes the specified sisMemberStatus.
        /// </summary>
        /// <param name="sisMemberStatus">The sisMemberStatus.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SisMembershipStatusQuery)]
        [HttpPost]
        public ActionResult Index(SisMemberStatus sisMemberStatus)
        {
            if (!string.IsNullOrEmpty(sisMemberStatus.Description))
            {
                sisMemberStatus.Description = sisMemberStatus.Description.Trim();
            }
            SessionUtil.CurrentPageSelected = 1;
            int id=Convert.ToInt32(sisMemberStatus.Id);
            var sisMemberStatussGrid = new SisMemberStatusSearch("SearchSisMemberStatusGrid", Url.Action("SisMemberStatusSearchGridData", new {sisMemberStatus.MemberStatus,sisMemberStatus.Description }));
            ViewData["SisMemberStatusGrid"] = sisMemberStatussGrid.Instance;

            return View(sisMemberStatus);
        }

        /// <summary>
        /// Currencies the search grid data.
        /// </summary>
        /// <param name="Code">The code.</param>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SisMembershipStatusQuery)]
        public JsonResult SisMemberStatusSearchGridData(string MemberStatus, string Description)
        {

            var sisMemberStatussGrid = new SisMemberStatusSearch("SearchSisMemberStatusGrid", Url.Action("SisMemberStatusSearchGridData", new { MemberStatus, Description }));
            var sisMemberStatuss = _SisMemberStatusManager.GetSisMemberStatusList(MemberStatus, Description);
            try
            {
                return sisMemberStatussGrid.DataBind(sisMemberStatuss.AsQueryable());

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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SisMembershipStatusQuery)]
        public ActionResult Details(int id)
        {
            return View();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SisMembershipStatusEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified sisMemberStatus.
        /// </summary>
        /// <param name="sisMemberStatus">The sisMemberStatus.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SisMembershipStatusEditOrDelete)]
        [HttpPost]
        public ActionResult Create(SisMemberStatus sisMemberStatus,FormCollection collection)
        {
            try
            {
                if (!string.IsNullOrEmpty(sisMemberStatus.Description))
                {
                    sisMemberStatus.Description = sisMemberStatus.Description.Trim();
                }
                sisMemberStatus.MemberStatus = sisMemberStatus.MemberStatus.ToUpper();
                sisMemberStatus.Description = sisMemberStatus.Description.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createSisMemberStatus = _SisMemberStatusManager.AddSisMemberStatus(sisMemberStatus);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(sisMemberStatus);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(sisMemberStatus);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SisMembershipStatusEditOrDelete)]
        public ActionResult Edit(int id)
        {
            SisMemberStatus sisMemberStatus = _SisMemberStatusManager.GetSisMemberStatusDetails(id);
            return View(sisMemberStatus);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="sisMemberStatus">The sisMemberStatus.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SisMembershipStatusEditOrDelete)]
        [HttpPost]
        public ActionResult Edit(int id,SisMemberStatus sisMemberStatus, FormCollection collection)
        {
            try
            {
                if (!string.IsNullOrEmpty(sisMemberStatus.Description))
                {
                    sisMemberStatus.Description = sisMemberStatus.Description.Trim();
                }
                sisMemberStatus.Id = id;
                sisMemberStatus.MemberStatus = sisMemberStatus.MemberStatus.ToUpper();
                sisMemberStatus.Description = sisMemberStatus.Description.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    var UpdateSisMemberStatus = _SisMemberStatusManager.UpdateSisMemberStatus(sisMemberStatus);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(sisMemberStatus);
                }
                
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(sisMemberStatus);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SisMembershipStatusEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteSisMemberStatus = _SisMemberStatusManager.DeleteSisMemberStatus(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

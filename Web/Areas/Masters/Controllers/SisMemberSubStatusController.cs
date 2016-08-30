using System.Collections.Generic;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using System;
using Iata.IS.Web.Util.Filters;


namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class SisMemberSubStatusController  : ISController
    {
        private readonly ISisMemberSubStatusManager _SisMemberSubStatusManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SisMemberSubStatusController"/> class.
        /// </summary>
        /// <param name="sisMemberSubStatusManager">The sis member sub status manager.</param>
        public SisMemberSubStatusController(ISisMemberSubStatusManager sisMemberSubStatusManager)
         {
             _SisMemberSubStatusManager = sisMemberSubStatusManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SisMembershipSubStatusQuery)]
        public ActionResult Index()
        {
            
            const string MemberSubStatus = "", Description = "";
            const int MemberStatusId = 0;
            var sisMemberSubStatusGrid = new SisMemberSubStatusSearch("SearchSisMemberSubStatusGrid", Url.Action("SisMemberSubStatusSearchGridData", "SisMemberSubStatus", new { MemberStatusId, MemberSubStatus, Description }));
            ViewData["SisMemberSubStatusGrid"] = sisMemberSubStatusGrid.Instance;
           
            return View();
        }

        /// <summary>
        /// Indexes the specified sis member sub status.
        /// </summary>
        /// <param name="sisMemberSubStatus">The sis member sub status.</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SisMembershipSubStatusQuery)]
        [HttpPost]
        public ActionResult Index(SisMemberSubStatus sisMemberSubStatus)
        {
            if (!string.IsNullOrEmpty(sisMemberSubStatus.Description))
            {
                sisMemberSubStatus.Description = sisMemberSubStatus.Description.Trim();
            }
            SessionUtil.CurrentPageSelected = 1;
            var sisMemberSubStatussGrid = new SisMemberSubStatusSearch("SearchSisMemberSubStatusGrid", Url.Action("SisMemberSubStatusSearchGridData", new {sisMemberSubStatus.Description }));
            ViewData["SisMemberSubStatusGrid"] = sisMemberSubStatussGrid.Instance;

            return View(sisMemberSubStatus);
        }

        /// <summary>
        /// Sises the member sub status search grid data.
        /// </summary>
        /// <param name="MemberStatusId">The member status id.</param>
        /// <param name="MemberSubStatus">The member sub status.</param>
        /// <param name="Description">The description.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SisMembershipSubStatusQuery)]
        public JsonResult SisMemberSubStatusSearchGridData( string Description)
        {

            var sisMemberSubStatussGrid = new SisMemberSubStatusSearch("SearchSisMemberSubStatusGrid", Url.Action("SisMemberSubStatusSearchGridData", new { Description }));
            var sisMemberSubStatuss = _SisMemberSubStatusManager.GetSisMemberSubStatusList(Description);
            try
            {
                return sisMemberSubStatussGrid.DataBind(sisMemberSubStatuss.AsQueryable());

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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SisMembershipSubStatusQuery)]
        public ActionResult Details(int id)
        {
            return View();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SisMembershipSubStatusEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified sis member sub status.
        /// </summary>
        /// <param name="sisMemberSubStatus">The sis member sub status.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SisMembershipSubStatusEditOrDelete)]
        [HttpPost]
        public ActionResult Create(SisMemberSubStatus sisMemberSubStatus,FormCollection collection)
        {
            try
            {
                if (!string.IsNullOrEmpty(sisMemberSubStatus.Description))
                {
                    sisMemberSubStatus.Description = sisMemberSubStatus.Description.Trim();
                }
                sisMemberSubStatus.Description = sisMemberSubStatus.Description.ToUpper();


                // Duplicate Record exist 
                if (_SisMemberSubStatusManager.CheckSubStatusDuplication(sisMemberSubStatus.Description))
                {
                  ShowErrorMessage(Messages.MemberSubStatusDuplicate);
                  return View(sisMemberSubStatus);
                }


                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createSisMemberSubStatus = _SisMemberSubStatusManager.AddSisMemberSubStatus(sisMemberSubStatus);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(sisMemberSubStatus);
                }
            }
            catch (ISBusinessException exception)
            {
                //ShowErrorMessage(exception.ErrorCode);
                return View(sisMemberSubStatus);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SisMembershipSubStatusEditOrDelete)]
        public ActionResult Edit(int id)
        {
            SisMemberSubStatus sisMemberSubStatus = _SisMemberSubStatusManager.GetSisMemberSubStatusDetails(id);
            sisMemberSubStatus.BeforeUpdateMemberSubStatus = sisMemberSubStatus.Description.Trim();
            return View(sisMemberSubStatus);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="sisMemberSubStatus">The sis member sub status.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SisMembershipSubStatusEditOrDelete)]
        [HttpPost]
        public ActionResult Edit(int id,SisMemberSubStatus sisMemberSubStatus, FormCollection collection)
        {
            try
            {
                if (!string.IsNullOrEmpty(sisMemberSubStatus.Description))
                {
                    sisMemberSubStatus.Description = sisMemberSubStatus.Description.Trim();
                }
                sisMemberSubStatus.Id = id;
                sisMemberSubStatus.Description = sisMemberSubStatus.Description.ToUpper();

                if (id == 6 && sisMemberSubStatus.Description != "TERMINATED") // TERMINATED sub status can not edit 
                {
                  ShowErrorMessage(Messages.MemberSubStatusTerminate);
                  return View(sisMemberSubStatus);
                }

                // Duplicate Record exist 
                if (sisMemberSubStatus.Description.ToUpper() != sisMemberSubStatus.BeforeUpdateMemberSubStatus.ToUpper() && _SisMemberSubStatusManager.CheckSubStatusDuplication(sisMemberSubStatus.Description))
                {
                  ShowErrorMessage(Messages.MemberSubStatusDuplicate);
                  return View(sisMemberSubStatus);
                }


                if (ModelState.IsValid)
                {
                  if (_SisMemberSubStatusManager.IsSubStatusExistanceInMemberProfile(id) && !sisMemberSubStatus.IsActive)
                   {
                      ShowErrorMessage(Messages.MemberSubStatusDeactivate);
                      return View(sisMemberSubStatus);
                   }
                  else
                  {
                    var UpdateSisMemberSubStatus = _SisMemberSubStatusManager.UpdateSisMemberSubStatus(sisMemberSubStatus);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                  }
  
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(sisMemberSubStatus);
                }
                
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(sisMemberSubStatus);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SisMembershipSubStatusEditOrDelete)]
        [HttpPost]
        public JsonResult ActiveDeactiveMemberSubStatus(int id, FormCollection collection)
        {
          UIExceptionDetail details;
          var deleteSisMemberSubStatus=false;
            try
            {
              // Check the count of members having Sub Status as input id  
              if(_SisMemberSubStatusManager.IsSubStatusExistanceInMemberProfile(id))
              {
                //If the count is greater than 0, deactivation will fail and an error message will be displayed
                //ShowErrorMessage(Messages.MemberSubStatusDeactivate);
                deleteSisMemberSubStatus = false;
              }
              else
              {
                deleteSisMemberSubStatus = _SisMemberSubStatusManager.DeleteSisMemberSubStatus(id);
                //return RedirectToAction("Index"); 
             }

              details = deleteSisMemberSubStatus
                   ? new UIExceptionDetail { IsFailed = false, Message = string.Format("Record deactivated successfully.") }
                   : new UIExceptionDetail { IsFailed = true, Message = string.Format(Messages.MemberSubStatusDeactivate) };

            }
            catch
            {
              details = new UIExceptionDetail { IsFailed = true, Message = string.Format("Error occured while activate/Deactivate the Member Sub Status. Please try-again") };
            }

            return Json(details);
        }
    }
}

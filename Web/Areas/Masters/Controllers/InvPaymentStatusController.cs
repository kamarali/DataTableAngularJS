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
    public class InvPaymentStatusController : ISController
    {
        private readonly IInvPaymentStatusManager _invPaymentStatusManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvPaymentStatusController"/> class.
        /// </summary>
        /// <param name="invPaymentStatusManager">The Invoice Payment Status manager.</param>
        public InvPaymentStatusController(IInvPaymentStatusManager invPaymentStatusManager)
         {
             _invPaymentStatusManager = invPaymentStatusManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MISCPaymentStatusQuery)]
        public ActionResult Index()
        {
            
            const string description = "";
            const int applicableFor = 0;
            var invPaymentStatusGrid = new InvPaymentStatusSearch("SearchInvPaymentStatusGrid", Url.Action("InvPaymentStatusSearchGridData", new { description, applicableFor}));
            ViewData["InvPaymentStatusGrid"] = invPaymentStatusGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified Invoice Payment Status.
        /// </summary>
        /// <param name="invPaymentStatus">The Invoice Payment Status.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MISCPaymentStatusQuery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(InvPaymentStatus invPaymentStatus)
        {
            SessionUtil.CurrentPageSelected = 1;
            string description = "";

            if (invPaymentStatus.Description != null)
            {
                description = invPaymentStatus.Description.Trim(); 
            }

            var invPaymentStatusGrid = new InvPaymentStatusSearch("SearchInvPaymentStatusGrid", Url.Action("InvPaymentStatusSearchGridData", new { description, invPaymentStatus.ApplicableFor }));
            ViewData["InvPaymentStatusGrid"] = invPaymentStatusGrid.Instance;
            return View();
        }

        /// <summary>
        /// Invoice Payment Status the search grid data.
        /// </summary>
        /// <param name="description">The Description</param>
        /// <param name="applicableFor">The Applicable For</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MISCPaymentStatusQuery)]
        public JsonResult InvPaymentStatusSearchGridData(string description, int applicableFor)
        {

            var invPaymentStatusGrid = new InvPaymentStatusSearch("SearchInvPaymentStatusGrid", Url.Action("InvPaymentStatusSearchGridData", new {  }));
            var invPaymentStatuses = _invPaymentStatusManager.GetInvPaymentStatusList(description, applicableFor);
            try
            {
                return invPaymentStatusGrid.DataBind(invPaymentStatuses.AsQueryable());

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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MISCPaymentStatusQuery)]
        public ActionResult Details(int id)
        {
            return View();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MISCPaymentStatusEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified Invoice Payment Status.
        /// </summary>
        /// <param name="invPaymentStatus">The invPaymentStatus.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MISCPaymentStatusEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InvPaymentStatus invPaymentStatus, FormCollection collection)
        {
            try
            {
                if(!String.IsNullOrEmpty(invPaymentStatus.Description))
                {
                    invPaymentStatus.Description = invPaymentStatus.Description.Trim();

                    if(invPaymentStatus.Description.Length > 100)
                    {
                        invPaymentStatus.Description.Substring(0, 100);
                    }
                    
                }
                

                if (ModelState.IsValid)
                {
                    var existingInvPaymentStatusCount =
                        _invPaymentStatusManager.GetAllInvPaymentStatusList().Count(
                            a =>
                            a.Description.ToLower() == invPaymentStatus.Description.ToLower() &&
                            a.ApplicableFor == invPaymentStatus.ApplicableFor);
                    if (existingInvPaymentStatusCount == 0)//if new entry
                    {
                        var createCurrency = _invPaymentStatusManager.AddInvPaymentStatus(invPaymentStatus);
                        ShowSuccessMessage(Messages.RecordSaveSuccessful);
                        return RedirectToAction("Index");
                        
                    } else //if duplicate record
                    {
                        ShowErrorMessage(Messages.DuplicateInvPaymentStatus);
                        return View(invPaymentStatus);
                        
                    }
                
                    
                }
                else
                {
                    return View(invPaymentStatus);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(invPaymentStatus);
            }
        }

       

        /// <summary>
        /// Active/deactive ach currency.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MISCPaymentStatusEditOrDelete)]
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Delete(int id)
        {
            _invPaymentStatusManager.DeleteInvPaymentStatus(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="invPaymentStatus">The invPaymentStatus.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MISCPaymentStatusEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, InvPaymentStatus invPaymentStatus, FormCollection collection)
        {
            try
            {
                invPaymentStatus.Id = id;
                invPaymentStatus.Description = invPaymentStatus.Description.ToUpper();
                if (ModelState.IsValid)
                {
                    var updateInvPaymentStatus = _invPaymentStatusManager.UpdateInvPaymentStatus(invPaymentStatus);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(invPaymentStatus);
                }
                
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(invPaymentStatus);
            }
        }

    }
}

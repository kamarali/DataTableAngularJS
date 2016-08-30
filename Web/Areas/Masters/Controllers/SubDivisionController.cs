using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using System;
using Iata.IS.Web.Util.Filters;


namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class SubDivisionController : ISController
    {
        private readonly ISubDivisionManager _SubDivisionManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubDivisionController"/> class.
        /// </summary>
        /// <param name="subDivisionManager">The sub division manager.</param>
        public SubDivisionController(ISubDivisionManager subDivisionManager)
        {
            _SubDivisionManager = subDivisionManager;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SubDivisionQuery)]
        public ActionResult Index()
        {
            
            const string Id = "", Name = "", CountryId = "";
            var subDivisionGrid = new SubDivisionSearch("SearchSubDivisionGrid", Url.Action("SubDivisionSearchGridData", "SubDivision", new { Id, Name, CountryId }));
            ViewData["SubDivisionGrid"] = subDivisionGrid.Instance;
           
            return View();
        }

        /// <summary>
        /// Indexes the specified sub division.
        /// </summary>
        /// <param name="subDivision">The sub division.</param>
        /// <returns></returns>
       [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SubDivisionQuery)]
        [HttpPost]
        public ActionResult Index(SubDivision subDivision)
        {
            SessionUtil.CurrentPageSelected = 1;
            var subDivisionGrid = new SubDivisionSearch("SearchSubDivisionGrid", Url.Action("SubDivisionSearchGridData", "SubDivision", new {subDivision.Id,subDivision.Name, subDivision.CountryId }));
            ViewData["SubDivisionGrid"] = subDivisionGrid.Instance;
           
            return View();
        }

        /// <summary>
        /// Subs the division search grid data.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="Name">The name.</param>
        /// <param name="CountryId">The country id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SubDivisionQuery)]
        public JsonResult SubDivisionSearchGridData(string Id, string Name, string CountryId)
        {
            var subDivisionsGrid = new SubDivisionSearch("SearchSubDivisionGrid", Url.Action("SubDivisionSearchGridData", new { Id, Name, CountryId }));
            var subDivisions = _SubDivisionManager.GetSubDivisionList(Id, Name, CountryId);
            try
            {
                return subDivisionsGrid.DataBind(subDivisions.AsQueryable());
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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SubDivisionEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified sub division.
        /// </summary>
        /// <param name="subDivision">The sub division.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
       [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SubDivisionEditOrDelete)]
        [HttpPost]
        public ActionResult Create(SubDivision subDivision, FormCollection collection)
        {
            try
            {
                subDivision.Id = subDivision.Id.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createSubDivision = _SubDivisionManager.AddSubDivision(subDivision);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(subDivision);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(subDivision);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="countryId">The country id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SubDivisionEditOrDelete)]
        public ActionResult Edit(string id,string countryId)
        {
            SubDivision subDivision = _SubDivisionManager.GetSubDivisionDetails(id,countryId);
            return View(subDivision);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="subDivision">The sub division.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
       [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SubDivisionEditOrDelete)]
        [HttpPost]
        public ActionResult Edit(string Id, SubDivision subDivision, FormCollection collection)
        {
            try
            {
                subDivision.Id = Id.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    SubDivision UpdatedsubDivision = _SubDivisionManager.UpdateSubDivision(subDivision);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(subDivision);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(subDivision);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.SubDivisionEditOrDelete)]
        public ActionResult Delete(string id,string countryId)
        {
            try
            {
                // TODO: Add delete logic here
                var subDivisionDelete = _SubDivisionManager.DeleteSubDivision(id, countryId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ShowErrorMessage(string.Format(Messages.RecordSaveException,ex.Message));
                return RedirectToAction("Index");
            }
        }
    }
}

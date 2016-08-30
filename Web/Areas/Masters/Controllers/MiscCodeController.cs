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
    public class MiscCodeController : ISController
    {
        private readonly IMiscCodeManager _MiscCodeManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="MiscCodeController"/> class.
        /// </summary>
        /// <param name="miscCodeManager">The misc code manager.</param>
        public MiscCodeController(IMiscCodeManager miscCodeManager)
         {
             _MiscCodeManager = miscCodeManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MiscCodeQuery)]
        public ActionResult Index()
        {
            const int Group = 0;
            const string Description = "";
            const string Name = "";
            var miscCodeGrid = new MiscCodeSearch("SearchMiscCodeGrid", Url.Action("MiscCodeSearchGridData", "MiscCode", new { Group, Name, Description }));
            ViewData["MiscCodeGrid"] = miscCodeGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified misc code.
        /// </summary>
        /// <param name="miscCode">The misc code.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MiscCodeQuery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MiscCode miscCode)
        {
            if (!string.IsNullOrEmpty(miscCode.Description))
            {
                miscCode.Description = miscCode.Description.Trim();
            }
            SessionUtil.CurrentPageSelected = 1;
            var miscCodesGrid = new MiscCodeSearch("SearchMiscCodeGrid", Url.Action("MiscCodeSearchGridData", new { miscCode.Group, miscCode.Name,miscCode.Description }));
            ViewData["MiscCodeGrid"] = miscCodesGrid.Instance;
            return View(miscCode);
        }

        /// <summary>
        /// Miscs the code search grid data.
        /// </summary>
        /// <param name="Code">The code.</param>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MiscCodeQuery)]
        public JsonResult MiscCodeSearchGridData(int Group, string Name,string Description)
        {
            var miscCodesGrid = new MiscCodeSearch("SearchMiscCodeGrid", Url.Action("MiscCodeSearchGridData", new { Group, Name, Description }));
            var miscCodes = _MiscCodeManager.GetMiscCodeList(Group, Name, Description);
            try
            {
                return miscCodesGrid.DataBind(miscCodes.AsQueryable());

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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MiscCodeQuery)]
        public ActionResult Details(int id)
        {
            return View();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MiscCodeEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified misc code.
        /// </summary>
        /// <param name="miscCode">The misc code.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MiscCodeEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MiscCode miscCode,FormCollection collection)
        {
            try
            {
                if (!string.IsNullOrEmpty(miscCode.Description))
                {
                    miscCode.Description = miscCode.Description.Trim();
                    if (miscCode.Description.Length > 1000)
                      miscCode.Description = miscCode.Description.Substring(0, 1000);
                }
                miscCode.Name = miscCode.Name.ToUpper();
                miscCode.ISSYSCode = "0";
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createMiscCode = _MiscCodeManager.AddMiscCode(miscCode);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(miscCode);
                }
            }
            catch (ISBusinessException ex)
            {
                ShowErrorMessage(ex.ErrorCode);
                return View(miscCode);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MiscCodeEditOrDelete)]
        public ActionResult Edit(int id)
        {
            MiscCode miscCode = _MiscCodeManager.GetMiscCodeDetails(id);
            return View(miscCode);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="miscCode">The misc code.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MiscCodeEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,MiscCode miscCode, FormCollection collection)
        {
            try
            {
                if (!string.IsNullOrEmpty(miscCode.Description))
                {
                    miscCode.Description = miscCode.Description.Trim();
                    if (miscCode.Description.Length > 1000)
                      miscCode.Description = miscCode.Description.Substring(0, 1000);
                }
                miscCode.Id = id;
                miscCode.Name = miscCode.Name.ToUpper();
                miscCode.ISSYSCode = "0";

                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    var UpdateMiscCode = _MiscCodeManager.UpdateMiscCode(miscCode);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(miscCode);
                }
                
            }
            catch (ISBusinessException ex)
            {
                ShowErrorMessage(ex.ErrorCode);
                return View(miscCode);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.MiscCodeEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteMiscCode = _MiscCodeManager.DeleteMiscCode(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
                return View();
            }
        }
    }
}

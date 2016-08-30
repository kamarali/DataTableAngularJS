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
    public class FileFormatController: ISController
    {
        private readonly IFileFormatManager _FileFormatManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileFormatController"/> class.
        /// </summary>
        /// <param name="fileFormatManager">The file format manager.</param>
        public FileFormatController(IFileFormatManager fileFormatManager)
         {
             _FileFormatManager = fileFormatManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.FileFormatQuery)]
        public ActionResult Index()
        {
            Dictionary<string, string> DownloadableList = new Dictionary<string, string>();
            DownloadableList.Add("","All");
            DownloadableList.Add("true", "Downloadable");
            DownloadableList.Add("false", "Not Downloadable");
            ViewData["SearchFileDownloadable"] = new SelectList(DownloadableList, "Key", "Value");

            const string description = "", fileVersion = "";
            const string IsFileDownloadable = "";
            var fileFormatGrid = new FileFormatSearch("SearchFileFormatGrid", Url.Action("FileFormatSearchGridData", "FileFormat", new { description, fileVersion, IsFileDownloadable }));
            ViewData["FileFormatGrid"] = fileFormatGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified file format.
        /// </summary>
        /// <param name="fileFormat">The file format.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.FileFormatQuery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FileFormat fileFormat, string SearchFileDownloadable)
        {
            if(!string.IsNullOrEmpty(fileFormat.Description))
            {
                fileFormat.Description = fileFormat.Description.Trim();
            }
            Dictionary<string, string> DownloadableList = new Dictionary<string, string>();
            DownloadableList.Add("", "All");
            DownloadableList.Add("true", "Downloadable");
            DownloadableList.Add("false", "Not Downloadable");
            ViewData["SearchFileDownloadable"] = new SelectList(DownloadableList, "Key", "Value", fileFormat.SearchFileDownloadable);

            SessionUtil.CurrentPageSelected = 1;
            fileFormat.SearchFileDownloadable = SearchFileDownloadable;
            var fileFormatsGrid = new FileFormatSearch("SearchFileFormatGrid", Url.Action("FileFormatSearchGridData", new { fileFormat.Description, fileFormat.SearchFileDownloadable }));
            ViewData["FileFormatGrid"] = fileFormatsGrid.Instance;
            return View(fileFormat);
        }

        /// <summary>
        /// Files the format search grid data.
        /// </summary>
        /// <param name="Description">The description.</param>
        /// <param name="FileDownloadable">The file downloadable.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.FileFormatQuery)]
        public JsonResult FileFormatSearchGridData(string Description, string SearchFileDownloadable)
        {

            var fileFormatsGrid = new FileFormatSearch("SearchFileFormatGrid", Url.Action("FileFormatSearchGridData", new { Description, SearchFileDownloadable }));
            var fileFormats = _FileFormatManager.GetFileFormatList(Description, SearchFileDownloadable);
            try
            {
                return fileFormatsGrid.DataBind(fileFormats.AsQueryable());

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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.FileFormatQuery)]
        public ActionResult Details(int id)
        {
            return View();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.FileFormatEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified file format.
        /// </summary>
        /// <param name="fileFormat">The file format.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.FileFormatEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FileFormat fileFormat,FormCollection collection)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileFormat.Description))
                {
                    fileFormat.Description = fileFormat.Description.Trim();
                    if (fileFormat.Description.Length > 255)
                      fileFormat.Description = fileFormat.Description.Substring(0, 255);
                }
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createFileFormat = _FileFormatManager.AddFileFormat(fileFormat);
                    ShowSuccessMessage("File Format Setup details saved successfully.");
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(fileFormat);
                }
            }
            catch(Exception ex)
            {
                ShowErrorMessage(string.Format(Messages.RecordSaveException,ex.Message));
                return View(fileFormat);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.FileFormatEditOrDelete)]
        public ActionResult Edit(int id)
        {
            FileFormat fileFormat = _FileFormatManager.GetFileFormatDetails(id);
            return View(fileFormat);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="fileFormat">The file format.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.FileFormatEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,FileFormat fileFormat, FormCollection collection)
        {
            try
            {
                fileFormat.Id = id;
                if (!string.IsNullOrEmpty(fileFormat.Description))
                {
                    fileFormat.Description = fileFormat.Description.Trim();
                    if (fileFormat.Description.Length > 255)
                      fileFormat.Description = fileFormat.Description.Substring(0, 255);
                }
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    var UpdateFileFormat = _FileFormatManager.UpdateFileFormat(fileFormat);
                    ShowSuccessMessage("File Format Setup details updated successfully.");
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(fileFormat);
                }
                
            }
            catch(Exception ex)
            {
                ShowErrorMessage(string.Format(Messages.RecordSaveException,ex.Message));
                return View(fileFormat);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.FileFormatEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteFileFormat = _FileFormatManager.DeleteFileFormat(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

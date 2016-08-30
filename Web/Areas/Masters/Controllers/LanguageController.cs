using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class LanguageController : ISController
    {
        private readonly ILanguageManager _languageManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageController"/> class.
        /// </summary>
        /// <param name="languageManager">The language manager.</param>
        public LanguageController(ILanguageManager languageManager)
         {
             _languageManager = languageManager;
         }

        //
        // GET: /Masters/Language/

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        //[ISAuthorize(Business.Security.Permissions.Masters.Masters.LanguageSetupQuery)]
        public ActionResult Index()
        {
            const string languageCode = "";
            const string languageDesc = "";
            var languageGrid = new LanguageSearch("SearchLanguageGrid", Url.Action("LanguageSearchGridData", "Language", new { languageCode, languageDesc }));
            ViewData["LanguageGrid"] = languageGrid.Instance;
            return View();
        }


        /// <summary>
        /// Indexes the specified reason code.
        /// </summary>
        /// <param name="language">The language code.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LanguageSetupQuery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Language language)
        {
            //SCP315028: no email id in correspondence
            if (!string.IsNullOrEmpty(language.Language_Desc))
            {
              language.Language_Desc = language.Language_Desc.Trim();
            }
            SessionUtil.CurrentPageSelected = 1;
            var languagesGrid = new LanguageSearch("SearchLanguageGrid", Url.Action("LanguageSearchGridData", new { language.Language_Code, language.Language_Desc }));
            ViewData["LanguageGrid"] = languagesGrid.Instance;
            return View();
        }

        /// <summary>
        /// Reasons the code search grid data.
        /// </summary>
        /// <param name="languageCode">The code.</param>
        /// <param name="languageDesc">The transaction type id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LanguageSetupQuery)]
        public JsonResult LanguageSearchGridData(string language_code, string language_desc)
        {
            
            var languagesGrid = new LanguageSearch("SearchLanguageGrid", Url.Action("LanguageSearchGridData", new { language_code, language_desc }));
            var languageCodes = _languageManager.GetLanguageList(language_code, language_desc);
            try
            {
                return languagesGrid.DataBind(languageCodes.AsQueryable());

            }
            catch (ISBusinessException be)
            {
                ViewData["errorMessage"] = be.ErrorCode;
                return null;
            }
        }

        //
        // GET: /Masters/Language/Details/

        /// <summary>
        /// Details the specified language code.
        /// </summary>
        /// <param name="languageCode">The Language Code.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LanguageSetupQuery)]
        public ActionResult Details(string languageCode)
        {
            Language langCode = _languageManager.GetAllLanguageList().Where(r => r.Language_Code == languageCode).FirstOrDefault();
            return View(langCode);
        }

        //
        // GET: /Masters/Language/Create

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LanguageSetupEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }


        //
        // POST: /Masters/Language/Create

        /// <summary>
        /// Creates the specified reason code.
        /// </summary>
        /// <param name="language">The reason code.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LanguageSetupEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Language language)
        {
            try
            {
                //315028: no email id in correspondence
                if (!string.IsNullOrEmpty(language.Language_Desc))
                {
                  language.Language_Desc = language.Language_Desc.Trim();
                  if (language.Language_Desc.Length > 200)
                    language.Language_Desc = language.Language_Desc.Substring(0, 200);
                }

                language.Language_Code = language.Language_Code.ToLower();
                if (ModelState.IsValid)
                {
                    // Add language code
                    _languageManager.AddLanguageCode(language, Server.MapPath(@"~\help\"));
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(language);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(string.Format(
                    exception.ErrorCode == ErrorCodes.DuplicateLanguage ? Messages.BPAXNS_10835 : Messages.BPAXNS_10836,
                    exception.Message));
                return View(language);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(string.Format(Messages.RecordSaveException, ex.Message));
                return View(language);
            }
        }


        //
        // GET: /Masters/Language/Edit/
        /// <summary>
        /// Edits the specified language code.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LanguageSetupEditOrDelete)]
        public ActionResult Edit(string id) // string
        {
            Language language = _languageManager.GetLanguageDetails(id);
            return View(language);
        }

        //
        // POST: /Masters/Language/Edit/
        /// <summary>
        /// Edits the specified languageCode.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="language">The language.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LanguageSetupEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Language language) //string
        {
            try
            {
                //315028: no email id in correspondence
                if (!string.IsNullOrEmpty(language.Language_Desc))
                {
                  language.Language_Desc = language.Language_Desc.Trim();
                  if (language.Language_Desc.Length > 200)
                    language.Language_Desc = language.Language_Desc.Substring(0, 200);
                }

                language.Language_Code = language.Language_Code.ToLower();
                // language.Language_Desc = language.Language_Desc.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    var UpdateLanguageCode = _languageManager.UpdateLanguageCode(language);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(language);
                }

            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(language);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(string.Format(Messages.RecordSaveException, ex.Message));
                return View(language);
            }
        }

        //
        // GET: /Masters/Language/Delete/
        /// <summary>
        /// Deletes the specified Language code.
        /// </summary>
        /// <param name="languageCode">The Language code.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.LanguageSetupEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                // TODO: Add delete logic here
                var deleteLanguageCode = _languageManager.DeleteLanguageCode(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}
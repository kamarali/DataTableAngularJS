using System.Collections.Generic;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using System;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class SourceCodeController : ISController
    {
        private readonly ISourceCodeManager _SourceCodeManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceCodeController"/> class.
        /// </summary>
        /// <param name="sourceCodeManager">The source code manager.</param>
        public SourceCodeController(ISourceCodeManager sourceCodeManager)
         {
             _SourceCodeManager = sourceCodeManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            
            const int SourceCodeIdentifier = 0;
            const int TransactionTypeId = 0;
            const string utilizationType = "";
            var sourceCodeGrid = new SourceCodeSearch("SearchSourceCodeGrid", Url.Action("SourceCodeSearchGridData", "SourceCode", new { SourceCodeIdentifier, TransactionTypeId, utilizationType }));
            ViewData["SourceCodeGrid"] = sourceCodeGrid.Instance;
           
            return View();
        }

        /// <summary>
        /// Indexes the specified source code.
        /// </summary>
        /// <param name="sourceCode">The source code.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(SourceCode sourceCode)
        {
            SessionUtil.CurrentPageSelected = 1;
            var sourceCodesGrid = new SourceCodeSearch("SearchSourceCodeGrid", Url.Action("SourceCodeSearchGridData", new { sourceCode.SourceCodeIdentifier, sourceCode.TransactionTypeId, sourceCode.UtilizationType }));
            ViewData["SourceCodeGrid"] = sourceCodesGrid.Instance;
           
            return View();
        }

        /// <summary>
        /// Sources the code search grid data.
        /// </summary>
        /// <param name="SourceCodeIdentifier">The source code identifier.</param>
        /// <param name="TransactionTypeId">The transaction type id.</param>
        /// <param name="utilizationType">Type of the utilization.</param>
        /// <returns></returns>
        public JsonResult SourceCodeSearchGridData(int SourceCodeIdentifier,int TransactionTypeId,string utilizationType)
        {

            var sourceCodesGrid = new SourceCodeSearch("SearchSourceCodeGrid", Url.Action("SourceCodeSearchGridData", new { SourceCodeIdentifier, TransactionTypeId, utilizationType }));
            var sourceCodes = _SourceCodeManager.GetSourceCodeList(SourceCodeIdentifier, TransactionTypeId, utilizationType);
            try
            {
                return sourceCodesGrid.DataBind(sourceCodes.AsQueryable());

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
        /// Creates the specified source code.
        /// </summary>
        /// <param name="sourceCode">The source code.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(SourceCode sourceCode,FormCollection collection)
        {
            try
            {
                sourceCode.UtilizationType = sourceCode.UtilizationType.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createSourceCode = _SourceCodeManager.AddSourceCode(sourceCode);
                    ShowSuccessMessage("SourceCode details saved successfully");
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(sourceCode);
                }
            }
            catch
            {
                ShowErrorMessage(Messages.RecordSaveException);
                return View(sourceCode);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            SourceCode sourceCode = _SourceCodeManager.GetSourceCodeDetails(id);
            return View(sourceCode);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="sourceCode">The source code.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(int id,SourceCode sourceCode, FormCollection collection)
        {
            try
            {
                sourceCode.Id = id;
                sourceCode.UtilizationType = sourceCode.UtilizationType.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    var UpdateSourceCode = _SourceCodeManager.UpdateSourceCode(sourceCode);
                    ShowSuccessMessage("SourceCode details saved successfully");
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(sourceCode);
                }
                
            }
            catch
            {
                ShowErrorMessage(Messages.RecordSaveException);
                return View(sourceCode);
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
                var deleteSourceCode = _SourceCodeManager.DeleteSourceCode(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

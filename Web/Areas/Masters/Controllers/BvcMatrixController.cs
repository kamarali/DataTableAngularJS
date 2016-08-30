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
    public class BvcMatrixController : ISController
    {
        private readonly IBvcMatrixManager _BvcMatrixManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BvcMatrixController"/> class.
        /// </summary>
        /// <param name="bvcMatrixManager">The bvcMatrix manager.</param>
        public BvcMatrixController(IBvcMatrixManager bvcMatrixManager)
        {
            _BvcMatrixManager = bvcMatrixManager;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.BvcMatrixQuery)]
        public ActionResult Index()
        {
            
            const string BvcMatrixCodeIcao = "";
            const string  ValidatedPmi = "",  effectiveFrom = "",  effectiveTo = "";
            var bvcMatrixGrid = new BvcMatrixSearch("SearchBvcMatrixGrid", Url.Action("BvcMatrixSearchGridData", "BvcMatrix", new { ValidatedPmi, effectiveFrom, effectiveTo }));
            ViewData["BvcMatrixGrid"] = bvcMatrixGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified bvcMatrix.
        /// </summary>
        /// <param name="bvcMatrix">The bvcMatrix.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.BvcMatrixQuery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(BvcMatrix bvcMatrix)
        {
            SessionUtil.CurrentPageSelected = 1;
            var bvcMatrixGrid = new BvcMatrixSearch("SearchBvcMatrixGrid", Url.Action("BvcMatrixSearchGridData", "BvcMatrix", new { bvcMatrix.ValidatedPmi, bvcMatrix.EffectiveFrom, bvcMatrix.EffectiveTo }));
            ViewData["BvcMatrixGrid"] = bvcMatrixGrid.Instance;
            return View();
        }

        /// <summary>
        /// BvcMatrixs the search grid data.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="BvcMatrixCodeIcao">The bvcMatrix code icao.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.BvcMatrixQuery)]
        public JsonResult BvcMatrixSearchGridData(string ValidatedPmi, string effectiveFrom, string effectiveTo)
        {
            var bvcMatrixsGrid = new BvcMatrixSearch("SearchBvcMatrixGrid", Url.Action("BvcMatrixSearchGridData", new { ValidatedPmi, effectiveFrom, effectiveTo }));
            var bvcMatrixs = _BvcMatrixManager.GetBvcMatrixList(ValidatedPmi, effectiveFrom, effectiveTo);
            try
            {
                return bvcMatrixsGrid.DataBind(bvcMatrixs.AsQueryable());
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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.BvcMatrixEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified bvcMatrix.
        /// </summary>
        /// <param name="bvcMatrix">The bvcMatrix.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.BvcMatrixEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BvcMatrix bvcMatrix, FormCollection collection)
        {
            try
            {
              if (Convert.ToInt32(bvcMatrix.EffectiveFrom) > Convert.ToInt32(bvcMatrix.EffectiveTo))
              {
                throw new ISBusinessException(ErrorCodes.InvalidEffectiveFromDate);
              }

                bvcMatrix.ValidatedPmi = bvcMatrix.ValidatedPmi.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createBvcMatrix = _BvcMatrixManager.AddBvcMatrix(bvcMatrix);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(bvcMatrix);
                }
            }
            catch (ISBusinessException exception)
            {
              ShowErrorMessage(exception.ErrorCode);
              return View(bvcMatrix);
            }
            catch
            {
                return View(bvcMatrix);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.BvcMatrixEditOrDelete)]
        public ActionResult Edit(int Id)
        {
            BvcMatrix bvcMatrix = _BvcMatrixManager.GetBvcMatrixDetails(Id);
            return View(bvcMatrix);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="bvcMatrix">The bvcMatrix.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.BvcMatrixEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int Id, BvcMatrix bvcMatrix, FormCollection collection)
        {
            try
            {
              if (Convert.ToInt32(bvcMatrix.EffectiveFrom) > Convert.ToInt32(bvcMatrix.EffectiveTo))
              {
                throw new ISBusinessException(ErrorCodes.InvalidEffectiveFromDate);
              }
                bvcMatrix.Id = Id;
                bvcMatrix.ValidatedPmi = bvcMatrix.ValidatedPmi.ToUpper();
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    BvcMatrix UpdatedbvcMatrix = _BvcMatrixManager.UpdateBvcMatrix(bvcMatrix);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(bvcMatrix);
                }
            }
            catch (ISBusinessException exception)
            {
              ShowErrorMessage(exception.ErrorCode);
              return View(bvcMatrix);
            }

            catch
            {
                return View(bvcMatrix);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.BvcMatrixEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(int Id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var bvcMatrixDelete = _BvcMatrixManager.DeleteBvcMatrix(Id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
    }
}

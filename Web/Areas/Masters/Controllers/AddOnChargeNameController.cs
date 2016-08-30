using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.Masters;
using Iata.IS.Model.MiscUatp.Common;
using System.Linq;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    public class AddOnChargeNameController : ISController
    {
        private readonly IAddOnChargeNameManager _AddOnChargeNameManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddOnChargeNameController"/> class.
        /// </summary>
        /// <param name="addOnChargeNameManager">The add on charge name manager.</param>
        public AddOnChargeNameController(IAddOnChargeNameManager addOnChargeNameManager)
         {
             _AddOnChargeNameManager = addOnChargeNameManager;
         }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {
            const int BillingCategoryId = 0;
            const string Name = "";
            var addOnChargeNameGrid = new AddOnChargeNameSearch("SearchAddOnChargeNameGrid", Url.Action("AddOnChargeNameSearchGridData", "AddOnChargeName", new { Name, BillingCategoryId }));
            ViewData["AddOnChargeNameGrid"] = addOnChargeNameGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified add on charge name.
        /// </summary>
        /// <param name="addOnChargeName">Name of the add on charge.</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult Index(AddOnChargeName addOnChargeName)
        {
            SessionUtil.CurrentPageSelected = 1;
            //Get  AddOnChargeName present in database
            var addOnChargeNameGrid = new AddOnChargeNameSearch("SearchAddOnChargeNameGrid", Url.Action("AddOnChargeNameSearchGridData", "AddOnChargeName", new { addOnChargeName.Name, addOnChargeName.BillingCategoryId }));
            ViewData["AddOnChargeNameGrid"] = addOnChargeNameGrid.Instance;
            //Display AddOnChargeName on UI
            return View();
        }

        /// <summary>
        /// Adds the on charge name search grid data.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="BillingCategoryId">The billing category id.</param>
        /// <returns></returns>
        [Authorize]
        public JsonResult AddOnChargeNameSearchGridData(string Name, int BillingCategoryId)
        {
            var addOnChargeNameGrid = new AddOnChargeNameSearch("SearchAddOnChargeNameGrid", Url.Action("AddOnChargeNameSearchGridData", new { Name, BillingCategoryId }));
            var addOnChargeNames = _AddOnChargeNameManager.GetAddOnChargeNameList(Name, BillingCategoryId);
            try
            {
                return addOnChargeNameGrid.DataBind(addOnChargeNames.AsQueryable());
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
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified add on charge name.
        /// </summary>
        /// <param name="addOnChargeName">Name of the add on charge.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult Create(AddOnChargeName addOnChargeName,FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createCountry = _AddOnChargeNameManager.AddAddOnChargeName(addOnChargeName);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(addOnChargeName);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(addOnChargeName);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        public ActionResult Edit(int Id)
        {
            AddOnChargeName addOnChargeName = _AddOnChargeNameManager.GetAddOnChargeNameDetails(Id);
            return View(addOnChargeName);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="addOnChargeName">Name of the add on charge.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int Id, AddOnChargeName addOnChargeName, FormCollection collection)
        {
            try
            {
                addOnChargeName.Id = Id;
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    AddOnChargeName UpdatedaddOnChargeName = _AddOnChargeNameManager.UpdateAddOnChargeName(addOnChargeName);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(addOnChargeName);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(addOnChargeName);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult Delete(int Id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var countryDelete = _AddOnChargeNameManager.DeleteAddOnChargeName(Id);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}
using System;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Web.UIModel.Grid.Masters;
using System.Linq;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Masters.Controllers
{
    //CMP #553: ACH Requirement for Multiple Currency Handling.
    public class AchCurrencySetUpController : ISController
    {
        private readonly IAchCurrencySetUpManager _achCurrencySetUpManager;
        private readonly IReferenceManager _referenceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AchCurrencySetUpController"/> class.
        /// </summary>
        /// <param name="achCurrencySetUpManager"></param>
        /// <param name="referenceManager"></param>
        public AchCurrencySetUpController(IAchCurrencySetUpManager achCurrencySetUpManager, IReferenceManager referenceManager)
        {
            _achCurrencySetUpManager = achCurrencySetUpManager;
            _referenceManager = referenceManager;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AchCurrencySetUpQuery)]
        public ActionResult Index()
        {
            //Get AchCurrencySetUp present in database
            var achCurrencySetUpGrid = new AchCurrencySetUpSearch("SearchAchCurrencySetUpGrid", Url.Action("AchCurrencySetUpSearchGridData", "AchCurrencySetUp", new { }));
            ViewData["AchCurrencySetUpGrid"] = achCurrencySetUpGrid.Instance;
            //Default size of grid.
            achCurrencySetUpGrid.DefaultPageSize = 5;
            //Display AchCurrencySetUp on UI
            return View();
        }

        /// <summary>
        /// Indexes the specified.
        /// </summary>
        /// <param name="achCurrencySetUp"></param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AchCurrencySetUpQuery)]
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Index(AchCurrencySetUp achCurrencySetUp)
        {
            var achCurrencySetUpGrid = new AchCurrencySetUpSearch("SearchAchCurrencySetUpGrid", Url.Action("AchCurrencySetUpSearchGridData", "AchCurrencySetUp", new { currencyCode = achCurrencySetUp.Name }));
            ViewData["AchCurrencySetUpGrid"] = achCurrencySetUpGrid.Instance;
            return View(achCurrencySetUp);
        }

        /// <summary>
        /// This function is used to search ach currency set up data.
        /// </summary>
        /// <param name="currencyCode">The currencyCode.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AchCurrencySetUpQuery)]
        public JsonResult AchCurrencySetUpSearchGridData(string currencyCode)
        {
            var achCurrencySetUpsGrid = new AchCurrencySetUpSearch("SearchAchCurrencySetUpGrid", Url.Action("AchCurrencySetUpSearchGridData", new { currencyCode }));
            var achCurrencySetUps = _achCurrencySetUpManager.GetAchCurrencySetUpList(currencyCode);

            return achCurrencySetUpsGrid.DataBind(achCurrencySetUps.AsQueryable());
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AchCurrencySetUpEditOrDelete)]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates the specified Ach currency.
        /// </summary>
        /// <param name="achCurrencySetUp"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AchCurrencySetUpEditOrDelete)]
        [HttpPost]
        [ValidateInput(true)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AchCurrencySetUp achCurrencySetUp, FormCollection collection)
        {
            try
            {
                achCurrencySetUp.Id = Convert.ToInt32(collection["achCurrencyCode"]);
                _achCurrencySetUpManager.AddAchCurrencySetUp(achCurrencySetUp);
                return RedirectToAction("Index");
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(achCurrencySetUp);
            }
        }

        /// <summary>
        /// Active/deactive ach currency.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.AchCurrencySetUpEditOrDelete)]
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult Delete(int id)
        {
            _achCurrencySetUpManager.DeleteAchCurrencySetUp(id);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// This function is used to get currency code name based on currency code.
        /// </summary>
        /// <param name="currencyCodeNum"></param>
        /// <returns></returns>
        public JsonResult GetCurrencyCodeName(int currencyCodeNum)
        {
            //Get currency code from DB.
            string currencyCodeName = _referenceManager.GetCurrencyCodeName(currencyCodeNum);

            return new JsonResult { Data = currencyCodeName, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


         /// <summary>
        /// This function is used to check currency is valid or not.
        /// </summary>
        /// <param name="currencyCodeNum"></param>
        /// <returns></returns>
        public JsonResult IsValidCurrency(int currencyCodeNum)
        {
             //Check, given currency is valid or not.
            bool currencyCodeName = _referenceManager.IsValidCurrency(currencyCodeNum);

            return new JsonResult { Data = currencyCodeName, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        
    }
}
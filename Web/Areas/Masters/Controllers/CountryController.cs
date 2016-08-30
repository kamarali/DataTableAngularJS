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
    public class CountryController : ISController
    {
         private readonly ICountryManager _CountryManager = null;

         /// <summary>
         /// Initializes a new instance of the <see cref="CountryController"/> class.
         /// </summary>
         /// <param name="countryManager">The country manager.</param>
         public CountryController(ICountryManager countryManager)
         {
             _CountryManager = countryManager;
         }

         /// <summary>
         /// Indexes this instance.
         /// </summary>
         /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CountryQuery)]
        public ActionResult Index()
        {

            const string Id = "";
            const string Name = "";
            var countryGrid = new CountrySearch("SearchCountryGrid", Url.Action("CountrySearchGridData", "Country", new { Name, Id }));
            ViewData["CountryGrid"] = countryGrid.Instance;
            return View();
        }

        /// <summary>
        /// Indexes the specified country.
        /// </summary>
        /// <param name="country">The country.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CountryQuery)]
        public ActionResult Index(Country country)
        {
            SessionUtil.CurrentPageSelected = 1;
            var countryGrid = new CountrySearch("SearchCountryGrid", Url.Action("CountrySearchGridData", "Country", new { country.Name, country.Id }));
            ViewData["CountryGrid"] = countryGrid.Instance;
            return View();
        }

        /// <summary>
        /// Countries the search grid data.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="CountryCodeIcao">The country code icao.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CountryQuery)]
        public JsonResult CountrySearchGridData(string Name, string Id)
        {
            var countrysGrid = new CountrySearch("SearchCountryGrid", Url.Action("CountrySearchGridData", new { Name, Id }));
            var countrys = _CountryManager.GetCountryList(Name, Id);
            try
            {
                return countrysGrid.DataBind(countrys.AsQueryable());
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
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CountryEditOrDelete)]
        public ActionResult Create()
        {
            GetDsFormatList("");
            return View();
        }
        private void GetDsFormatList(string selected)
        {
            Dictionary<string, string> DsFormatList = new Dictionary<string, string>();
            DsFormatList.Add("", "None");
            DsFormatList.Add("P", "PDF");
            DsFormatList.Add("X", "XML");
            ViewData["DsFormatList"] = new SelectList(DsFormatList, "Key", "Value", selected);
        }

        /// <summary>
        /// Creates the specified country.
        /// </summary>
        /// <param name="country">The country.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CountryEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Country country,FormCollection collection)
        {
            try
            {
                country.Id = country.Id.ToUpper();
                //SCP151404 - Master Tables - mismatch in case sensitivity when capturing new master entries 
                //country.Name = country.Name.ToUpper();
                GetDsFormatList(country.DsFormat);
                if (ModelState.IsValid)
                {
                    // TODO: Add insert logic here
                    var createCountry = _CountryManager.AddCountry(country);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(country);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(country);
            }
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CountryEditOrDelete)]
        public ActionResult Edit(string Id)
        {
            Country country = _CountryManager.GetCountryDetails(Id);
            GetDsFormatList(country.DsFormat);
            return View(country);
        }

        /// <summary>
        /// Edits the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="country">The country.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CountryEditOrDelete)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string Id, Country country, FormCollection collection)
        {
            try
            {
                country.Id = Id;
                country.Id = country.Id.ToUpper();
                //SCP151404 - Master Tables - mismatch in case sensitivity when capturing new master entries 
                //country.Name = country.Name.ToUpper();
                GetDsFormatList(country.DsFormat);
                if (ModelState.IsValid)
                {
                    // TODO: Add update logic here
                    Country Updatedcountry = _CountryManager.UpdateCountry(country);
                    ShowSuccessMessage(Messages.RecordSaveSuccessful);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(country);
                }
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(country);
            }
        }

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Masters.Masters.CountryEditOrDelete)]
        [HttpPost]
        public ActionResult Delete(string Id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var countryDelete = _CountryManager.DeleteCountry(Id);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }


    }
}

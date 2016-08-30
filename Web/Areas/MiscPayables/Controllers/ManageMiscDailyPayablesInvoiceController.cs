using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Web.Controllers.Base;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Business.Common;
using System.Web.Mvc;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Web.Util;
using System.Web.Script.Serialization;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Enums;
using Iata.IS.Web.UIModel.Grid.MUPayables;
using System.IO;

namespace Iata.IS.Web.Areas.MiscPayables.Controllers
{
    public class ManageMiscDailyPayablesInvoiceController : ManageMUPayablesControllerBase
    {
        private const string SearchResultGridActionMethod = "DailyPayableSearchResultGridData";
        public ManageMiscDailyPayablesInvoiceController(IMiscInvoiceManager miscInvoiceManager, ICalendarManager calendarManager)
            : base(miscInvoiceManager, calendarManager)
        {
          AreaText = "MiscDailyPay";
        }

        protected override BillingCategoryType BillingCategory
        {
            get
            {
                return BillingCategoryType.Misc;
            }
            set
            {
                base.BillingCategory = value;
            }
        }

        /// <summary>
        /// CMP #655: IS-WEB Display per Location ID
        /// Desc: Due to URL limitation issue, have seperate out Get and Post method
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
        [ISAuthorize(Business.Security.Permissions.Menu.MiscPayDailyBilateralDelivery)]
        public ActionResult Index(bool isRedirectUponLogin = false)
        {
            //CMP #665: User Related Enhancements-FRS-v1.2.doc
            var miscSearchCriteria = new MiscSearchCriteria {IsRedirectUponLogin = isRedirectUponLogin};
            return View(IndexInvoiceSearch(miscSearchCriteria));
        }

        /// <summary>
        /// CMP #655: IS-WEB Display per Location ID
        /// Desc: Due to URL limitation issue, have seperate out Get and Post method
        /// </summary>
        /// <param name="miscSearchCriteria"></param>
        /// <returns></returns>
       [ValidateAntiForgeryToken]
        [HttpPost]
        // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
        [ISAuthorize(Business.Security.Permissions.Menu.MiscPayDailyBilateralDelivery)]
        public override ActionResult Index(MiscSearchCriteria miscSearchCriteria)
        {

            return View(IndexInvoiceSearch(miscSearchCriteria));
        }


        /// <summary>
        /// CMP #655: IS-WEB Display per Location ID
        /// </summary>
        /// <param name="miscSearchCriteria"></param>
        /// <returns></returns>
        private MiscSearchCriteria IndexInvoiceSearch(MiscSearchCriteria miscSearchCriteria)
        {
            SessionUtil.SearchType = "ManageDailyPayablesInvoice";

            try
            {
                if (miscSearchCriteria != null)
                {
                    miscSearchCriteria.BillingCategory = BillingCategory;
                    // If Billing Member text is empty, reset the Billing Member Id
                    // this check is made to handle the scenario where user has explicitly deleted the contents 
                    // from Billing Member text box.
                    if (string.IsNullOrEmpty(miscSearchCriteria.BilledMemberText))
                    {
                        miscSearchCriteria.BillingMemberId = -1;
                    }

                    //CMP-665-User Related Enhancements-FRS-v1.2.doc
                    //[Sec 2.8 Conditional Redirection of Users upon Login in IS-WEB]
                    if (miscSearchCriteria.IsRedirectUponLogin)
                    {
                        miscSearchCriteria.DeliveryDateFrom = DateTime.UtcNow.Date.AddYears(-1).AddDays(1);
                        miscSearchCriteria.DeliveryDateTo = DateTime.UtcNow.Date;
                    }
                    else
                    {
                        miscSearchCriteria.DeliveryDateFrom = (miscSearchCriteria.DeliveryDateFrom.HasValue)
                                                                  ? miscSearchCriteria.DeliveryDateFrom.Value.
                                                                        ToLocalTime()
                                                                  : DateTime.UtcNow.Date.AddDays(-1);
                        miscSearchCriteria.DeliveryDateTo = (miscSearchCriteria.DeliveryDateTo.HasValue)
                                                            ? miscSearchCriteria.DeliveryDateTo.Value.ToLocalTime()
                                                            : DateTime.UtcNow.Date.AddDays(-1);
                    }
                    
                    //CMP #655: IS-WEB Display per Location ID          
                    //2.11	MISC IS-WEB PAYABLES - VIEW DAILY BILATERAL INVOICES SCREEN
                    var memberLocation = Ioc.Resolve<IManageContactsManager>(typeof(IManageContactsManager));
                    var associatedLocations = memberLocation.GetMemberAssociationLocForSearch(SessionUtil.UserId, SessionUtil.MemberId);
                    ViewData["AssociatedLocation"] = new MultiSelectList(associatedLocations.ToArray(), "locationId", "locationCode");
                    if (miscSearchCriteria.BillingMemberLoc == null)
                    { // For Default search, all association locations should be considered for grid result
                        foreach (var item in associatedLocations)
                        {
                            miscSearchCriteria.BillingMemberLoc += "," + item.LocationCode;
                        }
                        if (associatedLocations.Count == 0) miscSearchCriteria.BillingMemberLoc = ",0";
                    }
                    else
                    {// server Side Validation for Associatin Location
                        var selectedBillingMemberLocationList = miscSearchCriteria.BillingMemberLoc.Split(Convert.ToChar(","));
                        miscSearchCriteria.BillingMemberLoc = "";
                        foreach (var location in from location in selectedBillingMemberLocationList
                                                 where location != null
                                                 let contains = associatedLocations.SingleOrDefault(l => l.LocationCode == location)
                                                 where contains != null
                                                 select location)
                        {
                            miscSearchCriteria.BillingMemberLoc += "," + location;
                        }
                        if (miscSearchCriteria.BillingMemberLoc.Length == 0) miscSearchCriteria.BillingMemberLoc = ",0";
                    }
                    // End Code CMP#655

                }

                string criteria = miscSearchCriteria != null ? new JavaScriptSerializer().Serialize(miscSearchCriteria) : string.Empty;

                // CMP #665: User Related Enhancements-FRS-v1.2.doc [Sec 2.8 Conditional Redirection of users upon login in is-web]
                var invoiceSearchGrid = new MiscDailyPayableInvoiceSearchGrid(ControlIdConstants.SearchGrid, Url.Action(SearchResultGridActionMethod, new
                {
                    criteria
                }), miscSearchCriteria != null ? miscSearchCriteria.IsRedirectUponLogin : false);

                ViewData[ViewDataConstants.SearchGrid] = invoiceSearchGrid.Instance;

                //CMP #665-User Related Enhancements-FRS-v1.2 [Sec 2.10: IS-WEB MISC Daily Payables Invoice Search Screen] 
                var attachmentGrid = new MiscPayableAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData", new { }));
                ViewData[ViewDataConstants.AttachmentResultGrid] = attachmentGrid.Instance;
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
            }
            return miscSearchCriteria;
        }

        /// <summary>
        /// Fetch invoice searched result and display it in grid
        /// </summary>
        /// <returns></returns>
        public JsonResult DailyPayableSearchResultGridData(string criteria)
        {
            MiscSearchCriteria searchCriteria = null;

            if (Request.UrlReferrer != null)
            {
                SessionUtil.InvoiceSearchCriteria = Request.UrlReferrer.ToString();
                // Clearing the other two session variables so that 'Back to Billing History' is not seen.
                SessionUtil.MiscCorrSearchCriteria = null;
                SessionUtil.MiscInvoiceSearchCriteria = null;
            }
            //// TODO : Exception handling 
            if (!string.IsNullOrEmpty(criteria))
            {
                searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(MiscSearchCriteria)) as MiscSearchCriteria;
            }
            if (searchCriteria == null)
            {
                // if not criteria is fetch or can be created using the string, create empty default search.
                searchCriteria = new MiscSearchCriteria();
            }

            searchCriteria.BillingCategory = BillingCategory;
            //  searchCriteria.BillingPeriod = searchCriteria.BillingPeriod == 0 ? CalendarManager.GetCurrentBillingPeriod(DateTime.UtcNow).Period : searchCriteria.BillingPeriod;

            //CMP #655: IS-WEB Display per Location ID
            //2.11	MISC IS-WEB PAYABLES - VIEW DAILY BILATERAL INVOICES SCREEN
            if (searchCriteria.BillingMemberLoc != null)
            {
                if (searchCriteria.BillingMemberLoc.Length > 0)
                    searchCriteria.BillingMemberLoc = searchCriteria.BillingMemberLoc.Substring(1, searchCriteria.BillingMemberLoc.Length - 1);
            }


            // Create grid instance and retrieve data from database
            // CMP #665: User Related Enhancements-FRS-v1.2.doc [Sec 2.8 Conditional Redirection of users upon login in is-web]
            var invoiceSearchGrid = new MiscDailyPayableInvoiceSearchGrid(ControlIdConstants.SearchGrid,
                                                          Url.Action(SearchResultGridActionMethod,
                                                                     new
                                                                     {
                                                                         searchCriteria
                                                                     }), searchCriteria.IsRedirectUponLogin);

            // add billed member id to search criteria.
            searchCriteria.BilledMemberId = SessionUtil.MemberId;

            //SCP382334: Daily Bilateral screen is not loading
            var invoiceSearchedData = _miscUatpInvoiceManager.SearchDailyPayableInvoices(searchCriteria);
            return invoiceSearchGrid.DataBind(invoiceSearchedData.AsQueryable());
        }
    }
}

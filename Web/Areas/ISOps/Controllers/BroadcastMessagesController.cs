using System;
using System.Globalization;
using System.Web.Mvc;
using Iata.IS.AdminSystem;
using Iata.IS.Business;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.BroadcastMessages;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.ISOps.Controllers
{
    public class BroadcastMessagesController : ISController
    {
        public IBroadcastMessagesManager BroadcastMessagesManager { get; set; }

        [HttpGet]

        [ISAuthorize(Business.Security.Permissions.ISOps.ManageMasters.BroadcastNotificationAccess)]
        public ActionResult BroadcastMessages(string isDisplayMessage)
        {
            string startDateTime = string.Format("{0}", CalendarManager.ConvertUtcTimeToYmq(DateTime.UtcNow).ToString("dd-MMM-yy"));
            string timeHourMinutes = string.Format("{0}", CalendarManager.ConvertUtcTimeToYmq(DateTime.UtcNow).ToString("HH:mm"));

            ViewData["StartDateTimeValue"] = startDateTime;
            ViewData["TimeHourMinutes"] = timeHourMinutes;
            SetDefaultExpiryDate();
            // If send the message by clicking send button then it's set the initialization of send radio button container.
            ViewData["SendMesg"] = 0;
            if (!String.IsNullOrEmpty(isDisplayMessage))
                ViewData["SendMesg"] = 1;

            if (TempData["BrodMessage"] != null){
              ViewData["BrodMessage"] = TempData["BrodMessage"];
            }
            else
            {
              ViewData["BrodMessage"] = string.Empty;
            }
            return View();
        }

        /// <summary>
        /// Action method corresponding to view through which Announcement is added
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Announcements(ISMessagesAlerts annoucement)
        {

            // Add announcement record to database through manager method //dd-MMM-yyTHH:mm:ss
            try
            {
                if (string.IsNullOrEmpty(annoucement.StartDateTimeValue) ||
                    string.IsNullOrEmpty(annoucement.TimeHourMinutes))
                {
                    throw new ISBusinessException(ErrorCodes.InvalidDateAndTimeForAnnouncements);
                }

                string startDateTime = annoucement.StartDateTimeValue + " " + annoucement.TimeHourMinutes + ":49";
                // Get Start date DateTime object from Start date string
                var startDt = Convert.ToDateTime(startDateTime);

                // Add 23 hours, 59 minutes and 59 seconds to Expiry date
                var duration = new TimeSpan(0, 23, 59, 59);
                var expDate = annoucement.ExpiryDate.Add(duration);
                annoucement.ExpiryDate = expDate;

                // Check whether Expiry date is less than Start date, if yes throw an exception
                var isExpDateLessThanStartDate = DateTime.Compare(startDt, expDate);
                if(isExpDateLessThanStartDate == 1)
                {
                  throw new ISBusinessException(ErrorCodes.InvalidDateAndTimeForAnnouncementStartAndExpiryDate);
                }

              if (annoucement != null)
                {
                    DateTime dtValue;
                    var isParsed = DateTime.TryParseExact(startDateTime, FormatConstants.DynamicFieldDateTimeFormats,
                                                          CultureInfo.InvariantCulture, DateTimeStyles.None, out dtValue);
                    if (isParsed)
                    {
                        annoucement.StartDateTime = dtValue;
                    }

                    BroadcastMessagesManager.AddAnnouncements(annoucement);
                    ShowSuccessMessage("Announcement saved successfully.");
                    TempData["BrodMessage"] = annoucement.Message;
                }
            }
            catch (ISBusinessException ex)
            {
                var setExpiryDate = string.Format("{0}", annoucement.ExpiryDate.ToString("dd-MMM-yy"));
                var setExpiryDateSplit = setExpiryDate.Split(' ');
                ViewData["DefaultExpiryDate"] = setExpiryDateSplit.Length > 0 ? setExpiryDateSplit[0] : "";

                ViewData["Announcements"] = annoucement;
                ViewData["StartDateTimeValue"] = annoucement.StartDateTimeValue;
                ViewData["TimeHourMinutes"] = annoucement.TimeHourMinutes;
                ShowErrorMessage(ex.ErrorCode);
                return View("BroadcastMessages");
            }

            // Following code is used to set values back to view after Broadcasting message
            string startDate = string.Format("{0}", CalendarManager.ConvertUtcTimeToYmq(DateTime.UtcNow).ToString("dd-MMM-yy"));
            string timeHourMinutes = string.Format("{0}", CalendarManager.ConvertUtcTimeToYmq(DateTime.UtcNow).ToString("HH:mm"));

            var userExpiryDate = string.Format("{0}", annoucement.ExpiryDate.ToString("dd-MMM-yy"));
            var userExpiryDateSplit = userExpiryDate.Split(' ');
            ViewData["DefaultExpiryDate"] = userExpiryDateSplit.Length > 0 ? userExpiryDateSplit[0] : "";

            ViewData["StartDateTimeValue"] = startDate;
            ViewData["TimeHourMinutes"] = timeHourMinutes;
            
            // If send the message by clicking send button then it's set the initialization of send radio button container.
            ViewData["SendMesg"] = 0;
            
            if (TempData["BrodMessage"] != null)
            {
              ViewData["BrodMessage"] = TempData["BrodMessage"];
            }
            else
            {
              ViewData["BrodMessage"] = string.Empty;
            }
          
            return View("BroadcastMessages");
        }
        private DateTime SetDefaultExpiryDate()
        {
            var defaultExpiryDate = DateTime.UtcNow.Date.AddDays(SystemParameters.Instance.General.DefaultExpiryDaysforAnnoucements);
            var defaultExpiryDateSplit = defaultExpiryDate.ToString(FormatConstants.DateFormat).Split(' ');
            ViewData["DefaultExpiryDate"] = defaultExpiryDateSplit.Length > 0 ? defaultExpiryDateSplit[0] : "";

            return Convert.ToDateTime(ViewData["DefaultExpiryDate"]);
        }

        /// <summary>
        ///  Action method corresponding to view through which Message is added
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendMessages(ISMessageRecipients message)
        {
            // Add message record to database through manager method
            if (message != null)
            {
                try
                {
                    message.IsMessagesAlerts.StartDateTime = DateTime.UtcNow;
                    message.IsMessagesAlerts.ExpiryDate = DateTime.UtcNow.Date.AddDays(SystemParameters.Instance.General.DefaultExpiryDateforMessages);

                    message = BroadcastMessagesManager.AddMessages(message);
                    ShowSuccessMessage("Message sent successfully");

                    SetDefaultExpiryDate();
                }
                catch (ISBusinessException ex)
                {
                    ViewData["Messages"] = message;
                    SetDefaultExpiryDate();
                    ShowErrorMessage(ex.ErrorCode);

                    return View("BroadcastMessages");
                }
            }

            return RedirectToAction("BroadcastMessages", new { isDisplayMessage = "true" });
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using Castle.Core.Smtp;
using Iata.IS.AdminSystem;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.Business.Common;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core.Configuration;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Core.File;
using Iata.IS.Data;
using Iata.IS.Data.SubmissionDeadline;
using Iata.IS.Model.BroadcastMessages;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.SubmissionDeadline;
using log4net;
using log4net.Repository.Hierarchy;
using NVelocity;
using Iata.IS.Business.BroadcastMessages.Impl;
using SystemParameters = Iata.IS.AdminSystem.SystemParameters;


namespace Iata.IS.Business.SubmissionDeadline.Impl
{
    public class ISSubmissionDeadlineAlertImpl : IISSubmissionDeadlineAlert
    {
        public ISSubmissionDeadline Submissiondeadline;

        public ISSubmissionDeadlineAlertImpl(ISSubmissionDeadline _submissionDeadline)
        {
            Submissiondeadline = _submissionDeadline;
        }
        #region Private Members

        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public Properties

        public ICalendarManager CalendarManager { get; set; }

       

        public IBroadcastMessagesManager BroadcastMessages { get; set; }

        public List<SubmissionData> SubmissiondataModel { get; set; }

        public IMemberManager memberManager { get; set; }

        private readonly string _rootPath = ConfigurationManager.AppSettings["RootPath"];

        private readonly string _levelThreeFolderName = ConfigurationManager.AppSettings["LevelThreeFolderName"];

        public SystemParameters systemParametrs { get; set; }

        #endregion
        /// <summary>
        /// Fetch all Invoices which are having status like "Open" , "ready For submission", "Validation Error-WEB Invoice" for the current billing month, year ,period
        /// generate an email and send to the Billing Entity
        /// And generate an alert box 
        /// </summary>
        public void GenerateSubmissionDeadlineAlerts()
        {
            try
            {
                // get the current billing month , year and period
                //BillingPeriod billingPeriod = CalendarManager.GetCurrentBillingPeriod();
                BillingPeriod billingPeriod;
                try
                {
                    billingPeriod = CalendarManager.GetCurrentBillingPeriod();
                }
                catch (ISCalendarDataNotFoundException)
                {
                    billingPeriod = CalendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
                }

                _logger.Debug(String.Format("From Filter- Year: {0}, Month: {1}, Period: {2}", billingPeriod.Month, billingPeriod.Year, billingPeriod.Period));

                // Call the Method "Getpendinginvoices" which returns invoices 
                SubmissiondataModel = Submissiondeadline.GetPendingInvoices(billingPeriod.Month, billingPeriod.Year,
                                                                            billingPeriod.Period); 
                // Check for the count ; if count is greater then 0 then fetch contact list and send email to it
                if(SubmissiondataModel.Count > 0)
                {
                    GetContactList(SubmissiondataModel);
                }// end if
            }// End try
            catch (Exception ex)
            {
                _logger.Error("Exception occured while Fetching data from database.", ex);               
                throw;
            }// End catch
        }// end GenerateAlertsAndMailsForPendingInvoices

        /// <summary>
        /// This method is used to get contact information of users and will call SendMailForPendingInvoices method for sending mails
        /// </summary>
        /// <param name="listOfSubmissionData">list of submission data</param>
        private void GetContactList(List<SubmissionData> listOfSubmissionData)
        {
            // Make an object of emailaddress
            IEnumerable<string> emailAddress = null;
           
            // Create an object of submissiondata model
            List<SubmissionData> invoiceses = new List<SubmissionData>();

            try
            {
                // Fetches all distince member ids from lists
                IEnumerable<int> memberIds = listOfSubmissionData.Select(l => l.BillingMemberId).Distinct();

                // Iterate through all the member ids and generate alerts and email to it
                foreach (var memberId in memberIds)
                {

                    // Fetch list of contacts for members
                    var contactPax = memberManager.GetContactsForContactType(memberId,
                                                                             ProcessingContactType.
                                                                                 PAXOpenInvoicesContact);
                    var contactMisc = memberManager.GetContactsForContactType(memberId,
                                                                              ProcessingContactType.
                                                                                  MISCOpenInvoicesContact);
                    var contactCgo = memberManager.GetContactsForContactType(memberId,
                                                                             ProcessingContactType.
                                                                                 CGOOpenInvoicesContact);
                    var contactUatp = memberManager.GetContactsForContactType(memberId,
                                                                              ProcessingContactType.
                                                                                  UatpOpenInvoicesAlert);

                    
                   
                    // If contact type is not null then fetch email address of it
                    if (contactPax != null && contactPax.Count > 0)
                    {
                        int billingCategoryType = (int)BillingCategoryType.Pax;

                        // Get list of pending invoices for the given member id and categoryId pax
                        invoiceses =
                            listOfSubmissionData.Where(l => l.BillingMemberId == memberId).Where(
                                l => l.BillingCategoryId == billingCategoryType).ToList();

                        // If count of the invoices is greater than 0 then generate email addresses
                        if(invoiceses.Count > 0)
                        {
                            // Genertae an email address for the pax contacts
                            emailAddress = contactPax.Select(c => c.EmailAddress);
                            // Send mail for the pax invoices
                             SendMailForPendingInvoices(emailAddress, invoiceses, billingCategoryType);

                        }// End if
                        
                    }

                    if (contactMisc != null && contactMisc.Count > 0 )
                    {
                        int billingCategoryType = (int)BillingCategoryType.Misc;

                        // Get list of pending invoices for the given member id and categoryId pax
                        invoiceses =
                            listOfSubmissionData.Where(l => l.BillingMemberId == memberId).Where(
                                l => l.BillingCategoryId == billingCategoryType).ToList();

                        // If count of the invoices is greater than 0 then generate email addresses
                        if (invoiceses.Count > 0)
                        {
                            // Genertae an email address for the pax contacts
                            emailAddress = contactMisc.Select(c => c.EmailAddress);
                            // Send mail for the pax invoices
                            SendMailForPendingInvoices(emailAddress, invoiceses, billingCategoryType);

                        }// End if

                    }

                    if (contactCgo != null && contactCgo.Count > 0)
                    {
                        int billingCategoryType = (int)BillingCategoryType.Cgo;

                        // Get list of pending invoices for the given member id and categoryId pax
                        invoiceses =
                            listOfSubmissionData.Where(l => l.BillingMemberId == memberId).Where(
                                l => l.BillingCategoryId == billingCategoryType).ToList();

                        // If count of the invoices is greater than 0 then generate email addresses
                        if (invoiceses.Count > 0)
                        {
                            // Genertae an email address for the pax contacts
                            emailAddress = contactCgo.Select(c => c.EmailAddress);
                            // Send mail for the pax invoices
                            SendMailForPendingInvoices(emailAddress, invoiceses, billingCategoryType);

                        }// End if

                    }

                    if (contactUatp != null && contactUatp.Count > 0)
                    {
                        int billingCategoryType = (int)BillingCategoryType.Uatp;

                        // Get list of pending invoices for the given member id and categoryId pax
                        invoiceses =
                            listOfSubmissionData.Where(l => l.BillingMemberId == memberId).Where(
                                l => l.BillingCategoryId == billingCategoryType).ToList();

                        // If count of the invoices is greater than 0 then generate email addresses
                        if (invoiceses.Count > 0)
                        {
                            // Genertae an email address for the pax contacts
                            emailAddress = contactUatp.Select(c => c.EmailAddress);
                            // Send mail for the pax invoices
                           SendMailForPendingInvoices(emailAddress, invoiceses, billingCategoryType);

                        }// End if

                    }

                } // End foreach
            }// End try
            catch (Exception ex)
            {
                _logger.Error("Error occurred occured in MemberLocationUpdateHandler component.", ex);
                throw;
            }
        }// end GetContactList


        /// <summary>
        ///This method is used to send emails for the pending invoives to the email ids of member ids 
        /// </summary>
        /// <param name="toEmailList">List of email list</param>
        /// <param name="pendingInvoiceInfo">List of pending Invoices</param>
        public void SendMailForPendingInvoices(IEnumerable<string> toEmailList, List<SubmissionData> pendingInvoiceInfo , int billingCategoryType)
        {
            try
            {
                // get the current billing month , year and period
                BillingPeriod billingPeriod = CalendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);

                var dtf = CultureInfo.CurrentCulture.DateTimeFormat;
                string monthName = dtf.GetMonthName(billingPeriod.Month);
                string abbreviatedMonthName = dtf.GetAbbreviatedMonthName(billingPeriod.Month);

                // Concat month,year,period and build a string
                string billingMonth = abbreviatedMonthName + billingPeriod.Year.ToString("0000", CultureInfo.InvariantCulture) + "P" + billingPeriod.Period.ToString("0", CultureInfo.InvariantCulture);

                // Fetch the output file path
                var outputFilePath =
                    GetOutputFilePath(Convert.ToString(CalendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich)));//GetCurrentBillingPeriod().Period)));

                // Fetch billing file name
                var outputCsvFileName = GetOutputFileName(pendingInvoiceInfo[0].BillingNumericCode, billingMonth, billingCategoryType);

                // Combile path name and csv file name
                var outputCsvFilePath = Path.Combine(outputFilePath, outputCsvFileName);

                string invoiceNo;

            
                // Generate csv file for the pending invoices
                if(GenerateCSV(pendingInvoiceInfo, outputCsvFilePath, billingCategoryType, out invoiceNo))
                {
                    //get an object of the EmailSender component
                    var emailSender = Ioc.Resolve<IEmailSender>(typeof (IEmailSender));

                    //get an instance of email settings  repository
                    var emailSettingsRepository =
                        Ioc.Resolve<IRepository<EmailTemplate>>(typeof (IRepository<EmailTemplate>));

                    //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
                    var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof (ITemplatedTextGenerator));

                    

                    var timeStamp = abbreviatedMonthName + billingPeriod.Year.ToString("0000", CultureInfo.InvariantCulture) + "P" + billingPeriod.Period.ToString("0", CultureInfo.InvariantCulture);

                    var period = "P" + billingPeriod.Period.ToString("0", CultureInfo.InvariantCulture);
                    //object of the nVelocity data dictionary
                    var context = new VelocityContext();

                    context.Put("BillingCategory", Enum.GetName(typeof (BillingCategoryType), billingCategoryType));
                    context.Put("billingYear", billingPeriod.Year.ToString("0000", CultureInfo.InvariantCulture));
                    context.Put("billingMonth", abbreviatedMonthName);
                    context.Put("billingPeriod", period);
                    context.Put("SISOpsemailid", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

                    //Get the eMail settings for reacp sheet overview 
                    var emailSetting =
                        emailSettingsRepository.Get(es => es.Id == (int) EmailTemplateId.ISSubmissionDeadlineAlerts);

                    //generate email body text f
                    var body = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.ISSubmissionDeadlineAlerts,
                                                                            context);

                    //create a mail object to send mail
                    var overview = new MailMessage
                                       {From = new MailAddress(emailSetting.SingleOrDefault().FromEmailAddress)};
                    overview.IsBodyHtml = true;

                    foreach (var contact in toEmailList)
                    {
                        _logger.Debug("Sending mail to members Pending Invoices: " + contact);
                        overview.To.Add(new MailAddress(contact));
                    }

                    // Attache .csv file with email
                    overview.Attachments.Add(new Attachment(outputCsvFilePath));
                    //set subject of mail 
                    overview.Subject = emailSetting.SingleOrDefault().Subject.Replace("$billingperiod$",
                                                                                      timeStamp).Replace(
                                                                                          "$BillingCategory$",
                                                                                          Enum.GetName(
                                                                                              typeof (
                                                                                                  BillingCategoryType),
                                                                                              billingCategoryType));
                    //set body text of mail
                    overview.Body = body;

                    _logger.Debug("Sending mail to members Pending Invoices: " + body);

                    if (billingCategoryType == 1 || billingCategoryType == 2)
                    {
                        var messageRecipients = new ISMessageRecipients
                                                    {
                                                        MemberId = pendingInvoiceInfo[0].BillingMemberId,
                                                        ContactTypeId =
                                                            string.Format("{0}",
                                                                          billingCategoryType == 1
                                                                              ? (int)
                                                                                ProcessingContactType.
                                                                                    PAXOpenInvoicesContact
                                                                              : (int)
                                                                                ProcessingContactType.
                                                                                    CGOOpenInvoicesContact),
                                                        IsMessagesAlerts = new ISMessagesAlerts
                                                                               {
                                                                                   Message = "Open Invoice Alert",
                                                                                   StartDateTime = DateTime.UtcNow,
                                                                                   LastUpdatedOn = DateTime.UtcNow,
                                                                                   IsActive = true,
                                                                                   TypeId = (int) MessageType.Alert,
                                                                                   RAGIndicator =
                                                                                       (int) RAGIndicator.Green
                                                                               }
                                                    };

                        BroadcastMessages.AddAlerts(messageRecipients);
                    }

                    if (billingCategoryType == 3 || billingCategoryType == 4)
                    {
                        var messageRecipients = new ISMessageRecipients
                        {
                            MemberId = pendingInvoiceInfo[0].BillingMemberId,
                            ContactTypeId =
                                string.Format("{0}",
                                              billingCategoryType == 3
                                                  ? (int)
                                                    ProcessingContactType.MISCOpenInvoicesContact
                                                        
                                                  : (int)
                                                    ProcessingContactType.UatpOpenInvoicesAlert
                                                       ),
                            IsMessagesAlerts = new ISMessagesAlerts
                            {
                                Message = "Open Invoice Alert",
                                StartDateTime = DateTime.UtcNow,
                                LastUpdatedOn = DateTime.UtcNow,
                                IsActive = true,
                                TypeId = (int)MessageType.Alert,
                                RAGIndicator =
                                    (int)RAGIndicator.Green
                            }
                        };

                        BroadcastMessages.AddAlerts(messageRecipients);
                    }

                    //send the mail
                    emailSender.Send(overview);

                    //clear nvelocity context data
                    context = null;

                }

            }// End try
            catch (Exception exception)
            {
                _logger.Error("Error occured at sending emails to server", exception);
                throw;
            }// End catch

        }// End SendMailForPendingInvoices

        private string GetOutputFilePath(string currentBillingPeriod)
        {
            string csvPath;
            csvPath = Path.GetTempPath();
            var basePath = string.Format(@"{0}\{1}", csvPath, currentBillingPeriod);

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            return basePath;
        }

        /// <summary>
        /// This method is used to create file name
        /// </summary>
        /// <param name="billingNemericCode">Billing member numeric code</param>
        /// <param name="billingPeriod">billing period</param>
        /// <returns></returns>
        private string GetOutputFileName(string billingNemericCode , string billingPeriod , int billingCategoryType)
        {

            string billingCategory = string.Empty;

            switch(billingCategoryType)
            {
                case 1:
                    billingCategory = "P";
                    break;
                case 2:
                    billingCategory = "C";
                    break;
                case 3:
                    billingCategory = "M";
                    break;
                case 4:
                    billingCategory = "U";
                    break;
            }// End switch

            /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: No code change required, since billing member code numeric values are used as is for length more than 2.
            Ref: FRS Section 3.6 Table 24 Row 15 */

            int lengthOfBillingNumericCode = billingNemericCode.Length;

            switch (lengthOfBillingNumericCode)
            {
                case 1:
                    billingNemericCode = "00"
                                         + billingNemericCode;
                    break;
                case 2:
                    billingNemericCode = "0"
                                         + billingNemericCode;
                    break;
            }

            // Create output file prefix.
            var outputFilePrefix = string.Format(@"{0}-{1}-{2}-{3}", billingNemericCode,
                                                 "List of Invoices Pending Action ", billingCategory, billingPeriod);
            var outputCsvFileName = outputFilePrefix + ".CSV";

            return outputCsvFileName;
        }// end GetOutputFileName

        /// <summary>
        /// Create CSV for specified object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list of type T.</param>
        /// <param name="outputPath">The output path.</param>
        /// <param name="invoiceNo">The invoice no.</param>
        /// <returns></returns>
        public bool GenerateCSV<T>(List<T> list, string outputPath, int billingCategoryType,  out string invoiceNo)
        {
            invoiceNo = string.Empty;

            if (list.Count <= 0)
            {
                return false;
            }

            string billingCategory = Enum.GetName(typeof (BillingCategoryType), billingCategoryType);

            var sbCsvData = new StringBuilder();
            sbCsvData.Append("List of invoices with status 'Open' 'Ready for Submission' or 'Validation Error - WEB Invoice' for the Billing Category- " + billingCategory);
            sbCsvData.Append("\n");

            //Get all properties for Type T
            var propInfos = typeof(T).GetProperties();
            var displayPropInfos = new List<PropertyInfo>();

            //Write headers in CSV
            for (var propertyCount = 0; propertyCount <= propInfos.Length - 1; propertyCount++)
            {
                if (propInfos[propertyCount].GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().Count() <= 0)
                {
                    continue;
                }

                displayPropInfos.Add(propInfos[propertyCount]);

                var attribute = propInfos[propertyCount].GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().Single();
                var displayName = attribute.DisplayName;

                sbCsvData.Append(displayName.Trim());

                if (propertyCount < propInfos.Length - 1)
                {
                    sbCsvData.Append(",");
                }
            }
            sbCsvData.AppendLine();

            // Write headers data in CSV
            for (var propCollectionCount = 0; propCollectionCount <= list.Count - 1; propCollectionCount++)
            {
                T item = list[propCollectionCount];

                var propInfoCount = 0;
                foreach (var propInfo in displayPropInfos)
                {
                    var objectValue = item.GetType().GetProperty(propInfo.Name).GetValue(item, null);
                    if (objectValue != null)
                    {
                        var value = objectValue.ToString();

                        if (propInfo.Name.Equals("InvoiceNumber"))
                        {
                            invoiceNo = value;
                        }

                        if (objectValue.GetType().Equals(typeof(decimal)))
                        {
                            value = ((decimal)objectValue).ToString("00.000");
                        }

                        //Check if the value contains a comma and place it in quotes if so
                        if (value.Contains(","))
                        {
                            value = string.Concat("\"", value, "\"");
                        }

                        //Replace any \r or \n special characters from a new line with a space
                        if (value.Contains("\r"))
                        {
                            value = value.Replace("\r", " ");
                        }
                        if (value.Contains("\n"))
                        {
                            value = value.Replace("\n", " ");
                        }
                        sbCsvData.Append(value);
                    }

                    if (propInfoCount < propInfos.Length - 1)
                    {
                        sbCsvData.Append(",");
                    }

                    propInfoCount++;
                }

                sbCsvData.AppendLine();
            }

            if (string.IsNullOrEmpty(sbCsvData.ToString()))
            {
                return false;
            }

            System.IO.File.WriteAllText(outputPath, sbCsvData.ToString());

            return System.IO.File.Exists(outputPath);

        }


    }// end class ISSubmissionDeadlineHandler
}// End namespace

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using Castle.Core.Smtp;
using Iata.IS.Business.Common;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.MemberProfile.Impl;
using Iata.IS.Business.Pax;
using Iata.IS.Business.Pax.Impl;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.FutureSubmission;
using Iata.IS.Data.Impl;
using Iata.IS.Data.Pax;
using Iata.IS.Data.Pax.Impl;
using Iata.IS.Data.Reports;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.FutureSubmission;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.SubmissionDeadline;
using log4net;
using NVelocity;
using Iata.IS.Model.Base;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Data.MiscUatp;

namespace Iata.IS.Business.FutureSubmission.Impl
{
    /// <summary>
    ///loseFuture submission service run after 1 minute of current period.
    /// This will fetch all the invoices of the current period having status "Validate- future submition".
    /// All the invoices will be revalidated.
    /// </summary>
    public class CloseFututreSubmission : InvoiceManagerBase, ICloseFututreSubmission
    {
        #region Private Members

        // Logger property
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the CalendarManager
        /// </summary>
        public ICalendarManager CalendarManager { get; set; }

        /// <summary>
        /// Gets or sets the Future submitted invoices
        /// </summary>
        public List<FutureSubmissionInvoice> GetfutureSubmittedInvoices { get; set; }        

        /// <summary>
        /// Gets or sets the invoice repository.
        /// </summary>
        /// <value>The invoice repository.</value>
        public IInvoiceRepository InvoiceRepository { get; set; }

        /// <summary>
        /// Gets or sets the invoice repository.
        /// </summary>
        public IMiscInvoiceRepository MiscInvoiceRepository { get; set; }

        /// <summary>
        /// Member manager that will be injected by the container.
        /// </summary>
        public IMemberManager _MemberManager { get; set; }

        /// <summary>
        /// Gets or sets the validation error manager.
        /// </summary>
        /// <value>The validation error manager.</value>
        public IValidationErrorManager ValidationErrorManager { get; set; }

        /// <summary>
        /// Gets or sets the invoice manager Base
        /// </summary>
        //public InvoiceManagerBase InvoiceManagerBase { get; set; }

        /// <summary>
        /// Gets or sets the reference manager.
        /// </summary>
        /// <value>The reference manager.</value>
        public IReferenceManager ReferenceManager { get; set; }
        

        #endregion

        public IGetFutureStatusInvoice GetFutureStatusInvoices;

        public CloseFututreSubmission(IGetFutureStatusInvoice getFutureStatusInvoices, IValidationErrorManager validationErrorManager, IMemberManager memberManager, IInvoiceRepository invoiceRepository,IMiscInvoiceRepository miscInvoiceRepository, IReferenceManager referenceManager)
        {
           // InvoiceManagerBase = invoiceManagerBase;
            GetFutureStatusInvoices = getFutureStatusInvoices;
            InvoiceRepository = invoiceRepository;
            MiscInvoiceRepository = miscInvoiceRepository;
            _MemberManager = memberManager;
            ValidationErrorManager = validationErrorManager;
            ReferenceManager = referenceManager;
        }

        /// <summary>
        /// Validate the Future submitted invoices
        /// </summary>
        public void CloseFutureSubmissionOfInvoices()
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

                _logger.Debug(String.Format("From Filter- Year: {0}, Month: {1}, Period: {2}", billingPeriod.Month,
                                            billingPeriod.Year, billingPeriod.Period));

                _logger.Info(String.Format("From Filter- Year: {0}, Month: {1}, Period: {2}", billingPeriod.Month,
                                            billingPeriod.Year, billingPeriod.Period));

                // Get the invoices which is having status "Validated - future submitted" and of a current period
                GetfutureSubmittedInvoices = GetFutureStatusInvoices.GetFutureStatusInvoices(billingPeriod.Year,
                                                                                             billingPeriod.Month,
                                                                                             billingPeriod.Period);

                _logger.Info("Data fetched from the database");

                // If that invoices exist then validate it
                if (GetfutureSubmittedInvoices.Count > 0)
                {

                    _logger.Info("Validate submit invoices");

                    // Validate future submitted invoices
                    ValidateFutureSubmittedInvoice(GetfutureSubmittedInvoices);
                } // End if
            }
            catch (Exception exception)
            {
                _logger.Error("Error occured at the time of fetching data from database", exception);
                throw;
            }
        }// End CloseFutureSubmissionOfInvoices()


        /// <summary>
        /// Validate future submitted invoices when future period will be current period
        /// </summary>
        /// <param name="GetfutureSubmittedInvoices">List of future submitted invoices</param>
        public void ValidateFutureSubmittedInvoice(List<FutureSubmissionInvoice> GetfutureSubmittedInvoices)
        {
            try
            {
                if (GetfutureSubmittedInvoices.Count > 0)
                {
                    // Make an object of emailaddress
                    IEnumerable<string> emailAddress = null;

                    var emailErrorInvoice = new List<EmailFutureSubmittedInvoices>();

                    foreach (var getfutureSubmittedInvoice in GetfutureSubmittedInvoices)
                    {
                        // validation applied to Each invoices 
                        var invoices = FutureSubmittedInvoiceValidation(getfutureSubmittedInvoice.InvoiceId.ToString(),
                                                                       emailErrorInvoice);

                    } // End foreach


                    var billingMemberIdPax = emailErrorInvoice.Where(e=> e.BillingCategory == (int)BillingCategoryType.Pax).Select(l => l.BillingMemberId).Distinct();
                    var billingMemberIdMisc = emailErrorInvoice.Where(e=> e.BillingCategory == (int)BillingCategoryType.Misc).Select(l => l.BillingMemberId).Distinct();

                    foreach (int bmp in billingMemberIdPax)
                    {
                        List<EmailFutureSubmittedInvoices> invoiceInError =
                            emailErrorInvoice.Where(l => (l.BillingMemberId == bmp && l.BillingCategory == (int)BillingCategoryType.Pax)).Distinct().ToList();

                        if (invoiceInError.Count > 0)
                        {
                            // Fetch list of contacts for members
                            var contactPax = _MemberManager.GetContactsForContactType(bmp,ProcessingContactType.PAXValidationErrorAlert);

                            if (contactPax != null && contactPax.Count > 0)
                            {
                                // Genertae an email address for the pax contacts
                                emailAddress = contactPax.Select(c => c.EmailAddress);

                                _logger.Info("sending an email to the billing member fo Pax Invoice");


                                SendEmailToBillingMember(invoiceInError, emailAddress);

                            }
                        }
                    }
                    foreach (int bmm in billingMemberIdMisc)
                    {
                      List<EmailFutureSubmittedInvoices> invoiceInError =
                          emailErrorInvoice.Where(l => (l.BillingMemberId == bmm && l.BillingCategory == (int)BillingCategoryType.Misc)).Distinct().ToList();

                      if (invoiceInError.Count > 0)
                      {
                        // Fetch list of contacts for members
                        var contactMisc = _MemberManager.GetContactsForContactType(bmm,ProcessingContactType.MISCValidationErrorAlert);

                        if (contactMisc != null && contactMisc.Count > 0)
                        {
                          // Genertae an email address for the pax contacts
                          emailAddress = contactMisc.Select(c => c.EmailAddress);

                          _logger.Info("sending an email to the billing member of Misc Invoice");


                          SendEmailToBillingMember(invoiceInError, emailAddress);

                        }
                      }
                    }

                } // End if

            }
            catch (Exception exception)
            {
                _logger.Error("Error occured at the time of revalidation of invoices", exception);
                throw;
            }
        }// End ValidateFutureSubmittedInvoice()

        /// <summary>
        /// Validate Invoices 
        /// </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <returns>PaxInvoice</returns>
        public InvoiceBase FutureSubmittedInvoiceValidation(string invoiceId, List<EmailFutureSubmittedInvoices> emailErrorInvoice)
        {
            try
            {                
                // Create a list of webvalidation error
                var webValidationErrors = new List<WebValidationError>();

                //CMP626 :CMP-626-Future Submission for MISC and Provisional Settlement with ICH
                // fetch the whole Invoice Base object depends on the invoiceid 
                InvoiceBase invoice = InvoiceRepository.Single(id: new Guid(invoiceId)) ??
                          (InvoiceBase) MiscInvoiceRepository.Single(invoiceId: new Guid(invoiceId));

              // Re-fetch the billing and billed member - since we are getting a stale state (workaround)!
                var billingMember = _MemberManager.GetMember(invoice.BillingMemberId);
                var billedMember = _MemberManager.GetMember(invoice.BilledMemberId);

                // Get Final Parent Details for SMI, Currency, Clearing House abd Suspended Flag validations
                var billingFinalParent = _MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BillingMemberId));
                var billedFinalParent = _MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BilledMemberId));

                // Get ValidationErrors for invoice from DB.
                var validationErrorsInDb = ValidationErrorManager.GetValidationErrors(invoiceId);


                // IS Membership validations.
                // Check for the billed memberStatus
                if (!FutureSubmissionValidateBilledMemberStatus(invoice.BilledMember))
                {
                    webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoice.Id,
                                                                                         ErrorCodes.FutureSubmissionBilledMember
                                                                                             ));
                } // End if

                // Check for the billing member stattus
                if (!FutureSubValidateBillingMembershipStatus(billingMember))
                {
                    webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoice.Id,
                                                                                         ErrorCodes.FutureSubmissionBillingMember
                                                                                             ));
                } // End if


                // Ds Validation
                bool isDsValidation = FutureSubmissionDsValidation(billingMember, invoice);

                // Check for the ds valisation status
                if (!isDsValidation)
                {
                    webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoice.Id,
                                                                                         ErrorCodes.FutureSubmissionDs
                                                                                             ));
                } // End if
                    // If ds validation successfull then updtae ds
                else
                {

                    var invoiceManager = Ioc.Resolve<IInvoiceManager>();
                    // Updtae Ds
                    invoiceManager.UpdateInvoiceDetailsForLateSubmission(invoice, billingMember, billedMember);
                } // End if
                
                // Validate SMI
                // Validate SMI if it is "I" then Validate billing currency
                //CMP #624: ICH Rewrite-New SMI X : X and Non X SMI validate seperately
                if (invoice.SettlementMethodId == (int)SMI.IchSpecialAgreement ? !ValidateSettlementMethodX(invoice, billingFinalParent, billedFinalParent) : !ValidateSettlementMethod(invoice, billingFinalParent, billedFinalParent))
                {
                    webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoice.Id,
                                                                                         ErrorCodes.FutureSubmissionSMI
                                                                                             ));
                } // End if
                else
                {
                    // Check for the SMI; if "I" then validate billing currency
                    if (invoice.InvoiceSmi == SMI.Ich)
                    {
                      if (!ValidateBillingCurrency(invoice, billingFinalParent, billedFinalParent))
                        {
                            webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoice.Id,
                                                                                                 ErrorCodes.FutureSubmissionBillingCurrency
                                                                                                     ));
                        }
                    }
                    //CMP626 :CMP-626-Future Submission for MISC and Provisional Settlement with ICH
                    else if (invoice.InvoiceSmi == SMI.IchSpecialAgreement)
                    {
                      try
                      {
                        var exceptionDetailsList = new List<IsValidationExceptionDetail>();
                        
                        if (invoice.BillingCategoryId == (int)BillingCategoryType.Misc) // If billing catogery Misc Get Linking Detail also
                        {
                          //Get Linked original Invoice
                          MiscUatpInvoice linkedRm1Invoice;
                          MiscUatpInvoice linkedRm2Invoice;
                          MiscCorrespondence linkedCorrespondence;
                          var originalInvoice = GetLinkedMUOriginalInvoice((MiscUatpInvoice)invoice, out linkedRm1Invoice, out linkedRm2Invoice, out linkedCorrespondence);
                          //Call here IchWebService to verify SMI X
                          if (!CallSmiXIchWebServiceAndHandleResponse(invoice, exceptionDetailsList, ((MiscUatpInvoice)invoice).InvoiceTypeId, DateTime.UtcNow, string.Empty, false, linkedInvoice: originalInvoice))
                          {
                            //The first occurrence of ‘ErrorDescription’ from the Response Message from ICH will be populated in column ‘Error Description’ of the email
                            webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoice.Id, string.Empty, exceptionDetailsList.FirstOrDefault().ErrorDescription));
                          }
                        }
                        else
                        {
                          //Call here IchWebService to verify SMI X
                          if (!CallSmiXIchWebServiceAndHandleResponse(invoice, exceptionDetailsList, ((PaxInvoice)invoice).InvoiceTypeId, DateTime.UtcNow, string.Empty, false))
                          {
                            //The first occurrence of ‘ErrorDescription’ from the Response Message from ICH will be populated in column ‘Error Description’ of the email
                            webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoice.Id, string.Empty, exceptionDetailsList.FirstOrDefault().ErrorDescription));
                          }
                        }
                      }
                      catch (Exception ex)
                      {
                        webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoice.Id, ErrorCodes.FutureSubmissionSMI, "Invalid Settelment Method"));
                      }
                    }// End if
                } // End else

                // Validate Location
                // First check whether that location is exists in db or not; if exist then update it
                if (FutureSubmissionValidateLocationInDb(invoice))
                {
                    // Adds member location information.
                    UpdateMemberLocationInformation(invoice);
                } // End if
                else
                {
                    webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoice.Id,
                                                                                         ErrorCodes.FutureSubmissionLocation
                                                                                             ));
                } // End else

                // Update invoice status in case of error.
                if (webValidationErrors.Count > 0)
                {
                    if (invoice.InvoiceStatus == InvoiceStatusType.ErrorCorrectable)
                    {
                        int isInvoiceDeleted = GetFutureStatusInvoices.DeleteErrorCorrectableInvoices(invoice.Id);
                    }

                    invoice.InvoiceStatus = InvoiceStatusType.ErrorNonCorrectable;
                    invoice.ValidationErrors.AddRange(webValidationErrors);

                    invoice.ValidationStatus = InvoiceValidationStatus.Failed;

                    emailErrorInvoice.Add(new EmailFutureSubmittedInvoices
                                              {
                                                  BillingYear = invoice.BillingYear,
                                                  BillingMonth = invoice.BillingMonth,
                                                  PeriodNo = invoice.BillingPeriod,
                                                  BilledMemberCode = invoice.BilledMember.MemberCodeAlpha + "-" + invoice.BilledMember.MemberCodeNumeric,
                                                  BillingCategory = invoice.BillingCategoryId,
                                                  InvoiceNo = invoice.InvoiceNumber,
                                                  ErrorDesc = invoice.ValidationErrors,
                                                  BillingMemberCode = invoice.BillingMemberText,
                                                  BillingMemberId = invoice.BillingMemberId
                                              });
                } // End if
                else
                {
                    // If Billed or Billing member is suspended update Invoice suspended flag to true. 
                  if (ValidateSuspendedFlag(invoice, billingFinalParent, billedFinalParent))
                    {
                        invoice.SuspendedInvoiceFlag = true;
                    }
                    // Every validation is successful. Update invoice status as Ready for billing and invoice date as current date.
                    //var isInvoiceUpdated = GetFutureStatusInvoices.UpdateFutureSubmittedInvoice(invoice.Id);

                    if (invoice.InvoiceStatus != InvoiceStatusType.ErrorCorrectable)
                    {
                      invoice.InvoiceStatusId = (int) InvoiceStatusType.ReadyForBilling;
                      invoice.InvoiceStatus = InvoiceStatusType.ReadyForBilling;
                      invoice.ValidationStatus = InvoiceValidationStatus.Completed;
                      invoice.ValidationStatusId = (int) InvoiceValidationStatus.Completed;
                      // SCP340881 - SRM: Field not updated in dashboard download
                      // Update validation status date when marked 'Ready for billing'
                      invoice.ValidationDate = DateTime.UtcNow;
                    }
                }

                // Update validation errors in db.
                ValidationErrorManager.UpdateValidationErrors(invoice.Id, invoice.ValidationErrors, validationErrorsInDb);

                //CMP626 :CMP-626-Future Submission for MISC and Provisional Settlement with ICH
                InvoiceBase updatedInvoice = null;
                // Update invoice to database.
                switch (invoice.BillingCategoryId)
                {
                  case (int)BillingCategoryType.Pax:
                    // Update Duplicate Coupon DU Mark
                    InvoiceRepository.UpdateDuplicateCouponByInvoiceId(invoice.Id, invoice.BillingMemberId);
                    updatedInvoice = InvoiceRepository.Update((PaxInvoice)invoice);
                    break;
                  case (int)BillingCategoryType.Misc:
                    updatedInvoice = MiscInvoiceRepository.Update((MiscUatpInvoice)invoice);
                    break;
                }
                

                UnitOfWork.CommitDefault();

                if (updatedInvoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
                {
                    //_logger.InfoFormat("Setting LA parameter for invoice '{0}' after Invoice status changed as Ready for billing.", ConvertUtil.ConvertGuidToString(updatedInvoice.Id));
                    
                    //InvoiceRepository.UpdateInvoiceAndSetLaParameters(new Guid(invoiceId));
                    
                    //_logger.InfoFormat("Completed settting LA parameter for invoice '{0}' after Invoice status changed as Ready for billing.", ConvertUtil.ConvertGuidToString(updatedInvoice.Id));
                    
                    _logger.InfoFormat("Setting DS,BVC,Settlement,Suspension and other parameters for invoice '{0}' after Invoice status changed as Ready for billing.", ConvertUtil.ConvertGuidToString(updatedInvoice.Id));
                    
                    InvoiceRepository.UpdateInvoiceOnReadyForBilling(updatedInvoice.Id, updatedInvoice.BillingCategoryId, updatedInvoice.BillingMemberId, updatedInvoice.BilledMemberId, updatedInvoice.BillingCode);

                    _logger.InfoFormat("Completed setting DS,BVC,Settlement,Suspension and other parameters for invoice '{0}' after Invoice status changed as Ready for billing.", ConvertUtil.ConvertGuidToString(updatedInvoice.Id));

                    //SCP 152109: as discussed
                    //Desc: False alert was generated for correspondence to raise BM even when BM was already raised. 
                    //Problem identified to be because of future invoices not calling the SP to close the respective correspondences when marked RFB by the Job.
                    //Date: 26-July-2013
                    _logger.InfoFormat("Starting to Close relevant correspondence within invoice '{0}' after Invoice status changed as Ready for billing.", ConvertUtil.ConvertGuidToString(updatedInvoice.Id));

                    InvoiceRepository.CloseCorrespondenceOnInvoiceReadyForBilling(updatedInvoice.Id, updatedInvoice.BillingCategoryId);

                    _logger.InfoFormat("Completed Closing relevant correspondence within invoice '{0}' after Invoice status changed as Ready for billing.", ConvertUtil.ConvertGuidToString(updatedInvoice.Id));

                }

              _logger.InfoFormat("Setting expiry for invoice:{0}", invoice.InvoiceNumber);
              //SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
                // Update the expiry period of transactions for purging purpose.
              //var processingDashboardInvoiceStatusRepository = Ioc.Resolve<IProcessingDashboardInvoiceStatusRepository>(typeof (IProcessingDashboardInvoiceStatusRepository));
              //processingDashboardInvoiceStatusRepository.UpdatePurgingExpiryPeriod(invoice.Id);
              //_logger.Info("Completed setting expiry date.");
                return invoice;
            }
            catch (Exception exception)
            {
                _logger.Error("Error occured at the time of revalidation of invoices" + exception);
                throw;
            }
        }

        /// <summary>
        /// Validate Location; Check whether that Location exists in db or not
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        protected bool FutureSubmissionValidateLocationInDb(InvoiceBase invoice)
        {
            if (!string.IsNullOrWhiteSpace(invoice.BillingMemberLocationCode))
            {
                var locatioonCode =
                    LocationRepository.Single(
                        ml =>
                        ml.MemberId == invoice.BillingMemberId &&
                        ml.LocationCode.ToUpper() == invoice.BillingMemberLocationCode.ToUpper());
                if (locatioonCode != null && locatioonCode.IsActive)
                {
                    return true;
                }
                return false;
            }
            return true;
        }// End FutureSubmissionValidateLocationInDb()

        /// <summary>
        /// Validates the billed IS Membership status.
        /// Validation of the Billed Member will fail if the IS Membership Status of the Billed Member is ‘Terminated’ and 'Pending'.This will be a non-correctable error.
        /// </summary>
        /// <param name="billedMember">The billed member.</param>
        protected virtual bool FutureSubmissionValidateBilledMemberStatus(Member billedMember)
        {
            if (billedMember.IsMembershipStatus == MemberStatus.Terminated || billedMember.IsMembershipStatus == MemberStatus.Pending)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Validates the billing IS Membership status.
        /// A member can only  create an invoice/creditNote or Form C when his IS Membership is `Active’.
        /// </summary>
        /// <param name="billingMember">The billing member.</param>
        protected virtual bool FutureSubValidateBillingMembershipStatus(Member billingMember)
        {
            if (billingMember.IsMembershipStatus == MemberStatus.Active)
            {
                return true;
            }

            return false;
        }// End FutureSubValidateBillingMembershipStatus()

        /// <summary>
        /// Check for the ds validation
        /// </summary>
        /// <param name="billingMember"></param>
        /// <param name="invoice"></param>
        /// <returns></returns>
        protected bool FutureSubmissionDsValidation(Member billingMember, InvoiceBase invoice)
        {
            if (billingMember != null && !billingMember.DigitalSignApplication
             && invoice.DigitalSignatureRequired == DigitalSignatureRequired.Yes)
            {
                return false;
            }

            return true;
        }// End FutureSubmissionDsValidation()

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceInError"></param>
        /// <returns></returns>
        protected void SendEmailToBillingMember(List<EmailFutureSubmittedInvoices> invoiceInError, IEnumerable<string> toEmailList)
        {
            try
            {
                foreach (var emailFutureSubmittedInvoicese in invoiceInError)
                {
                    DateTime date = new DateTime(1900, emailFutureSubmittedInvoicese.BillingMonth, 1);

                    emailFutureSubmittedInvoicese.BillingMonYearPeriod = date.ToString("MMM") + " " +
                                                                         emailFutureSubmittedInvoicese.BillingYear + "-" + "P" +
                                                                         emailFutureSubmittedInvoicese.PeriodNo;




                    emailFutureSubmittedInvoicese.BillingCategoryName = Enum.GetName(typeof (BillingCategoryType),
                                                                                     emailFutureSubmittedInvoicese.
                                                                                         BillingCategory);

                    foreach (var VARIABLE in emailFutureSubmittedInvoicese.ErrorDesc)
                    {

                        emailFutureSubmittedInvoicese.DetailErrorDesc = emailFutureSubmittedInvoicese.DetailErrorDesc +
                                                                        VARIABLE.ErrorDescription;
                        emailFutureSubmittedInvoicese.DetailErrorDesc = emailFutureSubmittedInvoicese.DetailErrorDesc +
                                                                        ",";
                    }

                    int index = emailFutureSubmittedInvoicese.DetailErrorDesc.LastIndexOf(',');
                    emailFutureSubmittedInvoicese.DetailErrorDesc =  emailFutureSubmittedInvoicese.DetailErrorDesc.Substring(0, index);

                }


                //get an object of the EmailSender component
                var emailSender = Ioc.Resolve<IEmailSender>(typeof (IEmailSender));

                //get an instance of email settings  repository
                var emailSettingsRepository =
                    Ioc.Resolve<IRepository<EmailTemplate>>(typeof (IRepository<EmailTemplate>));

                //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
                var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof (ITemplatedTextGenerator));

                //object of the nVelocity data dictionary
                var context = new VelocityContext();

                context.Put("invoiceInError", invoiceInError);


                //Get the eMail settings for reacp sheet overview 
                var emailSetting =
                    emailSettingsRepository.Get(es => es.Id == (int) EmailTemplateId.FutureSubmittedInvoiceInError);

                //generate email body text f
                var body = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.FutureSubmittedInvoiceInError,
                                                                        context);

                //create a mail object to send mail
                var overview = new MailMessage {From = new MailAddress(emailSetting.SingleOrDefault().FromEmailAddress)};
                overview.IsBodyHtml = true;

                foreach (var contact in toEmailList)
                {
                    _logger.Debug("Sending mail to members Pending Invoices: " + contact);
                    overview.To.Add(new MailAddress(contact));
                }

                //set subject of mail 
                overview.Subject = emailSetting.SingleOrDefault().Subject;
                //set body text of mail
                overview.Body = body;


                _logger.Debug("Sending mail to members Pending Invoices: " + body);

                //send the mail
                emailSender.Send(overview);

                //clear nvelocity context data
                context = null;
            }
            catch (Exception exception)
            {
                _logger.Error("Error occured at sending emails to server", exception);
                throw;
            }
        }// End SendEmailToBillingMember

    }

}

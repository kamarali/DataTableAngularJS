using System;
using System.Linq;
using System.Net.Mail;
using Iata.IS.AdminSystem;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data.Pax.Impl;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using log4net;
using System.Reflection;
using Iata.IS.Model.Calendar;
using Iata.IS.Business.Common;
using Iata.IS.Data.AutoBillingClosure;
using System.Collections.Generic;
using Iata.IS.Model.AutoBillingClosure;
using Iata.IS.Data.Pax;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Model.Pax;
using Iata.IS.Data.Impl;
using Iata.IS.Core.DI;
using Castle.Core.Smtp;
using Iata.IS.Model.Common;
using Iata.IS.Data;
using NVelocity;
using System.Globalization;


namespace Iata.IS.Business.AutoBillingClosure.Impl
{
    public class AutoBillingInvoicesClosure : InvoiceManagerBase, IAutoBillingInvoicesClosure
    {
        #region Private Members

        // Logger property
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // For updating IS Validation Flag
        //private const string TimeLimitFlag = "TL";
        private const string DuplicateValidationFlag = "DU";
        private const string InvoiceDateFormat = "yyyy-MM-dd";

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the CalendarManager
        /// </summary>
        public new ICalendarManager CalendarManager { get; set; }

        public IGetAutoBillingInovices GetAutoBillingInvoices { get; set; }

        public List<AutoBillingInvoice> GetAutoBillingInvoiceList { get; set; }

        /// <summary>
        /// Gets or sets the invoice repository.
        /// </summary>
        /// <value>The invoice repository.</value>
        public new InvoiceRepository InvoiceRepository = new InvoiceRepository();


        /// <summary>
        /// Member manager that will be injected by the container.
        /// </summary>
        public new IMemberManager MemberManager { get; set; }


        /// <summary>
        /// Coupon Record Repository to perform duplicate check on coupn records.
        /// </summary>
        public ICouponRecordRepository CouponRecordRepository { get; set; }



        #endregion

        public void CloseAutoBillingInvoices()
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

                _logger.Info(String.Format("From Filter- Year: {0}, Month: {1}, Period: {2}", billingPeriod.Year,
                                            billingPeriod.Month, billingPeriod.Period));

                // Get the invoices which is having status "Validated - auto billing" and of a current period
                GetAutoBillingInvoiceList = GetAutoBillingInvoices.GetAutoBillingOpenInvoice(billingPeriod.Year,
                                                                                             billingPeriod.Month,
                                                                                             billingPeriod.Period);

                _logger.Info("Data fetched from the database");

                // If that invoices exist then validate it
                if (GetAutoBillingInvoiceList.Count > 0)
                {
                    // Process Each invoice for AutoBillingClosure
                    foreach (var autoBillingInvoice in GetAutoBillingInvoiceList)
                    {
                        
                          // if coupan data exists then
                          // get invoice data for processing coupon data
                            //SCP178233 - AB invoices not submitted
                            var invoice = InvoiceRepository.GetInvoiceCouponsData(new Guid(autoBillingInvoice.InvoiceId.ToString()));

                          if (invoice != null)
                          {
                            // Re-fetch the billing and billed member - since we are getting a stale state (workaround)!
                            var billingMember = MemberManager.GetMember(invoice.BillingMemberId);
                            var billedMember = MemberManager.GetMember(invoice.BilledMemberId);

                            // Check if invoice can be closed/finalized if yes then perform membership checks

                            // if IS Membership status of Billing member is not active 
                            if (billingMember.IsMembershipStatus != MemberStatus.Active)
                            {
                              // if true the  goto FLOW2
                              ProcessFlowTwo(invoice, billingMember, billedMember);
                            }
                            else if (billedMember.IsMembershipStatus == MemberStatus.Terminated ||
                                     billedMember.IsMembershipStatus == MemberStatus.Pending)
                            {
                              // if true the  goto FLOW2
                              ProcessFlowTwo(invoice, billingMember, billedMember);
                            }
                              // check if at least one coupon's data exists for invoice
                            else if (invoice.CouponDataRecord.Count == 0)
                            {
                              // if no coupan data exists then process FLOW3
                              ProcessFlowThree(invoice);
                            }
                            else
                            {
                              // Get Final Parent Details for SMI, Currency, Clearing House abd Suspended Flag validations
                              var billingFinalParent =
                                MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BillingMemberId));
                              var billedFinalParent =
                                MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BilledMemberId));

                              // Blocking Rule
                              #region Old Code - Commented
                              //var smiValue = string.Empty;
                              //var achZoneId = 3;

                              //switch (smiForBlockingRule)
                              //{
                              //    case (int)SMI.AchUsingIataRules:
                              //    case (int)SMI.Ach:
                              //        smiValue = "ACH";
                              //        BlockingRulesRepository.ValidateBlockingRules(invoice.BillingMemberId,
                              //                                                      invoice.BilledMemberId,
                              //                                                      invoice.BillingCategory, smiValue, achZoneId,
                              //                                                      achZoneId,
                              //                                                      out isCreditorBlocked, out isDebitorBlocked,
                              //                                                      out isCGrpBlocked, out isDGrpBlocked);
                              //        break;
                              //    case (int)SMI.Ich:
                              //        smiValue = "ICH";
                              //        BlockingRulesRepository.ValidateBlockingRules(invoice.BillingMemberId,
                              //                                                      invoice.BilledMemberId,
                              //                                                      invoice.BillingCategory, smiValue,
                              //                                                      invoice.BillingMember.IchConfiguration != null
                              //                                                          ? invoice.BillingMember.IchConfiguration.IchZoneId
                              //                                                          : 0,
                              //                                                      invoice.BilledMember.IchConfiguration != null
                              //                                                          ? invoice.BilledMember.IchConfiguration.IchZoneId
                              //                                                          : 0,
                              //                                                      out isCreditorBlocked, out isDebitorBlocked,
                              //                                                      out isCGrpBlocked, out isDGrpBlocked);
                              //        break;
                              //}
                            #endregion

                              var isCreditorBlocked = false;
                              var isDebitorBlocked = false;
                              var isCGrpBlocked = false;
                              var isDGrpBlocked = false;
                              var smiForBlockingRule = GetSettlementMethodForAutoBillingInvoice(invoice, invoice.BillingMember, invoice.BilledMember);
                              //SCP164383: Blocking Rule Failed
                              //Desc: Hooking a call to centralized code for blocking rule validation
                              ValidationForBlockedAirline(invoice.BillingMemberId, invoice.BilledMemberId, (SMI)smiForBlockingRule,
                                                        invoice.BillingCategory, out isCreditorBlocked, out isDebitorBlocked,
                                                        out isCGrpBlocked, out isDGrpBlocked, true);

                              // Blocked by Creditor/Debtor/Group Rule
                              if (isCreditorBlocked || isDebitorBlocked || isCGrpBlocked || isDGrpBlocked)
                              {
                                  //Settelment Method
                                  invoice.SettlementMethodId = (int)SMI.Bilateral;
                                  invoice.BillingCurrencyId = (int)BillingCurrency.USD;
                                  //SCP90118: KAL: Autobilling - blocking rules are not being referred to
                                  invoice.ClearingHouse = string.Empty;
                              }
                              else
                              {
                                //SCP90118: KAL: Autobilling - blocking rules are not being referred to
                                invoice.ClearingHouse = ReferenceManager.GetClearingHouseForInvoice(invoice, billingFinalParent, billedFinalParent);
                              }

                              //CMP-409 : Re-validate SMI and Listing Currency
                              // SCP177435 - EXCHANGE RATE 
                              if (!ValidateSettlementMethod(invoice, billingFinalParent, billedFinalParent) ||
                                  !ValidateBillingCurrency(invoice, billingFinalParent, billedFinalParent) ||
                                  !ValidateListingCurrency(invoice, billingFinalParent, billedFinalParent))
                              {
                                ProcessFlowTwo(invoice, billingMember, billedMember, true);
                              }
                              else
                              {
                                // Perform dupicte check on Coupon records and update 'IS Validation Flag' accordingly. Refer UC-G3320 Annexure 1.2
                                foreach (var primeCoupon in invoice.CouponDataRecord)
                                {
                                  PerformDuplicateCheck(primeCoupon, invoice);
                                }


                                // Update Invoice Date with YMQ format of SYSDATE
                                DateTime invoiceDate;
                                var cst =
                                  TimeZoneInfo.FindSystemTimeZoneById(
                                    SystemParameters.Instance.CalendarParameters.YmqTimeZoneName);

                                if (
                                  DateTime.TryParseExact(
                                    TimeZoneInfo.ConvertTimeFromUtc(invoice.InvoiceDate, cst).ToString(),
                                    InvoiceDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                                    out invoiceDate))
                                  invoice.InvoiceDate = invoiceDate;

                                // Update Invoice Status to 'Ready for Billing'
                                invoice.InvoiceStatusId = (int) InvoiceStatusType.ReadyForBilling;
                                invoice.InvoiceStatus = InvoiceStatusType.ReadyForBilling;
                                invoice.ValidationStatus = InvoiceValidationStatus.Completed;
                                invoice.ValidationStatusId = (int) InvoiceValidationStatus.Completed;

                                // Set Degital Sign info
                                SetDigitalSignatureInfo(invoice, billingMember, billedMember);

                                // Update suspended flag according to ach/Ach configuration.
                                if (ValidateSuspendedFlag(invoice, billingFinalParent, billedFinalParent))
                                {
                                  invoice.SuspendedInvoiceFlag = true;
                                }

                                // Set Sponsored By  
                                var ichConfiguration = billingMember.IchConfiguration;
                                if (ichConfiguration != null && ichConfiguration.SponsoredById.HasValue)
                                {
                                  invoice.SponsoredById = ichConfiguration.SponsoredById;
                                }

                                invoice.SupportingAttachmentStatus = SupportingAttachmentStatus.NotProcessed;
                              }
                            }
                            
                            // Update Invoice
                            InvoiceRepository.Update(invoice);
                            // Post changes to database
                            UnitOfWork.CommitDefault();

                            //SCP85837: PAX CGO Sequence Number
                            InvoiceRepository.UpdateTransSeqNoWithInBatch(new Guid(autoBillingInvoice.InvoiceId.ToString()));

                            // Update Invoice level flags. Refer  G3320-5.1 and G3320-8
                            InvoiceRepository.UpdateInvoiceOnReadyForBilling(
                              new Guid(autoBillingInvoice.InvoiceId.ToString()),
                              invoice.BillingCategoryId,
                              invoice.BillingMemberId,
                              invoice.BilledMemberId,
                              invoice.BillingCode);

                          }
                    }
                } 
            }
            catch (Exception exception)
            {
                _logger.Error("Error occured at the time of fetching data from database", exception);
                throw;
            }
        }

      /// <summary>
      /// Perform IS membership status validations checks on Auto Billing Invoices
      /// </summary>
      /// <param name="invoice"></param>
      /// <param name="billingMember"></param>
      /// <param name="billedMember"></param>
      /// <param name="isErrorInReValidation"></param>
      public void ProcessFlowTwo(PaxInvoice invoice, Member billingMember, Member billedMember,  bool  isErrorInReValidation = false)
        {
            try
            {
                // Make an object of emailaddress

                var emailErrorInvoice = new EmailAutoBillingInvoices();

                // update invoice status as 'Error - Non Correctable'    

                invoice.InvoiceStatus = InvoiceStatusType.ErrorNonCorrectable;
                invoice.InvoiceStatusId = (int)InvoiceStatusType.ErrorNonCorrectable;
                invoice.ValidationStatus = InvoiceValidationStatus.Failed;
                invoice.ValidationStatusId = (int)InvoiceValidationStatus.Failed;
                // Update LastUpdatedOn field
                invoice.LastUpdatedOn = DateTime.UtcNow;
                InvoiceRepository.Update(invoice);

                // Format Email 
                emailErrorInvoice.InvoiceNo = invoice.InvoiceNumber;
                emailErrorInvoice.PeriodNo = "P" + invoice.BillingPeriod;
                // Get month name in MMM format
                var date = new DateTime(1900, invoice.BillingMonth, 1);
                emailErrorInvoice.BillingMonth = date.ToString("MMM");
                emailErrorInvoice.BillingYear = invoice.BillingYear;
                emailErrorInvoice.BillingMemberId = invoice.BillingMemberId;
                emailErrorInvoice.BillingMemberAlphaCode = billingMember.MemberCodeAlpha;
                emailErrorInvoice.BillingMemberNumericCode = billingMember.MemberCodeNumeric;
                emailErrorInvoice.BillingMemberIsMembershipStatus = billingMember.IsMembershipStatus;
                emailErrorInvoice.BilledMemberAlphaCode = billedMember.MemberCodeAlpha;
                emailErrorInvoice.BilledMemberNumericCode = billedMember.MemberCodeNumeric;
                emailErrorInvoice.BilledMemberIsMembershipStatus = billedMember.IsMembershipStatus;


                // Fetch list of contacts for members
                var contactPax = MemberManager.GetContactsForContactType(emailErrorInvoice.BillingMemberId, ProcessingContactType.AutoBillingValueDeterminationAlerts);

                if (contactPax != null && contactPax.Count > 0)
                {
                    // Genertae an email address for the pax contacts
                    IEnumerable<string> emailAddress = contactPax.Select(c => c.EmailAddress);

                    // Send email 
                    SendEmailToBillingMember(emailErrorInvoice, emailAddress, isErrorInReValidation);
                  
                  _logger.Info("sending an email to the billing member");

                }
            }
            catch (Exception exception)
            {
                _logger.Error("Error occured at the time of IS membership status validations checks", exception);
                throw;
            }
        }

        public void ProcessFlowThree(PaxInvoice invoice)
        {
            try
            {
                // Update AutoBilling Coupon Record table 
                // For all coupons with current Invoice's Invoice I, update Invoice Id = null 
                //GetAutoBillingInvoices.UpdateAutoBillingCoupons(invoice.Id);
                // Delete the invoice
                GetAutoBillingInvoices.DeleteAutoBillingInvoice(invoice.Id);
                //InvoiceRepository.Delete(invoice);
                //UnitOfWork.CommitDefault();
            }
            catch (Exception exception)
            {
                _logger.Error("Error occured at the time of IS membership status validations checks", exception);
                throw;
            }
        }


        /// <summary>
        /// Perform Duplicate coupon check for each coupon
        /// </summary>
        /// <param name="primeCoupon"></param>
        /// <param name="invoice"></param>
        public void PerformDuplicateCheck(PrimeCoupon primeCoupon, PaxInvoice invoice)
        {
            try
            {
                DateTime billingDate;
                var billingYearToCompare = 0;
                var billingMonthToCompare = 0;
                string duplicateCouponErrorMessage = string.Empty;

                if (DateTime.TryParse(string.Format("{0}/{1}/{2}", invoice.BillingYear.ToString().PadLeft(2, '0'), invoice.BillingMonth.ToString().PadLeft(2, '0'), "01"), out billingDate))
                {
                    var billingDateToCompare = billingDate.AddMonths(-12);
                    billingYearToCompare = billingDateToCompare.Year;
                    billingMonthToCompare = billingDateToCompare.Month;
                }

                // Validate duplicate coupon - combination Ticket/FIM number, issuing airline, coupon number exist in other invoice created in last 12 months
                // has same billed member for current coupon. Refer - G3320-1.2
                var duplicateCouponCount = CouponRecordRepository.GetCouponRecordDuplicateCount(primeCoupon.TicketOrFimCouponNumber,
                                                                                                     primeCoupon.TicketDocOrFimNumber,
                                                                                                     primeCoupon.TicketOrFimIssuingAirline,
                                                                                                     invoice.BillingMemberId,
                                                                                                     invoice.BilledMemberId,
                                                                                                     billingYearToCompare,
                                                                                                     billingMonthToCompare, primeCoupon.SourceCodeId, invoice.Id);

                if (duplicateCouponCount > 0)
                {
                    duplicateCouponErrorMessage = string.Format(Messages.PrimeCouponDuplicateMessage, duplicateCouponCount);
                }
                if (!string.IsNullOrEmpty(duplicateCouponErrorMessage))
                {
                    primeCoupon.ISValidationFlag = DuplicateValidationFlag;
                }

                // Save Updated Coupon Record to database
                CouponRecordRepository.Update(primeCoupon);
            }
            catch (Exception exception)
            {
                _logger.Error("Error occured at the time of IS membership status validations checks", exception);
                throw;
            }
        }


      /// <summary>
      /// Sending mail to billing members of invoices with Invalid IS Membership Status:
      /// </summary>
      /// <param name="invoiceInError"></param>
      /// <param name="toEmailList"></param>
      /// <param name="isErrorInReValidation"></param>
      protected void SendEmailToBillingMember(EmailAutoBillingInvoices invoiceInError, IEnumerable<string> toEmailList, bool isErrorInReValidation)
        {
            try
            {
                //get an object of the EmailSender component
                var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

                //get an instance of email settings  repository
                var emailSettingsRepository =
                    Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

                //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
                var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));

                //object of the nVelocity data dictionary
                var context = new VelocityContext();

                context.Put("invoiceInError", invoiceInError);


              if(isErrorInReValidation)
              {
                //Get the eMail settings for auto billing invoice closure error 
                var emailSetting =
                    emailSettingsRepository.Get(es => es.Id == (int)EmailTemplateId.AutoBillingInvoiceClosureValidationError);

                invoiceInError.SisOpsEmailId = emailSetting.SingleOrDefault().FromEmailAddress;

                //generate email body text f
                var body = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.AutoBillingInvoiceClosureValidationError,
                                                                        context);

                //create a mail object to send mail
                var overview = new MailMessage
                {
                  From = new MailAddress(emailSetting.SingleOrDefault().FromEmailAddress),
                  IsBodyHtml = true
                };

                foreach (var contact in toEmailList)
                {
                  _logger.Debug("Sending mail to billing members of invoices where Re-validation of invoice failed : " + contact);
                  overview.To.Add(new MailAddress(contact));
                }

                //set subject of mail 
                overview.Subject = emailSetting.SingleOrDefault().Subject;
                // replace '(From environment)' with '' from Subject line
                overview.Subject.Replace("(From environment)", "(From environment)");

                //set body text of mail
                overview.Body = body;


                _logger.Debug("Sending mail to billing members of invoices with Invalid IS Membership Status: " + body);

                //send the mail
                emailSender.Send(overview);
              }
              else
              {
                //Get the eMail settings for auto billing invoice closure error 
                var emailSetting =
                    emailSettingsRepository.Get(es => es.Id == (int)EmailTemplateId.AutoBillingInvoiceClosureError);

                invoiceInError.SisOpsEmailId = emailSetting.SingleOrDefault().FromEmailAddress;

                //generate email body text f
                var body = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.AutoBillingInvoiceClosureError,
                                                                        context);

                //create a mail object to send mail
                var overview = new MailMessage
                {
                  From = new MailAddress(emailSetting.SingleOrDefault().FromEmailAddress),
                  IsBodyHtml = true
                };

                foreach (var contact in toEmailList)
                {
                  _logger.Debug("Sending mail to billing members of invoices with Invalid IS Membership Status: " + contact);
                  overview.To.Add(new MailAddress(contact));
                }

                //set subject of mail 
                overview.Subject = emailSetting.SingleOrDefault().Subject;
                // replace '(From environment)' with '' from Subject line
                overview.Subject.Replace("(From environment)", "(From environment)");

                //set body text of mail
                overview.Body = body;


                _logger.Debug("Sending mail to billing members of invoices with Invalid IS Membership Status: " + body);

                //send the mail
                emailSender.Send(overview);
              }
            }
            catch (Exception exception)
            {
                _logger.Error("Error occured at sending emails to server", exception);
                throw;
            }
        }
    }
}

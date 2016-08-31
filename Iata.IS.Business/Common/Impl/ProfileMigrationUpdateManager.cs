using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using Castle.Core.Smtp;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Data.MemberProfile.Impl;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using UnitOfWork = Iata.IS.Data.Impl.UnitOfWork;
using log4net;
using NVelocity;

namespace Iata.IS.Business.Common.Impl
{
  
    public class ProfileMigrationUpdateManager: IProfileMigrationUpdateManager
    {       
        /// <summary>
        /// Gets or sets the passenger repository.
        /// </summary>
        /// <value>The passenger repository.</value>
        public IRepository<PassengerConfiguration> PassengerRepository { get; set; }

        /// <summary>
        /// Gets or sets the cargo configuration repository.
        /// </summary>
        /// <value>The cargo configuration repository.</value>
        public IRepository<CargoConfiguration> CargoRepository { get; set; }

        /// <summary>
        /// Gets or sets the miscellaneous configuration repository.
        /// </summary>
        /// <value>The miscellaneous configuration repository.</value>
        public IRepository<MiscellaneousConfiguration> MiscellaneousRepository { get; set; }

        /// <summary>
        /// Calendar Manager, will be injected by the container
        /// </summary>
        /// <value>The calendar manager repository.</value>
        public ICalendarManager CalendarManager { get; set; }

        /// <summary>
        /// Gets or sets the member repository.
        /// </summary>
        /// <value>The member repository.</value>
        public IMemberRepository MemberRepository { get; set; }

        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ProfileMigrationUpdateManager(ICalendarManager calendarManager)
        {
            CalendarManager = calendarManager;

            _logger.Info("Profile migration update job - Reading default period");
        }

        /// <summary>
        /// Method is used to set current period. Only if migration date field has valid date.
        /// </summary>
        public void UpdateProfileMigrationData()
        {
            var _fileManager = Ioc.Resolve<IFileManager>(typeof(IFileManager));
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                var memberIdList = GetMemberIdList();

                //if default period is set to 0/blank then pick current period.
                var currentPeriod = new DateTime();
                DateTime date;

                if(ConfigurationManager.AppSettings["DefaultPeriod"]!=null && DateTime.TryParse(ConfigurationManager.AppSettings["DefaultPeriod"], out date))
                {
                    currentPeriod = Convert.ToDateTime(ConfigurationManager.AppSettings["DefaultPeriod"]);

                    _logger.InfoFormat("Default period - Year: {0}, Month: {1}, Period: {2}", currentPeriod.Year, currentPeriod.Month, currentPeriod.Day);
                }
                else
                {
                    var period = CalendarManager.GetCurrentBillingPeriod(ClearingHouse.Ich);
                    currentPeriod = new DateTime(period.Year, period.Month, period.Period);

                    _logger.InfoFormat("Current period - Year: {0}, Month: {1}, Period: {2}", currentPeriod.Year, currentPeriod.Month, currentPeriod.Day);
                }

                foreach (var memberId in memberIdList)
                {
                        var paxConfig = PassengerRepository.Get(mem => mem.MemberId == memberId).SingleOrDefault();
                        paxConfig = ProcessPaxMigrationData(paxConfig, currentPeriod);

                        var cgoConfig = CargoRepository.Get(mem => mem.MemberId == memberId).SingleOrDefault();
                        cgoConfig = ProcessCargoMigrationData(cgoConfig, currentPeriod);

                        var miscConfig = MiscellaneousRepository.Get(mem => mem.MemberId == memberId).SingleOrDefault();
                        miscConfig = ProcessMiscMigrationData(miscConfig, currentPeriod);

                        UpdateProfileConfiguration(paxConfig, cgoConfig, miscConfig);
                }

                _logger.Info("Profile migration update job - profile migration changes are done");

                //commit changes to DB.
                UnitOfWork.CommitDefault();

                _logger.InfoFormat("Profile migration update job - profile migration changes commited on {0}",
                                   DateTime.UtcNow);

                _logger.Info("Profile migration update job - Flush system parameter XML from cache.");

                _fileManager.FlushSystemParameterXmlFile();

                _logger.Info("Profile migration update job - Sending success email.");

                SendProfileMigrationUpdateNotification(isSuccessMsg: true);
                
                _logger.Info("Profile migration update job - successfully run");
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred in profile c update).", ex);

                SendProfileMigrationUpdateNotification(isSuccessMsg: false, exceptionMsg: ex.StackTrace);
            }
            finally
            {
                stopWatch.Stop();

                _logger.InfoFormat("Profile migration update job - Time Required: [{0}]", stopWatch.Elapsed);
            }

        }

        /// <summary>
        /// Get list of member id list
        /// </summary>
        /// <returns></returns>
        public List<int> GetMemberIdList()
        {
            var memberList = MemberRepository.GetAll().Select(mem => mem.Id).ToList();

            return memberList;
        }

        /// <summary>
        /// Update profile configuration 
        /// </summary>
        /// <param name="paxConfig">pax configuration</param>
        /// <param name="cgoConfig">cargo configuration</param>
        /// <param name="miscConfig">misc configuration</param>
        private void UpdateProfileConfiguration(PassengerConfiguration paxConfig, CargoConfiguration cgoConfig, MiscellaneousConfiguration miscConfig)
        {
            if (paxConfig != null)
            {
                PassengerRepository.Update(paxConfig);
            }

            if (cgoConfig != null)
            {
                CargoRepository.Update(cgoConfig);
            }

            if (miscConfig != null)
            {
                MiscellaneousRepository.Update(miscConfig);
            }
        }

        /// <summary>
        /// Set current period date, if migration field contains date.
        /// </summary>
        /// <param name="pax">pax configuration</param>
        /// <param name="currentBillingPeriod">current period</param>
        /// <returns></returns>
        private PassengerConfiguration ProcessPaxMigrationData(PassengerConfiguration pax, DateTime currentBillingPeriod)
        {
            var isUpdated = false;

            if (pax != null)
            {
                var nsPrimeIsidec = pax.NonSamplePrimeBillingIsIdecMigratedDate;
                var nsPrimeIsxml = pax.NonSamplePrimeBillingIsxmlMigratedDate;
                var nsPrimeIsweb = pax.NonSamplePrimeBillingIswebMigratedDate;

                if (groupCanUpdate(nsPrimeIsweb))
                {
                    pax.NonSamplePrimeBillingIsIdecMigratedDate = nsPrimeIsidec.HasValue
                                                                      ? currentBillingPeriod
                                                                      : nsPrimeIsidec;
                    pax.NonSamplePrimeBillingIsxmlMigratedDate = nsPrimeIsxml.HasValue
                                                                     ? currentBillingPeriod
                                                                     : nsPrimeIsxml;

                    isUpdated = true;
                }

                var nsrmisidec = pax.NonSampleRmIsIdecMigratedDate;
                var nsrmisxml = pax.NonSampleRmIsXmlMigratedDate;
                var nsrmisweb = pax.NonSampleRmIswebMigratedDate;

                if (groupCanUpdate(nsrmisweb))
                {
                    pax.NonSampleRmIsIdecMigratedDate = nsrmisidec.HasValue ? currentBillingPeriod : nsrmisidec;
                    pax.NonSampleRmIsXmlMigratedDate = nsrmisxml.HasValue ? currentBillingPeriod : nsrmisxml;
                    isUpdated = true;
                }

                var nsbmisidec = pax.NonSampleBmIsIdecMigratedDate;
                var nsbmisxml = pax.NonSampleBmIsXmlMigratedDate;
                var nsbmisweb = pax.NonSampleBmIswebMigratedDate;

                if (groupCanUpdate(nsbmisweb))
                {
                    pax.NonSampleBmIsIdecMigratedDate = nsbmisidec.HasValue ? currentBillingPeriod : nsbmisidec;
                    pax.NonSampleBmIsXmlMigratedDate = nsbmisxml.HasValue ? currentBillingPeriod : nsbmisxml;
                    isUpdated = true;
                }

                var nscmisidec = pax.NonSampleCmIsIdecMigratedDate;
                var nscmisxml = pax.NonSampleCmIsXmlMigratedDate;
                var nscmisweb = pax.NonSampleCmIswebMigratedDate;

                if (groupCanUpdate(nscmisweb))
                {
                    pax.NonSampleCmIsIdecMigratedDate = nscmisidec.HasValue ? currentBillingPeriod : nscmisidec;
                    pax.NonSampleCmIsXmlMigratedDate = nscmisxml.HasValue ? currentBillingPeriod : nscmisxml;
                    isUpdated = true;
                }

                var fromfxfisidec = pax.SampleFormFxfIsIdecMigratedDate;
                var fromfxfisxml = pax.SampleFormFxfIsxmlMigratedDate;
                var fromfxfisweb = pax.SampleFormFxfIswebMigratedDate;

                if (groupCanUpdate(fromfxfisweb))
                {
                    pax.SampleFormFxfIsIdecMigratedDate = fromfxfisidec.HasValue ? currentBillingPeriod : fromfxfisidec;
                    pax.SampleFormFxfIsxmlMigratedDate = fromfxfisxml.HasValue ? currentBillingPeriod : fromfxfisxml;
                    isUpdated = true;
                }

                if (isUpdated)
                {
                    pax.LastUpdatedOn = DateTime.UtcNow;
                    pax.LastUpdatedBy = 0;
                }
            }
            return pax;
        }

        /// <summary>
        /// Set current period date, if migration field contains date.
        /// </summary>
        /// <param name="cgo">cargo configuration</param>
        /// <param name="currentBillingPeriod">current period</param>
        /// <returns></returns>
        private CargoConfiguration ProcessCargoMigrationData(CargoConfiguration cgo, DateTime currentBillingPeriod)
        {
            var isUpdated = false;

            if (cgo != null)
            {
                var cgoPrimeIsidec = cgo.PrimeBillingIsIdecMigratedDate;
                var cgoPrimeIsxml = cgo.PrimeBillingIsxmlMigratedDate;
                var cgoPrimeIsweb = cgo.PrimeBillingIswebMigratedDate;

                if (groupCanUpdate(cgoPrimeIsweb))
                {
                    cgo.PrimeBillingIsIdecMigratedDate = cgoPrimeIsidec.HasValue ? currentBillingPeriod : cgoPrimeIsidec;
                    cgo.PrimeBillingIsxmlMigratedDate = cgoPrimeIsxml.HasValue ? currentBillingPeriod : cgoPrimeIsxml;
                    isUpdated = true;
                }

                var cgormisidec = cgo.RmIsIdecMigratedDate;
                var cgormisxml = cgo.RmIsXmlMigratedDate;
                var cgormisweb = cgo.RmIswebMigratedDate;

                if (groupCanUpdate(cgormisweb))
                {
                    cgo.RmIsIdecMigratedDate = cgormisidec.HasValue ? currentBillingPeriod : cgormisidec;
                    cgo.RmIsXmlMigratedDate = cgormisxml.HasValue ? currentBillingPeriod : cgormisxml;
                    isUpdated = true;
                }

                var cgobmisidec = cgo.BmIsIdecMigratedDate;
                var cgobmisxml = cgo.BmIsXmlMigratedDate;
                var cgobmisweb = cgo.BmIswebMigratedDate;

                if (groupCanUpdate(cgobmisweb))
                {
                    cgo.BmIsIdecMigratedDate = cgobmisidec.HasValue ? currentBillingPeriod : cgobmisidec;
                    cgo.BmIsXmlMigratedDate = cgobmisxml.HasValue ? currentBillingPeriod : cgobmisxml;
                    isUpdated = true;
                }

                var cgocmisidec = cgo.CmIsIdecMigratedDate;
                var cgocmisxml = cgo.CmIsXmlMigratedDate;
                var cgocmisweb = cgo.CmIswebMigratedDate;

                if (groupCanUpdate(cgocmisweb))
                {
                    cgo.CmIsIdecMigratedDate = cgocmisidec.HasValue ? currentBillingPeriod : cgocmisidec;
                    cgo.CmIsXmlMigratedDate = cgocmisxml.HasValue ? currentBillingPeriod : cgocmisxml;
                    isUpdated = true;
                }

                if (isUpdated)
                {
                    cgo.LastUpdatedOn = DateTime.UtcNow;
                    cgo.LastUpdatedBy = 0;
                }
            }
            return cgo;
        }

        /// <summary>
        /// Set current period date, if migration field contains date.
        /// </summary>
        /// <param name="misc">misc configuration</param>
        /// <param name="currentBillingPeriod">current period</param>
        /// <returns></returns>
        private MiscellaneousConfiguration ProcessMiscMigrationData(MiscellaneousConfiguration misc, DateTime currentBillingPeriod)
        {
            var isUpdated = false;

            if (misc != null)
            {
                var miscIsXml = misc.BillingIsXmlMigrationDate;
                var miscIsweb = misc.BillingIswebMigrationDate;

                if (groupCanUpdate(miscIsweb))
                {
                    misc.BillingIsXmlMigrationDate = miscIsXml.HasValue ? currentBillingPeriod : miscIsXml;
                    isUpdated = true;
                }

                if (isUpdated)
                {
                    misc.LastUpdatedOn = DateTime.UtcNow;
                    misc.LastUpdatedBy = 0;
                }
            }

            return misc;
        }

        /// <summary>
        /// Method return false, if idec, xml or isweb field contains migration year 1900. otherwise true. 
        /// </summary>
        /// <param name="iSweb">isweb migration date</param>
        /// <returns>ture/false</returns>
        private bool groupCanUpdate(DateTime? iSweb)
        {
            if (iSweb.HasValue && iSweb.Value.Year.Equals(1900) && iSweb.Value.Month.Equals(1) && iSweb.Value.Day.Equals(1))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Method is used to send email notification to SysOps and IS Admin. If failed.
        /// </summary>
        /// <param name="isSuccessMsg">flag to show is this method should send success or failure message</param>
        /// <param name="exceptionMsg">exception message</param>
        /// <returns></returns>
        private bool SendProfileMigrationUpdateNotification(bool isSuccessMsg = false, string exceptionMsg = null)
        {
            var message = new MailMessage();
            try
            {
                //declare an object of the nVelocity data dictionary
                VelocityContext context;
                //get an object of the EmailSender component
                var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
                List<MailAddress> isOpsMailAdressList = null;
                //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nVelocity template
                var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
                //get an instance of email settings  repository
                var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
                //object of the nVelocity data dictionary
                var emailBodyText = string.Empty;
                var fromEmailAdd = string.Empty;
                var emailSubject = string.Empty;
                context = new VelocityContext();
               
                if (isSuccessMsg)
                {
                    var emailSettingForMemberCreation =
                       emailSettingsRepository.Get(
                           esfopu => esfopu.Id == (int)EmailTemplateId.ProfileMigrationUpdateSuccessNotification);
                    
                    //generate email body text for own profile updates contact type mail
                    emailBodyText =
                        templatedTextGenerator.GenerateTemplatedText(
                            EmailTemplateId.ProfileMigrationUpdateSuccessNotification, context);

                    fromEmailAdd = emailSettingForMemberCreation.SingleOrDefault().FromEmailAddress;
                    emailSubject = emailSettingForMemberCreation.SingleOrDefault().Subject;
                }
                else
                {
                    
                    context.Put("ExceptionMessage", exceptionMsg);
                    var emailSettingForMemberCreation =
                        emailSettingsRepository.Get(
                            esfopu => esfopu.Id == (int) EmailTemplateId.ProfileMigrationUpdateFailurNotification).ToList();
                    fromEmailAdd = emailSettingForMemberCreation.SingleOrDefault().FromEmailAddress;
                    //generate email body text for own profile updates contact type mail
                    emailBodyText =
                        templatedTextGenerator.GenerateTemplatedText(
                            EmailTemplateId.ProfileMigrationUpdateFailurNotification, context);
                    emailSubject = emailSettingForMemberCreation.SingleOrDefault().Subject;
                }
                //create a mail object to send mail
                message = new MailMessage { From = new MailAddress(fromEmailAdd), IsBodyHtml = true };

                if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail))
                {
                    var emailAddressList = AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail;
                    var formatedEmailList = emailAddressList.Replace(',', ';');
                    isOpsMailAdressList = ConvertUtil.ConvertToMailAddresses(formatedEmailList);

                    foreach (var mailaddr in isOpsMailAdressList)
                    {
                        message.To.Add(mailaddr);
                    }

                }
              
                message.Subject = emailSubject;

                //set body text of mail
                message.Body = emailBodyText;

                // send the mail.
                emailSender.Send(message);
               
                //clear nvelocity context data
                context = null;

                return true;
            }
            catch (Exception exception)
            {
                message.Dispose();
                _logger.Error("Error occurred in sending mail for profile migration update).", exception);
                return false;
            }
        }

    }
}

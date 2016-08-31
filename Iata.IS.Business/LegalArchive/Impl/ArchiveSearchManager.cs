using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Data;
using Iata.IS.Model.Enums;
using Iata.IS.Model.LegalArchive;
using Iata.IS.Data.LegalArchive;
using System;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.MemberProfile;
using log4net;
using System.Reflection;
using System.Configuration;
using log4net.Repository.Hierarchy;
using NVelocity;
using System.Net.Mail;
using Castle.Core.Smtp;
using Iata.IS.Core.DI;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Model.Common;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using System.Data.Objects.SqlClient;
using Iata.IS.Data.Impl;

namespace Iata.IS.Business.LegalArchive.Impl
{
    public class ArchiveSearchManager : IArchiveSearchManager
    {

        #region Private Member
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _typeReceivables = BillingType.Receivables.ToString();
        private readonly string _typePayables = BillingType.Payables.ToString();
        #endregion

        #region Properties
        public IRepository<LegalArchiveSearch> LegalArchiveSearchRepository { get; set; }
        public IRepository<LegalArchiveLog> LegalArchiveLogRepository { get; set; }
        public IRepository<Member> MemberRepository { get; set; }
        public IRepository<User> UserRepository { get; set; }
        public IRepository<RetrievalJobDetails> JobDetailRepository { get; set; }
        public IRepository<RetrievalJobDetails> RetrievalJobDetailRepository { get; set; }
        public IRepository<RetrievalJobSummary> RetrievalJobSummaryRepository { get; set; }
        #endregion
        
        /// <summary>
        /// Get archive based on search criteria
        /// </summary>
        /// <parametes name="searchCriteria">The legal archive search criteria.</parametes>
        /// <parametes name="member id ">id of member</parametes>
        public IQueryable<LegalArchiveSearch> SearchArchives(LegalArchiveSearchCriteria searchCriteria, int memberId)
        {
            //getting Iqueryable list for legal archives
            var filteredList = LegalArchiveSearchRepository.GetAll();
            //CMP #666: MISC Legal Archiving Per Location ID

            IEnumerable<LegalArchiveSearch> iQueryableExcludingMiscInv = null;
            IEnumerable<LegalArchiveSearch> iQueryableOnlyMiscInv = null;

            if (searchCriteria.BillingCategoryId == -1 || searchCriteria.BillingCategoryId !=3)
            {
                iQueryableExcludingMiscInv = FetchExcludingMiscInvoice(searchCriteria, memberId, filteredList);
                if (searchCriteria.BillingCategoryId == -1)
                { 
                    iQueryableOnlyMiscInv = FetchOnlyMiscInvoice(searchCriteria, memberId, filteredList);
                    return iQueryableExcludingMiscInv.Concat(iQueryableOnlyMiscInv).AsQueryable();
                }
                return iQueryableExcludingMiscInv.AsQueryable();
            }

            if (searchCriteria.BillingCategoryId == 3)
            {
                iQueryableOnlyMiscInv = FetchOnlyMiscInvoice(searchCriteria, memberId, filteredList);
                return iQueryableOnlyMiscInv.AsQueryable();
            }
            return null;
        }

        /// <summary>
        /// CMP #666: MISC Legal Archiving Per Location ID
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="memberId"></param>
        /// <param name="filteredList"></param>
        /// <returns></returns>
         private IEnumerable<LegalArchiveSearch> FetchOnlyMiscInvoice(LegalArchiveSearchCriteria searchCriteria, int memberId, IQueryable<LegalArchiveSearch> filteredList)
         {
             
             if (searchCriteria == null)
             {
                 return null;
             }
             else
             {
                 //check if invoice number is passed or not 
                 if (!string.IsNullOrEmpty(searchCriteria.InvoiceNumber))
                 {
                     filteredList =
                         filteredList.Where(
                             invoice => invoice.InvoiceNumber.ToUpper().Contains(searchCriteria.InvoiceNumber.ToUpper()));
                 }

                 // Check if billing year is passed in search criteria.
                 if (searchCriteria.BillingYear > 0)
                 {
                     filteredList =
                         filteredList.Where(
                             invoice =>
                             invoice.BillingYear == searchCriteria.BillingYear);
                 }

                 // Check if billing month is passed in search criteria.
                 if (searchCriteria.BillingMonth > 0)
                 {
                     filteredList =
                         filteredList.Where(
                             invoice => invoice.BillingMonthId == searchCriteria.BillingMonth);
                 }

                 // Check if billing period is passed in search criteria.
                 if (searchCriteria.BillingPeriod > 0)
                 {
                     filteredList = filteredList.Where(invoice => invoice.BillingPeriod == searchCriteria.BillingPeriod);
                 }

                 // Check if member id is passed in search criteria.
                 // check for Receivables
                 if (searchCriteria.Type == 2)
                 {
                     filteredList =
                         filteredList.Where(invoice => invoice.MemberId == memberId);
                     filteredList =
                         filteredList.Where(invoice => invoice.CrBillingMemeberIsId == memberId);

                     if (searchCriteria.MemberId > 0)
                     {
                         filteredList =
                            filteredList.Where(invoice => invoice.DbBilledMemeberIsId == searchCriteria.MemberId);
                     }
                 }
                 //check for Payable
                 else if (searchCriteria.Type == 1)
                 {
                     filteredList =
                         filteredList.Where(invoice => invoice.MemberId == memberId);
                     filteredList =
                         filteredList.Where(invoice => invoice.DbBilledMemeberIsId == memberId);

                     if (searchCriteria.MemberId > 0)
                     {
                         filteredList =
                            filteredList.Where(invoice => invoice.CrBillingMemeberIsId == searchCriteria.MemberId);
                     }
                 }



                 // Check if SettlementMethod id is passed in search criteria.
                 if (searchCriteria.SettlementMethodId > 0)
                 {
                     filteredList =
                         filteredList.Where(
                             invoice => invoice.SettlementMethodId == searchCriteria.SettlementMethodId);
                 }

                 // check if billing country code is passed in search criteria 
                 if (!string.IsNullOrEmpty(searchCriteria.BillingCountryCode))
                 {
                     filteredList =
                         filteredList.Where(
                             invoice => invoice.BillingCountryCode == searchCriteria.BillingCountryCode);
                 }

                 // check if billing country code is passed in search criteria 
                 if (!string.IsNullOrEmpty(searchCriteria.BilledCountryCode))
                 {
                     filteredList =
                         filteredList.Where(invoice => invoice.BilledCountryCode == searchCriteria.BilledCountryCode);
                 }


                 // check if bill type is passed in search criteria 
                 if (searchCriteria.Type > 0)
                 {
                     filteredList =
                         filteredList.Where(
                             invoice =>
                             invoice.ReceivablePayableIndicator ==
                             (searchCriteria.Type == 1 ? _typePayables : _typeReceivables));
                 }
                
                 if (searchCriteria.ArchivalLocationId.Length > 0)
                 {
                     string[] selectedLocs = searchCriteria.ArchivalLocationId.ToUpper().Split(",".ToCharArray(),
                                                                                     StringSplitOptions.
                                                                                         RemoveEmptyEntries);

                     bool containsMainLoc = selectedLocs.Contains("MAIN", StringComparer.OrdinalIgnoreCase);

                     filteredList = containsMainLoc ? filteredList.Where(i => (i.LegalArchivalLocation == null || i.LegalArchivalLocation.ToUpper() == "MAIN" || selectedLocs.Contains(i.LegalArchivalLocation.ToUpper())) && i.BillingCategoryId == 3) :
                                                         filteredList.Where(i => (selectedLocs.Contains(i.LegalArchivalLocation.ToUpper())) && i.BillingCategoryId == 3);
                 }


                 return filteredList;
             }
             
         }

         /// <summary>
         /// CMP #666: MISC Legal Archiving Per Location ID
         /// </summary>
         /// <param name="searchCriteria"></param>
         /// <param name="memberId"></param>
         /// <param name="filteredList"></param>
         /// <returns></returns>
        private IEnumerable<LegalArchiveSearch> FetchExcludingMiscInvoice(LegalArchiveSearchCriteria searchCriteria,int memberId, IQueryable<LegalArchiveSearch> filteredList)
        {
          
            if (searchCriteria == null)
            {
                return null;
            }
            else
            {
                //check if invoice number is passed or not 
                if (!string.IsNullOrEmpty(searchCriteria.InvoiceNumber))
                {
                    filteredList =
                        filteredList.Where(
                            invoice => invoice.InvoiceNumber.ToUpper().Contains(searchCriteria.InvoiceNumber.ToUpper()));
                }

                // Check if billing year is passed in search criteria.
                if (searchCriteria.BillingYear > 0)
                {
                    filteredList =
                        filteredList.Where(
                            invoice =>
                            invoice.BillingYear == searchCriteria.BillingYear);
                }

                // Check if billing month is passed in search criteria.
                if (searchCriteria.BillingMonth > 0)
                {
                    filteredList =
                        filteredList.Where(
                            invoice => invoice.BillingMonthId == searchCriteria.BillingMonth);
                }

                // Check if billing period is passed in search criteria.
                if (searchCriteria.BillingPeriod > 0)
                {
                    filteredList = filteredList.Where(invoice => invoice.BillingPeriod == searchCriteria.BillingPeriod);
                }

                // Check if member id is passed in search criteria.
                // check for Receivables
                if (searchCriteria.Type == 2)
                {
                    filteredList =
                        filteredList.Where(invoice => invoice.MemberId == memberId);
                    filteredList =
                        filteredList.Where(invoice => invoice.CrBillingMemeberIsId == memberId);

                    if (searchCriteria.MemberId > 0)
                    {
                        filteredList =
                            filteredList.Where(invoice => invoice.DbBilledMemeberIsId == searchCriteria.MemberId);
                    }
                }
                    //check for Payable
                else if (searchCriteria.Type == 1)
                {
                    filteredList =
                        filteredList.Where(invoice => invoice.MemberId == memberId);
                    filteredList =
                        filteredList.Where(invoice => invoice.DbBilledMemeberIsId == memberId);

                    if (searchCriteria.MemberId > 0)
                    {
                        filteredList =
                            filteredList.Where(invoice => invoice.CrBillingMemeberIsId == searchCriteria.MemberId);
                    }
                }



                // Check if SettlementMethod id is passed in search criteria.
                if (searchCriteria.SettlementMethodId > 0)
                {
                    filteredList =
                        filteredList.Where(
                            invoice => invoice.SettlementMethodId == searchCriteria.SettlementMethodId);
                }

                // check if billing country code is passed in search criteria 
                if (!string.IsNullOrEmpty(searchCriteria.BillingCountryCode))
                {
                    filteredList =
                        filteredList.Where(
                            invoice => invoice.BillingCountryCode == searchCriteria.BillingCountryCode);
                }

                // check if billing country code is passed in search criteria 
                if (!string.IsNullOrEmpty(searchCriteria.BilledCountryCode))
                {
                    filteredList =
                        filteredList.Where(invoice => invoice.BilledCountryCode == searchCriteria.BilledCountryCode);
                }


                // check if bill type is passed in search criteria 
                if (searchCriteria.Type > 0)
                {
                    filteredList =
                        filteredList.Where(
                            invoice =>
                            invoice.ReceivablePayableIndicator ==
                            (searchCriteria.Type == 1 ? _typePayables : _typeReceivables));
                }

                if (searchCriteria.BillingCategoryId == -1){
                    filteredList =
                        filteredList.Where(
                            invoice => invoice.BillingCategoryId != 3);
                }
                else
                {
                    filteredList =
                        filteredList.Where(
                            invoice => invoice.BillingCategoryId == searchCriteria.BillingCategoryId);
                }
                 
            }
                return filteredList;
            
        }



        /// <summary>
        /// get search criteria object : convert model values string into search criteria object
        /// </summary>
        /// <param name="modelValues">values of search criteria</param>
        ///<returns>RetrievalJobDetails object</returns>
        public LegalArchiveSearchCriteria GetSearchCriteria(List<string> modelValues)
        {
            var model = new Dictionary<string, string>();
            foreach (var value in modelValues)
            {
                var val = value.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if(!model.ContainsKey(val[0]))
                model.Add(val[0], val.Length == 1 ? string.Empty : val[1]);
            }

            int billingYear, billingMonth, billingPeriod, billingCategoryId, settlementMethodId, memberId, type;

            var searchCriteria = new LegalArchiveSearchCriteria
            {
                InvoiceNumber = model["InvoiceNumber"],
                BillingCountryCode = model["BillingCountryCode"],
                BilledCountryCode = model["BilledCountryCode"],
            };

            if (int.TryParse(model["Type"], out type))
            {
                searchCriteria.Type = type;
            }

            if (int.TryParse(model["BillingYear"], out billingYear))
            {
                searchCriteria.BillingYear = billingYear;
            }

            if (int.TryParse(model["BillingMonth"], out billingMonth))
            {
                searchCriteria.BillingMonth = billingMonth;
            }

            if (int.TryParse(model["BillingPeriod"], out billingPeriod))
            {
                searchCriteria.BillingPeriod = billingPeriod;
            }

            if (int.TryParse(model["BillingCategoryId"], out billingCategoryId))
            {
                searchCriteria.BillingCategoryId = billingCategoryId;
            }

            if (int.TryParse(model["SettlementMethodId"], out settlementMethodId))
            {
                searchCriteria.SettlementMethodId = settlementMethodId;
            }

            if (int.TryParse(model["MemberId"], out memberId))
            {
                searchCriteria.MemberId = memberId;
            }

           
            

            return searchCriteria;  
        }

        /// <summary>
        /// Get all selected legal archives
        /// </summary>
        /// <param name="archiveIdList">archiveIdList</param>
        /// <param name="userId">user id </param>
        /// <param name="memberId">member id </param>
        /// <param name="searchCriteria">actual search criteria choose by user</param>
        /// <param name="jobId">out parameters for job id </param>
        /// <param name="totalNoInvoicesRetrieved">total no of invoices retrive</param>
        /// <returns>true if job_id is successfully created or flase</returns>
        public bool RetriveLegalArchive(List<string> archiveIdList, int userId, int memberId, LegalArchiveSearchCriteria searchCriteria, out string jobId, out int totalNoInvoicesRetrieved)
        {
            try
            {
                var memberData = MemberRepository.Single(mem => mem.Id == memberId);
                var userData = UserRepository.Single(user => user.Id == userId);
                jobId = GenerateJobId(memberId);
                totalNoInvoicesRetrieved = archiveIdList.Count;

                //get all archives those are selected by user
                var retrivedArchives = GetArchiveItemList(archiveIdList);

                // add archive job summary here
                var jobSummary = GetJobSummary(memberData, userData, jobId, totalNoInvoicesRetrieved, searchCriteria);
                RetrievalJobSummaryRepository.Add(jobSummary);
                UnitOfWork.CommitDefault();

                var jobSummaryId = jobSummary.Id;

                // using loop to add all archive information in job detail table
                foreach (var archiveLog in retrivedArchives)
                {
                    var jobDetail = GetJobDetail(archiveLog, jobSummaryId, userId);
                    JobDetailRepository.Add(jobDetail);

                    var invoiceId = archiveLog.InvoiceId;
                    var iua = archiveLog.Iua;
                    var invoiceType = archiveLog.InvoiceType;
                    InsertMessageInOracleQueue(invoiceId, iua, invoiceType, jobSummaryId, memberId);
                }

                UnitOfWork.CommitDefault();

                return true;
            }
            // TODO: Add exception logic here
            catch (ISBusinessException ex)
            {
                _logger.Debug("archive retrival failed.", ex);

                throw new ISBusinessException("");

                return false;
            }
        }

        /// <summary>
        /// get job summary model object
        /// </summary>
        /// <param name="archiveLog">LegalArchiveLog</param>
        /// <param name="jobSummaryId">job summary id</param>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        public RetrievalJobDetails GetJobDetail(LegalArchiveLog archiveLog, Guid jobSummaryId, int userId)
        {
            const string retrivalStatus = "P";
            var currentDateTime = DateTime.UtcNow;
           
            var retrievalJobDetail = new RetrievalJobDetails
            {
                RetrievalJobSummaryId = jobSummaryId,
                Arccustdesignator = archiveLog.ArcCustDesignator,
                Arccustaccounting = archiveLog.ArcCustAccounting,
                Arccustisid = archiveLog.ArcCustIsId,
                Crbillingmemberdesignator = archiveLog.CrBillingMemberDesignator,
                Crbillingmemberaccounting = archiveLog.CrBillingMemberAccounting,
                Crbillingmemberisid = archiveLog.CrBillingMemberIsId,
                Dbbilledmemberdesignator = archiveLog.DbBilledMemberDesignator,
                Dbbilledmemberaccounting = archiveLog.DbBilledMemberAccounting,
                Dbbilledmemberisid = archiveLog.DbBilledMemberIsId,
                Billingcategory = archiveLog.BillingCategory,
                Receivablespayablesindicator = archiveLog.ReceivablesPayablesIndicator,
                Invoicenumber = archiveLog.InvoiceNumber,
                Invoiceid = archiveLog.InvoiceId,
                Invoicetype = archiveLog.InvoiceType,
                Billingyear = archiveLog.BillingYear,
                Billingmonth = archiveLog.BillingMonth,
                Billingperiod = archiveLog.BillingPeriod,
                Invoicedate = archiveLog.InvoiceDate,
                Settlementindicator = archiveLog.SettlementIndicator,
                Crbillingmembercountry = archiveLog.CrBillingMemberCountry,
                Dbbilledmembercountry = archiveLog.DbBilledMemberCountry,
                Iua = archiveLog.Iua,
                Retrivalstatus = retrivalStatus,
                Retrivalrequestdatetime = currentDateTime,
                MiscLocationsCode = archiveLog.ArchivalLocationCode, //CMP-666-MISC Legal Archiving Per Location ID
                Lastupdatedby = userId,
                Lastupdatedon = currentDateTime
            };

            return retrievalJobDetail;
        }

        /// <summary>
        /// get job summary object from archive log 
        /// </summary>
        /// <param name="memberData">member object</param>
        /// <param name="userData">user object</param>
        /// <param name="jobId">job id</param>
        /// <param name="totalInvoiceRetrive">total no of invoice retrive</param>
        /// <param name="searchCriteria">archive search criteria</param>
        /// <returns>RetrievalJobSummary</returns>
        public RetrievalJobSummary GetJobSummary(Member memberData, User userData, string jobId, int totalInvoiceRetrive, LegalArchiveSearchCriteria searchCriteria)
        {
            const string jobStatus = "In Progress";
            var requestedUser = string.Format("{0} {1}", userData.FirstName, userData.LastName);
            
            string member = string.Empty;
            var memberManager = Ioc.Resolve<IMemberManager>(typeof (IMemberManager));
            // If search criteria provided.
            if(searchCriteria.MemberId != 0)
            {
              var memberDetails = memberManager.GetMemberDetails(searchCriteria.MemberId);
              member = string.Format("{0}-{1}", memberDetails.MemberCodeAlpha, memberDetails.MemberCodeNumeric);
            }

            var retrievalJobSummary = new Iata.IS.Model.LegalArchive.RetrievalJobSummary
            {
                Arcretrievaljobid = jobId,
                Totalnoinvoicesretrieved = totalInvoiceRetrive,
                Arccustdesignator = memberData.MemberCodeAlpha,
                Arccustaccounting = memberData.MemberCodeNumeric,
                Arccusisid = memberData.Id,
                Requestedon = DateTime.UtcNow,
                Requestedby = requestedUser,
                Jobstatus = jobStatus,
                Invoicenumber = searchCriteria.InvoiceNumber,
                Itype = searchCriteria.Type,
                Billingyear = searchCriteria.BillingYear,
                Billingmonth = searchCriteria.BillingMonth,
                Billingperiod = searchCriteria.BillingPeriod,
                Member = member,
                Billingcategory = searchCriteria.BillingCategoryId,
                Billinglocationcountry = searchCriteria.BillingCountryCode,
                Billedlocationcountry = searchCriteria.BilledCountryCode,
                Settlementmethod = searchCriteria.SettlementMethodId,
                Expirydatepurging = DateTime.UtcNow.AddDays(AdminSystem.SystemParameters.Instance.LegalArchivingDetails.RetentionPeriodofRetrievedLAJobsinDays+1),
                MiscLocationCodes = (searchCriteria.ArchivalLocationId.StartsWith(",")) ? searchCriteria.ArchivalLocationId.Substring(1) : searchCriteria.ArchivalLocationId,
                Lastupdatedby = userData.Id,
                Lastupdatedon = DateTime.UtcNow
            };

            return retrievalJobSummary;
        }

        /// <summary>
        /// Get job Id for retrive archives request
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns>job id as sting</returns>
        private string GenerateJobId(int memberId)
        {
            // job it is a combination of <member accounting code>-<date of retrive>-<job detail sequence number>
            var memberProfile = MemberRepository.Single(m => m.Id == memberId);
            var totalJobDetailRows = (JobDetailRepository.GetCount() + 1);

            var memberAccountingCode = memberProfile.MemberCodeNumeric;

            // create date like 20110801 = 01-Aug-2011
            var currentDate = string.Format("{0}{1}{2}", DateTime.UtcNow.Year,/*if value in single digit then convert into double digit*/ DateTime.UtcNow.Month < 10 ? "0" + DateTime.UtcNow.Month.ToString() : DateTime.UtcNow.Month.ToString(), /*if value in single digit then convert into double digit*/DateTime.UtcNow.Day < 10 ? "0" + DateTime.UtcNow.Day.ToString() : DateTime.UtcNow.Day.ToString());

            if (totalJobDetailRows >= 99999)
            {
                totalJobDetailRows = 1;
            }

            var jobId = string.Format("{0}-{1}-{2}", memberAccountingCode, currentDate, totalJobDetailRows);

            return jobId;
        }

        /// <summary>
        /// Get all archive items list
        /// </summary>
        /// <param name="archiveIdList">List of archives Id</param>
        /// <returns>LegalArchiveLog list</returns>
        private IEnumerable<LegalArchiveLog> GetArchiveItemList(List<string> archiveIdList)
        {
            //convert all id from string type into guid type 
            var archiveIds = archiveIdList.ConvertAll(id => id.ToGuid());

            IEnumerable<LegalArchiveLog> archiveLogs =
                LegalArchiveLogRepository.GetAll().Where(
                    log => archiveIds.Contains(log.Id)).ToList();

            return archiveLogs;
        }

        /// <summary>
        /// Following method will insert Member profile Update message to Oracle queue
        /// </summary>
        /// <param name="invoiceId">archive invoice id</param>
        /// <param name="iua">iua of archive log</param>
        /// <param name="invoiceType">invoice type</param>
        /// <param name="jobSummaryId">job summary id</param>
        /// <param name="memberId">member id</param>
        private void InsertMessageInOracleQueue(Guid invoiceId, string iua, string invoiceType, Guid jobSummaryId, int memberId)
        {
            try
            {
                IDictionary<string, string> messages = new Dictionary<string, string> {
                                                                              { "JOB_SUMMARY_ID", ConvertUtil.ConvertGuidToString(jobSummaryId) },
                                                                              { "INVOICE_ID", ConvertUtil.ConvertGuidToString(invoiceId) },
                                                                              { "INVOICE_TYPE",invoiceType},
                                                                              { "ARCHIVE_ID", iua }
                                                                            };

                var queueHelper = new QueueHelper(ConfigurationManager.AppSettings["LegalArchiveQueueName"].Trim());
                queueHelper.Enqueue(messages);
            }
            catch (Exception exception)
            {
                Logger.Error("Error occurred while adding message to queue.", exception);
                SendUnexpectedErrorNotificationToISAdmin("Retrive Archive Queue", exception.Message, memberId);
            }

        }

        /// <summary>
        /// To send email notification to IS admin about unexpected errror.
        /// </summary>
        /// <param name="erroMessage">"error message"</param>
        /// <param name="serviceName">"service name"</param>
        /// <param name="memberId">"member id"</param>
        /// <returns>bool</returns>
        public bool SendUnexpectedErrorNotificationToISAdmin(string serviceName, string erroMessage, int memberId)
        {
            try
            {
                var memberName = "";

                //declare an object of the nVelocity data dictionary
                VelocityContext context;
                //get an object of the EmailSender component
                var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
                List<MailAddress> isOpsMailAdressList = null;
                //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nVelocity template
                var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
                //get an instance of email settings  repository
                var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));


                var memberData = MemberRepository.Single(mem => mem.Id == memberId);

                _logger.Info("Getting Member data for sending email");

                if (memberData != null)
                {
                    memberName = memberData.MemberCodeNumeric + "-" + memberData.MemberCodeAlpha;
                }


                //object of the nVelocity data dictionary
                context = new VelocityContext();
                context.Put("ServiceName", serviceName);
                context.Put("ErrorMessage", erroMessage);
                context.Put("MemberName", memberName);
                var emailSettingForMemberCreation = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.ISAdminNotificationForUnexpectedError);
               
                //generate email body text for own profile updates contact type mail
                var emailToISAdminText = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.ISAdminNotificationForUnexpectedError, context);
                //create a mail object to send mail
                var msgForMemberCreation = new MailMessage { From = new MailAddress(emailSettingForMemberCreation.SingleOrDefault().FromEmailAddress), IsBodyHtml = true };

                if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail))
                {
                    var emailAddressList = AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail;
                    var formatedEmailList = emailAddressList.Replace(',', ';');
                    isOpsMailAdressList = ConvertUtil.ConvertToMailAddresses(formatedEmailList);

                    foreach (var mailaddr in isOpsMailAdressList)
                    {
                        msgForMemberCreation.To.Add(mailaddr);
                    }

                }

                var subject = emailSettingForMemberCreation.SingleOrDefault().Subject.Replace("$ServiceName$", serviceName).Replace("$MemberName$", memberName);

                msgForMemberCreation.Subject = subject;

                //set body text of mail
                msgForMemberCreation.Body = emailToISAdminText;

                // send the mail.
                emailSender.Send(msgForMemberCreation);

                return true;
            }
            catch (Exception exception)
            {
                _logger.Error("Error occurred in unexprected error Notification to IS admin Email Handler (Send Mails for a Single Member method).", exception);
                return false;
            }
        }
    }
}
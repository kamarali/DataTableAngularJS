using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using Castle.Core.Smtp;
using Iata.IS.AdminSystem;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using log4net;
using System.Reflection;
using Iata.IS.Model.LegalArchive;
using Iata.IS.Data;
using Iata.IS.Data.LegalArchive;
using Iata.IS.Model.Enums;
using NVelocity;
using Iata.IS.Business.MemberProfile;

namespace Iata.IS.Business.LegalArchive.Impl
{
    public class RetrievalManager : IRetrievalManager
    {
        #region Private  Member
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private RetrievalJobDetails _archieve = null;

        #endregion
       
        #region Properties
        public IRetrievalRepository RetrievalRepository { get; set; }
        public ILegalArchiveRetrievalJobSummaryRepository LegalArchiveRetrievalJobSummaryRepository { get; set; }
        public ILegalArchiveRetrievalJobDetailsRepository LegalArchiveRetrievalJobDetailsRepository { get; set; }
        public IRepository<InvoiceBase> InvoiceBaseRepository { get; set; }
        public IArchivalProcessing _archieveProcessing { get; set; }
        public ITemplatedTextGenerator TemplatedTextGenerator { get; set; }
       
        #endregion
       
       
      

        /// <summary>
        /// Legal Archive Retrieval Process
        /// </summary>
        /// <param name="invId"></param>
        /// <param name="archieveId"></param>
        /// <param name="jobSumId"></param>
        public void LegalArchiveRetrievalProcess(string invId, string archieveId, string jobSumId)
        {
            _logger.InfoFormat("Legal Archive Retrieval Process started...");

            Guid invoiceId = System.Guid.Empty;
            bool isResponseSuccess = true;
            FileStream fileStream = null;
            HttpWebResponse response = null;
            Stream responseStream = null;
            string archieveJobId = string.Empty;
            string _responseCode = string.Empty;

            invoiceId = ConvertUtil.ConvertStringtoGuid(invId);
            var jobSummaryId = ConvertUtil.ConvertStringtoGuid(jobSumId);


            try
            {
                string _rootPath = FileIo.GetForlderPath(SFRFolderPath.LARetrieved);
              
                LegalArchiveRetrievalJobDetailsRepository = Ioc.Resolve<ILegalArchiveRetrievalJobDetailsRepository>();
                LegalArchiveRetrievalJobSummaryRepository = Ioc.Resolve<ILegalArchiveRetrievalJobSummaryRepository>();
                _archieveProcessing = Ioc.Resolve<IArchivalProcessing>();

               
               
                // Get job Summary details
                var jobSummaryDetails = GetJobSummaryDetails(jobSummaryId);

                if (jobSummaryDetails != null)
                {
                    archieveJobId = jobSummaryDetails.Arcretrievaljobid;
                    _logger.InfoFormat("Job Summary details retrieved successfully from database.");
                }
                else
                {
                    _logger.Error("Error occured while retrieving Legal archieve retrieval job summary object from database.");
                    throw new Exception("Error occured while retrieving Legal archieve retrieval job summary object from database.");
                }

                // Retrieve Archieve entry from database
                _archieve = GetArchieveDetailsRecord(invoiceId, jobSummaryId, archieveId);

                if (_archieve == null)
                {
                    _logger.Error("Error occured while retrieving Legal archieve retrieval job details object from database.");
                    throw new Exception("Error occured while retrieving Legal archieve retrieval job details object from database.");
                }

                // Retrieve Base url from System parameters
                string baseUrl = SystemParameters.Instance.LegalArchivingDetails.CDCArkhineoBaseURL;
                string archieveRetrievalUserName = SystemParameters.Instance.LegalArchivingDetails.ArchieveRetrievalUserName;
                string archieveRetrievalPassword = SystemParameters.Instance.LegalArchivingDetails.ArchieveRetrievalPassword;

                // Create url for archive retrieval
                //string requestUrl = baseUrl + String.Format(ConfigurationSettings.AppSettings["RetrievalUrl"].ToString(), archieveId);
                string requestUrl = baseUrl + String.Format("/cfes/archives/{0}/data-object", archieveId);
                
                // Log info before creating Http web request
                _logger.InfoFormat("Creating Http Web request for Archieve Id {0}.", archieveId);

                // Log info before creating Http web request
                _logger.InfoFormat("Login Credential ==> UserId='{0}' : Password ='{1}'.", archieveRetrievalUserName, archieveRetrievalPassword);


                // Create Http web request for archieve retrieval
                var request = CreateHttpWebRequestForArchieveRetrieval(requestUrl, archieveRetrievalUserName, archieveRetrievalPassword);

                // Log info after creating Http web request
                _logger.InfoFormat("Http Web request created for Archieve Id {0}.", archieveId);

                // Log info before getting Http web response
                _logger.InfoFormat("Get Http Web response for Archieve Id {0}.", archieveId);

                try
                {
                    // Call GetResponse() which returns file bytes 
                    response = (HttpWebResponse)request.GetResponse();
                    // Get response stream byte array
                    responseStream = response.GetResponseStream();
                    // Get Webservice response code
                    _responseCode = response.StatusDescription;
                    _logger.InfoFormat("ResponseCode: {0}.", _responseCode);

                }
                catch (Exception ex)
                {
                    _logger.InfoFormat("Error in Web Response : {0}" ,ex);

                    var context = new VelocityContext();
                    // Send email if exception occured while retrieving response
                    SendMail(SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail, EmailTemplateId.LegalInvoiceArchiveRetrievalWebServiceInitializeFailureAlert, context);
                    isResponseSuccess = false;
                    throw;
                }

                // Log info after getting Http web response
                _logger.InfoFormat("Http Web response receieved successfully for Archieve Id {0}.", archieveId);

                // Get path to store retrieved Zip file
                string directoryPath = _rootPath + archieveJobId;
                

                // Log info before creating Directory for specified Job Id
                _logger.InfoFormat("Creating folder to store Legal Archieve file retrieved for archieve Id {0}.", archieveId);

                // Retrieve filename of returned file
                var content = response.Headers.Get("Content-Disposition").ToString();
                string fileName = content.Substring(content.IndexOf('"') + 1, (content.LastIndexOf('"') - content.IndexOf('"')) - 1);

                // Check whether Directory exists for specified Archieve Job Id, if not create Directory for specified job Id and save the zip file
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Log info after creating Directory for specified Job Id
                _logger.InfoFormat("Created folder successfully to store Legal Archieve file retrieved for archieve Id {0}.", archieveId);

                // Create new file with Write access mode
                fileStream = new FileStream(directoryPath + "\\" + fileName, FileMode.OpenOrCreate, FileAccess.Write);
                // Declare Byte array
                var buff = new byte[102400];
                // Declare count to read byte array
                int count = 0;

                // Loop through Byte array and write contents to file
                while ((count = responseStream.Read(buff, 0, 10400)) > 0)
                {
                    fileStream.Write(buff, 0, count);
                    fileStream.Flush();
                }

                // Log info after creating file from Byte stream
                _logger.InfoFormat("File saved successfully at specified position for archieve Id {0}.", archieveId);

                _logger.Info("jobSummaryId :--" + jobSummaryId);

                // If response is successfully returned, Update job details record with file details and set Status as Success. 
                var retrievalJobDetailsList = _archieveProcessing.UpdateArchieveRetrievalJobDetailsRecord(response.StatusDescription, fileName, directoryPath, "S", archieveId, jobSummaryId);
               
                // Add Success log details
                _logger.Info("Legal Archieve Retrieval job details record updated with file details and marked status as Success.");
                _logger.Info("Legal Archieve Retrieval service processing completed.");

              
                // Get retrieval details list
               // var retrievalJobDetailsList = GetRetrievalJobDetailsList(jobSummaryId);
               
                foreach (var retrievalJobDetailse in retrievalJobDetailsList)
                {
                    _logger.Info("RETRIVE STATUS :--" + retrievalJobDetailse.Status);
                }
                // Check whether retrieval job details list has records, if not throw an exception
                if (retrievalJobDetailsList != null)
                {
                    // Check whether all detail records within Job summary are Processed, if yes check for retrieval status  
                    if (retrievalJobDetailsList.Where(r => r.Status == "P").Count() == 0)
                    {
                        // Get job details count with failure status
                        var failureCount = retrievalJobDetailsList.Where(r => r.Status == "F").Count();

                        // If failure count == 0, set Job summary status as "COMPLETE", else set it as "INCOMPLETE"
                        if (failureCount == 0)
                        {
                            _archieveProcessing.UpdateArchieveRetrievalJobSummaryRecord("COMPLETE", jobSummaryId);
                            _logger.Info("Legal Archieve Retrieval job Summary record updated with status as COMPLETE.");
                        }
                        else
                        {
                            _archieveProcessing.UpdateArchieveRetrievalJobSummaryRecord("INCOMPLETE", jobSummaryId);
                            _logger.Info("Legal Archieve Retrieval job Summary record updated with status as INCOMPLETE.");
                        }
                    }
                    else
                    {
                        _logger.Info("Retrieval status 'P' count is not zero!!!");
                    }
                }
                else
                {
                    _logger.Error("Error occured while retrieving Legal archieve retrieval job details list from database.");
                    throw new Exception("Error occured while retrieving Legal archieve retrieval job details list from database.");
                }
            }
            catch (Exception ex)
            {
                if (isResponseSuccess)
                {
                    //SCP#491878 - SRM - Email alert without any details received in SIS OPS mailbox
                    var _memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
                    // Retrieve Invoice details
                    var invoice = InvoiceBaseRepository.Single(i => i.Id == invoiceId);
                   
                    //SCP#491878 - SRM - Email alert without any details received in SIS OPS mailbox 
                    var billingMemberDetails = _memberManager.GetMemberDetails(invoice.BillingMemberId);
                    string billingMemberText = (billingMemberDetails!=null) ? string.Format("{0}-{1}", billingMemberDetails.MemberCodeAlpha, billingMemberDetails.MemberCodeNumeric):null;
                    var billedMemberDetails = _memberManager.GetMemberDetails(invoice.BilledMemberId);
                    string billedMemberText = (billedMemberDetails != null) ? string.Format("{0}-{1}", billedMemberDetails.MemberCodeAlpha, billedMemberDetails.MemberCodeNumeric) : null;
                    
                    // Send failure email if response has receieved and exception occured in processing the response.
                    _logger.InfoFormat("Invoice Detail => BillingMemberText: {0}, BilledMemberText : {1}, BillingPeriod: {2}, BillingCategory : {3}, InvoiceNumber:{4}", billingMemberText, billedMemberText, invoice.DisplayBillingPeriod, invoice.BillingCategory, invoice.InvoiceNumber);
                    SendFailureAlert(billingMemberText, billedMemberText, invoice.DisplayBillingPeriod, invoice.BillingCategory, invoice.InvoiceNumber, _responseCode);
                   
                }

                if (_archieve != null)
                {
                    _archieveProcessing.UpdateArchieveRetrievalJobDetailsRecord(_responseCode, string.Empty, string.Empty, "F", _archieve.Iua, jobSummaryId);
                   
                    _archieveProcessing.UpdateArchieveRetrievalJobSummaryRecord("INCOMPLETE", jobSummaryId);
                    _logger.Info("Legal Archieve Retrieval job Summary record updated with status as INCOMPLETE.");
                }

                _logger.Error("Error while processing message from the queue.", ex);
            }
            finally
            {
                if (fileStream != null)
                {
                    // Close file stream
                    fileStream.Close();
                }

                if (responseStream != null)
                {
                    // Close response stream
                    responseStream.Close();
                }
            }
        }


        /// <summary>
        /// Get all job details based on summary id 
        /// </summary>
        /// <param name="jobSummaryId"> job summary id</param>
        /// <param name="imgAltText">image alt text</param>
        /// <param name="imgToolTip">image tool tip</param>
        /// <param name="imgPath">image display url</param>
        /// <returns>list of job details</returns>
        public List<RetrivalJobDetailGrid> GetJobDetailsByJobSummaryId(Guid jobSummaryId, string imgAltText, string imgToolTip, string imgPath)
        {
            var jobDetails = RetrievalRepository.GetRetrievalJobDetailsByJobSummaryId(jobSummaryId);
            foreach (var item in jobDetails)
            {
                //SCP442581 - legal archive in SIS
                item.Action =  string.Format("{0}${1}${2}", item.ZipFile, item.RetrievalJobId, jobSummaryId);
                item.IsFileExist = IsFileExist(item.RetrievalJobId, item.ZipFile);
            }

            return jobDetails;
        }

        /// <summary>
        /// Get all job summary based on member id 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="loggedInUserId"></param>
        /// <returns>list of job summary</returns>
        public List<RetrivalJobSummaryGrid> GetRetrievedJobs(int memberId, int loggedInUserId)
        {
            var jobSummaryList = RetrievalRepository.GetRetrievalJobSummaryByMemberId(memberId, loggedInUserId);
            if (jobSummaryList.Count > 0)
            {
                foreach (var item in jobSummaryList)
                {
                    item.BillingPeriodText = item.BillingPeriod < 0 ? "All" : item.BillingPeriod.ToString();
                }
            }

            return jobSummaryList;
        }

        /// <summary>
        /// Get first job summary id from list of job details for default selected
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="loggedInUserId"></param>
        /// <returns>summary Id</returns>
        public string GetJobSummaryId(int memberId, int loggedInUserId)
        {
            var jobSummaryList = RetrievalRepository.GetRetrievalJobSummaryByMemberId(memberId, loggedInUserId);
            return jobSummaryList.Count > 0 ? jobSummaryList.First().Id.ToString() : Guid.Empty.ToString();
        }

        /// <summary>
        /// Check if file is archived or not 
        /// </summary>
        /// <param name="jobId">archive log job id</param>
        /// <param name="zipFileName">archive file name</param>
        /// <returns>true if exist else false</returns>
        private bool IsFileExist(string jobId, string zipFileName)
        {
            if (string.IsNullOrEmpty(jobId) || string.IsNullOrEmpty(zipFileName))
            {
                return false;
            }
            else
            {
                var rootFolder = FileIo.GetForlderPath(SFRFolderPath.LARetrieved);
                var fileFullPath = string.Format(@"{0}{1}\{2}", rootFolder, jobId, zipFileName);
                var fileInfo = new FileInfo(fileFullPath);
                return fileInfo.Exists;
            }
        }

        /// <summary>
        /// Following method returns job summary details for given job summary Id
        /// </summary>
        /// <param name="jobSummaryId">Job Summary Id</param>
        /// <returns>Job Summary details</returns>
        public RetrievalJobSummary GetJobSummaryDetails(Guid jobSummaryId)
        {
            var jobSummary = LegalArchiveRetrievalJobSummaryRepository.Single(b => b.Id == jobSummaryId);
            if (jobSummary != null)
                return jobSummary;
            else
                return null;
        }

        /// <summary>
        /// Following method returns Archieve details object for given archieve Id
        /// </summary>
        /// <param name="invoiceID">invoiceID</param>
        ///  <param name="jobSummaryId">jobSummaryId</param>
        ///  <param name="IUA">IUA</param>
        /// <returns>Archieve details</returns>
        public RetrievalJobDetails GetArchieveDetailsRecord(Guid invoiceID,Guid jobSummaryId,string  IUA)
        {
            var archieveRecord = LegalArchiveRetrievalJobDetailsRepository.Single(b => b.RetrievalJobSummaryId == jobSummaryId && b.Invoiceid == invoiceID && b.Iua == IUA);
            if (archieveRecord != null)
                return archieveRecord;
            else
                return null;
        }

        /// <summary>
        /// Create Http Web request for archieve retrieval
        /// </summary>
        /// <param name="requestUrl">Web request url</param>
        /// <param name="userName">Web request username</param>
        /// <param name="password">Web request password</param>
        /// <returns>Http web request object</returns>
        public HttpWebRequest CreateHttpWebRequestForArchieveRetrieval(string requestUrl, string userName, string password)
        {
            // Create Http webrequest
            var request = (HttpWebRequest)WebRequest.Create(requestUrl);
            // Set request parameters
            request.Method = "GET";
            request.ContentType = "application/zip";
            request.Credentials = new NetworkCredential(userName, password);
            request.Accept = "*/*";

            return request;
        }

        /// <summary>
        /// Get retrieval job details list for given job summary Id.
        /// </summary>
        /// <param name="jobSummaryId">Job summary Id</param>
        /// <returns>Archieve retrieval job details list</returns>
        public List<RetrievalJobDetails> GetRetrievalJobDetailsList(Guid jobSummaryId)
        {
            var retrievalJobDetailsList = LegalArchiveRetrievalJobDetailsRepository.Get(r => r.RetrievalJobSummaryId == jobSummaryId);

            if (retrievalJobDetailsList != null)
                return retrievalJobDetailsList.ToList();
            else
                return null;
        }


        /// <summary>
        /// Sends the failure alert.
        /// </summary>
        public void SendFailureAlert(string billingMember, string billedMember, string billingPeriod, BillingCategoryType billingCategory, string invoiceNumber, string errorCode)
        {
            // Create an object of the nVelocity data dictionary
            var context = new VelocityContext();
            context.Put("BillingMember", billingMember);
            context.Put("BilledMember", billedMember);
            context.Put("BillingPeriod", billingPeriod);
            context.Put("BillingCategory", billingCategory);
            context.Put("InvoiceNumber", invoiceNumber);
            context.Put("ErrorCode", errorCode);

            SendMail(SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail, EmailTemplateId.LegalInvoiceArchiveRetrievalWebServiceResponseFailureAlert, context);
            _logger.Info("Sent IS-Admin alert");
        }

        private void SendMail(string emailAddress, EmailTemplateId emailTemplateId, VelocityContext context)
        {
            try
            {
                // Get an object of the EmailSender component
                var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
                // Get an instance of email settings repository
                var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
                // Verify and log TemplatedTextGenerator for null value.
                _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

                _logger.Info("Get Email Template ID :" + (int)emailTemplateId);
                var emailSettingForISAdminAlert = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)emailTemplateId);

                _logger.Info("Email Template :" + emailSettingForISAdminAlert.First());

                // Generate email body text for own profile updates contact type mail
                if (TemplatedTextGenerator != null)
                {

                  
                    var emailToISAdminText = TemplatedTextGenerator.GenerateTemplatedText(emailTemplateId, context);
                    // Create a mail object to send mail
                    var msgForISAdmin = new MailMessage
                    {
                        From = new MailAddress(emailSettingForISAdminAlert.SingleOrDefault().FromEmailAddress),
                        IsBodyHtml = true
                    };

                    var subject = emailSettingForISAdminAlert.SingleOrDefault().Subject;
                    _logger.Info("Email subject : " + subject);
           
                    msgForISAdmin.Subject = subject;


                    // loop through the contacts list and add them to To list of mail to be sent
                    if (!string.IsNullOrEmpty(emailAddress))
                    {
                        var mailAdressList = ConvertUtil.ConvertToMailAddresses(emailAddress);

                       
                        foreach (var mailaddr in mailAdressList)
                        {
                            msgForISAdmin.To.Add(mailaddr);
                            _logger.Info("Email Adress : " + mailaddr.Address);
           
                        }
                    }

                    _logger.Info("Email Body : " + emailToISAdminText);
                    //set body text of mail
                    msgForISAdmin.Body = emailToISAdminText;

                    _logger.Info("Sending Email.... ");
                    //send the mail
                    emailSender.Send(msgForISAdmin);

                }
            }// End try
            catch (Exception exception)
            {
                _logger.Error("Error occurred while sending an email to IS-Admin.", exception);
                return;
            }

        }

    }
}

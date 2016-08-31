using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Castle.Core.Smtp;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Common;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.MemberProfile.Impl;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Data.LegalArchive;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.LegalArchive;
using Iata.IS.Model.Reports.Enums;
using log4net;
using Iata.IS.Business.MemberProfile;
using NVelocity;


namespace Iata.IS.Business.LegalArchive.Impl
{
    public class ArchivalManager : IArchivalManager
    {
        #region Private  Member

        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const int EinvoiceFolderTypeId = 1;
        private const int ListingsFolderTypeId = 2;
        private const string EinvoicesFolderName = "E-INVOICE";
        private const string ListingsFolderName = "LISTINGS";
        #endregion

        #region Properties

        public ICalendarManager CalendarManager { get; set; }

        private IArchivalProcessing ArchivalProcessing { get; set; }

        public IRepository<LegalArchiveLog> LegalArchiveLogRepository { get; set; }

        public IMemberManager MemberManager { get; set; }

        public IRepository<InvoiceBase> InvoiceBaseRepository { get; set; }

        public IRepository<InvoiceOfflineCollectionMetaData> InvoiceOfflineCollectionMetaDataRepository { get; set; }

        public ITemplatedTextGenerator TemplatedTextGenerator { get; set; }

        #endregion


        public ArchivalManager(IArchivalProcessing queueInvoicesForArchive)
        {
            ArchivalProcessing = queueInvoicesForArchive;
        }


        public void QueueInvoicesForArchive(int period, int billingMonth, int billingYear, bool isReArchive = false)
        {
            try
            {
                _logger.Debug(String.Format("Year: {0}, Month: {1}, Period: {2}",
                                              billingYear, billingMonth, period));

                _logger.Info(String.Format("Year: {0}, Month: {1}, Period: {2}",
                                              billingYear, billingMonth, period));

                // Queue Invoices for Legal Archive for last completed billing period
                //Set False for ReArchive parameter
                ArchivalProcessing.QueueInvForArchive(billingYear, billingMonth, period, isReArchive);
            }
            catch (Exception ex)
            {

                _logger.Error("Error occured while Queue Invoices For Archive", ex);
                // throw new ISBusinessException(ErrorCodes.ZeroTaxBreakdownRecords, "Error occurred while sending SIS Webservice Error Notification to IS Administrator");

            }

        }

        private void ReQueueInvoiceForArchive(Guid invoiceId, int invoiceType, int delayInDequeue)
        {
            try
            {
                _logger.Debug(String.Format("Re-Queue Invoice ID : {0} , Invoice Type: {1} with delay time {2} sec", ConvertUtil.ConvertGuidToString(invoiceId), invoiceType, delayInDequeue));

                _logger.Info(String.Format("Re-Queue Invoice ID : {0} , Invoice Type: {1} with delay time {2} sec", ConvertUtil.ConvertGuidToString(invoiceId), invoiceType, delayInDequeue));

                ArchivalProcessing.ReQueueInvoiceForArchive(invoiceId, invoiceType, delayInDequeue);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occured while Re-Queue Invoices For Archive", ex);
                //SCP400947-SRM: Legal archiving pending for July P2
                throw;
            }

        }

        public bool LegalArchivalProccess(Guid invoiceId, int archiveType)
        {
            _logger.Info("Legal Archival Submission Proccess started successfully for Invoice ID : " + ConvertUtil.ConvertGuidToString(invoiceId));

            string archivalStatus = string.Empty;

            var isEligibleToCleanup = false;

            try
            {
            	var createdZipFilePath = string.Empty;
            	var applicativeMetadata = string.Empty;


            	//AddArchivalLogRecords
            	_logger.Info("Adding Archive Log Record in database Started.");
            	var legalArchiveLog = AddArchiveLogRecord(invoiceId, archiveType);
            	//231363: SRM: CDC.
            	//Get member detail based on member id.
            	var memberDetail = MemberManager.GetMemberDetails(legalArchiveLog.ArcCustIsId);

            	//231363: SRM: CDC.
            	//Desc: if compartment is not present for the member, we will not send invoices for legal archive to CDC 
            	//and we will update deposit status as 'AB' of invoices.)
            	if (!String.IsNullOrEmpty(memberDetail.CdcCompartmentIDforInv) && memberDetail.LegalArchivingRequired)
            	{
            		// formApplicationMetadata
            		_logger.Info("Creating Form Applicative Metadata.");

            		applicativeMetadata = FormApplicativeMetadata(legalArchiveLog);

            		_logger.InfoFormat("Applicative Metadata XML ==>> {0} ", applicativeMetadata);

            		//Create Zip File..(It will return empty string if no zip file created else will return File Path)
            		try
            		{
            			_logger.Info("Creation of  Archive Zip File started.");
            			createdZipFilePath = CreateArchiveZipFile(invoiceId, archiveType,
            			                                          legalArchiveLog.ListingIncluded == 1 ? true : false);

            		}
            		catch (Exception ex)
            		{
            			_logger.Error("Error occured in Zip file creation", ex);

            			archivalStatus = "FW";
            			// Send an email to IS-Admin in case there was failure in creating Archive Zip File 
            			SendMailArchivalZipFileCreationError(legalArchiveLog, ex.Message);
            		  return false;
            		}


            		if (createdZipFilePath.Length != 0 && applicativeMetadata.Length != 0)
            		{
            			_logger.Info("Archive Zip File Created and now calling web service to submit data");
            			//call Web API to Submit Zip File
            			var responseString = SubmitZipFile(applicativeMetadata, createdZipFilePath,
            			                                   legalArchiveLog.CdcCompartmentId);

            			_logger.InfoFormat("Web Service Response: {0}", responseString);

            			// for  Testing Responsestring 
            			// responseString = "<?xml version='1.0' encoding='UTF-8'?><technical-ra xmlns='http://www.arkhineo.fr/CFE/metadata/1.1' archive-id='20120106054702277ATRleOESFSOAXifUFSGAA0Qn' deposit-date='2012-01-06 05:47:02'><digests><applicative-metadata-digest algorithm='http://www.w3.org/2001/04/xmlenc#sha256'>tcxDxCzkoYcx5EWmYObZNJYf+BnvHwZ8FSXLJ15dQeU=</applicative-metadata-digest><data-object-digest algorithm='http://www.w3.org/2001/04/xmlenc#sha256'>s/ERbAhtJoU+N2ZbJhuVEyimxJYJ3ewGeFzY71WBf8k=</data-object-digest><descriptive-metadata-digest algorithm='http://www.w3.org/2001/04/xmlenc#sha256'>udxPrJ+aOECJ5MNCKnk7DwhIyC9Eht6+5cdS/sQnSvk=</descriptive-metadata-digest></digests></technical-ra>";

            			//Logic here for retry attempt for failed submission
            			if (responseString == "Failed")
            			{
            				for (int i = 1; i <= SystemParameters.Instance.LegalArchivingDetails.RetryAttemptforLA; i++)
            				{
            					_logger.InfoFormat("Retry Attempt for resubmission  {0} ......", i);
            					responseString = SubmitZipFile(applicativeMetadata, createdZipFilePath,
            					                               legalArchiveLog.CdcCompartmentId);

            					if (responseString != "Failed")
            					{
            						_logger.InfoFormat("Retry Attempt {0} passed.", i);
            						_logger.InfoFormat("Web Service Response for Attempt {0}: {1}", i, responseString);
            						break;
            					}
            					_logger.InfoFormat("Retry Attempt {0} failed.", i);

            					_logger.InfoFormat("Web Service Response for Attempt {0}: {1}", i, responseString);
            				}
            			}

            			if (responseString == "Failed")
            			{
                    //SCP400947-SRM: Legal archiving pending for July P2
                    try
                    {
                      // Impliment here logic for Re-Queue of same invoice with delay De-queuing
                      ReQueueInvoiceForArchive(invoiceId, archiveType,
                                               SystemParameters.Instance.LegalArchivingDetails.DelayInQueue);

                      legalArchiveLog.DepositStatus = "FW"; // Valid values "FW/SW------/FA/SA";
                      archivalStatus = "FW";

                      //SCP#461308: KAL- Alert for error in LA Deposit not sent to SIS Ops
                      //Send an email to IS-Admin in case there was failure in depositing item corresponding to passed LegalArchiveID
                      SendArchivalFailure(legalArchiveLog, responseString);

                      //   UpdateInvoiceForFailedSubmission(legalArchiveLog.InvoiceId, recPayIndicator, "FW");
                      _logger.InfoFormat("Deposit Status Updated for Failed Deposit of Invoice No: {0}",
                                         legalArchiveLog.InvoiceNumber);
                    }
                    catch (Exception ex)
                    {
                      throw;
                    }

            			  return false;
            			}
            			else
            			{
            				// Parse the response received as XElement.
            				XElement response = XElement.Parse(responseString);

            				// XElement innerNode = response.Element("technical-ra");
            				if (response.HasAttributes)
            				{
            					try
            					{
            						// Update invoice table 
            						archivalStatus = "SW";
            						//UpdateInvoiceForFailedSubmission(legalArchiveLog, "SW");
            						_logger.InfoFormat("Deposit Status Updated for Succesfull Deposit of Invoice No: {0}",
            						                   legalArchiveLog.InvoiceNumber);



            						legalArchiveLog.Iua = response.Attribute("archive-id").Value;

            						_logger.InfoFormat("DepositRequestDateTime: {0}", response.Attribute("deposit-date").Value);

            						var depositReDateTime = response.Attribute("deposit-date").Value.Substring(0,
            						                                                                           (response.Attribute(
            						                                                                           	"deposit-date").Value.
            						                                                                           	IndexOf(",")));

            						_logger.InfoFormat("DepositRequestDateTime: {0}", depositReDateTime);

            						legalArchiveLog.DepositRequestDateTime = Convert.ToDateTime(depositReDateTime);
            						legalArchiveLog.DepositStatus = "SW"; // Valid values "FW/SW------/FA/SA"

            						//Get Zip file size in kilo bytes,rounded to the next higher value, e.g. 102.874/102.875/102.876 will be rounded to 102.88
            						FileInfo f = new FileInfo(createdZipFilePath);
            						if (f.Exists)
            						{
            							_logger.InfoFormat("Zip File Size in  Bytes {0}", f.Length);

            							var filesizeKb = (decimal) f.Length/1024;

            							_logger.InfoFormat("Zip File Size in  KB  {0}", filesizeKb);

            							legalArchiveLog.ZippedFileArcSize = Math.Ceiling(filesizeKb*100)/100;

            							_logger.InfoFormat("Round off Zip File Size in  KB  {0}", legalArchiveLog.ZippedFileArcSize);
            						}
            						legalArchiveLog.ZippedFileArchived = createdZipFilePath; // "ZipFileName include extension";
            						legalArchiveLog.WebserviceResponseCodeText = responseString;

            					  isEligibleToCleanup = true;
            					}
            					catch (Exception ex)
            					{
            						legalArchiveLog.DepositStatus = "FW"; // Valid values "FW/SW------/FA/SA";

                        // Send an email to IS-Admin in case there was failure in creating Archive Zip File 
                        SendMailArchivalZipFileCreationError(legalArchiveLog, ex.Message);

            						_logger.Info("Error in parsing response XML : ", ex);
            					}

            				}
            				else
            				{
            					legalArchiveLog.DepositStatus = "FW"; // Valid values "FW/SW------/FA/SA";

            					//Send an email to IS-Admin in case there was failure in depositing item corresponding to passed LegalArchiveID
            					SendArchivalFailure(legalArchiveLog, responseString);
            				  return false;
            				}
            			}

            			// UpdateArchivalResponse
            			var isUpdate = UpdateArchivalResponse(legalArchiveLog);
            			if (isUpdate)
            			{
            				_logger.Info("Legal Archival Submission Proccess completed for Invoice ID : " + ConvertUtil.ConvertGuidToString(invoiceId));
                    // SCP343641: SIS: Admin Alert - Error creating Legal Invoice Archive zip file of CDC SIS Prod - 28feb2015
                    _logger.InfoFormat("Start of Cleanup process. Flag isEligibleToCleanup: {0}", isEligibleToCleanup);
                    if(isEligibleToCleanup)
                    {
                      _logger.InfoFormat("Attempting to delete Zip file after successful deposit.");
                      if (File.Exists(createdZipFilePath))
                      {
                        _logger.InfoFormat("Zip File exists at location: {0}", createdZipFilePath);
                        try
                        {
                          File.Delete(createdZipFilePath);
                          _logger.InfoFormat("Zip File deleted successfully from location: {0}", createdZipFilePath);
                        }
                        catch (Exception exception)
                        {
                          _logger.InfoFormat("Error in deleting Zip File from location {0} : {1}", createdZipFilePath, exception.StackTrace);
                        }
                      }
                      else
                      {
                        _logger.InfoFormat("Zip File does not exists at location: {0}", createdZipFilePath);
                      }
                    }
                    _logger.InfoFormat("End of Cleanup process.");
            			}
            		}
            	}
            	else
            	{
            		legalArchiveLog.DepositStatus = "AB";
            		archivalStatus = "AB";

            		UpdateArchivalResponse(legalArchiveLog);
                _logger.Info(string.Format("Legal Archiving failed due to compartment id is missing for Invoice ID: {0} and set archivalStatus = AB ", ConvertUtil.ConvertGuidToString(invoiceId)));
            	}
            }
            catch (Exception ex)
            {
              //SCP400947-SRM: Legal archiving pending for July P2
              try
              {

                archivalStatus = "FW";

                // Impliment here logic for Re-Queue of same invoice with delay De-queuing
                ReQueueInvoiceForArchive(invoiceId, archiveType,
                                          SystemParameters.Instance.LegalArchivingDetails.DelayInQueue);

                _logger.Error("Error occured at the time of Legal Archival Proccess", ex);
              }
              catch (Exception exp)
              {
                _logger.Error("Error occured at the time of Re Queue Legal Archival", exp);
                throw;
              }
            }

            finally
            {
              try
              {
                UpdateInvoiceAfterArchival(invoiceId, archiveType, archivalStatus);

                if (archivalStatus == "SW")
                {
                  _logger.Info(string.Format("Legal Archiving completed successfully for Invoice ID: {0} and set archivalStatus = SW ", ConvertUtil.ConvertGuidToString(invoiceId)));
                }
              }
              catch (Exception ex)
              {
                _logger.Error("Error occured at the time of UpdateInvoiceAfterArchival", ex);
                throw;
              }
            }

            return true;
        }


        #region Used Methods


        /// <summary>
        /// This method will be called before calling deposit API of CDC webservice.
        /// It will create a new record in Legal_Archive_Log table and add values in columns 1 to 31 except columns 24 and 25.
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="archiveType"></param>
        private LegalArchiveLog AddArchiveLogRecord(Guid invoiceId, int archiveType)
        {
            try
            {
                // Get Parameter Values from System Parameter
                var cdcArkhineoClientIdOfIata = SystemParameters.Instance.LegalArchivingDetails.CDCArkhineoClientIDofIATA;
                var cdcArkhineoCoffreId = SystemParameters.Instance.LegalArchivingDetails.CDCArkhineoCoffreID;
                var cdcArkhineoSectionId = SystemParameters.Instance.LegalArchivingDetails.CDCArkhineoSectionID;

                // Add ArchivalLog Records
                var legalArchiveLog = ArchivalProcessing.AddArchivalLogRecord(invoiceId, archiveType, cdcArkhineoClientIdOfIata, cdcArkhineoCoffreId, cdcArkhineoSectionId);
                return legalArchiveLog;
            }
            catch (Exception ex)
            {
                _logger.Error("Error occured at the time of Adding Archival Log Record", ex);
                throw ;
            }

        }


        /// <summary>
        ///  It will form applicative metadata Xml as per Descriptive metadata document shared by CDC.
        /// </summary>
        /// <param name="legalArchiveLog"></param>
        /// <returns>XML String</returns>
        private string FormApplicativeMetadata(LegalArchiveLog legalArchiveLog)
        {
            var sbInputXml = new StringBuilder();
            try
            {

                // Create XML of Metadata
                sbInputXml.Append("<?xml version=\"1.0\" ?>");

                sbInputXml.Append("<app_metadata>");
                sbInputXml.Append("<ACDS>" + legalArchiveLog.ArcCustDesignator + "</ACDS>");
                sbInputXml.Append("<ACAC>" + legalArchiveLog.ArcCustAccounting + "</ACAC>");
                sbInputXml.Append("<ACID>" + legalArchiveLog.ArcCustIsId + "</ACID>");
                sbInputXml.Append("<CRDS>" + legalArchiveLog.CrBillingMemberDesignator + "</CRDS>");
                sbInputXml.Append("<CRAC>" + legalArchiveLog.CrBillingMemberAccounting + "</CRAC>");
                sbInputXml.Append("<CRID>" + legalArchiveLog.CrBillingMemberIsId + "</CRID>");
                sbInputXml.Append("<DRDS>" + legalArchiveLog.DbBilledMemberDesignator + "</DRDS>");
                sbInputXml.Append("<DRAC>" + legalArchiveLog.DbBilledMemberAccounting + "</DRAC>");
                sbInputXml.Append("<DRID>" + legalArchiveLog.DbBilledMemberIsId + "</DRID>");
                sbInputXml.Append("<BCAT>" + legalArchiveLog.BillingCategory + "</BCAT>");

                sbInputXml.Append("<RPIN>" + legalArchiveLog.ReceivablesPayablesIndicator + "</RPIN>");
                sbInputXml.Append("<IVNO>" + legalArchiveLog.InvoiceNumber + "</IVNO>");
                sbInputXml.Append("<IVID>" + legalArchiveLog.InvoiceId + "</IVID>");
                sbInputXml.Append("<IVTP>" + legalArchiveLog.InvoiceType + "</IVTP>");
                sbInputXml.Append("<BILY>" + legalArchiveLog.BillingYear + "</BILY>");
                sbInputXml.Append("<BILM>" + legalArchiveLog.BillingMonth + "</BILM>");
                sbInputXml.Append("<BILP>" + legalArchiveLog.BillingPeriod + "</BILP>");
                sbInputXml.Append("<IVDT>" + legalArchiveLog.InvoiceDate.ToString("yyyyMMdd") + "</IVDT>");
                sbInputXml.Append("<SMID>" + legalArchiveLog.SettlementIndicator + "</SMID>");

                sbInputXml.Append("<CRCC>" + legalArchiveLog.CrBillingMemberCountry + "</CRCC>");
                sbInputXml.Append("<DRCC>" + legalArchiveLog.DbBilledMemberCountry + "</DRCC>");

                sbInputXml.Append("</app_metadata>");


            }
            catch (Exception ex)
            {

                _logger.Error("Error occured at the time of Creating Applicative Metadata", ex);
            }
            return sbInputXml.ToString();
        }

        /// <summary>
        /// This method will update archival webservice response in database.
        /// </summary>
        /// <param name="legalArchiveLog"></param>
        /// <returns>True/False</returns>
        private bool UpdateArchivalResponse(LegalArchiveLog legalArchiveLog)
        {
            try
            {
                // Check whether legal Archive Log Data record already exists in database
                var legalArchiveLogData = LegalArchiveLogRepository.Single(la => la.Id == legalArchiveLog.Id);


                if (legalArchiveLogData != null)
                {
                    legalArchiveLogData.Iua = legalArchiveLog.Iua;
                    legalArchiveLogData.WebserviceResponseCodeText = legalArchiveLog.WebserviceResponseCodeText;
                    legalArchiveLogData.DepositStatus = legalArchiveLog.DepositStatus;
                    legalArchiveLogData.DepositRequestDateTime = legalArchiveLog.DepositRequestDateTime;
                    legalArchiveLogData.ZippedFileArchived = legalArchiveLog.ZippedFileArchived;
                    legalArchiveLogData.ZippedFileArcSize = legalArchiveLog.ZippedFileArcSize;

                    LegalArchiveLogRepository.Update(legalArchiveLogData);

                    // Commit 
                    UnitOfWork.CommitDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occured at the time of Updating Archival Response in database.", ex);
                throw;
            }

            return true;
        }

        private void UpdateInvoiceAfterArchival(Guid invoiceId, int recPayIndicator, string status)
        {
            try
            {
                // Check whether Invoice record already exists in database
                var invoiceData = InvoiceBaseRepository.Single(inv => inv.Id == invoiceId);


                if (invoiceData != null)
                {
                    if (recPayIndicator == 2)
                    {
                        invoiceData.DepositStatusBL = status;
                    }
                    else if (recPayIndicator == 1)
                    {
                        invoiceData.DepositStatusBD = status;
                    }

                    InvoiceBaseRepository.Update(invoiceData);

                    // Commit 
                    UnitOfWork.CommitDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occured at the time of Updating  Deposit status in Invoice Table in database.", ex);

            }


        }

        #region Create Zip File and get File Path
        /// <summary>
        /// It will Create Archival Zip File 
        /// Locate folder in where offline collection is generated for the invoice
        /// Copy required files from respective folder in offline collection folder of the invoice and store it in Given location 
        /// Throws the exception if any exception occure while generating the zip file 
        /// </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <param name="archiveType">2: Receivables 1: Payables</param>
        /// <param name="includeListings">Billing category</param>
        /// <returns>
        /// 1. SFR path of zip file: If zip file successfully created
        /// 2. Empty string : If no document is copied in the zip file and hence no zip is created 
        /// </returns>
        public string CreateArchiveZipFile(Guid invoiceId, int archiveType, bool includeListings)
        {
            var invoiceBase = InvoiceBaseRepository.Single(i => i.Id == invoiceId);

            if (invoiceBase != null)
            {
                //Check Digital Sign status first
                if (invoiceBase.DigitalSignatureStatus == DigitalSignatureStatus.Pending || invoiceBase.DigitalSignatureStatus == DigitalSignatureStatus.Requested || invoiceBase.DigitalSignatureStatus == DigitalSignatureStatus.Failed)
                {
                    _logger.Info("Digital Signature process is not completed for this invoice");
                    throw new Exception("Digital Signature process is not completed for this invoice");
                }

                var billedMemberCode = MemberManager.GetMemberCode(invoiceBase.BilledMemberId);
                var billingMemberCode = MemberManager.GetMemberCode(invoiceBase.BillingMemberId);

                var invOfflineMetaDataCollection =
                  InvoiceOfflineCollectionMetaDataRepository.Get(
                    i =>
                    i.BilledMemberCode == billedMemberCode && i.BillingMemberCode == billingMemberCode &&
                    i.BillingCategoryId == invoiceBase.BillingCategoryId && i.BillingMonth == invoiceBase.BillingMonth &&
                    i.BillingYear == invoiceBase.BillingYear &&
                    i.InvoiceNumber.ToUpper() == invoiceBase.InvoiceNumber.ToUpper() && i.PeriodNo == invoiceBase.BillingPeriod &&
                    (i.OfflineCollectionFolderTypeId == EinvoiceFolderTypeId ||
                     i.OfflineCollectionFolderTypeId == ListingsFolderTypeId));


                if (invOfflineMetaDataCollection.Count() > 0)
                {
                    return GetInvoiceOfflineCollectionZip(invOfflineMetaDataCollection, billedMemberCode, billingMemberCode,
                                                   archiveType == 2 ? true : false, invoiceBase, includeListings);
                }

                _logger.Info("No invoice offline metadata found for the invoice");
                throw new Exception("No invoice offline metadata found for the invoice");
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the invoice offline collection.
        /// </summary>
        /// <param name="invoiceBase">invoiceBase</param>
        /// <param name="invoiceOfflineCollectionMetaDataCollection">invoice Offline Collection Meta Data Collection</param>
        /// <param name="billingMemberCode"></param>
        /// <param name="isReceivable">isReceivable</param>
        /// <param name="billedMemberCode"></param>
        /// <param name="includeListings">Billing category</param>
        private string GetInvoiceOfflineCollectionZip(IEnumerable<InvoiceOfflineCollectionMetaData> invoiceOfflineCollectionMetaDataCollection, string billedMemberCode, string billingMemberCode, bool isReceivable, InvoiceBase invoiceBase, bool includeListings)
        {
            if (invoiceBase != null)
            {
                var zipFolderName = GetZipFolderName(billedMemberCode, billingMemberCode, isReceivable, invoiceBase);
                var zipFilePath = Path.Combine(FileIo.GetForlderPath(SFRFolderPath.LegalArchivePath),
                                               string.Format("{0}.ZIP", zipFolderName));
                //var zipFilePath = Path.Combine(@"D:\REPORT",
                //                           string.Format("{0}.ZIP", zipFolderName));

                var tempFolderPath = Path.Combine(FileIo.GetForlderPath(SFRFolderPath.LADeposit), zipFolderName);
                //var tempFolderPath = Path.Combine(@"D:\REPORT", zipFolderName);


                _logger.InfoFormat("ZipFolderName  <=> ZipFilePath <=> TempFolderPath : {0} <=> {1} <=> {2}", zipFolderName, zipFilePath, tempFolderPath);


                //Copy all the documents in the folder created at Temp folder

                if (!Directory.Exists(tempFolderPath))
                    Directory.CreateDirectory(tempFolderPath); //Create temp folder on SFRRoot folder

                var isDocumentFound = CopyOfflineDocuments(tempFolderPath, isReceivable, invoiceBase.BillingCategory,
                                                           invoiceOfflineCollectionMetaDataCollection, includeListings);

                if (isDocumentFound)
                {
                    FileIo.ZipOutputFolder(tempFolderPath, zipFilePath);
                    return zipFilePath;
                }
                else
                {
                    _logger.Info("No document found to copy.Deleted temporary folder.");
                    File.Delete(tempFolderPath);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Format : LAR-AAA-B-CCC-YYYYMMPP-D-NNNNNNNNNN.ZIP
        /// </summary>
        /// <param name="billedMemberCode"></param>
        /// <param name="billingMemberCode"></param>
        /// <param name="isReceivable"></param>
        /// <param name="invoiceBase"></param>
        /// <returns></returns>
        private string GetZipFolderName(string billedMemberCode, string billingMemberCode, bool isReceivable, InvoiceBase invoiceBase)
        {
            var billingPeriod = string.Format("{0}{1}{2}", invoiceBase.BillingYear,
                                              invoiceBase.BillingMonth.ToString().PadLeft(2, '0'),
                                              invoiceBase.BillingPeriod.ToString().PadLeft(2, '0'));

            /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: No code change required, since billing and billed member code numeric values are used as is.
            Ref: FRS Section 3.6 Table 24 Row 14 */

            return string.Format("LAR-{0}-{1}-{2}-{3}-{4}-{5}", isReceivable ? billingMemberCode : billedMemberCode,
                                 isReceivable ? "R" : "P", isReceivable ? billedMemberCode : billingMemberCode, billingPeriod,
                                 invoiceBase.BillingCategory.ToString().ToUpper()[0], invoiceBase.InvoiceNumber);
        }

        /// <summary>
        /// Copies the Offline documents as per billing category and request type (i.e. Payables\Receivables)
        /// </summary>
        /// <param name="baseFolderPath">Base folder path</param>
        /// <param name="isReceivable">isReceivable</param>
        /// <param name="billingCategoryType">Billing category</param>
        /// <param name="invoiceOfflineCollectionMetaDataCollection">invoiceOfflineCollectionMetaDataCollection</param>
        /// <param name="includeListings">Billing category</param>
        private bool CopyOfflineDocuments(string baseFolderPath, bool isReceivable, BillingCategoryType billingCategoryType, IEnumerable<InvoiceOfflineCollectionMetaData> invoiceOfflineCollectionMetaDataCollection, bool includeListings)
        {
            bool isDocumentFound = false;
            var listingsFolderPath = string.Empty;
            var listingsOfflineMetadata = new InvoiceOfflineCollectionMetaData();

            var eInvoiceFolderPath = Path.Combine(baseFolderPath, EinvoicesFolderName);
            var eInvoiceOfflineMetaData = invoiceOfflineCollectionMetaDataCollection.FirstOrDefault(i => i.OfflineCollectionFolderTypeId == EinvoiceFolderTypeId);



            if (includeListings)
            {
                listingsFolderPath = Path.Combine(baseFolderPath, ListingsFolderName);
                listingsOfflineMetadata = invoiceOfflineCollectionMetaDataCollection.FirstOrDefault(i => i.OfflineCollectionFolderTypeId == ListingsFolderTypeId);

                if (listingsOfflineMetadata == null)
                {
                    _logger.Info("No invoice offline metadata found for Listings for the Invoice");
                    throw new Exception("No invoice offline metadata found for Listings for the Invoice");
                }
            }


            if (eInvoiceOfflineMetaData == null)
            {
                _logger.Info("No invoice offline metadata found for Envoice for the Invoice");
                throw new Exception("No invoice offline metadata found for Envoice for the Invoice");
            }


            switch (billingCategoryType)
            {
                case BillingCategoryType.Pax:
                    if (isReceivable)
                    {
                        isDocumentFound = CopyEinvoiceDocuments(eInvoiceOfflineMetaData.FilePath, eInvoiceFolderPath, true, true, false, BillingCategoryType.Pax);
                        if (includeListings)
                        {
                            isDocumentFound = CopyListingsDocuments(listingsOfflineMetadata.FilePath, listingsFolderPath, true, false) || isDocumentFound;
                        }

                    }
                    else
                    {
                        isDocumentFound = CopyEinvoiceDocuments(eInvoiceOfflineMetaData.FilePath, eInvoiceFolderPath, true, true, true, BillingCategoryType.Pax);
                        if (includeListings)
                        {
                            isDocumentFound = CopyListingsDocuments(listingsOfflineMetadata.FilePath, listingsFolderPath, true, false) || isDocumentFound;
                        }

                    }
                    break;
                case BillingCategoryType.Cgo:
                    if (isReceivable)
                    {
                        isDocumentFound = CopyEinvoiceDocuments(eInvoiceOfflineMetaData.FilePath, eInvoiceFolderPath, true, true, false, BillingCategoryType.Cgo);
                        if (includeListings)
                        {
                            isDocumentFound = CopyListingsDocuments(listingsOfflineMetadata.FilePath, listingsFolderPath, true, false) || isDocumentFound;
                        }

                    }
                    else
                    {
                        isDocumentFound = CopyEinvoiceDocuments(eInvoiceOfflineMetaData.FilePath, eInvoiceFolderPath, true, true, true, BillingCategoryType.Cgo);
                        if (includeListings)
                        {
                            isDocumentFound = CopyListingsDocuments(listingsOfflineMetadata.FilePath, listingsFolderPath, true, false) || isDocumentFound;
                        }

                    }
                    break;
                case BillingCategoryType.Misc:
                    if (isReceivable)
                    {
                        isDocumentFound = CopyEinvoiceDocuments(eInvoiceOfflineMetaData.FilePath, eInvoiceFolderPath, true, true, false, BillingCategoryType.Misc);
                        if (includeListings)
                        {
                            isDocumentFound = CopyListingsDocuments(listingsOfflineMetadata.FilePath, listingsFolderPath, false, true) || isDocumentFound;
                        }

                    }
                    else
                    {
                        isDocumentFound = CopyEinvoiceDocuments(eInvoiceOfflineMetaData.FilePath, eInvoiceFolderPath, true, true, true, BillingCategoryType.Misc);
                        if (includeListings)
                        {
                            isDocumentFound = CopyListingsDocuments(listingsOfflineMetadata.FilePath, listingsFolderPath, false, true) || isDocumentFound;
                        }

                    }
                    break;
                case BillingCategoryType.Uatp:
                    if (isReceivable)
                    {
                        isDocumentFound = CopyEinvoiceDocuments(eInvoiceOfflineMetaData.FilePath, eInvoiceFolderPath, true, true, false, BillingCategoryType.Uatp);

                        if (includeListings)
                        {
                            isDocumentFound = CopyListingsDocuments(listingsOfflineMetadata.FilePath, listingsFolderPath, false, true) || isDocumentFound;
                        }
                    }
                    else
                    {
                        isDocumentFound = CopyEinvoiceDocuments(eInvoiceOfflineMetaData.FilePath, eInvoiceFolderPath, true, true, true, BillingCategoryType.Uatp);
                        if (includeListings)
                        {
                            isDocumentFound = CopyListingsDocuments(listingsOfflineMetadata.FilePath, listingsFolderPath, false, true) || isDocumentFound;
                        }
                    }
                    break;
            }
            return isDocumentFound;
        }

        /// <summary>
        /// Copy Einvoices documents
        /// </summary>
        /// <param name="sourcePath">sourcePath</param>
        /// <param name="destinationPath">destinationPath</param>
        /// <param name="includePdf">includePdf</param>
        /// <param name="includeXml">includeXml</param>
        /// <param name="includeDsLog">includeDsLog</param>
        /// <param name="billingCategoryType">billingCategoryType</param>
        /// <returns>isDocumentFound</returns>
        private bool CopyEinvoiceDocuments(string sourcePath, string destinationPath, bool includePdf, bool includeXml, bool includeDsLog, BillingCategoryType billingCategoryType)
        {
            bool isEinvoiceDocumentFound = false;

            if (Directory.Exists(sourcePath))
            {
                foreach (var fileName in Directory.GetFiles(sourcePath))
                {
                    bool isDocumentFound = false;
                    if (includePdf && Path.GetExtension(fileName).ToUpper().CompareTo(".PDF") == 0) //Ignore PDF file
                    {
                        isDocumentFound = true;
                    }

                    if (Path.GetExtension(fileName).ToUpper().CompareTo(".XML") == 0 && includeXml)
                    {
                        var xmlFileName = string.Format("{0}{1}-", billingCategoryType.ToString().ToUpper()[0], "INV");
                        //Ignore Invoice XML
                        if (Path.GetFileName(fileName).ToUpper().StartsWith(xmlFileName))
                        {
                            isDocumentFound = true;
                        }
                    }

                    if (Path.GetExtension(fileName).ToUpper().CompareTo(".XML") == 0 && includeDsLog)
                    {
                        var xmlFileName = string.Format("{0}{1}-", billingCategoryType.ToString().ToUpper()[0], "XVF");
                        //Ignore DS verification log
                        if (Path.GetFileName(fileName).ToUpper().StartsWith(xmlFileName))
                        {
                            isDocumentFound = true;
                        }
                    }

                    if (isDocumentFound)
                    {
                        if (!Directory.Exists(destinationPath)) Directory.CreateDirectory(destinationPath);
                        File.Copy(fileName, Path.Combine(destinationPath, Path.GetFileName(fileName)), true);
                        isEinvoiceDocumentFound = true;
                    }
                }
            }

            if (!isEinvoiceDocumentFound)
            {
                //throw exception : Einvoice document not found for the invoice
                _logger.Info("throw exception : Einvoice document not found for the invoice");
                throw new Exception("Einvoice document not found for the invoice");
            }

            return isEinvoiceDocumentFound;

        }

        /// <summary>
        /// Copy Listings documents
        /// </summary>
        /// <param name="sourcePath">sourcePath</param>
        /// <param name="destinationPath">destinationPath</param>
        /// <param name="includeCsv">includeCsv</param>
        /// <param name="includePdf">includePdf</param>
        /// <returns></returns>
        private bool CopyListingsDocuments(string sourcePath, string destinationPath, bool includeCsv, bool includePdf)
        {
            bool isListingsDocFound = false;
            if (Directory.Exists(sourcePath))
            {
                foreach (var fileName in Directory.GetFiles(sourcePath))
                {
                    bool isDocumentFound = false;
                    if (includeCsv && Path.GetExtension(fileName).ToUpper().CompareTo(".CSV") == 0)
                    {
                        isDocumentFound = true;
                    }
                    if (includePdf && Path.GetExtension(fileName).ToUpper().CompareTo(".PDF") == 0)
                    {
                        isDocumentFound = true;
                    }

                    if (isDocumentFound)
                    {
                        if (!Directory.Exists(destinationPath)) Directory.CreateDirectory(destinationPath);
                        File.Copy(fileName, Path.Combine(destinationPath, Path.GetFileName(fileName)), true);
                        isListingsDocFound = true;
                    }

                }
            }
            if (!isListingsDocFound)
            {
                _logger.Info("throw exception : Listings document not found for the invoice");
                throw new Exception("Listings document not found for the invoice");
            }

            return isListingsDocFound;
        }
        #endregion

        #region Submit Zip File to API and get response
        /// <summary>
        /// call API and Submit Zip File with applicativeMetadata Information
        /// </summary>
        /// <param name="applicativeMetadata">(string)applicativeMetadata</param>
        /// <param name="zipFilePath">ziped file path</param>
        /// <param name="compartmentId">compartment Id</param>
        /// <returns>Response stream </returns>
        private string SubmitZipFile(string applicativeMetadata, string zipFilePath, string compartmentId)
        {
            var result = string.Empty;

            try
            {

                var cdcArkhineoBaseUrl = SystemParameters.Instance.LegalArchivingDetails.CDCArkhineoBaseURL;
                var cdcArkhineoCoffreId = SystemParameters.Instance.LegalArchivingDetails.CDCArkhineoCoffreID;
                var cdcArkhineoSectionId = SystemParameters.Instance.LegalArchivingDetails.CDCArkhineoSectionID;

                var cdcArkhineoLoginId = SystemParameters.Instance.LegalArchivingDetails.CDCArkhineoLoginID;
                var cdcArkhineoPassword = SystemParameters.Instance.LegalArchivingDetails.CDCArkhineoPassword;

                _logger.InfoFormat("System Parameter - Web request Credentials. LoginId: {0}, Password: {1}", cdcArkhineoLoginId, cdcArkhineoPassword);

                //Create URL

                // string URL = "https://pwd.arkhineo.fr/depot/cfes/ATQetpZkFSCAFTKJ/sections/ATRldn1fFSOAXiU_/compartments/ATRleOESFSOAXifU/archives";
                var URL = cdcArkhineoBaseUrl + "/depot/cfes/" + cdcArkhineoCoffreId + "/sections/" + cdcArkhineoSectionId + "/compartments/" + compartmentId + "/archives";

                _logger.Info("Web Service URL : " + URL);


                // Create Name value collection which will have parameters used by Deposit() 
                NameValueCollection formData = new NameValueCollection();

                // Create boundary variable for each parameter
                string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

                // Create HttpWeb request to call Deposit functionality
                System.Net.WebRequest webRequest = System.Net.WebRequest.Create(URL);

                // Set request parameters  
                webRequest.Method = "POST";
                webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
                // webRequest.Credentials = new NetworkCredential("depot-test@iata.com", "KBQpU");
                webRequest.Credentials = new NetworkCredential(cdcArkhineoLoginId, cdcArkhineoPassword);

                // Set file name
                // string FilePath = "D:\\PINV-UX-996-565.zip";

                formData.Clear();
                // Add values to Name value collection
                formData["dublinCore[source]"] = "SIS IATA TEST ENVIRONMENT FINAL";
                formData["dublinCore[format]"] = "Archive DS FINAL";
                formData["applicativeMetadata"] = applicativeMetadata;

                // Call GetPostStream() method which converts file to Byte array stream and returns 
                Stream postDataStream = GetPostStream(zipFilePath, formData, boundary);
                // set request content lenght
                webRequest.ContentLength = postDataStream.Length;

                // Call GetRequestStream() method which returns IO stream for writing data to Server resource
                Stream reqStream = webRequest.GetRequestStream();

                // Read and write stream data
                postDataStream.Position = 0;
                byte[] tempBuffer = new byte[postDataStream.Length];
                postDataStream.Read(tempBuffer, 0, tempBuffer.Length);
                postDataStream.Close();
                reqStream.Write(tempBuffer, 0, tempBuffer.Length);
                reqStream.Close();

                _logger.InfoFormat("Just Before Request - Web request Credentials. LoginId: {0}, Password: {1}", ((NetworkCredential)webRequest.Credentials).UserName, ((NetworkCredential)webRequest.Credentials).Password);

                // Get response stream from Server
                var webResponse = webRequest.GetResponse();

                _logger.InfoFormat("Response Header: {0}", webResponse.Headers.Get(0));

                _logger.InfoFormat("webResponse.IsMutuallyAuthenticated: {0}", webResponse.IsMutuallyAuthenticated);

                StreamReader sr = new StreamReader(webResponse.GetResponseStream());
                result = sr.ReadToEnd();
                //SIS_SCR_REPORT_23_jun-2016_2 :Improper_Resource_Shutdown_or_Release
                sr.Close();
                //SIS_SCR_REPORT_23_jun-2016_2: Heap_Inspection
                cdcArkhineoPassword = string.Empty;
            }
            catch (Exception ex)
            {
                result = "Failed";
                _logger.Error("Error occured at the time of Submition of Zip File to API", ex);
                //SCP#461308: KAL- Alert for error in LA Deposit not sent to SIS Ops
                //throw;
            }
            return result;
        }

        private Stream GetPostStream(string filePath, NameValueCollection formData, string boundary)
        {
            Stream postDataStream = new System.IO.MemoryStream();

            //adding form data
            string formDataHeaderTemplate = Environment.NewLine + "--" + boundary + Environment.NewLine + "Content-Disposition: form-data; name=\"{0}\";" + Environment.NewLine + Environment.NewLine + "{1}";

            foreach (string key in formData.Keys)
            {
                byte[] formItemBytes = System.Text.Encoding.UTF8.GetBytes(string.Format(formDataHeaderTemplate, key, formData[key]));
                postDataStream.Write(formItemBytes, 0, formItemBytes.Length);
            }

            //adding file data
            FileInfo fileInfo = new FileInfo(filePath);

            //New Code as per SCP 246949 - legal archive 
            //Description: Added correct content-type value and charset to make ENCODING related changes as advised by CDC.
            string fileHeaderTemplate = Environment.NewLine + "--" + boundary + Environment.NewLine + "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" + Environment.NewLine + "content-type=application/zip; charset=ISO-8859-1" + Environment.NewLine + Environment.NewLine;
            //Old Code
            //string fileHeaderTemplate = Environment.NewLine + "--" + boundary + Environment.NewLine + "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" + Environment.NewLine + "Content-Type:text/plain;" + Environment.NewLine + Environment.NewLine;

            byte[] fileHeaderBytes = System.Text.Encoding.UTF8.GetBytes(string.Format(fileHeaderTemplate, "dataObject", fileInfo.Name));

            postDataStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);

            FileStream fileStream = fileInfo.OpenRead();

            byte[] buffer = new byte[1024];

            int bytesRead = 0;

            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                postDataStream.Write(buffer, 0, bytesRead);
            }

            fileStream.Close();

            byte[] endBoundaryBytes = System.Text.Encoding.UTF8.GetBytes(Environment.NewLine + "--" + boundary + "--");
            postDataStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);

            return postDataStream;
        }

        #endregion

        #region Email Section

        private void SendArchiveStatusAlert(string Content)
        {
            try
            {
                // Get an object of the EmailSender component
                var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
                // Get an instance of email settings repository
                var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
                // Verify and log TemplatedTextGenerator for null value.
                _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

                // Generate email body text for ICH settlement web service failure notification email
                var context = new VelocityContext();
                context.Put("CurrentTime", DateTime.UtcNow.ToShortTimeString());
                context.Put("Content", Content);
                context.Put("n", "\n");
                // Verify and log TemplatedTextGenerator for null value.
                _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

                var emailSettingForISAdminAlert = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.LegalArchivingStatusNotification);

                // Generate email body text for own profile updates contact type mail
                if (TemplatedTextGenerator != null)
                {
                    var emailToISAdminText = TemplatedTextGenerator.GenerateTemplatedText(EmailTemplateId.LegalArchivingStatusNotification, context);
                    // Create a mail object to send mail
                    var msgForISAdmin = new MailMessage
                    {
                        From = new MailAddress(emailSettingForISAdminAlert.SingleOrDefault().FromEmailAddress),
                        IsBodyHtml = true
                    };

                    var subject = emailSettingForISAdminAlert.SingleOrDefault().Subject;
                    msgForISAdmin.Subject = subject;


                    // loop through the contacts list and add them to To list of mail to be sent
                    if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail))
                    {
                        var mailAdressList = ConvertUtil.ConvertToMailAddresses(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

                        foreach (var mailaddr in mailAdressList)
                        {
                            msgForISAdmin.To.Add(mailaddr);
                        }
                    }

                    //set body text of mail
                    msgForISAdmin.Body = emailToISAdminText;

                    //send the mail
                    emailSender.Send(msgForISAdmin);

                }
            }// End try
            catch (Exception ex)
            {

                _logger.Error("Error occurred while sending Archive Status Alert", ex);

                // throw new ISBusinessException(ErrorCodes.ICHSettlementErrorNotificationFailed, "Error occurred while sending  Archive WSConnect Failure  Notification to IS Administrator");

            }// End catch
        }


        /// <summary>
        /// This method will send an email alert to IS-Admin in case there was failure in calling CDC webservice for any Invoice zip submission. 
        /// </summary>
        private void SendArchiveWsConnectFailureAlertToIsAdmin(string invoiceId, string invoiceNumber, int billingPeriod, int billingMonth, int billingYear, int billingCategory)
        {
            try
            {
                // Get an object of the EmailSender component
                var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
                // Get an instance of email settings repository
                var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
                // Verify and log TemplatedTextGenerator for null value.
                _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

                var bCategory = (BillingCategoryType)billingCategory;
                // Generate email body text for ICH settlement web service failure notification email
                var context = new VelocityContext();
                context.Put("FailureTime", DateTime.UtcNow.ToShortTimeString());
                context.Put("InvoiceNumber", invoiceNumber);
                context.Put("InvoiceId", invoiceId);
                context.Put("BillingPeriod", billingPeriod);
                context.Put("BillingMonth", billingMonth);
                context.Put("BillingYear", billingYear);
                context.Put("BillingCategory", bCategory);
                
                //context.Put("EmailContent", emailContent);
                context.Put("n", "\n");
                // Verify and log TemplatedTextGenerator for null value.
                _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

                var emailSettingForIsAdminAlert = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.LegalArchiveWebServiceExceptionAlert);

                // Generate email body text for own profile updates contact type mail
                if (TemplatedTextGenerator != null)
                {
                    var emailToIsAdminText = TemplatedTextGenerator.GenerateTemplatedText(EmailTemplateId.LegalArchiveWebServiceExceptionAlert, context);
                    // Create a mail object to send mail
                    var msgForIsAdmin = new MailMessage
                    {
                        From = new MailAddress(emailSettingForIsAdminAlert.SingleOrDefault().FromEmailAddress),
                        IsBodyHtml = true
                    };

                    var subject = emailSettingForIsAdminAlert.SingleOrDefault().Subject;
                    msgForIsAdmin.Subject = subject;


                    // loop through the contacts list and add them to To list of mail to be sent
                    if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail))
                    {
                        var mailAdressList = ConvertUtil.ConvertToMailAddresses(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

                        foreach (var mailaddr in mailAdressList)
                        {
                            msgForIsAdmin.To.Add(mailaddr);
                        }
                    }

                    //set body text of mail
                    msgForIsAdmin.Body = emailToIsAdminText;

                    //send the mail
                    emailSender.Send(msgForIsAdmin);

                }
            }// End try
            catch (Exception ex)
            {

                _logger.Error("Error occurred while sending Archive WSConnect Failure Notification to IS Administrator", ex);

                // throw new ISBusinessException(ErrorCodes.ICHSettlementErrorNotificationFailed, "Error occurred while sending  Archive WSConnect Failure  Notification to IS Administrator");

            }// End catch
        }

        /// <summary>
        /// This method will send an email to IS-Admin in case there was failure in depositing item corresponding to passed LegalArchiveID.
        /// </summary>
        /// <param name="legalArchivelog"></param>
        /// <param name="webServiceResponse"></param>
        private void SendArchivalFailure(LegalArchiveLog legalArchivelog, string webServiceResponse)
        {
            try
            {
                var context = new VelocityContext();
                // Get an object of the EmailSender component
                var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
                // Get an instance of email settings repository
                var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
                // Verify and log TemplatedTextGenerator for null value.
                _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

                try
                {
                    //Create Email Parmeter Content
                    var archiveCustomer = legalArchivelog.ArcCustDesignator + " " + legalArchivelog.ArcCustAccounting;
                    var billingMember = legalArchivelog.CrBillingMemberDesignator + " " + legalArchivelog.CrBillingMemberAccounting;
                    var billedMember = legalArchivelog.DbBilledMemberDesignator + " " + legalArchivelog.DbBilledMemberAccounting;
                    var invoiceNumber = legalArchivelog.InvoiceNumber;

                    BillingCategoryType billingCategory = (BillingCategoryType)legalArchivelog.BillingCategory;
                    InvoicePeriod invoicePeriod = (InvoicePeriod)legalArchivelog.BillingPeriod;
                    var billingYear = legalArchivelog.BillingYear;
                    var billingMonth = Enum.Parse(typeof(Month), legalArchivelog.BillingMonth.ToString());
                    var billingPeriod = billingMonth + "-" + billingYear + " " + invoicePeriod;


                    // Generate email body text for ICH settlement web service failure notification email
                    context.Put("FailureTime", DateTime.UtcNow.ToShortTimeString());
                    context.Put("ArchiveCustomer", archiveCustomer);
                    context.Put("BillingMember", billingMember);
                    context.Put("BilledMember", billedMember);
                    context.Put("BillingPeriod", billingPeriod);
                    context.Put("BillingCategory", billingCategory.ToString().ToUpper());
                    context.Put("InvoiceNumber", invoiceNumber);
                    context.Put("WebServiceResponse", webServiceResponse);
                    context.Put("n", "\n");

                    _logger.InfoFormat("Template Parameters : ArchiveCustomer {0} , BillingMember {1} , BilledMember {2} ,BillingPeriod {3} ,BillingCategory {4} , InvoiceNumber {5}",
                            archiveCustomer, billingMember, billedMember, billingPeriod, billingCategory.ToString().ToUpper(), invoiceNumber);

                }
                catch (Exception ex)
                {
                    _logger.Info("Error in getting Template Parameters : ", ex);
                }

                // Verify and log TemplatedTextGenerator for null value.
                _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

                var emailSettingForISAdminAlert = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.LegalArchiveDepositRequestFailureAlert);

                // Generate email body text for own profile updates contact type mail
                if (TemplatedTextGenerator != null)
                {
                    var emailToISAdminText = TemplatedTextGenerator.GenerateTemplatedText(EmailTemplateId.LegalArchiveDepositRequestFailureAlert, context);
                    // Create a mail object to send mail
                    var msgForISAdmin = new MailMessage
                    {
                        From = new MailAddress(emailSettingForISAdminAlert.SingleOrDefault().FromEmailAddress),
                        IsBodyHtml = true
                    };

                    var subject = emailSettingForISAdminAlert.SingleOrDefault().Subject;
                    msgForISAdmin.Subject = subject;


                    // loop through the contacts list and add them to To list of mail to be sent
                    if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail))
                    {
                        var mailAdressList = ConvertUtil.ConvertToMailAddresses(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

                        foreach (var mailaddr in mailAdressList)
                        {
                            msgForISAdmin.To.Add(mailaddr);
                        }
                    }

                    //set body text of mail
                    msgForISAdmin.Body = emailToISAdminText;

                    //send the mail
                    emailSender.Send(msgForISAdmin);

                }
            }// End try
            catch (Exception ex)
            {

                _logger.Error("Error occurred while sending Archive Failure Notification to IS Administrator", ex);

                //   throw new ISBusinessException(ErrorCodes.ICHSettlementErrorNotificationFailed, "Error occurred while sending ICH Settlement Error Notification to IS Administrator");

            }// End catch
        }

        /// <summary>
        /// This method will send an email alert to IS-dmin in case there was failure in Archive Zip File Creation. 
        /// </summary>
        /// <param name="legalArchivelog"></param>
        /// <param name="error"></param>
        private void SendMailArchivalZipFileCreationError(LegalArchiveLog legalArchivelog, string error)
        {
            try
            {
                var context = new VelocityContext();
                // Get an object of the EmailSender component
                var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
                // Get an instance of email settings repository
                var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
                // Verify and log TemplatedTextGenerator for null value.
                _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

                //Create Email Parmeter Content
                try
                {
                    var archiveCustomer = legalArchivelog.ArcCustDesignator + " " + legalArchivelog.ArcCustAccounting;
                    var billingMember = legalArchivelog.CrBillingMemberDesignator + " " + legalArchivelog.CrBillingMemberAccounting;
                    var billedMember = legalArchivelog.DbBilledMemberDesignator + " " + legalArchivelog.DbBilledMemberAccounting;
                    var invoiceNumber = legalArchivelog.InvoiceNumber;

                    BillingCategoryType billingCategory = (BillingCategoryType)legalArchivelog.BillingCategory;
                    InvoicePeriod invoicePeriod = (InvoicePeriod)legalArchivelog.BillingPeriod;
                    var billingYear = legalArchivelog.BillingYear;
                    var billingMonth = Enum.Parse(typeof(Month), legalArchivelog.BillingMonth.ToString());
                    var billingPeriod = billingMonth + "-" + billingYear + " " + invoicePeriod;

                    // Generate email body text for ICH settlement web service failure notification email

                    context.Put("FailureTime", DateTime.UtcNow.ToShortTimeString());
                    context.Put("ArchiveCustomer", archiveCustomer);
                    context.Put("BillingMember", billingMember);
                    context.Put("BilledMember", billedMember);
                    context.Put("BillingPeriod", billingPeriod);
                    context.Put("BillingCategory", billingCategory.ToString().ToUpper());
                    context.Put("InvoiceNumber", invoiceNumber);
                    context.Put("ErrorDetail", error);

                    context.Put("n", "\n");
                    _logger.InfoFormat("Template Parameters : ArchiveCustomer {0} , BillingMember {1} , BilledMember {2} ,BillingPeriod {3} ,BillingCategory {4} , InvoiceNumber {5} ,ErrorDetail {6}",
                                   archiveCustomer, billingMember, billedMember, billingPeriod, billingCategory.ToString().ToUpper(), invoiceNumber, error);
                }
                catch (Exception ex)
                {

                    _logger.Info("Error in getting Template Parameters : ", ex);
                  throw;
                }

                // Verify and log TemplatedTextGenerator for null value.
                _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

                var emailSettingForISAdminAlert = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.LegalArchiveZipFileCreationErrorAlert);

                // Generate email body text for own profile updates contact type mail
                if (TemplatedTextGenerator != null)
                {
                    var emailToISAdminText = TemplatedTextGenerator.GenerateTemplatedText(EmailTemplateId.LegalArchiveZipFileCreationErrorAlert, context);
                    // Create a mail object to send mail
                    var msgForISAdmin = new MailMessage
                    {
                        From = new MailAddress(emailSettingForISAdminAlert.SingleOrDefault().FromEmailAddress),
                        IsBodyHtml = true
                    };

                    var subject = emailSettingForISAdminAlert.SingleOrDefault().Subject;
                    msgForISAdmin.Subject = subject;


                    // loop through the contacts list and add them to To list of mail to be sent
                    if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail))
                    {
                        var mailAdressList = ConvertUtil.ConvertToMailAddresses(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

                        foreach (var mailaddr in mailAdressList)
                        {
                            msgForISAdmin.To.Add(mailaddr);
                        }
                    }

                    //set body text of mail
                    msgForISAdmin.Body = emailToISAdminText;

                    //send the mail
                    emailSender.Send(msgForISAdmin);

                }
            }// End try
            catch (Exception ex)
            {

                _logger.Error("Error occurred while sending Legal Archival Zip File Creation Error Notification to IS Administrator", ex);
              throw;
              // throw new ISBusinessException(ErrorCodes.ICHSettlementErrorNotificationFailed, "Error occurred while sending Legal Archival Zip File Creation Error Notification to IS Administrator");

            }// End catch
        }


       /// <summary>
       /// SCP400947-SRM: Legal archiving pending for July P2
       /// This method added to send email in case of any exception in processing of invoice.
       /// </summary>
       /// <param name="invoiceId">invoice id</param>
       /// <param name="invoiceType">invoice type</param>
       /// <param name="exception">exception</param>
        public void SendArchivalDepositExceptionNotification(string invoiceId, string invoiceType, string exception)
        {
          try
          {
            var context = new VelocityContext();
            // Get an object of the EmailSender component
            var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
            // Get an instance of email settings repository
            var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
            // Verify and log TemplatedTextGenerator for null value.
            _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

            //Create Email Parmeter Content
            try
            {
              // Generate email body text for ICH settlement web service failure notification email
              context.Put("InvoiceId", invoiceId);
              context.Put("InvoiceType", invoiceType);
              context.Put("ErrorDetail", exception);
              context.Put("ExceptionTime", DateTime.UtcNow.ToShortTimeString());
              context.Put("n", "\n");

              _logger.InfoFormat("Exception Notification has been sent, for InvoiceId: {0}, InvoiceType: {1}, Error:{2}",invoiceId, invoiceType, exception);
            }
            catch (Exception ex)
            {
              _logger.Info("Error in getting Template Parameters : ", ex);
            }

            // Verify and log TemplatedTextGenerator for null value.
            _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

            var emailSettingForISAdminAlert = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.LegalArchiveDepositeExceptionNotification);

            // Generate email body text for own profile updates contact type mail
            if (TemplatedTextGenerator != null)
            {
              var emailToISAdminText = TemplatedTextGenerator.GenerateTemplatedText(EmailTemplateId.LegalArchiveDepositeExceptionNotification, context);
              // Create a mail object to send mail
              var msgForISAdmin = new MailMessage
              {
                From = new MailAddress(emailSettingForISAdminAlert.SingleOrDefault().FromEmailAddress),
                IsBodyHtml = true
              };

              var subject = emailSettingForISAdminAlert.SingleOrDefault().Subject;
              msgForISAdmin.Subject = subject;


              // loop through the contacts list and add them to To list of mail to be sent
              if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail))
              {
                var mailAdressList = ConvertUtil.ConvertToMailAddresses(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

                foreach (var mailaddr in mailAdressList)
                {
                  msgForISAdmin.To.Add(mailaddr);
                }
              }

              //set body text of mail
              msgForISAdmin.Body = emailToISAdminText;

              //send the mail
              emailSender.Send(msgForISAdmin);

            }
          }// End try
          catch (Exception ex)
          {
            _logger.Error("Error occurred while sending Legal Archival Zip File Creation Error Notification to IS Administrator", ex);
          }// End catch
        }

        #endregion

        #endregion
    }
}

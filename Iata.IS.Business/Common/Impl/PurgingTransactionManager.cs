using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Iata.IS.AdminSystem;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.Business.BroadcastMessages.Impl;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Data.Pax;
using Iata.IS.Data.Purging;
using Iata.IS.Model.BroadcastMessages;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using log4net;
using log4net.Repository.Hierarchy;
using NVelocity;


namespace Iata.IS.Business.Common.Impl
{
    public class PurgingTransactionManager : IPurgingTransactionManager
    {
        private const string PurgingTransactionFailedTitle = "Purging transaction data failed";
        private const string PurgingTransactionFailedMessage = "The purging transaction data failed";
        private const string PurgingTransactionMemberCode = "";
        // Logger instance.
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

      //  public IPurgingTransactionRepository PurgingTransactionRepository { get; set; }

        /// <summary>
        /// The purgingTransaction trigger method
        /// </summary>
        /// <param name="currentPeriod">The current period for Purging transaction</param>
        //public void PurgingTransactionTrigger(DateTime currentPeriod)
        //{
        //    try
        //    {
        //        PurgingTransactionRepository.DeleteTransaction(currentPeriod);

        //    }
        //    catch (Exception)
        //    {
        //        AddISSISOpsAlert(PurgingTransactionMemberCode, PurgingTransactionFailedTitle, PurgingTransactionFailedMessage, EmailTemplateId.ISAdminProcessedInvoiceCSVFailedNotification, currentPeriod.ToString());
        //        throw new Exception("The delete transaction data failed.");
        //    }

        //}

        /// <summary>
        /// To delete the transaction data
        /// </summary>
        /// <param name="billingPeriod">The current Billling Period</param>
        /// <param name="billingMonth">The current Billling Month</param>
        /// <param name="billingYear">The current Billling Year</param>
        /// <param name="lastClosedBillingPeriodDate">The current Billling Year</param>
        /// <param name="errorInvoiceRetentionPeriod">The current Billling Year</param>
        public bool PurgingTransactionData(int billingPeriod, int billingMonth, int billingYear, DateTime lastClosedBillingPeriodDate, int errorInvoiceRetentionPeriod)
        {
            bool result = false;
            DateTime currentPeriod = DateTime.Now;
            try
            {
              var purgingTransactionRepository = Ioc.Resolve<IPurgingTransactionRepository>(typeof(IPurgingTransactionRepository));
                currentPeriod = new DateTime(billingYear, billingMonth, billingPeriod);
                Logger.Info(string.Format("In PurgingTransactionData, Current Period(Year-Month-Period): {0:D4}{1:D2}P{2}", currentPeriod.Year, currentPeriod.Month, currentPeriod.Day));
                Logger.Info(string.Format("In PurgingTransactionData, Last Closed Billing Period(Year-Month-Period): {0:D4}{1:D2}P{2}", lastClosedBillingPeriodDate.Year, lastClosedBillingPeriodDate.Month, lastClosedBillingPeriodDate.Day));
                Logger.Info(string.Format("In PurgingTransactionData, Error Invoice Retention Period: {0}", errorInvoiceRetentionPeriod));
                var supportingDocPurgeList = purgingTransactionRepository.DeleteTransaction(currentPeriod, lastClosedBillingPeriodDate, errorInvoiceRetentionPeriod);
              
              // Enqueue the supporting document details.
                EnqueueSupportingDocuments(supportingDocPurgeList.ToList());
                result = true;
            }
            catch (FormatException)
            {
                result = false;
                throw new FormatException(string.Format("The value for year month or period is invalid. {0:D4}-{1:D2}-P{2}", billingYear, billingMonth, billingPeriod));
            }
            catch (Exception exception)
            {
              Logger.Error("Error while purging transactions: ", exception);
              const string serviceName = "Transaction purging";
              result = false;
              //AddISSISOpsAlert(PurgingTransactionMemberCode, PurgingTransactionFailedTitle, PurgingTransactionFailedMessage, EmailTemplateId.ISAdminProcessedInvoiceCSVFailedNotification, currentPeriod.ToString());
              var broadcastMessagesManager = Ioc.Resolve<IBroadcastMessagesManager>(typeof(IBroadcastMessagesManager));
              broadcastMessagesManager.SendISAdminExceptionNotification(EmailTemplateId.ISAdminExceptionNotification, serviceName, exception);
              Ioc.Release(broadcastMessagesManager);
              
            }

            return result;
        }

      private static void EnqueueSupportingDocuments(List<SupportingDocPurgingDetails> suppDocMessages)
      {
        Logger.InfoFormat("Queueing supporting documents for purging");
        Logger.InfoFormat("Number of supporting documents to be queued for purging [{0}]", suppDocMessages.Count);
        foreach (var suppDocMessage in suppDocMessages)
        {
          try
          {
            string fileName = suppDocMessage.FileName;
            var dotIndex = fileName.LastIndexOf('.');

            string fileExtension = fileName.Substring(dotIndex + 1, fileName.Length - dotIndex - 1);
            var supportingDocumentId = suppDocMessage.FileId;
            // The file name for supporting document/attachment is the .NET attachment GUID.
            var fullFilePath = Path.Combine(suppDocMessage.PurgingFilePath, supportingDocumentId+"."+fileExtension);

            Logger.InfoFormat("{0}: {1}", "Adding supporting document entry to queue", fullFilePath);
            var filePurgingQueueName = ConfigurationManager.AppSettings["FilePurgingQueueName"];
            if (!string.IsNullOrEmpty(filePurgingQueueName))
            {
              var queueHelper = new QueueHelper(filePurgingQueueName);
              IDictionary<string, string> queueMessage = new Dictionary<string, string>();

              queueMessage.Add("PURGING_FILE_PATH", fullFilePath);
              queueMessage.Add("PURGING_FILE_TYPE_ID", (suppDocMessage.PurgingFileTypeId).ToString());
              var fileId = suppDocMessage.FileId.ToString().Replace("-", string.Empty);
              queueMessage.Add("PURGING_FILE_ID", fileId);
              queueMessage.Add("SERVER_NAME", string.Empty);
              queueMessage.Add("SERVICE_NAME", string.Empty);
              queueHelper.Enqueue(queueMessage);

              Logger.InfoFormat("Enqueued file at [{0}]for purging", fullFilePath);
            }
          }
          catch (Exception exception)
          {
            Logger.ErrorFormat("Exception occured while enqueueing file at [{0}] ", exception);
            //Send Mail
          }
        }

      }

        /// <summary>
        /// To delete the temporary files
        /// </summary>
        /// <param name="billingPeriod">The billing Period</param>
        /// <param name="billingMonth">The billing Month</param>
        /// <param name="billingYear">The billing Year</param>
        public bool PurgingTemporaryFiles(int billingPeriod, int billingMonth, int billingYear)
        {
            bool result = false;
            DateTime currentPeriodText = DateTime.Now;
            try
            {
                currentPeriodText = new DateTime(billingYear, billingMonth, billingPeriod);
            }
            catch (Exception exception)
            {
              Logger.Error("Error while purging temporary files: ", exception);
            }


            var purgingTransactionRepository = Ioc.Resolve<IPurgingTransactionRepository>(typeof(IPurgingTransactionRepository));
            purgingTransactionRepository.DeleteTemporaryFiles(currentPeriodText);

            return true;
        }

        /// <summary>
        /// To delete the temporary files
        /// </summary>
        ///<param name="currentPeriod">The Current Period</param>
        public void PurgingTemporaryFilesTrigger(DateTime currentPeriod)
        {
          var purgingTransactionRepository = Ioc.Resolve<IPurgingTransactionRepository>(typeof(IPurgingTransactionRepository));
          purgingTransactionRepository.DeleteTemporaryFiles(currentPeriod);
        }

        public void AddISSISOpsAlert(string billingEntityCode, string message, string title, EmailTemplateId emailTemplateId, string billingPeriodText)
        {
            // Create an object of the nVelocity data dictionary
            var context = new VelocityContext();
            context.Put("MemberCode", billingEntityCode);
            context.Put("Period", billingPeriodText);

            var issisOpsAlert = new ISSISOpsAlert
            {
                Message = String.Format(message, billingEntityCode),
                AlertDateTime = DateTime.UtcNow,
                IsActive = true,
                EmailAddress = SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail,
                Title = title
            };

            BroadcastMessagesManager.AddAlert(issisOpsAlert, emailTemplateId, context);
        }
    }
}

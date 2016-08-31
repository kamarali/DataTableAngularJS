using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using Castle.Core.Smtp;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Common;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core.Configuration;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Data.Reports.ConfirmationDetails;
using Iata.IS.Data.Reports.ConfirmationSummary;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Reports.ConfirmationDetails;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using System.Linq;
using Iata.IS.Model.Reports.ConfirmationSummary;
using log4net;
using NVelocity;

namespace Iata.IS.Business.Reports.ConfirmationDetail.Impl
{
   public class ConfirmationDetailImpl : IConfirmationDetail
    {
        private IConfirmationDetailData ConfirmationDetailParam { get; set; }

        private IValueConfirmationSummary ValueConfirmationSummary { get; set; }

        public List<ConfirmationDetailModel> ConfirmDetailModel { get; set; }

        public List<ConfirmationSummaryModel> ConfirmSummaryModel { get; set; }
        /// <summary>
        /// IsInputFile Repository.
        /// </summary>
        public IRepository<IsInputFile> IsInputFileRepository { get; set; }

        public IReferenceManager ReferenceManager { get; set; }

        public ICalendarManager CalendarManager { get; set; }

        public IMemberManager memberManager { get; set; } 

        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        
        public ConfirmationDetailImpl(IConfirmationDetailData confirmationDetailData, IValueConfirmationSummary valueConfirmationSumary)
        {
            ConfirmationDetailParam = confirmationDetailData;
            ValueConfirmationSummary = valueConfirmationSumary;
        }

        public List<ConfirmationDetailModel> GetConfirmationDetails(int memberId, int clearanceMonth, int periodNo, int blingAirlineNo, int bledAirlineNo, string invoiceNo, string issuingAirline, string originalPMI, string validatedPMI, string AgreIndSupplied, string AgreIndValidated, string ATPCOReasonCode, int billingYear, int pageStartIndex, int pageEndIndex,int IsCountReq)
        {
            return ConfirmationDetailParam.GetConfirmationDetails(memberId, clearanceMonth, periodNo, blingAirlineNo, bledAirlineNo, invoiceNo, issuingAirline, originalPMI, validatedPMI, AgreIndSupplied, AgreIndValidated, ATPCOReasonCode, billingYear,pageStartIndex,pageEndIndex,IsCountReq);
        }

       /// <summary>
       /// This method is used to generate csv
       /// </summary>
       public void AutoGenerateCSV()
       {
           // Fetch previous closed clearence period
           var Billingperiod = CalendarManager.GetLastClosedBillingPeriod(ClearingHouse.Ich);

           List<ConfirmationDetailModel> ListOfValueConfirmationDetails = GetValueConfimationDetails(Billingperiod);

           // Check whether list is empty or not; if not then generate csv for each members
           if(ListOfValueConfirmationDetails.Count > 0)
           {
              
               GenerateCSVForValueConfirmationDetails(ListOfValueConfirmationDetails,Billingperiod);
           }// End if

           List<ConfirmationSummaryModel> ListOfValueConfirmationSummary = GetValueConfimationSummary(Billingperiod);

           if(ListOfValueConfirmationSummary.Count > 0)
           {
               GenerateCSVForValueConfirmationSummary(ListOfValueConfirmationSummary, Billingperiod);
           }

           try
           {

               List<ConfirmationDetailModel> valueConfirmModel = new List<ConfirmationDetailModel>();

           // Select distinct memberid
               IEnumerable<int> memberIds = ListOfValueConfirmationDetails.Select(l => l.BillingMemberid).Distinct();

             // Iterate through all the memberids and generate csv
           foreach (var memberId in memberIds)
           {
               // fetch list of invoices for the given member id
               valueConfirmModel = ListOfValueConfirmationDetails.Where(l => l.BillingMemberid == memberId).ToList();

               if (valueConfirmModel.Count <= 0)
               {
                   break;
               }
               // Check for the ParticipateInValueConfirmation and AutomatedReportRequired ; if its true then generate csv filr for that invoices
               if (valueConfirmModel[0].ParticipateInValueConfirmation == 1)
               {
                   if (valueConfirmModel[0].AutomatedReportRequired == 1)
                   {

                       string ZipFleName = GenerateZipFile(memberId,valueConfirmModel[0].BillingAirlineNumber, Billingperiod);
                       
                       // Make an object of emailaddress
                       IEnumerable<string> emailAddress = null;


                       // fetch onlt those contacts which are havin value Confirmation reports Alerts flag to true
                       var contact = memberManager.GetContactsForContactType(memberId,
                                                                             ProcessingContactType.
                                                                                 ValueConfirmationReportsAlerts);
                       if (contact != null && contact.Count > 0)
                       {

                           // fetch email address for sending mails
                           emailAddress = contact.Select(c => c.EmailAddress);

                           // sending mail to billing entity
                           SendMailForValueConfirmationAlert(emailAddress, Billingperiod, ZipFleName);

                       }
                   }
               }
           }

           }// End try
           catch (Exception exception)
           {
               _logger.Debug("Exception occured while Generate csvFile :", exception);
               throw;
           }// End catch
       }

       /// <summary>
       /// This method is used to fetch data for value confirmation details from data base
       /// </summary>
       /// <returns>List of value confirmation details</returns>
       private List<ConfirmationDetailModel> GetValueConfimationDetails(BillingPeriod billingPeriod)
       {
           try
           {

               _logger.Debug(String.Format("From Filter- Year: {0}, Month: {1}", billingPeriod.Month, billingPeriod.Year));

               // Fetch all the invoices for the previous period from db
               ConfirmDetailModel = ConfirmationDetailParam.GetConfirmationDetails(0, billingPeriod.Month, billingPeriod.Period, 0, 0, "",
                                                                                   "",
                                                                                   "", "", "", "", "",
                                                                                   billingPeriod.Year,-1,-1,1);

               _logger.Debug("successFully Fetch data from database");

               return ConfirmDetailModel;
           }// End try
           catch (Exception ex)
           {
               _logger.Debug("Error occured when tyring to fetch data from database", ex);
               throw;
           }// end catch

       }// End GetValueConfimationDetails

       private List<ConfirmationSummaryModel> GetValueConfimationSummary(BillingPeriod billingPeriod)
       {
           try
           {

               _logger.Debug(String.Format("From Filter- Year: {0}, Month: {1}", billingPeriod.Month, billingPeriod.Year));

               ConfirmSummaryModel = ValueConfirmationSummary.GetValueConfirmationSummaryData(billingPeriod.Month,
                                                                                              billingPeriod.Year, billingPeriod.Period);
               _logger.Debug("successFully Fetch data from database");

               return ConfirmSummaryModel;
           }
           catch (Exception ex)
           {
               _logger.Debug("Error occured when tyring to fetch data from database", ex);
               throw;
           }// end catch
       }// end GetValueConfimationSummary

       private void GenerateCSVForValueConfirmationDetails(List<ConfirmationDetailModel> ValueConfirmationModel, BillingPeriod billingPeriod)
       {
           List<ConfirmationDetailModel> valueConfirmModel = new List<ConfirmationDetailModel>();

           // Select distinct memberid
           IEnumerable<int> memberIds = ValueConfirmationModel.Select(l => l.BillingMemberid).Distinct();

             // Iterate through all the memberids and generate csv
           foreach (var memberId in memberIds)
           {
               // fetch list of invoices for the given member id
               valueConfirmModel = ValueConfirmationModel.Where(l => l.BillingMemberid == memberId).ToList();

               if (valueConfirmModel.Count <= 0)
               {
                   break;
               }

               // Check for the ParticipateInValueConfirmation and AutomatedReportRequired ; if its true then generate csv filr for that invoices
                   if (valueConfirmModel[0].ParticipateInValueConfirmation == 1)
                   {
                       if (valueConfirmModel[0].AutomatedReportRequired == 1)
                       {
                           string filePath = GetOutputFilePath(valueConfirmModel[0].BillingAirlineNumber, billingPeriod);

                           // Fetch csv file name
                           var CsvFileName = GetOutputCsvFileName(valueConfirmModel[0].BillingAirlineNumber,
                                                                  billingPeriod.Month, billingPeriod.Period,
                                                                  billingPeriod.Year, "D");

                           // Combine complete path of csv file
                           var OutputCsvpath = Path.Combine(filePath, CsvFileName);

                           foreach (ConfirmationDetailModel t in valueConfirmModel)
                           {
                               t.BilledAmountUSD = Convert.ToDouble(Math.Abs(t.BilledAmountUSD).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                                    CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

                               t.ProrateAmountasperATPCO = Convert.ToDouble(Math.Abs(t.ProrateAmountasperATPCO).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                   CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

                               t.BilledTotalTaxAmountUSD = Convert.ToDouble(Math.Abs(t.BilledTotalTaxAmountUSD).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                  CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

                               t.TotalTaxAmountasperATPCO = Convert.ToDouble(Math.Abs(t.TotalTaxAmountasperATPCO).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                  CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

                               t.BilledISCPer = Convert.ToDouble(Math.Abs(t.BilledISCPer).ToString("N3").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                  CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

                               t.ISCFeePer = Convert.ToDouble(Math.Abs(t.ISCFeePer).ToString("N3").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                  CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

                               t.BilledHandlingFeeAmountUSD = Convert.ToDouble(Math.Abs(t.BilledHandlingFeeAmountUSD).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                 CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

                               t.HandlingFeeAmount = Convert.ToDouble(Math.Abs(t.HandlingFeeAmount).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

                               t.BilledUATPPercentage = Convert.ToDouble(Math.Abs(ConvertUtil.Round(t.BilledUATPPercentage, 2)).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

                               t.UATPPercentage = Convert.ToDouble(Math.Abs(ConvertUtil.Round(t.UATPPercentage, 2)).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                               CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

                               t.MonthOfSales = t.MonthofSale == 0
                                                    ? string.Empty
                                                    : Convert.ToString(
                                                        t.MonthofSale);

                               t.YearofSales = t.YearofSale == 0
                                                   ? string.Empty
                                                   : Convert.ToString(
                                                       t.YearofSale);
                           }

                           bool IsCsvgenerate = GenerateCSV(valueConfirmModel, OutputCsvpath);

                       }// End if
                   }// End if
           }// End forEach

       }// End GenerateCSVForValueConfirmation


       private void GenerateCSVForValueConfirmationSummary(List<ConfirmationSummaryModel> ValueConfirmationModel, BillingPeriod billingPeriod)
       {
           List<ConfirmationSummaryModel> valueConfirmModel = new List<ConfirmationSummaryModel>();

           // Select distinct memberid
           IEnumerable<int> memberIds = ValueConfirmationModel.Select(l => l.BillingMemberid).Distinct();

           // Iterate through all the memberids and generate csv
           foreach (var memberId in memberIds)
           {
               // fetch list of invoices for the given member id
               valueConfirmModel = ValueConfirmationModel.Where(l => l.BillingMemberid == memberId).OrderBy(l => l.BilledEntityCode).ThenBy(l => l.AgreementIndicatorValidated).ThenBy(l => l.ValidatedPMI).ToList();

               if (valueConfirmModel.Count <= 0)
               {
                   break;
               }

             var modelToIterate =
               valueConfirmModel.Where(v => v.ValidatedPMI != "Z").ToList();
             foreach (var aggrementIndicatorValidated in valueConfirmModel.Where(v => v.ValidatedPMI != "Z").Select(m => m.AgreementIndicatorValidated).Distinct().ToList())
             {
               if (aggrementIndicatorValidated != null)
               {
                 var totalCount = valueConfirmModel.Where(v => v.ValidatedPMI != "Z").Where(m => m.AgreementIndicatorValidated == aggrementIndicatorValidated).Sum(m => m.TotalBillingRecord);

                 foreach (var model in valueConfirmModel.Where(v => v.ValidatedPMI != "Z").Where(m => m.AgreementIndicatorValidated == aggrementIndicatorValidated).ToList())
                 {
                   model.AgreementPercentage = (Convert.ToDecimal(model.TotalBillingRecord) /
                                                totalCount) * 100;


                   model.PercentageAggrement = (model.AgreementPercentage) == 0
                                                 ? ""
                                                 : string.Format("{0:0.00}", model.AgreementPercentage);
                 
                 }
               }
             }

               //   for (int i = 0; i < valueConfirmModel.Count; i++ )
               //{
                   //if(valueConfirmModel[i].ValidatedPMI.ToUpper() != "Z")
                   //{
                       //if (valueConfirmModel[i].TotalBillingRecord > 0 && valueConfirmModel[i].InvoiceCount > 0)
                       //valueConfirmModel[i].AgreementPercentage = (Convert.ToDecimal(valueConfirmModel[i].TotalBillingRecord)/
                       //                                            (valueConfirmModel[i].InvoiceCount))*100;

                       
                       //valueConfirmModel[i].PercentageAggrement = (valueConfirmModel[i].AgreementPercentage) == 0
                       //                                                ? ""
                       //                                                : string.Format("{0:0.00}", valueConfirmModel[i].AgreementPercentage);

                     

                       
                   //}
               //}

               // Check for the ParticipateInValueConfirmation and AutomatedReportRequired ; if its true then generate csv filr for that invoices
               if (valueConfirmModel[0].ParticipateInValueConfirmation == 1)
               {
                   if (valueConfirmModel[0].AutomatedReportRequired == 1)
                   {
                       string filePath = GetOutputFilePath(valueConfirmModel[0].BillingnumericCode, billingPeriod);

                       // Fetch csv file name
                       var CsvFileName = GetOutputCsvFileName(valueConfirmModel[0].BillingnumericCode,
                                                              billingPeriod.Month, billingPeriod.Period,
                                                              billingPeriod.Year, "A");

                       // Combine complete path of csv file
                       var OutputCsvpath = Path.Combine(filePath, CsvFileName);

                       bool IsCsvgenerate = GenerateCSV(valueConfirmModel, OutputCsvpath);

                   }// End if
               }// End if
           }// End forEach

       }// End GenerateCSVForValueConfirmation

       private string GenerateZipFile(int memberId,string billingNumber , BillingPeriod billingPeriod)
       {
           try
           {
               string filePath = GetOutputFilePath(billingNumber, billingPeriod);

               string path = string.Format(@"{0}{1}{2}{3}{4}{5}", "PBVC-", billingNumber, billingPeriod.Year.ToString("0000", CultureInfo.InvariantCulture), billingPeriod.Month.ToString("00", CultureInfo.InvariantCulture), billingPeriod.Period.ToString("00", CultureInfo.InvariantCulture), ".ZIP");
               var fileName = path;
               _logger.Info("Path of the file" + path);

               // Fetch folder path
               string ZipfilePath =
                   FileIo.GetFtpDownloadFolderPath(
                       billingNumber);

               var fileLocation=ZipfilePath;
               ZipfilePath = ZipfilePath + path;

               bool isZipFile = FileIo.ZipOutputFolder(filePath, ZipfilePath);
                  
               _logger.Info("File successfully saved on " + ZipfilePath);
               var isInputFile = new IsInputFile
               {
                   BillingMonth = billingPeriod.Month,
                   BillingPeriod = billingPeriod.Period,
                   BillingYear = billingPeriod.Year,
                   FileDate = DateTime.UtcNow,
                   FileFormat = FileFormatType.BvcCsvReport,
                   FileLocation = fileLocation,
                   //File location should not contain file name
                   BillingCategory = (int) BillingCategoryType.Pax,
                   SenderReceiver =memberId ,
                   FileName = fileName,
                   FileStatus = FileStatusType.AvailableForDownload,
                   SenderRecieverType = (int)FileSenderRecieverType.Member,
                   FileVersion = "0.1",
                   IsIncoming = true,
                   ReceivedDate = DateTime.UtcNow,
                   SenderReceiverIP = Dns.GetHostByName(Dns.GetHostName()).AddressList.First().ToString(),
                   OutputFileDeliveryMethodId = 1
               };
               //IsInputFileRepository.Add(isInputFile);
              // UnitOfWork.CommitDefault();
               ReferenceManager.AddIsInputFile(isInputFile);
               
               _logger.Info("Add Entry in Is File Log for file:" + ZipfilePath);
               return path;

          }
           catch (Exception ex)
           {
               
               throw;
           }

                 
       }
       /// <summary>
       /// This method is used to send mails 
       /// </summary>
       /// <param name="ValueConfirmModel">List of email address</param>
       /// <param name="ValueConfirmModel">List of invoices</param>
       public void SendMailForValueConfirmationAlert(IEnumerable<String> toEmailList, BillingPeriod billingTime, string zipFleName)
       {
           try
           {
               //get an object of the EmailSender component
               var emailSender = Ioc.Resolve<IEmailSender>(typeof (IEmailSender));

               //get an instance of email settings  repository
               var emailSettingsRepository =
                   Ioc.Resolve<IRepository<EmailTemplate>>(typeof (IRepository<EmailTemplate>));

               //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
               var templatedTextGenerator =
                   Ioc.Resolve<ITemplatedTextGenerator>(typeof (ITemplatedTextGenerator));

               var dtf = CultureInfo.CurrentCulture.DateTimeFormat;
               string monthName = dtf.GetMonthName(billingTime.Month);
               string abbreviatedMonthName = dtf.GetAbbreviatedMonthName(billingTime.Month);

               var timeStamp = abbreviatedMonthName + " " +  billingTime.Year.ToString("0000", CultureInfo.InvariantCulture) + " "  + "P" + billingTime.Period.ToString("0", CultureInfo.InvariantCulture);
               //object of the nVelocity data dictionary
               var context = new VelocityContext();

               context.Put("fileName", zipFleName);
               context.Put("billingMonth",
                           timeStamp);
               //context.Put("billingYear",
               //            billingTime.Year.ToString("0000", CultureInfo.InvariantCulture));
               //context.Put("billingPeriod",
               //            billingTime.Period.ToString("00", CultureInfo.InvariantCulture));
               context.Put("SISOpsemailid", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

               //Get the eMail settings for reacp sheet overview 
               var emailSetting =
                   emailSettingsRepository.Get(
                       es => es.Id == (int) EmailTemplateId.ValueConfirmationReportsAlerts);

               //generate email body text f
               var body =
                   templatedTextGenerator.GenerateTemplatedText(
                       EmailTemplateId.ValueConfirmationReportsAlerts,
                       context);

               //create a mail object to send mail
               var overview = new MailMessage
                                  {
                                      From =
                                          new MailAddress(
                                          emailSetting.SingleOrDefault().FromEmailAddress)
                                  };
               overview.IsBodyHtml = true;

               foreach (var contact in toEmailList)
               {
                   _logger.Debug("Sending mail to members Pending Invoices: " + contact);
                   overview.To.Add(new MailAddress(contact));
               }

               //set subject of mail 
               overview.Subject = emailSetting.SingleOrDefault().Subject.Replace("$billingPeriod$",
                                                                                timeStamp);

               _logger.Debug("Sending mail to members for value confirmation alert: " + body);

               //set body text of mail
               overview.Body = body;

               _logger.Debug("Sending mail to members for value confirmation alert: " + overview.Subject);

               //send the mail
               emailSender.Send(overview);

               //clear nvelocity context data
               context = null;

               _logger.Debug("SuccessFully sending messages to server");

           }// End try
           catch (Exception exception)
           {
               _logger.Debug("Exception occured while sending sms", exception);
               throw;
           }// end catch

       }//SendMailForValueConfirmationAlert()


       /// <summary>
       /// Generate csv file for the given invoices
       /// </summary>
       /// <param name="detailModel">List of invoices</param>
       /// <param name="outputPath">Output file path</param>
       /// <returns></returns>
       private bool GenerateCSV<T>(List<T> detailModel , string outputPath)
       {
           if(detailModel.Count <= 0)
           {
               return false;
           }

           var sbCsvData = new StringBuilder();

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
           //sbCsvData.Remove(sbCsvData.Length - 1, 1);
           sbCsvData.AppendLine();

           // Write headers data in CSV
           for (var propCollectionCount = 0; propCollectionCount <= detailModel.Count - 1; propCollectionCount++)
           {
               T item = detailModel[propCollectionCount];

               var propInfoCount = 0;
               foreach (var propInfo in displayPropInfos)
               {
                   var objectValue = item.GetType().GetProperty(propInfo.Name).GetValue(item, null);
                   if (objectValue != null)
                   {
                       var value = objectValue.ToString();

                       //if (objectValue.GetType().Equals(typeof(decimal)))
                       //{
                       //    value = ((decimal)objectValue).ToString("00.000");
                       //}

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
               sbCsvData.Remove(sbCsvData.Length - 1, 1);
               sbCsvData.AppendLine();
           }

           if (string.IsNullOrEmpty(sbCsvData.ToString()))
           {
               return false;
           }

           System.IO.File.WriteAllText(outputPath, sbCsvData.ToString());

           return System.IO.File.Exists(outputPath);
       }// End GenerateCSV

       private string GetOutputCsvFileName(string billingNumericCode , int billingMonth , int billingPeriod, int billingyear, string csvType)
       {
           // Create output file prefix.
           var outputFilePrefix = string.Format(@"{0}{1}-{2}{3}{4}{5}", "PBVC",csvType,
                                               billingNumericCode, billingyear.ToString("0000", CultureInfo.InvariantCulture),billingMonth.ToString("00", CultureInfo.InvariantCulture), billingPeriod.ToString("00",CultureInfo.InvariantCulture));
           var outputCsvFileName = outputFilePrefix + ".CSV";

           return outputCsvFileName;
       }

       private string GetOutputFilePath(string billingNumericCode , BillingPeriod billingPeriod)
       {
           string csvPath;
           csvPath = Path.GetTempPath();
           var basePath = string.Format(@"{0}\{1}{2}{3}{4}{5}", csvPath, "PBVC-", billingNumericCode, billingPeriod.Year.ToString("0000", CultureInfo.InvariantCulture), billingPeriod.Month.ToString("00", CultureInfo.InvariantCulture), billingPeriod.Period.ToString("00", CultureInfo.InvariantCulture));

           if (!Directory.Exists(basePath))
           {
               Directory.CreateDirectory(basePath);
           }
           return basePath;
       }
    }
}
 
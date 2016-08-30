using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Business;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Reports.ConfirmationDetail;
using Iata.IS.Core;
using Iata.IS.Core.File;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Reports.ConfirmationDetails;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
    public class ConfirmationDetailController : ISController
    {
        private readonly ICalendarManager _calenderManager;
        public readonly IConfirmationDetail _confirmationDetail;

        public ConfirmationDetailController(ICalendarManager calenderManager, IConfirmationDetail confirmationDetail)
        {
            _calenderManager = calenderManager;
            _confirmationDetail = confirmationDetail;
        }
        //
        // GET: /Reports/ConfirmationDetail/

        int memberId = SessionUtil.MemberId;

        [ISAuthorize(Business.Security.Permissions.Reports.Pax.BvcDetails)]
        public ActionResult ConfirmationDetail()
        {
            IsMemberNullOrEmpty(SessionUtil.MemberId);
            var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(Model.Enums.ClearingHouse.Ich);
            ViewData["MembrId"] = memberId;
            ViewData["CurrentMonth"] = currentPeriod.Month;
            ViewData["CurrentPeriod"] = currentPeriod.Period;
            ViewData["CurrentYear"] = currentPeriod.Year;
            return View();
        }

        [ISAuthorize(Business.Security.Permissions.Reports.Pax.BvcDetails)]
        [HttpGet]
        public ActionResult ExportToCSV(string clearanceMonth, string periodNo, string biligAirlineNo, string biledAirlineNo, string invoiceNo, string issuAirline, string originalPMI, string validatedPMI, string agrmntIndiSupplied, string agrmntIndiValidated, string atpcoReasCd, string memberId, string billingYear)
        {
            try
            {
                //Get Temp folder path
              var tempFolderPath = FileIo.GetForlderPath(SFRFolderPath.ISBvcCsvFolder);
                if (!Directory.Exists(tempFolderPath))
                    Directory.CreateDirectory(tempFolderPath);
                // Delete Oldest files from Temp Folder
                DeleteOldestFilesFromTempFolder(tempFolderPath);

                var reportFileName = tempFolderPath + @"\PAX-BVCReport" + DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ss");

                if (System.IO.File.Exists(reportFileName+ ".zip"))
                    System.IO.File.Delete(reportFileName + ".zip");

                CreateCSV(reportFileName, memberId, clearanceMonth, periodNo, biligAirlineNo, biledAirlineNo, invoiceNo, issuAirline, originalPMI, validatedPMI, agrmntIndiSupplied, agrmntIndiValidated, atpcoReasCd, billingYear);

                string fileName = System.IO.Path.GetFileName(reportFileName);

                string contentType;
                var reg = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileName);
                if (reg != null)
                {
                    contentType = reg.GetValue("Content Type") as string;
                }
                else
                {
                    contentType = "application/zip";
                }

               //FileLocation = System.IO.Path.GetFullPath(FileLocation);
                reportFileName =
                    System.IO.Path.GetFullPath(tempFolderPath + @"\" + System.IO.Path.GetFileName(reportFileName + ".zip"));

                if (!System.IO.File.Exists(reportFileName))
                {
                    var memoryStream = ConvertUtil.GetMemoryStreamForMessage("No Record Found.");

                    return File(memoryStream, "text/plain", "Download-CSV.txt");
                }
                return File(reportFileName, contentType, System.IO.Path.GetFileName(reportFileName));
            }
            catch (Exception exception)
            {
                var memoryStream = ConvertUtil.GetMemoryStreamForMessage("Error occurred while downloading file.");
                return File(memoryStream, "text/plain", "Download-CSV-Error.txt");
            }
        }

        private void CreateCSV(string reportFileName, string memberId, string clearanceMonth, string periodNo, string biligAirlineNo, string biledAirlineNo, string invoiceNo, string issuAirline, string originalPMI, string validatedPMI, string agrmntIndiSupplied, string agrmntIndiValidated, string atpcoReasCd, string billingYear)
        {
            if (!string.IsNullOrWhiteSpace(periodNo) && Convert.ToInt32(periodNo) == -1)
            {
                for (int i = 1; i <= 4; i++)
                {
                    var csvListModel = GetCsvListModel(i, memberId, clearanceMonth, biligAirlineNo, biledAirlineNo, invoiceNo, issuAirline, originalPMI, validatedPMI, agrmntIndiSupplied, agrmntIndiValidated, atpcoReasCd, billingYear);

                    var couponSpecialRecords = new List<SpecialRecord>();

                    CsvProcessor.GenerateCsvReport(csvListModel,
                                                   reportFileName + "_temp" + i + ".csv",
                                                   couponSpecialRecords, i == 1 ? true : false);

                }
                // Append Data in single CSV file here for Each Period 
                // This Logic is implimentd here to handle large data in case of All Period
                using (var output = System.IO.File.Create(reportFileName + ".csv"))
                {
                    foreach (var file in new[] { reportFileName + "_temp1.csv" , 
                                                     reportFileName + "_temp2.csv",
                                                     reportFileName + "_temp3.csv",
                                                     reportFileName + "_temp4.csv"})
                    {
                        if (System.IO.File.Exists(file))
                        {
                            using (var input = System.IO.File.OpenRead(file))
                            {
                                input.CopyTo(output);
                            }
                        }
                    }
                }
                //delete all temp files
                for (int i = 1; i <= 4; i++)
                {
                    if (System.IO.File.Exists(reportFileName + "_temp" + i + ".csv"))
                    {
                        System.IO.File.Delete(reportFileName + "_temp" + i + ".csv");
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(periodNo) && Convert.ToInt32(periodNo) > 0)
            {
                var csvListModel = GetCsvListModel(Convert.ToInt32(periodNo), memberId, clearanceMonth, biligAirlineNo, biledAirlineNo, invoiceNo, issuAirline, originalPMI, validatedPMI, agrmntIndiSupplied, agrmntIndiValidated, atpcoReasCd, billingYear);

                var couponSpecialRecords = new List<SpecialRecord>();
                CsvProcessor.GenerateCsvReport(csvListModel,
                                               reportFileName + ".csv",
                                               couponSpecialRecords);
            }
            //Zip CSV file 
            if (System.IO.File.Exists(reportFileName + ".csv"))
            {
                //Delete here file if its length is zero
                FileInfo f = new FileInfo(reportFileName + ".csv");
                if(f.Length==0)
                 System.IO.File.Delete(reportFileName + ".csv");
                else
                FileIo.ZipOutputFile(reportFileName + ".csv");
            }
      
        }

        private List<CsvConfirmationDetailModel> GetCsvListModel(int periodNo, string memberId, string clearanceMonth, string biligAirlineNo, string biledAirlineNo, string invoiceNo, string issuAirline, string originalPMI, string validatedPMI, string agrmntIndiSupplied, string agrmntIndiValidated, string atpcoReasCd, string billingYear)
        {
            var filteredList =
                _confirmationDetail.GetConfirmationDetails(
                    string.IsNullOrWhiteSpace(memberId) ? 0 : Convert.ToInt32(memberId),
                    string.IsNullOrWhiteSpace(clearanceMonth) ? -1 : Convert.ToInt32(clearanceMonth),
                    periodNo,
                    string.IsNullOrWhiteSpace(biligAirlineNo) ? -1 : Convert.ToInt32(biligAirlineNo),
                    string.IsNullOrWhiteSpace(biledAirlineNo) ? -1 : Convert.ToInt32(biledAirlineNo),
                    invoiceNo,
                    issuAirline,
                    originalPMI,
                    validatedPMI,
                    agrmntIndiSupplied,
                    agrmntIndiValidated,
                    atpcoReasCd,
                    string.IsNullOrWhiteSpace(billingYear) ? 0 : Convert.ToInt32(billingYear),
                    -1, -1, 1);


            var csvListModel = new List<CsvConfirmationDetailModel>();

            if (filteredList != null)
            {

                foreach (var confirmationDetailModel in filteredList)
                {
                    var csvcomfimationobject = new CsvConfirmationDetailModel
                                                   {
                                                       BillingAirline = confirmationDetailModel.BillingAirline,
                                                       BillingAirlineNumber =
                                                           confirmationDetailModel.BillingAirlineNumber,
                                                       BillingPeriod = confirmationDetailModel.BillingPeriod,
                                                       ClearanceMonth = confirmationDetailModel.ClearanceMonth,
                                                       ClearanceYear = confirmationDetailModel.ClearanceYear,
                                                       InvoiceNumber = confirmationDetailModel.InvoiceNumber,
                                                       BilledAirline = confirmationDetailModel.BilledAirline,
                                                       BilledAirlineNumber =
                                                           confirmationDetailModel.BilledAirlineNumber,
                                                       MonthOfSales = confirmationDetailModel.MonthOfSales,
                                                       YearofSales = confirmationDetailModel.YearofSales,
                                                       IssuingAirline = confirmationDetailModel.IssuingAirline,
                                                       DocumentNumber = confirmationDetailModel.DocumentNumber,
                                                       CouponNumber = confirmationDetailModel.CouponNumber,
                                                       OriginalPMI = confirmationDetailModel.OriginalPMI,
                                                       ValidatedPMI = confirmationDetailModel.ValidatedPMI,
                                                       AgreementIndicatorSupplied =
                                                           confirmationDetailModel.AgreementIndicatorSupplied,
                                                       AgreementIndicatorValidated =
                                                           confirmationDetailModel.AgreementIndicatorValidated,
                                                       ProrateMethodologySupplied =
                                                           confirmationDetailModel.ProrateMethodologySupplied,
                                                       ProrateMethodologyValidated =
                                                           confirmationDetailModel.ProrateMethodologyValidated,
                                                       NFPReasonCodeSupplied =
                                                           confirmationDetailModel.NFPReasonCodeSupplied,
                                                       NFPReasonCodeValidated =
                                                           confirmationDetailModel.NFPReasonCodeValidated,
                                                       BilledAmountUSD = confirmationDetailModel.BilledAmountUSD,
                                                       ProrateAmountasperATPCO =
                                                           confirmationDetailModel.ProrateAmountasperATPCO,
                                                       ProrateAmountBaseSupplied =
                                                           confirmationDetailModel.ProrateAmountBaseSupplied,
                                                       ProrateAmountBaseATPCO =
                                                           confirmationDetailModel.ProrateAmountBaseATPCO,
                                                       BilledTotalTaxAmountUSD =
                                                           confirmationDetailModel.BilledTotalTaxAmountUSD,
                                                       TotalTaxAmountasperATPCO =
                                                           confirmationDetailModel.TotalTaxAmountasperATPCO,
                                                       PublishedTaxAmountCurrency1 =
                                                           confirmationDetailModel.PublishedTaxAmountCurrency1,
                                                       PublishedTaxAmountCurrency2 =
                                                           confirmationDetailModel.PublishedTaxAmountCurrency2,
                                                       PublishedTaxAmountCurrency3 =
                                                           confirmationDetailModel.PublishedTaxAmountCurrency3,
                                                       PublishedTaxAmountCurrency4 =
                                                           confirmationDetailModel.PublishedTaxAmountCurrency4,
                                                       BilledISCPer = confirmationDetailModel.BilledISCPer,
                                                       ISCFeePer = confirmationDetailModel.ISCFeePer,
                                                       BilledHandlingFeeAmountUSD =
                                                           confirmationDetailModel.BilledHandlingFeeAmountUSD,
                                                       BilledUATPPercentage =
                                                           confirmationDetailModel.BilledUATPPercentage,
                                                       UATPPercentage = confirmationDetailModel.UATPPercentage,
                                                       ATPCOReasonCode = confirmationDetailModel.ATPCOReasonCode

                                                   };

                    csvListModel.Add(csvcomfimationobject);
                }
            }
            return csvListModel;
        }
        
        private void DeleteOldestFilesFromTempFolder(string path)
        {
            var tempDownloadDir = new DirectoryInfo(path);
            foreach (FileInfo file in tempDownloadDir.GetFiles())
            {
                if (file.CreationTimeUtc <= DateTime.UtcNow.AddDays(-1))
                {
                    file.Delete();
                }
            }

        }

    }
}

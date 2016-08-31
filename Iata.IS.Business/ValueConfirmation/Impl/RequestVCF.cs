using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Common;
using Iata.IS.Business.FileCore;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Data.SetInvoiceStatus;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.ValueConfirmation;
using log4net;
using Iata.IS.Data.ValueConfirmation;

namespace Iata.IS.Business.ValueConfirmation.Impl
{
  class RequestVCF: IRequestVCF
  {
    // Logger instance.
    private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public ISetInvoiceStatus IsetInvoiceStatus { get; set; }

    public void GenerateRequestVCF()
    {
      ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
      //get an object of the CalendarManager component
      var calendarMgr = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
        int regenerateFlag = 0;
        Guid requestId = Guid.NewGuid();
      //Retrieve current open billing periods for both clearing houses
      var currentIchBillingPeriod = calendarMgr.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
      //var currentAchBillingPeriod = calendarMgr.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ach);
      GenerateRequestVCFInternal(currentIchBillingPeriod, requestId, regenerateFlag);
     
    }

    public void GenerateRequestVCFInternal(BillingPeriod currentIchBillingPeriod, Guid requestId, int regenerateFlag = 0)
    {
        logger.InfoFormat("Request VCF Service For Request. [{0}] ", requestId);
        //Retrieve current open billing periods for both clearing houses

        //var currentIchBillingPeriod = calendarMgr.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
        //var currentAchBillingPeriod = calendarMgr.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ach);

        ////temporarily taking fake objects as ICH and ACH billing periods
        //var currentIchBillingPeriod = new BillingPeriod(2010, 12, 01);
        //var currentAchBillingPeriod = new BillingPeriod(2010, 12, 01);

        //get an object of the requestVCFRepository
        var requestVCFRepository = Ioc.Resolve<IRequestValueConfirmationRepository>(typeof(IRequestValueConfirmationRepository));
        //Call the sp to get the list of Request VCF Coupons
        var requestVCFCouponList = requestVCFRepository.GetRequestValueConfirmationData(currentIchBillingPeriod, currentIchBillingPeriod, SystemParameters.Instance.BVCDetails.MaxCouponRecordsPerVCF, regenerateFlag, requestId);
        
        logger.InfoFormat("Request VCF Service SP executed. [{0}] coupons returned.", requestVCFCouponList.Count);

        requestVCFRepository = null;
        if (requestVCFCouponList.Count == 0)
        {
            //Flag set to 2 for delete data from temp table 
            regenerateFlag = 2;
            var requestVcfRepository = Ioc.Resolve<IRequestValueConfirmationRepository>(typeof(IRequestValueConfirmationRepository));
            var pendingRequestVCFCouponList = requestVcfRepository.GetRequestValueConfirmationData(currentIchBillingPeriod, currentIchBillingPeriod, SystemParameters.Instance.BVCDetails.MaxCouponRecordsPerVCF, regenerateFlag, requestId);
            return;
        }

        CreateRequestVCFFile(currentIchBillingPeriod, requestVCFCouponList, requestId, regenerateFlag);
    }

    private void CreateRequestVCFFile(BillingPeriod currentIchBillingPeriod, List<RequestVCFCoupon> requestVCFCouponList, Guid requestId, int regenerateFlag)
    {
        string setInvoiceStatus = string.Empty;
      try
      {
        StringBuilder stringBuilderVCFGroupHeaderAndCoupons = new StringBuilder();
        StringBuilder stringBuilderCouponIds = new StringBuilder();
        const string seperator = ",";

        //var invoices = requestVCFCouponList.Select(c => c.InvoiceId).Distinct().ToList();

        // Logic to convert all the UniqueInvoiceNumber in Byte[] to string.
       // setInvoiceStatus = string.Join(",", invoices.Select(invoice => invoice).ToArray());

       // IsetInvoiceStatus.SetStatusOfInvoices(setInvoiceStatus, "P", "VCFREQUEST"); 

        long recordSequenceNumber = 1;
        //Todo: Convert Accounting Code to numeric.
        var currentBillingMemberAcctCode = requestVCFCouponList[0].BillingMemberAccountingCode;
        var currentBillingMemberAcctCodeConverted = ConvertAlhpaNumAccountingCodeToNumeric(currentBillingMemberAcctCode);
        var currentBilledMemberAcctCode = requestVCFCouponList[0].BilledMemberAccountingCode;
        var currentBilledMemberAcctCodeConverted = ConvertAlhpaNumAccountingCodeToNumeric(currentBilledMemberAcctCode);
        var currentBillingDate = requestVCFCouponList[0].BillingDate;

        //Write the first Group Header Record
        stringBuilderVCFGroupHeaderAndCoupons.Append("01");
        stringBuilderVCFGroupHeaderAndCoupons.Append(recordSequenceNumber.ToString().PadLeft(10,'0'));
        stringBuilderVCFGroupHeaderAndCoupons.Append(currentBillingMemberAcctCodeConverted.PadLeft(4, '0'));
        stringBuilderVCFGroupHeaderAndCoupons.Append(currentBilledMemberAcctCodeConverted.PadLeft(4, '0'));
        stringBuilderVCFGroupHeaderAndCoupons.Append(currentBillingDate);
        stringBuilderVCFGroupHeaderAndCoupons.Append(String.Empty.PadLeft(374));
        //Add Cr LF chars
        stringBuilderVCFGroupHeaderAndCoupons.Append("\r\n");

        for(var i = 0; i < requestVCFCouponList.Count; i++)
        {
          var cr = requestVCFCouponList[i];
          recordSequenceNumber += 1;
          if(!(currentBillingMemberAcctCode == cr.BillingMemberAccountingCode && currentBilledMemberAcctCode == cr.BilledMemberAccountingCode
          && currentBillingDate == cr.BillingDate))
          {
            currentBillingMemberAcctCode = cr.BillingMemberAccountingCode;
            currentBillingMemberAcctCodeConverted = ConvertAlhpaNumAccountingCodeToNumeric(currentBillingMemberAcctCode);
            currentBilledMemberAcctCode = cr.BilledMemberAccountingCode;
            currentBilledMemberAcctCodeConverted = ConvertAlhpaNumAccountingCodeToNumeric(currentBilledMemberAcctCode);
            currentBillingDate = cr.BillingDate;

            //Write the Group Header Record
            stringBuilderVCFGroupHeaderAndCoupons.Append("01");
            stringBuilderVCFGroupHeaderAndCoupons.Append(recordSequenceNumber.ToString().PadLeft(10,'0'));
            stringBuilderVCFGroupHeaderAndCoupons.Append(currentBillingMemberAcctCodeConverted.PadLeft(4, '0'));
            stringBuilderVCFGroupHeaderAndCoupons.Append(currentBilledMemberAcctCodeConverted.PadLeft(4, '0'));
            stringBuilderVCFGroupHeaderAndCoupons.Append(currentBillingDate);
            stringBuilderVCFGroupHeaderAndCoupons.Append(String.Empty.PadLeft(374));
            //Add Cr LF chars
            stringBuilderVCFGroupHeaderAndCoupons.Append("\r\n");
            recordSequenceNumber += 1;
          }

          //Write the Coupon record
          stringBuilderVCFGroupHeaderAndCoupons.Append("02");
          stringBuilderVCFGroupHeaderAndCoupons.Append(recordSequenceNumber.ToString().PadLeft(10,'0'));
          stringBuilderVCFGroupHeaderAndCoupons.Append(currentBillingMemberAcctCodeConverted.PadLeft(4, '0'));
          stringBuilderVCFGroupHeaderAndCoupons.Append(currentBilledMemberAcctCodeConverted.PadLeft(4, '0'));
          stringBuilderVCFGroupHeaderAndCoupons.Append(cr.InvoiceNo.PadRight(10,' '));
          stringBuilderVCFGroupHeaderAndCoupons.Append(currentBillingDate);
          stringBuilderVCFGroupHeaderAndCoupons.Append(cr.PeriodNo.ToString().PadLeft(2,'0'));
          stringBuilderVCFGroupHeaderAndCoupons.Append(cr.BatchSequenceNumber.ToString().PadLeft(5,'0'));
          stringBuilderVCFGroupHeaderAndCoupons.Append(cr.BatchRecordSequenceNumber.ToString().PadLeft(5,'0'));
          stringBuilderVCFGroupHeaderAndCoupons.Append(cr.TicketCouponNumber.ToString().PadLeft(2,'0'));
          stringBuilderVCFGroupHeaderAndCoupons.Append(ConvertAlhpaNumAccountingCodeToNumeric(cr.TicketIssuingAirline).PadLeft(4,'0'));
          stringBuilderVCFGroupHeaderAndCoupons.Append(cr.TicketDocumentNo.ToString().PadLeft(11,'0'));
          stringBuilderVCFGroupHeaderAndCoupons.Append(cr.CheckDigit);
          stringBuilderVCFGroupHeaderAndCoupons.Append('I');
          stringBuilderVCFGroupHeaderAndCoupons.Append('I');
          stringBuilderVCFGroupHeaderAndCoupons.Append(cr.ListingCurrencyCodeNum.ToString().PadLeft(3,'0'));
          stringBuilderVCFGroupHeaderAndCoupons.Append(cr.BillingCurrencyCodeNum.ToString().PadLeft(3,'0'));


          stringBuilderVCFGroupHeaderAndCoupons.Append(
            Math.Abs(cr.ExchangeRate).ToString("N5").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty)
            .Replace(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(16, '0'));

          stringBuilderVCFGroupHeaderAndCoupons.Append(cr.SourceCode.ToString().PadRight(2, ' '));
          
          stringBuilderVCFGroupHeaderAndCoupons.Append(
            Math.Abs(cr.CouponGrossValue).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty)
            .Replace(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(11, '0'));

          stringBuilderVCFGroupHeaderAndCoupons.Append(
            Math.Abs(cr.InterServChargePercent).ToString("N3").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty)
            .Replace(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(5, '0'));

          stringBuilderVCFGroupHeaderAndCoupons.Append(
            Math.Abs(cr.HandlingFeeAmount).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty)
            .Replace(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(9, '0'));

          

          stringBuilderVCFGroupHeaderAndCoupons.Append(
            Math.Abs(cr.UATPPercent).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty)
            .Replace(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(4, '0'));
          stringBuilderVCFGroupHeaderAndCoupons.Append(String.Empty.PadLeft(11));

          stringBuilderVCFGroupHeaderAndCoupons.Append(
            Math.Abs(cr.CouponTaxAmount).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty)
            .Replace(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(11, '0'));
          stringBuilderVCFGroupHeaderAndCoupons.Append((cr.ProrateMethodology == null) ? " " : cr.ProrateMethodology.PadLeft(1));
          stringBuilderVCFGroupHeaderAndCoupons.Append((cr.AgreementIndicator == null) ? "  " : cr.AgreementIndicator.PadRight(2));

          stringBuilderVCFGroupHeaderAndCoupons.Append(cr.CouponRecordId);
          
          stringBuilderVCFGroupHeaderAndCoupons.Append(String.Empty.PadLeft(30));

          stringBuilderVCFGroupHeaderAndCoupons.Append(String.Empty.PadLeft(8));
          stringBuilderVCFGroupHeaderAndCoupons.Append(String.Empty.PadLeft(26,'0'));
          stringBuilderVCFGroupHeaderAndCoupons.Append(String.Empty.PadLeft(3));
          stringBuilderVCFGroupHeaderAndCoupons.Append(String.Empty.PadLeft(22,'0'));
          stringBuilderVCFGroupHeaderAndCoupons.Append(String.Empty.PadLeft(12));
          stringBuilderVCFGroupHeaderAndCoupons.Append(String.Empty.PadLeft(27, '0'));
          stringBuilderVCFGroupHeaderAndCoupons.Append(String.Empty.PadLeft(3));
          stringBuilderVCFGroupHeaderAndCoupons.Append(String.Empty.PadLeft(4, '0'));
          stringBuilderVCFGroupHeaderAndCoupons.Append(String.Empty.PadLeft(87));
        
          //Add Cr LF chars
          stringBuilderVCFGroupHeaderAndCoupons.Append("\r\n");

          stringBuilderCouponIds.Append(cr.CouponRecordId);
          stringBuilderCouponIds.Append(seperator);
        }

        logger.InfoFormat("Request VCF Service string builder ready.");

        StringBuilder stringBuilderVCFHeader = new StringBuilder();
        string VCKKey = Guid.NewGuid().ToString().PadRight(50);

        stringBuilderVCFHeader.Append("VCF");
        stringBuilderVCFHeader.Append("IS ");
        stringBuilderVCFHeader.Append(String.Empty.PadLeft(100));
        var todaysDate = DateTime.UtcNow;
        stringBuilderVCFHeader.Append(todaysDate.ToString("yyyy"));
        stringBuilderVCFHeader.Append(todaysDate.ToString("MM"));
        stringBuilderVCFHeader.Append(todaysDate.ToString("dd"));
        stringBuilderVCFHeader.Append(recordSequenceNumber.ToString().PadLeft(10, '0'));
        stringBuilderVCFHeader.Append(VCKKey);
        stringBuilderVCFHeader.Append(String.Empty.PadLeft(226));
        //Add Cr LF chars
        stringBuilderVCFHeader.Append("\r\n");

        stringBuilderVCFGroupHeaderAndCoupons.Insert(0, stringBuilderVCFHeader.ToString());

        //Create ACH Recap Sheet file

        //var fileServerRepository = Ioc.Resolve<IRepository<FileServer>>(typeof(IRepository<FileServer>));

        //Get details of the Templates File Server
       // var requestVCFFileServerPath = fileServerRepository.Get(fs => fs.ServerType == "ATPCO Request Value Confirmation FTP Push" && fs.Status == 1);

        //combine root path of ach recap sheet file server and the ach recap sheet file name.
       // var fileServerEntry = requestVCFFileServerPath.SingleOrDefault();

        var requestVCFFilePath = FileIo.GetATPCOFTPDownloadFolderPath();  //fileServerEntry == null ? string.Empty : fileServerEntry.BasePath.Trim();

        var requestVCFFileName  = string.Empty;
        
        if (SystemParameters.Instance.Atpco.ApplicationMode.ToUpper() == "PROD".ToUpper())
        {
          requestVCFFileName =    "V16.PROD.XMT16RCN.BVCCPN";
        }
        else
        {
          requestVCFFileName = "V16.UTST.XMT16RCN.BVCCPN";
        }


        var requestVCFZipFileName = string.Empty;

        if (SystemParameters.Instance.Atpco.ApplicationMode.ToUpper() == "PROD".ToUpper())
        {
          requestVCFZipFileName = "'V16.PROD.XMT16RCZ.BVCCPN'" + "_" + VCKKey.Trim();
        }
        else
        {
          requestVCFZipFileName = "'V16.UTST.XMT16RCZ.BVCCPN'" + "_" + VCKKey.Trim();
        }


        logger.Info("Request VCF File being generated at path: " + requestVCFFilePath);

        var streamWriter = File.CreateText(requestVCFFilePath + "\\" + requestVCFFileName);

        streamWriter.Write(stringBuilderVCFGroupHeaderAndCoupons.ToString().TrimEnd(new char[] { '\r', '\n' }));

        streamWriter.Close();

        var zipFileLocation = FileIo.ZipOutputFile(requestVCFFilePath + "\\" + requestVCFFileName, requestVCFFilePath, requestVCFZipFileName);

        logger.InfoFormat("Request VCF File created successfully. Now will update the invoices and coupons.");

        StringBuilder stringBuilderInvoiceIds = new StringBuilder();
       

        //get the distinct invoice ids
        var distinctInvoiceIds = (from cr in requestVCFCouponList
                                  select cr.InvoiceId).Distinct();

        //now loop through the distinct invoice ids
        foreach (var invoiceId in distinctInvoiceIds)
        {
          stringBuilderInvoiceIds.Append(invoiceId);
          stringBuilderInvoiceIds.Append(seperator);
        }

        //get an object of the requestVCFRepository
        var requestVCFRepository = Ioc.Resolve<IRequestValueConfirmationRepository>(typeof(IRequestValueConfirmationRepository));

        //AIASLATimeInSeconds is the SLA seconds
        //AIASLANoOfRecords is the Number for records for which the SLA is defined
        //DateTime ExpectedResponseDateTime = todaysDate.AddSeconds(Math.Ceiling((double)recordSequenceNumber / SystemParameters.Instance.BVCDetails.AIASLANoOfRecords) * SystemParameters.Instance.BVCDetails.AIASLATimeInSeconds);
        int ExpectedResponseTime = (int)((Math.Ceiling((double)recordSequenceNumber / SystemParameters.Instance.BVCDetails.AIASLANoOfRecords) * SystemParameters.Instance.BVCDetails.AIASLATimeInSeconds))/60;

       // requestVCFFileName = requestVCFFileName.Substring(0, requestVCFFileName.LastIndexOf('.')) + ".zip";

        logger.Info("Request VCF CouponIds" + stringBuilderCouponIds.ToString().TrimEnd(new char[] { ',' }));
        logger.Info("Request VCF InvoiceIds" + stringBuilderInvoiceIds.ToString().TrimEnd(new char[] { ',' }));
        logger.Info("Request VCF name" + requestVCFZipFileName);
        if (regenerateFlag == 0)
        {
            logger.Info("Update Invoice and Coupons for Requested to ATPCO and adding entry in IS FILE LOG table.");
            requestVCFRepository.UpdateInvoicesAndCouponsForRequestVCF(stringBuilderInvoiceIds.ToString().TrimEnd(new char[] { ',' }),
                                                                       stringBuilderCouponIds.ToString().TrimEnd(new char[] { ',' }),
                                                                       requestVCFZipFileName,
                                                                       requestVCFFilePath,
                                                                       (int) FileSenderRecieverType.ATPCO,
                                                                       VCKKey.Trim(),
                                                                       ExpectedResponseTime);
        }
        if (regenerateFlag == 1)
        {
            logger.Info("Adding entry of regenerated file in IS FILE LOG table.");
            var IsInputFileRepository = Ioc.Resolve<IRepository<IsInputFile>>(typeof(IRepository<IsInputFile>));
            //Add entry in IS FILE LOG Table after regenerating ACH Recap Sheet.
            var isInputFile = new IsInputFile
            {
                BillingMonth = currentIchBillingPeriod.Month,
                BillingPeriod = currentIchBillingPeriod.Period,
                BillingYear = currentIchBillingPeriod.Year,
                BillingCategory =(int)BillingCategoryType.Pax,
                FileDate = DateTime.UtcNow,
                FileFormat = FileFormatType.ValueConfirmation,
                FileLocation = requestVCFFilePath,
                //File location should not contain file name
                FileName = requestVCFZipFileName,
                FileStatus = FileStatusType.AvailableForDownload,
                SenderRecieverType = (int)FileSenderRecieverType.ATPCO,
                FileVersion = "0.1",
                IsIncoming = true,
                ReceivedDate = DateTime.UtcNow,
                SenderReceiverIP = Dns.GetHostByName(Dns.GetHostName()).AddressList.First().ToString(),
                OutputFileDeliveryMethodId = 1
            };
            IsInputFileRepository.Add(isInputFile);
            UnitOfWork.CommitDefault();
        }
        logger.Info("Entry of file added in IS FILE LOG table.");
          //IsetInvoiceStatus.SetStatusOfInvoices(setInvoiceStatus, "S", "VCFREQUEST"); 
          GenerateRequestVCFInternal(currentIchBillingPeriod, requestId, regenerateFlag);
      }
      catch (Exception exception)
      {
        logger.Error("Exception occurred in CreateRequestVCFFile method of Request VCF Generation Service.", exception);
       // IsetInvoiceStatus.SetStatusOfInvoices(setInvoiceStatus, "E", "VCFREQUEST"); 
      }
    }

    private string ConvertAlhpaNumAccountingCodeToNumeric(string code)
    {
      var strBldr = new StringBuilder(String.Empty);
      foreach (var ch in code.ToCharArray())
      {
        if (Char.IsLetter(ch))
        {
          strBldr.Append((Convert.ToInt16(Char.ToUpper(ch)) - 55).ToString());
        }
        else
        {
          strBldr.Append(ch);
        }
      }
      return strBldr.ToString();
    }
  }
}

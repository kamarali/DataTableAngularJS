using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Iata.IS.Business.Reports.Pax;
using Iata.IS.Business.Reports.Pax.Impl;
using Iata.IS.Core.DI;
using Iata.IS.Core.File;
using Iata.IS.Data.Pax;
using Iata.IS.Data.Pax.Impl;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Model.Reports.Pax;
using log4net;
using Iata.IS.Model.Enums;

namespace Iata.IS.Business.Common.Impl
{
  public class PaxListingsReportGenerator : IPaxListingsReportGenerator
  {
    /// <summary>
    /// Creates the listings.
    /// 1] Interline Passenger RM Form F/XF Listing (applicable for Sampling ONLY)
    /// 2] Passenger - Sampling Form D Listing
    /// </summary>
    /// <param name="paxInvoice">The pax invoice.</param>
    /// <param name="listingPath">The listing path.</param>
    /// <param name="errors"></param>
    /// <param name="logger"></param>
    /// <param name="paxInvoiceCoupons"></param>
    public void CreatePaxListings(PaxInvoice paxInvoice, string listingPath, StringBuilder errors, ILog logger, IList<PrimeCoupon> paxInvoiceCoupons)
    {

      if (paxInvoice != null)
      {

        /* Report Logic		[Interline Passenger RM Form F/XF Listing (applicable for Sampling ONLY)]	
         * The RM Listing report is to be generated for each source code within an Invoice having Rejection memos
         * Gross, ISC , Other Commission, UATP, Handling Fee, VAT and Tax amounts show the values taking into the respective sign 
         * Even though PAX IS-IDEC supports only 2 decimals for amounts, 3 decimals will be shown */
        if (paxInvoice.BillingCode == (int)BillingCode.SamplingFormF || paxInvoice.BillingCode == (int)BillingCode.SamplingFormXF)
        {
          logger.Info("Interline Passenger RM Form F/XF Listing Report.");
          #region Pax Sampling RM Listings
          // Group rejection memos on source code 
          logger.Info("Grouping & iterating rejection memos on source code.");
          // Generating multiple reports on different threads will speed-up the overall process
          foreach (var rejectionMemos in paxInvoice.RejectionMemoRecord.GroupBy(rm => rm.SourceCodeId))
          {
            int rmserialNo = 1;
            var rmspecialRecords = new List<SpecialRecord>();
            var sampleRMListingrecords = new List<PaxSampleRMListingReport>();
            logger.InfoFormat("Populate PaxSampleRMListingReport record for source code [{0}].", rejectionMemos.Key);
            // Populate PaxSampleRMListingReport records and add it to collection and Sort the records in correct order.
            (from sampleRMListingRecord in rejectionMemos
             orderby sampleRMListingRecord.SourceCodeId, sampleRMListingRecord.BatchSequenceNumber, sampleRMListingRecord.RecordSequenceWithinBatch
             select sampleRMListingRecord).ToList().ForEach(rejectionMemo => sampleRMListingrecords.Add(PopulatePaxSampleRMReportRecord(paxInvoice, rejectionMemo, rmserialNo++)));
            if (sampleRMListingrecords.Count > 0)
            {
              logger.InfoFormat("Adding special records for source code [{0}].", rejectionMemos.Key);
              // Create Special records like GrossTotal and add it to SpecialRecord Collection

              #region Add Special Records

              rmspecialRecords.Add(new SpecialRecord
                                   {
                                     Cells = new List<SpecialCell>
                                                                {
                                                                  new SpecialCell
                                                                    {
                                                                      Key = "Serial No",
                                                                      Data = "Grand Total"
                                                                    },
                                                                  new SpecialCell
                                                                    {
                                                                      Key = "Rejection Memo",
                                                                      Data = sampleRMListingrecords.Count.ToString()
                                                                    },
                                                                  new SpecialCell
                                                                    {
                                                                      Key = "Gross Fare Value",
                                                                      Data =
                                                                        rejectionMemos.Sum(rm => rm.TotalGrossDifference).ToString(
                                                                          PaxReportConstants.DecimalStringFormat)
                                                                    },
                                                                  new SpecialCell
                                                                    {
                                                                      Key = "ISC Amount",
                                                                      Data =
                                                                        rejectionMemos.Sum(rm => rm.IscDifference).ToString(
                                                                          PaxReportConstants.DecimalStringFormat)
                                                                    },
                                                                  new SpecialCell
                                                                    {
                                                                      Key = "Other Comm Amount",
                                                                      Data =
                                                                        rejectionMemos.Sum(rm => rm.OtherCommissionDifference).ToString(
                                                                          PaxReportConstants.DecimalStringFormat)
                                                                    },
                                                                  new SpecialCell
                                                                    {
                                                                      Key = "UATP Amount",
                                                                      Data =
                                                                        rejectionMemos.Sum(rm => rm.UatpAmountDifference).ToString(
                                                                          PaxReportConstants.DecimalStringFormat)
                                                                    },
                                                                  new SpecialCell
                                                                    {
                                                                      Key = "Handling Fee Amount",
                                                                      Data =
                                                                        rejectionMemos.Sum(rm => rm.HandlingFeeAmountDifference).ToString(
                                                                          PaxReportConstants.DecimalStringFormat)
                                                                    },
                                                                  new SpecialCell
                                                                    {
                                                                      Key = "Tax Amount",
                                                                      Data =
                                                                        rejectionMemos.Sum(rm => rm.TotalTaxAmountDifference).ToString(
                                                                          PaxReportConstants.DecimalStringFormat)
                                                                    },
                                                                  new SpecialCell
                                                                    {
                                                                      Key = "VAT Amount",
                                                                      Data =
                                                                        rejectionMemos.Sum(rm => rm.TotalVatAmountDifference).ToString(
                                                                          PaxReportConstants.DecimalStringFormat)
                                                                    },
                                                                  new SpecialCell
                                                                    {
                                                                      Key = "Net Reject Amount",
                                                                      Data =
                                                                        rejectionMemos.Sum(rm => rm.TotalNetRejectAmount).ToString(
                                                                          PaxReportConstants.DecimalStringFormat)
                                                                    },
                                                                  new SpecialCell
                                                                    {
                                                                      Key = "Net Reject Amount x Constant",
                                                                      Data =
                                                                        rejectionMemos.Sum(rm => rm.TotalNetRejectAmountAfterSamplingConstant).
                                                                        ToString(PaxReportConstants.DecimalStringFormat)
                                                                    }
                                                                }

                                   });

              #endregion

              logger.InfoFormat("Generating csv report for source code [{0}].", rejectionMemos.Key);
              // Generate appropriate file name and pass it to GenerateCSVReport method to generate csv report
              CsvProcessor.GenerateCsvReport(sampleRMListingrecords,
                                             Path.Combine(listingPath,
                                                          string.Format("{0}SLSC{1}-{2}.CSV",
                                                                        PaxReportConstants.PaxBillingCategory,
                                                                        rejectionMemos.Key.ToString().PadLeft(2, '0'),
                                                                        paxInvoice.InvoiceNumber)).ToUpper(),
                                             rmspecialRecords);
              logger.InfoFormat("Interline Passenger RM Form F/XF Listing csv Report Generated  for source code [{0}] on thread [{1}].",
                                rejectionMemos.Key,
                                Thread.CurrentThread.ManagedThreadId);
            }
            sampleRMListingrecords.Clear();
            sampleRMListingrecords = null;
            rmspecialRecords.Clear();
            rmspecialRecords = null;
          }


          #endregion
        }

        /* Report Logic		[Passenger - Sampling Form D Listing]  
         * The Report listing is applicable for Billing Code 5 
         * As Form D is a listing report and not Invoice , From and To Airline Address are not required
         * Even though PAX IS-IDEC supports only 2 decimals for amounts, 3 decimals will be shown */
        if (paxInvoice.BillingCode == (int)BillingCode.SamplingFormDE)
        {
          logger.Info("Passenger - Sampling Form D Listing Report.");

          #region Pax Sampling Form D Listings

          var samplingFormDListingrecords = new List<PaxSamplingFormDListingReport>();
          int serialNo = 1;
          var specialRecords = new List<SpecialRecord>();
          logger.Info("Populating sampling FormD Listing report records...");
          // Populate Sampling FormD records and add it to collection and Sort the records in correct order.
          (from formDLisingRecord in paxInvoice.SamplingFormDRecord
           orderby formDLisingRecord.BatchNumberOfProvisionalInvoice, formDLisingRecord.RecordSeqNumberOfProvisionalInvoice
           select formDLisingRecord).ToList().ForEach(samplingFormDRecord => samplingFormDListingrecords.Add(PopulateSamplingFormDListingRecord(paxInvoice, samplingFormDRecord, serialNo++)));
          if (samplingFormDListingrecords.Count > 0)
          {
            logger.Info("Creating special records for sampling FormD Listing report...");
            // Create special records like GrossTotal and add it to SpecialRecord collection

            #region Add Special Records

            specialRecords.Add(new SpecialRecord
                                 {
                                   Cells = new List<SpecialCell>
                                           {
                                             new SpecialCell
                                               {
                                                 Key = "Serial No",
                                                 Data = "Grand Total"
                                               },
                                             new SpecialCell
                                               {
                                                 Key = "Document No",
                                                 Data = samplingFormDListingrecords.Count.ToString()
                                               },
                                             new SpecialCell
                                               {
                                                 Key = "Provisional Gross Amount/ALF",
                                                 Data =
                                                   paxInvoice.SamplingFormDRecord.Sum(sfd => sfd.ProvisionalGrossAlfAmount).ToString(
                                                     PaxReportConstants.DecimalStringFormat)
                                               },

                                             new SpecialCell
                                               {
                                                 Key = "Gross Fare Value",
                                                 Data =
                                                   paxInvoice.SamplingFormDRecord.Sum(sfd => sfd.EvaluatedGrossAmount).ToString(
                                                     PaxReportConstants.DecimalStringFormat)
                                               },
                                             new SpecialCell
                                               {
                                                 Key = "ISC Amount",
                                                 Data =
                                                   paxInvoice.SamplingFormDRecord.Sum(sfd => sfd.IscAmount).ToString(
                                                     PaxReportConstants.DecimalStringFormat)
                                               },
                                             new SpecialCell
                                               {
                                                 Key = "Other Comm Amount",
                                                 Data =
                                                   paxInvoice.SamplingFormDRecord.Sum(sfd => sfd.OtherCommissionAmount).ToString(
                                                     PaxReportConstants.DecimalStringFormat)
                                               },
                                             new SpecialCell
                                               {
                                                 Key = "UATP Amount",
                                                 Data =
                                                   paxInvoice.SamplingFormDRecord.Sum(sfd => sfd.UatpAmount).ToString(
                                                     PaxReportConstants.DecimalStringFormat)
                                               },
                                             new SpecialCell
                                               {
                                                 Key = "Handling Fee Amount",
                                                 Data =
                                                   paxInvoice.SamplingFormDRecord.Sum(sfd => sfd.HandlingFeeAmount).ToString(
                                                     PaxReportConstants.DecimalStringFormat)
                                               },
                                             new SpecialCell
                                               {
                                                 Key = "Tax Amount",
                                                 Data =
                                                   paxInvoice.SamplingFormDRecord.Sum(sfd => sfd.TaxAmount).ToString(
                                                     PaxReportConstants.DecimalStringFormat)
                                               },
                                             new SpecialCell
                                               {
                                                 Key = "VAT Amount",
                                                 Data =
                                                   paxInvoice.SamplingFormDRecord.Sum(sfd => sfd.VatAmount).ToString(
                                                     PaxReportConstants.DecimalStringFormat)
                                               },
                                             new SpecialCell
                                               {
                                                 Key = "Coupon Total Amount",
                                                 Data =
                                                   paxInvoice.SamplingFormDRecord.Sum(sfd => sfd.EvaluatedNetAmount).ToString(
                                                     PaxReportConstants.DecimalStringFormat)
                                               }
                                           }
                                 });

            #endregion

            logger.Info("Generating sampling FormD Listing report...");
            // Create appropriate file name and generate SamplingFormD csv report
            CsvProcessor.GenerateCsvReport(samplingFormDListingrecords,
                                           Path.Combine(listingPath, string.Format("{0}SFORMD-{1}.CSV", PaxReportConstants.PaxBillingCategory, paxInvoice.InvoiceNumber)).ToUpper(),
                                           specialRecords);
            logger.Info("Passenger - Sampling Form D Listing Report generated.");
          }

          samplingFormDListingrecords.Clear();
          samplingFormDListingrecords = null;
          specialRecords.Clear();
          specialRecords = null;

          #endregion
        }

        /* Report Logic [Interline Passenger Coupon Listing (applicable for Sampling as well as non-sampling)]
         * Separate listings will be produced for every Source Code 						
         * Show amounts and percentages values taking into the respective sign field into consideration.						
         * Even though PAX IS-IDEC supports only 2 decimals for amounts, 3 decimals will be shown */

        var filteredCoupons = paxInvoiceCoupons.GroupBy(cr => cr.SourceCodeId).ToList();
        if (filteredCoupons.Count > 0)  //paxInvoice.CouponDataRecord != null)
        {
          logger.DebugFormat("Interline Passenger Coupon Listing report. Coupon Count: {0}", filteredCoupons.Count);

          #region Interline Passenger Coupon Listing

          // Group CouponDataRecords by Source Code
          logger.Info("Grouping CouponDataRecords by Source Code...");
          // Generating multiple reports on different threads will speed-up the overall process
          foreach (var primeCoupons in filteredCoupons) //paxInvoice.CouponDataRecord.GroupBy(cr => cr.SourceCodeId))
          {
            var couponListingRecords = new List<PaxCouponListingReport>();
            var couponSpecialRecords = new List<SpecialRecord>();
            int couponSerialNo = 1;

            // Populate PaxCouponListing Report record and add it to collection
            logger.InfoFormat("Populating PaxCouponListing Report record for source code [{0}]...", primeCoupons.Key);
            // Sort the records in correct order.
            (from couponListingRecord in primeCoupons
             orderby couponListingRecord.SourceCode , couponListingRecord.BatchSequenceNumber , couponListingRecord.RecordSequenceWithinBatch
             select couponListingRecord).ToList().ForEach(primeCoupon => couponListingRecords.Add(PopulatePaxCouponListingRecord(paxInvoice, primeCoupon, couponSerialNo++)));

            if (couponListingRecords.Count > 0)
            {
              // Create special records like GrossTotal and add it to SpecialRecord collection
              logger.InfoFormat("Creating special records for PaxCouponListing Report  for source code [{0}]...", primeCoupons.Key);

              #region Add Special Records

              couponSpecialRecords.Add(new SpecialRecord
                                         {
                                           Cells = new List<SpecialCell>
                                                     {
                                                       new SpecialCell
                                                         {
                                                           Key = "Serial No",
                                                           Data = "Grand Total"
                                                         },
                                                       new SpecialCell
                                                         {
                                                           Key = "Document/Form/Serial/FIM No",
                                                           Data = couponListingRecords.Count.ToString()
                                                         },
                                                       new SpecialCell
                                                         {
                                                           Key = "Gross Fare Value",
                                                           Data =
                                                             primeCoupons.Sum(pc => pc.CouponGrossValueOrApplicableLocalFare).ToString(
                                                               PaxReportConstants.DecimalStringFormat)
                                                         },
                                                       new SpecialCell
                                                         {
                                                           Key = "ISC Amount",
                                                           Data =
                                                             primeCoupons.Sum(pc => pc.IscAmount).ToString(PaxReportConstants.DecimalStringFormat)
                                                         },
                                                       new SpecialCell
                                                         {
                                                           Key = "Other Comm Amount",
                                                           Data =
                                                             primeCoupons.Sum(pc => pc.OtherCommissionAmount).ToString(
                                                               PaxReportConstants.DecimalStringFormat)
                                                         },
                                                       new SpecialCell
                                                         {
                                                           Key = "UATP Amount",
                                                           Data =
                                                             primeCoupons.Sum(pc => pc.UatpAmount).ToString(PaxReportConstants.DecimalStringFormat)
                                                         },
                                                       new SpecialCell
                                                         {
                                                           Key = "Handling Fee Amount",
                                                           Data =
                                                             primeCoupons.Sum(pc => pc.HandlingFeeAmount).ToString(
                                                               PaxReportConstants.DecimalStringFormat)
                                                         },
                                                       new SpecialCell
                                                         {
                                                           Key = "Tax Amount",
                                                           Data =
                                                             primeCoupons.Sum(pc => pc.TaxAmount).ToString(PaxReportConstants.DecimalStringFormat)
                                                         },
                                                       new SpecialCell
                                                         {
                                                           Key = "VAT Amount",
                                                           Data =
                                                             primeCoupons.Sum(pc => pc.VatAmount).ToString(PaxReportConstants.DecimalStringFormat)
                                                         },
                                                       new SpecialCell
                                                         {
                                                           Key = "Coupon Total Amount",
                                                           Data =
                                                             primeCoupons.Sum(pc => pc.CouponTotalAmount).ToString(
                                                               PaxReportConstants.DecimalStringFormat)
                                                         }
                                                     }

                                         });

              #endregion

              // Generate appropriate file name and generate Pax Coupon listing csv report
              logger.InfoFormat("Generating PaxCouponListing Report  for source code [{0}]...", primeCoupons.Key);

              if (paxInvoice.BillingCode == (int) BillingCode.SamplingFormAB)
              {
                //For SamplingFormAB listing file name should start with PSASC instead of PNSSC.
                CsvProcessor.GenerateCsvReport(couponListingRecords,
                                               Path.Combine(listingPath,
                                                            string.Format("{0}SASC{1}-{2}.CSV",
                                                                          PaxReportConstants.PaxBillingCategory,
                                                                          primeCoupons.Key.ToString().PadLeft(2, '0'),
                                                                          paxInvoice.InvoiceNumber)).ToUpper(),
                                               couponSpecialRecords);
              }
              else
              {
                CsvProcessor.GenerateCsvReport(couponListingRecords,
                                               Path.Combine(listingPath,
                                                            string.Format("{0}NSSC{1}-{2}.CSV",
                                                                          PaxReportConstants.PaxBillingCategory,
                                                                          primeCoupons.Key.ToString().PadLeft(2, '0'),
                                                                          paxInvoice.InvoiceNumber)).ToUpper(),
                                               couponSpecialRecords);
              }
              logger.InfoFormat("Interline Passenger Coupon Listing report for source code [{0}] generated.", primeCoupons.Key);
            }

            couponListingRecords.Clear();
            couponListingRecords = null;
            couponSpecialRecords.Clear();
            couponSpecialRecords = null;
          }
          filteredCoupons.Clear();
          filteredCoupons = null;

          #endregion
        }

        /* Report Logic [Interline Passenger RM/BM/CM  Listing (applicable for Non-Sampling ONLY)]
         * "The RM/ Bm/ CM  Listing report is to be generated for each source code within an Invoice having Rejection memos, Billing Memos, Credit memos.
         * Gross, ISC , Other Comm, UATP, Handling Fee, VAT and Tax amounts show the values taking into the respective sign fields into consideration.
         * Even though PAX IS-IDEC supports only 2 decimals for amounts, 3 decimals will be shown 
         * Column Rejection Memo/Billing Memo/Credit Memo may contain commas, this needs to be handled appropriately
         * This Report would be downloaded from the Invoice search screen.*/
        if (paxInvoice.BillingCode == (int)BillingCode.NonSampling)
        {
          logger.InfoFormat("Interline Passenger NonSampling RM/BM/CM  Listing report");
          if (paxInvoice.RejectionMemoRecord != null && paxInvoice.RejectionMemoRecord.Count > 0)
          {
            logger.InfoFormat("NonSampling Rejection memos");
            #region Pax Non Sampling RM Listings
            // Group RejectionMemo Records on Source Code
            logger.InfoFormat("Grouping NonSampling Rejection memo records by source code...");
            // Parallel.ForEach will process different reports on different threads
            foreach (var rejectionMemos in paxInvoice.RejectionMemoRecord.GroupBy(rm => rm.SourceCodeId))
            {

              var nonSampleRMListingrecords = new List<PaxNonSamplingRmBmCmListingReport>();
              int rmSerialNo = 1;
              var rmSpecialRecords = new List<SpecialRecord>();

              // Populate Pax Non-Sampling rejection memo report record and add it to collection
              logger.InfoFormat("Populating NonSampling Rejection memo records for source code[{0}]...", rejectionMemos.Key);
              
              // Sort the records in correct order.
              (from nsRMListingrecord in rejectionMemos
               orderby nsRMListingrecord.SourceCodeId, nsRMListingrecord.BatchSequenceNumber, nsRMListingrecord.RecordSequenceWithinBatch
               select nsRMListingrecord).ToList().ForEach(rejectionMemo => nonSampleRMListingrecords.Add(PopulatePaxNonSamplingRejectionMemoRecord(paxInvoice, rejectionMemo, rmSerialNo++)));

              if (nonSampleRMListingrecords.Count > 0)
              {

                // Create Special record like Gross Total and add it to collection 
                logger.InfoFormat("creating special records for NonSampling Rejection memo for source code[{0}]...", rejectionMemos.Key);

                #region Add Special Records

                rmSpecialRecords.Add(new SpecialRecord
                                       {
                                         Cells = new List<SpecialCell>
                                                                    {
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Serial No",
                                                                          Data = "Grand Total"
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Rejection Memo/Billing Memo/Credit Memo",
                                                                          Data = nonSampleRMListingrecords.Count.ToString()
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Gross Fare Value",
                                                                          Data =
                                                                            rejectionMemos.Sum(rm => rm.TotalGrossDifference).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "ISC Amount",
                                                                          Data =
                                                                            rejectionMemos.Sum(rm => rm.IscDifference).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Other Comm Amount",
                                                                          Data =
                                                                            rejectionMemos.Sum(rm => rm.OtherCommissionDifference).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "UATP Amount",
                                                                          Data =
                                                                            rejectionMemos.Sum(rm => rm.UatpAmountDifference).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Handling Fee Amount",
                                                                          Data =
                                                                            rejectionMemos.Sum(rm => rm.HandlingFeeAmountDifference).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Tax Amount",
                                                                          Data =
                                                                            rejectionMemos.Sum(rm => rm.TotalTaxAmountDifference).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "VAT Amount",
                                                                          Data =
                                                                            rejectionMemos.Sum(rm => rm.TotalVatAmountDifference).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Net Reject/ Net Credit Amount",
                                                                          Data =
                                                                            rejectionMemos.Sum(rm => rm.TotalNetRejectAmount).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        }
                                                                    }

                                       });

                #endregion

                // Create appropriate file name and Generate Pax non-sampling RM listing report
                logger.InfoFormat("Generating NonSampling Rejection memo report for source code[{0}]...", rejectionMemos.Key);
                CsvProcessor.GenerateCsvReport(nonSampleRMListingrecords,
                                               Path.Combine(listingPath,
                                                            string.Format("{0}NSSC{1}-{2}.CSV",
                                                                          PaxReportConstants.PaxBillingCategory,
                                                                          rejectionMemos.Key.ToString().PadLeft(2, '0'),
                                                                          paxInvoice.InvoiceNumber)).ToUpper(),
                                               rmSpecialRecords);
                logger.InfoFormat("Interline Passenger NonSampling RM/BM/CM  Listing report for Rejection memo;source code[{0}] generated.", rejectionMemos.Key);
              }

              nonSampleRMListingrecords.Clear();
              nonSampleRMListingrecords = null;
              rmSpecialRecords.Clear();
              rmSpecialRecords = null;
            }

            #endregion
          }
          if (paxInvoice.BillingMemoRecord != null && paxInvoice.BillingMemoRecord.Count > 0)
          {
            logger.InfoFormat("NonSampling Billing memos");
            #region Pax Non Sampling BM Listings
            // Group BillingMemo Records on Source Code
            logger.InfoFormat("Grouping NonSampling Billing memo records by source code...");
            foreach (var billingMemos in paxInvoice.BillingMemoRecord.GroupBy(bm => bm.SourceCodeId))
            {
              var nonSampleBMListingrecords = new List<PaxNonSamplingRmBmCmListingReport>();
              int bmSerialNo = 1;
              var bmSpecialRecords = new List<SpecialRecord>();

              // Populate Pax Non-Sampling billing memo report record and add it to collection
              logger.InfoFormat("Populating NonSampling Billing memo records for source code[{0}]...", billingMemos.Key);
              (from nsBMListingRecord in billingMemos
               orderby nsBMListingRecord.SourceCodeId, nsBMListingRecord.BatchSequenceNumber, nsBMListingRecord.RecordSequenceWithinBatch
               select nsBMListingRecord).ToList().ForEach(billingMemo => nonSampleBMListingrecords.Add(PopulatePaxNonSamplingBillingMemoRecord(paxInvoice, billingMemo, bmSerialNo++)));

              if (nonSampleBMListingrecords.Count > 0)
              {
                // Create Special record like Gross Total and add it to collection 
                logger.InfoFormat("creating special records for NonSampling Billing memo for source code[{0}]...", billingMemos.Key);

                #region Add Special Records

                bmSpecialRecords.Add(new SpecialRecord
                                       {
                                         Cells = new List<SpecialCell>
                                                                    {
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Serial No",
                                                                          Data = "Grand Total"
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Rejection Memo/Billing Memo/Credit Memo",
                                                                          Data = nonSampleBMListingrecords.Count.ToString()
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Gross Fare Value",
                                                                          Data =
                                                                            billingMemos.Sum(bm => bm.TotalGrossAmountBilled).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "ISC Amount",
                                                                          Data =
                                                                            billingMemos.Sum(bm => bm.TotalIscAmountBilled).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Other Comm Amount",
                                                                          Data =
                                                                            billingMemos.Sum(bm => bm.TotalOtherCommissionAmount).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "UATP Amount",
                                                                          Data =
                                                                            billingMemos.Sum(bm => bm.TotalUatpAmountBilled).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Handling Fee Amount",
                                                                          Data =
                                                                            billingMemos.Sum(bm => bm.TotalHandlingFeeBilled).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Tax Amount",
                                                                          Data =
                                                                            billingMemos.Sum(bm => bm.TaxAmountBilled).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "VAT Amount",
                                                                          Data =
                                                                            billingMemos.Sum(bm => bm.TotalVatAmountBilled).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Net Reject/ Net Credit Amount",
                                                                          Data =
                                                                            billingMemos.Sum(bm => bm.NetAmountBilled).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        }
                                                                    }

                                       });

                #endregion

                // Create appropriate file name and Generate Pax non-sampling BM listing report
                logger.InfoFormat("Generating NonSampling Billing memo report for source code[{0}]...", billingMemos.Key);
                CsvProcessor.GenerateCsvReport(nonSampleBMListingrecords,
                                               Path.Combine(listingPath,
                                                            string.Format("{0}NSSC{1}-{2}.CSV",
                                                                          PaxReportConstants.PaxBillingCategory,
                                                                          billingMemos.Key.ToString().PadLeft(2, '0'),
                                                                          paxInvoice.InvoiceNumber)).ToUpper(),
                                               bmSpecialRecords);
                logger.InfoFormat("Interline Passenger NonSampling RM/BM/CM  Listing report for Billing memo;source code[{0}] generated.", billingMemos.Key);
              }

              nonSampleBMListingrecords.Clear();
              nonSampleBMListingrecords = null;
              bmSpecialRecords.Clear();
              bmSpecialRecords = null;
            }

            #endregion
          }
          if (paxInvoice.CreditMemoRecord != null && paxInvoice.CreditMemoRecord.Count > 0)
          {
            logger.InfoFormat("NonSampling Credit memos");
            #region Pax Non Sampling CM Listings
            // Group CreditMemo Records on Source Code
            logger.InfoFormat("Grouping NonSampling Credit memo records by source code...");
            foreach (var creditMemos in paxInvoice.CreditMemoRecord.GroupBy(cm => cm.SourceCodeId))
            {
              var nonSampleCMListingrecords = new List<PaxNonSamplingRmBmCmListingReport>();
              int cmSerialNo = 1;
              var cmSpecialRecords = new List<SpecialRecord>();

              // Populate Pax Non-Sampling credit memo report record and add it to collection
              logger.InfoFormat("Populating NonSampling Credit memo records for source code[{0}]...", creditMemos.Key);

              // Sort the records in correct order.
              (from nsCMLisingRecord in creditMemos
               orderby nsCMLisingRecord.SourceCode, nsCMLisingRecord.BatchSequenceNumber, nsCMLisingRecord.RecordSequenceWithinBatch
               select nsCMLisingRecord).ToList().ForEach(creditMemo => nonSampleCMListingrecords.Add(PopulatePaxNonSamplingCreditMemoRecord(paxInvoice, creditMemo, cmSerialNo++)));

              if (nonSampleCMListingrecords.Count > 0)
              {
                // Creating special records like GrossTotal and add it to collection
                logger.InfoFormat("creating special records for NonSampling credit memo for source code[{0}]...", creditMemos.Key);

                #region Add Special Records

                cmSpecialRecords.Add(new SpecialRecord
                                       {
                                         Cells = new List<SpecialCell>
                                                                    {
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Serial No",
                                                                          Data = "Grand Total"
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Rejection Memo/Billing Memo/Credit Memo",
                                                                          Data = nonSampleCMListingrecords.Count.ToString()
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Gross Fare Value",
                                                                          Data =
                                                                            creditMemos.Sum(cm => cm.TotalGrossAmountCredited).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "ISC Amount",
                                                                          Data =
                                                                            creditMemos.Sum(cm => cm.TotalIscAmountCredited).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Other Comm Amount",
                                                                          Data =
                                                                            creditMemos.Sum(cm => cm.TotalOtherCommissionAmountCredited).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "UATP Amount",
                                                                          Data =
                                                                            creditMemos.Sum(cm => cm.TotalUatpAmountCredited).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Handling Fee Amount",
                                                                          Data =
                                                                            creditMemos.Sum(cm => cm.TotalHandlingFeeCredited).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Tax Amount",
                                                                          Data =
                                                                            creditMemos.Sum(cm => cm.TaxAmount).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "VAT Amount",
                                                                          Data =
                                                                            creditMemos.Sum(cm => cm.VatAmount).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        },
                                                                      new SpecialCell
                                                                        {
                                                                          Key = "Net Reject/ Net Credit Amount",
                                                                          Data =
                                                                            creditMemos.Sum(cm => cm.NetAmountCredited).ToString(
                                                                              PaxReportConstants.DecimalStringFormat)
                                                                        }
                                                                    }

                                       });

                #endregion

                // Create appropriate file name and Generate Pax non-sampling CM listing report
                logger.InfoFormat("Generating NonSampling credit memo report for source code[{0}]...", creditMemos.Key);

                CsvProcessor.GenerateCsvReport(nonSampleCMListingrecords,
                                               Path.Combine(listingPath,
                                                            string.Format("{0}NSSC{1}-{2}.CSV",
                                                                          PaxReportConstants.PaxBillingCategory,
                                                                          creditMemos.Key.ToString().PadLeft(2, '0'),
                                                                          paxInvoice.InvoiceNumber)).ToUpper(),
                                               cmSpecialRecords);
                logger.InfoFormat("Interline Passenger NonSampling RM/BM/CM  Listing report for credit memo;source code[{0}] generated.", creditMemos.Key);
              }
              nonSampleCMListingrecords.Clear();
              nonSampleCMListingrecords = null;
              cmSpecialRecords.Clear();
              cmSpecialRecords = null;
            }

            #endregion
          }
        }
      }
    }

    /// <summary>
    /// Create the invoice detail pdf
    /// 1. Non sampling prime coupons
    /// 2.Non sampling fims
    /// 3. non samples rm,bm,cm
    /// 4. formf,xf,d,ab
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="listingPath"></param>
    /// <param name="errors"></param>
    /// <param name="logger"></param>
    /// <param name="paxInvoiceCoupons"></param>
    public void CreatePaxDetailsPdf(string invoiceId, string listingPath, StringBuilder errors, ILog logger, PaxInvoice paxInvoice)
    {
      var invoiceRepository = new InvoiceRepository();
      var paxreportManager = new PaxReportManager();


      logger.InfoFormat("Generate pdf for the invoiceid " + invoiceId);

      PaxInvoice invoice =null;

      if (paxInvoice.SubmissionMethod == SubmissionMethod.IsWeb || paxInvoice.SubmissionMethod == SubmissionMethod.AutoBilling)
        invoice = invoiceRepository.GetInvoiceDetailsData(invoiceId, null);
      else
        invoice = paxInvoice;

      var formABinvoice = invoiceRepository.GetFormDetailsData(invoiceId, null, (int) BillingCode.SamplingFormAB);
      var formFinvoice = invoiceRepository.GetFormDetailsData(invoiceId, null, (int) BillingCode.SamplingFormF);
      var formXFinvoice = invoiceRepository.GetFormDetailsData(invoiceId, null, (int) BillingCode.SamplingFormXF);
      var formDinvoice = invoiceRepository.GetFormDetailsData(invoiceId, null, (int) BillingCode.SamplingFormDE);

      var reportFileName = "PLIST-" + invoice.InvoiceNumber + ".pdf";

      logger.InfoFormat("Report file name is " + reportFileName);
      var pdfFileName = Path.Combine(listingPath, reportFileName);

      // Create pdf for the pax details
      paxreportManager.BuildInvoiceDetailReport(invoice, paxInvoice.CouponDataRecord, formABinvoice, formFinvoice, formXFinvoice, formDinvoice, pdfFileName);
    }

    /// <summary>
    /// Populates the pax non sampling credit memo record.
    /// </summary>
    /// <param name="creditMemo">The credit memo.</param>
    /// <param name="serialNo">The serial no.</param>
    /// <returns></returns>
    private PaxNonSamplingRmBmCmListingReport PopulatePaxNonSamplingCreditMemoRecord(PaxInvoice paxInvoice, CreditMemo creditMemo, int serialNo)
    {
      return new PaxNonSamplingRmBmCmListingReport
               {
                 SerialNo = serialNo.ToString(),
                 BillingAirlineCode = paxInvoice.BillingMember.MemberCodeNumeric,
                 BilledAirlineCode = paxInvoice.BilledMember.MemberCodeNumeric,
                 InvoiceNumber = paxInvoice.InvoiceNumber,
                 BillingMonthYear =
                   PaxCsvReportHelper.GetFormattedBillingMonthYear(paxInvoice.BillingMonth,
                                                                   paxInvoice.BillingYear,
                                                                   PaxReportConstants.PaxListingReportDateFormat),
                 PeriodNo = paxInvoice.BillingPeriod.ToString(),
                 SampleOrNonSample = PaxCsvReportHelper.GetSampleOrNonSampleDisplayText(paxInvoice.BillingCode),
                 SourceCode = creditMemo.SourceCodeId.ToString(),
                 BatchNo = creditMemo.BatchSequenceNumber.ToString(),
                 SequenceNo = creditMemo.RecordSequenceWithinBatch.ToString(),
                 MemoNumber = creditMemo.CreditMemoNumber,
                 CurrencyOfListing = paxInvoice.ListingCurrencyDisplayText,
                 //paxInvoice.ListingCurrencyId.HasValue ? paxInvoice.ListingCurrencyId.ToString() : string.Empty,
                 GrossFareValue = creditMemo.TotalGrossAmountCredited.ToString(PaxReportConstants.DecimalStringFormat),
                 IscAmount = creditMemo.TotalIscAmountCredited.ToString(PaxReportConstants.DecimalStringFormat),
                 OtherCommissionAmount = creditMemo.TotalOtherCommissionAmountCredited.ToString(PaxReportConstants.DecimalStringFormat),
                 UatpAmount = creditMemo.TotalUatpAmountCredited.ToString(PaxReportConstants.DecimalStringFormat),
                 HandlingFeeAmount = creditMemo.TotalHandlingFeeCredited.ToString(PaxReportConstants.DecimalStringFormat),
                 TaxAmount = creditMemo.TaxAmount.ToString(PaxReportConstants.DecimalStringFormat),
                 VatAmount = creditMemo.VatAmount.ToString(PaxReportConstants.DecimalStringFormat),
                 NetRejectCreditAmount = creditMemo.NetAmountCredited.ToString(PaxReportConstants.DecimalStringFormat)
               };
    }

    /// <summary>
    /// Populates the pax non sampling billing memo record.
    /// </summary>
    /// <param name="billingMemo">The billing memo.</param>
    /// <param name="serialNo">The serial no.</param>
    /// <returns></returns>
    private PaxNonSamplingRmBmCmListingReport PopulatePaxNonSamplingBillingMemoRecord(PaxInvoice paxInvoice, BillingMemo billingMemo, int serialNo)
    {
      return new PaxNonSamplingRmBmCmListingReport
      {
        SerialNo = serialNo.ToString(),
        BillingAirlineCode = paxInvoice.BillingMember.MemberCodeNumeric,
        BilledAirlineCode = paxInvoice.BilledMember.MemberCodeNumeric,
        InvoiceNumber = paxInvoice.InvoiceNumber,
        BillingMonthYear = PaxCsvReportHelper.GetFormattedBillingMonthYear(paxInvoice.BillingMonth, paxInvoice.BillingYear, PaxReportConstants.PaxListingReportDateFormat),
        PeriodNo = paxInvoice.BillingPeriod.ToString(),
        SampleOrNonSample = PaxCsvReportHelper.GetSampleOrNonSampleDisplayText(paxInvoice.BillingCode),
        SourceCode = billingMemo.SourceCodeId.ToString(),
        BatchNo = billingMemo.BatchSequenceNumber.ToString(),
        SequenceNo = billingMemo.RecordSequenceWithinBatch.ToString(),
        MemoNumber = billingMemo.BillingMemoNumber,
        CurrencyOfListing = paxInvoice.ListingCurrencyDisplayText,
        //paxInvoice.ListingCurrencyId.HasValue ? paxInvoice.ListingCurrencyId.ToString() : string.Empty,
        GrossFareValue = billingMemo.TotalGrossAmountBilled.ToString(PaxReportConstants.DecimalStringFormat),
        IscAmount = billingMemo.TotalIscAmountBilled.ToString(PaxReportConstants.DecimalStringFormat),
        OtherCommissionAmount = billingMemo.TotalOtherCommissionAmount.ToString(PaxReportConstants.DecimalStringFormat),
        UatpAmount = billingMemo.TotalUatpAmountBilled.ToString(PaxReportConstants.DecimalStringFormat),
        HandlingFeeAmount = billingMemo.TotalHandlingFeeBilled.ToString(PaxReportConstants.DecimalStringFormat),
        TaxAmount = billingMemo.TaxAmountBilled.ToString(PaxReportConstants.DecimalStringFormat),
        VatAmount = billingMemo.TotalVatAmountBilled.ToString(PaxReportConstants.DecimalStringFormat),
        NetRejectCreditAmount = billingMemo.NetAmountBilled.ToString(PaxReportConstants.DecimalStringFormat)
      };
    }

    /// <summary>
    /// Populates the pax non sampling rejection memo record.
    /// </summary>
    /// <param name="rejectionMemo">The rejection memo.</param>
    /// <param name="serialNo">The serial no.</param>
    /// <returns></returns>
    private PaxNonSamplingRmBmCmListingReport PopulatePaxNonSamplingRejectionMemoRecord(PaxInvoice paxInvoice, RejectionMemo rejectionMemo, int serialNo)
    {
      return new PaxNonSamplingRmBmCmListingReport
      {

        SerialNo = serialNo.ToString(),
        BillingAirlineCode = paxInvoice.BillingMember.MemberCodeNumeric,
        BilledAirlineCode = paxInvoice.BilledMember.MemberCodeNumeric,
        InvoiceNumber = paxInvoice.InvoiceNumber,
        BillingMonthYear = PaxCsvReportHelper.GetFormattedBillingMonthYear(paxInvoice.BillingMonth, paxInvoice.BillingYear, PaxReportConstants.PaxListingReportDateFormat),
        PeriodNo = paxInvoice.BillingPeriod.ToString(),
        SampleOrNonSample = PaxCsvReportHelper.GetSampleOrNonSampleDisplayText(paxInvoice.BillingCode),
        SourceCode = rejectionMemo.SourceCodeId.ToString(),
        BatchNo = rejectionMemo.BatchSequenceNumber.ToString(),
        SequenceNo = rejectionMemo.RecordSequenceWithinBatch.ToString(),
        MemoNumber = rejectionMemo.RejectionMemoNumber,
        CurrencyOfListing = paxInvoice.ListingCurrencyDisplayText,
        //paxInvoice.ListingCurrencyId.HasValue ? paxInvoice.ListingCurrencyId.ToString() : string.Empty,
        GrossFareValue = rejectionMemo.TotalGrossDifference.ToString(PaxReportConstants.DecimalStringFormat),
        IscAmount = rejectionMemo.IscDifference.ToString(PaxReportConstants.DecimalStringFormat),
        OtherCommissionAmount = rejectionMemo.OtherCommissionDifference.ToString(PaxReportConstants.DecimalStringFormat),
        UatpAmount = rejectionMemo.UatpAmountDifference.ToString(PaxReportConstants.DecimalStringFormat),
        HandlingFeeAmount = rejectionMemo.HandlingFeeAmountDifference.ToString(PaxReportConstants.DecimalStringFormat),
        TaxAmount = rejectionMemo.TotalTaxAmountDifference.ToString(PaxReportConstants.DecimalStringFormat),
        VatAmount = rejectionMemo.TotalVatAmountDifference.ToString(PaxReportConstants.DecimalStringFormat),
        NetRejectCreditAmount = rejectionMemo.TotalNetRejectAmount.ToString(PaxReportConstants.DecimalStringFormat)
      };
    }

    /// <summary>
    /// Populates the pax coupon listing report record.
    /// </summary>
    /// <param name="primeCoupon">The prime coupon.</param>
    /// <param name="serialNo">The serial no.</param>
    /// <returns></returns>
    private PaxCouponListingReport PopulatePaxCouponListingRecord(PaxInvoice paxInvoice, PrimeCoupon primeCoupon, int serialNo)
    {
      return new PaxCouponListingReport
               {
                 SerialNo = serialNo.ToString(),
                 BillingAirlineCode = paxInvoice.BillingMember.MemberCodeNumeric, 
                 BilledAirlineCode = paxInvoice.BilledMember.MemberCodeNumeric,
                 InvoiceNumber = paxInvoice.InvoiceNumber,
                 BillingMonthYear =
                   PaxCsvReportHelper.GetFormattedBillingMonthYear(paxInvoice.BillingMonth,
                                                                   paxInvoice.BillingYear,
                                                                   PaxReportConstants.PaxListingReportDateFormat),
                 PeriodNo = paxInvoice.BillingPeriod.ToString(),
                 SampleOrNonSample = PaxCsvReportHelper.GetSampleOrNonSampleDisplayText(paxInvoice.BillingCode),
                 SourceCode = primeCoupon.SourceCodeId.ToString(),
                 BatchNo = primeCoupon.BatchSequenceNumber.ToString(),
                 SequenceNo = primeCoupon.RecordSequenceWithinBatch.ToString(),
                 IssuingAirline = primeCoupon.TicketOrFimIssuingAirline,
                 CouponNo = primeCoupon.TicketOrFimCouponNumber.ToString(),
                 DocumentNo = primeCoupon.TicketDocOrFimNumber.ToString(), //AirlineTicketDocOrFimNumber,
                 CheckDigit = primeCoupon.CheckDigit.ToString(),
                 FromAirport = primeCoupon.FromAirportOfCoupon,
                 ToAirport = primeCoupon.ToAirportOfCoupon,
                 CurrencyOfListing = paxInvoice.ListingCurrencyDisplayText,
                 GrossFareValue = primeCoupon.CouponGrossValueOrApplicableLocalFare.ToString(PaxReportConstants.DecimalStringFormat),
                 ETicketIndicator = primeCoupon.ETicketIndicator,
                 OriginalPmi = primeCoupon.OriginalPmi,
                 AgreementIndicatorSupplied = primeCoupon.AgreementIndicatorSupplied,
                 IscRate = primeCoupon.IscPercent.ToString(PaxReportConstants.DecimalStringFormat),
                 IscAmount = primeCoupon.IscAmount.ToString(PaxReportConstants.DecimalStringFormat),
                 OtherCommissionRate = primeCoupon.OtherCommissionPercent.ToString(PaxReportConstants.DecimalStringFormat),
                 OtherCommissionAmount = primeCoupon.OtherCommissionAmount.ToString(PaxReportConstants.DecimalStringFormat),
                 UatpRate = primeCoupon.UatpPercent.ToString(PaxReportConstants.DecimalStringFormat),
                 UatpAmount = primeCoupon.UatpAmount.ToString(PaxReportConstants.DecimalStringFormat),
                 HandlingFeeAmount = primeCoupon.HandlingFeeAmount.ToString(PaxReportConstants.DecimalStringFormat),
                 CurrencyAdjustmentIndicator = primeCoupon.CurrencyAdjustmentIndicator,
                 TaxAmount = primeCoupon.TaxAmount.ToString(PaxReportConstants.DecimalStringFormat),
                 VatAmount = primeCoupon.VatAmount.ToString(PaxReportConstants.DecimalStringFormat),
                 CouponTotalAmount = primeCoupon.CouponTotalAmount.ToString(PaxReportConstants.DecimalStringFormat)
               };
    }

    /// <summary>
    /// Populates the sampling form D listing record.
    /// </summary>
    /// <param name="paxInvoice">The pax invoice.</param>
    /// <param name="samplingFormDRecord">The sampling form D record.</param>
    /// <param name="serialNo">The serial no.</param>
    /// <returns></returns>
    private PaxSamplingFormDListingReport PopulateSamplingFormDListingRecord(PaxInvoice paxInvoice, SamplingFormDRecord samplingFormDRecord, int serialNo)
    {
      return new PaxSamplingFormDListingReport
      {
        SerialNo = serialNo.ToString(),
        BillingAirlineCode = paxInvoice.BillingMember.MemberCodeNumeric,
        BilledAirlineCode = paxInvoice.BilledMember.MemberCodeNumeric,
        InvoiceNumber = paxInvoice.InvoiceNumber,
        ProvisionalBillingMonthYear = PaxCsvReportHelper.GetFormattedBillingMonthYear(paxInvoice.ProvisionalBillingMonth, paxInvoice.ProvisionalBillingYear, PaxReportConstants.PaxListingReportDateFormat),
        BillingMonthYear = PaxCsvReportHelper.GetFormattedBillingMonthYear(paxInvoice.BillingMonth, paxInvoice.BillingYear, PaxReportConstants.PaxListingReportDateFormat),
        PeriodNo = paxInvoice.BillingPeriod.ToString(),
        IssuingAirline = samplingFormDRecord.TicketIssuingAirline,
        DocumentNo = samplingFormDRecord.TicketDocNumber.ToString(),
        CouponNo = samplingFormDRecord.CouponNumber.ToString(),
        ProvisionalInvoiceNo = samplingFormDRecord.ProvisionalInvoiceNumber,
        BatchNo = samplingFormDRecord.BatchNumberOfProvisionalInvoice.ToString(),
        SequenceNo = samplingFormDRecord.RecordSeqNumberOfProvisionalInvoice.ToString(),
        AgreementIndcSupplied = samplingFormDRecord.AgreementIndicatorSupplied,
        OriginalPmi = samplingFormDRecord.OriginalPmi,
        AgreementIndValidated = samplingFormDRecord.AgreementIndicatorValidated,
        ValidatedPmi = samplingFormDRecord.ValidatedPmi,
        ProvisionalGrossAmt = samplingFormDRecord.ProvisionalGrossAlfAmount.ToString(PaxReportConstants.DecimalStringFormat),
        CurrencyOfListing = paxInvoice.ListingCurrencyDisplayText,
        //paxInvoice.ListingCurrencyId.HasValue ? Convert.ToString(paxInvoice.ListingCurrencyId.Value) : string.Empty,
        GrossFareValue = samplingFormDRecord.EvaluatedGrossAmount.ToString(PaxReportConstants.DecimalStringFormat),
        IscRate = samplingFormDRecord.IscPercent.ToString(PaxReportConstants.DecimalStringFormat),
        IscAmount = samplingFormDRecord.IscAmount.ToString(PaxReportConstants.DecimalStringFormat),
        OtherCommRate = samplingFormDRecord.OtherCommissionPercent.ToString(PaxReportConstants.DecimalStringFormat),
        OtherCommAmount = samplingFormDRecord.OtherCommissionAmount.ToString(PaxReportConstants.DecimalStringFormat),
        UatpRate = samplingFormDRecord.UatpPercent.ToString(PaxReportConstants.DecimalStringFormat),
        UatpAmount = samplingFormDRecord.UatpAmount.ToString(PaxReportConstants.DecimalStringFormat),
        HandlingFeeAmount = samplingFormDRecord.HandlingFeeAmount.ToString(PaxReportConstants.DecimalStringFormat),
        TaxAmount = samplingFormDRecord.TaxAmount.ToString(PaxReportConstants.DecimalStringFormat),
        VatAmount = samplingFormDRecord.VatAmount.ToString(PaxReportConstants.DecimalStringFormat),
        CouponTotalAmount = samplingFormDRecord.EvaluatedNetAmount.ToString(PaxReportConstants.DecimalStringFormat)
      };
    }

    /// <summary>
    /// Populates the pax sample RM report record.
    /// </summary>
    /// <param name="rejectionMemo">The rejection memo.</param>
    /// <param name="serialNo">The serial no.</param>
    /// <returns></returns>
    private PaxSampleRMListingReport PopulatePaxSampleRMReportRecord(PaxInvoice paxInvoice, RejectionMemo rejectionMemo, int serialNo)
    {
      return new PaxSampleRMListingReport
      {

        SerialNo = serialNo.ToString(),
        BillingAirlineCode = paxInvoice.BillingMember.MemberCodeNumeric,
        BilledAirlineCode = paxInvoice.BilledMember.MemberCodeNumeric,
        InvoiceNumber = paxInvoice.InvoiceNumber,
        BillingMonthYear = PaxCsvReportHelper.GetFormattedBillingMonthYear(paxInvoice.BillingMonth, paxInvoice.BillingYear, PaxReportConstants.PaxListingReportDateFormat),
        PeriodNo = paxInvoice.BillingPeriod.ToString(),
        SampleOrNonSample = PaxCsvReportHelper.GetSampleOrNonSampleDisplayText(paxInvoice.BillingCode),
        SourceCode = rejectionMemo.SourceCodeId.ToString(),
        BatchNo = rejectionMemo.BatchSequenceNumber.ToString(),
        SequenceNo = rejectionMemo.RecordSequenceWithinBatch.ToString(),
        RejectionMemo = rejectionMemo.RejectionMemoNumber,
        CurrencyOfListing = paxInvoice.ListingCurrencyDisplayText,
        //paxInvoice.ListingCurrencyId.HasValue ? paxInvoice.ListingCurrencyId.ToString() : string.Empty,
        GrossFareValue = rejectionMemo.TotalGrossDifference.ToString(PaxReportConstants.DecimalStringFormat),
        IscAmount = rejectionMemo.IscDifference.ToString(PaxReportConstants.DecimalStringFormat),
        OtherCommisionAmount = rejectionMemo.OtherCommissionDifference.ToString(PaxReportConstants.DecimalStringFormat),
        UatpAmount = rejectionMemo.UatpAmountDifference.ToString(PaxReportConstants.DecimalStringFormat),
        HandlingFeeAmount = rejectionMemo.HandlingFeeAmountDifference.ToString(PaxReportConstants.DecimalStringFormat),
        TaxAmount = rejectionMemo.TotalTaxAmountDifference.ToString(PaxReportConstants.DecimalStringFormat),
        VatAmount = rejectionMemo.TotalVatAmountDifference.ToString(PaxReportConstants.DecimalStringFormat),
        NetRejectAmount = rejectionMemo.TotalNetRejectAmount.ToString(PaxReportConstants.DecimalStringFormat),
        SamplingConstant = rejectionMemo.SamplingConstant.ToString(PaxReportConstants.DecimalStringFormat),
        NetRejectAmountXConstant = rejectionMemo.TotalNetRejectAmountAfterSamplingConstant.ToString(PaxReportConstants.DecimalStringFormat)
      };
    }

    /// <summary>
    /// Populates the sampling form C listing record.
    /// </summary>
    /// <param name="formC">The form C.</param>
    /// <param name="formCRecord">The form C record.</param>
    /// <param name="serialNo">The serial no.</param>
    /// <returns></returns>
    private PaxSamplingFormCListingReport PopulateSamplingFormCListingRecord(SamplingFormC formC, SamplingFormCRecord formCRecord, int serialNo)
    {
      return new PaxSamplingFormCListingReport
      {
        SerialNo = serialNo.ToString(),
        BillingAirlineCode = formC.FromMember.MemberCodeNumeric,
        BilledAirlineCode = formC.ProvisionalBillingMember.MemberCodeNumeric,
        NilFormCIndicator = formC.NilFormCIndicator,
        CurrencyOfListing = formC.ListingCurrencyId.ToString(),
        ProvisionalBillingMonthYear = PaxCsvReportHelper.GetFormattedBillingMonthYear(formC.ProvisionalBillingMonth, formC.ProvisionalBillingYear, PaxReportConstants.PaxListingReportDateFormat),
        IssuingAirline = formCRecord.TicketIssuingAirline,
        DocumentNo = formCRecord.DocumentNumber.ToString(),
        CouponNo = formCRecord.CouponNumber.ToString(),
        ProvisionalInvoiceNo = formCRecord.ProvisionalInvoiceNumber,
        BatchNo = formCRecord.BatchNumberOfProvisionalInvoice.ToString(),
        SequenceNo = formCRecord.RecordSeqNumberOfProvisionalInvoice.ToString(),
        ProvisionalGrossAmountAlf = formCRecord.GrossAmountAlf.ToString(PaxReportConstants.DecimalStringFormat),
        ReasonCode = formCRecord.ReasonCode,
        AdditionalRemarks = formCRecord.Remarks
      };
    }

    /// <summary>
    /// Creates the form C listing.
    /// </summary>
    /// <param name="samplingFormCs">The List of sampling form C grouped by Billing Member & Billed Member.</param>
    /// <param name="listingPath">The listing path.</param>
    /// <param name="errors">The errors.</param>
    /// <param name="logger">The logger.</param>
    /// <returns></returns>
    public void CreateFormCListing(List<SamplingFormC> samplingFormCs, string listingPath, StringBuilder errors, ILog logger)
    {
      /* Report Requirements		[Passenger - Sampling Form C Listing]										
       * 1. Form C report is a listing report of all the coupons which are rejected from the Provisional Invoice 
       * 2. The report should have a grant total of the Gross Amt/ Alf and Total of Items .
       * 3. The listing should be sorted in the same manner it is captured in IS-WEB or has come through IS-IDEC/IS-XML
       * 4. Column 'Additional Remarks' may contain commas within it, this will need to be handled appropriately
       * 5. Even though PAX IS-IDEC supports only 2 decimals for amounts, 3 decimals will be shown */
      logger.Info("Passenger - Sampling Form C Listing report.");
      if (samplingFormCs.Count > 0)
      {

        // Populate Sampling FormC listing report records and add to collection
        logger.Info("Populating Sampling FormC listing report records...");

        // List of sampling form C which was grouped by billing Member & billed member 
        // grouped again by Listing Currency Id, So that every collection of (BillingMember, Billedmember & ListingCurrencyId)
        // will generate only one csv report.

        //Parallel.ForEach(samplingFormCs.GroupBy(sfc => sfc.ListingCurrencyId),
        //                 samplingFormCCurrencyGroup => 
        foreach (var samplingFormCCurrencyGroup in samplingFormCs.GroupBy(sfc => sfc.ListingCurrencyId))
        {
          var records = new List<PaxSamplingFormCListingReport>();
          int serialNo = 1;
          double totalGrossAmountAlf = 0;
          var samplingFormCCurrencyGroupList = samplingFormCCurrencyGroup.ToList();
          samplingFormCCurrencyGroupList.ForEach(
            // Sort the records in correct order.
            sfc =>
            (from formCListingRecord in sfc.SamplingFormCDetails
             orderby formCListingRecord.BatchNumberOfProvisionalInvoice, formCListingRecord.RecordSeqNumberOfProvisionalInvoice
             select formCListingRecord).ToList().ForEach(formCRecord =>
                                                    {
                                                      records.Add(PopulateSamplingFormCListingRecord(sfc, formCRecord, serialNo++));
                                                      totalGrossAmountAlf += formCRecord.GrossAmountAlf;
                                                    }));
          if (records.Count > 0)
          {
            // Create special records and add it to special record collection
            logger.Info("Creating special records for Sampling FormC listing report...");

            #region Add Special Records

            var specialRecords = new List<SpecialRecord>();
            specialRecords.Add(new SpecialRecord
                                 {
                                   Cells = new List<SpecialCell>
                                             {
                                               new SpecialCell
                                                 {
                                                   Key = "Serial No",
                                                   Data = "Grand Total"
                                                 },
                                               new SpecialCell
                                                 {
                                                   Key = "Document No",
                                                   Data = records.Count.ToString()
                                                 },
                                               new SpecialCell
                                                 {
                                                   Key = "Provisional Gross Amount/ALF",
                                                   Data = totalGrossAmountAlf.ToString(PaxReportConstants.DecimalStringFormat)
                                                 }
                                             }
                                 });

            #endregion

            var receivableCsvFileName =
              Path.Combine(listingPath,
                           PaxCsvReportHelper.GetFormCFileName(samplingFormCCurrencyGroupList[0].ProvisionalBillingMember.MemberCodeNumeric,
                                                               samplingFormCCurrencyGroupList[0].ProvisionalBillingMonth,
                                                               samplingFormCCurrencyGroupList[0].ProvisionalBillingYear,
                                                               samplingFormCCurrencyGroupList[0].ListingCurrencyId)).ToUpper();
            if (!File.Exists(receivableCsvFileName))
            {
              CsvProcessor.GenerateCsvReport(records, receivableCsvFileName, specialRecords);
            }

            //Note : Now generate the Form C listing file for Payable prospective
            //and while generating a Offline archive zip delete the payables Csv if it is downloaded by a receivable and vice versa.
            var payableCsvFileName =
              Path.Combine(listingPath,
                           PaxCsvReportHelper.GetFormCFileName(samplingFormCCurrencyGroupList[0].FromMember.MemberCodeNumeric,
                                                               samplingFormCCurrencyGroupList[0].ProvisionalBillingMonth,
                                                               samplingFormCCurrencyGroupList[0].ProvisionalBillingYear,
                                                               samplingFormCCurrencyGroupList[0].ListingCurrencyId)).ToUpper();
            if (File.Exists(receivableCsvFileName) && !File.Exists(payableCsvFileName))
            {
              logger.InfoFormat("Generated csv for receivable's prospective.");
              File.Copy(receivableCsvFileName, payableCsvFileName, true);
              logger.InfoFormat("Generated csv for payable's prospective.");
            }

            logger.Info("Passenger - Sampling Form C Listing report generated.");

            records.Clear();
            specialRecords.Clear();
            records = null;
            specialRecords = null;
          }
          else
          {
            logger.Info("Passenger - Sampling Form C Details records not found.");
          }
        }
        //);
      }

    }

  }
}
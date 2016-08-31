using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Iata.IS.Business.Reports.Cargo;
using Iata.IS.Business.Reports.Cargo.Impl;
using Iata.IS.Business.Reports.Pax;
using Iata.IS.Business.Reports.Pax.Impl;
using Iata.IS.Core.DI;
using Iata.IS.Core.File;
using Iata.IS.Data;
using Iata.IS.Data.Cargo;
using Iata.IS.Data.Cargo.Impl;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Cargo.Enums;
using Iata.IS.Model.Common;
using Iata.IS.Model.Reports.Cargo;
using Iata.IS.Model.Reports.Pax;
using log4net;
using TransactionType = Iata.IS.Model.Enums.TransactionType;

namespace Iata.IS.Business.Common.Impl
{
    /// <summary>
    /// Creates Listing report for cargo invoice
    /// </summary>
    public class CgoListingsReportGenerator : ICgoListingsReportGenerator
    {
     
      private List<ReasonCode> reasonCodes;

        /// <summary>
        /// Creates RM BM CM and AWB Listing report for cargo invoice
        /// </summary>
        /// <param name="cargoInvoice">cargoInvoice</param>
        /// <param name="listingPath">listingPath</param>
        /// <param name="errors">errors</param>
        /// <param name="logger">logger</param>
        public void CreateCgoListings(CargoInvoice cargoInvoice, string listingPath, StringBuilder errors, log4net.ILog logger)
        {
          var reasonCodeRepository = Ioc.Resolve<IRepository<ReasonCode>>();
          reasonCodes = reasonCodeRepository.GetAll().ToList();

            if (cargoInvoice != null)
            {
                CreateCgoRmBmCmListing(cargoInvoice, listingPath, errors, logger);
                CreateCgoAwbListing(cargoInvoice, listingPath, errors, logger);
            }

        }

        /// <summary>
        /// Creates RM BM CM Listing report for cargo invoice
        /// </summary>
        /// <param name="cargoInvoice">CargoInvoice</param>
        /// <param name="listingPath">listingPath</param>
        /// <param name="errors">errors</param>
        /// <param name="logger">logger</param>
        private void CreateCgoRmBmCmListing(CargoInvoice cargoInvoice, string listingPath, StringBuilder errors, log4net.ILog logger)
        {
            decimal weightChargesSum = 0;
            decimal valuationChargesSum = 0;
            decimal otherChargesSum = 0;
            decimal iScAmountSum = 0;
            decimal vatAmountSum = 0;
            decimal netAmountSum = 0;
            var serialNo = 1;
            var listingRecords = new List<CgoRmBmCmListingReport>();
            var specialRecords = new List<SpecialRecord>();


            logger.InfoFormat("Interline Cgo RM/BM/CM  Listing report");


            #region Cgo RM Listings

            if (cargoInvoice.CGORejectionMemo != null && cargoInvoice.CGORejectionMemo.Count > 0)
            {
                logger.InfoFormat("Rejection memos");
                logger.InfoFormat("Ordering rejection memos by BatchSequenceNumber and RecordSequenceWithinBatch...");

                foreach (var rejectionMemo in cargoInvoice.CGORejectionMemo.OrderBy(rm => rm.BatchSequenceNumber).OrderBy(rm => rm.RecordSequenceWithinBatch))
                {
                    // Populate Cgo rejection memo report record and add it to collection
                    logger.InfoFormat("Populating Cgo Rejection memo records");
                    listingRecords.Add(PopulateCgoRejectionMemoRecord(cargoInvoice, rejectionMemo, serialNo++));

                    weightChargesSum += rejectionMemo.TotalWeightChargeDifference.HasValue ? rejectionMemo.TotalWeightChargeDifference.Value : 0;
                    valuationChargesSum += rejectionMemo.TotalValuationChargeDifference.HasValue ? rejectionMemo.TotalValuationChargeDifference.Value : 0;
                    otherChargesSum += rejectionMemo.TotalOtherChargeDifference.HasValue ? rejectionMemo.TotalOtherChargeDifference.Value : 0;
                    iScAmountSum += rejectionMemo.TotalIscAmountDifference.HasValue ? rejectionMemo.TotalIscAmountDifference.Value : 0;
                    vatAmountSum += Convert.ToDecimal(rejectionMemo.TotalVatAmountDifference.HasValue ? rejectionMemo.TotalVatAmountDifference.Value : 0);
                    netAmountSum += rejectionMemo.TotalNetRejectAmount.HasValue ? rejectionMemo.TotalNetRejectAmount.Value : 0;
                }
            }

            #endregion

            #region Cgo BM Listings

            if (cargoInvoice.CGOBillingMemo != null && cargoInvoice.CGOBillingMemo.Count > 0)
            {
                logger.InfoFormat("Billing memos");

                foreach (var billingMemo in cargoInvoice.CGOBillingMemo.OrderBy(bm => bm.BatchSequenceNumber).OrderBy(bm => bm.RecordSequenceWithinBatch))
                {
                    // Populate Cgo rejection memo report record and add it to collection
                    logger.InfoFormat("Populating Cgo Billing memo records ");
                    listingRecords.Add(PopulateCgoBillingMemoRecord(cargoInvoice, billingMemo, serialNo++));

                    weightChargesSum += billingMemo.BilledTotalWeightCharge.HasValue ? billingMemo.BilledTotalWeightCharge.Value : 0;
                    valuationChargesSum += billingMemo.BilledTotalValuationAmount.HasValue ? billingMemo.BilledTotalValuationAmount.Value : 0;
                    otherChargesSum += billingMemo.BilledTotalOtherChargeAmount;
                    iScAmountSum += billingMemo.BilledTotalIscAmount;
                    vatAmountSum += billingMemo.BilledTotalVatAmount.HasValue ? billingMemo.BilledTotalVatAmount.Value : 0;
                    netAmountSum += billingMemo.NetBilledAmount.HasValue ? billingMemo.NetBilledAmount.Value : 0;
                }
            }

            #endregion

            #region Cgo CM Listings

            if (cargoInvoice.CGOCreditMemo != null && cargoInvoice.CGOCreditMemo.Count > 0)
            {
                logger.InfoFormat("Credit memos");

                foreach (var creditMemo in cargoInvoice.CGOCreditMemo.OrderBy(cm => cm.BatchSequenceNumber).OrderBy(cm => cm.RecordSequenceWithinBatch))
                {
                    // Populate Cgo rejection memo report record and add it to collection
                    logger.InfoFormat("Populating Cgo Credit memo records ");
                    listingRecords.Add(PopulateCgoCreditMemoRecord(cargoInvoice, creditMemo, serialNo++));

                    weightChargesSum += creditMemo.TotalWeightCharges.HasValue ? creditMemo.TotalWeightCharges.Value : 0;
                    valuationChargesSum += creditMemo.TotalValuationAmt.HasValue ? creditMemo.TotalValuationAmt.Value : 0;
                    otherChargesSum += creditMemo.TotalOtherChargeAmt;
                    iScAmountSum += creditMemo.TotalIscAmountCredited;
                    vatAmountSum += creditMemo.TotalVatAmountCredited.HasValue ? creditMemo.TotalVatAmountCredited.Value : 0;
                    netAmountSum += creditMemo.NetAmountCredited.HasValue ? creditMemo.NetAmountCredited.Value : 0;
                }
            }

            #endregion

            #region Add Special Records

            specialRecords.Add(new SpecialRecord
                                   {
                                       Cells = new List<SpecialCell>
                                                       {
                                                           new SpecialCell
                                                               {
                                                                   Key = "Sr No",
                                                                   Data = "Grand Total"
                                                               },
                                                           new SpecialCell
                                                               {
                                                                   Key = "Seq No",
                                                                   Data = "Total No of Items"
                                                               },
                                                           new SpecialCell
                                                               {
                                                                   Key = "Rejection Memo/Billing Memo/Credit Memo",
                                                                   Data = listingRecords.Count.ToString()
                                                               },

                                                           new SpecialCell
                                                               {
                                                                   Key = "Weight Charges",
                                                                   Data =
                                                                       weightChargesSum.ToString(
                                                                           CargoReportConstants.DecimalStringFormat)
                                                               },
                                                           new SpecialCell
                                                               {
                                                                   Key = "Valuation Charges",
                                                                   Data =
                                                                       valuationChargesSum.ToString(
                                                                           CargoReportConstants.DecimalStringFormat)
                                                               },

                                                           new SpecialCell
                                                               {
                                                                   Key = "Other Charges",
                                                                   Data =
                                                                       otherChargesSum.ToString(
                                                                           CargoReportConstants.DecimalStringFormat)
                                                               },

                                                           new SpecialCell
                                                               {
                                                                   Key = "ISC Amount",
                                                                   Data =
                                                                       iScAmountSum.ToString(
                                                                           CargoReportConstants.DecimalStringFormat)
                                                               },
                                                           new SpecialCell
                                                               {
                                                                   Key = "VAT Amount",
                                                                   Data =
                                                                       vatAmountSum.ToString(
                                                                           CargoReportConstants.DecimalStringFormat)
                                                               },

                                                           new SpecialCell
                                                               {
                                                                   Key = "Net Amount",
                                                                   Data =
                                                                       netAmountSum.ToString(
                                                                           CargoReportConstants.DecimalStringFormat)
                                                               }
                                                       }

                                   });

            #endregion

            #region Write to CSV file

            // Create appropriate file name and Generate Cgo RM BM CM listing 
            logger.InfoFormat("Generating Interline Cargo RM/BM/CM Listing report");
            CsvProcessor.GenerateCsvReport(listingRecords,
                                           Path.Combine(listingPath,
                                                        string.Format("{0}MEMLST-{1}.CSV",
                                                                      CargoReportConstants.CgoBillingCategory,
                                                                      cargoInvoice.InvoiceNumber)).ToUpper(),
                                           specialRecords);
            logger.InfoFormat("Interline Cargo RM/BM/CM Listing report for Invoice [{0}] generated.",
                              cargoInvoice.InvoiceNumber);

            #endregion

        }

        /// <summary>
        /// Creates AWB Listing report for cargo invoice
        /// </summary>
        /// <param name="cargoInvoice">cargoInvoice</param>
        /// <param name="listingPath">listingPath</param>
        /// <param name="errors">errors</param>
        /// <param name="logger">logger</param>
        private void CreateCgoAwbListing(CargoInvoice cargoInvoice, string listingPath, StringBuilder errors, log4net.ILog logger)
        {
            decimal weightChargesSum = 0;
            decimal valuationChargesSum = 0;
            decimal otherChargesSum = 0;
            decimal wtChargesSubIscSum = 0;
            decimal iscRateSum = 0;
            decimal iscAmountSum = 0;
            decimal vatAmountSum = 0;
            decimal awbTotalAmountSum = 0;
            decimal billedWeightSum = 0;

            var cgoAwbListingRecords = new List<CgoAwbListingReport>();
            var cgoAwbSpecialRecords = new List<SpecialRecord>();
            var awbSerialNo = 1;

            if (cargoInvoice.AwbDataRecord != null)
            {
                logger.Info("Interline Cargo AWB Listing report.");

                #region Add AWB records
                logger.Info("Creating List of Cargo AWB records.");
                foreach (var awb in cargoInvoice.AwbDataRecord.OrderBy(awb1 => awb1.BatchSequenceNumber).OrderBy(awb1 => awb1.RecordSequenceWithinBatch).OrderBy(awb1 => awb1.BillingCodeId))
                {
                  cgoAwbListingRecords.Add(PopulateCgoAwbRecord(cargoInvoice, awb, awbSerialNo++));
                    weightChargesSum += Convert.ToDecimal(awb.WeightCharges.HasValue ? awb.WeightCharges.Value : 0);
                    valuationChargesSum += Convert.ToDecimal(awb.ValuationCharges.HasValue ? awb.ValuationCharges.Value : 0);
                    otherChargesSum += Convert.ToDecimal(awb.OtherCharges);
                    wtChargesSubIscSum += Convert.ToDecimal(awb.AmountSubjectToIsc);

                    iscRateSum += Convert.ToDecimal(awb.IscPer);
                    iscAmountSum += Convert.ToDecimal(awb.IscAmount);
                    vatAmountSum += Convert.ToDecimal(awb.VatAmount);
                    awbTotalAmountSum += Convert.ToDecimal(awb.AwbTotalAmount.HasValue ? awb.AwbTotalAmount.Value : 0);
                    billedWeightSum += Convert.ToDecimal(awb.BilledWeight);

                }
                logger.Info("Created List of Cargo AWB records.");
                #endregion

                #region Add Special Records
                logger.Info("Creating List of Cargo AWB special records.");
                cgoAwbSpecialRecords.Add(new SpecialRecord
                {
                    Cells = new List<SpecialCell>
                                                       {
                                                           new SpecialCell
                                                               {
                                                                   Key = "Sr No",
                                                                   Data = "Grand Total"
                                                               },
                                                           new SpecialCell
                                                               {
                                                                   Key = "Issue Airline",
                                                                   Data = "No of Items"
                                                               },
                                                           new SpecialCell
                                                               {
                                                                   Key = "AWB Serial No",
                                                                   Data = cgoAwbListingRecords.Count.ToString()
                                                               },
                                                           new SpecialCell
                                                               {
                                                                   Key = "Weight Charges",
                                                                   Data =
                                                                       weightChargesSum.ToString(
                                                                           CargoReportConstants.DecimalStringFormat)
                                                               },
                                                           new SpecialCell
                                                               {
                                                                   Key = "Valuation Charges",
                                                                   Data =
                                                                       valuationChargesSum.ToString(
                                                                           CargoReportConstants.DecimalStringFormat)
                                                               },
                                                           new SpecialCell
                                                               {
                                                                   Key = "Other Charges",
                                                                   Data =
                                                                       otherChargesSum.ToString(
                                                                           CargoReportConstants.DecimalStringFormat)
                                                               },
                                                           new SpecialCell
                                                               {
                                                                   Key = "Wt Charges Sub ISC",
                                                                   Data =
                                                                       wtChargesSubIscSum.ToString(
                                                                           CargoReportConstants.DecimalStringFormat)
                                                               },
                                                               new SpecialCell
                                                               {
                                                                   Key = "ISC Rate",
                                                                   Data =
                                                                       iscRateSum.ToString(
                                                                           CargoReportConstants.DecimalStringFormat)
                                                               },
                                                               new SpecialCell
                                                               {
                                                                   Key = "Other Charges",
                                                                   Data =
                                                                       otherChargesSum.ToString(
                                                                           CargoReportConstants.DecimalStringFormat)
                                                               },
                                                           new SpecialCell
                                                               {
                                                                   Key = "ISC Amount",
                                                                   Data =
                                                                       iscAmountSum.ToString(
                                                                           CargoReportConstants.DecimalStringFormat)
                                                               },
                                                           new SpecialCell
                                                               {
                                                                   Key = "VAT Amount",
                                                                   Data =
                                                                       vatAmountSum.ToString(
                                                                           CargoReportConstants.DecimalStringFormat)
                                                               },
                                                           new SpecialCell
                                                               {
                                                                   Key = "AWB Total Amount",
                                                                   Data =
                                                                       awbTotalAmountSum.ToString(
                                                                           CargoReportConstants.DecimalStringFormat)
                                                               },
                                                           new SpecialCell
                                                               {
                                                                   Key = "Billed Weight",
                                                                   Data =
                                                                       billedWeightSum.ToString(
                                                                           CargoReportConstants.DecimalStringFormat)
                                                               }
                                                       }

                });
                logger.Info("Created List of Cargo AWB special records.");
                #endregion

                #region Write to CSV file

                // Create appropriate file name and Generate Cgo AWB listing 
                logger.InfoFormat("Generating Interline Cargo AWB Listing report (CSV)");
                CsvProcessor.GenerateCsvReport(cgoAwbListingRecords,
                                               Path.Combine(listingPath,
                                                            string.Format("{0}AWBLST-{1}.CSV",
                                                                          CargoReportConstants.CgoBillingCategory,
                                                                          cargoInvoice.InvoiceNumber)).ToUpper(),
                                               cgoAwbSpecialRecords);
                logger.InfoFormat("Interline Cargo AWB Listing report for Invoice [{0}] generated.",
                                  cargoInvoice.InvoiceNumber);

                #endregion

            }
        }

        private CgoAwbListingReport PopulateCgoAwbRecord(CargoInvoice cgoInvoice, AwbRecord awb, int awbSerialNo)
        {
            return new CgoAwbListingReport()
                       {

                           SerialNo = awbSerialNo.ToString(),
                           BillingAirlineCode = cgoInvoice.BillingMember.MemberCodeNumeric,
                           BilledAirlineCode = cgoInvoice.BilledMember.MemberCodeNumeric,
                           InvoiceNumber = cgoInvoice.InvoiceNumber,
                           BillingMonthYear = CgoCsvReportHelper.GetFormattedBillingMonthYear(cgoInvoice.BillingMonth,
                                                                               cgoInvoice.BillingYear,
                                                                               CargoReportConstants.
                                                                               BillingMonthYearDateFormat),
                           PeriodNo = cgoInvoice.BillingPeriod.ToString("00"),
                           BillingCode = CgoCsvReportHelper.GetBillingCodeCharacter((BillingCode)awb.BillingCodeId),
                           BatchNo = awb.BatchSequenceNumber.ToString(),
                           SequenceNo = awb.RecordSequenceWithinBatch.ToString(),
                           AwbIssueDate = awb.AwbDate.HasValue ? awb.AwbDate.Value.ToString(CargoReportConstants.AwbIssueDateFormat) : string.Empty,
                           IssueAirline = awb.AwbIssueingAirline,
                           AwbSerialNo = awb.AwbSerialNumber.ToString(),
                           CheckDigit = awb.AwbCheckDigit.ToString(),
                           ConsignmentOrig = awb.ConsignmentOriginId,
                           ConsignmentDest = awb.ConsignmentDestinationId,
                           CarriageFrom = awb.CarriageFromId,
                           CarriageTo = awb.CarriageToId,
                           CarriageDate = awb.DateOfCarriage.HasValue ? awb.DateOfCarriage.Value.ToString(CargoReportConstants.AwbCarriageDateFormat) : string.Empty,
                           ListingCurrency = cgoInvoice.ListingCurrencyDisplayText,
                           WeightCharges = awb.WeightCharges.HasValue ? awb.WeightCharges.Value.ToString(CargoReportConstants.DecimalStringFormat) : string.Empty,
                           ValuationCharges = awb.ValuationCharges.HasValue ? awb.ValuationCharges.Value.ToString(CargoReportConstants.DecimalStringFormat) : string.Empty,
                           OtherCharges = awb.OtherCharges.ToString(CargoReportConstants.DecimalStringFormat),
                           WtChargesSubIS = awb.AmountSubjectToIsc.ToString(CargoReportConstants.DecimalStringFormat),
                           IscRate = awb.IscPer.ToString(CargoReportConstants.DecimalStringFormat),
                           IscAmount = awb.IscAmount.ToString(CargoReportConstants.DecimalStringFormat),
                           VatAmount = awb.VatAmount.HasValue ? awb.VatAmount.Value.ToString(CargoReportConstants.DecimalStringFormat) : string.Empty,
                           AwbTotalAmount = awb.AwbTotalAmount.HasValue ? awb.AwbTotalAmount.Value.ToString(CargoReportConstants.DecimalStringFormat) : string.Empty,
                           CurrAdjustmentInd = awb.CurrencyAdjustmentIndicator,
                           BilledWeight = awb.BilledWeight.HasValue ? awb.BilledWeight.Value.ToString(CargoReportConstants.DecimalStringFormat) : string.Empty,
                           KgLbInd = awb.KgLbIndicator,
                           PartShipment = awb.PartShipmentIndicator,
                           ProvisoReqSpaInd = awb.ProvisoReqSpa,
                           CcaInd = awb.CcaIndicator ? "Y" : "N",
                           ProratePercent = awb.ProratePer.ToString()
                       };
        }


        private CgoRmBmCmListingReport PopulateCgoRejectionMemoRecord(CargoInvoice cgoInvoice, CargoRejectionMemo rejectionMemo, int serialNo)
        {
            var transactionType = GetTransactionType(rejectionMemo.RejectionStage);

            var reasonCodesfromDb = reasonCodes.Single(rCode => rCode.Code == rejectionMemo.ReasonCode && rCode.TransactionTypeId == (int)transactionType);

            return new CgoRmBmCmListingReport()
                       {
                           SerialNo = serialNo.ToString(),
                           BillingAirlineCode = cgoInvoice.BillingMember.MemberCodeNumeric,
                           BilledAirlineCode = cgoInvoice.BilledMember.MemberCodeNumeric,
                           InvoiceNumber = cgoInvoice.InvoiceNumber,
                           BillingMonthYear =
                               CgoCsvReportHelper.GetFormattedBillingMonthYear(cgoInvoice.BillingMonth,
                                                                               cgoInvoice.BillingYear,
                                                                               CargoReportConstants.
                                                                                   BillingMonthYearDateFormat),
                           PeriodNo = cgoInvoice.BillingPeriod.ToString(),
                           BillingCode = CgoCsvReportHelper.GetBillingCodeCharacter((BillingCode)rejectionMemo.BillingCode),
                           BatchNo = rejectionMemo.BatchSequenceNumber.ToString(),
                           SequenceNo = rejectionMemo.RecordSequenceWithinBatch.ToString(),
                           MemoNumber = rejectionMemo.RejectionMemoNumber,
                           StageNo = rejectionMemo.RejectionStage.ToString(),
                           CurrencyOfListing = cgoInvoice.ListingCurrencyDisplayText,
                           WeightChargeAmount = rejectionMemo.TotalWeightChargeDifference.HasValue ? rejectionMemo.TotalWeightChargeDifference.Value.ToString(CargoReportConstants.DecimalStringFormat) : string.Empty,
                           ValuationChargeAmount = rejectionMemo.TotalValuationChargeDifference.HasValue ? rejectionMemo.TotalValuationChargeDifference.Value.ToString(CargoReportConstants.DecimalStringFormat) : string.Empty,
                           OtherCommissionAmount = rejectionMemo.TotalOtherChargeDifference.HasValue ? rejectionMemo.TotalOtherChargeDifference.Value.ToString(CargoReportConstants.DecimalStringFormat) : string.Empty,
                           IscAmount = rejectionMemo.TotalIscAmountDifference.HasValue ? rejectionMemo.TotalIscAmountDifference.Value.ToString(CargoReportConstants.DecimalStringFormat) : string.Empty,
                           VatAmount = rejectionMemo.TotalVatAmountDifference.HasValue ? rejectionMemo.TotalVatAmountDifference.Value.ToString(CargoReportConstants.DecimalStringFormat) : string.Empty,
                           NetRejectCreditAmount = rejectionMemo.TotalNetRejectAmount.HasValue ? rejectionMemo.TotalNetRejectAmount.Value.ToString(CargoReportConstants.DecimalStringFormat) : string.Empty,
                           ReasonCode = rejectionMemo.ReasonCode,
                           ReasonDescription = reasonCodesfromDb != null ? reasonCodesfromDb.Description : string.Empty
                       };
        }

        /// <summary>
        /// Gets the type of the transaction.
        /// </summary>
        /// <param name="rejectionStage">The rejection stage.</param>
        /// <returns></returns>
        /// <remarks>Returns transaction type as Rejection Memo 1 if invalid rejection stage passed.</remarks> 
        private static TransactionType GetTransactionType(int rejectionStage)
        {
            TransactionType transactionType;
            switch (rejectionStage)
            {
                case 1:
                    transactionType = TransactionType.CargoRejectionMemoStage1;
                    break;
                case 2:
                    transactionType = TransactionType.CargoRejectionMemoStage2;
                    break;
                case 3:
                    transactionType = TransactionType.CargoRejectionMemoStage3;
                    break;
                default:
                    transactionType = TransactionType.CargoRejectionMemoStage1;
                    break;
            }

            return transactionType;
        }

        private CgoRmBmCmListingReport PopulateCgoBillingMemoRecord(CargoInvoice cgoInvoice, CargoBillingMemo billingMemo, int serialNo)
        {
            var transactionTypeId = billingMemo.ReasonCode == "6A"
                                    ? (int)TransactionType.CargoBillingMemoDueToAuthorityToBill
                                    : billingMemo.ReasonCode == "6B" ? (int)TransactionType.CargoBillingMemoDueToExpiry : (int)TransactionType.CargoBillingMemo;
            var reasonCodesfromDb = reasonCodes.Single(rCode => rCode.Code == billingMemo.ReasonCode && rCode.TransactionTypeId == (int)transactionTypeId);
			
            return new CgoRmBmCmListingReport()
            {
                SerialNo = serialNo.ToString(),
                BillingAirlineCode = cgoInvoice.BillingMember.MemberCodeNumeric,
                BilledAirlineCode = cgoInvoice.BilledMember.MemberCodeNumeric,
                InvoiceNumber = cgoInvoice.InvoiceNumber,
                BillingMonthYear =
                    CgoCsvReportHelper.GetFormattedBillingMonthYear(cgoInvoice.BillingMonth,
                                                                    cgoInvoice.BillingYear,
                                                                    CargoReportConstants.
                                                                        BillingMonthYearDateFormat),
                PeriodNo = cgoInvoice.BillingPeriod.ToString(),
                BillingCode = CgoCsvReportHelper.GetBillingCodeCharacter((BillingCode)billingMemo.BillingCode),
                BatchNo = billingMemo.BatchSequenceNumber.ToString(),
                SequenceNo = billingMemo.RecordSequenceWithinBatch.ToString(),
                MemoNumber = billingMemo.BillingMemoNumber,
                StageNo = string.Empty,
                CurrencyOfListing = cgoInvoice.ListingCurrencyDisplayText,
                WeightChargeAmount = billingMemo.BilledTotalWeightCharge.HasValue ? billingMemo.BilledTotalWeightCharge.Value.ToString(CargoReportConstants.DecimalStringFormat) : string.Empty,
                ValuationChargeAmount = billingMemo.BilledTotalValuationAmount.HasValue ? billingMemo.BilledTotalValuationAmount.Value.ToString(CargoReportConstants.DecimalStringFormat) : string.Empty,
                OtherCommissionAmount = billingMemo.BilledTotalOtherChargeAmount.ToString(CargoReportConstants.DecimalStringFormat),
                IscAmount = billingMemo.BilledTotalIscAmount.ToString(CargoReportConstants.DecimalStringFormat),
                VatAmount = billingMemo.BilledTotalVatAmount.HasValue ? billingMemo.BilledTotalVatAmount.Value.ToString(CargoReportConstants.DecimalStringFormat) : string.Empty,
                NetRejectCreditAmount = billingMemo.NetBilledAmount.HasValue ? billingMemo.NetBilledAmount.Value.ToString(CargoReportConstants.DecimalStringFormat) : string.Empty,
                ReasonCode = billingMemo.ReasonCode,
                ReasonDescription = reasonCodesfromDb != null ? reasonCodesfromDb.Description : string.Empty

            };
        }

        private CgoRmBmCmListingReport PopulateCgoCreditMemoRecord(CargoInvoice cgoInvoice, CargoCreditMemo creditMemo, int serialNo)
        {
          var reasonCodesfromDb = reasonCodes.Single(rCode => rCode.Code == creditMemo.ReasonCode && rCode.TransactionTypeId == (int)TransactionType.CargoCreditMemo);

            return new CgoRmBmCmListingReport()
            {
                SerialNo = serialNo.ToString(),
                BillingAirlineCode = cgoInvoice.BillingMember.MemberCodeNumeric,
                BilledAirlineCode = cgoInvoice.BilledMember.MemberCodeNumeric,
                InvoiceNumber = cgoInvoice.InvoiceNumber,
                BillingMonthYear =
                    CgoCsvReportHelper.GetFormattedBillingMonthYear(cgoInvoice.BillingMonth,
                                                                    cgoInvoice.BillingYear,
                                                                    CargoReportConstants.
                                                                        BillingMonthYearDateFormat),
                PeriodNo = cgoInvoice.BillingPeriod.ToString(),
                BillingCode = CgoCsvReportHelper.GetBillingCodeCharacter((BillingCode)creditMemo.BillingCode),
                BatchNo = creditMemo.BatchSequenceNumber.ToString(),
                SequenceNo = creditMemo.RecordSequenceWithinBatch.ToString(),
                MemoNumber = creditMemo.CreditMemoNumber,
                StageNo = string.Empty,
                CurrencyOfListing = cgoInvoice.ListingCurrencyDisplayText,
                WeightChargeAmount = creditMemo.TotalWeightCharges.HasValue ? creditMemo.TotalWeightCharges.Value.ToString(CargoReportConstants.DecimalStringFormat) : string.Empty,
                ValuationChargeAmount = creditMemo.TotalValuationAmt.HasValue ? creditMemo.TotalValuationAmt.Value.ToString(CargoReportConstants.DecimalStringFormat) : string.Empty,
                OtherCommissionAmount = creditMemo.TotalOtherChargeAmt.ToString(CargoReportConstants.DecimalStringFormat),
                IscAmount = creditMemo.TotalIscAmountCredited.ToString(CargoReportConstants.DecimalStringFormat),
                VatAmount = creditMemo.TotalVatAmountCredited.HasValue ? creditMemo.TotalVatAmountCredited.Value.ToString(CargoReportConstants.DecimalStringFormat) : string.Empty,
                NetRejectCreditAmount = creditMemo.NetAmountCredited.HasValue ? creditMemo.NetAmountCredited.Value.ToString(CargoReportConstants.DecimalStringFormat) : string.Empty,
                ReasonCode = creditMemo.ReasonCode,
                ReasonDescription = reasonCodesfromDb != null ? reasonCodesfromDb.Description : string.Empty
            };
        }
    }
}

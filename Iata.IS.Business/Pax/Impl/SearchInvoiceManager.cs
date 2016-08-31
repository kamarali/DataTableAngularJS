using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Iata.IS.Business.Common;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Data.Pax;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using log4net;

namespace Iata.IS.Business.Pax.Impl
{
    public class SearchInvoiceManager : ISearchInvoiceManager
    {
        public IRepository<InvoiceTotal> InvoiceTotalRecordRepository { get; set; }
        public IRepository<PaxInvoiceSearch> PaxInvoiceSearchRepository { get; set; }
        public IManagePaxInvoiceRepository ManagePaxInvoiceRepository { get; set; }
        
        //Created object to monitor performance of SP by loging in and out time.
        ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        /// <summary>
        /// Get invoices based on params.
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="currentPage"></param>
        /// <param name="noOfRecords"></param>
        /// <returns></returns>
        public List<PaxInvoiceSearchDetails> GetInvoices(SearchCriteria searchCriteria, int pageNo, int pageSize, string sortColumn, string sortOrder)
        {
            Logger.Info("SP CAll: Iata.IS.Business:GetInvoices() Started");

            //SCP - 85037: Calling stored procedure to get desired data. Previously view was calling
            var filteredList = ManagePaxInvoiceRepository.GetPaxManageInvoices(searchCriteria, pageSize, pageNo, sortColumn, sortOrder);

            Logger.Info("SP CAll: Iata.IS.Business:GetInvoices() Completed ");

            return filteredList;
        }
        /// <summary>
        /// Get all the Payables invoices
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        public IQueryable<PaxInvoiceSearch> GetAllPayables(SearchCriteria searchCriteria)
        {
            // Check if invoice number is passed in search criteria.
            // if passed,then find invoices with specified invoice number.

            //var filteredList = _invoiceRepository.GetAllPayables();
            var filteredList = PaxInvoiceSearchRepository.GetAll();

            if (searchCriteria != null)
            {
                //Display invoice with Invoice Status Presented
                filteredList = filteredList.Where(invoice => invoice.InvoiceStatusId == (int)InvoiceStatusType.Presented);

                // Check if billing year and billing month is passed in search criteria.
                // if passed,then find invoices with specified billing year and billing month.
                if (searchCriteria.BillingYear > 0 && searchCriteria.BillingMonth > 0)
                {
                    filteredList = filteredList.Where(invoice => invoice.BillingYear == searchCriteria.BillingYear && invoice.BillingMonth == searchCriteria.BillingMonth);
                }

                // Check if billing period is passed in search criteria.
                // if passed,then find invoices with specified billing period.
                if (searchCriteria.BillingPeriod > 0)
                {
                    filteredList = filteredList.Where(invoice => invoice.BillingPeriod == searchCriteria.BillingPeriod);
                }

                // Check if billing code is passed in search criteria
                // if passed,then find invoices with specified billing code
                if (searchCriteria.BillingCode != null && searchCriteria.BillingCode >= 0)
                {
                    filteredList = filteredList.Where(invoice => invoice.BillingCodeId == searchCriteria.BillingCode.Value);
                }

                // Check if billing member is passed in search criteria
                // if passed,then find invoices with specified billing member ID
                if (searchCriteria.BillingMemberId > 0)
                {
                    filteredList = filteredList.Where(invoice => invoice.BillingMemberId == searchCriteria.BillingMemberId);
                }

                // Check if billed member is passed in search criteria.
                // if passed,then find invoices with specified billed member ID.
                // Note: Do not check whether billed member id is greater than 0 - since IS-OPS users will have 0 member id. They should not see any member invoices.
                filteredList = filteredList.Where(invoice => invoice.BilledMemberId == searchCriteria.BilledMemberId);

                // Check if Invoice SMI is passed in search criteria.
                // if passed,then find invoices with specified SMI.
                if (searchCriteria.InvoiceSmi > 0)
                {
                    filteredList = filteredList.Where(invoice => invoice.SettlementMethodId == searchCriteria.SettlementMethodId);
                }

                // Check if invoice number is passed in search criteria.
                // if passed,then find invoices with specified invoice number.
                if (!string.IsNullOrEmpty(searchCriteria.InvoiceNumber))
                {
                    filteredList = filteredList.Where(invoice => invoice.InvoiceNumber.ToUpper().Contains(searchCriteria.InvoiceNumber.ToUpper()));
                }

                //Check if Filename is passed in search criteria
                //if passed,then find invoices with specified File name contains
                //if (!string.IsNullOrEmpty(searchCriteria.FileName))
                //{
                //  filteredList = filteredList.Where(invoice => invoice.InputFileNameDisplayText.ToUpper().Contains(searchCriteria.FileName.ToUpper()));
                //}
            }
            return filteredList;
        }

    }
}

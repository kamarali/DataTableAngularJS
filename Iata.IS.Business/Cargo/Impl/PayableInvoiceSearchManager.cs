using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Cargo;
using Iata.IS.Data;
using Iata.IS.Business.Cargo;

namespace Iata.IS.Business.Cargo.Impl
{
    class PayableInvoiceSearchManager: IPayableInvoiceSearchManager
    {
        // public IRepository<InvoiceTotal> InvoiceTotalRecordRepository { get; set; }
        public IRepository<CargoInvoiceSearch> CargoInvoiceSearchRepository { get; set; }

        /// <summary>
        /// Gets or sets the reference manager.
        /// </summary>
        /// <value>The reference manager.</value>
        //public IReferenceManager ReferenceManager { get; set; }

        public IQueryable<CargoInvoiceSearch> GetInvoices(SearchCriteria searchCriteria)
        {
            // Check if invoice number is passed in search criteria.
            // if passed,then find invoices with specified invoice number.
            // System.Linq.IQueryable<Iata.IS.Model.Cargo.CargoInvoiceSearch> tt=null;
            var filteredList = CargoInvoiceSearchRepository.GetAll();

            //Comment on 7.sep.11
            // var filteredList = new List<CargoInvoiceSearch>();
            //end Comment on 7.sep.11
            if (searchCriteria != null)
            {
                if (!string.IsNullOrEmpty(searchCriteria.InvoiceNumber))
                {
                    filteredList = filteredList.Where(invoice => invoice.InvoiceNumber.ToUpper().Contains(searchCriteria.InvoiceNumber.ToUpper()));
                }
                
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

                // Check if billed member is passed in search criteria.
                // if passed,then find invoices with specified billed member ID.
                if (searchCriteria.BilledMemberId > 0)
                {
                    filteredList = filteredList.Where(invoice => invoice.BilledMemberId == searchCriteria.BilledMemberId);
                }

                // Check if settlement method is passed in search criteria.
                // if passed,then find invoices with specified settlement method.
                //if (searchCriteria.InvoiceSmi > 0)
                //{
                //    filteredList = filteredList.Where(invoice => invoice.SettlementMethodId == searchCriteria.SMI);
                //}

                // Check if invoice status is passed in search criteria.
                // if passed,then find invoices with specified invoice status.

                if (searchCriteria.InvoiceStatus > 0)
                {
                    filteredList = filteredList.Where(invoice => invoice.InvoiceStatusId == searchCriteria.InvoiceStatusId);
                }

                //Check if submission method is passed in search criteria
                //if passed,then find invoices with specified submission method

                //TODO: Submission method field is not yet present in database.This code should be uncommented once the field is in place

                //if (searchCriteria.SMI > 0) 
                //{
                //    filteredList = filteredList.Where(invoice => invoice.SubmissionMethodId == searchCriteria.SMI);
                //}

                //Check if submission method is passed in search criteria
                //if passed,then find invoices with specified submission method
                if (!string.IsNullOrEmpty(searchCriteria.FileName))
                {
                    filteredList = filteredList.Where(invoice => invoice.InputFileNameDisplayText.ToUpper().Contains(searchCriteria.FileName.ToUpper()));
                }

                //if passed,then find invoices for specific user id.
                if (searchCriteria.OwnerId > 0)
                {
                    filteredList = filteredList.Where(invoice => invoice.InvoiceOwnerId == searchCriteria.OwnerId);
                }

            }

            return filteredList.AsQueryable();
        }

        /// <summary>
        /// Get all the Payables invoices
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>

       // public IQueryable<CargoInvoiceSearch> GetAllPayables(PayableSearch searchCriteria)
        public IQueryable<CargoInvoiceSearch> GetAllPayables(SearchCriteria searchCriteria)
        {
            // Check if invoice number is passed in search criteria.
            // if passed,then find invoices with specified invoice number.

            //var filteredList = _invoiceRepository.GetAllPayables();
            //var filteredList = PaxInvoiceSearchRepository.GetAll();
            System.Linq.IQueryable<Iata.IS.Model.Cargo.CargoInvoiceSearch> DD = null;
            var filteredList = new List<CargoInvoiceSearch>();
            return filteredList.AsQueryable();
        }
    }
}

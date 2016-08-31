using System.Collections.Generic;
using System.Linq;
using Iata.IS.Data;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Cargo;
using Iata.IS.Data.Cargo;
//using Iata.IS.Model.Cargo.Enums;

namespace Iata.IS.Business.Cargo.Impl
{
  public class SearchInvoiceManager : ISearchInvoiceManager
  {
   // public IRepository<InvoiceTotal> InvoiceTotalRecordRepository { get; set; }
      public IRepository<CargoInvoiceSearch> CargoInvoiceSearchRepository { get; set; }
      public ICargoInvoiceRepository CargoInvoiceRepository { get; set; }

    /// <summary>
    /// Gets or sets the reference manager.
    /// </summary>
    /// <value>The reference manager.</value>
    //public IReferenceManager ReferenceManager { get; set; }

      public IQueryable<CargoInvoiceSearchDetails> GetInvoices(SearchCriteria searchCriteria, int pageNo, int pageSize, string sortColumn, string sortOrder)
      {
          //SCP - 85037: Calling stored procedure to get desired data. Previously view was calling
          var filteredList = CargoInvoiceRepository.GetCargoManageInvoices(searchCriteria, pageSize, pageNo, sortColumn, sortOrder);

          #region Old Code before SCP85039
          //// Check if invoice number is passed in search criteria.
          //// if passed,then find invoices with specified invoice number.
          // // System.Linq.IQueryable<Iata.IS.Model.Cargo.CargoInvoiceSearch> tt=null;
          //  var filteredList = CargoInvoiceSearchRepository.GetAll();

          //  //Comment on 7.sep.11
          // // var filteredList = new List<CargoInvoiceSearch>();
          //  //end Comment on 7.sep.11
          //  if (searchCriteria != null)
          //  {

          //      if (!string.IsNullOrEmpty(searchCriteria.InvoiceNumber))
          //      {

          //          filteredList = filteredList.Where(invoice => invoice.InvoiceNumber.ToUpper().Contains(searchCriteria.InvoiceNumber.ToUpper()));
          //      }

          //      //// Check if invoice status is passed in search criteria.
          //      //// if passed,then find invoices with specified invoice status.
          //      //if (searchCriteria.InvoiceStatus > 0)
          //      //{

          //      //    filteredList = filteredList.Where(invoice => invoice.InvoiceStatusId == searchCriteria.InvoiceStatusId);
          //      //}


          //      // Check if billing year and billing month is passed in search criteria.
          //      // if passed,then find invoices with specified billing year and billing month.
          //      if (searchCriteria.BillingYear > 0 && searchCriteria.BillingMonth > 0)
          //      {

          //          filteredList = filteredList.Where(invoice => invoice.BillingYear == searchCriteria.BillingYear && invoice.BillingMonth == searchCriteria.BillingMonth);
          //      }


          //      // Check if billing period is passed in search criteria.
          //      // if passed,then find invoices with specified billing period.
          //      if (searchCriteria.BillingPeriod > 0)
          //      {

          //          filteredList = filteredList.Where(invoice => invoice.BillingPeriod == searchCriteria.BillingPeriod);
          //      }


          //      //// Check if billing code is passed in search criteria
          //      //// if passed,then find invoices with specified billing code
          //      //if (searchCriteria.BillingCode != null && searchCriteria.BillingCode >= 0)
          //      //{

          //      //    filteredList = filteredList.Where(invoice => invoice.BillingCodeId == searchCriteria.BillingCode.Value);
          //      //}


          //      // Check if billing member is passed in search criteria
          //      // if passed,then find invoices with specified billing member ID
          //      // Note: Do not check whether billing member id is greater than 0 - since IS-OPS users will have 0 member id. They should not see any member invoices.

          //      filteredList = filteredList.Where(invoice => invoice.BillingMemberId == searchCriteria.BillingMemberId);

          //      // Check if billed member is passed in search criteria.
          //      // if passed,then find invoices with specified billed member ID.
          //      if (searchCriteria.BilledMemberId > 0)
          //      {

          //          filteredList = filteredList.Where(invoice => invoice.BilledMemberId == searchCriteria.BilledMemberId);
          //      }


          //      // Check if settlement method is passed in search criteria.
          //      // if passed,then find invoices with specified settlement method.
          //      if (searchCriteria.InvoiceSmi > 0)
          //      {

          //          filteredList = filteredList.Where(invoice => invoice.SettlementMethodId == searchCriteria.SettlementMethodId);
          //      }


          //      // Check if invoice status is passed in search criteria.
          //      // if passed,then find invoices with specified invoice status.

          //      if (searchCriteria.InvoiceStatus > 0)
          //      {

          //          filteredList = filteredList.Where(invoice => invoice.InvoiceStatusId == searchCriteria.InvoiceStatusId);
          //      }


          //      //Check if submission method is passed in search criteria
          //      //if passed,then find invoices with specified submission method

          //      //TODO: Submission method field is not yet present in database.This code should be uncommented once the field is in place

          //      if (searchCriteria.SubmissionMethodId > 0)
          //      {

          //          filteredList = filteredList.Where(invoice => invoice.SubmissionMethodId == searchCriteria.SubmissionMethodId);
          //      }


          //      //Check if submission method is passed in search criteria
          //      //if passed,then find invoices with specified submission method
          //      if (!string.IsNullOrEmpty(searchCriteria.FileName))
          //      {

          //          filteredList = filteredList.Where(invoice => invoice.InputFileNameDisplayText.ToUpper().Contains(searchCriteria.FileName.ToUpper()));
          //      }


          //      //if passed,then find invoices for specific user id.
          //      if (searchCriteria.OwnerId > 0)
          //      {

          //          filteredList = filteredList.Where(invoice => invoice.InvoiceOwnerId == searchCriteria.OwnerId);
          //      }


          //      //// DiplayText Retrieval from miscCodes and setting it to required property 
          //      //foreach (var paxInvoice in filteredList)
          //      //{

          //      //  //paxInvoice.InvoiceStatusDisplayText = paxInvoice.InvoiceStatusDisplayText = ReferenceManager.GetInvoiceStatusDisplayValue(paxInvoice.InvoiceStatusId);
          //      //  //paxInvoice.SettlementMethodDisplayText = ReferenceManager.GetSettlementMethodDisplayValueForSearchResult(paxInvoice.SettlementMethodId);
          //      //  //paxInvoice.SubmissionMethodDisplayText = ReferenceManager.GetDisplayValue(MiscGroups.FileSubmissionMethod, paxInvoice.SubmissionMethodId);
          //      //}
          //  } 
          #endregion
          return filteredList.AsQueryable();
      }
    /// <summary>
    /// Get all the Payables invoices
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <returns></returns>
    public IQueryable<CargoInvoiceSearch> GetAllPayables(SearchCriteria searchCriteria)
    {
      // Check if invoice number is passed in search criteria.
      // if passed,then find invoices with specified invoice number.
        var filteredList = CargoInvoiceSearchRepository.GetAll();
        //System.Linq.IQueryable<Iata.IS.Model.Cargo.CargoInvoiceSearch> DD = null;
        //var filteredList = new List<CargoInvoiceSearch>();


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

            //// Check if billing code is passed in search criteria
            //// if passed,then find invoices with specified billing code
            //if (searchCriteria.BillingCode != null && searchCriteria.BillingCode >= 0)
            //{
            //    filteredList = filteredList.Where(invoice => invoice.BillingCodeId == searchCriteria.BillingCode.Value);
            //}

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
      return filteredList.AsQueryable();
    }

  }
}

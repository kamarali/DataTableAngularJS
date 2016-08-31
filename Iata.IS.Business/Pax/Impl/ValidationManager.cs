using System;
using System.Linq;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Base;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Business.Pax.Impl
{
  public class ValidationManager : IValidationManager
  {
    /// <summary>
    /// Calendar Manager, will be injected by the container.
    /// </summary>
    /// <value>The calendar manager repository.</value>
    public ICalendarManager CalendarManager { get; set; }

    /// <summary>
    /// Invoice Repository, will be injected by the container.
    /// </summary>
    public IRepository<Invoice> InvoiceRepository { get; set; }

    /// <summary>
    /// Tax Repository, will be injected by the container.
    /// </summary>
    public IRepository<TaxCode> TaxRepository { get; set; }

    /// <summary>
    /// Airport Repository, will be injected by the container.
    /// </summary>
    public IRepository<Airport> AirportRepository { get; set; }

    /// <summary>
    /// Coupon Record Tax Repository, will be injected by the container.
    /// </summary>
    public IRepository<CouponRecordTax> CouponRecordTaxRepository { get; set; }

    /// <summary>
    /// Currency Adjustment Indicator Repository, will be injected by the container.
    /// </summary>
    public IRepository<Currency> CurrencyAdjustmentIndicatorRepository { get; set; }

    /// <summary>
    /// Gets or sets the source code repository.
    /// </summary>
    /// <value>The source code repository.</value>
    public IRepository<SourceCode> SourceCodeRepository { get; set; }


    /// <summary>
    /// Gets or sets the currency repository.
    /// </summary>
    /// <value>The currency repository.</value>
    public IRepository<Currency> CurrencyRepository { get; set; }

    /// <summary>
    /// Gets or sets the member repository.
    /// </summary>
    /// <value>The member repository.</value>
    public IRepository<Member> MemberRepository { get; set; }

    /// <summary>
    /// Gets or sets the airline repository.
    /// </summary>
    /// <value>The airline repository.</value>
    //public IRepository<Airline> AirlineRepository { get; set; }  
   
    /// <summary>
    /// Validates the invoice date by comparing it with current billing period closure date.
    /// </summary>
    /// <param name="invoiceDate">The invoice date.</param>
    /// <returns>True if successful; otherwise false.</returns>
    public bool ValidateInvoiceDate(DateTime invoiceDate)
    {
      var result = true;
      var billingPeriod = CalendarManager.GetCurrentBillingPeriod(DateTime.Today);

      // Invoice date should not be greater than current billing period closure date.
      if(invoiceDate > billingPeriod.EndDate)
      {
        result = false;
      }

      return result;
    }

    /// <summary>
    /// Validates the invoice number.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <returns>True if successful; otherwise false.</returns>
    public bool ValidateInvoiceNumber(string invoiceNumber, int billingYear, int billingMemberId)
    {
      //The ‘Invoice Number’ provided by the Billing Member should be unique for that calendar year 
     
      //var lastYearInvoices = InvoiceRepository.GetQueryable(invoice => invoice.BillingYear == invoiceHeader.BillingYear && invoice.BillingMemberId == invoiceHeader.BillingMemberId);
      // TODO: Remove this code once member profile EDMX mapping is done.
      var lastYearInvoices = InvoiceRepository.Get(invoice => invoice.BillingYear == billingYear);
      var duplicateInvoiceCount = lastYearInvoices.Count(invoice => invoice.InvoiceNumber.ToUpper() == invoiceNumber.ToUpper()) > 0;

      if (duplicateInvoiceCount)
      {
        throw new ISBusinessException(ErrorCodes.DuplicateInvoiceFound);
      }

      // TODO: Stored Procedure call to find duplicate invoice number for billing member id for that calendar year. Need to uncomment once Member Profile EDMX mapping done.
      //var duplicateInvoiceCount = InvoiceRepository.IsInvoiceNumberExists(invoiceNumber, billingMemberId);

      //if (duplicateInvoiceCount > 0)
      //{
      //  throw new ISBusinessException(ErrorCodes.DuplicateInvoiceFound);
      //}

      return true;
    }

    /// <summary>
    /// Validates the Currency Adjustment Indicator.
    /// </summary>
    /// <param name="currencyAdjustmentIndicator">The Currency Adjustment Indicator.</param>
    /// <returns>True if successful; otherwise false.</returns>
    public bool ValidateCurrencyAdjustmentIndicator(string currencyAdjustmentIndicator)
    {
      var result = true;
      var countCurrency = CurrencyAdjustmentIndicatorRepository.GetCount(currAdjustRecord => currAdjustRecord.Code == currencyAdjustmentIndicator);

      if (countCurrency == 0)
      {
        result = false;
      }

      return result;
    }

    /// <summary>
    /// Validates the tax code.
    /// </summary>
    /// <param name="taxCode">The tax code.</param>
    /// <returns>True if successful; otherwise false.</returns>
    public bool ValidateTaxCode(string taxCode)
    {
      var result = true;
      var countTaxCode = TaxRepository.GetCount(taxCodeObject => taxCodeObject.Description == taxCode);

      if (countTaxCode == 0)
      {
        result = false;
      }

      return result;
    }

    /// <summary>
    /// Validates the Airport code.
    /// </summary>
    /// <param name="airportCode">The From/To Airport code.</param>
    /// <returns>True if successful; otherwise false.</returns>
    public bool ValidateAirportCode(string airportCode)
    {
      var result = true;
      var airportRecordCode = AirportRepository.Single(airportRecord => airportRecord.IcaoCode == airportCode);

      if (airportRecordCode == null)
      {
        result = false;
      }

      return result;
    }

    /// <summary>
    /// Determines whether [is valid source code] [the specified source code id].
    /// </summary>
    /// <param name="sourceCodeId">The source code id.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid source code] [the specified source code id]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidSourceCode(int sourceCodeId, int transactionTypeId)
    {
      return SourceCodeRepository.GetCount(sourceCode => sourceCode.Id == sourceCodeId && sourceCode.TransactionTypeId == transactionTypeId) > 0;
    }


    /// <summary>
    /// Determines whether [is valid currency code] [the specified currency code].
    /// </summary>
    /// <param name="currencyCode">The currency code.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid currency code] [the specified currency code]; otherwise, <c>false</c>.
    /// </returns>
    public bool ValidateCurrencyCode(string currencyCode)
    {
      return CurrencyRepository.GetCount(currency => currency.Code == currencyCode) > 0;
    }
    
    /// <summary>
    /// Validates the air line code.
    /// </summary>
    /// <param name="airlineCode">The airline code.</param>
    /// <returns></returns>
    public bool ValidateAirLineCode(string airlineCode)
    {
      return MemberRepository.GetCount(member => member.MemberCodeNumeric == airlineCode) > 0;
    }
  }
}

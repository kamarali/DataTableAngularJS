using System;

namespace Iata.IS.Business.Pax
{
  /// <summary>
  /// 
  /// </summary>
  public interface IValidationManager
  {
    /// <summary>
    /// Validates the invoice date.
    /// </summary>
    /// <param name="invoiceDate">The invoice date.</param>
    /// <returns>True if successful; otherwise false.</returns>
    bool ValidateInvoiceDate(DateTime invoiceDate);

    /// <summary>
    /// Validates the invoice number.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <returns>True if successful; otherwise false.</returns>
    bool ValidateInvoiceNumber(string invoiceNumber, int billingYear, int billingMemberId);

    /// <summary>
    /// Validates the tax code.
    /// </summary>
    /// <param name="taxCode">The tax code.</param>
    /// <returns>True if successful; otherwise false.</returns>
    bool ValidateTaxCode(string taxCode);

    /// <summary>
    /// Validates the airport code.
    /// </summary>
    /// <param name="airportCode">The airport code.</param>
    /// <returns>True if successful; otherwise false.</returns>
    bool ValidateAirportCode(string airportCode);

    /// <summary>
    /// Validates the Currency Adjustment Indicator.
    /// </summary>
    /// <param name="currencyAdjustmentIndicator">The Currency Adjustment Indicator.</param>
    /// <returns>True if successful; otherwise false.</returns>
    bool ValidateCurrencyAdjustmentIndicator(string currencyAdjustmentIndicator);

    /// <summary>
    /// Determines whether [is valid source code] [the specified source code id].
    /// </summary>
    /// <param name="sourceCodeId">The source code id.</param>
    /// <param name="transactionTypeId"></param>
    /// <returns>
    /// 	<c>true</c> if [is valid source code] [the specified source code id]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidSourceCode(int sourceCodeId, int transactionTypeId);

    /// <summary>
    /// Determines whether [is valid currency code] [the specified currency code].
    /// </summary>
    /// <param name="currencyCode">The currency code.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid currency code] [the specified currency code]; otherwise, <c>false</c>.
    /// </returns>
    bool ValidateCurrencyCode(string currencyCode);
    

    /// <summary>
    /// Validates the air line code.
    /// </summary>
    /// <param name="airlineCode">The airline code.</param>
    /// <returns></returns>
    bool ValidateAirLineCode(string airlineCode);

  }
}

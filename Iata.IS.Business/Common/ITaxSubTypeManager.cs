using System.Collections.Generic;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Business.Common
{
  public interface ITaxSubTypeManager
  {
    /// <summary>
    /// Adds the taxSubType.
    /// </summary>
    /// <param name="taxSubType">The taxSubType.</param>
    /// <returns></returns>
    TaxSubType AddTaxSubType(TaxSubType taxSubType);

    /// <summary>
    /// Updates the taxSubType.
    /// </summary>
    /// <param name="taxSubType">The taxSubType.</param>
    /// <returns></returns>
    TaxSubType UpdateTaxSubType(TaxSubType taxSubType);

    /// <summary>
    /// Deletes the taxSubType.
    /// </summary>
    /// <param name="taxSubTypeId">The taxSubType id.</param>
    /// <returns></returns>
    bool DeleteTaxSubType(int taxSubTypeId);

    /// <summary>
    /// Gets the taxSubType details.
    /// </summary>
    /// <param name="taxSubTypeId">The taxSubType id.</param>
    /// <returns></returns>
    TaxSubType GetTaxSubTypeDetails(int taxSubTypeId);

    /// <summary>
    /// Gets all taxSubType list.
    /// </summary>
    /// <returns></returns>
    List<TaxSubType> GetAllTaxSubTypeList();

    /// <summary>
    /// Gets the tax sub type list.
    /// </summary>
    /// <param name="taxSubType">The tax sub type.</param>
    /// <param name="taxType">The tax type.</param>
    /// <returns></returns>
    List<TaxSubType> GetTaxSubTypeList(string taxSubType, string taxType);
  }
}

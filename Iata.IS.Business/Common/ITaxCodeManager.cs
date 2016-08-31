using System.Collections.Generic;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Business.Common
{
    public interface ITaxCodeManager
    {
        /// <summary>
        /// Adds the tax code.
        /// </summary>
        /// <param name="taxCode">The tax code.</param>
        /// <returns></returns>
        TaxCode AddTaxCode(TaxCode taxCode);

        /// <summary>
        /// Updates the tax code.
        /// </summary>
        /// <param name="taxCode">The tax code.</param>
        /// <returns></returns>
        TaxCode UpdateTaxCode(TaxCode taxCode);

        /// <summary>
        /// Deletes the tax code.
        /// </summary>
        /// <param name="taxCodeId">The tax code id.</param>
        /// <returns></returns>
        bool DeleteTaxCode(string taxCodeId);

        /// <summary>
        /// Gets the tax code details.
        /// </summary>
        /// <param name="taxCodeId">The tax code id.</param>
        /// <returns></returns>
        TaxCode GetTaxCodeDetails(string taxCodeId);

        /// <summary>
        /// Gets all tax code list.
        /// </summary>
        /// <returns></returns>
        List<TaxCode> GetAllTaxCodeList();

        /// <summary>
        /// Gets the tax code list.
        /// </summary>
        /// <param name="TaxCodeTypeId">The tax code type id.</param>
        /// <param name="Description">The description.</param>
        /// <returns></returns>
        List<TaxCode> GetTaxCodeList(string TaxCodeId, string Description);
    }
}

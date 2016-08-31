using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax;

namespace Iata.IS.Business.Common
{
    public interface IVatIdentifierManager
    {
        /// <summary>
        /// Adds the vat identifier.
        /// </summary>
        /// <param name="vatIdentifier">The vat identifier.</param>
        /// <returns></returns>
        VatIdentifier AddVatIdentifier(VatIdentifier vatIdentifier);

        /// <summary>
        /// Updates the vat identifier.
        /// </summary>
        /// <param name="vatIdentifier">The vat identifier.</param>
        /// <returns></returns>
        VatIdentifier UpdateVatIdentifier(VatIdentifier vatIdentifier);

        /// <summary>
        /// Deletes the vat identifier.
        /// </summary>
        /// <param name="vatIdentifierId">The vat identifier id.</param>
        /// <returns></returns>
        bool DeleteVatIdentifier(int vatIdentifierId);

        /// <summary>
        /// Gets the vat identifier details.
        /// </summary>
        /// <param name="vatIdentifierId">The vat identifier id.</param>
        /// <returns></returns>
        VatIdentifier GetVatIdentifierDetails(int vatIdentifierId);

        /// <summary>
        /// Gets all vat identifier list.
        /// </summary>
        /// <returns></returns>
        List<VatIdentifier> GetAllVatIdentifierList();

        /// <summary>
        /// Gets the vat identifier list.
        /// </summary>
        /// <param name="vatIdentifier">The vat identifier.</param>
        /// <param name="billingCategory">The billing category.</param>
        /// <param name="Description">The description.</param>
        /// <returns></returns>
        List<VatIdentifier> GetVatIdentifierList(string Identifier, int billingCategory, string Description);
    }
}

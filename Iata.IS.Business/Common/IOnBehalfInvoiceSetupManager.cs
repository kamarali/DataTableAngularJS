using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
    public interface IOnBehalfInvoiceSetupManager
    {
        /// <summary>
        /// Adds the on behalf invoice setup.
        /// </summary>
        /// <param name="onBehalfInvoiceSetup">The on behalf invoice setup.</param>
        /// <returns></returns>
        OnBehalfInvoiceSetup AddOnBehalfInvoiceSetup(OnBehalfInvoiceSetup onBehalfInvoiceSetup);

        /// <summary>
        /// Updates the on behalf invoice setup.
        /// </summary>
        /// <param name="onBehalfInvoiceSetup">The on behalf invoice setup.</param>
        /// <returns></returns>
        OnBehalfInvoiceSetup UpdateOnBehalfInvoiceSetup(OnBehalfInvoiceSetup onBehalfInvoiceSetup);

        /// <summary>
        /// Deletes the on behalf invoice setup.
        /// </summary>
        /// <param name="onBehalfInvoiceSetupId">The on behalf invoice setup id.</param>
        /// <returns></returns>
        bool DeleteOnBehalfInvoiceSetup(int onBehalfInvoiceSetupId);

        /// <summary>
        /// Gets the on behalf invoice setup details.
        /// </summary>
        /// <param name="onBehalfInvoiceSetupId">The on behalf invoice setup id.</param>
        /// <returns></returns>
        OnBehalfInvoiceSetup GetOnBehalfInvoiceSetupDetails(int onBehalfInvoiceSetupId);

        /// <summary>
        /// Gets all on behalf invoice setup list.
        /// </summary>
        /// <returns></returns>
        List<OnBehalfInvoiceSetup> GetAllOnBehalfInvoiceSetupList();

        /// <summary>
        /// Gets the on behalf invoice setup list.
        /// </summary>
        /// <param name="billingCategoryId">The billing category id.</param>
        /// <param name="transmitterCode">The transmitter code.</param>
        /// <param name="ChargeCategoryId">The charge category id.</param>
        /// <param name="chargeCodeId">The charge code id.</param>
        /// <returns></returns>
        List<OnBehalfInvoiceSetup> GetOnBehalfInvoiceSetupList(int billingCategoryId, string transmitterCode, int ChargeCategoryId, int chargeCodeId);

        List<OnBehalfInvoiceSetup> GetAllOnBehalfOfMemberList();
    }
}

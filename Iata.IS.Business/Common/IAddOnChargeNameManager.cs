using System.Collections.Generic;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Business.Common
{
    public interface IAddOnChargeNameManager
    {
        /// <summary>
        /// Adds the name of the add on charge.
        /// </summary>
        /// <param name="addOnChargeName">Name of the add on charge.</param>
        /// <returns></returns>
        AddOnChargeName AddAddOnChargeName(AddOnChargeName addOnChargeName);

        /// <summary>
        /// Updates the name of the add on charge.
        /// </summary>
        /// <param name="addOnChargeName">Name of the add on charge.</param>
        /// <returns></returns>
        AddOnChargeName UpdateAddOnChargeName(AddOnChargeName addOnChargeName);

        /// <summary>
        /// Deletes the name of the add on charge.
        /// </summary>
        /// <param name="addOnChargeNameId">The add on charge name id.</param>
        /// <returns></returns>
        bool DeleteAddOnChargeName(int addOnChargeNameId);

        /// <summary>
        /// Gets the add on charge name details.
        /// </summary>
        /// <param name="addOnChargeNameId">The add on charge name id.</param>
        /// <returns></returns>
        AddOnChargeName GetAddOnChargeNameDetails(int addOnChargeNameId);

        /// <summary>
        /// Gets all add on charge name list.
        /// </summary>
        /// <returns></returns>
        List<AddOnChargeName> GetAllAddOnChargeNameList();

        /// <summary>
        /// Gets the add on charge name list.
        /// </summary>
        /// <param name="addOnChargeName">Name of the add on charge.</param>
        /// <param name="billingCategoryId">The billing category id.</param>
        List<AddOnChargeName> GetAddOnChargeNameList(string addOnChargeName, int billingCategoryId);

    }
}

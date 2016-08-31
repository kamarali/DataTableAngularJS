using System.Collections.Generic;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Business.Common
{
    public interface IChargeCategoryManager
    {
        /// <summary>
        /// Adds the charge category.
        /// </summary>
        /// <param name="chargeCategory">The charge category.</param>
        /// <returns></returns>
        ChargeCategory AddChargeCategory(ChargeCategory chargeCategory);

        /// <summary>
        /// Updates the charge category.
        /// </summary>
        /// <param name="chargeCategory">The charge category.</param>
        /// <returns></returns>
        ChargeCategory UpdateChargeCategory(ChargeCategory chargeCategory);

        /// <summary>
        /// Deletes the charge category.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        bool DeleteChargeCategory(int id);

        /// <summary>
        /// Gets the charge category details.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        ChargeCategory GetChargeCategoryDetails(int id);

        /// <summary>
        /// Gets all charge category list.
        /// </summary>
        /// <returns></returns>
        List<ChargeCategory> GetAllChargeCategoryList();

        /// <summary>
        /// Gets the charge category list.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="billingCategoryId">The billing category id.</param>
        /// <returns></returns>
        List<ChargeCategory> GetChargeCategoryList(string name, int billingCategoryId);
    }
}

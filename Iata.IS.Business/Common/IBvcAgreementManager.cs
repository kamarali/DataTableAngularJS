using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Pax;

namespace Iata.IS.Business.Common
{
    public interface IBvcAgreementManager
    {
        /// <summary>
        /// Adds the Bvc Agreement.
        /// </summary>
        /// <param name="bvcAgreement">Bvc Agreement.</param>
        /// <returns></returns>
        BvcAgreement AddBVCAgreement(BvcAgreement bvcAgreement);

        /// <summary>
        /// Updates the Bvc Agreement.
        /// </summary>
        /// <param name="bvcAgreement">The Bvc Agreement.</param>
        /// <returns></returns>
        BvcAgreement UpdateBVCAgreement(BvcAgreement bvcAgreement);

        /// <summary>
        /// Deletes the Bvc Agreement.
        /// </summary>
        /// <param name="id">The Bvc agreement mapping id.</param>
        /// <returns></returns>
        bool ActiveDeactiveBVCAgreement(string id);

        /// <summary>
        /// Gets Bvc agreement.
        /// </summary>
        /// <param name="id">The mapping id.</param>
        /// <returns></returns>
        BvcAgreement GetBVCAgreementDetails(string id);

        /// <summary>
        /// Gets all Bvc agreements.
        /// </summary>
        /// <returns></returns>
        List<BvcAgreement> GetAllBVCAgreementList();

        /// <summary>
        /// Gets the Bvc Agreements list.
        /// </summary>
        /// <param name="billingMemberId">The Billing Member.</param>
        /// <param name="billedMemberId">The Billed Member.</param>
        /// <returns></returns>
        List<BvcAgreement> GetBVCAgreementList(Int32 billingMemberId, Int32 billedMemberId);
    }
}

using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;
namespace Iata.IS.Model.Cargo
{
    [Serializable]
    public class CgoVatIdentifier : MasterBase<int>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the identifier code.
        /// </summary>
        /// <value>The identifier code.</value>
        public string IdentifierCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is oc applicable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is oc applicable; otherwise, <c>false</c>.
        /// </value>
        public bool IsOcApplicable { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        //public string BillingCategory
        //{
        //    get
        //    {
        //        return ((BillingCategoryType)BillingCategoryCode).ToString();
        //    }
        //}
    }
}

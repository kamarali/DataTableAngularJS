using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.Enums;
namespace Iata.IS.Model.Common
{
    [Serializable]
    public class OnBehalfInvoiceSetup : MasterBase<int>
    {

        /// <summary>
        /// Gets or sets the billing category id.
        /// </summary>
        /// <value>
        /// The billing category id.
        /// </value>
        public int BillingCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the transmitter code.
        /// </summary>
        /// <value>
        /// The transmitter code.
        /// </value>
        public string TransmitterCode { get; set; }

        /// <summary>
        /// Gets or sets the charge category id.
        /// </summary>
        /// <value>
        /// The charge category id.
        /// </value>
        public int ChargeCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the charge code id.
        /// </summary>
        /// <value>
        /// The charge code id.
        /// </value>
        public int ChargeCodeId { get; set; }

        /// <summary>
        /// Gets or sets the charge category.
        /// </summary>
        /// <value>
        /// The charge category.
        /// </value>
        public ChargeCategory ChargeCategory { get; set; }

        /// <summary>
        /// Gets the name of the charge category.
        /// </summary>
        /// <value>
        /// The name of the charge category.
        /// </value>
        public string ChargeCategoryName { get { return this.ChargeCategory.Name; } }

        /// <summary>
        /// Gets or sets the charge code.
        /// </summary>
        /// <value>
        /// The charge code.
        /// </value>
        public ChargeCode ChargeCode { get; set; }

        /// <summary>
        /// Gets the name of the charge code.
        /// </summary>
        /// <value>
        /// The name of the charge code.
        /// </value>
        public string ChargeCodeName { get { return this.ChargeCode.Name; } }

        public string BillingCategory
        {
            get
            {
              return BillingCategoryId == 4 ? ((BillingCategoryType)BillingCategoryId).ToString().ToUpper() : ((BillingCategoryType)BillingCategoryId).ToString();
            }
        }
    }
}

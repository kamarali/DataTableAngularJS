using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Model.Common
{
    [Serializable]
    public class CgoRMReasonAcceptableDiff : MasterBase<int>
    {
        /// <summary>
        /// Gets or sets the reason code id.
        /// </summary>
        /// <value>
        /// The reason code id.
        /// </value>
        public int ReasonCodeId { get; set; }

        /// <summary>
        /// Gets or sets the effective from.
        /// </summary>
        /// <value>
        /// The effective from.
        /// </value>
        public string EffectiveFrom { get; set; }

        /// <summary>
        /// Gets or sets the effective to.
        /// </summary>
        /// <value>
        /// The effective to.
        /// </value>
        public string EffectiveTo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [weight charges amount].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [weight charges amount]; otherwise, <c>false</c>.
        /// </value>
        public bool WeightChargesAmount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [valuation charges amount].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [valuation charges amount]; otherwise, <c>false</c>.
        /// </value>
        public bool ValuationChargesAmount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [isc amount].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [isc amount]; otherwise, <c>false</c>.
        /// </value>
        public bool IscAmount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [oc amount].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [oc amount]; otherwise, <c>false</c>.
        /// </value>
        public bool OcAmount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [vat amount].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [vat amount]; otherwise, <c>false</c>.
        /// </value>
        public bool VatAmount { get; set; }

        public ReasonCode ReasonCode { get; set; }

         public string ReasonCodeName
         {
            get { return ReasonCode!=null?this.ReasonCode.Code:string.Empty; }
         }
         
         private int _transactionTypeId;
         
         public int TransactionTypeId
         {
             get { return ReasonCode != null ? ReasonCode.TransactionTypeId : _transactionTypeId; }
             set { _transactionTypeId = value; }
         }
         

    }
}

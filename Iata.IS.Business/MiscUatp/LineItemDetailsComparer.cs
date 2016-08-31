using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.MiscUatp;
namespace Iata.IS.Business.MiscUatp
{
    /// <summary>
    /// CMP:539 add new comparator to check the duplicate lineitemdetails within line item
    /// </summary>
    class LineItemDetailsComparer : IEqualityComparer<LineItemDetail>
    {
        /// <summary>
        /// Gets the equal line item details.
        /// </summary>
        /// <value>The equal line item details.</value>
        public List<LineItemDetail> EqualLineItemDetails
        {
            get
            {
                return (_equalLineItemDetails.GroupBy(rec => rec.DetailNumber).Distinct().ToList()).Select(g => g.ElementAt(0)).ToList();

            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemDetailsComparer"/> class.
        /// </summary>
        public LineItemDetailsComparer()
        {
            _equalLineItemDetails = new List<LineItemDetail>();
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly List<LineItemDetail> _equalLineItemDetails;

        /// <summary>
        /// Equals the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public bool Equals(LineItemDetail x, LineItemDetail y)
        {
            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            bool isEqual = x.LineItemNumber == y.LineItemNumber && x.DetailNumber == y.DetailNumber;
            if (isEqual)
            {
                _equalLineItemDetails.Add(x);
            }

            return isEqual;
        }


        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="lineItemDetailRecord">The coupon record.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(LineItemDetail lineItemDetailRecord)
        {
            //Check whether the object is null
            if (ReferenceEquals(lineItemDetailRecord, null)) return 0;

            //Get hash code for the BatchSequenceNumber field if it is not null.
            int hashBatchSequenceNumber = lineItemDetailRecord.DetailNumber.GetHashCode();

            /*//Get hash code for the RecordSequenceWithinBatch field.
            int hashRecordSequenceWithinBatch = lineItemDetailRecord.RecordSequenceWithinBatch.GetHashCode();

            //Calculate the hash code for the CouponRecord.*/
            return hashBatchSequenceNumber;
        }

    }
}

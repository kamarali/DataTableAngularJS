using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Common
{
    /*CMP#674-Validation of Coupon and AWB Breakdowns in Rejections
   * Desc: Model to map CMP#674 validation cursor returned from SP. It has details about RMs failing validation. 
   * Field in this model will be then used for constructing error description. 
   * This model is very much closed to GTT GTT_RM_CPN_DROPPED. 
   * It is common for both Pax and Cgo billing categories. */
    public class InvalidRejectionMemoDetails
    {
        public string RejectionMemoNumber { get; set; }
        public int BatchNumber { get; set; }
        public int SequenceNumber { get; set; }
        public string TicketIssuingAirline { get; set; }
        public long TicketDocOrAwbNumber { get; set; }
        public int CouponNumber { get; set; }
        public int RejectedRMOccurrence { get; set; }
        public int RejectingRMOccurrence { get; set; }
    }
}

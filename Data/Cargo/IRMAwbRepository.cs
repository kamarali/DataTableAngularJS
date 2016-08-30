using System;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Cargo.Common;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Data.Cargo
{
  public interface IRMAwbRepository : IRepository<RMAwb>
  {

    /// <summary>
    /// Gets the RM awb duplicate count.
    /// </summary>
    /// <param name="rejectionStage">The rejection stage.</param>
    /// <param name="awbIssuingAirline">The awb issuing airline.</param>
    /// <param name="carriageFromId">The carriage from id.</param>
    /// <param name="carriageToId">The carriage to id.</param>
    /// <param name="awbSerialNumber">The awb serial number.</param>
    /// <param name="awbIssueDate">The awb issue date.</param>
    /// <param name="billingmemberId">The billingmember id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="awbBillingCode">The billing code.</param>
    /// <returns></returns>
    long GetRMAwbDuplicateCount(int rejectionStage, string awbIssuingAirline, string carriageFromId, string carriageToId,
                                int awbSerialNumber, DateTime? awbIssueDate, int billingmemberId, int billedMemberId,
                                int billingMonth, int billingYear, int awbBillingCode);

    
    /// <summary>
    /// LoadStrategy method overload of Single method
    /// </summary>
    /// <param name="rmCouponId">RMCouponBreakdown Id</param>
    /// <returns>RMCoupon object</returns>
    RMAwb Single(Guid rmCouponId);


    /// <summary>
    /// Gets the RM awb linking details.
    /// </summary>
    /// <param name="issuingAirline">The issuing airline.</param>
    /// <param name="serialNo">The serial no.</param>
    /// <param name="rmId">The rm id.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="awbBillingCode">The awb billing code.</param>
    /// <returns></returns>
    RMLinkedAwbDetails GetRMAwbLinkingDetails(string issuingAirline, int serialNo, Guid rmId, int billingMemberId, int billedMemberId, int awbBillingCode);

    /// <summary>
    /// Get linking details for rejection memo Coupon when multiple records are found in rejected enity then as per user selection fetch data for selected coupon
    /// </summary>
    /// <param name="awbId"></param>
    /// <param name="rmId"></param>
    /// <returns></returns>
    RMLinkedAwbDetails GetLinkedAwbAmountDetails(Guid awbId, Guid rmId);

    RMAwb GetRejectionMemoWithAwb(Guid rmAwbId);
  }
}

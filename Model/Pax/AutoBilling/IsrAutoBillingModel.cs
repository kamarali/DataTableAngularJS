using System;
using System.Collections.Generic;

namespace Iata.IS.Model.Pax.AutoBilling
{
  public class IsrAutoBillingModel
  {
    /// <summary>
    /// Gets or sets the primeCoupon.
    /// Used for record 06,97 and 98
    /// </summary>
    /// <value>The Coupon.</value>
    public List<PrimeCoupon> CouponCollection { get; set; }

    /// <summary>
    /// Gets or sets the Record05.
    /// Used for record 05
    /// </summary> 
    /// <value>The Record5.</value>
    public List<PrimeCouponMarketingDetails> CouponMarketingDetailsCollection { get; set; }

    /// <summary>
    /// Gets or sets the Record50 record.
    /// Used for Record 50.
    /// </summary>
    /// <value>The Record50Request.</value>
    public List<Record50LiftRequest> Record50RequestCollection { get; set; }

    /// <summary>
    /// Gets or sets the primeCoupon.
    /// Used for record 06,97 and 98
    /// </summary>
    /// <value>The Coupon.</value>
    public List<IsrUnrequestedCoupon> IsrUnrequestedCouponCollection { get; set; }

    /// <summary>
    /// To Add the coupons whose status need to be updated.
    /// </summary>
    public List<AutoBillUpdateCouponStatusModel> AutoBillingUpdateCouponCollection { get; set; }

    /// <summary>
    /// To add the invoice Ids whose source code total needs to updated.
    /// </summary>
    public List<AutoBillUpdateSourceCodeTotalModel> AutoBillingUpdateSourCodeCollection { get; set; }

    /// <summary>
    /// Gets or sets the ProrationCouponError record.
    /// Used for REcord 99
    /// </summary>
    /// <value>The ProrationCouponError.</value>
    public ProrationCouponError CouponError { get; set; }

    /// <summary>
    /// To set the PaxInvoice Object
    /// </summary>
    public List<PaxInvoice> PaxAutoBillInvoice { get; set; }

    /// <summary>
    /// Thisis used to set Invoice Id as a parent for Coupon.
    /// </summary>
    public string CouponParentId { get; set; }

    public IsrAutoBillingModel()
    {
      CouponCollection = new List<PrimeCoupon>();
      CouponMarketingDetailsCollection = new List<PrimeCouponMarketingDetails>();
      Record50RequestCollection = new List<Record50LiftRequest>();
      IsrUnrequestedCouponCollection = new List<IsrUnrequestedCoupon>();
      AutoBillingUpdateCouponCollection = new List<AutoBillUpdateCouponStatusModel>();
      AutoBillingUpdateSourCodeCollection = new List<AutoBillUpdateSourceCodeTotalModel>();
      PaxAutoBillInvoice = new List<PaxInvoice>();
    }
  }
}

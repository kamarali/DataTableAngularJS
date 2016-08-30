using System.Collections.Generic;

namespace Iata.IS.Model.Pax.AutoBilling
{
    public class AutoBillingPrimeCoupon : PrimeCoupon
    {
        public List<AutoBillingPrimeCouponTax> AutoBillingTaxBreakdown { get; set; }

        public List<AutoBillingPrimeCouponVat> AutoBillingVatBreakdown { get; set; }

        public List<AutoBillingPrimeCouponAttachment> AutoBillingAttachments { get; set; }

        public int Status { get; set; }

        public AutoBillingPrimeCoupon()
        {
            AutoBillingTaxBreakdown = new List<AutoBillingPrimeCouponTax>();
            AutoBillingVatBreakdown = new List<AutoBillingPrimeCouponVat>();
            AutoBillingAttachments = new List<AutoBillingPrimeCouponAttachment>();
        }
    }
}




namespace Iata.IS.Model.Pax.AutoBilling
{
    /// <summary>
    /// To match with data model to create Entity Framework Context object 
    /// </summary>
    public class AutoBillingPrimeCouponVat : PrimeCouponVat
    {
        public int Status { get; set; }
    }
}

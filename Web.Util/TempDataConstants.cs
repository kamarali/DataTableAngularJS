namespace Iata.IS.Web.Util
{
  public class TempDataConstants
  {
    public const string FromBillingHistory = "FromBillingHistory";
    public const string FromCorrespondence = "FromCorrespondence";
    public const string RejectedRecordIds = "rejectedRecordIds";
    public const string CorrespondenceNumber = "correspondenceNumber";
    public const string MemberNumber = "MemberNumber";
    public const string IsSuperUser = "IsSuperUser";
    public const string PrimeCouponRecord = "PrimeCouponRecord";
    public const string SamplingFormDRecord = "SamplingFormDRecord";

    //Cargo related tempdata constants
    public const string AwbPrepaidRecord = "AwbPrepaidRecord";
    public const string AwbChargeCollectRecord = "AwbChargeCollectRecord";

    /* CMP #624: ICH Rewrite-New SMI X
    * Description: Preserve invoice SMI as it will be useful on later stages for invoice header validation on server side 
    * Refer FRS Section 2.14 Change #9 */
    public const string PreviousInvoiceSMI = "PreviousInvoiceSMI";

  }
}

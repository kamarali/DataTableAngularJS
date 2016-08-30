using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Cargo.Base
{
  public class AWBBase : EntityBase<Guid>
  {
    public int BdSerialNumber { get; set; }

    public DateTime? AwbDate { get; set; }

    public string AwbIssueingAirline { get; set; }

    public int AwbSerialNumber { get; set; }

    /// <summary>
    /// Property to be displayed in listing grid.
    /// </summary>
    public string AwbSerialNumberCheckDigit
    {
      get { return string.Format("{0}{1}", (AwbSerialNumber > 0 ? Convert.ToString(AwbSerialNumber).PadLeft(7,'0') : string.Empty), Convert.ToString(AwbCheckDigit) ); }
    }

    public int AwbCheckDigit { get; set; }

    public int AwbBillingCode { get; set; }

    public string AirlineOwnUse { get; set; }

    public bool AttachmentIndicatorOriginal { get; set; }

    public bool? AttachmentIndicatorValidated { get; set; }

    public int? NumberOfAttachments { get; set; }

    public string ISValidationFlag { get; set; }

    public string ReasonCode { get; set; }

    public string FilingReference { get; set; }

    public string ConsignmentOriginId { get; set; }

    public string ConsignmentDestinationId { get; set; }

    public string CarriageFromId { get; set; }

    public string CarriageToId { get; set; }

    public DateTime? TransferDate { get; set; }

    public string ReferenceField1 { get; set; }

    public string ReferenceField2 { get; set; }

    public string ReferenceField3 { get; set; }

    public string ReferenceField4 { get; set; }

    public string ReferenceField5 { get; set; }

    /// <summary>
    /// Number of child records required in case of IDEC validations.
    /// </summary>
    public long NumberOfChildRecords { get; set; }

      //PRORATE LADDER HEADER FIELD.
    public string ProrateCalCurrencyId { get; set; }

    public double? TotalProrateAmount { get; set; }

    public string AwbDateDisplayText { get; set; }

  }
}

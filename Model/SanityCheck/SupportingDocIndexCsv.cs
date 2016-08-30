using FileHelpers;

namespace Iata.IS.Model.SanityCheck
{
  [DelimitedRecord(",")]
  public class SupportingDocIndexCsv
  {
    [FieldQuoted('"', QuoteMode.OptionalForRead)]
    [FieldTrim(TrimMode.Both)]
    public string SerialNo;
    [FieldQuoted('"', QuoteMode.OptionalForRead)]
    [FieldTrim(TrimMode.Both)]
    public string BillingEntityCode;
    [FieldQuoted('"', QuoteMode.OptionalForRead)]
    [FieldTrim(TrimMode.Both)]
    public string Month;
    [FieldQuoted('"', QuoteMode.OptionalForRead)]
    [FieldTrim(TrimMode.Both)]
    public string Period;
    [FieldQuoted('"', QuoteMode.OptionalForRead)]
    [FieldTrim(TrimMode.Both)]
    public string BilledCodeEntity;
    [FieldQuoted('"', QuoteMode.OptionalForRead)]
    [FieldTrim(TrimMode.Both)]
    public string BillingCategory;
    [FieldQuoted('"', QuoteMode.OptionalForRead)]
    [FieldTrim(TrimMode.Both)]
    public string InvoiceNo;
    [FieldOptional()]
    [FieldQuoted('"', QuoteMode.OptionalForRead)]
    [FieldTrim(TrimMode.Both)]
    public string BatchNo;
    [FieldOptional()]
    [FieldQuoted('"', QuoteMode.OptionalForRead)]
    [FieldTrim(TrimMode.Both)]
    public string SequenceNo;
    [FieldOptional()]
    [FieldQuoted('"', QuoteMode.OptionalForRead)]
    [FieldTrim(TrimMode.Both)]
    public string BreakDownSrNo;
    [FieldOptional()]
    [FieldQuoted('"', QuoteMode.OptionalForRead)]
    [FieldTrim(TrimMode.Both)]
    public string AttachmentFileName;
    [FieldIgnored]
    public string LineNo;
  }
}

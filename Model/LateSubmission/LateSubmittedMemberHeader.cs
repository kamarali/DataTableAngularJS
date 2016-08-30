namespace Iata.IS.Model.LateSubmission
{
  public class LateSubmissionMemberSummary
  {
    public int MemberId { get; set; }

    public string MemberName { get; set; }

    public int NoOfInvoices { get; set; }

    public decimal PassengerBilling { get; set; }

    public decimal CargoBilling { get; set; }

    public decimal UatpBilling { get; set; }

    public decimal MiscBilling { get; set; }

    public string Currency { get; set; }

    public string FormattedPassengerBilling { get; set; }

    public string FormattedCargoBilling { get; set; }

    public string FormattedUatpBilling { get; set; }

    public string FormattedMiscBilling { get; set; }

  }
}

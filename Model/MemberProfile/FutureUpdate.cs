namespace Iata.IS.Model.MemberProfile
{
  public class FutureUpdate
  {
    public string FieldId;
    public string FieldName;
    public int FieldType = 1; /* 1 = textbox, 2 = checkbox, 3 = date picker, 4 = drop down, 5 = textarea */
    public string CurrentValue;
    public string CurrentDisplayValue;
    public string FutureValue;
    public string FutureDisplayValue;
    public bool HasFuturePeriod;
    public string FuturePeriod;
    public string FutureDate;
    public object HtmlAttributes;
    public bool DisplayEditLink;
    public int? SponsoredById;
    public int? AggregatedById;
    public string EditLinkClass = "futureEditLink";
    public bool IsFieldMandatory;
  }
}
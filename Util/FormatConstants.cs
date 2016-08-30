namespace Iata.IS.Web.Util
{
  public class FormatConstants
  {
    public const string DateFormat = "dd-MMM-yy";
    public const string PeriodFormat = "yyyy-MMM-dd";
    public const string MonthNameFormat = "MMM";
    public const string FullYearFormat = "yyyy";
    public const string GridColumnDateFormat = "{0:dd-MMM-yy}";
    public const string GridColumnDateFullYearFormat = "{0:dd-MMM-yyyy}";
    public const string CorrespondenceNumberFormat = "00000000000";
    public const string ReadOnlyHeaderDateFormat = "dd-MMM-yyyy";
    public const string DateFormatFullYear = "dd-MMM-yyyy";

    // This for is hidden date for converting date string by splitting to day, month and year. 
    public const string HiddenDateFormat = "dd-MM-yyyy";
    /* Format of date coming from IS-XML */
    public const string ISXmlDateFormat = "yyyy-MM-dd";
    public const string DynamicFieldDateTimeFormat = "dd-MMM-yyTHH:mm:ss";
    /* Format of dateTime coming from IS-XML */
    public const string ISXmlDateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
    public const string DynamicFieldDateTimeFormats = "dd-MMM-yy HH:mm:ss"; //"dd-MM-yyyy HH:mm:ss";
    public const string TwoDecimalsFormat = "#,##0.00";
    public const string TwoDecimalsEditFormat = "0.00";
    public const string ThreeDecimalsFormat = "#,##0.000";
    public const string ThreeDecimalsEditFormat = "0.000";
    public const string FourDecimalsFormat = "#,##0.0000";
    public const string FourDecimalsEditFormat = "0.0000";
    public const string ExchangeRateFormat = "#,##0.00000";
    public const string ExchangeRateEditFormat = "0.00000";
    public const string BillingPeriodFormat = "yyyy-M-d";

    public const string GridColumnDateTimeFormat = "{0:dd-MMM-yy HH:mm }";
    public const string GridColumnDateFullYearTimeFormat = "{0:dd-MMM-yyyy HH:mm }";
    //Fixed Spira issue IN:005629, for showing date in yyMMMpp format, where pp is period
    public const string GridColumnyyyMMMpp = "{0:yyyy-MMM-dd}";
    public const string ValidEmailPattern = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";
  }
}

namespace Iata.IS.Web.UIModel.ErrorDetail
{
  public class UIMessageDetail
  {
    public string ErrorCode { get; set; }

    public string Message{ get; set; }

    public bool IsFailed { get; set; }

    public string RedirectUrl { get; set; }

    public bool isRedirect { get; set; }

    public int Id { get; set; }

    public bool LineItemDetailExpected { get; set; }
    
    // Used for Showing Alert
    public bool IsAlert { get; set; }

    public bool IsRecalAlert { get; set; }

    public string AlertMessage { get; set; }

    public string OtherUrl { get; set; }
  }
}
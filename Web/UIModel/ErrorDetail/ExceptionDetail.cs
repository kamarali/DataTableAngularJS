namespace Iata.IS.Web.UIModel.ErrorDetail
{
  /// <summary>
  /// Used to send exception details to UI
  /// </summary>
  public class UIExceptionDetail
  {
    public string Message{ get; set; }

    public bool IsFailed { get; set; }
  }
}
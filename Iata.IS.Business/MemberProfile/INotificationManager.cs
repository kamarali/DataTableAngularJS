namespace Iata.IS.Business.MemberProfile
{
  /// <summary>
  /// 
  /// </summary>
  public struct EmailInfo
  {
    public string To{ get; set;}
    public string From{ get; set;}
    public string CC{ get; set;}
    public string Subject{ get; set;}
    public string Message{get; set;}
    public string Bcc{ get; set;}
  }


  /// <summary>
  /// 
  /// </summary>
  public interface INotificationManager
  {
    /// <summary>
    /// Sends the email.
    /// </summary>
    /// <param name="emailInfo">The email info.</param>
    void SendEmail(EmailInfo emailInfo);
  }
}
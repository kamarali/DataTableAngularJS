namespace Iata.IS.Model.Enums
{
  public enum SupportingAttachmentStatus
  {
    /// <summary>
    /// Processing is pending.
    /// </summary>
    NotProcessed = 1,

    /// <summary>
    /// Processing.
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Processing successful.
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Required but not requested.
    /// </summary>
    RequiredButNotRequested = 4
  }
}

using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.BroadcastMessages
{
  public class ISSISOpsAlert : EntityBase<Guid>
  {
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    /// <value>
    /// The error message.
    /// </value>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the email address of IS Admin.
    /// </summary>
    /// <value>
    /// The email address of IS Admin.
    /// </value>
    public string EmailAddress { get; set; }

    /// <summary>
    /// Gets or sets the alert date time.
    /// </summary>
    /// <value>
    /// The alert date time.
    /// </value>
    public DateTime AlertDateTime { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is active.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
    /// </value>
    public bool IsActive { get; set; }

  }
}

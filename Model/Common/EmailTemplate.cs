using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  /// <summary>
  /// Represents Entity For Email Template Related Configuration Details
  /// </summary>
  public class EmailTemplate : EntityBase<int>
  {
    /// <summary>
    /// Email Template Name
    /// </summary>
    public string TemplateName { get; set; }

    /// <summary>
    /// Template File Name
    /// </summary>
    public string TemplateFileName { get; set; }

    /// <summary>
    /// Email Subject //All Dynamic Contents Should Be Defined As System Variable // Syntax For System Variable Should be $CONTACT$
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// Template Absolute Path
    /// </summary>
    public string TemplatePath { get; set; }

    /// <summary>
    /// From Email Address
    /// </summary>
    public string FromEmailAddress { get; set; }

    /// <summary>
    /// From Email Address User ID
    /// </summary>
    public string FromUserId { get; set; }

    /// <summary>
    /// From Email Address Password
    /// </summary>
    public string FromPassword { get; set; }

    /// <summary>
    /// Remarks
    /// </summary>
    public string Remarks { get; set; }

    /// <summary>
    /// Is Active
    /// </summary>
    public bool IsActive { get; set; }
  }
}

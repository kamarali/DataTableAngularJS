using System.ComponentModel;

namespace Iata.IS.Core.Configuration
{
  public class UIParameters
  {
    [DisplayName("Default Page Size")]
    public int DefaultPageSize { get; set; }

      [DisplayName("Page Size Options")]
    public string PageSizeOptions { get; set; }
  }
}
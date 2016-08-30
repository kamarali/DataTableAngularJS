using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iata.IS.Web.UIModel.ErrorDetail
{
  public class CustomUiMessageDetail : UIMessageDetail
  {
    public string Value { get; set; }

    public string FutureFieldValue { get; set; }
  }

  public class CustomUiLocationMessageDetail : CustomUiMessageDetail
  {
    public string DisplayCommercialName { get; set; }
  }
}
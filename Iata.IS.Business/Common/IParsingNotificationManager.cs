using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Business.Common
{
  public interface IParsingNotificationManager
  {
    void SendSisAdminAlert(string fileName);
  }
}

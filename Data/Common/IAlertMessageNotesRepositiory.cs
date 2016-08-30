using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;

namespace Iata.IS.Data.Common
{
  public interface IAlertMessageNotesRepositiory
  {
    List<AlertsMessagesAnnouncementsResultSet> GetAlertsMessagesAnnouncements(int type, string memberType, int userId,
                                                                              DateTime thresholdDate);

    void ClearAlertMessage(Guid messageId, int userId);
  }
}

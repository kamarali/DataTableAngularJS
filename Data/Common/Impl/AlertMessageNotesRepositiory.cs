using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;

namespace Iata.IS.Data.Common.Impl
{
  public class AlertMessageNotesRepositiory: Repository<InvoiceBase>, IAlertMessageNotesRepositiory
  {
    public List<AlertsMessagesAnnouncementsResultSet> GetAlertsMessagesAnnouncements(int type, string memberType, int userId, DateTime thresholdDate)
    {
        //SCP 000000  : Handled Elmah Issue "The data reader is incompatible with the specified 'ISDataContext.AlertsMessagesAnnouncementsResultSet'. 
        //A member of the type, 'MessageId', does not have a corresponding column in the data reader with the same name"
        //Description : Even in case of session expired this call was made to get Alert/Message count. Because of expired session user id was passed as 0 to SP.
        //This casued above exception (OR ORA-01403: no data found exception). Hence added below if condition to bypass SP call.
        if (userId > 0)
        {
            var parameters = new ObjectParameter[4];
            parameters[0] = new ObjectParameter(AlertMessageNotesRepositioryConstants.DataType, type);
            parameters[1] = new ObjectParameter(AlertMessageNotesRepositioryConstants.MemberType, memberType);
            parameters[2] = new ObjectParameter(AlertMessageNotesRepositioryConstants.UserId, userId);
            parameters[3] = new ObjectParameter(AlertMessageNotesRepositioryConstants.ThresholdDate, thresholdDate);

            var list =
                ExecuteStoredFunction<AlertsMessagesAnnouncementsResultSet>(
                    AlertMessageNotesRepositioryConstants.GetAlertsMsgsNotes, parameters) as
                IEnumerable<AlertsMessagesAnnouncementsResultSet>;
            return list.ToList();
        }
        else
        {
            return new List<AlertsMessagesAnnouncementsResultSet>();
        }
    }


    public void ClearAlertMessage(Guid messageId, int userId)
    {
      var parameters = new ObjectParameter[2];
      parameters[0] = new ObjectParameter(AlertMessageNotesRepositioryConstants.MessageId, messageId);
      parameters[1] = new ObjectParameter(AlertMessageNotesRepositioryConstants.UsersId, userId);
      ExecuteStoredProcedure(AlertMessageNotesRepositioryConstants.ClearAlertMessage, parameters);
    }
  }
}

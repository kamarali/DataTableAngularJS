using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;

namespace Iata.IS.Data.Common.Impl
{
  public class LogDebugDataRepository : Repository<DebugLog>, ILogDebugDataRepository
  {
    //SCP210204: IS-WEB Outage
     public void InsertLogDebugData(DebugLog log)
     {
       var parameters = new ObjectParameter[7];
       parameters[0] = new ObjectParameter("LOG_DATE_I", typeof(DateTime)) { Value = log.LogDate };
       parameters[1] = new ObjectParameter("LOG_METHOD_NAME_I", typeof(string)) { Value = log.LogMethodName };
       parameters[2] = new ObjectParameter("LOG_CLASS_NAME_I", typeof(string)) { Value = log.LogClassName };
       parameters[3] = new ObjectParameter("LOG_CATEGORY_CODE_I", typeof(string)) { Value = log.LogCategory };
       parameters[4] = new ObjectParameter("LOG_TEXT_I", typeof(string)) { Value = log.LogText };
       parameters[5] = new ObjectParameter("LOG_USER_ID_I", typeof(string)) { Value = log.LogUserId };
       parameters[6] = new ObjectParameter("LOG_REF_ID_I", typeof(string)) { Value = log.LogRefId };
       ExecuteStoredProcedure("InsertLogDebugData", parameters);

     }
  }
}

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.ValueConfirmation;

namespace Iata.IS.Data.ValueConfirmation.Impl
{
  public class ResponseValueConfirmationRepository :Repository<InvoiceBase>,IResponseValueConfirmationRepository
  {
    public void UpdateValueConfirmationData(string couponRecords,Guid fileId)
    {
      var parameters = new ObjectParameter[2];
      parameters[0] = new ObjectParameter(ValueConfirmationConstants.CouponString, typeof(string)) { Value = couponRecords };
      parameters[1] = new ObjectParameter(ValueConfirmationConstants.FileId, typeof(Guid)) { Value = fileId };

      // Execute stored procedure
      ExecuteStoredProcedure(ValueConfirmationConstants.UpdateValueConfirmationRecords, parameters);
    }
  }
}

using System;
using System.Collections.Generic;
using Iata.IS.Model.ValueConfirmation;

namespace Iata.IS.Data.ValueConfirmation
{
  public interface IResponseValueConfirmationRepository
  {
    void UpdateValueConfirmationData(string couponRecords, Guid fileId);
  }
}

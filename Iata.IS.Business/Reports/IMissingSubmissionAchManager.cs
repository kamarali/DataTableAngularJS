using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Business.Reports
{
  public interface IMissingSubmissionAchManager
  {
    void CheckMissingSubmission(bool isSummary);
  }
}

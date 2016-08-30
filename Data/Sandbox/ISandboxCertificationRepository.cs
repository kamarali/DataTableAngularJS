using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.SandBox;

namespace Iata.IS.Data.Sandbox
{
  public interface ISandboxCertificationRepository : IRepository<CertificationParameterMaster>
  {
      IQueryable<CertificationParameterMaster> GetCertificationParameter();
    IList<CertificationTransactionDetailsReport> GetCertificationTransDetails();
  }
}

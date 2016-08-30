using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.SandBox;

namespace Iata.IS.Data.Sandbox.Impl
{
  public class SandboxCertificationRepository : Repository<CertificationParameterMaster>, ISandboxCertificationRepository
  {
    
      public IQueryable<CertificationParameterMaster> GetCertificationParameter()
      {
          var cetificationParamerer = EntityObjectSet.Include("BillingCategoryMaster").Include("FileFormatMaster").Include("TransactionTypeMaster").Include("TransactionGroupMaster");
        return cetificationParamerer;
      }


      public IList<CertificationTransactionDetailsReport> GetCertificationTransDetails()
      {

        var parameters = new ObjectParameter[7];
        parameters[0] = new ObjectParameter("MEMBER_ID_IN", typeof(int)) { Value = null };
        parameters[1] = new ObjectParameter("RECEIVED_ON_FROMDATE_IN", typeof(DateTime)) { Value = DateTime.Now.AddDays(-100) };
        parameters[2] = new ObjectParameter("RECEIVED_ON_TODATE_IN", typeof(DateTime)) { Value = DateTime.Now };
        parameters[3] = new ObjectParameter("FILE_TYPE_IN", typeof(int)) { Value = null };
        parameters[4] = new ObjectParameter("BILLING_CATEGORY_IN", typeof(int)) { Value = null };
        parameters[5] = new ObjectParameter("REQUEST_TYPE_IN", typeof(string)) { Value = null };
        parameters[6] = new ObjectParameter("TRANS_GROUP_ID_IN", typeof(int)) { Value = null };

        var certificationTransactionDetailse = ExecuteStoredFunction<CertificationTransactionDetailsReport>("GetCertTransDetails", parameters);

        return certificationTransactionDetailse.ToList();

      }


  }
}

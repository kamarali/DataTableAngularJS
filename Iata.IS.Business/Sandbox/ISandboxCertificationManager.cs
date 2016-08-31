using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.SandBox;

namespace Iata.IS.Business.Sandbox
{
 public interface ISandboxCertificationManager
  {
   // Get Certification Parameter Master by Billing Category and Transaction type

   CertificationParameterMaster GetCertificationParameters(BillingCategoryType billingCategoryType, TransactionType transactionType);
   CertificationParameterMaster GetCertificationParameters(BillingCategoryType billingCategoryType, TransactionType transactionType,FileFormatType fileFormatType);

   List<CertificationParameterMaster> GetCertificationParameters(BillingCategoryType billingCategoryType,
                                                                        TransactionGroup transactionGroup,
                                                                        FileFormatType fileFormatType);
   List<CertificationParameterMaster> GetAllRecord();
   List<CertificationParameterMaster> GetSandBoxList(int billingCategoryId, int FileFormatId, int TransactionTypeId);
   CertificationParameterMaster UpdateSandBox(CertificationParameterMaster certificationParameterMaster);

   //List<CertificationTransactionDetails> GetAllTransactionDetails();
  }
}

using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;

namespace Iata.IS.Data.Reports.UatpAtcan.Impl
{
    public class UatpAtcanDetailsRepository : Repository<InvoiceBase>, IUatpAtcanDetailsRepository
    {
       public List<UatpAtcanDetails> GetUatpAtcanDetails(int memberId, int period, int billingMonth, int billingYear, int billingTypeId)
       {
           var parameters = new ObjectParameter[5];
           parameters[0] = new ObjectParameter(UatpAtcanConstants.UatpAtcanMemberIdPara, typeof(int)) { Value = memberId };
           parameters[1] = new ObjectParameter(UatpAtcanConstants.UatpAtcanPerionNoPara, typeof(int)) { Value = period };
           parameters[2] = new ObjectParameter(UatpAtcanConstants.UatpAtcanBillingMonthPara, typeof(int)) { Value = billingMonth };
           parameters[3] = new ObjectParameter(UatpAtcanConstants.UatpAtcanYearPara, typeof(int)) { Value = billingYear };
           parameters[4] = new ObjectParameter(UatpAtcanConstants.UatpAtcanBillingTypeIdPara, typeof(int)) { Value = billingTypeId };
           var uatpAtcanDetailsList = ExecuteStoredFunction<UatpAtcanDetails>(UatpAtcanConstants.GetUatpAtcanDetailsFunction, parameters);
           return uatpAtcanDetailsList.ToList();
       }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile.Impl
{
  class ContactTypeRepository : Repository<ContactType>, IContactTypeRepository
  {
    public override IQueryable<ContactType> GetAll()
    {
      var contactTypeList = EntityObjectSet.Include("ContactTypeGroup").Include("ContactTypeSubGroup");

      return contactTypeList;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int[] GetMaxContactTypeIdAndMaxSeqNum()
    {
      var maxValues = new int[2];
      var parameters = new ObjectParameter[2];
      parameters[0] = new ObjectParameter(ContactTypDetailsConstants.ContactTpeId, typeof(int));
      parameters[1] = new ObjectParameter(ContactTypDetailsConstants.SequenceNumber, typeof(int));
      ExecuteStoredProcedure(ContactTypDetailsConstants.GetMaxContactTypeIdAndMaxSeqNum, parameters);
      maxValues[0] = int.Parse(parameters[0].Value.ToString());
      maxValues[1] = int.Parse(parameters[1].Value.ToString());
      return maxValues;
    }
  }
}

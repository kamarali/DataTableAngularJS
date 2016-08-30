using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.MemberProfileDataFile
{
  public class MemberProfileIntermediateDataModel
  {
    public int MemberId { get; set; }
    public string AccountId { get; set; }
    public List<string> ContactsList = new List<string>();

    public MemberProfileIntermediateDataModel()
    {
      ContactsList = new List<string>();
    }

  }
}

using System.Collections.Generic;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Business.MemberProfile
{
  public interface IMemberLocationCsvGenerator
  {
    char Separator { get; set; }
    string FilePath { get; set; }

    bool CreateCsvFile(List<Location> list);
  }
}


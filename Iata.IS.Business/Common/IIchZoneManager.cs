using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
    public interface IIchZoneManager
    {
        IchZone AddIchZone(IchZone ichZone);

        IchZone UpdateIchZone(IchZone ichZone);

        bool DeleteIchZone(int ichZoneId);

        IchZone GetIchZoneDetails(int ichZoneId);

        List<IchZone> GetAllIchZoneList();

        List<IchZone> GetIchZoneList(string Zone, string ClearanceCurrency, string Description);
    }
}

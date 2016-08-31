using System.Collections.Generic;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Business.Common
{
    public interface IAircraftTypeManager
    {
        AircraftType AddAircraftType(AircraftType aircraftType);

        AircraftType UpdateAircraftType(AircraftType aircraftType);

        bool DeleteAircraftType(string aircraftTypeId);

        AircraftType GetAircraftTypeDetails(string aircraftTypeId);

        List<AircraftType> GetAllAircraftTypeList();

        List<AircraftType> GetAircraftTypeList(string id, string description);
    }
}

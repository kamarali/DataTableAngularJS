using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Enums
{ 
  //CMP#588: Misc Customized Listing. 
  public enum MUChargeCodes
  {
    AirportFee = 8,
    AirportLighting = 9,
    AirportMisc = 10,
    AirportParking = 11,
    AirportPaxServices = 12,
    AirportRunwayCharges = 13,
    AirportSecurity = 14,
    AirportUtilities = 101,
    //CMP#588: MISC Customized Listings: Layout Definition for MLD6.
    AtcApproach = 1,
    AtcCommunication = 2,
    AtcEnRoute = 3,
    AtcMeteorology = 4,
    AtcMisc = 5,
    AtcOceanic = 6,
    AtcOverflight =7,
    //CMP#588_phase2: Misc Customized listing.
    GrdHndPaxTrans = 49,
    GrdHndCrewAccommodation = 37,
    GrdHndRampHandling =  51,
    GrdHndRentEquipment = 52,
    GrdHndMotorFuel = 47,
    GrdHndMisHandlingPax = 92,
    GrdHndBaggageDelivery = 32,
    GrdHndMishandlingBaggage= 45,
    //CMP#679: MISC Customized Listings: Layout Definition for MLD7.
    GroundHandlingCrewTransportation = 38,
    GroundHandlingBaggage = 31,
    GroundHandlingCargoHandling = 33,
    GroundHandlingCleaning = 35,
    GroundHandlingCustomsServiceCharge = 39,
    GroundHandlingDeicing = 40,
    GroundHandlingMisc = 44,
    GroundHandlingPassengerHandling = 48,
    GroundHandlingPassengerSecurity = 50,
    GroundHandlingDepartureStamps = 41,
    GroundHandlingImmigrationFines = 42,
    GroundHandlingLounge = 43,
    GroundHandlingStpc = 54,
    GroundHandlingCatering = 34,
    //CMP#679: MISC Customized Listings: Layout Definition for MLD15.
    GroundHandlingStand = 53,
    GroundHandlingUtilities = 103,
    GroundHandlingCommission = 36
  }
}

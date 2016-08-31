using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Business.Pax
{
  public interface IMemberManager
  {
    /// <summary>
    /// This method will get locations corresponding to member ID passed
    /// </summary>
    /// <param name="memberId">ID of airline member</param>
    /// <returns></returns>
    List<Location> GetMemberLocations(int memberId);

    /// <summary>
    /// This method returns list of all airline members which will be used in populating issuing airline 
    /// //and airline flight designator codes on 'Add Coupon' record screen
    /// </summary>
    /// <returns></returns>
    List<Member> GetMemberList();

    /// <summary>
    /// This will return the list of all the Airport Codes
    /// </summary>
    /// <returns></returns>
    List<Airport> GetAirportCodes();

    /// <summary>
    /// This method will get the details of the Member corresponding to its memberId provided.
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns>Member Object if memberId matches member in database</returns>
    Member GetMemberDetails(int memberId);

    /// <summary>
    /// This method will retrieve the Invoice Member location information.
    /// </summary>
    /// <param name="locationId"></param>
    /// <returns>MemberLocationDetails</returns>
    MemberLocationInformation GetInvoiceMemberLocationDetails(string locationId);
    
    /// <summary>
    /// This method will retrieve the Invoice Member allowed file extentions.
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    List<string> GetAllowedFileExtentions();

    string GetFileDirectoryPath(string type);
    string GetFileModifiedName(int attachmentId);
    bool CheckFileDuplicateStatus(string fileName);
  }
}

using System.Collections.Generic;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.Master;

namespace Iata.IS.Business.Common
{
  public interface IChargeCodeTypeManager
  {
    /// <summary>
    /// Gets all the Charge Code Types from the data base
    /// </summary>
    /// <returns>List of Charge Code Type</returns>
    List<ChargeCodeType> GetAllChargeCodeTypeList();

    /// <summary>
    /// This function is used to get misc list of charge code type based on criteria. 
    /// </summary>
    /// <param name="ChargeCategoryId"></param>
    /// <param name="ChargeCodeId"></param>
    /// <param name="chargeCodeTypeName"></param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    List<ChargeCodeTypeSearchData> GetMiscChargeCodeType(int ChargeCategoryId, int ChargeCodeId, string chargeCodeTypeName);

    /// <summary>
    /// This function is used to add chargeCodeType values.
    /// </summary>
    /// <param name="chargeCodeType"></param>
    /// <param name="userId"></param>
    //CMP #636: Standard Update Mobilization
    void AddChargeCodeType(ChargeCodeType chargeCodeType, int userId);

    /// <summary>
    /// This function is used to update charge code type data
    /// </summary>
    /// <param name="chargeCode"></param>
    /// <param name="userId"></param>
    //CMP #636: Standard Update Mobilization
    void UpdateChargeCodeType(ChargeCodeType chargeCodeType, int userId);

    /// <summary>
    /// Deletes the charge code type.
    /// </summary>
    /// <param name="chargeCodeTypeId">The charge code type id.</param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    void DeleteChargeCodeType(int chargeCodeTypeId, int userId);
    
    /// <summary>
    /// This function is used for get charge code type details based on id.
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    ChargeCodeType GetChargeCodeTypeDetails(int Id);
  }
}

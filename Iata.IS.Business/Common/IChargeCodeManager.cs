using System.Collections.Generic;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.Master;

namespace Iata.IS.Business.Common
{
  public interface IChargeCodeManager
  {
    /// <summary>
    /// Adds the charge code.
    /// </summary>
    /// <param name="chargeCode">The charge code.</param>
    /// <returns></returns>
    void AddChargeCode(ChargeCode chargeCode, int userId);

    /// <summary>
    /// Updates the charge code.
    /// </summary>
    /// <param name="chargeCode">The charge code.</param>
    /// <returns></returns>
    void UpdateChargeCode(ChargeCode chargeCode, int userId);

    /// <summary>
    /// Deletes the charge code.
    /// </summary>
    /// <param name="chargeCodeId">The charge code id.</param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    void DeleteChargeCode(int chargeCodeId, int userId);

    /// <summary>
    /// Gets the charge code details.
    /// </summary>
    /// <param name="chargeCodeId">The charge code id.</param>
    /// <returns></returns>
    ChargeCode GetChargeCodeDetails(int chargeCodeId);

    /// <summary>
    /// Gets all charge code list.
    /// </summary>
    /// <returns></returns>
    List<ChargeCode> GetAllChargeCodeList();

    /// <summary>
    /// Gets the charge code list.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="chargeCategoryId">The charge category id.</param>
    /// <returns></returns>
    List<ChargeCode> GetChargeCodeList(string name, int chargeCategoryId);

    /// <summary>
    /// This function is used to get misc charge code based on charge category and charge code.
    /// </summary>
    /// <param name="chargeCategoryId"></param>
    /// <param name="chargeCodeId"></param>
    /// <returns></returns>
    // CMP #636: Standard Update Mobilization
    List<ChargeCodeSearchData> GetMiscChargeCode(int chargeCategoryId, int chargeCodeId);
  }
}

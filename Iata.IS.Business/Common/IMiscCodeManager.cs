using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
  public interface IMiscCodeManager
  {
    /// <summary>
    /// Adds the misc code.
    /// </summary>
    /// <param name="miscCode">The misc code.</param>
    /// <returns></returns>
    MiscCode AddMiscCode(MiscCode miscCode);

    /// <summary>
    /// Updates the misc code.
    /// </summary>
    /// <param name="miscCode">The misc code.</param>
    /// <returns></returns>
    MiscCode UpdateMiscCode(MiscCode miscCode);

    /// <summary>
    /// Deletes the misc code.
    /// </summary>
    /// <param name="miscCodeId">The misc code id.</param>
    /// <returns></returns>
    bool DeleteMiscCode(int miscCodeId);

    /// <summary>
    /// Gets the misc code details.
    /// </summary>
    /// <param name="miscCodeId">The misc code id.</param>
    /// <returns></returns>
    MiscCode GetMiscCodeDetails(int miscCodeId);

    /// <summary>
    /// Gets all misc code list.
    /// </summary>
    /// <returns></returns>
    List<MiscCode> GetAllMiscCodeList();

    /// <summary>
    /// Gets the misc code list.
    /// </summary>
    /// <param name="GroupId">The group id.</param>
    /// <param name="Name">The name.</param>
    /// <param name="Description">The description.</param>
    /// <returns></returns>
    List<MiscCode> GetMiscCodeList(int GroupId, string Name, string Description);
  }
}

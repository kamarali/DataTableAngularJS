using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;
namespace Iata.IS.Business.Common
{
    public interface IUnlocCodeManager
    {
        /// <summary>
        /// Adds the unloc code.
        /// </summary>
        /// <param name="unlocCode">The unloc code.</param>
        /// <returns></returns>
        UnlocCode AddUnlocCode(UnlocCode unlocCode);

        /// <summary>
        /// Updates the unloc code.
        /// </summary>
        /// <param name="unlocCode">The unloc code.</param>
        /// <returns></returns>
        UnlocCode UpdateUnlocCode(UnlocCode unlocCode);

        /// <summary>
        /// Deletes the unloc code.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        bool DeleteUnlocCode(string Id);

        /// <summary>
        /// Gets the unloc code details.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        UnlocCode GetUnlocCodeDetails(string Id);

        /// <summary>
        /// Gets all unloc code list.
        /// </summary>
        /// <returns></returns>
        List<UnlocCode> GetAllUnlocCodeList();
        
        /// <summary>
        /// Check if Unloc code exists in database.
        /// </summary>
        /// <param name="unlocCode"></param>
        /// <returns></returns>
        bool IsValidUnlocCode(string unlocCode);

        /// <summary>
        /// Gets the unloc code list.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        List<UnlocCode> GetUnlocCodeList(string Id, string Name);
    }
}

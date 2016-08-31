using System.Collections.Generic;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Business.Common
{
    public interface IUomCodeManager
    {
        /// <summary>
        /// Adds the uom code.
        /// </summary>
        /// <param name="uomCode">The uom code.</param>
        /// <returns></returns>
        UomCode AddUomCode(UomCode uomCode);

        /// <summary>
        /// Updates the uom code.
        /// </summary>
        /// <param name="uomCode">The uom code.</param>
        /// <returns></returns>
        UomCode UpdateUomCode(UomCode uomCode);

        /// <summary>
        /// Deletes the uom code.
        /// </summary>
        /// <param name="uomCodeId">The uom code id.</param>
        /// <returns></returns>
        bool DeleteUomCode(string uomCodeId);

        /// <summary>
        /// Gets the uom code details.
        /// </summary>
        /// <param name="uomCodeId">The uom code id.</param>
        /// <returns></returns>
        UomCode GetUomCodeDetails(string uomCodeId);

        /// <summary>
        /// Gets all uom code list.
        /// </summary>
        /// <returns></returns>
        List<UomCode> GetAllUomCodeList();

        /// <summary>
        /// Gets the uom code list.
        /// </summary>
        /// <param name="Description">The description.</param>
        /// <returns></returns>
        List<UomCode> GetUomCodeList(string uomCodeId,int type, string Description);
    }
}

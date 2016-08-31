using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
    public interface ISisMemberSubStatusManager
    {

        /// <summary>
        /// Adds the sis member sub status.
        /// </summary>
        /// <param name="sisMemberSubStatus">The sis member sub status.</param>
        /// <returns></returns>
        SisMemberSubStatus AddSisMemberSubStatus(SisMemberSubStatus sisMemberSubStatus);

        /// <summary>
        /// Updates the sis member sub status.
        /// </summary>
        /// <param name="sisMemberSubStatus">The sis member sub status.</param>
        /// <returns></returns>
        SisMemberSubStatus UpdateSisMemberSubStatus(SisMemberSubStatus sisMemberSubStatus);

        /// <summary>
        /// Deletes the sis member sub status.
        /// </summary>
        /// <param name="sisMemberSubStatusId">The sis member sub status id.</param>
        /// <returns></returns>
        bool DeleteSisMemberSubStatus(int sisMemberSubStatusId);

        /// <summary>
        /// Gets the sis member sub status details.
        /// </summary>
        /// <param name="sisMemberSubStatusId">The sis member sub status id.</param>
        /// <returns></returns>
        SisMemberSubStatus GetSisMemberSubStatusDetails(int sisMemberSubStatusId);

        /// <summary>
        /// Gets all sis member sub status list.
        /// </summary>
        /// <returns></returns>
        List<SisMemberSubStatus> GetAllSisMemberSubStatusList();

        /// <summary>
        /// Gets the sis member sub status list.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        List<SisMemberSubStatus> GetSisMemberSubStatusList(string description);

        /// <summary>
        /// Check the Member Sub status existance in Member Detail table
        /// </summary>
        /// <param name="sisMemberSubStatusId"></param>
        /// <returns>true/false</returns>
      bool IsSubStatusExistanceInMemberProfile(int sisMemberSubStatusId);

      /// <summary>
      /// Check for duplicate entry
      /// </summary>
      /// <param name="sisMemberSubStatusDesc"></param>
      /// <returns></returns>
      bool CheckSubStatusDuplication(string sisMemberSubStatusDesc);

    }
}

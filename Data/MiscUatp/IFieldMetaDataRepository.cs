using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Data.MiscUatp
{
  public interface IFieldMetaDataRepository : IRepository<FieldMetaData>
  {
    //CMP #636: Standard Update Mobilization
    //Get field meta data based on charge code id, charge code type and billing category.
    List<FieldMetaData> GetFieldMetadata(int chargeCodeId, Nullable<int> chargeCodeTypeId, Guid? lineItemDetailId, Int32 billingCategoryId);

    /// <summary>
    /// Gets the field metadata for group.
    /// </summary>
    /// <param name="chargeCodeId">The charge code id.</param>
    /// <param name="chargeCodeTypeId">The charge code type id.</param>
    /// <param name="groupId">The group id.</param>
    /// <returns></returns>
    FieldMetaData GetFieldMetadataForGroup(int chargeCodeId, int? chargeCodeTypeId, Guid groupId);

     /// <summary>
    /// Fetch data for optional groups to populate optional field dropdown
    /// </summary>
    /// <param name="chargeCodeId"></param>
    /// <param name="chargeCodeTypeId"></param>
    /// <returns></returns>
    List<DynamicGroupDetail> GetOptionalGroupMetadata(int chargeCodeId, Nullable<int> chargeCodeTypeId);

    /// <summary>
    /// Return metadata for optional group
    /// </summary>
    /// <param name="chargeCodeId"></param>
    /// <param name="chargeCodeTypeId"></param>
    /// <param name="groupId"></param>
    /// <returns></returns>
    FieldMetaData GetOptionalFieldMetadataForGroup(int chargeCodeId, Nullable<int> chargeCodeTypeId, Guid groupId);
  }
}

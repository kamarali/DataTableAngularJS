namespace Iata.IS.Data.MiscUatp.Impl
{
  internal static class FieldMetadataRepositoryConstants
  {
    #region Stored Procedure Constants

    public const string ChargeCodeIdParameterName = "Charge_Code_Id_I";
    public const string ChargeCodeTypeIdParameterName = "Charge_Code_Type_Id_I";
    //CMP #636: Standard Update Mobilization
    public const string BillingCategoryIdParameterName = "BILLING_CATEGORY_ID_I";
    public const string LineItemDetailIdParameterName = "Line_Item_Detail_Id_I";
    public const string GroupIdParameterName = "GROUP_ID_I";

    public const string GetOptionalGroupFunctionName = "GetDynamicOptionalGroupList";
    #endregion
  }
}

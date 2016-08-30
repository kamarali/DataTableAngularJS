using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Devart.Data.Oracle;
using Iata.IS.Core;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MiscUatp.Extensions;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.MiscUatp.Enums;
using Microsoft.Data.Extensions;
using MISC = Iata.IS.Data.MiscUatp.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
  class FieldMetaDataRepository : Repository<FieldMetaData>, IFieldMetaDataRepository
  {
    /// <summary>
    /// Fetch field metadata for given combination of ChargeCodeId and ChargeCodeTypeId
    /// </summary>
    /// <param name="chargeCodeId">Charge Code Id</param>
    /// <param name="chargeCodeTypeId">Charge Code Type Id</param>
    /// <param name="lineItemDetailId">Charge Code Type Id</param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    public List<FieldMetaData> GetFieldMetadata(int chargeCodeId, Nullable<int> chargeCodeTypeId, Guid? lineItemDetailId, Int32 billingCategoryId)
    {
      var lineItemParameter = new OracleParameter(FieldMetadataRepositoryConstants.LineItemDetailIdParameterName, lineItemDetailId ?? null);
      lineItemParameter.OracleDbType = OracleDbType.Raw;
      return ExecuteSP(StoredProcedures.GetFieldMetadata //name of stored proc
        , lineItemDetailId != null ? true : false
          , new OracleParameter[] { // INPUT parameters expected by the stored proc
                new OracleParameter(FieldMetadataRepositoryConstants.ChargeCodeIdParameterName, chargeCodeId) 
                ,new OracleParameter(FieldMetadataRepositoryConstants.ChargeCodeTypeIdParameterName, chargeCodeTypeId) 
                , lineItemParameter, new OracleParameter(FieldMetadataRepositoryConstants.BillingCategoryIdParameterName, billingCategoryId) 
            }
          , r => this.FetchRecords(r) //action to be done with the set of results
          );
      //return new List<FieldMetaData>();
    }

    /// <summary>
    /// Gets the field metadata for group.
    /// </summary>
    /// <param name="chargeCodeId">The charge code id.</param>
    /// <param name="chargeCodeTypeId">The charge code type id.</param>
    /// <param name="groupId">The group id.</param>
    /// <returns></returns>
    public FieldMetaData GetFieldMetadataForGroup(int chargeCodeId, int? chargeCodeTypeId, Guid groupId)
    {
      var chargeCodeTypeIdParameter = new OracleParameter(FieldMetadataRepositoryConstants.ChargeCodeTypeIdParameterName, chargeCodeTypeId ?? null);

      var groupIdParameter = new OracleParameter(FieldMetadataRepositoryConstants.GroupIdParameterName, groupId);
      groupIdParameter.OracleDbType = OracleDbType.Raw;

      var groupMetadata =  ExecuteSP(StoredProcedures.GetFieldMetadataForGroup //name of stored proc
        , false // Data values will not be available in this case.
          , new[] { // INPUT parameters expected by the stored proc
                new OracleParameter(FieldMetadataRepositoryConstants.ChargeCodeIdParameterName, chargeCodeId) 
                , chargeCodeTypeIdParameter 
                , groupIdParameter
            }
          , r => this.FetchRecords(r) //action to be done with the set of results
          );
      return groupMetadata != null ? groupMetadata[0] : null;
    }

    /// <summary>
    /// Return metadata for optional group
    /// </summary>
    /// <param name="chargeCodeId"></param>
    /// <param name="chargeCodeTypeId"></param>
    /// <param name="groupId"></param>
    /// <returns></returns>
    public FieldMetaData GetOptionalFieldMetadataForGroup(int chargeCodeId, int? chargeCodeTypeId, Guid groupId)
    {
      var groupIdParameter = new OracleParameter(FieldMetadataRepositoryConstants.GroupIdParameterName, groupId);
      groupIdParameter.OracleDbType = OracleDbType.Raw;

      var groupMetadata = ExecuteSP(StoredProcedures.GetDynamicFieldsForOptionalGroup //name of stored proc
        , false // Data values will not be available in this case.
          , new[] { // INPUT parameters expected by the stored proc
                groupIdParameter
            }
          , r => this.FetchRecords(r) //action to be done with the set of results
          );
      return (groupMetadata != null && groupMetadata.Count > 0) ? groupMetadata[0] : null;
    }

    /// <summary>
    /// Fetch record for Fields
    /// </summary>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    private List<FieldMetaData> FetchRecords(LoadStrategyResult loadStrategyResult)
    {
      var metadata = new List<FieldMetaData>();
      if (loadStrategyResult.IsLoaded(GetDynamicFields.MetadataField))
      {
        metadata = FieldMetaDataRepository.LoadEntities(base.EntityObjectSet, loadStrategyResult, null);
      }

      return metadata;
    }

    /// <summary>
    /// Load entities from cursor
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<FieldMetaData> LoadEntities(ObjectSet<FieldMetaData> objectSet, LoadStrategyResult loadStrategyResult, Action<FieldMetaData> link)
    {
      var groups = new List<FieldMetaData>();
      //Load fieldmetadata for Fields
      var muMaterializers = new MuMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(GetDynamicFields.MetadataGroup))
      {
        // first result set includes group metadata
        groups = muMaterializers.FieldMetaDataMaterializer.Materialize(reader).Bind(objectSet).ToList();
if (!reader.IsClosed)
        reader.Close();
      }

      var fields = new List<FieldMetaData>();
      if (loadStrategyResult.IsLoaded(GetDynamicFields.MetadataField) && groups.Count != 0)
        fields = FieldMetaDataRepository.LoadChildEntities(objectSet, loadStrategyResult, null);


      groups = groups.Where(g => g.Level == 1).ToList();

      groups.AddRange(fields.Where(f => f.ParentId == new Guid()));
      groups.OrderBy(d => d.DisplayOrder).ToList();

      SortFieldsInGroup(ref groups);
      return groups;
    }

    /// <summary>
    /// Load child entities attributes/fields for fields
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<FieldMetaData> LoadChildEntities(ObjectSet<FieldMetaData> objectSet, LoadStrategyResult loadStrategyResult, Action<FieldMetaData> link)
    {
      var fields = new List<FieldMetaData>();
      var muMaterializers = new MuMaterializers();
      //Load fieldmetadata for Fields
      using (OracleDataReader reader = loadStrategyResult.GetReader(GetDynamicFields.MetadataField))
      {
        //while (reader.Read())
        //{
        //  object obj = reader["REQUIRED_TYPE_ID"];
        //  if (obj != null)
        //  {
        //    string objType = obj.GetType().FullName;
        //  }
        //}
        // first result set includes the category
        fields = muMaterializers.FieldMetaDataMaterializer.Materialize(reader).Bind(objectSet).ToList();
        reader.Close();
      }

      //Load attributes
      if (loadStrategyResult.IsLoaded(GetDynamicFields.MetadataAttribute) && fields.Count != 0)
        FieldMetaDataRepository.LoadAttributeEntities(objectSet
            , loadStrategyResult
            , null);

      if (loadStrategyResult.IsLoaded(GetDynamicFields.FieldValue) && fields.Count != 0)
        FieldMetaDataRepository.LoadFieldValueEntities(objectSet.Context.CreateObjectSet<FieldValue>()
            , loadStrategyResult
            , null);

      return fields;
    }

    /// <summary>
    /// Load attribute entites for fields
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<FieldMetaData> LoadAttributeEntities(ObjectSet<FieldMetaData> objectSet, LoadStrategyResult loadStrategyResult, Action<FieldMetaData> link)
    {
      if (link == null) link = new Action<FieldMetaData>(c => { });
      var childMetadata = new List<FieldMetaData>();
      var muMaterializers = new MuMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(GetDynamicFields.MetadataAttribute))
      {
        // first result set includes the category
        foreach (var c in
          muMaterializers.FieldMetaDataMaterializer.Materialize(reader).Bind(objectSet).ForEach<FieldMetaData>(link))
        {
          childMetadata.Add(c);
        }
                if (!reader.IsClosed)
                    reader.Close();
            }
           return childMetadata;
        
    }

    public static List<FieldValue> LoadFieldValueEntities(ObjectSet<FieldValue> objectSet, LoadStrategyResult loadStrategyResult, Action<FieldValue> link)
    {
      if (link == null) link = new Action<FieldValue>(c => { });
      var childValue = new List<FieldValue>();
      var muMaterializers = new MuMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(GetDynamicFields.FieldValue))
      {
        // first result set includes the category
        foreach (var c in
          muMaterializers.FieldValueMaterializer.Materialize(reader).Bind(objectSet).ForEach<FieldValue>(link))
        {
          childValue.Add(c);
        }
                if (!reader.IsClosed)
                    reader.Close();
            }
            return childValue;
        
    }

    /// <summary>
    /// Call stored procedure to fetch field metadata for given combination of ChargeCodeId and ChargeCodeTypeId
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sp"></param>
    /// <param name="IsLoadFieldValue"></param>
    /// <param name="oraInputParameters"></param>
    /// <param name="fetch"></param>
    /// <returns></returns>
    private T ExecuteSP<T>(StoredProcedure sp, bool IsLoadFieldValue, OracleParameter[] oraInputParameters, Func<LoadStrategyResult, T> fetch)
    {
      using (var result = new LoadStrategyResult())
      {
        using (var cmd = Context.CreateStoreCommand(sp.Name, CommandType.StoredProcedure) as OracleCommand)
        {
          cmd.Parameters.AddRange(oraInputParameters);

          // Add result parameters to Oracle Parameter Collection
          foreach (SPResultObject resObj in sp.GetResultSpec())
          {
            var resultParam = new OracleParameter(resObj.ParameterName, OracleDbType.Cursor);
            resultParam.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(resultParam);

            //if the entity is requested, add it to the result
            if (resObj.EntityName == GetDynamicFields.FieldValue)
            {
              if (IsLoadFieldValue)
                result.Add(resObj.EntityName, resultParam);
            }
            else
              result.Add(resObj.EntityName, resultParam);
          }

          using (cmd.Connection.CreateConnectionScope())
          {
            //Execute SP

            //Set CommandTimeout value to value given in the Config file 
            //if it NOT in the config then it will be set to default value 0.
            cmd.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);

            cmd.ExecuteNonQuery();

            //Allow the caller to populate results
            return fetch(result);
          }
        }
      }
    }

    /// <summary>
    /// Sort fields in groups for Required Type
    /// </summary>
    /// <param name="groups"></param>
    private static void SortFieldsInGroup(ref List<FieldMetaData> groups)
    {
      foreach (var field in groups)
      {
        var fieldList = new List<FieldMetaData>(field.SubFields);
        field.SubFields.Clear();
        var tempList = new List<FieldMetaData>();
        tempList = fieldList.Where(s=>s.RequiredType != RequiredType.Optional).OrderBy(o=>o.DisplayOrder).ToList();
        field.SubFields.AddRange(tempList);
        tempList = fieldList.Where(s => s.RequiredType == RequiredType.Optional).OrderBy(o => o.DisplayOrder).ToList();
        field.SubFields.AddRange(tempList);
      }
    }

    /// <summary>
    /// Fetch data for optional groups to populate optional field dropdown
    /// </summary>
    /// <param name="chargeCodeId"></param>
    /// <param name="chargeCodeTypeId"></param>
    /// <returns></returns>
    public List<DynamicGroupDetail> GetOptionalGroupMetadata(int chargeCodeId, Nullable<int> chargeCodeTypeId)
    {
      var parameterArray = new ObjectParameter[] { new ObjectParameter(FieldMetadataRepositoryConstants.ChargeCodeIdParameterName, typeof(Int32)) { Value = chargeCodeId }
        , new ObjectParameter(FieldMetadataRepositoryConstants.ChargeCodeTypeIdParameterName, typeof(Int32)) { Value = chargeCodeTypeId } };

      var optionalGroup = ExecuteStoredFunction<DynamicGroupDetail>(FieldMetadataRepositoryConstants.GetOptionalGroupFunctionName, parameterArray);

      return optionalGroup.ToList();
    }

    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result.
    /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <param name="entityName"></param>
    /// <returns></returns>
    public static List<FieldMetaData> LoadEntities(ObjectSet<FieldMetaData> objectSet, LoadStrategyResult loadStrategyResult, Action<FieldMetaData> link, string entityName)
    {
        if (link == null)
            link = new Action<FieldMetaData>(c => { });

        var fieldMetaDataList = new List<FieldMetaData>();
        
        var miscUatpMaterializer = new Materializers();
        using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
        {
            // first result set includes the category
          fieldMetaDataList = miscUatpMaterializer.FieldMetaDataMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
if (!reader.IsClosed)          
reader.Close();
        }

        //Load LIDetFieldValuesFieldMetaData by calling respective LoadEntities method
        if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LIDFMDataSource) && fieldMetaDataList.Count != 0)
        {
          DataSourceRepository.LoadEntities(objectSet.Context.CreateObjectSet<DataSource>(), loadStrategyResult, null, LoadStrategy.MiscEntities.LIDFMDataSource);
          //The fetched child records should use the Parent entities.
        }

        return fieldMetaDataList;
    }
  }
}
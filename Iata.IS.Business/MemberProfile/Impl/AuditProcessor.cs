using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.MemberProfile;
using System.Linq;
using Iata.IS.Model.MemberProfile.Enums;
using log4net;

namespace Iata.IS.Business.MemberProfile.Impl
{
  /// <summary>
  /// Processes the audit entries for an entity T.
  /// </summary>
  /// <typeparam name="T">The entity type for which the audit needs to be processed.</typeparam>
  public class AuditProcessor<T> where T : EntityBase<int>
  {
    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private readonly IEnumerable<PropertyInfo> _auditableProperties;
    private const string MaskedValue = "********";
    private const string YesValue = "Yes";
    private const string NoValue = "No";
    private const string UpdateFlavorParameter = "UpdateFlavor";
    private const string ElementGroupParameter = "ElementGroup";
    private const string ElementGroupDisplayNameParameter = "ElementGroupDisplayName";
    private const string ElementTableParameter = "ElementTable";
    private const string ElementNameParameter = "ElementName";
    private const string IncludeRelationParameter = "IncludeRelationId";
    private const string IncludeDisplayNameParameter = "IncludeDisplayNames";
    private const string MaskValuesParameter = "MaskValues";
    private const string IgnoreValueParameter = "IgnoreValue";

    private const string DisplayValueSuffix = "DisplayValue";
    private const string FutureDisplayValueSuffix = "FutureDisplayValue";
    private const string FutureUpdateDateSuffix = "FutureDate";
    private const string FutureUpdatePeriodSuffix = "FuturePeriod";
    private const string FutureValueSuffix = "FutureValue";

    /// <summary>
    /// Gets or sets the future updates repository.
    /// </summary>
    /// <value>The FutureUpdates Repository.</value>
    public IRepository<FutureUpdates> FutureUpdatesRepository { get; set; }

    /// <summary>
    /// Constructor - creates a list of auditable properties using reflection.
    /// </summary>
    public AuditProcessor()
    {
      // Find auditable properties from T.
      _auditableProperties = GetAuditableProperties();

      FutureUpdatesRepository = Ioc.Resolve<IRepository<FutureUpdates>>(typeof(IRepository<FutureUpdates>));
        
    }

    /// <summary>
    /// Returns a list of property info objects for the auditable properties (using reflection).
    /// </summary>
    private static IEnumerable<PropertyInfo> GetAuditableProperties()
    {
      var type = typeof(T);

      var props = from propertyInfo in type.GetProperties()
                  where Attribute.IsDefined(propertyInfo, typeof(AuditAttribute))
                  select propertyInfo;

      return props;
    }

    /// <summary>
    /// Processes the model instances and creates the list of FutureUpdates objects.
    /// </summary>
    /// <param name="memberId">Id of the member for which the processing is being done.</param>
    /// <param name="actionType">Either Crete/Update/Delete.</param>
    /// <param name="elementGroupType">The group type for which the audit entries are to be created/updated.</param>
    /// <param name="originalModel">Original model instance (from the database).</param>
    /// <param name="modifiedModel">Modified model instance.</param>
    /// <param name="updatedBy">Id if the user who has modified.</param>
    /// <param name="pendingUpdates">List of pending future updates.</param>
    public List<FutureUpdates> ProcessAuditEntries(int memberId, ActionType actionType, ElementGroupType elementGroupType, T originalModel, T modifiedModel, int updatedBy, List<FutureUpdates> pendingUpdates = null)
    {
      var futureUpdateList = new List<FutureUpdates>();
      
      // Process each auditable property.
      foreach (var auditableProperty in _auditableProperties)
      {
        var updateFlavor = UpdateFlavor.ImmediateUpdate;
        var groupType = ElementGroupType.MemberDetails;
        var groupDisplayName = string.Empty;
        var elementTable = string.Empty;
        var elementName = string.Empty;
        var includeDisplayName = false;
        var includeRelation = false;
        var maskValues = false;
        var ignoreValue = int.MinValue;
        var valuesIdentical = false;
        string originalDisplayName = null;
        string modifiedDisplayName = null;
        var changeEffectiveOn = DateTime.UtcNow;

        // Get the values of the parameters specified for the Audit attribute.
        foreach (var customAttributeNamedArgument in auditableProperty.GetCustomAttributesData().SelectMany(attributeData => attributeData.NamedArguments))
        {
          switch (customAttributeNamedArgument.MemberInfo.Name)
          {
            case UpdateFlavorParameter:
              updateFlavor = (UpdateFlavor)customAttributeNamedArgument.TypedValue.Value;
              break;

            case ElementGroupParameter:
              groupType = (ElementGroupType)customAttributeNamedArgument.TypedValue.Value;
              break;

            case ElementGroupDisplayNameParameter:
              groupDisplayName = (string)customAttributeNamedArgument.TypedValue.Value;
              break;

            case ElementTableParameter:
              elementTable = (string)customAttributeNamedArgument.TypedValue.Value;
              break;

            case ElementNameParameter:
              elementName = (string)customAttributeNamedArgument.TypedValue.Value;
              break;

            case IncludeDisplayNameParameter:
              includeDisplayName = (bool)customAttributeNamedArgument.TypedValue.Value;
              break;

            case IncludeRelationParameter:
              includeRelation = (bool)customAttributeNamedArgument.TypedValue.Value;
              break;

            case MaskValuesParameter:
              maskValues = (bool)customAttributeNamedArgument.TypedValue.Value;
              break;

            case IgnoreValueParameter:
              ignoreValue = (int)customAttributeNamedArgument.TypedValue.Value;
              break;
          }
        }

        // Skip all unwanted group entries.
        if (groupType != elementGroupType)
        {
          continue;
        }

        // Give a default value for the display name if it is not specified.
        if (string.IsNullOrEmpty(groupDisplayName))
        {
          groupDisplayName = elementTable;
        }

        // Get the original and the modified values.
        var originalValue = actionType != ActionType.Create ? auditableProperty.GetValue(originalModel, null) : null;
        var modifiedValue = updateFlavor == UpdateFlavor.ImmediateUpdate || actionType == ActionType.Create ? auditableProperty.GetValue(modifiedModel, null) : GetPropertyValue(modifiedModel, auditableProperty.Name, FutureValueSuffix);

        // Check if values are to be masked.
        if (maskValues)
        {
          originalValue = MaskedValue;
          modifiedValue = MaskedValue;
          originalDisplayName = MaskedValue;
          modifiedDisplayName = MaskedValue;
        }
        else
        {
          // Check if this is a future update property.
          if (updateFlavor != UpdateFlavor.ImmediateUpdate)
          {
            if (updateFlavor == UpdateFlavor.FutureUpdateDate)
            {
              var futureUpdateDate = GetPropertyValue(modifiedModel, auditableProperty.Name, FutureUpdateDateSuffix);
              if (futureUpdateDate != null)
              {
                changeEffectiveOn = DateTime.ParseExact(futureUpdateDate.ToString(), "dd-MMM-yy", CultureInfo.InvariantCulture);
              }
            }
            else if (updateFlavor == UpdateFlavor.FutureUpdatePeriod)
            {
              var futureUpdatePeriod = GetPropertyValue(modifiedModel, auditableProperty.Name, FutureUpdatePeriodSuffix);
              if (futureUpdatePeriod != null)
              {
                changeEffectiveOn = DateTime.ParseExact(futureUpdatePeriod.ToString(), "yyyy-MMM-dd", CultureInfo.InvariantCulture);
              }
            }
          }
        }

        // Check if display names need to be included.
        GetDisplayNames(auditableProperty, modifiedModel, originalValue, modifiedValue, includeDisplayName, ref originalDisplayName, ref modifiedDisplayName);

        try
        {
          // Verify whether audit entry needs to be created.
          if (originalValue == null && modifiedValue == null) // Both values are null.
          {
            // If original and modified values are null and also ChangeEffectiveOn value is null then no need to add record to db
            if ((changeEffectiveOn == null) || (changeEffectiveOn.ToShortDateString() == DateTime.UtcNow.ToShortDateString()))
            {
              // Original and modified values are null.
              _logger.InfoFormat("Original and modified values are null.");
              continue;
            }
            else
            {
              // If original and modified values are null and ChangeEffectiveOn value is NOT null then the record should be deleted from db
              _logger.Info("Deleting Future Update Record since no update is specified for this field.");
              var pendingUpdate = pendingUpdates.SingleOrDefault(update => update.ElementName == elementName);

              // Only if there is a pending entry.
              if (pendingUpdate != null)
              {
                  var objFutureContext = FutureUpdatesRepository.Get(f => f.Id == pendingUpdate.Id).SingleOrDefault();
                  FutureUpdatesRepository.Delete(objFutureContext);
                  continue;
              }
            }
          }

          if ((modifiedValue == null) && ((changeEffectiveOn == null) || (changeEffectiveOn.ToShortDateString()  == DateTime.UtcNow.ToShortDateString())))
          {
            // Modified value is null
            _logger.InfoFormat("Modified value is null.");
            continue;
          }

          if ((ignoreValue != int.MinValue) && (originalValue == null) && ((modifiedValue != null) && ((int)modifiedValue == ignoreValue)))
          {
            // Modified value is to be ignored
            _logger.InfoFormat("Modified value is to be ignored.");
            continue;
          }

          if ((actionType == ActionType.Update) && (originalValue != null && modifiedValue != null) && (originalValue.ToString() == modifiedValue.ToString()))
          {
            valuesIdentical = true;
          }
        }
        catch (Exception ex)
        {
          _logger.Error("Error comparing property values", ex);

          // Ignore property and continue.
          continue;
        }

        // Check if an existing audit trail entry needs to be updated instead of creating a new one. This case may arise when there are pending future updates.
        if ((updateFlavor != UpdateFlavor.ImmediateUpdate) && (pendingUpdates != null))
        {
          // Check if this field has any pending future updates.
          var pendingUpdate = pendingUpdates.SingleOrDefault(update => update.ElementName == elementName);

          // Only if there is a pending entry.
          if (pendingUpdate != null)
          {
            if (modifiedValue != null)
            {
              if ((((pendingUpdate.NewVAlue == modifiedValue.ToString())) && updateFlavor == UpdateFlavor.FutureUpdateDate &&
                   pendingUpdate.ChangeEffectiveOn == changeEffectiveOn) ||
                  (((pendingUpdate.NewVAlue == modifiedValue.ToString())) && updateFlavor == UpdateFlavor.FutureUpdatePeriod &&
                   pendingUpdate.ChangeEffectivePeriod == changeEffectiveOn))
              {
                // Values are same as well as future date/period is same.
                continue;
              }
            }
            else
            {
              if ((((pendingUpdate.NewVAlue == null) || (pendingUpdate.NewVAlue == modifiedValue)) && updateFlavor == UpdateFlavor.FutureUpdateDate &&
                   pendingUpdate.ChangeEffectiveOn == changeEffectiveOn) ||
                  (((pendingUpdate.NewVAlue == null) || (pendingUpdate.NewVAlue == modifiedValue)) && updateFlavor == UpdateFlavor.FutureUpdatePeriod &&
                   pendingUpdate.ChangeEffectivePeriod == changeEffectiveOn))
              {
                // Values are same as well as future date/period is same.
                continue;
              }
          }

          // Update the pending future update.
            {
              pendingUpdate.ActionType = actionType;
              pendingUpdate.OldVAlue = originalValue != null ? originalValue.ToString() : null;
              pendingUpdate.NewVAlue = modifiedValue != null ? modifiedValue.ToString() : null;
              pendingUpdate.OldValueDisplayName = originalDisplayName;
              pendingUpdate.NewValueDisplayName = modifiedDisplayName;
              pendingUpdate.ChangeEffectiveOn = updateFlavor != UpdateFlavor.FutureUpdatePeriod ? changeEffectiveOn : (DateTime?)null;
              pendingUpdate.ChangeEffectivePeriod = updateFlavor == UpdateFlavor.FutureUpdatePeriod ? changeEffectiveOn : (DateTime?)null;
              pendingUpdate.RelationId = includeRelation ? modifiedModel.Id : (int?)null;
              pendingUpdate.LastUpdatedBy = updatedBy;
              pendingUpdate.LastUpdatedOn = DateTime.UtcNow;
              pendingUpdate.ModifiedOn = DateTime.UtcNow;
              pendingUpdate.DisplayGroup = string.IsNullOrEmpty(pendingUpdate.DisplayGroup) && string.IsNullOrEmpty(groupDisplayName) ? null : groupDisplayName;

              // Existing log entry will be updated.
              futureUpdateList.Add(pendingUpdate);

              // Go for the next record.
              continue;
            }
          }
        }

        // If the values are identical after reaching here - then we have to skip this update.
        if (valuesIdentical)
        {
          continue;
        }

        // Create a new audit log entry.
        futureUpdateList.Add(new FutureUpdates
        {
          ActionType = actionType,
          ChangeEffectiveOn = updateFlavor != UpdateFlavor.FutureUpdatePeriod || actionType == ActionType.Create ? changeEffectiveOn : (DateTime?)null,
          ChangeEffectivePeriod = updateFlavor == UpdateFlavor.FutureUpdatePeriod && actionType != ActionType.Create ? changeEffectiveOn : (DateTime?)null,
          ElementGroupTypeId = (int)groupType,
          DisplayGroup = groupDisplayName,
          TableName = elementTable,
          ElementName = elementName,
          IsChangeApplied = updateFlavor == UpdateFlavor.ImmediateUpdate || actionType == ActionType.Create,
          RelationId = includeRelation ? modifiedModel.Id : (int?)null,
          OldVAlue = originalValue != null ? originalValue.ToString() : null,
          NewVAlue = modifiedValue != null ? modifiedValue.ToString() : null,
          OldValueDisplayName = originalDisplayName,
          NewValueDisplayName = modifiedDisplayName,
          MemberId = memberId,
          LastUpdatedBy = updatedBy,
          LastUpdatedOn = DateTime.UtcNow,
          ModifiedOn = DateTime.UtcNow
        });
      }

      return futureUpdateList;
    }

    private static void GetDisplayNames(PropertyInfo auditableProperty, T modifiedModel, object originalValue, object modifiedValue, bool includeDisplayName, ref string originalDisplayName, ref string modifiedDisplayName)
    {
      // Check if display names are to be included or if the value is a boolean.
      if (includeDisplayName || auditableProperty.PropertyType == typeof(bool))
      {
        if (modifiedValue != null)
        {
          //if (modifiedValue.ToString() == bool.TrueString)
          if (string.Equals(modifiedValue.ToString(), bool.TrueString,StringComparison.CurrentCultureIgnoreCase))
          {
            modifiedDisplayName = YesValue;
            originalDisplayName = originalValue != null ? NoValue : string.Empty;
          }
          //else if (modifiedValue.ToString() == bool.FalseString)
          else if (string.Equals(modifiedValue.ToString(), bool.FalseString, StringComparison.CurrentCultureIgnoreCase))
          {
            modifiedDisplayName = NoValue;
            originalDisplayName = originalValue != null ? YesValue : string.Empty;
          }
          else
          {
            // Get the display names for the drop-downs.
            var orgDisplayValue = GetPropertyValue(modifiedModel, auditableProperty.Name, DisplayValueSuffix);
            var modifiedDisplayValue = GetPropertyValue(modifiedModel, auditableProperty.Name, FutureDisplayValueSuffix);

            originalDisplayName = orgDisplayValue != null ? orgDisplayValue.ToString() : null;
            modifiedDisplayName = modifiedDisplayValue != null ? modifiedDisplayValue.ToString() : null;
          }
        }
      }
      else
      {
        // Use the original and modified values as the display names.
        originalDisplayName = originalValue != null ? originalValue.ToString() : string.Empty;
        modifiedDisplayName = modifiedValue != null ? modifiedValue.ToString() : string.Empty;
      }
    }

    private static object GetPropertyValue(T instance, string propertyName, string propertySuffix)
    {
      // Type instance that needs to be queried.
      var type = typeof(T);

      // Find the property in the type using reflection with the specified name.
      var memberInfoList = type.FindMembers(MemberTypes.Property, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty, (info, criteria) => Equals(info.Name, criteria), propertyName + propertySuffix);

      // We are expecting only one such property.
      if (memberInfoList.Length == 1)
      {
        var propertyInfo = memberInfoList[0] as PropertyInfo;
        return propertyInfo != null ? propertyInfo.GetValue(instance, null) : null;
      }

      return null;
    }
  }
}
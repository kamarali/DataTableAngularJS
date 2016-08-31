using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Iata.IS.Core;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.MiscUatp.Enums;
using Iata.IS.Web.Util.DynamicFields.Base;

namespace Iata.IS.Web.Util.ModelBinders.Misc
{
  public class LineItemDetailModelBinder : DefaultModelBinder
  {
    public const string TaxCode = "TaxCode";
    private const string CalculatedAmount = "CalculatedAmount";
    private const string VatSubType = "VATSubType";

    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var lineItemDetailModel = model as LineItemDetail;

      // Make sure we have a line item instance.
      if (lineItemDetailModel != null)
      {
        var form = controllerContext.HttpContext.Request.Form;
        int? serviceStartDay;
        if (!string.IsNullOrEmpty(form[ControlIdConstants.ServiceStartDay]))
        {
          serviceStartDay = Convert.ToInt32(form[ControlIdConstants.ServiceStartDay]);
          var serviceMonthYear = form[ControlIdConstants.ServiceStartDateDropdown].Split('-');
          lineItemDetailModel.StartDate = new DateTime(Convert.ToInt32(serviceMonthYear[1]), Convert.ToInt32(serviceMonthYear[0]), serviceStartDay.Value);
        }
        if (!string.IsNullOrEmpty(form[ControlIdConstants.ServiceEndDay]))
        {
          var serviceEndDay = Convert.ToInt32(form[ControlIdConstants.ServiceEndDay]);
          var serviceMonthYear = form[ControlIdConstants.ServiceEndDateDropdown].Split('-');
          lineItemDetailModel.EndDate = new DateTime(Convert.ToInt32(serviceMonthYear[1]), Convert.ToInt32(serviceMonthYear[0]), serviceEndDay);
        }

        var taxAmountFieldIds = form.AllKeys.Where(a => a.StartsWith(CalculatedAmount));
        var id = string.Empty;
        foreach (string fieldId in taxAmountFieldIds)
        {
          // get id prefix from fieldId(e.g. 1 in case of Amount1)
          id = fieldId.Substring(CalculatedAmount.Length, fieldId.Length - CalculatedAmount.Length);

          if (string.IsNullOrEmpty(id)) continue;
          lineItemDetailModel.TaxBreakdown.Add(new LineItemDetailTax
          {
            Amount = string.IsNullOrEmpty(form[string.Format("Amount{0}", id)]) ? (decimal?)null : Convert.ToDecimal(form[string.Format("Amount{0}", id)]),
            Percentage = string.IsNullOrEmpty(form[string.Format("Percentage{0}", id)]) ? (double?)null : Convert.ToDouble(form[string.Format("Percentage{0}", id)]),
            /*SCP257502:  VAT NODE MISSING-Upendra*/
            CalculatedAmount =
              string.IsNullOrEmpty(form[string.Format("CalculatedAmount{0}", id)])
                ? (decimal?)null
                : Convert.ToDecimal(form[string.Format("CalculatedAmount{0}", id)]),
            CategoryCode = form[string.Format("CategoryCode{0}", id)],
            Id = (form[string.Format("TaxId{0}", id)] != string.Empty ? form[string.Format("TaxId{0}", id)].ToGuid() : new Guid()),
            Description = form[string.Format("TaxDescription{0}", id)],
            Type = TaxType.Tax,
            SubType = form[string.Format("SubType{0}", id)]
          });
        }

        var vatSubTypeFieldIds = form.AllKeys.Where(a => a.Contains(VatSubType));
        foreach (var vatSubTypeFieldId in vatSubTypeFieldIds)
        {
          id = vatSubTypeFieldId.Substring(VatSubType.Length, vatSubTypeFieldId.Length - VatSubType.Length);
          if (string.IsNullOrEmpty(id)) continue;
          lineItemDetailModel.TaxBreakdown.Add(new LineItemDetailTax
          {
            SubType = form[string.Format("VATSubType{0}", id)],
            Amount =
              string.IsNullOrEmpty(form[string.Format("VATBaseAmount{0}", id)]) ? (decimal?)null : Convert.ToDecimal(form[string.Format("VATBaseAmount{0}", id)]),
            Percentage = string.IsNullOrEmpty(form[string.Format("VATPercent{0}", id)]) ? (double?)null : Convert.ToDouble(form[string.Format("VATPercent{0}", id)]),
            CalculatedAmount = GetVatCalculatedAmount(form[string.Format("VATCalculatedAmount{0}", id)]),
            CategoryCode = form[string.Format("VATCategoryCode{0}", id)],
            Description = form[string.Format("VATDescription{0}", id)],
            Id = string.IsNullOrEmpty(form[string.Format("VATId{0}", id)]) ? new Guid() : form[string.Format("VATId{0}", id)].ToGuid(),
            Type = TaxType.VAT
          });
        }

        // For add on charges.
        var addOnChargeNameFieldIds = form.AllKeys.Where(a => a.StartsWith(ControlIdConstants.AddChargeName));
        foreach (string addOnChargeNameFieldId in addOnChargeNameFieldIds)
        {
          id = addOnChargeNameFieldId.Substring(ControlIdConstants.AddChargeName.Length, addOnChargeNameFieldId.Length - ControlIdConstants.AddChargeName.Length);
          if (string.IsNullOrEmpty(id)) continue;

          var nameId = string.Format(ControlIdConstants.AddChargeName + "{0}", id);
          var baseAmountId = string.Format(ControlIdConstants.ChargeableAmount + "{0}", id);
          var addChargePercentageId = string.Format(ControlIdConstants.AddChargePercentage + "{0}", id);
          var addChargeAmountId = string.Format(ControlIdConstants.AddChargeAmount + "{0}", id);

          lineItemDetailModel.AddOnCharges.Add(new LineItemDetailAddOnCharge
          {
            Name = form[nameId],
            ChargeableAmount = string.IsNullOrEmpty(form[baseAmountId]) ? (decimal?)null : Convert.ToDecimal(form[baseAmountId]),
            Percentage = string.IsNullOrEmpty(form[addChargePercentageId]) ? (double?)null : Convert.ToDouble(form[addChargePercentageId]),
            Amount = string.IsNullOrEmpty(form[addChargeAmountId]) ? 0 : Convert.ToDecimal(form[addChargeAmountId]),
            Id = (form[string.Format("Id{0}", id)] != string.Empty ? form[string.Format("Id{0}", id)].ToGuid() : new Guid()),
          });
        }

        SetDynamicFieldValues(ref lineItemDetailModel, form);
      }
      return model;
    }

    /// <summary>
    /// This method sets the dynamic field values in LineItemDetail in metadata hierarchy form.
    /// </summary>
    /// <param name="lineItemDetailModel"></param>
    /// <param name="form"></param>
    private static void SetDynamicFieldValues(ref LineItemDetail lineItemDetailModel, NameValueCollection form)
    {
      var dynamicFieldIds = form.AllKeys.Where(a => a.Contains(ControlIdConstants.DynamicFieldPrefix));

      // Control id will be in the form: DF_{ParentMetadataId}_{metadataId}_Group{Count}_DFValue{Count}
      foreach (var dynamicFieldId in dynamicFieldIds)
      {
        // Do all processing on control Id only if value is entered for the field.
        if (!string.IsNullOrEmpty(form[dynamicFieldId]))
        {
          // Get Field metadata id and parent metadata id from the control Id.
          string parentMetadataId;
          string fieldMetadataId = GetFieldMetadataIdFromControlId(dynamicFieldId, out parentMetadataId);

          // If parent metadata Id is null then it is first level group.
          if (string.IsNullOrEmpty(parentMetadataId))
          {
            // Add field value for group in field values collection.
            lineItemDetailModel.FieldValues.Add(new FieldValue { FieldMetaDataId = fieldMetadataId.ToGuid(), ControlId = dynamicFieldId });
          }
          else
          {
            // Else search for the parent field value and Add field value of current control to AttributeValues of parent field value.
            // If more than 1 field value is present for the parent field then use 
            // a. group count in case of group metadata b. use suffix of current field metadata to search using control Id.

            // Check parent Id in first level group values.
            var parentFieldValues = lineItemDetailModel.FieldValues.Where(fv => fv.FieldMetaDataId.ToString().Equals(parentMetadataId));

            FieldValue parentFieldValue = null;
            var parentFieldValuesCount = parentFieldValues.Count();
            if (parentFieldValuesCount == 1)
            {
              parentFieldValue = (parentFieldValues.ToList())[0];
            }
            else if (parentFieldValuesCount == 0)
            {
              // If parent metadata is not present at Group field values level then it is Field Metadata or Group within Group.

              // If more than 1 field value is present for same field metadata then it is field multiple occurrence or multiple occurrences of parent group.
              // Get all the group field values whose attribute field value is for parent metadata id.
              parentFieldValues = lineItemDetailModel.FieldValues.Where(fv => fv.AttributeValues.Where(av => av.FieldMetaDataId.ToString().Equals(parentMetadataId)).Count() > 0);
              parentFieldValuesCount = parentFieldValues.Count();

              // If only 1 group is present with more than 1 attribute values then it could be case of field multiple occurrence.
              // In all the cases check for matching parent value using parent control id suffix.
              if (parentFieldValuesCount > 0 )
              {
                parentFieldValue = GetMatchingParentValue(parentFieldValues, dynamicFieldId, fieldMetadataId, parentMetadataId);
              }

              // If parent field value is not present at 2nd level.
              if (parentFieldValues.Count() == 0)
              {
                // Check whether parent field value is present at 3rd level.
                parentFieldValues = lineItemDetailModel.FieldValues.Where(fv => fv.AttributeValues.Where(av => av.AttributeValues.Where(childAttrValues => childAttrValues.FieldMetaDataId.ToString().Equals(parentMetadataId)).Count() > 0).Count() > 0);

                if (parentFieldValues.Count() == 1)
                {
                  parentFieldValue = (parentFieldValues.ToList())[0].AttributeValues.Where(av => av.FieldMetaDataId.ToString().Equals(parentMetadataId)).ToList()[0];
                }
              }

              // If value is entered for Attribute of field and not for Field then make entry of Field with empty value and then add attribute value.
              if (parentFieldValue == null && !string.IsNullOrEmpty(form[dynamicFieldId]))
              {
                string parentFieldControlId;
                FieldValue parentsParentFieldValue = GetParentsParentFieldValue(dynamicFieldId, fieldMetadataId, dynamicFieldIds, lineItemDetailModel.FieldValues, out parentFieldControlId);
                if (parentsParentFieldValue != null)
                {
                  // Make entry of parent Field with empty value and then add attribute value.
                  parentFieldValue = new FieldValue { FieldMetaDataId = parentMetadataId.ToGuid(), ControlId = parentFieldControlId };
                  parentsParentFieldValue.AttributeValues.Add(parentFieldValue);
                }
              }

            }
            // Group multiple occurrence.
            else if (parentFieldValues.Count() > 1)
            {
              // If more than 1 field value is present for the parent field then use 
              // a. group count in case of group metadata 
              // Group control id is of the form DF_{metadata-id}_DFGroupId{Count}
              // Serach group using control id of the group.
              int groupCount = GetGroupCount(dynamicFieldId);
              var parentGroupControlId = Constants.DynamicFieldPrefix + parentMetadataId + Constants.DynamicGroupIdSuffix + groupCount;
              parentFieldValues = lineItemDetailModel.FieldValues.Where(fv => fv.ControlId.ToString().Equals(parentGroupControlId));
              parentFieldValue = (parentFieldValues.ToList())[0];
            }

            if (parentFieldValue != null)
            {
              // Add field value of current field into parent field value of its parent.
              if (!string.IsNullOrEmpty(form[dynamicFieldId]))
              {
                var fieldValue = form[dynamicFieldId];
                parentFieldValue.AttributeValues.Add(new FieldValue { FieldMetaDataId = fieldMetadataId.ToGuid(), Value = fieldValue, ControlId = dynamicFieldId });
              }
            }
          }
        }
      }

      // Remove all sub group field values if no values are entered for any of the field in it.
      // Get all groups containing sub group.
      var groupWithSubGroupFieldValues = lineItemDetailModel.FieldValues.Where(fv => fv.AttributeValues.Where(av => av.ControlId.Contains(Constants.DynamicGroupIdSuffix)).Count() > 0);
      foreach (var groupFieldValue in groupWithSubGroupFieldValues)
      {
        var subGroupFieldValue = groupFieldValue.AttributeValues.Where(av => av.ControlId.Contains(Constants.DynamicGroupIdSuffix)).ToList();
        if (subGroupFieldValue.Count == 1)
        {
          if (subGroupFieldValue[0].AttributeValues.Count == 0)
          {
            groupFieldValue.AttributeValues.Remove(subGroupFieldValue[0]);
          }
        }
      }

      // Remove field value of group for which value is not entered for any of its fields/attributes.
      lineItemDetailModel.FieldValues.RemoveAll(fieldValue => fieldValue.AttributeValues.Count == 0);

    }

    /// <summary>
    /// Gets the parents parent field value of current field from the field value collection.
    /// </summary>
    /// <param name="dynamicFieldId">Dynamic field control id.</param>
    /// <param name="fieldMetadataId">Field metadata id.</param>
    /// <param name="dynamicFieldIds">Dynamic field control ids collection.</param>
    /// <param name="fieldValues">Line item detail field values.</param>
    /// <param name="parentFieldControlId">Parent field control id </param>
    /// <returns></returns>
    private static FieldValue GetParentsParentFieldValue(string dynamicFieldId, string fieldMetadataId, IEnumerable<string> dynamicFieldIds,
      List<FieldValue> fieldValues, out string parentFieldControlId)
    {
      var parentControlIdSuffix = dynamicFieldId.Remove(dynamicFieldId.IndexOf("_" + fieldMetadataId), fieldMetadataId.Length + 1);
      parentControlIdSuffix = parentControlIdSuffix.TrimStart(ControlIdConstants.DynamicFieldPrefix.ToCharArray());

      var parentControlId = parentFieldControlId = dynamicFieldIds.Where(df => df.EndsWith(parentControlIdSuffix)).ToList()[0];

      var groupCount = GetGroupCount(parentControlId);
      string parentsParentMetadataId;

      // Get parent metadata id.
      GetFieldMetadataIdFromControlId(parentControlId, out parentsParentMetadataId);

      if (!string.IsNullOrEmpty(parentsParentMetadataId))
      {
        var parentGroupControlId = Constants.DynamicFieldPrefix + parentsParentMetadataId + Constants.DynamicGroupIdSuffix + groupCount;
        // Get parent group field value.
        var parentGroupFieldValues = fieldValues.Where(fv => fv.ControlId.Equals(parentGroupControlId));
        if (parentGroupFieldValues.Count() != 0)
        {
          return parentGroupFieldValues.ToList()[0];
        }
        else
        {
          parentGroupFieldValues = fieldValues.Where(fv => fv.AttributeValues.Where(av => av.ControlId.Equals(parentGroupControlId)).Count() > 0);

          if (parentGroupFieldValues.Count() == 1)
          {
            // Get field value from groups field values collection.
            return (parentGroupFieldValues.ToList())[0].AttributeValues[0];
          }
        }
      }
      return null;
    }

    /// <summary>
    /// Gets the group count from control id.
    /// </summary>
    /// <param name="dynamicFieldId">Dynamic field control id.</param>
    /// <returns></returns>
    private static int GetGroupCount(string dynamicFieldId)
    {
      var groupCount = 1;
      // If dynamicFieldId is of group then it contains "_DFGroupId" suffix
      if (dynamicFieldId.Contains(Constants.DynamicGroupIdSuffix))
      {
        var groupCountStringIndex = dynamicFieldId.IndexOf(Constants.DynamicGroupIdSuffix);
        var groupCountSuffix = dynamicFieldId.Substring(groupCountStringIndex);
        int.TryParse(groupCountSuffix.Replace(Constants.DynamicGroupIdSuffix, string.Empty), out groupCount);
      }
      // DF_{id}_Grp{Count}_DFValue{Count}
      else if (dynamicFieldId.Contains(Constants.DynamicGroupCountString))
      {
        var groupCountStringIndex = dynamicFieldId.IndexOf(Constants.DynamicGroupCountString);
        var dfValueSuffixIndex = dynamicFieldId.IndexOf(Constants.DynamicFieldValueSuffix);
        var groupCountStartIndex = groupCountStringIndex + Constants.DynamicGroupCountString.Length;
        var groupCountLength = dfValueSuffixIndex - groupCountStartIndex;
        int.TryParse(dynamicFieldId.Substring(groupCountStringIndex + Constants.DynamicGroupCountString.Length, groupCountLength), out groupCount);
      }

      return groupCount;
    }

    /// <summary>
    /// This method returns matching parent field value in case of field/group multiple occurrence.
    /// </summary>
    /// <param name="parentFieldValues">Parent field values.</param>
    /// <param name="dynamicFieldId">Dynamic field control id.</param>
    /// <param name="fieldMetadataId">Field metadata id.</param>
    /// <param name="parentMetadataId">Parent metadata id.</param>
    /// <returns></returns>
    private static FieldValue GetMatchingParentValue(IEnumerable<FieldValue> parentFieldValues, string dynamicFieldId, string fieldMetadataId, string parentMetadataId)
    {
      // Check if parent is Group or field value.
      var parentValue = parentFieldValues.ToList()[0].AttributeValues.Where(av => av.FieldMetaDataId.ToString().Equals(parentMetadataId)).ToList()[0];

      string parentControlIdSuffix;
      int groupCount = GetGroupCount(dynamicFieldId);

      parentControlIdSuffix = dynamicFieldId.Remove(dynamicFieldId.IndexOf("_" + fieldMetadataId), fieldMetadataId.Length + 1);
      parentControlIdSuffix = parentControlIdSuffix.TrimStart(ControlIdConstants.DynamicFieldPrefix.ToCharArray());

      // If parent is Group then group control Id suffix will be of the form {MID}_DFGroupId{Count}
      if (parentValue.ControlId.Contains(Constants.DynamicGroupIdSuffix))
      {
        parentControlIdSuffix = parentControlIdSuffix.Remove(parentControlIdSuffix.IndexOf(Constants.DynamicGroupCountString));
        parentControlIdSuffix += Constants.DynamicGroupIdSuffix + groupCount;
      }

      var parentsParentFieldValue = parentFieldValues.Where(fv => fv.AttributeValues.Where(av => av.ControlId.EndsWith(parentControlIdSuffix)).Count() > 0);

      if (parentsParentFieldValue.Count() > 0)
        return parentsParentFieldValue.ToList()[0].AttributeValues.Where(av => av.ControlId.EndsWith(parentControlIdSuffix)).ToList()[0];
      else
        return null;
    }

    /// <summary>
    /// Gets the field metadata id from control id.
    /// </summary>
    /// <param name="dynamicFieldId">The dynamic field id.</param>
    /// <returns></returns>
    private static string GetFieldMetadataIdFromControlId(string dynamicFieldId, out string parentMetadataId)
    {
      string fieldMetadataId;
      // If control Id contains group count then only consider it while retrieving metadata id from control id.
      if (dynamicFieldId.Contains(Constants.DynamicGroupIdSuffix))
      {
        var indexOfGroupSuffix = dynamicFieldId.IndexOf(Constants.DynamicGroupIdSuffix);
        fieldMetadataId = dynamicFieldId.Substring(0, indexOfGroupSuffix);
      }
      else
      {
        var indexOfDfValueSuffix = dynamicFieldId.IndexOf(Constants.DynamicGroupCountString);
        fieldMetadataId = dynamicFieldId.Substring(0, indexOfDfValueSuffix);
      }
      fieldMetadataId = fieldMetadataId.TrimStart(ControlIdConstants.DynamicFieldPrefix.ToCharArray());
      // Split metadata ids on '_'
      var metadataIds = fieldMetadataId.Split('_');
      parentMetadataId = string.Empty;
      if (metadataIds.Length == 1)
      {
        fieldMetadataId = metadataIds[0];
      }
      else
      {
        parentMetadataId = metadataIds[0];
        fieldMetadataId = metadataIds[1];
      }

      return fieldMetadataId;
    }


    private static decimal? GetVatCalculatedAmount(string calculatedAmountInString)
    {
      decimal? returnValue = null;
      decimal parsedValue;
      if (!string.IsNullOrEmpty(calculatedAmountInString) && decimal.TryParse(calculatedAmountInString, out parsedValue))
      {
        returnValue = parsedValue;
      }

      return returnValue;
    }

  }
}

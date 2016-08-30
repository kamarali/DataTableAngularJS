<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile.Enums" %>
<table  border="0" cellpadding="5" cellspacing="0">
  <thead align="left" valign="middle">
    <tr>
      <td style="width: 110px; font-weight: bold;">
      </td>
      <td style="width: 110px; font-weight: bold;">
        Invoice PDF
      </td>
      <td style="width: 175px; font-weight: bold;">
        Details Listings
      </td>
      <td style="width: 175px; font-weight: bold;">
        Supporting Documents
      </td>
      <% if (ViewData["TabType"] == "pax" || ViewData["TabType"] == "cgo")
         {%>
      <td style="width: 100px; font-weight: bold;">
        Memo Details
      </td>
      <%
         }%>
      <td style="width: 200px; font-weight: bold;">
        Digital Signature Files
      </td>
    </tr>
  </thead>
  <tbody align="left"  valign="middle">
    <tr>
      <td style="font-weight: bold;">
        As a Billed Entity
      </td>
      <td>
        <%:Html.ProfileField("IsPdfAsOtherOutputAsBilledEntity", null, SessionUtil.UserCategory, ControlType.CheckBox, new Dictionary<string, object> { { "id", "IsPdfAsOtherOutputAsBilledEntity" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
{
  FieldId = "IsPdfAsOtherOutputAsBilledEntity",
  FieldName = "IsPdfAsOtherOutputAsBilledEntity",
  FieldType = 2,
  CurrentValue = Model.IsPdfAsOtherOutputAsBilledEntity != null ? Model.IsPdfAsOtherOutputAsBilledEntity.ToString() : string.Empty,
  FutureValue = Model.IsPdfAsOtherOutputAsBilledEntityFutureValue != null ? Model.IsPdfAsOtherOutputAsBilledEntityFutureValue.ToString() : string.Empty,
  HasFuturePeriod = false,
  FutureDate = Model.IsPdfAsOtherOutputAsBilledEntityFutureDate != null ? Model.IsPdfAsOtherOutputAsBilledEntityFutureDate.ToString() : string.Empty}, null, true,true)%>
      </td>
      <td>
        <%:Html.ProfileField("IsDetailListingAsOtherOutputAsBilledEntity", null, SessionUtil.UserCategory, ControlType.CheckBox, new Dictionary<string, object> { { "id", "IsDetailListingAsOtherOutputAsBilledEntity" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
{
  FieldId = "IsDetailListingAsOtherOutputAsBilledEntity",
  FieldName = "IsDetailListingAsOtherOutputAsBilledEntity",
  FieldType = 2,
  CurrentValue = Model.IsDetailListingAsOtherOutputAsBilledEntity != null ? Model.IsDetailListingAsOtherOutputAsBilledEntity.ToString() : string.Empty,
  FutureValue = Model.IsDetailListingAsOtherOutputAsBilledEntityFutureValue != null ? Model.IsDetailListingAsOtherOutputAsBilledEntityFutureValue.ToString() : string.Empty,
  HasFuturePeriod = false,
  FutureDate = Model.IsDetailListingAsOtherOutputAsBilledEntityFutureDate != null ? Model.IsDetailListingAsOtherOutputAsBilledEntityFutureDate.ToString() : string.Empty
}, null, true, true)%>
      </td>
      <td>
        <%:Html.ProfileField("IsSuppDocAsOtherOutputAsBilledEntity", null, SessionUtil.UserCategory, ControlType.CheckBox, new Dictionary<string, object> { { "id", "IsSuppDocAsOtherOutputAsBilledEntity" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
{
  FieldId = "IsSuppDocAsOtherOutputAsBilledEntity",
  FieldName = "IsSuppDocAsOtherOutputAsBilledEntity",
  FieldType = 2,
  CurrentValue = Model.IsSuppDocAsOtherOutputAsBilledEntity != null ? Model.IsSuppDocAsOtherOutputAsBilledEntity.ToString() : string.Empty,
  FutureValue = Model.IsSuppDocAsOtherOutputAsBilledEntityFutureValue != null ? Model.IsSuppDocAsOtherOutputAsBilledEntityFutureValue.ToString() : string.Empty,
  HasFuturePeriod = false,
  FutureDate = Model.IsSuppDocAsOtherOutputAsBilledEntityFutureDate != null ? Model.IsSuppDocAsOtherOutputAsBilledEntityFutureDate.ToString() : string.Empty
}, null, true, true)%>
      </td>
      <% if (ViewData["TabType"] == "pax" || ViewData["TabType"] == "cgo")
         {%>
      <td>
        <%:Html.ProfileField("IsMemoAsOtherOutputAsBilledEntity", null, SessionUtil.UserCategory, ControlType.CheckBox, new Dictionary<string, object> { { "id", "IsMemoAsOtherOutputAsBilledEntity" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
{
  FieldId = "IsMemoAsOtherOutputAsBilledEntity",
  FieldName = "IsMemoAsOtherOutputAsBilledEntity",
  FieldType = 2,
  CurrentValue = Model.IsMemoAsOtherOutputAsBilledEntity != null ? Model.IsMemoAsOtherOutputAsBilledEntity.ToString() : string.Empty,
  FutureValue = Model.IsMemoAsOtherOutputAsBilledEntityFutureValue != null ? Model.IsMemoAsOtherOutputAsBilledEntityFutureValue.ToString() : string.Empty,
  HasFuturePeriod = false,
  FutureDate = Model.IsMemoAsOtherOutputAsBilledEntityFutureDate != null ? Model.IsMemoAsOtherOutputAsBilledEntityFutureDate.ToString() : string.Empty
}, null, true, true)%>
      </td>
      <%
         }%>
      <td>
        <%:Html.ProfileField("IsDsFileAsOtherOutputAsBilledEntity", null, SessionUtil.UserCategory, ControlType.CheckBox, new Dictionary<string, object> { { "id", "IsDsFileAsOtherOutputAsBilledEntity" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
{
  FieldId = "IsDsFileAsOtherOutputAsBilledEntity",
  FieldName = "IsDsFileAsOtherOutputAsBilledEntity",
  FieldType = 2,
  CurrentValue = Model.IsDsFileAsOtherOutputAsBilledEntity != null ? Model.IsDsFileAsOtherOutputAsBilledEntity.ToString() : string.Empty,
  FutureValue = Model.IsDsFileAsOtherOutputAsBilledEntityFutureValue != null ? Model.IsDsFileAsOtherOutputAsBilledEntityFutureValue.ToString() : string.Empty,
  HasFuturePeriod = false,
  FutureDate = Model.IsDsFileAsOtherOutputAsBilledEntityFutureDate != null ? Model.IsDsFileAsOtherOutputAsBilledEntityFutureDate.ToString() : string.Empty
}, null, true, true)%>
      </td>
    </tr>
    <tr>
      <td style="font-weight: bold;">
        As a Billing Entity
      </td>
      <td>
        <%:Html.ProfileField("IsPdfAsOtherOutputAsBillingEntity", null, SessionUtil.UserCategory, ControlType.CheckBox, new Dictionary<string, object> { { "id", "IsPdfAsOtherOutputAsBillingEntity" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
{
  FieldId = "IsPdfAsOtherOutputAsBillingEntity",
  FieldName = "IsPdfAsOtherOutputAsBillingEntity",
  FieldType = 2,
  CurrentValue = Model.IsPdfAsOtherOutputAsBillingEntity != null ? Model.IsPdfAsOtherOutputAsBillingEntity.ToString() : string.Empty,
  FutureValue = Model.IsPdfAsOtherOutputAsBillingEntityFutureValue != null ? Model.IsPdfAsOtherOutputAsBillingEntityFutureValue.ToString() : string.Empty,
  HasFuturePeriod = false,
  FutureDate = Model.IsPdfAsOtherOutputAsBillingEntityFutureDate != null ? Model.IsPdfAsOtherOutputAsBillingEntityFutureDate.ToString() : string.Empty
}, null, true,true)%>
      </td>
      <td>
        <%:Html.ProfileField("IsDetailListingAsOtherOutputAsBillingEntity", null, SessionUtil.UserCategory, ControlType.CheckBox, new Dictionary<string, object> { { "id", "IsDetailListingAsOtherOutputAsBillingEntity" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
{
  FieldId = "IsDetailListingAsOtherOutputAsBillingEntity",
  FieldName = "IsDetailListingAsOtherOutputAsBillingEntity",
  FieldType = 2,
  CurrentValue = Model.IsDetailListingAsOtherOutputAsBillingEntity != null ? Model.IsDetailListingAsOtherOutputAsBillingEntity.ToString() : string.Empty,
  FutureValue = Model.IsDetailListingAsOtherOutputAsBillingEntityFutureValue != null ? Model.IsDetailListingAsOtherOutputAsBillingEntityFutureValue.ToString() : string.Empty,
  HasFuturePeriod = false,
  FutureDate = Model.IsDetailListingAsOtherOutputAsBillingEntityFutureDate != null ? Model.IsDetailListingAsOtherOutputAsBillingEntityFutureDate.ToString() : string.Empty
}, null, true, true)%>
      </td>
      <td>
      </td>
      <% if (ViewData["TabType"] == "pax" || ViewData["TabType"] == "cgo")
         {%>
      <td>
        <%:Html.ProfileField("IsMemoAsOtherOutputAsBillingEntity", null, SessionUtil.UserCategory, ControlType.CheckBox, new Dictionary<string, object> { { "id", "IsMemoAsOtherOutputAsBillingEntity" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
{
  FieldId = "IsMemoAsOtherOutputAsBillingEntity",
  FieldName = "IsMemoAsOtherOutputAsBillingEntity",
  FieldType = 2,
  CurrentValue = Model.IsMemoAsOtherOutputAsBillingEntity != null ? Model.IsMemoAsOtherOutputAsBillingEntity.ToString() : string.Empty,
  FutureValue = Model.IsMemoAsOtherOutputAsBillingEntityFutureValue != null ? Model.IsMemoAsOtherOutputAsBillingEntityFutureValue.ToString() : string.Empty,
  HasFuturePeriod = false,
  FutureDate = Model.IsMemoAsOtherOutputAsBillingEntityFutureDate != null ? Model.IsMemoAsOtherOutputAsBillingEntityFutureDate.ToString() : string.Empty,
}, null, true, true)%>
      </td>
      <%
         }%>
      <td>
        <%:Html.ProfileField("IsDsFileAsOtherOutputAsBillingEntity", null, SessionUtil.UserCategory, ControlType.CheckBox, new Dictionary<string, object> { { "id", "IsDsFileAsOtherOutputAsBillingEntity" }, { "class", "currentFieldValue" } }, null, new FutureUpdate
{
  FieldId = "IsDsFileAsOtherOutputAsBillingEntity",
  FieldName = "IsDsFileAsOtherOutputAsBillingEntity",
  FieldType = 2,
  CurrentValue = Model.IsDsFileAsOtherOutputAsBillingEntity != null ? Model.IsDsFileAsOtherOutputAsBillingEntity.ToString() : string.Empty,
  FutureValue = Model.IsDsFileAsOtherOutputAsBillingEntityFutureValue != null ? Model.IsDsFileAsOtherOutputAsBillingEntityFutureValue.ToString() : string.Empty,
  HasFuturePeriod = false,
  FutureDate = Model.IsDsFileAsOtherOutputAsBillingEntityFutureDate != null ? Model.IsDsFileAsOtherOutputAsBillingEntityFutureDate.ToString() : string.Empty,
}, null, true, true)%>
      </td>
    </tr>
  </tbody>
</table>

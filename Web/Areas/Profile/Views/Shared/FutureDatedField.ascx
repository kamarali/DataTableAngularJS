<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.FutureUpdate>" %>
<%
  switch (Model.FieldType)
  {
    case 1:
%>
<%: Html.TextBox(Model.FieldName, Model.CurrentValue, Model.HtmlAttributes)%>
<%
  break;

    case 2:    
%>
<%: Html.CheckBox(Model.FieldName, bool.Parse(Model.CurrentValue), Model.HtmlAttributes)%>
<%
  break;
    case 3:
%>
Date Picker TBD
<%
  break;

    case 4:
%>
<%: Html.TextBox(Model.FieldName, Model.CurrentValue, Model.HtmlAttributes)%>
<%
  break;

    case 5:    
%>
<%: Html.TextArea(Model.FieldName, Model.CurrentValue,Model.HtmlAttributes)%>
<%
  break;

    case 6:
%>
<%: Html.TextBox(Model.FieldName, Model.CurrentValue, Model.HtmlAttributes)%>
<%
  break;

    case 7:    
%>
<%: Html.TextBox(Model.FieldName, Model.CurrentValue, Model.HtmlAttributes)%>
<%
  break;

     case 8:    
%>
<%: Html.CountryDropdownList(Model.FieldName, Model.CurrentValue,Model.HtmlAttributes)%>
<%
  break;

     case 9:    
%>
<%: Html.MigrationStatusDropdownList(Model.FieldName, Model.CurrentValue, Model.HtmlAttributes)%>
<%
  break;

     case 10:    
%>
<%: Html.RejectionOnValidatonFailureDropdownList(Model.FieldName, Model.CurrentValue, Model.HtmlAttributes)%>
<%
  break;

        case 11:    
%>
<%: Html.IchZoneDropdownList(Model.FieldName, Model.CurrentValue, Model.HtmlAttributes)%>
<%
  break;

    case 12:    
%>
<%: Html.IchCategoryDropdownList(Model.FieldName, Model.CurrentValue, Model.HtmlAttributes)%>
<%
  break;

         case 13:    
%>
<%: Html.SamplingCareerTypeDropdownList(Model.FieldName, Model.CurrentValue, Model.HtmlAttributes)%>
<%
  break;

  case 14:    
%>
<%: Html.CurrencyDropdownList(Model.FieldName, Model.CurrentValue, (IDictionary<string, object>)Model.HtmlAttributes)%>
<%
  break;

  case 15:    
%>
<%: Html.AggregatedTypeDropdownList(Model.FieldName, Model.CurrentValue, Model.HtmlAttributes)%>
<%
  break;
  case 16:    
%>
<%: Html.IchMembershipStatusDropdownList(Model.FieldName, Model.CurrentValue, Model.HtmlAttributes)%>
<%
  break;
  case 17:    
%>
<%: Html.IchMembershipStatusDropdownList(Model.FieldName, Model.CurrentValue, Model.HtmlAttributes)%>
<%
  break;
  case 18:
%>
<%: Html.MembershipStatusDropdownList(Model.FieldName, Model.CurrentValue, (IDictionary<string, object>)Model.HtmlAttributes)%>
<%
  break;
  } %>
<%if ((Model.FieldType == 16) || (Model.FieldType == 17) || (Model.FieldType == 18))
  {%>
<a class="statusEditLink hidden ignoredirty" onclick="return popupFutureUpdateDialog('#<%:Model.FieldId %>', <%: Model.FieldType %>, <%: Model.HasFuturePeriod ? 1 : 0 %>, '<%: Url.Content("~/Content/Images/calendar.gif") %>');"
  href="#">Edit...</a>
<%}%>
<%else if ((Model.FieldType == 2) && (Model.FieldId == "IsParticipateInAutoBilling"))
  {%>
<a class="autoBillingEditLink hidden ignoredirty" onclick="return popupFutureUpdateDialog('#<%:Model.FieldId%>', <%:Model.FieldType%>, <%:Model.HasFuturePeriod ? 1 : 0%>, '<%:Url.Content("~/Content/Images/calendar.gif")%>');"
  href="#">Edit...</a>
<%
  }%>
<%else if ((Model.FieldType == 2) && (Model.FieldId == "IsConsolidatedProvisionalBillingFileRequired"))
  {%>
<a class="provBillingFileEditLink hidden ignoredirty" onclick="return popupFutureUpdateDialog('#<%:Model.FieldId%>', <%:Model.FieldType%>, <%:Model.HasFuturePeriod ? 1 : 0%>, '<%:Url.Content("~/Content/Images/calendar.gif")%>');"
  href="#">Edit...</a>
<%
  }%>
<%else if ((Model.FieldType == 2) && (Model.FieldId == "ISUatpInvIgnoreFromDsproc"))
  {%>
<a class="ignoreUATPEditLink hidden ignoredirty" onclick="return popupFutureUpdateDialog('#<%:Model.FieldId%>', <%:Model.FieldType%>, <%:Model.HasFuturePeriod ? 1 : 0%>, '<%:Url.Content("~/Content/Images/calendar.gif")%>');"
  href="#">Edit...</a>
<%
  }%>
<%else if ((Model.FieldType == 2) && (Model.FieldId == "IsBillingDataSubmittedByThirdPartiesRequired"))
  {%>
<a class="BillingDataSubmittedByThirdPartyEditLink hidden ignoredirty" onclick="return popupFutureUpdateDialog('#<%:Model.FieldId%>', <%:Model.FieldType%>, <%:Model.HasFuturePeriod ? 1 : 0%>, '<%:Url.Content("~/Content/Images/calendar.gif")%>');"
  href="#">Edit...</a>
<%
  }%>
<%else if ((Model.FieldType == 1) && (Model.FieldId == "CutOffTime"))
  {%>
<a class="cuttOffTimeEditLink hidden ignoredirty" onclick="return popupFutureUpdateDialog('#<%:Model.FieldId%>', <%:Model.FieldType%>, <%:Model.HasFuturePeriod ? 1 : 0%>, '<%:Url.Content("~/Content/Images/calendar.gif")%>');"
  href="#">Edit...</a>
<%
  }%>

  <%else if ((Model.FieldType == 14) && (Model.FieldId == "ListingCurrencyId"))
  {%>
<a class="listingCurrencyEditLink hidden ignoredirty" onclick="return popupFutureUpdateDialog('#<%:Model.FieldId%>', <%:Model.FieldType%>, <%:Model.HasFuturePeriod ? 1 : 0%>, '<%:Url.Content("~/Content/Images/calendar.gif")%>');"
  href="#">Edit...</a>
<%
  }%>

   <%else if ((Model.FieldType == 2) && (Model.FieldId == "IsIsrFileRequired"))
  {%>
<a class="isIsrFileRequiredEditLink hidden ignoredirty" onclick="return popupFutureUpdateDialog('#<%:Model.FieldId%>', <%:Model.FieldType%>, <%:Model.HasFuturePeriod ? 1 : 0%>, '<%:Url.Content("~/Content/Images/calendar.gif")%>');"
  href="#">Edit...</a>
<%
  }%>
    
<%else if ((Model.FieldType != 6) && (Model.FieldType != 7))
  {%>
<a class="futureEditLink hidden ignoredirty" onclick="return popupFutureUpdateDialog('#<%:Model.FieldId%>', <%:Model.FieldType%>, <%:Model.HasFuturePeriod ? 1 : 0%>, '<%:Url.Content("~/Content/Images/calendar.gif")%>');"
  href="#">Edit...</a>
<%
  }%>


<%: Html.Hidden(Model.FieldName + "Old", Model.CurrentValue)%>
<%: Html.Hidden(Model.FieldName + "FutureValue", Model.FutureValue)%>
<%: Html.Hidden(Model.FieldName + "DisplayValue", Model.CurrentDisplayValue)%>
<%: Html.Hidden(Model.FieldName + "FutureDisplayValue", Model.FutureDisplayValue)%>
<%: Model.HasFuturePeriod ? Html.Hidden(Model.FieldName + "FuturePeriod", Model.FuturePeriod) : Html.Hidden(Model.FieldId + "FutureDate", Model.FutureDate)%>
<%if (!string.IsNullOrEmpty(Model.FutureValue))
  {%>
<image id="<%:Model.FieldId %>FutureDateInd" src="<%:Url.Content("~/Content/Images/Exclamation.gif") %>"
  onclick="return displayFutureUpdateDetails('#<%:Model.FieldId%>',<%: Model.HasFuturePeriod ? 1 : 0 %>,<%:Model.FieldType%>)" />
<%} %>
<%else
  {%>
<image class="hidden" id="<%:Model.FieldId %>FutureDateInd" src="<%:Url.Content("~/Content/Images/Exclamation.gif") %>"
  onclick="return displayFutureUpdateDetails('#<%:Model.FieldId%>',<%: Model.HasFuturePeriod ? 1 : 0 %>,<%:Model.FieldType%>)" />
<%} %>

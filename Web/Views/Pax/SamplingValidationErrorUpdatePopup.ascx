<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.ValidationErrorCorrection>" %>
<script type="text/javascript">
    $(document).ready(function () {

        SamplingValidateUpdatePopup();


    });
  </script>
  <link href="<%:Url.Content("~/Content/superfish.css")%>" rel="stylesheet" type="text/css" />
  <link href="<%:Url.Content("~/Content/ui-sis/jquery-ui-1.8.12.custom.css")%>" rel="stylesheet" type="text/css" />
  <link href="<%:Url.Content("~/Content/jquery.autocomplete.css")%>" rel="stylesheet" type="text/css" />
  <link href="<%:Url.Content("~/Content/Site.css")%>" rel="stylesheet" type="text/css" />
  <link href="<%:Url.Content("~/Content/ui.jqgrid.css")%>" rel="stylesheet" type="text/css" />
  <link href="<%:Url.Content("~/Content/Forms.css")%>" rel="stylesheet" type="text/css" />
  <!--[if IE 6]>
  <link href="<%:Url.Content("~/Content/Forms-IE6.css")%>" rel="stylesheet" type="text/css" />
  <![endif]-->
  <!--[if IE 7]>
  <link href="<%:Url.Content("~/Content/Forms-IE7.css")%>" rel="stylesheet" type="text/css" />
  <![endif]-->
  <![if !IE]>
  <link href="<%:Url.Content("~/Content/Forms-FF.css")%>" rel="stylesheet" type="text/css" />
  <![endif]>
<h2>
  Update Validation Error</h2>
  <form id="SamplingUpdateValidationForm" action="" method="post">
  <div class="searchCriteria">
<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
        <div>
              <div style="width:400px;">
       
          <input id="lblFileName" name="lblFileName" readOnly="True" style="width:300px;border:&#32;none;&#32;background-color:#f5faff" type="text" value="File Name:" />
          <%:Html.TextBoxFor(model => model.SamplingUpdateFileName, new { @readOnly = true, style = "width:300px;border: none;  background-color:#f5faff" })%>
       </div>
       <div>
            <input id="lblExceptionCode" name="lblExceptionCode" readOnly="True" type="text" value="Exception Code:" style="border:&#32;none;&#32;background-color:#f5faff"/>
          <%:Html.TextBoxFor(invoice => invoice.SamplingUpdateExceptionCode, new { @readOnly = true, style = "border: none; background-color:#f5faff" })%>
            </div>
            <div >
              <input id="lblErrorDescription" name="lblErrorDescription" readOnly="True" type="text" value="Error Description:" style="border:&#32;none;&#32;background-color:#f5faff"/>
            
              <%:Html.TextBoxFor(invoice => invoice.SamplingErrorDescription, new { @readOnly = true, style = "width:300px;border: none;  background-color:#f5faff" })%>
            </div>
            
        </div>
        <div>
            <div style="width:400px">
         <input id="lblFieldName" name="lblFieldName" readOnly="True" style="width:300px;border:&#32;none;&#32;background-color:#f5faff" type="text" value="Field Name:" />
          <%:Html.TextBoxFor(invoice => invoice.SamplingFieldName, new { @readOnly = true, style = "width:300px;border: none; background-color:#f5faff" })%>
        </div>
        <div>
          <input id="lblFieldValue" name="lblFieldValue" readOnly="True" style="width:300px;border:&#32;none;&#32;background-color:#f5faff" type="text" value="Field Value:"/>
          <%:Html.TextBoxFor(invoice => invoice.SamplingFieldValue, new { @readOnly = true, style = "border: none; background-color:#f5faff" })%>
        </div>
        <div>
          <label><span>*</span>
            New Value:</label>
          <%:Html.TextBoxFor(invoice => invoice.SamplingNewValue, new { style = "width:300px;", maxLength = 500 })%>
         
        </div>
           <%:Html.HiddenFor(model => model.BatchUpdateAllowed)%>
           <%:Html.HiddenFor(model => model.ExceptionSummaryId)%>
           <%:Html.HiddenFor(model => model.ExceptionDetailId)%>
            <%:Html.HiddenFor(model => model.ErrorLevel)%>
            <%:Html.HiddenFor(model => model.PkReferenceId)%>
           <!--//SCP252342 - SRM: ICH invoice in ready for billing status-->
           <%:Html.HiddenFor(model => model.LastUpdatedOn)%>
        </div> 
    </div>
    <div class="clear">
  </div>
</div>
 <div>

            <div class="buttonContainer" align="left">
                <input class="primaryButton" type="button" value="Update" onclick="javascript:Samplingupdatebuttonclick(0);"/>  
                <input class="primaryButton" type="button"  value="Batch Update" onclick="javascript:SamplingBatchUpdatebuttonclick();" id="BatchUpdateButtonAllowed"/>
                <input class="secondaryButton" type="button"  value="Close" onclick="javascript:SamplingcloseUpdate();" id="SamplingErrorUpdateClose"/>

            </div>
        </div> 
</div>
</form>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.ValidationErrorCorrection>" %>
<script type="text/javascript">
    $(document).ready(function () {

        ValidateUpdatePopup();
     

    });
  </script>
<h2>
  Update Validation Error</h2>
  <form id="UpdateValidationForm" action="" method="post">
  <div class="searchCriteria">
<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
        <div>
              <div style="width:350px;">
        <label>
          File Name:</label>
          <%:Html.TextBoxFor(model => model.UpdateFileName, new { @readOnly = true, style = "width:300px;border: none;  background-color:#f5faff",tabindex=-1 })%>
       </div>
       <div>
            <label>
            Exception Code:</label>
            
          <%:Html.TextBoxFor(invoice => invoice.UpdateExceptionCode, new { @readOnly = true, style = "border: none; background-color:#f5faff", tabindex = -1 })%>
            </div>
            <div >
              <label>
                Error Description:</label>
            
              <%:Html.TextAreaFor(invoice => invoice.ErrorDescription, new { @readOnly = true, style = "width:300px;border: none;", tabindex = -1 })%>
            </div>
            
        </div>
        <div>
            <div style="width:350px">
          <label>
            Field Name:</label>
          <%:Html.TextBoxFor(invoice => invoice.FieldName, new { @readOnly = true, style = "width:300px;border: none; background-color:#f5faff", tabindex = -1 })%>
        </div>
        <div>
          <label>
            Field Value:</label>
          <%:Html.TextBoxFor(invoice => invoice.FieldValue, new { @readOnly = true, style = "width:170px;border: none; background-color:#f5faff", tabindex = -1 })%>
        </div>
        <div>
          <label><span>*</span>
            New Value:</label>
          <%:Html.TextBoxFor(invoice => invoice.NewValue, new { @class = "upperCase", style = "width:300px;", maxLength = 500 ,tabindex=1})%>
          
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
                <input class="primaryButton" id="UpdateButton" type="button" value="Update" onclick="javascript:updatebuttonclick(0);" />  
                <input class="primaryButton" type="button"  value="Batch Update" onclick="javascript:BatchUpdatebuttonclick();" id="BatchUpdateButtonAllowed"/>
                <input class="secondaryButton" type="button"  value="Close" onclick="javascript:closeUpdate();" />

            </div>
        </div> 
</div>
</form>
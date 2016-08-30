<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.ValidationErrorCorrection>" %>
<script type="text/javascript">
    $(document).ready(function () {

        ValidateUpdatePopup();

    });

    $(function () {
        
        $("#tabs").tabs({

            cache: false
        });
    });
</script>
<h2>
    Update Validation Error</h2>
<form id="UpdateValidationForm" action="" method="post">
<div class="searchCriteria">
    <div class="solidBox dataEntry">
        <div class="fieldContainer horizontalFlow">
            <div class="">
                <div style="width: 400px;">
                <label for="lblFileName">File Name: </label>
                    <%:Html.TextBoxFor(model => model.UpdateFileName, new { @readOnly = true, style = "width:300px;border: none;  background-color:#f5faff", tabindex=-1 })%>
                </div>
                <div>
                <label for="lblExceptionCode">Exception Code:</label>
                    <%:Html.TextBoxFor(invoice => invoice.UpdateExceptionCode, new { @readOnly = true, style = "border: none; background-color:#f5faff", tabindex = -1 })%>
                </div>
                <div>
                <label for="lblErrorDescription">Error Description:</label>
                    <%:Html.TextBoxFor(invoice => invoice.ErrorDescription, new { @readOnly = true, style = "width:300px;border: none;  background-color:#f5faff", tabindex = -1 })%>
                </div>
            </div>
            <div>
                <div style="width: 400px">
                <label for="lblFieldName">Field Name:</label>
                    <%:Html.TextBoxFor(invoice => invoice.FieldName, new { @readOnly = true, style = "width:300px;border: none; background-color:#f5faff", tabindex = -1 })%>
                </div>
                <div>
                <label for="lblFieldValue">Field Value:</label>
                    <%:Html.TextBoxFor(invoice => invoice.FieldValue, new { @readOnly = true, style = "border: none; background-color:#f5faff", tabindex = -1 })%>
                </div>
                <div>
                    <label for="lblNewValue"><span>*</span>New Value:</label>
                    <%:Html.TextBoxFor(invoice => invoice.NewValue, new { @class = "upperCase", id = "NewValue", style = "width:300px;", maxLength = 500, tabindex = 1, autofocus = "autofocus" })%>
                </div>
                <%:Html.HiddenFor(model => model.BatchUpdateAllowed)%>
                <%:Html.HiddenFor(model => model.ExceptionSummaryId)%>
                <%:Html.HiddenFor(model => model.ExceptionDetailId)%>
                <%:Html.HiddenFor(model => model.ErrorLevel)%>
                <%:Html.HiddenFor(model => model.PkReferenceId)%>
                 <!-- ID : 252342 - SRM: ICH invoice in ready for billing status-->
               <%:Html.HiddenFor(model => model.LastUpdatedOn)%>
            </div>
        </div>
        <div class="clear">
        </div>
    </div>
    <div class="buttonContainer">
        <input class="primaryButton" style="border: 1px solid #ca8402; background: url() #ef9c00 no-repeat;"
            type="button" value="Update" onclick="javascript:updatebuttonclick(0);" tabindex='2'/>
        <input class="primaryButton" type="button" value="Batch Update" onclick="javascript:BatchUpdatebuttonclick();"
            id="BatchUpdateButtonAlloweds" tabindex='3' />
        <input class="secondaryButton" type="button" value="Close" onclick="javascript:closeUpdate();" id="invoiceUpdateClose" tabindex='4'/>
    </div>
</div>
</form>

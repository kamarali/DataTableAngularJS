<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.ValidationErrorCorrection>" %>

<%:Html.TextBoxFor(model => model.BatchUpdatedFieldCount, new { @readOnly = true, style = "width:20px; border: none;" })%> records will be updated.

<div>

            <div class="buttonContainer" align="left">
                <input class="primaryButton" type="submit" value="Ok" onclick="javascript:updatebuttonclick(1);" />  
                <input class="secondaryButton" type="button"  value="Cancel" onclick="javascript:closeBatchUpdate();" />

            </div>
        </div> 
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<form id="formProrateSlip" action="">
<%: Html.TextArea(ControlIdConstants.ProrateSlipeControl, string.Empty, 20, 96, new { @class = "notValidCharsTextarea" })%>
<div class="buttonContainer">
  <input class="secondaryButton" type="button" value="OK" onclick="validateProrateSlip();" />
  <input class="secondaryButton" type="button" value="Close" onclick="$('#divProrateSlip').dialog('close');" />
</div>
</form>

<%--TODO:Move this function to appropriate js file--%>
<script type="text/javascript">
  function InitializeProrateSlip(maxCharactersInLine, maxLines) {
    $('#ProrateSlipDetailsText').val($("#hiddenprorateSlip").val());
    $('#hiddenprorateSlip').removeClass("validateCharacters");

    $("#formProrateSlip").validate({
      rules: {
        ProrateSlipDetailsText: {
          allowedCharactersForTextAreaFields: true,
          ValidatePaxTextareaField: [maxCharactersInLine, maxLines]
        }
      },
      messages: {
        ProrateSlipDetailsText: "Invalid prorate slip value."
      }
    });
  }
  function validateProrateSlip() {
    if ($("#formProrateSlip").validate().form()) {
      $('#hiddenprorateSlip').val($("#ProrateSlipDetailsText").val());
      $("#hiddenprorateSlip").change();
      $('#divProrateSlip').dialog('close');
    }
  }
</script>
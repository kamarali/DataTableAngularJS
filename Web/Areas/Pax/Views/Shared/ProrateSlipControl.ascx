<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<form id="formProrateSlip" action="">
  <%: Html.TextArea(ControlIdConstants.ProrateSlipeControl, string.Empty, 20, 180, new { @class = "notValidCharsTextarea" })%>
<div class="buttonContainer">
  <input class="secondaryButton" type="button" value="OK" onclick="validateProrateSlip();" />
  <input class="secondaryButton" type="button" value="Close" onclick="$('#divProrateSlip').dialog('close');" />
</div>
</form>

<%--TODO:Move this function to appropriate js file--%>
<script type="text/javascript">
  function InitializeProrateSlip(maxCharactersInLine, maxLines) {


      var $ProrateSlipDetails = $('#ProrateSlipDetailsText');
      

      //$ProrateSlipDetails.removeAttr("disabled");
      //$ProrateSlipDetails.attr('readonly', 'readonly');
      
    $ProrateSlipDetails.val($("#hiddenprorateSlip").val());

    // Below done for Firefox. Firefox inserts a new line in the beginning.
    if ($ProrateSlipDetails.val() != null) {
      var value = $ProrateSlipDetails.val().replace(/\r\n|\r|\n/, '');
      $ProrateSlipDetails.val(value);
    }

    // Get array containing 80 character sized blocks.
    var prorateSlipInChunks = chunk($ProrateSlipDetails.val(), maxCharactersInLine);
    // Join the array into a string with newline after every 80 characters.
    if (prorateSlipInChunks != null)
      $ProrateSlipDetails.val(prorateSlipInChunks.join('\n'));

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

  // Check if 80 characters reached in one line of text area. If yes, take the current character to next line.
  $('#ProrateSlipDetailsText').bind('keypress', { maxColChars: maxCharactersInLine }, checkProrateSlipLinelength);
}
  function validateProrateSlip() {
    if ($("#formProrateSlip").validate().form()) {
      $('#hiddenprorateSlip').val($("#ProrateSlipDetailsText").val());
      $("#hiddenprorateSlip").change();
      $('#divProrateSlip').dialog('close');
    }
  }

  // Function to insert new line character if 80 characters is reached in a line of textarea.
  function checkProrateSlipLinelength(event) {    
    var data = event.data;
    var value = event.currentTarget.value;
    var maxCharsInLine = data.maxColChars;

    var key = event.keyCode ? event.keyCode : event.which ? event.which : event.charCode;
    
    //Get lines entered for field
    var lines = value.split(/\r\n|\r|\n/);
    // get the last line.
    var lastLineIndex = lines.length -1;

    if (lines[lastLineIndex] != null) {
    // 13: Keycode for Return.
      if (lines[lastLineIndex].length == maxCharsInLine && key != 13) {
        event.currentTarget.value += '\n';
      }
    }

    return;
  }

  function chunk(text, n) {
    var ret = [];
    for (var i = 0, len = text.length; i < len; i += n) {
      ret.push(text.substr(i, n))
    }
    return ret;
  }

</script>
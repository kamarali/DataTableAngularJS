<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div id="linkedRecords">
  <h2>
    Below records were found. Please select one.</h2>
  <div>
    <table id="linkedRecordsGrid">
    </table>
  </div>
  <div class="buttonContainer">
    <input class="secondaryButton" type="button" value="OK" onclick="onLinkingDialogClose()" />
  </div>
</div>

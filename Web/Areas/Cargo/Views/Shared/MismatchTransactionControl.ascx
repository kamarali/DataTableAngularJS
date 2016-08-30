<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

    <div>
    <%Html.RenderPartial("MismatchedTransactionsSearchControl", ViewData[ViewDataConstants.MismatchTransactionModel]); %>
    </div>
    <div>
    <table id="MismatchTransactionGrid"></table>
    </div>
    <div class="buttonContainer">
  <input class="primaryButton" type="submit" value="Update" id="UpdateButton" />
  <input class="secondaryButton" type="button" value="Close" id="CloseButton" />
</div>
   
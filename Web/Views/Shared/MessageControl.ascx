<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="errorContainer" class="hidden">
  <%
    Html.RenderPartial("ValidationErrorControl");%>
</div>
<%
  var successMessage = (string)(TempData[ViewDataConstants.SuccessMessage] ?? ViewData[ViewDataConstants.SuccessMessage]);
  var errorMessage = (string)(TempData[ViewDataConstants.ErrorMessage] ?? ViewData[ViewDataConstants.ErrorMessage]);

  if (!string.IsNullOrEmpty(successMessage))
  {%>
<div id="successMessageContainer" class="serverMessage serverSuccessMessage">
  <div style="float: left; width: 20px">
    <img src='<%:Url.Content("~/Content/Images/success_message.png")%>' alt="Success" />
  </div>

  <!-- SCP85837: PAX CGO Sequence Number -->
  <%if (successMessage.Contains("¥"))
      {
        //remove useless '-' from message.
          var sucMsg = successMessage.Split('¥');
        foreach (var suc in sucMsg)
        {%>
        <div style="margin-left: 23px">
            <%: suc%>
          </div>
        <%}                
      }
      else
      {%>
         <div style="margin-left: 23px">
    <%: successMessage%>
  </div>
      <%}%>
  
</div>
<%}
  if (!string.IsNullOrEmpty(errorMessage))
  {%>
<div id="errorMessageContainer" class="serverMessage serverErrorMessage">
  <div style="float: left; width: 20px">
    <img src='<%:Url.Content("~/Content/Images/error_message.png")%>' alt="Error" />
  </div>
  <%if (errorMessage.Contains("¥"))
      {
        //remove useless '-' from message.
        var errMsg = errorMessage.Replace("- Error in the application.",string.Empty).Split('¥');
        foreach (var err in errMsg)
        {%>
        <div style="margin-left: 23px">
            <%: err%>
          </div>
        <%}                
      }
      else
      {%>
          <div style="margin-left: 23px">
            <%: errorMessage%>
          </div>
      <%}%>

</div>
<%}%>
<div id="clientSuccessMessageContainer" class="hidden clientMessage clientSuccessMessage">
  <div style="float: left; width: 20px">
    <img src='<%:Url.Content("~/Content/Images/success_message.png")%>' alt="Success" />
  </div>
  <div style="margin-left: 23px" id="clientSuccessMessage">
  </div>
</div>
<div id="clientErrorMessageContainer" class="hidden clientMessage clientErrorMessage">
  <div style="float: left; width: 20px">
    <img src='<%:Url.Content("~/Content/Images/error_message.png")%>' alt="Error" />
  </div>
  <div style="margin-left: 23px" id="clientErrorMessage">
  </div>
</div>
<div id="pendingChangesDialog" style="display: none;">
  <p>You have unsaved changes on this page. Do you want to continue without saving?</p>
  <input type="button" value="Proceed" id="proceedNavigation" class="ignoredirty continue" />
  <input type="button" value="Cancel" id="cancelNavigation" class="ignoredirty cancel" />
</div>
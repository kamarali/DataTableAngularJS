<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div id="gridSuccessMessageContainer" class="serverMessage serverSuccessMessage hidden">
  <span>
    <img style="border-style: none; vertical-align: middle" src='<%:Url.Content("~/Content/Images/success_message.png")%>'
      alt="Success" /></span>
</div>
<div id="gridErrorMessageContainer" class="serverMessage serverErrorMessage hidden">
  <span>
    <img style="border-style: none; vertical-align: middle" src='<%:Url.Content("~/Content/Images/error_message.png")%>'
      alt="Error" /></span>
</div>

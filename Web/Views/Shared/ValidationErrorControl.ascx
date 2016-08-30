<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div class="errorMessage">
  <label>
    Please review and correct the highlighted fields. Move the mouse over <img style="border-style: none; vertical-align: middle" src='<%: Url.Content("~/Content/Images/error_icon.png") %>'
      alt="Error" /> to see error details for each field.</label>
</div>

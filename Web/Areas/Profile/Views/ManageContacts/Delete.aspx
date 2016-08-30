<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.ContactType>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Delete Contact Type
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Delete Contact Type</h2> 
 <% using (Html.BeginForm())
     {%>
  <%: Html.ValidationSummary(true) %>
  <fieldset>
    <legend>Contact Type</legend>
    <div class="editor-label">
      <label>
        Contact Type Name</label>
    </div>
    <div class="editor-field">
      <%: Html.TextBoxFor(contactType => contactType.ContactTypeName, new { @maxLength = 200 })%>
      <%: Html.ValidationMessageFor(contactType => contactType.ContactTypeName)%>
    </div>
    <div class="editor-label">
      <label>
        Required</label>
    </div>
    <div class="editor-field">
      <%: Html.CheckBoxFor(contactType => contactType.Required)%>
      <%: Html.ValidationMessageFor(contactType => contactType.Required)%>
    </div>
    <div class="editor-label">
      <label>
        Order No.</label>
    </div>
    <div class="editor-field">
      <%: Html.TextBoxFor(contactType => contactType.OrderNo, new { @maxLength = 4,@class="integer" })%>
      <%: Html.ValidationMessageFor(contactType => contactType.OrderNo)%>
    </div>
    <div class="editor-label">
      <label>
        Dependent Field</label>
    </div>
    <div class="editor-field">
      <%: Html.TextBoxFor(contactType => contactType.DependentField, new { @maxLength = 100 })%>
      <%: Html.ValidationMessageFor(contactType => contactType.DependentField)%>
    </div>
    <div class="editor-label">
      <label>
        Type</label>
    </div>
    <div class="editor-field">
      <%: Html.TextBoxFor(contactType => contactType.TypeId)%>
      <%: Html.ValidationMessageFor(contactType => contactType.TypeId)%>
    </div>
    <div class="editor-label">
      <label>
        Group</label>
    </div>
    <div class="editor-field">
      <%: Html.ContactTypeGroupDropdownListFor(contactType => contactType.GroupId)%>
      <%: Html.ValidationMessageFor(contactType => contactType.GroupId)%>
    </div> 
    <div class="editor-label">
      <label>
        Subgroup</label>
    </div>
    <div class="editor-field">
      <%: Html.ContactTypeSubGroupDropdownList(contactType => contactType.SubGroupId)%>
      <%: Html.ValidationMessageFor(contactType => contactType.SubGroupId)%>
    </div> 
    <div class="editor-label">
      <label>
        Is Active</label>
    </div>
    <div class="editor-field">
      <%: Html.CheckBoxFor(contactType => contactType.IsActive)%>
      <%: Html.ValidationMessageFor(contactType => contactType.IsActive)%>
    </div>
    <div class="horizontalFlow">
      <div class="editor-label">
        <label>
          Member</label>
      </div>
      <div class="editor-field">
        <%: Html.CheckBoxFor(contactType => contactType.Member)%>
        <%: Html.ValidationMessageFor(contactType => contactType.Member)%>
      </div>
      <div class="editor-label">
        <label>
          PAX</label>
      </div>
      <div class="editor-field">
        <%: Html.CheckBoxFor(contactType => contactType.Pax)%>
        <%: Html.ValidationMessageFor(contactType => contactType.Pax)%>
      </div>
      <div class="editor-label">
        <label>
          Cargo</label>
      </div>
      <div class="editor-field">
        <%: Html.CheckBoxFor(contactType => contactType.Cgo)%>
        <%: Html.ValidationMessageFor(contactType => contactType.Cgo)%>
      </div>
      <div class="editor-label">
        <label>
          Miscelleneous</label>
      </div>
      <div class="editor-field">
        <%: Html.CheckBoxFor(contactType => contactType.Misc)%>
        <%: Html.ValidationMessageFor(contactType => contactType.Misc)%>
      </div>
      <div class="editor-label">
        <label>
          UATP</label>
      </div>
      <div class="editor-field">
        <%: Html.CheckBoxFor(contactType => contactType.Uatp)%>
        <%: Html.ValidationMessageFor(contactType => contactType.Uatp)%>
      </div>
      <div class="editor-label">
        <label>
          ICH</label>
      </div>
      <div class="editor-field">
        <%: Html.CheckBoxFor(contactType => contactType.Ich)%>
        <%: Html.ValidationMessageFor(contactType => contactType.Ich)%>
      </div>
      <div class="editor-label">
        <label>
          ACH</label>
      </div>
      <div class="editor-field">
        <%: Html.CheckBoxFor(contactType => contactType.Ach)%>
        <%: Html.ValidationMessageFor(contactType => contactType.Ach)%>
      </div>
      <div class="editor-label">
        <label>
          Technical</label>
      </div>
     <div class="editor-field">
        <%: Html.CheckBoxFor(contactType => contactType.Technical)%>
        <%: Html.ValidationMessageFor(contactType => contactType.Technical)%>
      </div>
    </div>
    <div class="buttonContainer">
      <input type="button" value="Save" class="primaryButton" onclick="DeleteContactType('<%:Url.Action("Index", "ManageContacts", new { area = "Profile"}) %>')" />
       <input type="submit" value="hiddenSave" class="hidden"  id="hiddenSave" />
    </div>
  </fieldset>
  <% } %>
  <div>
    <%: Html.ActionLink("Back to List", "Index") %>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript">
  function DeleteContactType(redirectUrl) {
    if (confirm(" Are you sure you want to delete this contact type?")) {
      $("#hiddenSave").trigger("click");
    }
    else {
      window.location.href = redirectUrl;
    }

  }
</script>
</asp:Content>

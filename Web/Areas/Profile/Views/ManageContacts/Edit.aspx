<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MemberProfile.ContactType>" %>

<%@ Import Namespace="Iata.IS.Model.MemberProfile.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Profile and User Management :: Contact Type
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  
  <h1>
    Edit Contact Type</h1>
  <% using (Html.BeginForm("Edit", "ManageContacts", FormMethod.Post, new { @id = "ContactsMaster" }))
       {%>
       <%: Html.AntiForgeryToken() %>
  <%: Html.ValidationSummary(true) %>
  <fieldset class="solidBox dataEntry">
        <div class="fieldContainer horizontalFlow">
            <div>
                <div>
                    <label>
                        <span class="required">* </span>Contact Type Name</label>
                    <%: Html.TextBoxFor(contactType => contactType.ContactTypeName, new {@class="alphaNumericWithSpace", @maxLength = 200 })%>
                    <%: Html.ValidationMessageFor(contactType => contactType.ContactTypeName)%>
                </div>
                <div>
                    <label>
                        Required</label>
                    <%: Html.CheckBoxFor(contactType => contactType.Required)%>
                    <%: Html.ValidationMessageFor(contactType => contactType.Required)%>
                </div>
            </div>
            <div>
                <div>
                    <label>
                        Sequence No.</label>
                    <%: Html.TextBoxFor(contactType => contactType.SequenceNo, new { @maxLength = 3, @class = "integer" })%>
                    <%: Html.ValidationMessageFor(contactType => contactType.SequenceNo)%>
                </div>
                <div>
                    <label>
                        Type</label>
                    <%: Html.TextBoxFor(contactType => contactType.TypeOfContactType,new{@readonly="readonly"})%>
                    <%: Html.ValidationMessageFor(contactType => contactType.TypeId)%>
                </div>
            </div>
            <div class="bottomLine">
                <div>
                    <label>
                       <span class="required">* </span> Contact Type Group</label>
                    <%: Html.ContactTypeGroupDropdownListFor(model => model.GroupId)%>
                    <%: Html.ValidationMessageFor(contactType => contactType.GroupId)%>
                </div>
                <div>
                    <label>
                      <span class="required">* </span>  Contact Type Sub Group</label>
                    <%: Html.ContactTypeSubGroupDropdownList(model => model.SubGroupId)%>
                    <%: Html.ValidationMessageFor(contactType => contactType.SubGroupId)%>
                </div>
            </div>
            <div>
                <div>
                    <label>
                        Member</label>
                    <%: Html.RadioButton( "rbEditContactType","Member", Model.Member)%>
                </div>
                <div>
                    <label>
                        PAX</label>
                    <%: Html.RadioButton( "rbEditContactType","Pax" ,Model.Pax)%>
                </div>
            </div>
            <div>
                <div>
                    <label>
                        Cargo</label>
                    <%: Html.RadioButton("rbEditContactType", "Cgo", Model.Cgo)%>
                </div>
                <div>
                    <label>
                        Miscelleneous</label>
                    <%: Html.RadioButton("rbEditContactType", "Misc", Model.Misc)%>
                </div>
            </div>
            <div>
                <div>
                    <label>
                        UATP</label>
                    <%: Html.RadioButton("rbEditContactType", "Uatp", Model.Uatp)%>
                </div>
                <div>
                    <label>
                        ICH</label>
                    <%: Html.RadioButton("rbEditContactType", "Ich", Model.Ich)%>
                </div>
            </div>
            <div class="bottomLine">
                <div>
                    <label>
                        ACH</label>
                    <%: Html.RadioButton("rbEditContactType", "Ach", Model.Ach)%>
                </div>
            </div>
            <div>
                <div>
                    <label>
                        Allowed for ACH Ops Contact Reporting</label>
                    <%: Html.CheckBoxFor(contactType => contactType.AchOpsContactRpt)%>
                    <%: Html.ValidationMessageFor(contactType => contactType.AchOpsContactRpt)%>
                </div>
                <div>
                    <label>
                        Allowed for ICH Ops Contact Reporting</label>
                    <%: Html.CheckBoxFor(contactType => contactType.IchOpsConctactRpt)%>
                    <%: Html.ValidationMessageFor(contactType => contactType.IchOpsConctactRpt)%>
                </div>
            </div>
            <div class="bottomLine">
                <div>
                    <label>
                        Allowed for Other Members Contact Reporting</label>
                    <%: Html.CheckBoxFor(contactType => contactType.IsAccConctactRpt)%>
                    <%: Html.ValidationMessageFor(contactType => contactType.IsAccConctactRpt)%>
                </div>
                <div>
                    <label>
                        Allowed for Own Members Contact Reporting</label>
                    <%: Html.CheckBoxFor(contactType => contactType.IsAccOwnContactRpt)%>
                    <%: Html.ValidationMessageFor(contactType => contactType.IsAccOwnContactRpt)%>
                </div>
            </div>
            <div>
                <div>
                    <label>
                        Viewable Member</label>
                    <%: Html.CheckBoxFor(contactType => contactType.ViewableMem)%>
                    <%: Html.ValidationMessageFor(contactType => contactType.ViewableMem)%>
                </div>
                <div>
                    <label>
                        Editable Member</label>
                    <%: Html.CheckBoxFor(contactType => contactType.EditableMem)%>
                    <%: Html.ValidationMessageFor(contactType => contactType.ViewableMem)%>
                </div>
            </div>
            <div>
                <div>
                    <label>
                        Viewable SIS</label>
                    <%: Html.CheckBoxFor(contactType => contactType.ViewableSIS)%>
                    <%: Html.ValidationMessageFor(contactType => contactType.ViewableSIS)%>
                </div>
                <div>
                    <label>
                        Editable SIS</label>
                    <%: Html.CheckBoxFor(contactType => contactType.EditableSIS)%>
                    <%: Html.ValidationMessageFor(contactType => contactType.EditableSIS)%>
                </div>
            </div>
            <div>
                <div>
                    <label>
                        Viewable ACH</label>
                    <%: Html.CheckBoxFor(contactType => contactType.ViewableACH)%>
                    <%: Html.ValidationMessageFor(contactType => contactType.ViewableACH)%>
                </div>
                <div>
                    <label>
                        Editable ACH</label>
                    <%: Html.CheckBoxFor(contactType => contactType.EditableACH)%>
                    <%: Html.ValidationMessageFor(contactType => contactType.EditableACH)%>
                </div>
            </div>
            <div class="bottomLine">
                <div>
                    <label>
                        Viewable ICH</label>
                    <%: Html.CheckBoxFor(contactType => contactType.ViewableICH)%>
                    <%: Html.ValidationMessageFor(contactType => contactType.ViewableICH)%>
                </div>
                 <div>
                    <label>
                        Editable ICH</label>
                    <%: Html.CheckBoxFor(contactType => contactType.EditableICH)%>
                    <%: Html.ValidationMessageFor(contactType => contactType.EditableICH)%>
                </div>
            </div>
            <div>
                <div>
                    <label>
                        Active</label>
                    <%: Html.CheckBoxFor(contactType => contactType.IsActive, new { @Checked="checked"})%>
                    <%: Html.ValidationMessageFor(contactType => contactType.IsActive)%>
                </div>
            </div>
        </div>
    </fieldset>
    <div class="buttonContainer">
        <input type="submit" value="Save" class="primaryButton" />
        <%: Html.LinkButton("Back", Url.Action("Index", "ManageContacts"))%>
    </div>
  <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script src="<%:Url.Content("~/Scripts/Masters/ContactsValidate.js")%>" type="text/javascript"></script>
</asp:Content>

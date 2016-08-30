<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.Member>" %>
<div class="solidBox ">
    <div class="fieldContainer horizontalFlow">
        <div>
            Member Name:
            <%: Html.TextBoxFor(m => m.CommercialName, new { @class = "autocComplete largeTextField" })%>
            <%: Html.HiddenFor(m => m.Id) %>
        </div>
    </div>
    <div class="clear">
    </div>
</div>

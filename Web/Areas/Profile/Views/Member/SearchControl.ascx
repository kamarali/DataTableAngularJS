<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.Member>" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile.Enums" %>

<%
  if (SessionUtil.UserCategory != UserCategory.Member)
  {%>
<%
    using (Html.BeginForm("FetchMemberDetails", "Member", FormMethod.Post, new { id = "FetchMember" }))
    {%>
    <%: Html.AntiForgeryToken() %>
<div>
  Member Name:
  <%:Html.TextBoxFor(m => m.DisplayCommercialName, new { @class = "autocComplete largeTextField" })%>
  <%:Html.HiddenFor(m => m.Id)%>
  <%:Html.Hidden("selectedMemberId", Model.Id)%>
  <input class="primaryButton" type="submit" id="searchButton" value="Search" onclick="GetMembervalidation()" />
</div><br />
<%
    }
  }%>

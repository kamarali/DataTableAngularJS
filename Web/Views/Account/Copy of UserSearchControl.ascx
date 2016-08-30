<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SIS.Web.UIModels.Account.CreateUserView>" %>
<% using (Html.BeginForm())
   { %>
<fieldset class="solidBox dataEntry">
<legend>Search Criteria</legend>
    <div class="fieldContainer horizontalFlow">
<div class="bottomLine">

<label>First Name:<br /></label><%: Html.TextBox("FirstName") %><br />
<label>Last Name:<br /></label><%: Html.TextBox("LastName") %><br />
<label>Email Address:<br /></label><%: Html.TextBox("EmailAddress") %><br />
<label>Status:<br /></label><%: Html.TextBox("Status")%><br />
<input type="submit" value="Search" />
</div>

<div class="">
    <div class="halfWidthColumn">
        <div>
            <div>
                <div>
                    <div style="width: 750px;" dir="ltr" id="gbox_MemberNotificationContactListGrid"
                        class="ui-jqgrid ui-widget ui-widget-content ui-corner-all">
                        <div style="width: 750px;" id="SearchResults" >
                            <div class="ui-jqgrid-titlebar ui-widget-header ui-corner-top ui-helper-clearfix">
                                <span class="ui-jqgrid-title">Search Results</span>
                            </div>
                            <div class="ui-state-default ui-jqgrid-hdiv" style="width: 750px; display: block;">
                                <div class="ui-jqgrid-hbox">
                                     <table  style="width:750px;"></table>
                                </div>
                            </div>
                            <div style="height: 150px; width: 750px; display: block;" class="ui-jqgrid-bdiv">
                                <div style="position: relative; height: 154px; width: 750px;">
                                    <div style="height: 0px;">
                                    <%@ Import Namespace="SIS.Web.UIModels.Account"%>
                                    <% List<TableRowForHelper> SearchItemsReturnList = (List<TableRowForHelper>)ViewData["SearchResultsForDataStructure"]; %>
                                    <% if (SearchItemsReturnList != null && SearchItemsReturnList.Count() > 0) %>
                                    <% {%>
                                    <%: Html.Table("SearchResultsHtml", SearchItemsReturnList)%>
                                    <% }%> 
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
</div>
</fieldset>
<% } %>

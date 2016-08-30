<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile.Enums" %>
<table width="100%" cellpadding="0" cellspacing="0">
    <tr>
        <td align="left">
            <h1>
                SIS <span style="padding-left: 23px;">Simplified Invoicing and Settlement</span></h1>
        </td>
        <td align="right">
            <div id="userInfo">
                <ul class="quickLinks">
                    <% 
					if (Request.IsAuthenticated)
					{
					%>
						<li class="greetings">Welcome
							<%: Html.ActionLink(SessionUtil.Username ?? " ", "UserModify", "Account", new { area = "" }, new { style = "background: none; padding: 0px; text-decoration: underline;" })%></li>
						<%--<li> <a href="~/Home/Index" id="Alerlink"></a></li>--%>
                        <%-- <li> <a href="/Home/Index" id="messagelink"></a> </li>--%>
						<li>
							<%:Html.ActionLink("Alerts", "Index", "Home", new { area = ""},new{id = "Alerlink" })%></li>
						<li>
							<%:Html.ActionLink("Messages", "Index", "Home", new { area = "" }, new { id = "messagelink" })%></li>
						<li>
							<%:Html.ActionLink("Home", "Index", "Home", new { area = ""  }, null)%></li>
						<li>
                            <%--SCP291479: IS-WEB-Contact Help Desk title update--%>
							<%: Html.ActionLink("Contact IS Help Desk", "RedirectToIataHelpDeskForm", "Account", null, new { target = "_blank" })%>
						</li>
						<li style="text-decoration: none;">
                        <%
                            if ((SessionUtil.UserCategory != UserCategory.SisOps) && (SessionUtil.UserCategory != UserCategory.AchOps) && (SessionUtil.UserCategory != UserCategory.IchOps))
                            {%>
								<%:
								Html.ActionLink("Help", "RedirectToHelpContent", "Home", new {area = "", requestUrl = Request.Path},
								new {target = "_blank", id = "helplink", @class = "ignoredirty"})%>
                          <%}%> </li>
							<li style="padding-top: 0px;"><ul style="padding:0px">
							
                            <%if (SessionUtil.IsLogOutProxyOption)
							  {
								  using (Html.BeginForm("LogOffProxy", "Account", FormMethod.Post, new { id = "frmLogOffProxy", style = "display:inline" }))
								  { 
									%>
									<li><a href="#" onclick="JavaScript:SubmitProxyLogOffForm($(this).parent().parent());">
										Log Off Proxy</a></li>
									<%
								  }
							  }
							  else
							  {
									using (Html.BeginForm("LogOff", "Account", FormMethod.Post, new { id = "frmLogOff" }))
									  { 
										%>
										<li><a href="#" onclick="JavaScript:SubmitLogOffForm();">Log Off</a></li>
										<%}

                              } %></ul></li>
                  <%}%>
                    
                </ul>
            </div>
        </td>
    </tr>
</table>
<script type="text/javascript">
    function HelplinkIntegration(requestUrl) {
        alert(requestUrl);
        window.open('/Home/RedirectToHelpContent?requestUrl=' + requestUrl, 'Help', '', '_new');
    }

    $(document).ready(function () {
        GetAlertMessageCountss();
    });

    function GetAlertMessageCountss() {

        $.post('<%: Url.Action("GetAlertMessagesCount","Home") %>', function (data) {

            $('#Alerlink').text('Alerts (' + data.Alert + ')');
            $('#messagelink').text('Messages (' + data.Message + ')');
        });
    }

//    SCP264718 : SIS validation for "Your Rejection Memo Number" 
//    Description: Removing JavaScript internal function setInterval() call, which is used to trigger 
//    call to CreateSessionForAlerts() function on regular time interval (in Mili seconds.). 
//    setInterval(function () {
//        CreateSessionForAlerts();        
//    }, 300000
//     );

    function CreateSessionForAlerts() {
        $(function () {            
            $.post('/Home/SetSessionToManageAlerts', { key: 'CallingMethod', value: 'AutoAlert' }, function (data) {
                GetAlertMessageCountss();
            });
        });
    }

</script>

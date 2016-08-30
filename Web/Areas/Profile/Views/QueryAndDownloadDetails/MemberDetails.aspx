<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Reports :: Member and Contact Report
</asp:Content>
<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="Script">
  <script src="<%:Url.Content("~/Scripts/Member/QueryAndDownloadDetails.js")%>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Member and Contact Report</h1>
  <h2>Query and Download</h2>
  <br />
  <% using (Html.BeginForm("DownloadReport", "QueryAndDownloadDetails", FormMethod.Post, new { id = "formSearch" }))
     {%>
     <%: Html.AntiForgeryToken() %>
  <div>
    <%Html.RenderPartial("MemberDetailsControl"); %>
  </div>
  <div>
    <%Html.RenderPartial("DisplayOptionsControl");%>
  </div>
  <div>
    <%Html.RenderPartial("SearchResultsGrid");%>
  </div>
  <script type="text/javascript">

    //courtesy of BoogieJack.com
    function killCopy(e) {
      return false
    }
    function reEnable() {
      return true
  }
  //SCP122028 : Member Contact Information ( FIX for Mozilla - to make dropdowns work)
//    document.onselectstart = new Function("return false")
//    if (window.sidebar) {
//      document.onmousedown = killCopy
//      document.onclick = reEnable
//    }


    var $Resultdialog;
    var $AddressLabelResultdialog;

    $(document).ready(function () {

        var firstClick = true;

        //Model popup to display search result.
        $Resultdialog = $('<div></div>')
    		.html($("#divSearchResult"))
    		.dialog({
    		    autoOpen: false,
    		    title: 'Member Information',
    		    height: 400,
    		    width: 600,
    		    modal: true,
    		    resizable: true
    		});

        //Model popup to display search result.
        $AddressLabelResultdialog = $('<div></div>')
    		.html($("#addressLabel"))
    		.dialog({
    		    autoOpen: false,
    		    title: 'Contact Information',
    		    height: 550,
    		    width: 500,
    		    modal: true,
    		    resizable: true
    		});

        // onselectstart="return false;" onclick="return false;" onmousedown="return false;" onfocus="return false;"
        // onselectstart="return false;" onclick="return false;" onmousedown="return false;" onfocus="return false;"
        //    		$("div[role='dialog']").click(function () {
        //    		  return false;
        //    		});
        //    		$("div[role='dialog']").focus(function () {
        //    		  return false;
        //    		});
        //    		$("div[role='dialog']").mousedown(function () {
        //    		  return false;
        //    		});
        //    		$("div[role='dialog']").select(function () {
        //    		  return false;
        //    		});
        // SCP186215: Member Code Mismatch between Member and Location Details
        // CMP597: added parameter to know the call from MemberContact report.
        // If call is from Member Contact report then INCLUDE_MEMBER_TYPE parameter is set to 9 in
        // method GetAllMemberList to exclude Members having ‘IS Membership Sub Status’ as “Terminated” (irrespective of ‘IS Membership Status’)
        registerAutocomplete('CommercialName', 'Id', '<%:Url.Action("GetAllMemberList", "Data", new { area = "", userCategoryId = ViewData["UserCategoryID"], isFromMemContactRprt = "true" })%>', 0, true, onMemberNameChange, null, null, null, onBlankMemberName);

        //Perform search on search click.
        $('#btnDownloadReport').click(function () {
            var searchCriteria = PopulateSearchCriteria();
            if (searchCriteria) {

                if ($("#rdContactDetails").prop('checked') && $("#addressLabelFormat").prop('checked')) {
                    downloadReport();
                }
                else {
                    if (!firstClick) {
                        //Reload jqGrid if not the first search. 
                        $.jgrid.gridUnload('#list');
                        $("#list").trigger("reloadGrid");
                    }
                    firstClick = false;
                    $('#addressLabelFormat').html("");
                    searchMemberAndContactDetails('<%: Url.Action("Search", "QueryAndDownloadDetails", new { area = "Profile"}) %>', searchCriteria);
                }
            }
        });
    });

  </script>
  <%
    }%>
</asp:Content>
